using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Teresa;

namespace TeresaExample.GooglePages
{
    public class SearchNavFragment : Fragment
    {
        public enum DivById
        {
            [EnumMember(true)]
            hdtb
        }

        public enum DivByCustom
        {
            [EnumMember(".hdtb_mitem:nth-of-type(1)")]
            Web,
            [EnumMember(".hdtb_mitem:nth-of-type(2)")]
            Images,
            [EnumMember(".hdtb_mitem:nth-of-type(3)")]
            Videos,
            [EnumMember(".hdtb_mitem:nth-of-type(4)")]
            Shopping,
            [EnumMember(".hdtb_mitem:nth-of-type(5)")]
            News,
            [EnumMember(".hdtb_mitem:nth-of-type(6)")]
            More,
            [EnumMember(".hdtb_mitem:nth-of-type(7)")]
            SearchTools
        }

        /// <summary>
        /// Alternative ways to locate the menus with their text.
        /// However, it is not preferred due to: 
        /// 1) it is least efficient compared with other mechanisms.
        /// 2) The displayed text might varied with different language/other settings.
        /// 3) The link may not appear in the page. For example, when "Web" is selected, the
        /// tag whose tagname is "a" is removed from its "div" parent element.
        /// </summary>
        public enum LinkByText
        {
            Web,
            Images,
            Videos,
            Shopping,
            News,
            More,
            [EnumMember("Search tools")]
            SearchTools
        }
    }
}
