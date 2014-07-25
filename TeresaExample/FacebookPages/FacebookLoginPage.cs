using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Teresa;

namespace TeresaExample.Pages
{
public class FacebookLoginPage : Page
{
    public override Uri SampleUri
    {
        get { return new Uri("https://www.facebook.com"); }
    }

    public enum TextByName
    {
        firstname,
        lastname,
        [EnumMember("reg_email__")]
        regEmail,
        [EnumMember("reg_email_confirmation__")]
        regEmailConfirmation,
        [EnumMember("reg_passwd__")]
        regPassword
    }

    ////To show ByAncestor and "Check" can be treated as "Checkbox" 
    //public enum CheckByAncestor
    //{
    //    [EnumMember("td.login_form_label_field")]
    //    persistent
    //}

    //public enum LinkByAncestor
    //{
    //    [EnumMember("td.login_form_label_field")]
    //    forgetPassword
    //}

    public enum SelectById
    {
        month,
        day,
        year
    }

    //For demo purposes: to show how different Css Selector can be used to cope with complex cases
    public enum RadioByCustom
    {
        [EnumMember("[name='sex'][value='1']")]
        female,
        [EnumMember("span#u_0_g span:nth-of-type(2) input")]
        male
    }

    public enum LabelByText
    {
        Female,
        Male
    }

    //For demo purposes: ByText should always be avoided
    public enum ButtonByText
    {
        [EnumMember("Sign Up")]
        SignUp
    }
}
}
