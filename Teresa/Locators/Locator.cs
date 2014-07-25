using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using OpenQA.Selenium;
using OpenQA.Selenium.Remote;


namespace Teresa
{
    /// <summary>
    /// Locator keeps the means to find the right IWebElement and performs most common operations on it.
    /// </summary>
    public class Locator
    {
        /// <summary>
        /// Defines the root node of a HTML file, that would be used as the default fragment if there is no one defined.
        /// </summary>
        public enum HtmlByCustom
        {
            [EnumMember("", true)]
            Root
        }

        #region Useful strings for effective operations on FindElement

        public const int DefaultWaitToFindElement = 2000;
        public const int ImmediateWaitToFindElement = 100;

        //The valid period of the IWebElement returned by calling FindElement().
        //Consecutive callings to FindElement() with the same argument within this period would return the same
        //IWebElement cached as lastFoundElement.
        public const int ValidElementStatePeriodInTicks = 1000;
        
        public const string KEY_PREFIX = "=";
        public const string VALUE_LOCATOR = ":";

        public static List<string> ReservedPrefixes = new List<string>()
        {
            Operation.TextOf+KEY_PREFIX, Operation.ValueOf+KEY_PREFIX, Operation.IndexOf + KEY_PREFIX,
            Operation.ValueSign, Operation.IndexSign, Operation.AllOptions
        };

        public static bool LastTrySuccess { get; protected set; }
        #endregion

        #region Static functions
        /// <summary>
        /// Factory of Locators based on the type of the IWebElement denoted by Enum id.
        /// </summary>
        /// <param name="id">The Enum identifier containing CSS of the target IWebElement.</param>
        /// <param name="parent">Locator of the parent IWebElement of the target IWebElement.</param>
        /// <returns>Locator with behaviore matched with the type of the element.</returns>
        public static Locator LocatorOf(Enum id, Fragment parent)
        {
            EnumMemberAttribute usage = EnumMemberAttribute.EnumMemberAttributeOf(id);
            switch (usage.TagName)
            {
                case HtmlTagName.Table:
                    return new TableLocator(id, parent);
                case HtmlTagName.Text:
                    return new TextLocator(id, parent);
                case HtmlTagName.Radio:
                    return new RadioLocator(id, parent);
                case HtmlTagName.Select:
                    return new SelectLocator(id, parent);
                case HtmlTagName.Checkbox:
                case HtmlTagName.Check:
                    return new CheckboxLocator(id, parent);
                default:
                    return new Locator(id, parent);
            }
        }
        #endregion

        #region Properties and Fields
        //Locator of the parent IWebElement
        public Fragment Parent { get; protected set; }

        //Enum identify to determine CSS selector and some other features related with the IWebElement.
        public Enum Identifier { get; protected set; }

        //Stores the IWebElement that is found by calling FindElement() last time
        protected IWebElement lastFoundElement = null;

        /// <summary>
        /// Used only to select one IWebElement from a collection returned by FindElementsByCssSelector(css).
        /// This stores the filters used when calling FindElement(Fun<IWebElement, bool>).
        /// </summary>
        private Func<IWebElement, bool> lastFilters = null;

        //Stores the last calling of FindElement() to avoid InvalidElementStateException or StaleElementReferenceException.
        //private long lastTick = Int64.MinValue;
        #endregion

