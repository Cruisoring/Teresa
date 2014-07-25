#undef STEP_BY_STEP

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.Configuration;
using OpenQA.Selenium;
using OpenQA.Selenium.Interactions;
using OpenQA.Selenium.Interactions.Internal;
using OpenQA.Selenium.Remote;
using Teresa.Utility;

namespace Teresa
{
    public static class IWebElementExtension
    {
        public static bool IsValid(this IWebElement element)
        {
            try
            {
                return element.Displayed;
            }
            catch (Exception)
            {
                return false;
            }
        }

        /// <summary>
        /// This method evaluates if one IWebElement contains a child selected by the predicate. It can be used to select
        /// one container element based on its child elements.
        /// </summary>
        /// <param name="element">The container element to be evaludated.</param>
        /// <param name="childEnum">Enum Identifier of the child element, which carries the CSS selector.</param>
        /// <param name="predicate">Function to see if the child element meets predefined criteria.</param>
        /// <returns>"true": The container IWebElement contains the expected child IWebElement, otherwise "false".</returns>
        public static bool HasChildOf(this IWebElement element, Enum childEnum, Func<IWebElement, bool> predicate)
        {
            string childLocatorCss = childEnum.Css();
            IWebElement child = null;
            try
            {
                //child = GenericWait<IWebElement>.Until(() => 
                //    element.FindElementByCss(childLocatorCss), null, 1000);
                ReadOnlyCollection<IWebElement> candidates = null;
                candidates = GenericWait<ReadOnlyCollection<IWebElement>>.Until(
                    () => candidates = element.FindElementsByCss(childLocatorCss),
                    x => x.Count  != 0,
                    200
                    );
                var qualified = candidates.Where(predicate).ToList();
                bool result = qualified.Count != 0;
                return result;
            }
            catch (Exception)
            {
                return false;
            }
        }

        /// <summary>
        /// This method evaluates if a specific child IWebElement identified by childEnum contains partialText.
        /// </summary>
        /// <param name="element">The container element to be evaluated.</param>
        /// <param name="childEnum">Enum Identifier of the child element, which carries the CSS selector.</param>
        /// <param name="partialText">Partial text expected within the Text of the child element.</param>
        /// <returns>"true": The container IWebElement contains the expected child IWebElement, otherwise "false".</returns>
        public static bool HasChildOfText(this IWebElement element, Enum childEnum, string partialText)
        {
            bool result = element.HasChildOf(childEnum, 
                x => x.Text.Contains(partialText));

        #if (STEP_BY_STEP)
        {  
          if (result)
              Console.WriteLine("Element found whose text is: " + element.Text);
        }
        #endif

            return result;
        }

        public static bool HasChildOfLink(this IWebElement element, Enum childEnum, string partialText)
        {
            Func<IWebElement, bool> predicate = (x) =>
            {
                string link = x.GetAttribute("href");
                return link.Contains(partialText);
            };
            return element.HasChildOf(childEnum, predicate);
        }

        public static IWebElement FindElementByCss(this IWebElement parentElement, string css)
        {
            try
            {
                var result = parentElement.FindElement(By.CssSelector(css));
                return result;
            }
            catch (Exception e)
            {
                Console.WriteLine("{0} of findelement by CSS: '{1}'", e, css);
                return null;
            }
        }

        public static ReadOnlyCollection<IWebElement> FindElementsByCss(this IWebElement parentElement, string css)
        {
            return parentElement.FindElements(By.CssSelector(css));
        }

        //public static Func<IWebElement, bool> ByIndexPredicate(int index)
        //{
        //    return GenericPredicate<IWebElement>.IndexPredicateOf(index);
        //}

        public static Func<IWebElement, string> GetTextString = x => x.Text;
        public static Func<IWebElement, string> GetValueString = x => x.GetAttribute("value");
        public static Func<IWebElement, string> GetLinkString = x => x.GetAttribute("href");
        //public static Func<IWebElement, string> GetGoogleLinkString = (x) =>
        //{
        //    string href = x.GetAttribute("href");
        //    if (href == null)
        //        throw new NullReferenceException("failed to extract href from " + x);

