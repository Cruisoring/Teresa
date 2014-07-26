Teresa
======

TERESA is a compact CSS Selector based Selenium WebDriver wrapper and enabler published on Github. Instead of wrapping
WebDriver functions, TERESA focuses more on the optimization the whole process of mapping web pages to classes, 
mechanisms and procedures of locating elements and performing operation with LEAST lines of code.

Based on combined application of Enum, unified Indexer interface for getting/setting operations, 
and lots of Reflection, TERESA provides a systematic way to build test cases over WebDriver:
- Identify elements with Enum and Attribute, and the CSS Selectors are automatically deducted from the Enum type 
names and entry names, so as to relieve WebDriver users from the tedious work of composing CSS/XPATH selectors manually.
- Mapping the hierarchical relationships of elements directly with the nested Enum types; a concise but crafty 
mechanism is used to locate elements more efficiently and effectively.
- Unified Enum based Indexer interface to enable Finding element and Getting/Setting operations in a single 
self-explaining sentence. Consequently, most one-off functions can be avoided and productivity can be enhanced 
dramatically.
- Special element wrapper (SelectLocator, TextLocator and etc.), rich set of extension methods (Control-click, 
Hover, Highlight and etc.), as well as some novel tools (IndexPredicate, Wait.Until ...) are provided to make 
operations on elements easier or more intuitive with almost no extra work.

A detailed introduction and tutorial can be found at:
http://www.codeproject.com/Articles/800496/TERESA-a-compact-WebDriver-Enabler#tutorial


Tutorial

Procedures of Using TERESA
Here I assume you have some experience of using WebDriver and NUIT to develop UAT cases.

Similar to the way of using WebDriver directly, you might need to follow this procedure to build a new project 
to test a web page:

Inspect the web page under test, highlight the elements to be used for Getting or Setting and get the keys 
that distinguish them from others for CSS selector composing. For those who are not familiar with CSS Selector, 
The 30 CSS Selectors you Must Memorize is a good article introducing all mechanisms we used in TERESA.
“id”, “class” are best choices if they are uniquely used throughout the page or within a segment you have 
identified with Enum (IsCollection=”true”);
For elements appeared under or adjacent to other elements that are easier to be located, then sometime it is 
very reliable to use such relationships with more significant elements.
If the targets element are to be mapped to Locator instances instead of TexLocator/SelectLocator/..., and they 
are the only visible child of their container, then locating the parent shall be sufficient to support actions 
like “click”, “hover” and “highlight”. “ListItemAllByClass.action_menu_item” in previous section is a good example, 
the “click” action on the IWebElement referred by it actually happen on the its only child.
Some commonly used attributes, such as “name”/”href”/”value”, can be very helpful to show the meaning or reason of 
the locating, especially when you have some codes using them.
Sometime, elements may lack significant feature to distinguish them from others, then some other uncommon attributes 
can be very helpful. For example, the navigation bar items usually have different functions defined by “onclick()” 
and can be accessed by calling IWebElement.GetAttribute(string), then keywords after that may be copied directly for 
locating with clear intention meaning.
Another case you might encounter is when there are a serial of very similar elements are positioned with fixed order, 
while the text, class, value of them are either exactly same or changed with culture, just like the Navigation bar 
appeared above the search result of Google. Then using their order could be a very convenient manner to avoid modifying 
CSS selectors from time to time.
Sometime, you also need to locate an element based on its text, though it is not supported directly by CSS selector, 
as an effective means, it can still be considered.
Then you need to define a tree of Enum types/members within a class extended from Page class. The quick reference of 
HtmlTagName and Mechnisms might be helpful for you to decide the locating strategy.
Usually, you don’t need to implement any other function or constructor than the property of “Uri SampleUri”, you can 
specify the address of the target page you are testing.
The Enum types, following the rules discussed in CSS Selector from Enum to identify both element type and mechanism 
used (in a format of “TagnameByMechanism” or “TagnameAllByMechanism”), shall be defined as nested types of the 
extended Page class just as the nested target element within a web page.
Each member of these Enum types is used both as Identifier of the target element in the web page and CSS selector to
locate corresponding IWebElement. You can use meaningful string as their name, but must include the key needed by the 
mechanism of CSS selector in EnumMemberAttribute. In addition, if the Enum member is used to identify a Fragment (A 
special Locator functions as Container of its children Locators), then “IsFragment=true” is also be clarified in its 
EnumMemberAttribute.
Notice: The Locator generated from these Enum members are wrapper of tools to find some kind of IWebElement, instead 
of the IWebElement themselves. Thus you are free to define multiple Enum members with different mechanisms to locate 
one IWebElemetn, or you can define any Enum members within this Page file even if their CSS Selectors are only usable in other web pages as long as you don’t use it to try to locate an non-existed IWebElement.
Finally it is time to design test cases, and the quick reference of Getting Operations and Setting Operations might 
be useful.
The unique interface of target Page: the Indexer (string this[Enum theEnum, string extraInfo = null, Func<IWebElement, 
bool> filters = null]) would simply choose the Locator identified by “Enum theEnum”, then use its unique Indexer 
interface (string this[string extraInfo, Func<IWebElement, bool> filters = null]) to perform any operation introduced
in Getting Operations & Setting Operations is always preferred. For example, the calling of Getter() as below would
get the “style” of a text input element:

