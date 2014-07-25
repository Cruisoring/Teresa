using Teresa;

namespace TeresaUnitTesting.EnumTests
{
    public enum ButtonById
    {
        button1,
        [EnumMember("actualButton2Id", false, "User defined description")]
        button2
    }

    public enum TextAllByClass
    {
        class1Text,
        class2Text,
        Text_with_space
    }

    public enum RadioByCustom
    {
        [EnumMember("input[type=radio]#radio1_Id")]
        radio1
    }

    public enum InvalidCombination
    {
        Just_for_test
    }

    public class Fragment
    {
        public enum ButtonById
        {
            [EnumMember("otherId")]
            button1
        }
        public enum ButtonByClass
        {
            class11,
            class22
        }
    }
}
