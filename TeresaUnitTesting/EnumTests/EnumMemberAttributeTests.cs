using System;
using NUnit.Framework;
using Teresa;

namespace TeresaUnitTesting.EnumTests
{
    [TestFixture]
    public class EnumMemberAttributeTests
    {
        [Test]
        public void EnumFullNameTest()
        {
            string fullName = Fragment.ButtonByClass.class11.FullName();
            Assert.AreEqual(fullName, "Fragment.ButtonByClass.class11");
        }

        [Test]
        public void EnumMemberAttribute_NoExplictDefinition1_AsExpected()
        {
            EnumMemberAttribute memberAttribute = EnumMemberAttribute.EnumMemberAttributeOf(ButtonById.button1);
            Console.WriteLine("CSS: " + memberAttribute.Css);
            Assert.IsTrue(memberAttribute.Css.Contains("button#button1"));
            Assert.IsNotNullOrEmpty(memberAttribute.Description);
        }

        [Test]
        public void EnumMemberAttribute_WithValueDefined_AsExpected()
        {
            EnumMemberAttribute memberAttribute = EnumMemberAttribute.EnumMemberAttributeOf(Fragment.ButtonById.button1);
            Console.WriteLine("CSS: " + memberAttribute.Css);
            Assert.IsTrue(memberAttribute.Css.Contains("button#otherId"));
        }

        [Test]
        public void EnumMemberAttribute_NoExplictDefinition2_AsExpected()
        {
            EnumMemberAttribute memberAttribute = EnumMemberAttribute.EnumMemberAttributeOf(Fragment.ButtonByClass.class11);
            Console.WriteLine("CSS: " + memberAttribute.Css);
            Assert.IsTrue(memberAttribute.Css.Contains("button.class11"));
            Assert.IsNotNullOrEmpty(memberAttribute.Description);
        }

        [Test]
        public void EnumMemberAttribute_FullyDefined_AsExpected()
        {
            EnumMemberAttribute memberAttribute = EnumMemberAttribute.EnumMemberAttributeOf(ButtonById.button2);
            Console.WriteLine("CSS: " + memberAttribute.Css);
            Assert.IsTrue(memberAttribute.Css.Contains("button#actualButton2Id"));
            Assert.AreEqual(memberAttribute.Description, "User defined description");
        }

        [Test]
        public void EnumMemberAttribute_PartialDefined_AsExpected()
        {
            EnumMemberAttribute memberAttribute = EnumMemberAttribute.EnumMemberAttributeOf(RadioByCustom.radio1);
            Console.WriteLine("CSS: " + memberAttribute.Css);
            Assert.AreEqual(memberAttribute.Css, "input[type=radio]#radio1_Id");
            Assert.AreEqual(memberAttribute.IsFragment, false);
            Assert.IsNotNullOrEmpty(memberAttribute.Description);
        }

        [Test]
        public void EnumMemberAttribute_DifferentiateMemberOfSameTypeName_CssDifferent()
        {
            EnumMemberAttribute memberAttribute = EnumMemberAttribute.EnumMemberAttributeOf(ButtonById.button1);
            Console.WriteLine("CSS: " + memberAttribute.Css);
            Assert.IsTrue(memberAttribute.Css.Contains("button#button1"));

            EnumMemberAttribute memberAttribute2 = EnumMemberAttribute.EnumMemberAttributeOf(Fragment.ButtonById.button1);
            Console.WriteLine("CSS: " + memberAttribute2.Css);
            Assert.IsTrue(memberAttribute2.Css.Contains("button#otherId"));
        }

        [Test]
        public void EnumMemberAttribute_MembersOfSameTypeAndEntryName_WithDifferentValues()
        {
            string value1 = ButtonById.button1.Value();
            Console.WriteLine("Value of ButtonById.button1: " + value1);

            string value2 = Fragment.ButtonById.button1.Value();
            Console.WriteLine("Value of Fragment.ButtonById.button1: " + value2);
            Assert.AreNotEqual(value1, value2);
        }

        [Test]
        public void EnumMemberAttribute_ValidateCaching()
        {
            Assert.IsFalse(EnumMemberAttribute.CachedEnumMemberAttributes.ContainsKey(Fragment.ButtonByClass.class22));
            EnumMemberAttribute memberAttribute = EnumMemberAttribute.EnumMemberAttributeOf(Fragment.ButtonByClass.class22);
            Console.WriteLine("CSS: " + memberAttribute.Css);

            Assert.IsTrue(EnumMemberAttribute.CachedEnumMemberAttributes.ContainsKey(Fragment.ButtonByClass.class22));
            EnumMemberAttribute memberAttribute2 = EnumMemberAttribute.EnumMemberAttributeOf(Fragment.ButtonByClass.class22);
            Assert.AreEqual(memberAttribute, memberAttribute2);
        }
    }
}