        //    Uri uri = new Uri(href);
        //    var querys = HttpUtility.ParseQueryString(uri.Query, Encoding.UTF8);
        //    return querys["url"]??"";
        //};

        public static bool TextEquals(this IWebElement element, string exactText)
        {
            return element.Text.Equals(exactText);
        }

        public static bool TextContains(this IWebElement element, string partialText)
        {
            return element.Text.Contains(partialText);
        }

        public static bool ValueContains(this IWebElement element, string valueString)
        {
            string theValue = element.GetAttribute("value");
            return theValue == null ? false : theValue.Contains(valueString);
        }

        public static bool LinkContains(this IWebElement element, string linkPartialString)
        {
            string link = element.GetAttribute("href");
            return link == null ? false : link.Contains(linkPartialString);
        }

        //public static bool GoogleLinkContains(this IWebElement element, string decodedLinkPartialString)
        //{
        //    string link = GetGoogleLinkString(element);
        //    return link == null ? false : link.Contains(decodedLinkPartialString);
        //}

        /// <summary>
        /// To click element covered by others.
        /// </summary>
        /// <param name="element">The IWebElement to be clicked.</param>
        public static void ClickByScript(this IWebElement element)
        {
            DriverManager.ExecuteScript("arguments[0].click()", element);
        }

        /// <summary>
        /// To hover the mouse over the element and remain for a period for observation.
        /// </summary>
        /// <param name="element">The IWebElement to be hovered over.</param>
        /// <param name="remainMillis">Time elapsed to show the hover effect.</param>
        public static void Hover(this IWebElement element, int remainMillis = 1000)
        {
            Actions hover = new Actions(DriverManager.Driver);
            hover.MoveToElement(element);
            hover.Perform();
            //Keep the mouse hovering for a period to enable testers to watch the effect
            Thread.Sleep(remainMillis);
        }

        /// <summary>
        /// Ctrl+click requires three actions: a send_keys action to press the CTRL key, 
        /// then a click action, and then another send_keys action to release the CTRL key.
        /// <see cref="https://code.google.com/p/selenium/wiki/AdvancedUserInteractions"/>
        /// </summary>
        /// <param name="element">The element on which to be control-clicked.</param>
        public static void ControlClick(this IWebElement element)
        {
            //new Actions(DriverManager.Driver).KeyDown(Keys.Control).Click(element).Release().Perform();
            element.SpecialClickByScript(true);
        }

        /// <summary>
        /// Shift+click requires three actions: a send_keys action to press the SHIFT key, 
        /// then a click action, and then another send_keys action to release the SHIFT key.
        /// <see cref="https://code.google.com/p/selenium/wiki/AdvancedUserInteractions"/>
        /// </summary>
        /// <param name="element">The element on which to be shift-clicked.</param>
        public static void ShiftClick(this IWebElement element)
        {
            new Actions(DriverManager.Driver).KeyDown(Keys.Shift).Click(element).Release().Perform();
            //Or by script
            //element.SpecialClickByScript(false, false, true);
        }

        /// <summary>
        /// ControlClick() failed to select multiple options in a select element, try JavaScript instead.
        /// <see cref="http://marcgrabanski.com/simulating-mouse-click-events-in-javascript/"/>
        /// <see cref="https://developer.mozilla.org/en-US/docs/Web/API/event.initMouseEvent"/>
        /// <see cref="http://stackoverflow.com/questions/15609901/simulate-ctrl-click-on-richfaces3-3-richextendeddatatable"/>
        /// </summary>
        /// <param name="element">The element on which to be control-clicked.</param>
        /// <param name="ctrlKey">whether or not control key was depressed during the Event.</param>
        /// <param name="altKey">whether or not alt key was depressed during the Event.</param>
        /// <param name="shiftKey">whether or not shift key was depressed during the Event.</param>
        /// <param name="button">This property returns an integer value indicating the button that changed state.
        ///     0 for standard "click"; 1 for middle button; 2 for right button.
        /// </param>
        public static void SpecialClickByScript(this IWebElement element, bool ctrlKey=false, bool altKey=false, bool shiftKey=false, int button=0)
        {
            string parameters =
                string.Format("'click', true, true, window, 0, 0, 0, 0, 0, {0}, {1}, {2}, false, {3}, null", 
                    ctrlKey, altKey, shiftKey, button).ToLower();

            string script = string.Format("var controlclick = document.createEvent('MouseEvents');"+
                "controlclick.initMouseEvent({0});"
                + "arguments[0].dispatchEvent(controlclick);", parameters);
            DriverManager.ExecuteScript(script, element);
        }