string attribute = Page.CurrentPage[GooglePage.TextByName.textToBeSearched, "style"];

While using the Setter() in this way would click a link whose Text is “Web”.

Page.CurrentPage[SearchNavFragment.LinkByText.Web] = "click";
Although using the Indexer is always perferred, some function of WebDriver might have not been supported by 
TERESA. However, you can still use the Locator based mechanism to find IWebElement and perform operation 
directly on it. To perform the above clicking, following codes can be used:

Locator web = Page.CurrentPage.LocatorOf(SearchNavFragment.LinkByText.Web);
IWebElement element = web.FindElement();
element.Click();
Next, with two examples, I will show you how to use TERESA step by step.

Basic Operations in Facebook SignUp
This example means to demonstrate the basic Getting/Setting operations supported by using the unique Indexer 
interface of the Page instance (string this[Enum theEnum, string extraInfo, Func<IWebElement, bool> filters = null]).

Mark the page source

As the captured picture shows, most of elements to be used in this example are marked out with the source 
and Enum Identifier.


Page class to map elements

Then we need to define a FacebookPage class, following the guideline of Organizing Locators Together, like this:

public class FacebookLoginPage : Page
{
    public override Uri SampleUri
    {
        get { return new Uri("https://www.facebook.com"); }
    }

    public enum TextByName
    {
        firstname,
        lastname,
        [EnumMember("reg_email__")]
        regEmail,
        [EnumMember("reg_email_confirmation__")]
        regEmailConfirmation,
        [EnumMember("reg_passwd__")]
        regPassword
    }

    public enum SelectById
    {
        month,
        day,
        year
    }

    //For demo purposes: to show how different Css Selector can be used to cope with complex cases
    public enum RadioByCustom
    {
        [EnumMember("[name='sex'][value='1']")]
        female,
        [EnumMember("span#u_0_g span:nth-of-type(2) input")]
        male
    }

    public enum LabelByText
    {
        Female,
        Male
    }

    //For demo purposes: ByText should always be avoided
    public enum ButtonByText
    {
        [EnumMember("Sign Up")]
        SignUp
    }
}
It just includes the overrided “Uri SampleUri” property and a serial of Enum types and their members. 
For the five text boxes (<input type=”text”...> as that of “First Name”), there are three straightforward 
and convenient means to build up the CSS Selector: the id “u_0_1”, the name “firstname” and attribute 
“aria-label”=”First Name”. “aria-label” seems to be not a popular attribute for me, so I tried to avoid 
using it. Although Mechanisms.ById is always preferred, the meaning of “u_0_1” is too ambiguous thus all 
five elements need to be defined a meaningful name manually. So finally, I choose Mechanisms.ByName by 
defining enum type name as “TextByName” for all these five text input fields collectively. Then I just
copy the names of “firstname” and “lastname” directly as its members, but for the remaining three (“Your 
Email”, “Re-enter Email” and “New Password”), their “name”attributes look too long, so I did some extra 
work by defining “regEmail”, “regEmailConfirmation” and “regPassword” to convey the real values of 
“reg_email__”, “reg_email_confirmation__” and “reg_passwd__” respectively.

For the three select input for “Birthday”, their ids of “month”, “day” and “year” are quite good for 
direct Copy+Paste, so I did that within a new Enum type “SelectById”.

