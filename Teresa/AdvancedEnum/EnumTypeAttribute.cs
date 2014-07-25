using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Text;
using System.Threading.Tasks;

namespace Teresa
{
    [AttributeUsage(AttributeTargets.Enum, Inherited = false, AllowMultiple = false), Serializable]
    public class EnumTypeAttribute : Attribute
    {
        public const string CollectionIndicator = "All";
        public const string By = "By";

        public static Dictionary<string, EnumTypeAttribute> CachedEnumTypeAttributes = new Dictionary<string, EnumTypeAttribute>();

        public static EnumTypeAttribute TypeAttributeOf(Type enumType)
        {
            string typeName = enumType.Name;
            if (!CachedEnumTypeAttributes.ContainsKey(typeName))
            {
                CachedEnumTypeAttributes.Add(typeName, ParseTypeName(typeName));
            }
            return CachedEnumTypeAttributes[typeName];
        }

        private static EnumTypeAttribute ParseTypeName(string enumTypeName)
        {
            int byPosition = enumTypeName.IndexOf(By, StringComparison.OrdinalIgnoreCase);
            if (byPosition == -1)
                throw new InvalidEnumArgumentException(
                    "The type name of enumId must be a pattern of ***By*** for further processing");

            string prefix = enumTypeName.Substring(0, byPosition);
            string suffix = enumTypeName.Substring(byPosition);

            bool isCollection = false;
            HtmlTagName tagName = HtmlTagName.Any;

            if (!string.IsNullOrEmpty(prefix))
            {
                if (prefix.EndsWith(CollectionIndicator))
                {
                    isCollection = true;
                    prefix = prefix.Substring(0, prefix.Length - CollectionIndicator.Length);
                }
                if (!Enum.TryParse(prefix, true, out tagName))
                {
                    throw new Exception("ElementWrapper type of " + prefix + " is not supported yet.");
                }
                //if (tagName.Equals(HtmlTagName.Radio))
                //    isCollection = true;
            }

            Mechanisms mechanism = Mechanisms.ById;
            if (!Enum.TryParse(suffix, true, out mechanism))
            {
                var values = Enum.GetValues(typeof(Mechanisms)).OfType<Enum>().ToList();
                StringBuilder sb = new StringBuilder();
                foreach (var theEnum in values)
                {
                    sb.AppendFormat("{0}, ", theEnum);
                }
                string MechanismOptions = sb.ToString().Trim(new char[] { ' ', ',' });

                throw new Exception(string.Format("The valid Enum Type name must be ended with one of:\r\n" + MechanismOptions));
            }

            return new EnumTypeAttribute(isCollection, mechanism, tagName);
        }

