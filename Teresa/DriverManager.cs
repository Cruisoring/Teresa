using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Firefox;
using OpenQA.Selenium.IE;
using OpenQA.Selenium.PhantomJS;
using OpenQA.Selenium.Remote;
using OpenQA.Selenium.Safari;

namespace Teresa
{
    public enum WebDriverTypes
    {
        ChromeDriver,
        FirefoxDriver,
        InternetExplorerDriver,
        SafariDriver,
        HtmlUnitDriver,
        AndroidDriver,
        PhantomJSDriver,
        EventFiringWebDriver
    }

    public static class DriverManager
    {
        private static WebDriverTypes _webDriverTypeType = WebDriverTypes.FirefoxDriver;

        private static IWebDriver driver = null;

        private static RemoteWebDriver driverOf(WebDriverTypes _webDriverTypeType)
        {
            switch (_webDriverTypeType)
            {
                case WebDriverTypes.ChromeDriver:
                    return new ChromeDriver();
                case WebDriverTypes.FirefoxDriver:
                    return new FirefoxDriver();
                case WebDriverTypes.InternetExplorerDriver:
                    return new InternetExplorerDriver();
                //case WebDriverTypes.SafariDriver:
                //    return new SafariDriver();
                //case WebDriverTypes.PhantomJSDriver:
                //    return new PhantomJSDriver();
                default:
                    throw new NotSupportedException();
            }
        }

        public static IWebDriver Driver
        {
            get
            {
                if (driver == null)
                {
                    //EventFiringWebDriver eventFiringWebDriver = new EventFiringWebDriver(driverOf(_webDriverTypeType));

                    //eventFiringWebDriver.Navigated += DriverManager.OnNavigated;
                    //eventFiringWebDriver.ExceptionThrown += DriverManager.EventFiringWebDriverOnExceptionThrown;

                    //driver = eventFiringWebDriver;

                    driver = driverOf(_webDriverTypeType);

                    //Maximize the browser to avoid failing to click an element out of the window
                    driver.Manage().Window.Maximize();
                }
                return driver;
            }
        }

        public static ReadOnlyCollection<IWebElement> FindElementsByCssSelector(string css)
        {
            var result = driver.FindElements(By.CssSelector(css));
            return result;
        }

        public static IWebElement FindElementByCssSelector(string css)
        {
            var result = driver.FindElement(By.CssSelector(css));
            return result;
        }

        public static object ExecuteScript(string script, params object[] args)
        {
            return (driver as IJavaScriptExecutor).ExecuteScript(script, args);
        }

        //public static void EventFiringWebDriverOnExceptionThrown(object sender, WebDriverExceptionEventArgs webDriverExceptionEventArgs)
        //{
        //    Console.WriteLine("OnExceptionThrown: " + webDriverExceptionEventArgs);
        //}

        //public static void OnNavigated(object sender, WebDriverNavigationEventArgs webDriverNavigationEventArgs)
        //{
        //    if (driver != null && Page.CurrentPage != null)
        //    {
        //        Page.CurrentPage.ActualUri = CurrentUri;
        //    }
        //}

        public static void SetDriverType(WebDriverTypes theDriverType = WebDriverTypes.ChromeDriver)
        {
            if (_webDriverTypeType != theDriverType)
            {
                _webDriverTypeType = theDriverType;
                if (driver != null)
                {
                    driver.Quit();
                    driver = null;
                }
            }
        }

        public static string Title { get { return Driver.Title; }}

        public static Uri CurrentUri { get { return new Uri(Driver.Url);}}

        public static void NavigateTo(string url)
        {
            Uri destinationUri = new Uri(url);
            foreach (Page page in Page.Pages)
            {
                if (page.Equals(destinationUri))
                {
                    page.Navigate(destinationUri);
                    return;
                }
            }

            throw new Exception("There is no page matched with " + url);
        }
    }
}