For the two radio buttons of “Female” and “Male”, there are two sets of Enums defined to locate the 
radio buttons themselves and the labels attached with them respectively. Though it is quite convenient 
to use the IDs (“u_0_d” and “u_0_e”) directly, the Mechanisms.ByCustom is used instead to show how to 
define the whole CSS selector by your self. The “Female” radio element is assigned with 
“[name='sex'][value='1']” and “span#u_0_g span:nth-of-type(2) input” for “Male”. However, you must make 
sure the CSS selector points to the <input type=”radio”> element. I have wasted quite some time when I 
forgot to append “input” to the second value and as a result, the “Male” can be checked, but the “Selected” 
always returns “False”.

Anyway, that is basically what you need to do to define the locating strategies for each elements on 
the page under testing. Now I would explain the steps to perform basic Getting/Setting operations on 
them in “FacebookLoginTest.cs” within the attached “TERESAExample” project.

Codes to Get/Set

The first block of codes means to demonstrate how to read property/attribute of the target IWebElements 
located with its Enum Identifiers as below:

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

    //verify the name of the element is accessible
    valueString = Page.CurrentPage[FacebookLoginPage.TextByName.firstname, "name"];
    Assert.AreEqual("firstname", valueString);

    //verify the Css selector is composed as expected
    valueString = Page.CurrentPage[FacebookLoginPage.TextByName.firstname, "css"];
    Assert.IsTrue(valueString.Contains(@"input[type][name*='firstname']"));

    //verify the ParentCss() returns null when the Enum is defined directly in a page
    valueString = Page.CurrentPage[FacebookLoginPage.TextByName.firstname, "parentcss"];
    
The meaning and usage of the Indexer (string this[Enum theEnum, string extraInfo, 
Func<IWebElement, bool> filters = null]) of the page instance can be found in Getting Operations.
The output of these Getting operation via Indexer is below:

"Sign Up"=[TERESAExample.Pages.FacebookLoginPage+ButtonByText.SignUp];

"_6j mvm _6wk _6wl _58mi _3ma _6o _6v"=[TERESAExample.Pages.FacebookLoginPage+ButtonByText.SignUp, "class"];

""=[TERESAExample.Pages.FacebookLoginPage+ButtonByText.SignUp, "non-existed"];

"firstname"=[TERESAExample.Pages.FacebookLoginPage+TextByName.firstname, "name"];

" input[type][name*='firstname'],  textarea[name*='firstname']"=[TERESAExample.Pages.FacebookLoginPage+TextByName.firstname, "css"];

""=[TERESAExample.Pages.FacebookLoginPage+TextByName.firstname, "parentcss"];
The second part is means to input data into the five text boxes. Notice that you can still use TERESA 
as a means to help you locating the IWebElement only, then encapsulate the operations directly on 
IWebElement as the clear and input “Tom” into “First Name” shows.

 
    Page.CurrentPage[FacebookLoginPage.TextByName.firstname] = "Jack";
    //With extraInfo="sendkeys" to call IWebElement.SendKeys() via Indexer of the Locator class
    Page.CurrentPage[FacebookLoginPage.TextByName.firstname, "sendkeys"] = "Tom";
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
The third part demonstrates how to operate <input type=”select”> by SendKeys(), text appeared, value or
index as discussed in More Tailored Operations by Special Locators.

The last part handles the two radios and the “Sign Up” button as below:

 Collapse | Copy Code
    //Choose radio by clicking associated lable
    Page.CurrentPage[FacebookLoginPage.LabelByText.Female] = "true";
    Assert.AreEqual(true.ToString(), Page.CurrentPage[FacebookLoginPage.RadioByCustom.female, "selected"]);
    Assert.AreEqual(false.ToString(), Page.CurrentPage[FacebookLoginPage.RadioByCustom.male]);

    //Choose radio directly
    Page.CurrentPage[FacebookLoginPage.LabelByText.Male] = "true";
    Assert.AreEqual(true.ToString(), Page.CurrentPage[FacebookLoginPage.RadioByCustom.male, ""]);

    //Click the sign-up button
    Page.CurrentPage[FacebookLoginPage.ButtonByText.SignUp] = "true";
As you can see, each line of the above codes can perform one set of Getting/Setting operations on a
specific element by wrapping the atomic operations like FindElement()/SendKeys()/Click()/GetAttributes 
by simply calling the Indexer with their Enum Identifier. Because of strict rule enforced on the Enum 
type names, you can avoid composing CSS selectors manually and with meaningful Enum member names, it 
should be not hard for others to guess out the meaning of these codes.