        /// <summary>
        /// Retrieve the IWebElement based on the CSS associated with the Enum Identifier, and optional
        /// the filters when Enum Identifier selects a collection.
        /// </summary>
        /// <param name="filters">
        /// ByIndex to select one IWebElement from a collection of them, would be used by this Locator and its parents.
        ///     For Locator of a single IWebElement whose IsCollection=="false", filters is always neglected.
        ///     For Locator of multiple IWebElements whose IsCollection=="true", 
        ///         filters is used to choose from a Collection returned by FindElementsByCssSelector().
        /// Noticeably, the filters is sticky. That is, if one Locator uses a filters, then it and its parent would store
        /// it for following calling of FindElement(null).
        /// The defintion of filters is similar to Queryable.Where<TSource> Method (IQueryable<TSource>, Expression<Func<TSource, Int32, Boolean>>)
        /// <seealso cref="http://msdn.microsoft.com/en-us/library/vstudio/bb548547(v=vs.100).aspx"/>
        /// </param>
        /// <returns>
        /// The IWebElement chosen by the CSS selector and the filters, and casted to IWebElement.
        /// </returns>
        public virtual IWebElement FindElement(Func<IWebElement, bool> filters = null, int waitInMills = DefaultWaitToFindElement)
        {
            //TODO: if it is necessary to validate the lastSelectedELement or regard it as valid within a limited time?
            //*/ Perform some simple operation to validate that the lastFoundElement is still valid
            if (lastFoundElement != null && (filters == null || filters == lastFilters)
                && lastFoundElement.IsValid())
                return lastFoundElement;
            /*/
            //Otherwise, using the Environment.Ticks to make sure each Element is valid for a limited time (1 second)
            if (lastFoundElement != null && (filters == null || filters == lastFilters)
                //&& (Environment.TickCount-lastTick<ValidElementStatePeriodInTicks)
                )
                return lastFoundElement;
            //*/

            IWebElement parentElement = null;
            if (Parent != null && ! Parent.Identifier.Equals(HtmlByCustom.Root))
            {
                parentElement = Parent.FindElement(filters, waitInMills);
                if (parentElement == null || !parentElement.IsValid())
                {
                    return null;
                }
            }

            string css = Identifier.Css();

            //When CSS matches multiple IWebElements, further selection shall be applied by filters
            //Or, When selection is based on ByText mechanism, then EnumMember.Value is used
            if (Identifier.IsCollection() || Identifier.Mechanism().Equals(Mechanisms.ByText))
            {
                Func<string, ReadOnlyCollection<IWebElement>> findFunc = (parentElement == null) ? 
                    (Func<string, ReadOnlyCollection<IWebElement>>)DriverManager.FindElementsByCssSelector : parentElement.FindElementsByCss;

                //Select the candidate element by calling FindElementsByCssSelector(css) of either the parentElement or the Driver
                var candidates = WaitToFindElements(findFunc, css, waitInMills);

        #if(STEP_BY_STEP)
                Console.WriteLine("There are {0} candidates selected by '{1}'", candidates.Count, css);
        #endif

                if (Identifier.IsCollection())
                {
                    //Store the filters if it is not null, otherwise use the last filters
                    lastFilters = filters ?? lastFilters;

                    if (lastFilters == null)
                        throw new NullReferenceException();

                    var filtered = candidates.Where(lastFilters);

                    lastFoundElement = filtered.FirstOrDefault();
                }
                else
                {
                    lastFoundElement = candidates.FirstOrDefault(item => item.Text.Equals(Identifier.Value()));
                }
            }
            //When CSS select one IWebElement, then filter is ignored
            //Notice the GenericWait.Until() would keeps running until timeout or no Exception is thrown
            else
            {
                Func<string, IWebElement> find = (parentElement == null)
                    ? (Func<string, IWebElement>)DriverManager.FindElementByCssSelector
                    : parentElement.FindElementByCss;

                lastFoundElement = GenericWait<IWebElement>.TryUntil( () => find(css),
                    x => x != null & x.IsValid(), ImmediateWaitToFindElement );
            }

            //if(lastFoundElement == null)
            //    throw new NoSuchElementException("Failed to find Element by CSS: " + css);

            //Keeps the moment of this opertion
            //lastTick = Environment.TickCount;
            return lastFoundElement;
        }

        public IWebElement TryFindElement(Func<IWebElement, bool> filters = null, int waitInMills = DefaultWaitToFindElement)
        {
            Func<string, ReadOnlyCollection<IWebElement>> findFunc = null;
            if (Parent != null && !Parent.Identifier.Equals(HtmlByCustom.Root))
            {
                IWebElement parentElement = Parent.TryFindElement(filters, waitInMills);

                if (parentElement == null)
                    return null;

                findFunc = parentElement.FindElementsByCss;
            }
            else
            {
                findFunc = DriverManager.FindElementsByCssSelector;
            }

            var candidates = WaitToFindElements(findFunc, Identifier.Css(), waitInMills);

            if (candidates.Count == 0)
                return null;

            if (Identifier.Mechanism() == Mechanisms.ByText)
            {
                return candidates.FirstOrDefault(item => item.Text.Equals(Identifier.Value()));
            }
            else if (!Identifier.IsCollection())
            { 
                if (candidates.Count != 1)
                    throw new Exception("There are " + candidates.Count + " matches of " + this);
                var result = candidates.First();

                if (result == null || !result.Displayed)
                    return null;

                return result;
            }
            else //The Identifier matches with a collection of IWebElement
            {
                var filtered = candidates.Where(filters);
                return filtered.FirstOrDefault();
            }
        }

        /// <summary>
        /// Assuming Elements can be found with CSS selector within time specified by waitInMills, keep executing the
        /// FindElementsByCssSelector() of either Driver or a specific IWebElement until timeout.
        /// </summary>
        /// <param name="findFunc">The FindElementsByCssSelector() of either Driver or a specific IWebElement.</param>
        /// <param name="css">CSS Selector string.</param>
        /// <param name="waitInMills">Maximum time to wait when the findFunc returns 0-length collection.</param>
        /// <returns>The non-empty collection immediately or empty collection after time is out.</returns>
        public ReadOnlyCollection<IWebElement> WaitToFindElements(Func<string, ReadOnlyCollection<IWebElement>> findFunc, 
            string css,
            int waitInMills)
        {
            return GenericWait<ReadOnlyCollection<IWebElement>>.Until(
                () =>findFunc(css), collction => collction.Count != 0, waitInMills
                );
        }

        public override string ToString()
        {
            return (Parent == null) ?
                String.Format("{0}({1})", Identifier.TypeAndName(), Identifier.Css()) :
                String.Format("{0}({1})->{2}({3})", Parent.Identifier.TypeAndName(), Parent.Identifier.Css(),
                    Identifier.TypeAndName(), Identifier.Css());
        }

