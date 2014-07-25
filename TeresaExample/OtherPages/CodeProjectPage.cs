using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Teresa;

namespace TeresaExample
{
    public class CodeProjectPage : Page
    {
        public override Uri SampleUri
        {
            get { return new Uri("http://www.codeproject.com/"); }
        }

        public override bool Equals(Uri other)
        {
            return other.Host.Contains("codeproject.com");
        }

        public enum SpanByClass
        {
            [EnumMember("member-signin")]
            SignIn
        }

        public class NavBarFragment  : Fragment
        {
            public enum DivByClass
            {
                [EnumMember(true)]
                navbar
            }

            public enum LinkByText
            {
                home,
                articles,
                [EnumMember("quick answers")]
                quickAnswers,
                discussions,
                features,
                community,
                help
            }
        }
    }
}
