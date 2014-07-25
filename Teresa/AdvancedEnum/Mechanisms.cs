namespace Teresa
{
    /// <summary>
    /// This Enum defines common CSS Select mechanisms using in this library.
    /// A good introduction can be found in http://code.tutsplus.com/tutorials/the-30-css-selectors-you-must-memorize--net-16048
    /// The use of the Mechanisms combined with HtmlTagName can be found in 
    /// </summary>
    public enum Mechanisms
    {
        //Common Mechanisms, in order of efficiency preference
        //Comments: the {T}, {A}, {A0}, {A1} appeared below is placeholder of the Values of the EnumMemberAttribute
        //          {T} is the tag name determined by the HtmlTagName
        //          {A} is specified by the EnumMemberAttribute.Value() where there is no "=" 
        //          {A0} is specified by the first part of EnumMemberAttribute.Value() before "="
        //          {A1} is optional second part of EnumMemberAttribute.Value() after "="
        ById,           //{T}#{A}: Using Id for element finding
        ByClass,        //{T}.{A}: Using Classname for element finding

        ByAdjacent,     //{A}+{T}: Select only the element {T} that is immediately preceded by the former element {A}
        ByParent,       //{A}>{T}: Finding tag {T} that is direct child of some parent tag {A}
        ByAncestor,     //{A} {T}: Finding tag {T} that is descendant of some parent tag {A}
        BySibling,      //{A}~{T}: Select only the element {T} that is preceded by the former element {A}

        ByUrl,         //{T}[href*={A}]: Selects tag with link contains keyword, equals to ByAttribute when the attribute is "href"
        ByName,         //{T}[name*={A}]: Selects tag with name contains keyword, equals to ByAttribute when the attribute is "name"
        ByValue,        //{T}[value*={A}]: Selects tag with value contains keyword, equals to ByAttribute when the attribute is "value"
            
        ByAttribute,    //{T}[{A}]: the [{A}] can also represent any selector of below:
                        //      [{A0}] , [{A0}={A1}] , [{A0}*={A1}] , [{A0}^={A1}] , [{A0}$={A1}] , [{A0}~={A1}] , [{A0}-*={A1}] means
                        //      with attribute, match exactly, contains, starts with, ends with, with value of, names starts with A0 resepectively
        ByAttr,         //Shorthand of ByAttribute

        //Discussion of nth-of-type and nth-child: http://css-tricks.com/the-difference-between-nth-child-and-nth-of-type/
        ByOrder,        //{A0} {T}:nth-of-type({A1}): Selects Nth(from 1) of the type
        ByOrderLast,    //{A0} {T}:nth-last-of-type({A1}): Selects, from the end, Nth(from 1) of the type
        ByChild,        //{A0} {T}:nth-child({A1}): By Nth(from 1) of the nested children, 
        ByChildLast,    //{A0} {T}:nth-last-child({A1}): Selects, from the end, Nth(from 1) of the nested children, 

        ByCustom,       //{A}: All css text definded by Value() of EnumMemberAttribute

        ByTag,          //{T}: Use only the default tagnames to locate IWebElement
        ByText,         //Using LINQ instead to select the intended element
    }
}