        /// <summary>
        /// Default methods to extract information or perform operation on the 
        /// concerned IWebElement selected by calling FindElement().
        /// For get(): it would return the text of the IWebElement.
        ///     Otherwise, it might return tagname, selected, enabled, displayed, 
        ///     location and size information as a string.
        /// For set(): based on the value, operations like "click", "hover", "doubleclick" and "submit" would be performed
        ///     on the specific IWebElement.
        /// </summary>
        /// <param name="extraInfo">
        /// For get():
        ///     Description of the kind of information expected.
        /// For set():
        ///     Not used yet, but can be extended to generate filters used by the FindElement(filters).
        /// </param>
        /// <param name="filters">
        /// ByIndex to select one IWebElement from a collection of them, would be used by this Locator and its parents.
        /// </param>
        /// <returns>
        /// For get():
        ///     return the requested value in string format.
        /// </returns>
        public virtual string this[string extraInfo, Func<IWebElement, bool> filters = null]
        {
            get
            {
                IWebElement element = FindElement(filters);

                if (element == null)
                    throw new NoSuchElementException("Failed to find element with " + Identifier.Description());

                string resultString;
                if (string.IsNullOrEmpty(extraInfo))
                {
                    resultString = element.Text;
                    Console.WriteLine("\"{0}\"=[{1}];", resultString, Identifier.FullName());
                    return resultString;
                }

                switch (extraInfo.ToLower())
                {
                    case Operation.TextOf:
                        resultString = element.Text;
                        break;
                    case Operation.TagName:
                        resultString = element.TagName;
                        break;
                    case Operation.Selected:
                        resultString = element.Selected.ToString();
                        break;
                    case Operation.Enabled:
                        resultString = element.Enabled.ToString();
                        break;
                    case Operation.Displayed:
                        resultString = element.Displayed.ToString();
                        break;
                    case Operation.Location:
                        resultString = element.Location.ToString();
                        break;
                    case Operation.Size:
                        resultString = element.Size.ToString();
                        break;
                    case Operation.Css:
                        resultString = Identifier.Css();
                        break;
                    case Operation.ParentCss:
                        resultString = Parent==null ? "" : Parent.Identifier.Css();
                        break;
                    case Operation.FullCss:
                        resultString = Identifier.FullCss();
                        break;
                    default:
                        resultString = element.GetAttribute(extraInfo);
                        break;
                }
                Console.WriteLine("\"{0}\"=[{1}, \"{2}\"];", resultString, Identifier.FullName(), extraInfo);
                return resultString;
            }
            set
            {
                if (value == null)
                    throw new ArgumentNullException();

                IWebElement element = FindElement(filters);

                if (string.Compare(extraInfo, Operation.SendKeys, StringComparison.InvariantCultureIgnoreCase)==0)
                {
                    if (element == null || !element.IsValid())
                        throw new NoSuchElementException("Failed to find element with " + Identifier.FullName());

                    element.SendKeys(value);
                    Console.WriteLine("[{0}, \"{1}\"]=\"{2}\";", Identifier.FullName(), extraInfo, value);
                    return;
                }

                string action = value.ToLowerInvariant();
                bool isTry = action.StartsWith(Operation.Try);

                if (isTry)
                {
                    LastTrySuccess = false;
                    if (element == null || !element.IsValid())
                        return;
                    action = action.Substring(Operation.TryLength);
                }

                if (element == null || !element.IsValid())
                    throw new NoSuchElementException("Failed to find element with " + Identifier.FullName());

                switch (action)
                {
                    case Operation.True:
                    case Operation.Click:
                        element.Click();
                        break;
                    case Operation.Submit:
                        element.Submit();
                        break;
                    case Operation.ClickScript:
                        element.ClickByScript();
                        break;
                    case Operation.Hover:
                        element.Hover();
                        break;
                    case Operation.ControlClick:
                        element.ControlClick();
                        break;
                    case Operation.ShiftClick:
                        element.ShiftClick();
                        break;
                    case Operation.DoubleClick:
                        element.DoubleClick();
                        break;
                    case Operation.Show:
                        element.Show();
                        break;
                    case Operation.HighLight:
                        element.HighLight();
                        break;
                    default:
                        Console.WriteLine("Failed to parse command associated with '{0}'.", value);
                        return;
                }

                if (isTry)
                    LastTrySuccess = true;

                Console.WriteLine("[{0}]=\"{1}\";", Identifier.FullName(), value);
            }
        }

        public bool TryExecute(string action, string extraInfo = null, Func<IWebElement, bool> filters = null)
        {
            if (action == null)
                return false;

            //Try to find the IWebElement with FindElementsByCssSelector
            IWebElement element = TryFindElement(filters);

            if (element != null)
            {
                this[extraInfo, filters] = action;
                return true;
            }

            return false;
        }

        protected Locator(Enum identifier, Fragment parent = null)
        {
            Identifier = identifier;
            Parent = parent;
        }

        protected Locator(){}
    }
}
