using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using NUnit.Framework;
using OpenQA.Selenium;
using Teresa;
using TeresaExample.SelectExamples;

namespace TeresaExample
{
    [TestFixture]
    public class SelectExamplesTest
    {
        [TestFixtureSetUp]
        public void LoadSelectSamples()
        {
            var fileInfo = new FileInfo(@".\SelectExamples.html");
            var fileUri = new Uri(fileInfo.FullName);
            DriverManager.NavigateTo(fileUri.AbsoluteUri);
        }

        [Test]
        public void SelectAttribute_ValidateGetter()
        {
            string temp = Page.CurrentPage[SelectExamplesPage.SelectByName.food, "multiple"];
            temp = Page.CurrentPage[SelectExamplesPage.SelectByName.food, "ismultiple"];

            temp = Page.CurrentPage[SelectExamplesPage.SelectByName.food];
            //Still click "Peaches" even if the value is "false"
            Page.CurrentPage[SelectExamplesPage.SelectByName.food, "p"] = "false";
            Thread.Sleep(2000);
            temp = Page.CurrentPage[SelectExamplesPage.SelectByName.food, Operation.AllSelected];
            temp = Page.CurrentPage[SelectExamplesPage.SelectByName.food];
            temp = Page.CurrentPage[SelectExamplesPage.SelectByName.food, "text"];
            temp = Page.CurrentPage[SelectExamplesPage.SelectByName.food, "index"];
            temp = Page.CurrentPage[SelectExamplesPage.SelectByName.food, "#"];
            temp = Page.CurrentPage[SelectExamplesPage.SelectByName.food, "value"];
            temp = Page.CurrentPage[SelectExamplesPage.SelectByName.food, "$"];

            Page.CurrentPage[SelectExamplesPage.SelectByName.sports, Operation.AllOptions] = "true";
            temp = Page.CurrentPage[SelectExamplesPage.SelectByName.sports, Operation.AllSelected];
            Thread.Sleep(8000);
            
            Page.CurrentPage[SelectExamplesPage.SelectByName.food, "bana"] = "false";
            temp = Page.CurrentPage[SelectExamplesPage.SelectByName.food, Operation.AllSelected];
            Thread.Sleep(2000);

            temp = Page.CurrentPage[SelectExamplesPage.SelectByName.techniques, "multiple"];
            temp = Page.CurrentPage[SelectExamplesPage.SelectByName.techniques, "ismultiple"];

            Page.CurrentPage[SelectExamplesPage.SelectByName.techniques, Operation.AllOptions] = "true";
            temp = Page.CurrentPage[SelectExamplesPage.SelectByName.techniques, Operation.AllSelected];
            Thread.Sleep(20000);

            Page.CurrentPage[SelectExamplesPage.SelectByName.techniques, "g9"] = "true";
            Thread.Sleep(2000);
            temp = Page.CurrentPage[SelectExamplesPage.SelectByName.techniques, Operation.AllSelected];

            Page.CurrentPage[SelectExamplesPage.SelectByName.techniques, "g9"] = "false";
            Thread.Sleep(2000);
        }

        [Test]
        public void Select_ControlClick()
        {
            Page.CurrentPage[SelectExamplesPage.SelectByName.cars, "text=Volvo"] = "true";
            Thread.Sleep(2000);
            Page.CurrentPage[SelectExamplesPage.SelectByName.cars, "text=Saab"] = "true";
            Thread.Sleep(2000);
            Page.CurrentPage[SelectExamplesPage.SelectByName.cars, "text=Opel"] = "true";
            Thread.Sleep(2000);
            Page.CurrentPage[SelectExamplesPage.SelectByName.cars, "text=Volvo"] = "false";
            Thread.Sleep(2000);
            Page.CurrentPage[SelectExamplesPage.SelectByName.cars, "alloptions"] = "false";
            Thread.Sleep(2000);
            
        }

        [Test]
        public void SelectOption_ByText()
        {
            //By default, the SelectLocator would call SendKeys("Golf" + TAB), which would select the option of "Golf"
            Page.CurrentPage[SelectExamplesPage.SelectByName.sports, "go"] = "true";
            Assert.AreEqual("Golf", Page.CurrentPage[SelectExamplesPage.SelectByName.sports]);

            //Only the exactly matched option would be clicked
            Page.CurrentPage[SelectExamplesPage.SelectByName.sports, "text=Polo"] = "true";
            Assert.IsTrue(Page.CurrentPage[SelectExamplesPage.SelectByName.sports, "allselected"].Contains("Polo"));

            //It would also select option of "Soccer" by SendKeys("soc" + TAB)
            //WARNING: sometime, this case may fail when sendkeys() doesn't work as expected!!!!!!!!!
            Page.CurrentPage[SelectExamplesPage.SelectByName.sports, "soc"] = "true";
            string allSelected = Page.CurrentPage[SelectExamplesPage.SelectByName.sports, "allselected"];
            Assert.IsTrue(allSelected.Contains("Soccer"));

            //But the "text=" prefix means the text must be exactly matched with the option, 
            //so "text=polo" won't select option "Polo"
            Assert.Catch<NoSuchElementException>(() =>
                Page.CurrentPage[SelectExamplesPage.SelectByName.sports, "text=polo"] = "true");
            Assert.IsFalse(Page.CurrentPage[SelectExamplesPage.SelectByName.sports, "allselected"].Contains("Polo"));

            //Option of "Golf" would be clicked even the value is "FALSE"
            Page.CurrentPage[SelectExamplesPage.SelectByName.sports, "polo"] = "FALSE";
            Assert.IsFalse(Page.CurrentPage[SelectExamplesPage.SelectByName.sports, "allselected"].Contains("Polo"));

            //Select all options
            Page.CurrentPage[SelectExamplesPage.SelectByName.sports, "alloptions"] = "true";
            Assert.IsTrue(Page.CurrentPage[SelectExamplesPage.SelectByName.sports, "allselected"].Contains("Hockey"));
            //De-select all options
            Page.CurrentPage[SelectExamplesPage.SelectByName.sports, "alloptions"] = "False";
            Assert.AreEqual("", Page.CurrentPage[SelectExamplesPage.SelectByName.sports, "allselected"]);
        }

        [Test]
        public void SelectOption_ByIndex()
        {
            Page.CurrentPage[SelectExamplesPage.SelectByName.food, "index=2"] = "true";
            Assert.AreEqual("Peaches", Page.CurrentPage[SelectExamplesPage.SelectByName.food]);

            Assert.Catch<IndexOutOfRangeException>(()=> 
                Page.CurrentPage[SelectExamplesPage.SelectByName.food, "index=22"] = "true");

            Page.CurrentPage[SelectExamplesPage.SelectByName.food, "#8"] = "true";
            Assert.AreEqual("Chocolate Cake", Page.CurrentPage[SelectExamplesPage.SelectByName.food]);

        }

        [Test]
        public void SelectOption_ByValue()
        {
            Page.CurrentPage[SelectExamplesPage.SelectByName.food, "value=2"] = "true";
            Assert.AreEqual("Carrots", Page.CurrentPage[SelectExamplesPage.SelectByName.food]);

            Assert.Catch<NoSuchElementException>(() => 
                Page.CurrentPage[SelectExamplesPage.SelectByName.food, "value=22"] = "true");

            Page.CurrentPage[SelectExamplesPage.SelectByName.food, "$1"] = "true";
            Assert.AreEqual("Apples", Page.CurrentPage[SelectExamplesPage.SelectByName.food]);

        }
    }
}
