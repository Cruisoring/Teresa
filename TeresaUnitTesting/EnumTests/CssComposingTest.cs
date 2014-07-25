using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using Teresa;

namespace TeresaUnitTesting.EnumTests
{

    public class SamplePage : Page {
        public class TopFragment : Fragment {
            public enum DivByClass { [EnumMember(true)]topbar }
            public enum LinkById { linkHome, back, [EnumMember("fwd")]forward }
            public class LeftTopFragment : Fragment{
                public enum SectionByClass { [EnumMember(true)]left}
                public enum ImageByName { Previous, Next }
            }
        }
        public class BottomFragment : Fragment {
            public enum FormByAttr { [EnumMember("someAttr=xyz", true)]bottomNav}
            public enum LinkById { linkHome, back, [EnumMember("fwd")]forward }
        }
        public override Uri SampleUri
        {
            get { return null; }
        }
    }

    [TestFixture]
    public class CssComposingTest
    {
        private IEnumerable<Mechanisms> allMechanismses = Enum.GetValues(typeof (Mechanisms)).OfType<Mechanisms>();
        private IEnumerable<HtmlTagName> allTags = Enum.GetValues(typeof (HtmlTagName)).OfType<HtmlTagName>();

        [Test]
        public void Composing_WithoutValueSplitter()
        {
            int i = 0;
            string enumValue = "x";
            Console.WriteLine("When EnumValue = '{0}'", enumValue);
            foreach (var tag in allTags)
            {
                foreach (var mechanismse in allMechanismses)
                {
                    EnumTypeAttribute typeAttribute = new EnumTypeAttribute(false, mechanismse, tag);
                    EnumMemberAttribute memberAttribute = new EnumMemberAttribute(enumValue);
                    string css = EnumMemberAttribute.CssSelectorOf(typeAttribute, memberAttribute, enumValue);
                    Console.WriteLine(string.Format("{0, 3}) {1, -20} : {2}", i++, typeAttribute, css));
                }
            }
        }

        [Test]
        public void Composing_WithValueSplitter()
        {
            int i = 0;
            string enumValue = "A0=A1";
            Console.WriteLine("When EnumValue = '{0}'", enumValue);
            foreach (var tag in allTags)
            {
                foreach (var mechanismse in allMechanismses)
                {
                    EnumTypeAttribute typeAttribute = new EnumTypeAttribute(false, mechanismse, tag);
                    EnumMemberAttribute memberAttribute = new EnumMemberAttribute(enumValue);
                    string css = EnumMemberAttribute.CssSelectorOf(typeAttribute, memberAttribute, enumValue);
                    Console.WriteLine(string.Format("{0, 3}) {1, -20} : {2}", i++, typeAttribute, css));
                }
            }
        }

        [Test]
        public void CssComposing_MechanismOfByCustom()
        {
            int i = 0;
            string enumValue = " cite";
            Console.WriteLine("When EnumValue = '{0}'", enumValue);
            foreach (var tag in allTags)
            {
                Mechanisms mechanisms = Mechanisms.ByCustom;
                EnumTypeAttribute typeAttribute = new EnumTypeAttribute(false, mechanisms, tag);
                EnumMemberAttribute memberAttribute = new EnumMemberAttribute(enumValue);
                string css = EnumMemberAttribute.CssSelectorOf(typeAttribute, memberAttribute, enumValue);
                Console.WriteLine(string.Format("{0, 3}) {1, -20} : {2}", i++, typeAttribute, css));
            }
        }

        [Test]
        public void CssComposing_FullCss()
        {
            string topForwardCss = SamplePage.TopFragment.LinkById.linkHome.FullCss();
            string trimmed = topForwardCss.Replace("  ", " ").Trim();
            Assert.AreEqual(trimmed, "div.topbar a#linkHome");

            string nextCss = SamplePage.TopFragment.LeftTopFragment.ImageByName.Next.FullCss();
            trimmed = nextCss.Replace("  ", " ").Trim();
            Console.WriteLine(trimmed);
        }
    }
}
