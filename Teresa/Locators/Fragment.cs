using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using OpenQA.Selenium;
using OpenQA.Selenium.Remote;

namespace Teresa
{
    public class Fragment : Locator
    {
        #region Static fields and functions
        public static Type FragmentType = typeof (Fragment);
        public static string FragmentTypeName = FragmentType.Name;
        private static Assembly callingAssembly = null;
        protected static Assembly CallingAssembly
        {
            get
            {
                if (callingAssembly == null)
                    callingAssembly = callingAssemblyByStackTrace();
                return callingAssembly;
            }
        }

        private static Type fragmentTypeInCallingAssembly = null;

        protected static Type FragmentTypeInCallingAssembly
        {
            get
            {
                if (fragmentTypeInCallingAssembly == null)
                {
                    var types = CallingAssembly.GetTypes();
                    fragmentTypeInCallingAssembly = types.FirstOrDefault(t => t.Name.Equals(FragmentTypeName));
                }
                return fragmentTypeInCallingAssembly;
            }
        }

        protected static Assembly callingAssemblyByStackTrace()
        {
            Assembly thisAssembly = Assembly.GetExecutingAssembly();

            StackTrace stackTrace = new StackTrace();
            StackFrame[] frames = stackTrace.GetFrames();

            foreach (var stackFrame in frames)
            {
                var ownerAssembly = stackFrame.GetMethod().DeclaringType.Assembly;
                if (ownerAssembly != thisAssembly)
                    return ownerAssembly;
            }
            return thisAssembly;
        }

        //static Fragment()
        //{
        //    if (CallingAssembly == null)
        //        CallingAssembly = callingAssemblyByStackTrace();

        //    FragmentTypeInCallingAssembly = CallingAssembly.GetType(FragmentType.Name);
        //}

        //Obsoleted when the type of "Fragment" of the calling Assemby is also kept and used to compare.
        ///// <summary>
        ///// Temporary solution to compare the type with that of Fragment class when Type.IsSubclassOf(FragmentType) fails
        ///// working due to different AppDomains.
        ///// </summary>
        ///// <param name="theType"></param>
        ///// <returns></returns>
        //public static bool IsSubclassOfFragment(Type theType)
        //{
        //    string typeName = theType.Name;
        //    Type baseType = theType;
        //    do
        //    {
        //        if (typeName == FragmentTypeName)
        //            return true;
        //        baseType = baseType.BaseType;
        //        if (baseType == null)
        //            break;
        //        typeName = baseType.Name;
        //    } while (true);
        //    return false;
        //}

        /// <summary>
        /// Get all nested Enum entries defined directly within this Fragment type.
        /// </summary>
        /// <param name="fragmentType">Type of the Fragment.</param>
        /// <returns>All Enum entries whose EnumMemberAttribute contains CSS Selectors.</returns>
        protected static List<Enum> GetNestedElementIds(Type fragmentType)
        {
            if (fragmentType != FragmentType && !fragmentType.IsSubclassOf(FragmentType) 
                && fragmentType != FragmentTypeInCallingAssembly && !fragmentType.IsSubclassOf(FragmentTypeInCallingAssembly))
                throw new Exception("This shall only be called from Fragment classes");

            var types = fragmentType.GetNestedTypes().Where(t => t.IsEnum).ToList();
            if (types.Count == 0)
                return null;

            var result = new List<Enum>();
            foreach (var type in types)
            {
                var enumValues = type.GetEnumValues().OfType<Enum>().ToList();
                result.AddRange(enumValues);
            }
            return result;
        }

        /// <summary>
        /// Get the Enum that identify the fragment. In another words, each fragment type shall contain at least one
        /// Enum entry whose IsFragment() == true.
        /// </summary>
        /// <param name="fragment">Instance of the specific Fragment type.</param>
        /// <returns>The first Enum whose IsFragment() is "true".</returns>
        public static Enum GetFragmentId(Fragment fragment)
        {
            return GetNestedElementIds(fragment.GetType()).FirstOrDefault(x => x.IsFragment());
        }

        /// <summary>
        /// Get the Enum that identify the fragment type.
        /// </summary>
        /// <param name="fragmentType">Type of the specific Fragment.</param>
        /// <returns>The first Enum whose IsFragment() is "true".</returns>
        public static Enum GetFragmentId(Type fragmentType)
        {
            var ids = GetNestedElementIds(fragmentType);
            if (ids == null)
                return null;

            return ids.FirstOrDefault(x => x.IsFragment());
        }
        #endregion