        /// <summary>
        /// Double-clicks the mouse on the specified element.
        /// </summary>
        /// <param name="element">The element on which to be double-clicked.</param>
        public static void DoubleClick(this IWebElement element)
        {
            Actions actions = new Actions(DriverManager.Driver);
            actions.DoubleClick(element);
            actions.Perform();
        }

        /// <summary>
        /// This funtion would make the hiding element shown in the screen by calling ILocator.LocationOnScreenOnceScrolledIntoView
        /// which would scroll the browser to show the element into view.
        /// <see cref="http://selenium.googlecode.com/git/docs/api/dotnet/html/P_OpenQA_Selenium_ILocatable_LocationOnScreenOnceScrolledIntoView.htm"/>
        /// </summary>
        /// <param name="element">The element need to be shown.</param>
        public static void Show(this IWebElement element)
        {
            ILocatable iLocatable = element as ILocatable;
            if (iLocatable == null)
                return;

#if DEBUG
            Point originalLocation = element.Location;
            Console.WriteLine("Original Position: {0}", originalLocation);
#endif

            Point position = iLocatable.LocationOnScreenOnceScrolledIntoView;
#if DEBUG
            Console.WriteLine("After Scrolling: {0}", position);
#endif
        }

        private const string defaultHighlightStyle = "color: green; border: 2px solid red; background-color: yellow;";
        /// <summary>
        /// Highlight the element with JavaScript.
        /// <see cref="http://selenium.polteq.com/en/highlight-elements-with-selenium-webdriver/"/>
        /// </summary>
        /// <param name="element">The element to be highlighted.</param>
        /// <param name="highlightStyle">Style string used to highlight the element.</param>
        /// <param name="timeInMilliSec">Time of keep the element highlighted.</param>
        public static void HighLight(this IWebElement element, string highlightStyle=defaultHighlightStyle, int timeInMilliSec=1000)
        {
            //*/ Highlight by script: changing the style of concerned element
            string oldStyle = element.GetAttribute("style");
            IJavaScriptExecutor js = (IJavaScriptExecutor)DriverManager.Driver;
            js.ExecuteScript("arguments[0].setAttribute('style', arguments[1]);",
                element, highlightStyle);
            Thread.Sleep(timeInMilliSec);
            js.ExecuteScript("arguments[0].setAttribute('style', arguments[1]);",
                element, oldStyle);
            /*/ Highlight with Actions, but failed to get the expected effect
            IHasInputDevices iHasInputDevices = DriverManager.Driver as IHasInputDevices;
            if (iHasInputDevices == null)
                return;

            IMouse mouse = iHasInputDevices.Mouse;
            if (mouse == null)
                return;

            RemoteWebElement remoteWebElement = element as RemoteWebElement;
            if (remoteWebElement == null)
                return;

            ICoordinates iCoordinates = remoteWebElement.Coordinates;

            if (!remoteWebElement.Displayed)
                remoteWebElement.Show();

            //Point mouseLocation = 
            Point location = element.Location;
            Size size = element.Size;

            //mouse.MouseMove(iCoordinates);

            Actions actions = new Actions(DriverManager.Driver);
            actions.MoveToElement(remoteWebElement);
            actions.KeyDown(Keys.Control);
            actions.ClickAndHold();
            //actions.MoveToElement(element, size.Width+1, size.Height+1);
            actions.MoveByOffset(size.Width + 2, size.Height + 2);
            actions.Release();
            actions.Perform();
            //*/
        }
    }
}
