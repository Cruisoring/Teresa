using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using OpenQA.Selenium;

namespace Teresa
{
    public class SelectLocator : Locator
    {
        public const string OPTION = "option";
        public const string MULTIPLE = "multiple";
        public const string INDEX = "index";

        /// <summary>
        /// Gets a valueKey indicating whether the parent element supports multiple selections.
        /// </summary>
        public bool IsMultiple { get; private set; }

        public ReadOnlyCollection<IWebElement> Options { get; private set; }

        public IWebElement Selected
        {
            get { return Options.FirstOrDefault(x => x.Selected); }
        }

        public List<IWebElement> AllSelected
        {
            get { return Options.Where(x => x.Selected).ToList(); }
        }

        /// <summary>
        /// The overrided FindElement() would parse the result to see if it has "multiple" attribute and stores the options
        /// </summary>
        /// <param name="filters"></param>
        /// <param name="waitInMills"></param>
        /// <returns></returns>
        public override IWebElement FindElement(Func<IWebElement, bool> filters = null, int waitInMills = DefaultWaitToFindElement)
        {
            IWebElement lastFound = lastFoundElement;
            IWebElement result = base.FindElement(filters, waitInMills);
            if (result == null)
                throw new NoSuchElementException();

            if (result != lastFound)
            {
                bool isMultiple = false;
                string valueOfMultipleAttribute = result.GetAttribute(MULTIPLE);
                bool.TryParse(valueOfMultipleAttribute, out isMultiple);
                IsMultiple = isMultiple;

                Options = result.FindElementsByCss(OPTION);
            }

            return result;
        }

        /// <summary>
        /// Entry point to perform operations on the IWebElement selected with the Identifier and filters.
        /// </summary>
        /// <param name="extraInfo">
        /// For Getter(): specify the kind of information to be retrieved from the Select element.
        ///     If it is null, "" or "textKey": return the textKey of the first selected option, or the textKey of the whole select.
        ///     If it is "index" or "#": then either the "index" attribute valueKey of the Selected option (if it is defined) or
        ///         or the sequence number (from 0) of the Selected option in the Options are returned as a string.
        ///     If it is "valueKey" or "$": the valueKey of the "valueKey" attribute is returned if it is defined, or null if undefined.
        ///     If it is "isMultiple": then "true" or "false" is returned to show if the Select element enable multiple select
        ///     If it is "AllSelected": all selected items are listed with their textKey
        /// For Setter(): defines which option is to be affected.
        ///     
        /// </param>
        /// <param name="filters"></param>
        /// <returns></returns>
        public override string this[string extraInfo, Func<IWebElement, bool> filters = null]
        {
            get
            {
                IWebElement element = FindElement(filters);
                string resultString;
                if (string.IsNullOrEmpty(extraInfo))
                {
                    resultString = Selected==null ? FindElement().Text : Selected.Text;
                    Console.WriteLine("\"{0}\"=[{1}];", resultString, Identifier.FullName());
                    return resultString;
                }

                switch (extraInfo.ToLowerInvariant())
                {
                    case Operation.TextOf:
                        resultString = Selected == null ? FindElement().Text : Selected.Text;
                        break;
                    case Operation.IndexOf:
                    case Operation.IndexSign:
                        if (Selected == null)
                            resultString = "-1";
                        else
                        {
                            resultString = Selected.GetAttribute(INDEX);
                            resultString = resultString == null ? Options.IndexOf(Selected).ToString() : resultString;
                        }
                        break;
                    case Operation.ValueOf:
                    case Operation.ValueSign:
                        resultString = Selected==null ? null : Selected.GetAttribute("value");
                        break;
                    case Operation.IsMultiple:
                        resultString = IsMultiple.ToString();
                        break;
                    case Operation.AllOptions:
                        var optionTexts = Options.Select(e => e.Text);
                        resultString = string.Join(", ", optionTexts);
                        break;
                    case Operation.AllSelected:
                    {
                        var allSelectedText = AllSelected.Select(e => e.Text);
                        resultString = string.Join(", ", allSelectedText);
                        break;
                    }
                    default:
                        return base[extraInfo, filters];
                }
                Console.WriteLine("\"{0}\"=[{1}, \"{2}\"];", resultString, Identifier.FullName(), extraInfo);
                return resultString;
            }
            set
            {
                IWebElement element = FindElement(filters);
                string prefix = string.IsNullOrEmpty(extraInfo) ? null :
                    ReservedPrefixes.FirstOrDefault(s => extraInfo.StartsWith(s, StringComparison.InvariantCultureIgnoreCase));

                if (prefix == null)
                {
                    //By default, when "extraInfo" is not specified, SendKeys(valueKey+Tab) to let browser click the target option
                    //Notice: this click would happen no matter if the valueKey is "true" or "false", and it doesn't select multiple options.
                    string keysToBeSend = extraInfo ?? value;
                    //WARNING: Sometime, even when it works in debugging, SendKeys() to multiple 
                    // select may fail to select the option
                    foreach (char ch in keysToBeSend)
                    {
                        element.SendKeys(ch.ToString());
                        Thread.Sleep(100);
                    }
                    element.SendKeys(Keys.Tab);
                    Thread.Sleep(500);
                    Console.WriteLine("[{0}]=\"{1}\";", Identifier.FullName(), keysToBeSend);
                    return;
                }
                string key = extraInfo.Substring(prefix.Length);

                //The valueKey string shall be "True" or "False" to indicate the concerned option shall be selected or deselected
                bool toSelect = false;
                if (!bool.TryParse(value, out toSelect))
                {     //Do nothing if the valueKey is not a boolean valueKey
                    Console.WriteLine("Failed to parse command associated with '{0}'.", value);
                    return;
                }

                if (prefix.EndsWith(KEY_PREFIX))
                    prefix = prefix.Substring(0, prefix.Length - KEY_PREFIX.Length);

                switch (prefix)
                {
                    case Operation.TextOf:
                        SelectByText(key, toSelect);
                        break;
                    case Operation.IndexOf:
                    case Operation.IndexSign:
                        SelectByIndex(key, toSelect);
                        break;
                    case Operation.ValueOf:
                    case Operation.ValueSign:
                        SelectByValue(key, toSelect);
                        break;
                    case Operation.AllOptions:
                        SelectAll(toSelect);
                        break;
                }
                Console.WriteLine("[{0}, \"{1}\"]=\"{2}\";", Identifier.FullName(), extraInfo, value);
            }
        }

