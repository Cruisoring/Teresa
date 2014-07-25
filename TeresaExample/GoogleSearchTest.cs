#if DEBUG
#define Highlight_Target
#endif

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using NUnit.Framework;
using OpenQA.Selenium;
using Teresa;
using Teresa.Utility;
using TeresaExample.GooglePages;

namespace TeresaExample
{
    [TestFixture]
    public class GoogleSearchTest
    {
        [SetUp]
        public void Setup()
        {
            DriverManager.NavigateTo("http://www.google.com");
        }

        [Test]
        public void SearchTest()
        {
            //Input something to search
            Page.CurrentPage[GooglePage.TextByName.textToBeSearched] = "WebDriver wrapper";
            Assert.AreEqual("WebDriver wrapper", Page.CurrentPage[GooglePage.TextByName.textToBeSearched]);

            //Only for TextLocator, the command shall be specified as "extraInfo" instead of "value", just like this:
            Page.CurrentPage[GooglePage.TextByName.textToBeSearched, "highlight"] = "";

            //After input something, the button of "Google Search" (<button name="btnK">) would disappear
            //So any command on it would throw exception, using "tryXXX" would quit and set "LastTrySuccess" to "false"
            //Notice also the failed "tryXXX" would halt the browser for a period (10s or 20s?)
            Page.CurrentPage[GooglePage.ButtonByName.SearchBeforeInput] = "tryclick";
            if (!Page.LastTrySuccess)
                Page.CurrentPage[GooglePage.ButtonByName.Search] = "clickscript";

            //Alternatively, SendKeys(Keys.Enter) could function just like click the button
            //Page.CurrentPage[GooglePage.ButtonByName.Search, "sendkeys"] = Keys.Enter;

            Page.CurrentPage[GooglePage.LinkByClass.SearchByVoice] = "hover";

            //Scroll page to bottom to show the link of "Next"
            Page.CurrentPage[GooglePage.LinkById.Next] = "show";
#if Highlight_Target
            //Highlight it now, otherwise it may not be noticed
            Page.CurrentPage[GooglePage.LinkById.Next] = "highlight";
#endif

            //There is no link whose Text is "Web", so the click would throw Exception:
            Assert.Catch<NoSuchElementException>(() =>
                Page.CurrentPage[SearchNavFragment.LinkByText.Web] = "click");

            Page.CurrentPage[SearchNavFragment.LinkByText.Web] = "tryclick";

#if Highlight_Target
            Page.CurrentPage[SearchNavFragment.DivByCustom.Web] = "highlight";
#endif
            //However, click the Div container of "Web" would be OK, though nothing would happen
            Page.CurrentPage[SearchNavFragment.DivByCustom.Web] = "click";

            var text = Page.CurrentPage[SearchNavFragment.DivByCustom.Web, "selected"];

            //Shall throw NullReferenceException since there is no filters applied to choose parent of LinkByParent.Title
            Assert.Catch<NullReferenceException>(() =>
                Page.CurrentPage[GooglePage.ResultItemFragment.LinkByParent.Title] = "click");

            //Click the Downward Arrow of the third result item (index = 2)
            Page.CurrentPage[GooglePage.ResultItemFragment.LinkByParent.DownArrow, null, 
                GenericPredicate<IWebElement>.IndexPredicateOf(2)] = "click";

            //Validate now the above Downward Arrow can still be highlighted when "filters" is missing
            Page.CurrentPage[GooglePage.ResultItemFragment.AnyByCustom.LinkAddress] = "highlight";

            Page.CurrentPage[GooglePage.ResultItemFragment.ListItemAllByClass.g] = "highlight";

            Page.CurrentPage[GooglePage.ResultItemFragment.AnyByCustom.LinkAddress, null,
                (e) => e.HasChildOfText(GooglePage.ResultItemFragment.AnyByCustom.LinkAddress,"stackoverflow.com")] = "highlight";

            Page.CurrentPage[GooglePage.ResultItemFragment.LinkByParent.Title] = "click";

            DriverManager.Driver.Navigate().Back();

            string css = Page.CurrentPage[GooglePage.ResultItemFragment.LinkByParent.Title, "css"];
            string parentCss = Page.CurrentPage[GooglePage.ResultItemFragment.LinkByParent.Title, "parentcss"];
            string fullCss = Page.CurrentPage[GooglePage.ResultItemFragment.LinkByParent.Title, "fullcss"];
            string href = Page.CurrentPage[GooglePage.ResultItemFragment.LinkByParent.Title, "href"];
            string clas = Page.CurrentPage[GooglePage.ResultItemFragment.AnyByCustom.LinkAddress, "class"];

            //Control click to open a new tab
            Page.CurrentPage[GooglePage.ResultItemFragment.LinkByParent.Title, null,
                (e) => e.HasChildOfText(GooglePage.ResultItemFragment.AnyByCustom.LinkAddress,"code.google")] = "controlclick";

            //Notice: when a Google DownArrow is focused, clicking another DownArrow immediately would just close the previous DownArrow
            //Thus it is better to click another element first
            Page.CurrentPage[GooglePage.ResultItemFragment.SpanByClass.Description, null,
                GenericPredicate<IWebElement>.IndexPredicateOf(2)] = "click";
#if Highlight_Target
            Page.CurrentPage[GooglePage.ResultItemFragment.SpanByClass.Description] = "highlight";
#endif

            //Define predicate to choose both Result item container (identified by ListItemAllByClass.g)
            // and the action menu item (identified by ListItemAllByClass.action_menu_item)
            Func<IWebElement, bool> predicate = (e) =>
            {
                string elementClass = e.GetAttribute("class");
                return (elementClass=="g" && e.HasChildOfText(GooglePage.ResultItemFragment.AnyByCustom.LinkAddress,
                    "stackoverflow.com") || (elementClass.Contains("action-menu-item") &&e.Text == "Similar"));
            };

            Page.CurrentPage[GooglePage.ResultItemFragment.LinkByParent.DownArrow, null, predicate] = "click";

            //To highlight the action menu item whose text is "Similar"
            Page.CurrentPage[GooglePage.ResultItemFragment.ListItemAllByClass.action_menu_item, null, predicate] = "tryhighlight";

            if (Page.LastTrySuccess)
            Page.CurrentPage[GooglePage.ResultItemFragment.ListItemAllByClass.action_menu_item] = "click";

            Thread.Sleep(2000);
        }

