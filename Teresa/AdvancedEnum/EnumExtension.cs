using System;
using System.Collections.Generic;
using System.Text;

namespace Teresa
{
    public static class EnumExtension
    {
        public static readonly Dictionary<Enum, string> EnumValues = new Dictionary<Enum, string>();

        public static string TypeAndName(this Enum theEnum)
        {
            string fullName = theEnum.FullName();
            int index = fullName.LastIndexOf('.');
            return fullName.Substring(index + 1);
        }

        public static string FullName(this Enum theEnum)
        {
            if (!EnumMemberAttribute.EnumMemberFullNames.ContainsKey(theEnum))
            {
                //EnumMemberAttribute.EnumMemberFullNames.Add(theEnum, 
                //    string.Format("{0}.{1}", theEnum.GetType().FullName, theEnum));

                Type theType = theEnum.GetType();
                StringBuilder sb = new StringBuilder();
                do
                {
                    sb.Insert(0, theType.Name+".");
                    theType = theType.DeclaringType;
                } while (theType!=null);
                sb.Append(theEnum);
                return sb.ToString();
            }
            return EnumMemberAttribute.EnumMemberFullNames[theEnum];
        }

        public static string Value(this Enum theEnum)
        {
            if (EnumValues.ContainsKey(theEnum))
                return EnumValues[theEnum];

            EnumMemberAttribute memberAttribute = EnumMemberAttribute.EnumMemberAttributeOf(theEnum);
            if (!EnumValues.ContainsKey(theEnum))
            {
                EnumValues.Add(theEnum, memberAttribute.Value);
            }
            return EnumValues[theEnum];
        }

        public static void RegisterEnumValue(Enum theEnum, string theValue)
        {
            if (EnumValues.ContainsKey(theEnum))
                throw new Exception("The value of " + theEnum + " cannot be registered twice!");

            EnumValues.Add(theEnum, theValue);
        }

        public static string DefaultValue(this Enum theEnum)
        {
            return theEnum.ToString().Replace('_', ' ');
        }

        public static void ChangeValue(this Enum theEnum, string valueString)
        {
            if (!EnumValues.ContainsKey(theEnum))
            {
                throw new Exception("There is no record for " + theEnum);
            }
            EnumValues[theEnum] = valueString;
        }

        public static string Description(this Enum theEnum)
        {
            EnumMemberAttribute memberAttribute = EnumMemberAttribute.EnumMemberAttributeOf(theEnum);
            return memberAttribute.Description;
        }

        public static bool IsFragment(this Enum theEnum)
        {
            EnumMemberAttribute memberAttribute = EnumMemberAttribute.EnumMemberAttributeOf(theEnum);
            return memberAttribute.IsFragment;
        }

        public static bool IsCollection(this Enum theEnum)
        {
            EnumMemberAttribute memberAttribute = EnumMemberAttribute.EnumMemberAttributeOf(theEnum);
            return memberAttribute.IsCollection;
        }

        public static string Css(this Enum theEnum)
        {
            EnumMemberAttribute memberAttribute = EnumMemberAttribute.EnumMemberAttributeOf(theEnum);
            return memberAttribute.Css;
        }

        private static bool isFragment(Type theType)
        {
            string fragmentName = Fragment.FragmentType.Name;
            if (theType.Name == fragmentName)
                return true;

            Type baseType = theType;
            do
            {
                if (baseType.Name == fragmentName)
                    return true;
                baseType = baseType.BaseType;
            } while (baseType != null);
            return false;
        }

        public static string FullCss(this Enum theEnum)
        {
            try
            {
                StringBuilder sb = new StringBuilder(theEnum.Css());
                Type enumType = theEnum.GetType();
                Type declaringType = enumType.DeclaringType;
                do
                {
                    if (!isFragment(declaringType))
                        break;

                    Enum id = Fragment.GetFragmentId(declaringType);
                    if (id==null || id.Equals(Locator.HtmlByCustom.Root))
                        break;
                    
                    sb.Insert(0, id.Css() + " ");

                    declaringType = declaringType.DeclaringType;
                } while (declaringType != null);
                return sb.ToString();
            }
            catch (Exception)
            {
                return null;
            }
        }

        public static HtmlTagName TagName(this Enum theEnum)
        {
            EnumMemberAttribute memberAttribute = EnumMemberAttribute.EnumMemberAttributeOf(theEnum);
            return memberAttribute.TagName;
        }

        public static Mechanisms Mechanism(this Enum theEnum)
        {
            EnumMemberAttribute memberAttribute = EnumMemberAttribute.EnumMemberAttributeOf(theEnum);
            return memberAttribute.Mechanism;
        }

        //static EnumExtensions()
        //    DefaultRegisterEnumType(typeof(Mechanisms));
        //}
    }
}
