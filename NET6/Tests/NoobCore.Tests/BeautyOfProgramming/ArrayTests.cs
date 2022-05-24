// ***********************************************************************
// Assembly         : NoobCore.Tests
// Author           : Administrator
// Created          : 05-19-2022
//
// Last Modified By : Administrator
// Last Modified On : 05-19-2022
// ***********************************************************************
// <copyright file="ArrayTests.cs" company="NoobCore.Tests">
//     Copyright (c) . All rights reserved.
// </copyright>
// <summary></summary>
// ***********************************************************************
using NUnit.Framework;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NoobCore.Tests.BeautyOfProgramming
{
    /// <summary>
    /// Class ArrayTests.
    /// </summary>
    [TestFixture]
    public class ArrayTests
    {
        [TestCaseSource(nameof(DivSource))]
        public void Div(int size) {
            Assert.Greater(size, 0);
            var reverseInts = Enumerable.Range(1, size).Select(a => a+1).ToArray();
            Console.WriteLine($"source,reverse ints:[{string.Join(",",reverseInts)}]");
            for (int i = reverseInts.Length-1; i >=0; i--)
            {
                reverseInts[i]/=reverseInts[0];
            }
            Console.WriteLine($" div,reverse ints:[{string.Join(",", reverseInts)}]");

            var ints = Enumerable.Range(1, size).Select(a => a+1).ToArray();
            Console.WriteLine($"source,ints:[{string.Join(",", ints)}]");
            for (int i =0;i< ints.Length; i++)
            {
                ints[i] /= ints[0];
            }
            Console.WriteLine($" div,ints:[{string.Join(",", ints)}]");

        }

        /// <summary>
        /// Divs the source.
        /// </summary>
        /// <returns>IEnumerable.</returns>
        public static IEnumerable DivSource()
        {
            yield return new TestCaseData(1);
            yield return new TestCaseData(5);
            yield return new TestCaseData(10);
            yield return new TestCaseData(100);
        }
    }

}
