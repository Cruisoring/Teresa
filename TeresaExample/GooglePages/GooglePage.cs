using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using OpenQA.Selenium;
using Teresa;

namespace TeresaExample.GooglePages
{
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
            public enum ListItemAllByClass
            {
                [EnumMember(true)]
                g,
                [EnumMember("action-menu-item")]
                action_menu_item
            }

            public enum LinkByParent
            {
                [EnumMember("h3")]
                Title,

                //This is the Arrow right of the LinkAddress
                [EnumMember("div.action-menu")]
                DownArrow
            }

            public enum AnyByCustom
            {
                //Notice that the value starts with " " to compose CSS of "* cite" finally
                [EnumMember(" cite")]
                LinkAddress
            }

            public enum SpanByClass
            {
                [EnumMember("st")]
                Description
            }
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

        public override void ParseQuery(Uri uri)
        {
            QueryValues = HttpUtility.ParseQueryString(uri.Query+uri.Fragment.Replace("#", "&"), UriEncoding);
        }

        public static string GetGoogleLinkUrl(IWebElement element)
        {
            string href = element.GetAttribute("href");
            if (href == null)
                throw new NullReferenceException("failed to extract href from " + element.TagName);

            Uri uri = new Uri(href);
            var querys = HttpUtility.ParseQueryString(uri.Query, Encoding.UTF8);
            return querys["url"]??"";
        }

    }
}