In this example, all Enum types are defined within FacebookPage class directly instead of nested within 
some Fragment classes, and these Enum members are used to identify some unique elements thus not calling 
FindElemenets() of WebDriver. In next example, I would use Google Search to show how to exploit this 
framework with some more advanced skills.

Advanced Uses in Google Search
Most of the sample operations have been discussed in “Locating the IWebElement with Locators”.

Classes to map elements

To demonstrate the layered structure of Locator tree as discussed in Organizing Locators Together, 
there are two Fragment contained within the GooglePage instance: SearchNavbarFragment and ResultItemFragment.
The former is refered by the “ICollection<Enum> FragmentEnums” with its Enum Identifier of 
“SearchNavFragment.DivById.hdtb” and defined in a separate file to map the button like links above the 
searched result as below:

    public class SearchNavFragment : Fragment
   {
        public enum DivById
        {
            [EnumMember(true)]
            hdtb
        }
        public enum DivByCustom
        {
            [EnumMember(".hdtb_mitem:nth-of-type(1)")]
            Web,
            [EnumMember(".hdtb_mitem:nth-of-type(2)")]
            Images,
            [EnumMember(".hdtb_mitem:nth-of-type(3)")]
            Videos,
            [EnumMember(".hdtb_mitem:nth-of-type(4)")]
            Shopping,
            [EnumMember(".hdtb_mitem:nth-of-type(5)")]
            News,
            [EnumMember(".hdtb_mitem:nth-of-type(6)")]
            More,
            [EnumMember(".hdtb_mitem:nth-of-type(7)")]
            SearchTools
        }

        public enum LinkByText
        {
            Web,
            Images,
            Videos,
            Shopping,
            News,
            More,
            [EnumMember("Search tools")]
            SearchTools
        }
    } 
As you might have noticed, the Enum type DivByCustom and LinkByText share a same set of members. 
Actually, if you just need to click on the link, then DivByCustom can be used more securely because 
the link identified by LinkByText would disappear when it is clicked. However, using a simple mechanism 
of “tryclick” and checking if it is success might be an elegant option as described later.

The ResultItemFragment, as we have discussed in Google Search Example: Introduction, defines result items 
as the captured image demonstrated:


The GooglePage class also defines some Enum types as its children as below:

     public class GooglePage : Page
    {
        public enum TextByName
        {
            [EnumMember("q")]
            textToBeSearched
        }
        public enum LinkByClass
        {
            [EnumMember("gsst_a")]
            SearchByVoice
        }

        public enum ButtonByName
        {
            [EnumMember("btnK")]
            SearchBeforeInput,
            [EnumMember("btnG")]
            Search,
        }

        public enum LinkByUrl
        {
            [EnumMember("options")]
            Options,
            [EnumMember("Login")]
            Login
        }

        public enum LinkById
        {
            [EnumMember("pnnext")]
            Next
        }

        public class ResultItemFragment : Fragment
        {
            ...
        }


        public override ICollection<Enum> FragmentEnums
        {
            get { return new Enum[]{SearchNavFragment.DivById.hdtb}; }
        }

        public override Uri SampleUri
        {
            get { return new Uri("http://www.google.com/"); }
        }

        public override bool Equals(Uri other)
        {
            return other.Host.Contains("google.com");
        }
     ...
    }
With this structure, shared Fragments like SearchNavFragment can be shared among Page classes although 
each Page instance need to construct a different instance of it pointing to the Page itself. At the same 
time, the embedded Fragment like would also be constructed with Reflection technique. By putting Enum types
in different Page/Fragment, their Enum members are organized in a hierarchical manner, thus Locators generated
with Reflection technique would present an exact hierarchical pattern to enable using IWebElement.FindElement()
instead of IWebDriver.FindElement().

It also need to be mentioned that when I try to open "http://www.google.com/", the browser would be 
re-directed to "http://www.google.com.au/". So I have to override the default Equals(Uri) to make the 
above Page can be used wherever you are.

Codes to Get/Set

To demonstrate the locating mechanisms, I have included two test cases and you can find the source codes 
in “GoogleSearchTest.cs”. The codes of the first test “public void SearchTest()” are explained here in their order.

The first three sentences:

Page.CurrentPage[GooglePage.TextByName.textToBeSearched] = "WebDriver wrapper";
Assert.AreEqual("WebDriver wrapper", Page.CurrentPage[GooglePage.TextByName.textToBeSearched]);
Page.CurrentPage[GooglePage.TextByName.textToBeSearched, "highlight"] = "";
They are used to show after input keyword and confirmation, how to use “HighLight(string)” defined in 
IWebElementExtension.cs to highlight the target element with JavaScript.



