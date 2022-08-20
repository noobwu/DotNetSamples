﻿// ***********************************************************************
// Assembly         : NoobCore.Tests
// Author           : Administrator
// Created          : 08-20-2022
//
// Last Modified By : Administrator
// Last Modified On : 08-20-2022
// Link             : https://github.com/dotnet/roslyn/blob/main/src/EditorFeatures/Test/Utilities/BloomFilterTests.cs
// ***********************************************************************
// <copyright file="BloomFilterTests.cs" company="NoobCore.Tests">
//     Copyright (c) . All rights reserved.
// </copyright>
// <summary></summary>
// ***********************************************************************
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace NoobCore.Tests.Algorithms
{
    /// <summary>
    /// Class BloomFilterTests.
    /// </summary>
    public class BloomFilterTests
    {
        /// <summary>
        /// Generates the strings.
        /// </summary>
        /// <param name="count">The count.</param>
        /// <returns>IEnumerable&lt;System.String&gt;.</returns>
        private static IEnumerable<string> GenerateStrings(int count)
        {
            for (var i = 1; i <= count; i++)
            {
                yield return GenerateString(i);
            }
        }

        /// <summary>
        /// Generates the string.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>System.String.</returns>
        private static string GenerateString(int value)
        {
            const string Alphabet = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            var builder = new StringBuilder();

            while (value > 0)
            {
                var v = value % Alphabet.Length;
                var c = Alphabet[v];
                builder.Append(c);
                value /= Alphabet.Length;
            }

            return builder.ToString();
        }

        /// <summary>
        /// Tests the specified is case sensitive.
        /// </summary>
        /// <param name="isCaseSensitive">if set to <c>true</c> [is case sensitive].</param>
        private static void Test(bool isCaseSensitive)
        {
            var comparer = isCaseSensitive ? StringComparer.Ordinal : StringComparer.OrdinalIgnoreCase;
            var strings = GenerateStrings(2000).Skip(500).Take(1000).ToSet(comparer);
            var testStrings = GenerateStrings(100000);
            //Console.WriteLine($"strings:{strings},testStrings:{testStrings}");

            for (var d = 0.1; d >= 0.0001; d /= 10)
            {
                var filter = new BloomFilter(strings.Count, d, isCaseSensitive);
                filter.AddRange(strings);

                var correctCount = 0.0;
                var incorrectCount = 0.0;
                foreach (var test in testStrings)
                {
                    var actualContains = strings.Contains(test);
                    var filterContains = filter.ProbablyContains(test);

                    if (!filterContains)
                    {
                        // if the filter says no, then it can't be in the real set.
                        Assert.False(actualContains);
                    }

                    if (actualContains == filterContains)
                    {
                        correctCount++;
                    }
                    else
                    {
                        incorrectCount++;
                    }
                }

                var falsePositivePercentage = incorrectCount / (correctCount + incorrectCount);
                Assert.True(falsePositivePercentage < (d * 1.5), string.Format("falsePositivePercentage={0}, d={1}", falsePositivePercentage, d));
            }
        }

        /// <summary>
        /// Defines the test method Test1.
        /// </summary>
        [Fact]
        public void Test1()
           => Test(isCaseSensitive: true);

        /// <summary>
        /// Defines the test method TestInsensitive.
        /// </summary>
        [Fact]
        public void TestInsensitive()
            => Test(isCaseSensitive: false);

        /// <summary>
        /// Defines the test method TestEmpty.
        /// </summary>
        [Fact]
        public void TestEmpty()
        {
            for (var d = 0.1; d >= 0.0001; d /= 10)
            {
                var filter = new BloomFilter(0, d, isCaseSensitive: true);
                Assert.False(filter.ProbablyContains(string.Empty));
                Assert.False(filter.ProbablyContains("a"));
                Assert.False(filter.ProbablyContains("b"));
                Assert.False(filter.ProbablyContains("c"));

                var testStrings = GenerateStrings(100000);
                foreach (var test in testStrings)
                {
                    Assert.False(filter.ProbablyContains(test));
                }
            }
        }

        /// <summary>
        /// Creates the longs.
        /// </summary>
        /// <param name="ints">The ints.</param>
        /// <returns>HashSet&lt;System.Int64&gt;.</returns>
        private static HashSet<long> CreateLongs(List<int> ints)
        {
            var result = new HashSet<long>();

            for (var i = 0; i < ints.Count; i += 2)
            {
                var long1 = ((long)ints[i]) << 32;
                var long2 = (long)ints[i + 1];

                result.Add(long1 | long2);
            }

            return result;
        }
    }
}
