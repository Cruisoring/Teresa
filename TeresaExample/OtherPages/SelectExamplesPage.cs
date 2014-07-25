using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Security.AccessControl;
using System.Text;
using System.Threading.Tasks;
using Teresa;

namespace TeresaExample.SelectExamples
{
    public class SelectExamplesPage : Page
    {
        public enum SelectByName 
        {
            sports,
            cars,
            food,
            techniques
        }

        public override Uri SampleUri
        {
            get
            {
                var fileInfo = new FileInfo(@".\SelectExamples.html");
                return new Uri(fileInfo.FullName);
            }
        }
    }
}