        //[Test]
        //public void SearchAcrossMultiplePages()
        //{
        //    string keyword = "Enum Usage";
        //    Page.CurrentPage[GooglePage.TextByName.textToBeSearched] = keyword;
        //    Page.CurrentPage[GooglePage.ButtonByName.SearchBeforeInput] = "tryclick";
        //    if (!Page.LastTrySuccess)
        //        Page.CurrentPage[GooglePage.ButtonByName.Search] = "click";

        //    bool found = false;

        //    Func<IWebElement, bool> predicate = (x) =>
        //    {
        //        bool result = x.HasChildOfText(GooglePage.ResultItemFragment.LinkByParent.Title,
        //            "Advanced Enum Usage with examples");

        //        if (result)
        //        {
        //            Console.WriteLine("{0:mm:ss.fff}: x.Text='{1}'", DateTime.Now, x.Text.Substring(0, 20));
        //            found = true;
        //        }
        //        return result;
        //    };

        //    do
        //    {
        //        Page.CurrentPage[GooglePage.ResultItemFragment.LinkByParent.Title,
        //            null, predicate] = "tryclickscript";

        //        Page.CurrentPage[GooglePage.ButtonByName.SearchBeforeInput] = "tryclick";

        //        if (!found)
        //        {
        //            Thread.Sleep(1000);
        //            Page.CurrentPage[GooglePage.LinkById.Next] = "tryclickscript";
        //        }
        //        else
        //            break;

        //    } while (!found && Page.LastTrySuccess);

        //    Thread.Sleep(5000);

        //    Page.CurrentPage[CodeProjectPage.SpanByClass.SignIn] = "highlight";

        //    //Sample to demonstrate how to design operation in traditional manner, notice there is no output line to indicate this action
        //    Locator signIn = Page.CurrentPage.LocatorOf(CodeProjectPage.SpanByClass.SignIn);
        //    IWebElement element = signIn.FindElement();
        //    element.Hover();

        //    //Page.CurrentPage[CodeProjectPage.SpanByClass.SignIn] = "hover";
        //    Page.CurrentPage[CodeProjectPage.NavBarFragment.LinkByText.home] = "highlight";
        //    Page.CurrentPage[CodeProjectPage.NavBarFragment.LinkByText.home] = "click";
        //}
    }
}