        /// <summary>
        /// Select the first option whose text is textKey or contains textKey.
        /// </summary>
        /// <param name="textKey">The text of the option to be selected. If an exact match is not found,
        /// this method will perform a substring match.</param>
        /// <param name="toSelect">indicate weather select or de-select the matched option: 
        ///     "true" to select, "false" to de-select.</param>
        /// <exception cref="NoSuchElementException">Thrown if there is no element with the given textKey present.</exception>
        public void SelectByText(string textKey, bool toSelect = true)
        {
            if (textKey == null)
                throw new ArgumentNullException("textKey", "textKey must not be null");

            IWebElement targetOption = Options.FirstOrDefault(e => e.Text == textKey);

            if (targetOption != null)
            {
                if (targetOption.Selected != toSelect)
                    targetOption.Click();
                return;
            }

            //Since no option with the exactly matched text, try to find the first option whose text contains textKey
            targetOption = Options.FirstOrDefault(x => x.Text.Contains(textKey));
            if (targetOption == null)
                throw new NoSuchElementException("Cannot locate element with textKey: " + textKey);

            if (targetOption.Selected != toSelect)
                targetOption.Click();
        }

        /// <summary>
        /// Select the option with the indexString, first try to match it with the "index" attribute of the options,
        /// then if it doesn't work, try to match with option with its order in all Option elements.
        /// <remarks>
        /// Although 'index' is widely used as an attribute of the option tags, it is not defined officially, thus it can be any value other than number.
        /// <see cref="http://www.w3schools.com/tags/tag_option.asp"/>
        /// </remarks>
        /// </summary>
        /// <param name="indexKey">The valueKey of the index attribute of the option to be selected, or a number in string form.</param>
        /// <param name="toSelect">indicate weather select or de-select the matched option: 
        ///     "true" to select, "false" to de-select.</param>
        /// <exception cref="NoSuchElementException">Thrown if there is no option matched with the given indexKey.</exception>
        public void SelectByIndex(string indexKey, bool toSelect = true)
        {
            if (indexKey == null)
                throw new ArgumentNullException("indexKey", "indexKey must not be null");

            //Try to match the option with attribute of "index" and its valueKey is matched without assumption the indexKey is a number
            IWebElement targetOption = Options.FirstOrDefault(x => x.GetAttribute("index") == indexKey);
            if (targetOption != null)
            {
                if (targetOption.Selected != toSelect)
                    targetOption.Click();
                return;
            }

            //When options don't have "index" attribute, then their order (index of first one is '0') is used to get the right one
            int index = -1;
            if (!int.TryParse(indexKey, out index))
                throw new NoSuchElementException("Failed to locate option with index of " + indexKey);

            if (index < 0 || index >= Options.Count)
                throw new IndexOutOfRangeException("Index is out of range: '{0}'." + indexKey);

            targetOption = Options[index];
            if (targetOption.Selected != toSelect)
                targetOption.Click();
        }

        /// <summary>
        /// Select an option by the valueKey.
        /// </summary>
        /// <param name="valueKey">The valueKey of the option to be selected.</param>
        /// <param name="toSelect">indicate weather select or de-select the matched option: 
        ///     "true" to select, "false" to de-select.</param>
        /// <exception cref="NoSuchElementException">Thrown when no element with the specified valueKey is found.</exception>
        public void SelectByValue(string valueKey, bool toSelect = true)
        {
            IWebElement targetOption = Options.FirstOrDefault(o => o.GetAttribute("value") == valueKey);
            if (targetOption == null)
                throw new NoSuchElementException("Cannot locate option with value: " + valueKey);
            
            if(targetOption.Selected != toSelect)
                targetOption.Click();
        }

        /// <summary>
        /// Select/De-select all options. Notice: Works only for select with "multiple" attribute defined. 
        /// </summary>
        /// <param name="toSelect">indicate weather select or de-select the matched option: 
        ///     "true" to select, "false" to de-select.</param>
        public void SelectAll(bool toSelect = true)
        {
            if (!IsMultiple)
                throw new InvalidOperationException("Cannot select or de-select all options when IsMultiple=" + IsMultiple);

            foreach (var opt in Options)
            {
                if (opt.Selected != toSelect)
                    DoClick(opt);
            }
        }

        /// <summary>
        /// Perform clicking on the option item.
        /// </summary>
        /// <param name="concernedOption">The option element to be clicked.</param>
        private void DoClick(IWebElement concernedOption)
        {
            //if (IsMultiple)
            //    concernedOption.ControlClickScript();
            //else 
                concernedOption.Click();
        }

        public SelectLocator(Enum identifier, Fragment parent)
            : base(identifier, parent)
        {}
    }
}
