using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
namespace NoobCore.Tests.Compares
{
    /// <summary>
    /// 
    /// </summary>
    [TestFixture]
    public class ComparableTests
    {
        /// <summary>
        /// 
        /// </summary>
        [TestCase]
        public void OrderBy() {
            Person tom27 = new Person("Tom", 27);
            Person roger21 = new Person("Roger", 21);
            Person fred24 = new Person("Fred", 24);
            Person fred30 = new Person("Fred", 30);

            List<Person> people = new List<Person>() { tom27, roger21, fred24, fred30 };

            List<Person> sorted = people.OrderBy(x => x.Name)
                                        .ThenBy(x => x.Age)
                                        .ToList();
            Console.WriteLine(string.Join(Environment.NewLine, sorted));
            Assert.AreEqual(fred24, sorted[0]);
        }
        /// <summary>
        /// 
        /// </summary>
        [TestCase]
        public void LinqSort()
        {
            Person tom27 = new Person("Tom", 27);
            Person roger21 = new Person("Roger", 21);
            Person fred24 = new Person("Fred", 24);
            Person fred30 = new Person("Fred", 30);

            List<Person> people = new List<Person>() { tom27, roger21, fred24, fred30 };

            people.Sort((x, y) => {
                int ret = string.Compare(x.Name, y.Name);
                return ret != 0 ? ret : x.Age.CompareTo(y.Age);
            });

            Console.WriteLine(string.Join(Environment.NewLine, people));
            Assert.AreEqual(fred24, people[0]);
        }
        /// <summary>
        /// 
        /// </summary>
        [TestCase]
        public void ComparerSort() {
            Person tom27 = new Person("Tom", 27);
            Person roger21 = new Person("Roger", 21);
            Person fred24 = new Person("Fred", 24);
            Person fred30 = new Person("Fred", 30);

            List<Person> people = new List<Person>() { tom27, roger21, fred24, fred30 };

            people.Sort(new PersonComparer());
            Assert.AreEqual(fred24, people[0]);
            Console.WriteLine(string.Join(Environment.NewLine, people));
        }
    }
    /// <summary>
    /// 
    /// </summary>

    public class PersonComparer : IComparer<Person>
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        public int Compare(Person x, Person y)
        {
            if (object.ReferenceEquals(x, y))
            {
                return 0;
            }

            if (x == null)
            {
                return -1;
            }

            if (y == null)
            {
                return 1;
            }

            int ret = String.Compare(x.Name, y.Name);
            return ret != 0 ? ret : x.Age.CompareTo(y.Age);
        }
    }

    /// <summary>
    /// 
    /// </summary>

    public class Person
    {
        /// <summary>
        /// 
        /// </summary>
        public string Name;
        /// <summary>
        /// 
        /// </summary>
        public int Age;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <param name="age"></param>
        public Person(string name, int age)
        {
            this.Name = name;
            this.Age = age;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return $"[{Name},{Age}]";
        }

        /// <summary>
        /// Equalses the specified other.
        /// </summary>
        /// <param name="other">The other.</param>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        public bool Equals(Person other)
        {

            //Check whether the compared object is null.
            if (object.ReferenceEquals(other, null)) return false;

            //Check whether the compared object references the same data.
            if (object.ReferenceEquals(this, other)) return true;

            //Check whether the products' properties are equal.
            return Name.Equals(other.Name) && Age.Equals(other.Age);
        }

        /// <summary>
        /// Returns a hash code for this instance.
        /// </summary>
        /// <returns>A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table.</returns>
        public override int GetHashCode()
        {

            //Get hash code for the Code field. 
            int hashName = Name.GetHashCode();

            //Get hash code for the Code field. 
            int hashAge= Age.GetHashCode();

            //Calculate the hash code for the product. 
            return hashName ^ hashAge;
        }
    }
}
