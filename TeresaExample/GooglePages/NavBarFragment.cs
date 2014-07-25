using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Teresa;

namespace TeresaExample.GooglePages
{
    public class NavBarFragment : Fragment
    {
        public enum DivById
        {
            [EnumMember("gb", true)]
            TopBar
        }

        public enum LinkByAttr
        {
            [EnumMember("href=plus")]
            Plus,
            [EnumMember("href=mail")]
            Gmail,
            [EnumMember("href=img")]
            Images,
            [EnumMember("href=options")]
            Options,
            [EnumMember("href=Login")]
            Login
        }
    }
}
