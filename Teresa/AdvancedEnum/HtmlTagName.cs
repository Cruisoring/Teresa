namespace Teresa
{
    public enum HtmlTagName
    {
        Any,    //Associated with "*". The star symbol will target every single element on the page
                // and adds too much weight on the browser, thus it shall always be avoided.

        Html,
        Head,
        Title,
        Body,

        Text,
        Image,
        Img,
        Button,
        Link,
        A,
        Select,
        Label,

        Checkbox,
        Check,  //Shorthand of "Checkbox".

        Radio,

        Div,
        Span,
        Section,
        Paragraph,
        P,
        Fieldset,

        ListItem,
        List,   //Either "ol" or "ul"

        Header,
        H1,
        H2,
        H3,
        H4,
        H5,
        H6,

        Form,

        Table,
        TableHead,
        Thead,
        TableBody,
        Tbody,
        TableFoot,
        Tfoot,
        TableRow,
        Tr,
        TableCell,
        Td,
        TableHeadCell,
        Th
    }
}