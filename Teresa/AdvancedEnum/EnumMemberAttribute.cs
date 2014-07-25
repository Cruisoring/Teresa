using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Teresa
{
[AttributeUsage(AttributeTargets.Field, Inherited = false, AllowMultiple = false), Serializable]
public class EnumMemberAttribute : Attribute
{
    public static readonly char EnumValueSplitter = '=';
    public static readonly Dictionary<Enum, EnumMemberAttribute> CachedEnumMemberAttributes = new Dictionary<Enum, EnumMemberAttribute>();
    public static readonly Dictionary<Enum, string> EnumMemberFullNames = new Dictionary<Enum, string>();

    public static string DescriptionOf(EnumTypeAttribute typeAttribute, string enumValue)
    {
        return string.Format("{0}{1} Locator {2}: {3}", typeAttribute.IsCollection ? "All" : "",
            typeAttribute.TagName, typeAttribute.Mechanism, enumValue);
    }

    public static EnumMemberAttribute EnumMemberAttributeOf(Enum theEnum)
    {
        if (CachedEnumMemberAttributes.ContainsKey(theEnum))
            return CachedEnumMemberAttributes[theEnum];

        Type enumType = theEnum.GetType();
        if (!EnumMemberFullNames.ContainsKey(theEnum))
        {
            EnumMemberFullNames.Add(theEnum, string.Format("{0}.{1}", enumType.FullName, theEnum));
        }

        //Get the Type Attribute: IsCollection, Mechanism and HtmlTag
        EnumTypeAttribute typeAttribute = EnumTypeAttribute.TypeAttributeOf(enumType);
        EnumMemberAttribute memberAttribute = null;

        //Try to get the EnumMemberAttribute defined in codes, especially the Meaning of theEnum
        var fi = enumType.GetField(theEnum.ToString());
        var usageAttributes = GetCustomAttributes(fi, typeof(EnumMemberAttribute), false);
        int attributesCount = usageAttributes.Count();
        string enumValue = null;

        if (attributesCount == 0)
        {
            enumValue = theEnum.DefaultValue();
            memberAttribute = new EnumMemberAttribute(enumValue, false, DescriptionOf(typeAttribute, enumValue));
        }
        else if (attributesCount == 1)
        {
            EnumMemberAttribute attr = (EnumMemberAttribute)usageAttributes[0];
            enumValue = attr.Value ?? theEnum.DefaultValue();
            //if (enumValue != theEnum.DoString())
            //    theEnum.ChangeValue(enumValue);

            memberAttribute = new EnumMemberAttribute(enumValue,
                attr.IsFragment, attr.Description ?? DescriptionOf(typeAttribute, enumValue));
        }

        if (memberAttribute == null)
            throw new Exception("Unexpected situation when memberAttribute is null.");

        string css = CssSelectorOf(typeAttribute, memberAttribute, enumValue);
        memberAttribute = new EnumMemberAttribute(memberAttribute.Value, memberAttribute.IsFragment, memberAttribute.Description, css,
            typeAttribute.Mechanism, typeAttribute.IsCollection, typeAttribute.TagName);
        CachedEnumMemberAttributes.Add(theEnum, memberAttribute);
        return memberAttribute;
    }

    public static string CssSelectorOf(EnumTypeAttribute typeAttribute, EnumMemberAttribute memberAttribute, string memberValue)
    {
        HtmlTagName tagName = typeAttribute.TagName;
        string tagLocator = tagName.Value();
        Mechanisms mechanism = typeAttribute.Mechanism;
        string mechanismFormat = typeAttribute.Mechanism.Value();

        string css;
        switch (mechanism)
        {
            case Mechanisms.ByAttr:
            case Mechanisms.ByAttribute:
            {
                int index = memberValue.IndexOf(EnumValueSplitter);
                if (index == -1) //The Value of the Enum entry shall be treated as the attribute name
                {
                    string withAttr = string.Format("[{0}]", memberValue);
                    css = string.Format(tagLocator, string.Empty, withAttr);
                }
                else
                {
                    string attrString = string.Format(mechanismFormat, 
                        memberValue.Substring(0, index), memberValue.Substring(index+1));
                    css = string.Format(tagLocator, string.Empty, attrString);
                }
                break;
            }
            case Mechanisms.ByOrder:
            case Mechanisms.ByOrderLast:
            case Mechanisms.ByChild:
            case Mechanisms.ByChildLast:
            {
                string[] pair = memberValue.Split(EnumValueSplitter);

                string attrString = null;
                switch(pair.Count())
                {
                    case 2:
                        attrString = string.Format(mechanismFormat, string.Empty, pair[1]).Trim();
                        css = string.Format(tagLocator, pair[0], attrString);
                        break;
                    case 1:
                        attrString = string.Format(mechanismFormat, string.Empty, memberValue).TrimStart();
                        css = string.Format(tagLocator, string.Empty, attrString);
                        break;
                    default:
                        throw new InvalidEnumArgumentException(mechanismFormat +
                                                               " expects one item or two items seperated by '='.");
                }
                break;
            }
            case Mechanisms.ByText:
            case Mechanisms.ByTag:
            {
                //Notice: this is not preferrable with CSS Locator.
                //The value of the Enum member shall be used afterwards
                css = string.Format(tagLocator, string.Empty, string.Empty);
                break;
            }
            case Mechanisms.ByCustom:
            {
                css = memberValue;
                break;
            }
            case Mechanisms.ByAdjacent:
            case Mechanisms.ByAncestor:
            case Mechanisms.ByParent:
            case Mechanisms.BySibling:
            {
                string[] pair = memberValue.Split(EnumValueSplitter);

                string attrString = null;
                switch (pair.Count())
                {
                    case 2:
                        attrString = string.Format(mechanismFormat, pair[0], string.Empty);
                        css = string.Format(tagLocator, attrString, pair[1]);
                        break;
                    case 1:
                        attrString = string.Format(mechanismFormat, memberValue, string.Empty);
                        css = string.Format(tagLocator, attrString, string.Empty);
                        break;
                    default:
                        throw new InvalidEnumArgumentException(mechanismFormat +
                                                               " expects one item or two items seperated by '='.");
                }
                break;
            }
            default:
            //case Mechanisms.ById:
            //case Mechanisms.ByClass:
            //case Mechanisms.ByUrl:
            //case Mechanisms.ByName:
            //case Mechanisms.ByValue:
            {
                string[] pair = memberValue.Split(EnumValueSplitter);
                string byId = string.Format(mechanismFormat, pair.Last());
                css = string.Format(tagLocator, string.Empty, byId);
                break;
            }
        }
        return css;
    }

    private bool _isFragment;
    private string _value;
    private string description;
    private string _css;
    private bool isCollection;
    private HtmlTagName tagName;
    private Mechanisms mechanism;

    public bool IsFragment
    {
        get { return _isFragment; }
    }

    public string Value
    {
        get { return _value; }
    }

    public string Description
    {
        get { return description; }
    }

    public string Css
    {
        get { return _css; }
    }

    public bool IsCollection
    {
        get { return isCollection; }
    }

    public HtmlTagName TagName
    {
        get { return tagName; }
    }

    public Mechanisms Mechanism
    {
        get { return mechanism; }
    }

    public override string ToString()
    {
        return string.Format("{0}{1} '{2}'=>CSS({3})", tagName, mechanism, _value, _css);
    }


    public EnumMemberAttribute(bool _isFragment = false)
    {
        this._isFragment = _isFragment;
    }

    public EnumMemberAttribute(string valueString = null, bool _isFragment = false, string description = null)
    {
        this._isFragment = _isFragment;
        this._value = valueString;
        this.description = description;
    }

    protected EnumMemberAttribute(string valueString, bool _isFragment, string description, string css,
        Mechanisms mechanism, bool isCollection = false, HtmlTagName tag = HtmlTagName.Any)
    {
        this._isFragment = _isFragment;
        this._value = valueString;
        this.description = description;
        this._css = css;
        this.mechanism = mechanism;
        this.isCollection = isCollection;
        this.tagName = tag;
    }
}
}