        /// <summary>
        /// Contains the Enums associated with IWebElement and corresponding Locator instances.
        /// </summary>
        public Dictionary<Enum, Locator> Children { get; protected set; }

        /// <summary>
        /// To enable one fragments hold zero, one or more child fragments, the locators of them shall be listed.
        /// </summary>
        public virtual ICollection<Enum> FragmentEnums { get { return null; }}

        #region Methods
        /// <summary>
        /// Get the Locator associated with the Enum identifier.
        /// </summary>
        /// <param name="theEnum">The Enum Identifier associated with a IWebElement.</param>
        /// <returns>The locator associated with that IWebElement.</returns>
        public Locator LocatorOf(Enum theEnum)
        {
            if (Children.ContainsKey(theEnum))
                return Children[theEnum];

            return null;
        }

        /// <summary>
        /// Load a child fragment with its type.
        /// </summary>
        /// <param name="fragmentType">Type of the Fragment concerned.</param>
        public void Load(Type fragmentType)
        {
            if (!fragmentType.IsSubclassOf(FragmentType))
                throw new ArgumentException(fragmentType + " is not SubClass of " + FragmentType);

            var nestedIds = GetNestedElementIds(fragmentType);
            Enum fragmentEnum = nestedIds.FirstOrDefault(x => x.IsFragment());
            if (fragmentEnum != null)
                Load(fragmentEnum);
            else
            {
                foreach (var nestedId in nestedIds)
                {
                    if (!Children.ContainsKey(nestedId))
                    {
                        Locator childLocator = Locator.LocatorOf(nestedId, this);
                        Children.Add(nestedId, childLocator);
                    }
                }
            }
        }

        /// <summary>
        /// Load a child fragment with its identifier.
        /// </summary>
        /// <param name="fragmentEnum">The Enum identifying the fragment whose IsFragment=="true".</param>
        public void Load(Enum fragmentEnum)
        {
            if (Children.ContainsKey(fragmentEnum))
                return;
                //throw new Exception("The Locators contained by " + declaringType + " is already loaded.");

            if (!fragmentEnum.IsFragment())
                throw new ArgumentException("The EnumMemberAttribute.IsFragment of " + fragmentEnum + " is false.");
            
            Type declaringType = fragmentEnum.GetType().DeclaringType;

            if (!declaringType.IsSubclassOf(FragmentType))
                throw new ArgumentException("The type of " + declaringType + " is not SubClass of " + FragmentType);

            Fragment childFragment = null;

            ConstructorInfo ctor = declaringType.GetConstructor(new[] { FragmentType });

            if (ctor == null && FragmentTypeInCallingAssembly != null && FragmentTypeInCallingAssembly != FragmentType)
            {
                ctor = declaringType.GetConstructor(new[] { FragmentTypeInCallingAssembly });
            }
            
            if (ctor != null)
            {
                childFragment = (Fragment)ctor.Invoke(new object[] { this });
            }
            else
            {
                ctor = declaringType.GetConstructors().FirstOrDefault(x => x.GetParameters().Count() == 0);
                if (ctor == null)
                    throw new NullReferenceException("No default constructor() defined for " + declaringType);

                childFragment = (Fragment)ctor.Invoke(new object[] {});
                childFragment.Parent = this;
            }

            Children.Add(fragmentEnum, childFragment);
            foreach (KeyValuePair<Enum, Locator> kvp in childFragment.Children)
            {
                Children.Add(kvp.Key, kvp.Value);
            }
        }

        /// <summary>
        /// Unloads child fragment using its identifier.
        /// </summary>
        /// <param name="fragmentEnum">The Enum identifying the fragment whose IsFragment=="true".</param>
        public void Unload(Enum fragmentEnum)
        {
            if (!fragmentEnum.IsFragment())
                throw new ArgumentException("The EnumMemberAttribute.IsFragment of " + fragmentEnum + " is false.");

            Type declaringType = fragmentEnum.GetType().DeclaringType;

            if (!declaringType.IsAssignableFrom(FragmentType))
                throw new ArgumentException("The type of " + declaringType + " is not of Fragment type.");

            if (!Children.ContainsKey(fragmentEnum))
                //throw new Exception("The Locators contained by " + declaringType + " is not loaded yet.");
                return;

            var nestedIds = GetNestedElementIds(declaringType);
            if (nestedIds == null || nestedIds.Count == 0)
                return;

            foreach (var id in nestedIds)
            {
                if (Children.ContainsKey(id))
                    Children.Remove(id);
            }
        }

