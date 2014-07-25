using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using Teresa.Utility;

namespace TeresaUnitTesting.EnumTests
{
    public class Person
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public int Age { get; set; }

        public override string ToString()
        {
            return string.Format("{0} {1}: {2}", FirstName, LastName, Age);
        }
    }


    [TestFixture]
    public class GenericPredicateTest
    {
        private static Person[] Persons = new[]
        {
            new Person(){FirstName = "Tom", LastName = "Smith", Age = 33},
            new Person(){FirstName = "Mary", LastName = "Smith", Age = 11},
            new Person(){FirstName = "John", LastName = "Wilson", Age = 43},
            new Person(){FirstName = "Tom", LastName = "Johnson", Age = 43},
            new Person(){FirstName = "Grace", LastName = "Smith", Age = 63}
        };

        public void PrintPersons(IEnumerable<Person> persons)
        {
            foreach (var person in persons)
            {
                Console.WriteLine(person);
            }
        }

        //[Test]
        //public void IntPredicateTest_NoComparer()
        //{
        //    Func<Person, bool> predicate = GenericPredicate<Person>.PredicateOf(43, p => p.Age);
        //    var personsOfAge43 = Persons.Where(predicate).ToList();
        //    Console.WriteLine("Persons whose age is 43: ");
        //    PrintPersons(personsOfAge43);
        //    Assert.AreEqual(2, personsOfAge43.Count());
        //}

        //[Test]
        //public void IntPredicateTest_WithCustomizedComparer()
        //{
        //    //Predicate returns true when 'actual' is larger or equal to expected.
        //    Func<Person, bool> predicate = GenericPredicate<Person>.PredicateOf(18, p => p.Age, 
        //        (actual, expected) => actual >= expected);

        //    var personsOfOver18 = Persons.Where(predicate).ToList();
        //    Console.WriteLine("Persons whose age is over 18: ");
        //    PrintPersons(personsOfOver18);
        //    Assert.AreEqual(4, personsOfOver18.Count());
        //}

        //[Test]
        //public void StringPredicateTest_NoCustomizedComparer()
        //{
        //    //Predicate returns true when the person's FirstName is "Tom".
        //    Func<Person, bool> predicate = GenericPredicate<Person>.PredicateOf("Tom", p => p.FirstName);

        //    var personsOfTom = Persons.Where(predicate).ToList();
        //    Console.WriteLine("Persons whose FirstName is Tom: ");
        //    PrintPersons(personsOfTom);
        //    Assert.AreEqual(2, personsOfTom.Count());
        //}

        //[Test]
        //public void StringPredicateTest_WithStringComparison()
        //{
        //    //Predicate returns true when the person's FirstName is "TOM".
        //    Func<Person, bool> predicate = GenericPredicate<Person>.PredicateOf("TOM", p => p.FirstName, 
        //        StringComparison.InvariantCulture);

        //    var personsOfTOM = Persons.Where(predicate).ToList();
        //    Assert.AreEqual(0, personsOfTOM.Count());
            
        //    //Predicate returns true when the person's FirstName is "TOM", IgnoreCase.
        //    predicate = GenericPredicate<Person>.PredicateOf("TOM", p => p.FirstName, 
        //        StringComparison.InvariantCultureIgnoreCase);
            
        //    personsOfTOM = Persons.Where(predicate).ToList();
        //    PrintPersons(personsOfTOM);
        //    Assert.AreEqual(2, personsOfTOM.Count());
        //}

        //[Test]
        //public void ContainsStringPredicateTest()
        //{
        //    //Predicate returns true when the person's LasName contains "son".
        //    Func<Person, bool> predicate = GenericPredicate<Person>.ContainsPredicateOf("son", p => p.LastName);

        //    var personsOfContains = Persons.Where(predicate).ToList();
        //    Console.WriteLine("Persons whose LastName contains 'son': ");
        //    PrintPersons(personsOfContains);
        //    Assert.AreEqual(2, personsOfContains.Count());
        //}

        [Test]
        public void IndexPredicateTest()
        {
            //Predicate returns true only for the third one.
            Func<Person, bool> predicate = GenericPredicate<Person>.IndexPredicateOf(2);

            var thirdPerson = Persons.First(predicate);
            Console.WriteLine("Third person of the Persons:");
            Console.WriteLine(thirdPerson);

            //Use the predicate again would not return the third person because its count is not reset
            var nextThirdPerson = Persons.FirstOrDefault(predicate);
            Console.WriteLine("Try to get the Third person again, and it is:");
            Console.WriteLine(nextThirdPerson);

            Assert.IsNull(nextThirdPerson);
        }

    }
}