It shall be noticed that the format of perform “highlight” on Text box is different from other Locators, 
actually TextLocator swap the “value” and “extraInfo” string parameters before calling the Setter of Locator 
class which would output like this:

[TERESAExample.GooglePages.GooglePage+TextByName.textToBeSearched]="WebDriver wrapper";
"WebDriver wrapper"=[TERESAExample.GooglePages.GooglePage+TextByName.textToBeSearched, "value"];
[TERESAExample.GooglePages.GooglePage+TextByName.textToBeSearched]="highlight";
After entering the keyword, now we must click the blue button. However, this button is a totally 
different one than the “Google Search” button appeared in a blank page that has a name of "btnK", 
while this has "btnG". Suppose you need to perform some action but cannot guarantee its success, 
then “try” + “action” can be used to avoid throwing exception:

            Page.CurrentPage[GooglePage.ButtonByName.SearchBeforeInput] = "tryclick";
            if (!Page.LastTrySuccess)
                Page.CurrentPage[GooglePage.ButtonByName.Search] = "click";
The "tryclick" would fail, because the button identified by “ButtonByName.SearchBeforeInput” is gone, by 
checking “Page.LastTrySuccess”, we can confirm it’s failed and "click" the button associated with 
“ButtonByName.Search” and it will succeed.

            Page.CurrentPage[GooglePage.LinkByClass.SearchByVoice] = "hover";
            //Scroll page to bottom to show the link of "Next"
            Page.CurrentPage[GooglePage.LinkById.Next] = "show";
The above two sentences are just used to show effect of Hover() and Show(). The first one would move 
mouse over the image of microphone, then “Search by voice” would appear. The “Next” button, usually 
in the very bottom of the page, could be made visible by scroll page to bottom using script.

            Assert.Catch<NoSuchElementException>(() =>
                Page.CurrentPage[SearchNavFragment.LinkByText.Web] = "click");
            Page.CurrentPage[SearchNavFragment.LinkByText.Web] = "tryclick";
            Page.CurrentPage[SearchNavFragment.DivByCustom.Web] = "click"; 
As mentioned earlier, the link “<a>” related with LinkByText.Web does not exist in dynamic page like 
Google, so clicking on it would throw NoSuchElementException. However, you can always click on the empty 
container associated with DivByCustom.Web, then there will be nothing happen or just like you click on a 
link. Executing “tryclick” command on a non-exist element, like the link identified with LinkByText.Web, 
can be safe although need some time to wait for its execution. By the way, two functions within Locator 
(bool TryExecute(string, string, Func<IWebElement, bool>) and IWebElement TryFindElement(Func<IWebElement, bool>, int))
can be used to get similar result if you prefer using Locator instance directly.

By so far, the “Func<IWebElement, bool> filters” of the Indexer (string this[Enum theEnum, string 
extraInfo, Func<IWebElement, bool> filters = null]) has not been used. But the following codes would 
show you how it can be helpful to handle collective elements/containers.

Following lines of codes are used to show when “filters” parameter is mandatory to locate one IWebElement 
that is either one of a collection ones with same CSS Selector, or contained by one of them.

            //Shall throw () if there is no filters applied to choose parent of LinkByParent.Title
            Assert.Catch<NullReferenceException>(() =>
                Page.CurrentPage[GooglePage.ResultItemFragment.LinkByParent.Title] = "click");

            //Click the Downward Arrow of the third result item (index = 2)
            Page.CurrentPage[GooglePage.ResultItemFragment.LinkByParent.DownArrow, null, 
                GenericPredicate<IWebElement>.IndexPredicateOf(2)] = "click";

            //Validate now the above Downward Arrow can still be highlighted when "filters" is missing
            Page.CurrentPage[GooglePage.ResultItemFragment.AnyByCustom.LinkAddress] = "highlight";
            Page.CurrentPage[GooglePage.ResultItemFragment.ListItemAllByClass.g] = "highlight";
