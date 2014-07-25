using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenQA.Selenium;
using OpenQA.Selenium.Remote;

namespace Teresa
{
    public class RadioLocator : Locator
    {
        public override string this[string extraInfo, Func<IWebElement, bool> filters = null]
        {
            get
            {
                if (string.IsNullOrEmpty(extraInfo) || extraInfo == "selected")
                {
                    var target = FindElement(filters);
                    string result = target.Selected.ToString();
                    Console.WriteLine("\"{0}\"=[{1}];", result, Identifier.FullName());
                    return result;
                }

                return base[extraInfo, filters];
            }
            set
            {
                bool toCheck = false;
                if (bool.TryParse(value, out toCheck))
                {
                    var target = FindElement(filters);
                    if (toCheck != target.Selected)
                    {
                        target.Click();
                        Console.WriteLine("[{0}]=\"{1}\";", Identifier.FullName(), toCheck);
                    }
                }
            }
        }

        public RadioLocator(Enum identifier, Fragment parent)
            : base(identifier, parent)
        {}
    }
}