        static EnumTypeAttribute()
        {
            #region Registering Default CSS construction formats needed by different Mechanism
            //{T}#{A}: Using SectionByAttr as finding mechanism.
                //CSS example: E#myid:	Matches any E element with ID equal to "myid".
            EnumExtension.RegisterEnumValue(Mechanisms.ById, "#{0}");
            //{T}.{A}: Using Classname for finding
                //CSS example: DIV.warning	Language specific. (In SinglePage, the same as DIV[class~="warning"].)
            EnumExtension.RegisterEnumValue(Mechanisms.ByClass, ".{0}");
            //{T}: Use only the default tagnames to locate IWebElement
            EnumExtension.RegisterEnumValue(Mechanisms.ByTag, "");


            //{A}+{T}: Select the element {T} sharing same parent and is immediately preceded by the former element {A}
                //CSS example: E + F	Matches any F element that is immediately following an element E.
            EnumExtension.RegisterEnumValue(Mechanisms.ByAdjacent, "{0}+{1}");
            //{A}>{T}: Finding tag {T} that is direct child of some parent tag {A}
                //CSS example: E F	Matches any F element that is a descendant of an E element.
            EnumExtension.RegisterEnumValue(Mechanisms.ByAncestor, "{0} {1}");
            //{A} {T}: Finding tag {T} that is descendant of some parent tag {A}
                //CSS example: E > F	Matches any F element that is a child of an element E.
            EnumExtension.RegisterEnumValue(Mechanisms.ByParent, "{0}>{1}");
            //{A}~{T}: Select only the element {T} that is preceded by the former element {A}
                //CSS example: E ~ F	Matches any F element that is following an element E.
            EnumExtension.RegisterEnumValue(Mechanisms.BySibling, "{0}~{1}");

            //[href*={A}]: Selects tag with link contains keyword, equals to ByAttribute when the attribute is "href"
                //CSS example: E[href*='foo']	Matches any E element that points to a link containing 'foo'.
            EnumExtension.RegisterEnumValue(Mechanisms.ByUrl, "[href*='{0}']");
            //[name*={A}]: Selects tag with name contains keyword, equals to ByAttribute when the attribute is "name"
                //CSS example: E[name*="warning"]	Matches any E element whose name attribute contains "warning".
            EnumExtension.RegisterEnumValue(Mechanisms.ByName, "[name*='{0}']");
            //[value*={A}]: : Selects tag with value contains keyword, equals to ByAttribute when the attribute is "value"
                //CSS example: E[value*="warning"]	Matches any E element whose value contains "warning".
            EnumExtension.RegisterEnumValue(Mechanisms.ByValue, "[value*='{0}']");

            //{T}[{A}]: the [{A}] can also represent any selector of below:
            //      [{A0}] , [{A0}={A1}] , [{A0}*={A1}] , [{A0}^={A1}] , [{A0}$={A1}] , [{A0}~={A1}] , [{A0}-*={A1}] means
            //      with attribute, match exactly, contains, starts with, ends with, with value of, names starts with A0 resepectively
                //CSS example:  E[foo]	Matches E element with "foo" attribute.
                //              E[foo='key'] Matches E element whose "foo" attribute equals to "key".
                //              E[foo*='key'] Matches E element whose "foo" attribute contains "key".
                //              E[foo^='key'] Matches E element whose "foo" attribute starts with "key".
                //              E[foo$='key'] Matches E element whose "foo" attribute ends with "key".
                //              E[foo~='key'] Matches E element whose "foo" attribute include a value of "key" seperated by ' ' with other values.
                //              E[foo-*='key'] Matches E element who has attribute starts with "foo" and its value equals to "key".
            EnumExtension.RegisterEnumValue(Mechanisms.ByAttribute, "[{0}='{1}']");
            //Shorthand of ByAttribute
            EnumExtension.RegisterEnumValue(Mechanisms.ByAttr, "[{0}='{1}']");

            //Discussion of nth-of-type and nth-child: http://css-tricks.com/the-difference-between-nth-child-and-nth-of-type/
            //{A0}:nth-of-type({A1}) {T}: Selects tag of {T} whose container is Nth(from 1) of the type {A0}
                //CSS example: E:nth-of-type(3)	Matches the 3rd element of E type. {0} = tagname of the container, {1} sequence number
            EnumExtension.RegisterEnumValue(Mechanisms.ByOrder, "{0}:nth-of-type({1})");
            //{A0}:nth-last-of-type({A1}) {T}: Selects tag of {T} whose container is Nth(from end) of the type {A0}
            //CSS example: E:nth-last-of-type(3)	Matches the 3rd element of E type. {0} = tagname of the container, {1} sequence number
            EnumExtension.RegisterEnumValue(Mechanisms.ByOrderLast, "{0}:nth-last-of-type({1})");
            //{A0}:nth-child({A1}) {T}: Selects tag of {T} whose container is Nth(from 1) of its parent, AND the type is {A0}
            EnumExtension.RegisterEnumValue(Mechanisms.ByChild, "{0}:nth-child({1})");
            //{A0}:nth-last-child({A1}) {T}: Select tag of {T} whose container is Nth(from end) of its parent, AND the type is {A0}
            EnumExtension.RegisterEnumValue(Mechanisms.ByChildLast, "{0}:nth-last-child({1})");

            EnumExtension.RegisterEnumValue(Mechanisms.ByText, "");
            EnumExtension.RegisterEnumValue(Mechanisms.ByCustom, "{0}");
            #endregion

            #region Registering Default CSS construction formats needed by different Tag types

            //Associated with "*" or EMPTY. The star symbol will target every single element on the page
            // and adds too much weight on the browser, thus it shall always be avoided.
            EnumExtension.RegisterEnumValue(HtmlTagName.Any, "{0} {1}");

            EnumExtension.RegisterEnumValue(HtmlTagName.Html, "html");
            EnumExtension.RegisterEnumValue(HtmlTagName.Title, "title");
            EnumExtension.RegisterEnumValue(HtmlTagName.Body, "body");

            EnumExtension.RegisterEnumValue(HtmlTagName.Text, "{0} input[type]{1}, {0} textarea{1}");
            EnumExtension.RegisterEnumValue(HtmlTagName.Button,
                "{0} button{1}, {0} input[type=button]{1}, {0} input.btn{1}, {0} input.button{1}, {0} div[role=button]{1}");
            EnumExtension.RegisterEnumValue(HtmlTagName.Link, "{0} a{1}");
            EnumExtension.RegisterEnumValue(HtmlTagName.Image, "{0} img{1}");
            EnumExtension.RegisterEnumValue(HtmlTagName.Paragraph, "{0} p{1}");

            EnumExtension.RegisterEnumValue(HtmlTagName.List, "{0} ol{1}, {0} ul{1}");
            EnumExtension.RegisterEnumValue(HtmlTagName.ListItem, "{0} li{1}");

            EnumExtension.RegisterEnumValue(HtmlTagName.TableHead, "{0} thead{1}");
            EnumExtension.RegisterEnumValue(HtmlTagName.TableBody, "{0} tbody{1}");
            EnumExtension.RegisterEnumValue(HtmlTagName.TableFoot, "{0} tfoot{1}");
            EnumExtension.RegisterEnumValue(HtmlTagName.TableRow, "{0} tr{1}");
            EnumExtension.RegisterEnumValue(HtmlTagName.TableCell, "{0} td{1}");
            EnumExtension.RegisterEnumValue(HtmlTagName.TableHeadCell, "{0} th{1}");


            //EnumExtension.RegisterEnumValue(HtmlTagName.Textarea, "{0} textarea{1}");
            EnumExtension.RegisterEnumValue(HtmlTagName.Radio, "{0} input[type=radio]{1}");
            EnumExtension.RegisterEnumValue(HtmlTagName.Checkbox, "{0} input[type=checkbox]{1}");  //Preferrable "{0} input{1}"
            EnumExtension.RegisterEnumValue(HtmlTagName.Check, "{0} input[type=checkbox]{1}");  //Preferrable "{0} input{1}"

            var htmlTagNames = Enum.GetValues(typeof(HtmlTagName)).OfType<HtmlTagName>();
            foreach (HtmlTagName htmlTagName in htmlTagNames)
            {
                if (!EnumExtension.EnumValues.ContainsKey(htmlTagName))
                    EnumExtension.RegisterEnumValue(htmlTagName,
                        "{0} " + htmlTagName.ToString().ToLowerInvariant() + "{1}");
            }
            #endregion
        }

        public bool IsCollection { get; protected set; }
        public Mechanisms Mechanism { get; protected set; }
        public HtmlTagName TagName { get; protected set; }

        public override string ToString()
        {
            return string.Format("{0}{1}", TagName, Mechanism);
        }

        public EnumTypeAttribute(bool isCollection, Mechanisms mechanism, HtmlTagName tagName)
        {
            IsCollection = isCollection;
            Mechanism = mechanism;
            TagName = tagName;
        }

    }
}