        /// <summary>
        /// Indexer to operate the IWebElements contained by the Fragment by:
        /// first, get the Locator associated with the IWebElement that is identified by theEnum.
        /// then, calling GetString() or SetString() of that Locator to perform desired operation.
        /// </summary>
        /// <param name="theEnum">The Enum identifying the IWebElement.</param>
        /// <param name="extraInfo">Instructive info needed to call GetString()/SetString().</param>
        /// <param name="filters"> ByIndex to select one IWebElement from a collection of them, would be used by this Fragment and its parents.</param>
        /// <returns>The result of GetString() or input to SetString().</returns>
        public string this[Enum theEnum, string extraInfo = null, Func<IWebElement, bool> filters = null]
        {
            get
            {
                var locator = LocatorOf(theEnum);
                if (locator == null)
                {
                    Console.WriteLine("Failed to get the Locator of " + theEnum );
                    return null;
                }
                else
                    return locator[extraInfo, filters];
            }
            set
            {
                Locator locator = LocatorOf(theEnum);
                if (locator != null)
                    locator[extraInfo, filters] = value;
                else
                {
                    Console.WriteLine("Failed to get the locator of " + theEnum + " CSS: " + theEnum.Css());
                }
            }
        }

        /// <summary>
        /// This method is called during the initialization to build up dictionary of searching Locators with Enum.
        /// </summary>
        protected void Populate()
        {
            Type thisType = this.GetType();

            //Load Enum Identifiers directly defined within this Fragment
            var nestedIds = GetNestedElementIds(thisType);
            if (nestedIds != null && nestedIds.Count != 0)
            {
                //No need to store a reference to Fragment itself
                nestedIds.Remove(Identifier);

                foreach (var id in nestedIds)
                {
                    Locator childLocator = id.IsFragment() ? 
                        new Fragment(id, this)
                        : Locator.LocatorOf(id, this);
                    Children.Add(id, childLocator);
                }
            }

            //Load Enum Identifiers of the nested Fragment class
            var nestedFragmentTypes = thisType.GetNestedTypes().Where(t => t.IsSubclassOf(FragmentType));
            foreach (Type nestedFragmentType in nestedFragmentTypes)
            {
                Load(nestedFragmentType);
            }

            //Load Enum Identifiers from the Contained Fragments identified by their Enum IDs
            if (FragmentEnums == null || FragmentEnums.Count == 0)
                return;

            foreach (Enum fragmentEnum in FragmentEnums)
            {
                Load(fragmentEnum);
            }
        }

        /// <summary>
        /// Convenient method to get the IWebElement from a collection identified by "enumId" with the "filters".
        /// Notice: the "filters" can be used to screen out both the 
        /// </summary>
        /// <param name="enumId">
        /// The Enum identify a collection of IWebElement, 
        /// Or, a unique IWebElement within a Fragment locating a collection of parent IWebElement.
        /// </param>
        /// <param name="filters">
        /// Filters applied to select the single one from the collection identified by "enumId" directly.
        /// Or applied to get the parent IWebElement collection first, then get the child from the selected parent.
        /// </param>
        /// <returns>The first IWebElement matched by the "filters".</returns>
        public IWebElement ElementOf(Enum enumId, Func<IWebElement, bool> filters)
        {
            Locator locator = LocatorOf(enumId);
            return locator.FindElement(filters);
        }

        /// <summary>
        /// Retrieve the Index-th of the IWebElement collection located by collectionEnum.
        /// </summary>
        /// <param name="collectionEnum">The Enum identifier of a collection of IWebElement.</param>
        /// <param name="index">The Index number, from 0, of the concerned one within the collection.</param>
        /// <returns>The IWebElement whose order within the collection is identical to "Index".</returns>
        public IWebElement ElementByIndex(Enum collectionEnum, int index)
        {
            EnumMemberAttribute usage = EnumMemberAttribute.EnumMemberAttributeOf(collectionEnum);
            if (!usage.IsCollection)
                throw new Exception(collectionEnum.ToString() + " shall be IsCollection=true.");

            var thisElement = FindElement();
            var candidates = thisElement.FindElementsByCss(usage.Css);

            return candidates[index];
        }

        #endregion End of Methods

        #region Constructors
        protected Fragment() : this(null) {}

        protected Fragment(Fragment parent)
        {
            Parent = parent;
            Identifier = GetFragmentId(this.GetType());
            Children = new Dictionary<Enum, Locator>();

            Populate();
        }

        protected Fragment(Enum identifier, Fragment parent = null)
            : base(identifier, parent)
        {
            Children = new Dictionary<Enum, Locator>();
            Populate();
        }

        #endregion End of Constructors
    }
}
