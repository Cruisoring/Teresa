using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Text;
using System.Threading.Tasks;
using System.Web.Management;

namespace Teresa.Utility
{
    /// <summary>
    /// Repository of common Predicate Factory methods.
    /// </summary>
    /// <typeparam name="T">Type of the target object to be evaluated.</typeparam>
    public class GenericPredicate<T>
    {
        /// <summary>
        /// Method to generate Predicate function to judge if the target object has an index of collection.
        /// <remarks>Don't store the Predicate, use it only once within clauses such as Where() or Select().</remarks>
        /// </summary>
        /// <param name="index">The index of the target within a collection, '0' means it appears as the first one.</param>
        /// <returns>The Predicate with unified Func<T, bool> signature.</returns>
        public static Func<T, bool> IndexPredicateOf(int index)
        {
            return new GenericPredicate<T>(index).Predicate;
        }

        public readonly int Index;
        //The count is initialized as -1, so judge the first target would change it to 0.
        private int count = -1;

        /// <summary>
        /// This Predicate is supposed to be used within clauses such as Where() or Select() to evaluate the target
        /// ojbect with a T typed collection.
        /// Because each judgement would use it once and increase the count by 1, so only the 'Index-th' target of the
        /// related collection would return 'true'.
        /// </summary>
        public Func<T, bool> Predicate
        {
            get
            {
                return (T t) =>
                {
                    count++;
                    return count == Index;
                };
            }
        }

        public GenericPredicate(int index)
        {
            Index = index;
        }
    }
}
