using System.ComponentModel;
using NUnit.Framework;
using Teresa;

namespace TeresaUnitTesting.EnumTests
{
    [TestFixture]
    public class EnumTypeAttributeTests
    {
        [Test]
        public void EnumTypeParsing_ValidMechnismAndTag1_Success()
        {
            EnumTypeAttribute typeAttribute = EnumTypeAttribute.TypeAttributeOf(typeof (ButtonById));
            Assert.AreEqual(typeAttribute.IsCollection, false);
            Assert.AreEqual(typeAttribute.Mechanism, Mechanisms.ById);
            Assert.AreEqual(typeAttribute.TagName, HtmlTagName.Button);
        }

        [Test]
        public void EnumTypeParsing_TextAll_IsCollection()
        {
            EnumTypeAttribute typeAttribute = EnumTypeAttribute.TypeAttributeOf(typeof (TextAllByClass));
            Assert.AreEqual(typeAttribute.IsCollection, true);
            Assert.AreEqual(typeAttribute.Mechanism, Mechanisms.ByClass);
            Assert.AreEqual(typeAttribute.TagName, HtmlTagName.Text);
        }

        [Test]
        public void EnumTypeParsing_InvalidMechnismAndTag_ExceptionThrown()
        {
            Assert.Catch<InvalidEnumArgumentException>(
                () => EnumTypeAttribute.TypeAttributeOf(typeof (InvalidCombination))
            );
        }

        [Test]
        public void EnumTypeParsing_ValidateCachingOnTypeName_Works()
        {
            EnumTypeAttribute typeAttribute = EnumTypeAttribute.TypeAttributeOf(typeof(ButtonById));
            EnumTypeAttribute typeAttribute2 = EnumTypeAttribute.TypeAttributeOf(typeof(Fragment.ButtonById));
            Assert.AreEqual(typeAttribute, typeAttribute2);
        }
    }
}
