using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenQA.Selenium;

namespace Teresa
{
    public class CheckboxLocator : Locator
    {
        public override string this[string extraInfo, Func<IWebElement, bool> filters = null]
        {
            get
            {
                if (string.IsNullOrEmpty(extraInfo) || extraInfo == "selected" || extraInfo == "checked")
                    return FindElement(filters).Selected.ToString();

                return base[extraInfo, filters];
            }
            set
            {
                bool toCheck = false;
                if (bool.TryParse(value, out toCheck))
                {
                    if(toCheck != FindElement(filters).Selected)
                        FindElement().Click();
                }
            }
        }

        public CheckboxLocator(Enum identifier, Fragment parent) : base(identifier, parent)
        {}
    }
}
