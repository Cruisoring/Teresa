using System;
using NUnit.Framework;
using Teresa;

namespace TeresaUnitTesting.EnumTests
{
    [TestFixture]
    public class EnumExtensionTests
    {
        [Test]
        public void EnumExtension_RegisterValue_AsExpected()
        {
            Assert.IsFalse(EnumExtension.EnumValues.ContainsKey(InvalidCombination.Just_for_test));
            EnumExtension.RegisterEnumValue(InvalidCombination.Just_for_test, "Just for test");
            Assert.IsTrue(EnumExtension.EnumValues.ContainsKey(InvalidCombination.Just_for_test));
            Assert.AreEqual(InvalidCombination.Just_for_test.Value(), "Just for test");
        }

        [Test]
        public void EnumExtension_AfterUsingEnumTypeAttribute_MechanismValuesRegistered()
        {
            EnumTypeAttribute typeAttribute = EnumTypeAttribute.TypeAttributeOf(typeof(ButtonById));
            Assert.IsTrue(EnumExtension.EnumValues.ContainsKey(Mechanisms.ByClass));
        }

        [Test]
        public void ValidateEnumValue_Implicit()
        {
            string enumValue = ButtonById.button1.Value();
            Console.WriteLine("enumValue of ButtonById.button1: " + enumValue);
            Assert.IsTrue(enumValue.Contains("button1"));
        }

        [Test]
        public void ValidateEnumValue_Explicit()
        {
            string enumValue = ButtonById.button2.Value();
            Console.WriteLine("enumValue of ButtonById.button2: " + enumValue);
            Assert.IsTrue(enumValue.Contains("actualButton2Id"));
        }

        [Test]
        public void ValidateEnumValue_ByCustom()
        {
            string enumValue = RadioByCustom.radio1.Value();
            Console.WriteLine("enumValue of RadioByCustom.radio1: " + enumValue);
            Assert.AreEqual(enumValue, "input[type=radio]#radio1_Id");
        }

        [Test]
        public void EnumExtension_OfSameEnumTypeName_DifferentValue()
        {
            string enumValue = ButtonById.button1.Value();
            Console.WriteLine("enumValue of ButtonById.button1: " + enumValue);
            string enumValue2 = TeresaUnitTesting.EnumTests.Fragment.ButtonById.button1.Value();
            Console.WriteLine("enumValue of Fragment.ButtonById.button1: " + enumValue2);
            Assert.AreNotEqual(enumValue, enumValue2);
        }
    }

}
