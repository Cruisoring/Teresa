using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Teresa
{
    public static class GenericWait<T>
    {
        /// <summary>
        /// This method execute func() continuously by calling Wait.Until() until timeout or expected condition is met.
        /// </summary>
        /// <param name="func">
        /// Any function returning T as result.
        /// For functions whose signature has one or more parameters, for example: 
        ///     public T someFunc(int param), 
        ///  This method can be called with assitance of LINQ as below:
        ///     Until(()=>someFunc(param), isExpected)
        /// </param>
        /// <param name="isExpected">ByIndex to judge if the result returned by func() is expected</param>
        /// <param name="timeoutMills">Time waited before draw conclusion that the command cannnot succeed.</param>
        /// <returns>The last result returned by func().</returns>
        public static T Until(Func<T> func, Func<T, bool> isExpected=null, int timeoutMills=Wait.TimeoutInMills)
        {
            if (func == null)
                throw new ArgumentNullException();

            T result = default(T);
            Func<bool> predicate = () =>
            {
                result = func();
                return isExpected ==null || isExpected(result);
            };

            Wait.Until(predicate, timeoutMills);
            return result;
        }

        /// <summary>
        /// This method execute func() continuously by calling Wait.Until() until timeout or expected condition is met.
        /// Unlike Until(), the Exception thrown by Wait.Until() would be silently discarded and returns the default(T).
        /// </summary>
        /// <param name="func">
        /// Any function returning T as result.
        /// For functions whose signature has one or more parameters, for example: 
        ///     public T someFunc(int param), 
        ///  This method can be called with assitance of LINQ as below:
        ///     Until(()=>someFunc(param), isExpected)
        /// </param>
        /// <param name="isExpected">ByIndex to judge if the result returned by func() is expected</param>
        /// <param name="timeoutMills">Time waited before draw conclusion that the command cannnot succeed.</param>
        /// <returns>The last result returned by func() or default(T) when Exception is caught.</returns>
        public static T TryUntil(Func<T> func, Func<T, bool> isExpected = null, int timeoutMills = Wait.TimeoutInMills)
        {
            if (func == null)
                throw new ArgumentNullException();

            T result = default(T);
            Func<bool> predicate = () =>
            {
                result = func();
                return isExpected == null || isExpected(result);
            };

            try
            {
                Wait.Until(predicate, timeoutMills);
            }
            catch (Exception)
            {
                ;
            }
            return result;
        }

    }
}
