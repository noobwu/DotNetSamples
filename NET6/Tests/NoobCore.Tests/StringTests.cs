using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
namespace NoobCore.Tests
{
    /// <summary>
    /// 
    /// </summary>
    [TestFixture]
    public class StringTests
    {
        /// <summary>
        /// Generates the string.
        /// </summary>
        /// <param name="len">The length.</param>
        [TestCaseSource("GenerateStringSource")]
        public void GenerateString(int len) {
            Assert.Greater(len, 0);
            string str = new string('s', len);
            Console.WriteLine($"len:{len},byteLen:{Encoding.UTF8.GetBytes(str).Length}");
            Assert.AreEqual(str.Length, len);
        }

        /// <summary>
        /// Generates the string source.
        /// </summary>
        /// <returns></returns>
        public static IEnumerable GenerateStringSource()
        {
            yield return new TestCaseData(2);
            yield return new TestCaseData(4);
            yield return new TestCaseData(8);
            yield return new TestCaseData(16);
            yield return new TestCaseData(32);
            yield return new TestCaseData(64);
            yield return new TestCaseData(128);
            yield return new TestCaseData(256);
            yield return new TestCaseData(512);
            yield return new TestCaseData(1024);
        }
    }
}