The “LinkByParent.Title” refers the clickable page titles like “horejsek/python-webdriverwrapper · GitHub” 
and “WebDriver : Compilation error in custom created Wrapper ...” that show in your browser. Although the 
Enum type name means the IWebElement is unique, that uniqueness is only valid within their container - 
ResultItemFragment identified by “ListItemAllByClass.g”. Because of the cascading procedure of locating 
an IWebElement actually starts from locating the parent IWebElement first, the Locator of “LinkByParent.Title” 
would fail to get the parent IWebElement with only CSS selectors without conditions specified by “filters”. 
Consequently, the first sentence “Page.CurrentPage[GooglePage.ResultItemFragment.LinkByParent.Title] = "click"” 
would throw NullReferenceException.

Then the second sentence provides a IndexPredicate (that is introduced in Google Search Example: Filters 
Counting IWebElements) as below:

Page.CurrentPage[GooglePage.ResultItemFragment.LinkByParent.DownArrow, null, 
  GenericPredicate<IWebElement>.IndexPredicateOf(2)] = "click";
As a result, the Locator of “ListItemAllByClass.g” would choose the third result item after calling 
IWebDriver.FindElemens() to get a “ReadOnlyCollection<IWebElement>” and deliver it back to Locator 
of “LinkByParent.Title” to get the unique IWebElement identified by “LinkByParent.DownArrow” and click it.

Because the “filters” is sticky (the Locator consuming it would keep it until another non-null 
“filters” is provided), the next two sentences would highlight both the container (ResultItemFragment 
identified by “ListItemAllByClass.g”) and another sibling IWebElement (by AnyByCustom.LinkAddress) 
as pictures below, and you can find more explanation from Google Search Example: Filters with Buffering.



The following two sentences are used to show more practical use of matching IWebElement by conditions 
of its sibling within the same container discussed in Google Search Example: Filters Involving Other IWebElement:

Page.CurrentPage[GooglePage.ResultItemFragment.AnyByCustom.LinkAddress, null,  
  (e)=>e.HasChildOfText(GooglePage.ResultItemFragment.AnyByCustom.LinkAddress,"stackoverflow.com")] = "highlight";
Page.CurrentPage[GooglePage.ResultItemFragment.LinkByParent.Title] = "click";
Because type name of “AnyByCustom” doesn’t contain “All” like “AnyAllByCustom”, thus “AnyByCustom.LinkAddress”
would simply ignore any filters within its FindElement(), the provided “filters” 
((e)=>e.HasChildOfText(GooglePage.ResultItemFragment.AnyByCustom.LinkAddress,"stackoverflow.com") is actually 
consumed by the container identified by “ListItemAllByClass.g”, and the extended methods 
HasChildOfText(Enum childEnum, string partialText) and HasChildOfLink(Enum childEnum, string partialText) 
encapsulate the logic needed to find an IWebElement based on a child of it. So instead of comparing all 
text of a container, only concerned part is used to increase matching efficiency and accuracy.

The first sentence would highlight the link address of a result item as figure below shows, and then 
perform clicking on the actual link component above it.


These two-step procedure is not necessary if you do not need to indicate the source of the matching: 
you can use only one sentence to perform clicking on a result item based on matching with another one 
associated just like this:

Page.CurrentPage[GooglePage.ResultItemFragment.LinkByParent.Title, 
  null,(e) => e.HasChildOfText(GooglePage.ResultItemFragment.AnyByCustom.LinkAddress,"code.google")] = "controlclick";
It would open the web page in another tab, whose link address contains "code.google".

In case we need to perform operations on one IWebElement of a similar collection associated with a Locator, 
and it is also contained by another collective container, then actually we need to combine two predicates 
together as “filters” used in the Indexer as discussed in Google Search Example: Filters for Multiple Locators.
The related codes is listed here:

Func<IWebElement, bool> predicate = (e) =>
{   
    string elementClass = e.GetAttribute("class");
    return (elementClass=="g" && e.HasChildOfText(GooglePage.ResultItemFragment.AnyByCustom.LinkAddress, 
      "stackoverflow.com") || (elementClass.Contains("action-menu-item") &&e.Text == "Similar"));};

Page.CurrentPage[GooglePage.ResultItemFragment.LinkByParent.DownArrow, null, predicate] = "click";
Page.CurrentPage[GooglePage.ResultItemFragment.ListItemAllByClass.action_menu_item, null, predicate] = "tryhighlight";
if (Page.LastTrySuccess)
    Page.CurrentPage[GooglePage.ResultItemFragment.ListItemAllByClass.action_menu_item] = "click";
Since the action menu item “Similar” does exist, so it is highlighted and then clicked, as the picture below shows.
