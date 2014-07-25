using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenQA.Selenium;

namespace Teresa
{
    public class TextLocator : Locator
    {
        public override string this[string extraInfo, Func<IWebElement, bool> filters = null]
        {
            //Authors may create two types of controls that allow users to input text. 
            //The INPUT element creates a single-line input control and the TEXTAREA element 
            //creates a multi-line input control. In both cases, the input text becomes the control's current value.
            get { return base[extraInfo??"value", filters]; }
            set
            {
                IWebElement element = FindElement(filters);
                if (!string.IsNullOrEmpty(extraInfo))
                {
                    //Fill the value before calling Indexer of Locator
                    base[extraInfo, filters] = string.IsNullOrEmpty(value) ? extraInfo : value;
                }
                else if (string.IsNullOrEmpty(value))
                {
                    element.Clear();
                    Console.WriteLine("[{0}]=\"{1}\";", Identifier.FullName(), "");
                }
                else
                {
                    //Input "Ctrl+A" to select the text within the element.
                    element.SendKeys(Keys.Control + "a");
                    //Input "Tab" after the value to select item filled by AJAX, notice that filters is not explicitly
                    //used here because it is stored due to the above call of FindElement(filters).
                    element.SendKeys(value + Keys.Tab);
                    Console.WriteLine("[{0}]=\"{1}\";", Identifier.FullName(), value);
                }
            }
        }

        public TextLocator(Enum identifier, Fragment parent) : base(identifier, parent)
        {}
    }
}
