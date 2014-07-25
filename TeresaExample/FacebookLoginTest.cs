using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using NUnit.Framework;
using OpenQA.Selenium;
using Teresa;
using TeresaExample.Pages;

namespace TeresaExample
{
    [TestFixture]
    public class FacebookLoginTest
    {
        [Test]
        public void SignUpTest()
        {
            DriverManager.NavigateTo("https://www.facebook.com");

            //When "extraInfo" is missing, the default value of "null" would make Locator return its text
            string valueString = Page.CurrentPage[FacebookLoginPage.ButtonByText.SignUp];
            Assert.AreEqual(valueString, "Sign Up");
            //To verify the Indexer can get the "class" attribute appeared within the button
            valueString = Page.CurrentPage[FacebookLoginPage.ButtonByText.SignUp, "class"];
            Assert.AreEqual(valueString, "_6j mvm _6wk _6wl _58mi _3ma _6o _6v");
            //Assure no exception is throw when querying a meaningless "extraInfo", 
            //  which would call IWebElement.GetAttribute() and returns null
            valueString = Page.CurrentPage[FacebookLoginPage.ButtonByText.SignUp, "non-existed"];
            Assert.IsNull(valueString);

            valueString = Page.CurrentPage[FacebookLoginPage.ButtonByText.SignUp, "enabled"];
            valueString = Page.CurrentPage[FacebookLoginPage.ButtonByText.SignUp, "displayed"];
            valueString = Page.CurrentPage[FacebookLoginPage.ButtonByText.SignUp, "location"];
            valueString = Page.CurrentPage[FacebookLoginPage.ButtonByText.SignUp, "size"];
            valueString = Page.CurrentPage[FacebookLoginPage.ButtonByText.SignUp, "css"];
            valueString = Page.CurrentPage[FacebookLoginPage.ButtonByText.SignUp, "parentcss"];
            valueString = Page.CurrentPage[FacebookLoginPage.ButtonByText.SignUp, "fullcss"];
            valueString = Page.CurrentPage[FacebookLoginPage.ButtonByText.SignUp, "style"];



            //verify the name of the element is accessible
            valueString = Page.CurrentPage[FacebookLoginPage.TextByName.firstname, "name"];
            Assert.AreEqual("firstname", valueString);
            //verify the Css selector is composed as expected
            valueString = Page.CurrentPage[FacebookLoginPage.TextByName.firstname, "css"];
            Assert.IsTrue(valueString.Contains(@"input[type][name*='firstname']"));
            //verify the ParentCss() returns null when the Enum is defined directly in a page
            valueString = Page.CurrentPage[FacebookLoginPage.TextByName.firstname, "parentcss"];

            Page.CurrentPage[FacebookLoginPage.TextByName.firstname] = "Jack";

            //With extraInfo="sendkeys" to call IWebElement.SendKeys() via Indexer of the Locator class
            Page.CurrentPage[FacebookLoginPage.TextByName.firstname, "sendkeys"] = "Tom";
            Thread.Sleep(500);
            Assert.AreEqual("JackTom", Page.CurrentPage[FacebookLoginPage.TextByName.firstname]);

            //Example to show how to use the locating mechanism to find the element and perform operation in traditional way
            Locator firstNameLocator = Page.CurrentPage.LocatorOf(FacebookLoginPage.TextByName.firstname);
            IWebElement firstNameElement = firstNameLocator.FindElement();
            firstNameElement.Clear();
            firstNameElement.SendKeys("Tom");

            Page.CurrentPage[FacebookLoginPage.TextByName.lastname] = "Smith";
            Page.CurrentPage[FacebookLoginPage.TextByName.regEmail] = "youremail@hotmail.com";
            Page.CurrentPage[FacebookLoginPage.TextByName.regEmailConfirmation] = "youremail@hotmail.com";
            Page.CurrentPage[FacebookLoginPage.TextByName.regPassword] = "Password#$@0";

            //Default way to choose option: by entering first characters + TAB
            Page.CurrentPage[FacebookLoginPage.SelectById.month, "Ap"] = "true";
            //Assert the SelectLocator returns text by default when "extraInfo" is missing
            Assert.AreEqual("Apr", Page.CurrentPage[FacebookLoginPage.SelectById.month]);
            //Assert SelectLocator returns index value as its order, even when it is not defined in the web page
            Assert.AreEqual("4", Page.CurrentPage[FacebookLoginPage.SelectById.month, "#"]);
            //Assert the value of the selected item
            Assert.AreEqual("4", Page.CurrentPage[FacebookLoginPage.SelectById.month, "$"]);

            //Select "day" by text of the option, with "text=" to specify it must be matched exactly
            Page.CurrentPage[FacebookLoginPage.SelectById.day, "text=15"] = "true";
            Assert.AreEqual(Page.CurrentPage[FacebookLoginPage.SelectById.day, "text"], "15");

            //Select "year" by actually input "201" + TAB, thus the first one "2014" shall be selected
            Page.CurrentPage[FacebookLoginPage.SelectById.year, "201"] = "no matter what";
            //Confirm it goes as expected
            Assert.AreEqual(Page.CurrentPage[FacebookLoginPage.SelectById.year], "2014");

            //Select "day" with "index", because the element has no "index" attribute, so the nature order is used actually
            Page.CurrentPage[FacebookLoginPage.SelectById.day, "Index=8"] = "true";
            Assert.AreEqual(Page.CurrentPage[FacebookLoginPage.SelectById.day, "$"], "8");
            //Select "day" by "value" and validate the result
            Page.CurrentPage[FacebookLoginPage.SelectById.day, "$24"] = "true";
            Assert.AreEqual(Page.CurrentPage[FacebookLoginPage.SelectById.day, ""], "24");

            //Choose radio by clicking associated lable
            Page.CurrentPage[FacebookLoginPage.LabelByText.Female] = "true";
            Assert.AreEqual(true.ToString(), Page.CurrentPage[FacebookLoginPage.RadioByCustom.female, "selected"]);
            Assert.AreEqual(false.ToString(), Page.CurrentPage[FacebookLoginPage.RadioByCustom.male]);

            //Choose radio directly
            Page.CurrentPage[FacebookLoginPage.LabelByText.Male] = "true";
            Assert.AreEqual(true.ToString(), Page.CurrentPage[FacebookLoginPage.RadioByCustom.male, ""]);

            //Click the sign-up button
            //Page.CurrentPage[FacebookLoginPage.ButtonByText.SignUp] = "true";
            Page.CurrentPage[FacebookLoginPage.ButtonByText.SignUp] = "submit";
        }
    }
}
