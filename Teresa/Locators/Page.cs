using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using OpenQA.Selenium.Remote;

namespace Teresa
{
    public abstract class Page : Fragment, IEquatable<Uri>
    {
        #region Static members
        public static Type PageType = typeof (Page);

        public static List<Page> Pages = new List<Page>();

        private static Page currentPage = null;
        public static Page CurrentPage {
            get
            {
                if (currentPage == null || !currentPage.IsCurrentPage())
                {
                    currentPage = Pages.FirstOrDefault(p => p.IsCurrentPage());
                    if (currentPage == null)
                        throw new Exception("No page is matched with " + DriverManager.CurrentUri);
                    currentPage.ActualUri = DriverManager.CurrentUri;
                }
                return currentPage;
            }
        }

        static Page()
        {
            //Assembly assembly = callingAssemblyByStackTrace();
            //var allTypes = assembly.GetTypes();

            var allTypes = CallingAssembly.GetTypes();

            List<Type> pageTypes = allTypes.Where(t => t.IsSubclassOf(PageType)).ToList();

            foreach (var pageType in pageTypes)
            {
                ConstructorInfo ctor = pageType.GetConstructors().FirstOrDefault(x => x.GetParameters().Count() == 0);
                if (ctor == null)
                    throw new NullReferenceException("No default constructor() defined for " + pageType);

                Page pageInstance = (Page)ctor.Invoke(new object[] { });
                Pages.Add(pageInstance);
            }
        }

        #endregion

        public abstract Uri SampleUri { get; }

        public virtual Encoding UriEncoding { get { return Encoding.UTF8; }}

        public NameValueCollection QueryValues { get; protected set; }

        private Uri actualUri = null;
        public Uri ActualUri
        {
            get { return actualUri; }
            set
            {
                if (actualUri == null || actualUri.AbsoluteUri != value.AbsoluteUri)
                {
                    actualUri = value;
                    ParseQuery(actualUri);
                }
            }
        }

        public virtual string QueryValueOf(string key)
        {
            if (0!=Uri.Compare(ActualUri, DriverManager.CurrentUri, 
                UriComponents.AbsoluteUri, UriFormat.SafeUnescaped, StringComparison.OrdinalIgnoreCase))
                ActualUri = DriverManager.CurrentUri;

            return QueryValues[key];
        }

        public virtual void ParseQuery(Uri uri)
        {
            QueryValues = HttpUtility.ParseQueryString(uri.Query, UriEncoding);            
        }

        public virtual void Navigate(Uri uri = null)
        {
            DriverManager.Driver.Navigate().GoToUrl(uri??SampleUri);
            Page.currentPage = this;
            ActualUri = DriverManager.CurrentUri;
        }

        public virtual bool Equals(Uri other)
        {
            var result = Uri.Compare(actualUri ?? SampleUri, other,
                UriComponents.NormalizedHost | UriComponents.Path,
                UriFormat.SafeUnescaped, StringComparison.OrdinalIgnoreCase);
            return result == 0;
        }

        public bool IsCurrentPage()
        {
            return Equals(DriverManager.CurrentUri);
        }

        protected Page()
        {
            Parent = null;
            Identifier = HtmlByCustom.Root;
            Children = new Dictionary<Enum, Locator>();

            Populate();
        }
    }
}
