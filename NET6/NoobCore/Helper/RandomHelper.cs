// ***********************************************************************
// Assembly         : NoobCore
// Author           : Administrator
// Created          : 2022-09-24
//
// Last Modified By : Administrator
// Last Modified On : 2022-09-24
// ***********************************************************************
// <copyright file="RandomHelper.cs" company="NoobCore">
//     Copyright (c) . All rights reserved.
// </copyright>
// <summary></summary>
// ***********************************************************************
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

/// <summary>
/// The Helper namespace.
/// </summary>
namespace NoobCore.Helper
{
    /// <summary>
    /// Class RandomHelper.
    /// </summary>
    public static  class RandomHelper
    {
        private static readonly Random Rnd = new Random();

        /// <summary>
        /// Returns a random number within a specified range.
        /// </summary>
        /// <param name="minValue">The inclusive lower bound of the random number returned.</param>
        /// <param name="maxValue">The exclusive upper bound of the random number returned. maxValue must be greater than or equal to minValue.</param>
        /// <returns>
        /// A 32-bit signed integer greater than or equal to minValue and less than maxValue; 
        /// that is, the range of return values includes minValue but not maxValue. 
        /// If minValue equals maxValue, minValue is returned.
        /// </returns>
        public static int Next(int minValue, int maxValue)
        {
            lock (Rnd)
            {
                return Rnd.Next(minValue, maxValue);
            }
        }

        /// <summary>
        /// Returns a nonnegative random number less than the specified maximum.
        /// </summary>
        /// <param name="maxValue">The exclusive upper bound of the random number to be generated. maxValue must be greater than or equal to zero.</param>
        /// <returns>
        /// A 32-bit signed integer greater than or equal to zero, and less than maxValue; 
        /// that is, the range of return values ordinarily includes zero but not maxValue. 
        /// However, if maxValue equals zero, maxValue is returned.
        /// </returns>
        public static int Next(int maxValue)
        {
            lock (Rnd)
            {
                return Rnd.Next(maxValue);
            }
        }

        /// <summary>
        /// Returns a nonnegative random number.
        /// </summary>
        /// <returns>A 32-bit signed integer greater than or equal to zero and less than <see cref="int.MaxValue"/>.</returns>
        public static int Next()
        {
            lock (Rnd)
            {
                return Rnd.Next();
            }
        }

        /// <summary>
        /// Gets random of given objects.
        /// </summary>
        /// <typeparam name="T">Type of the objects</typeparam>
        /// <param name="objs">List of object to select a random one</param>
        public static T GetRandomOf<T>(params T[] objs)
        {
            if (objs.IsEmpty())
            {
                throw new ArgumentNullException(nameof(objs));
            }
            return objs[Next(0, objs.Length)];
        }

        /// <summary>
        /// Gets random item from the given list.
        /// </summary>
        /// <typeparam name="T">Type of the objects</typeparam>
        /// <param name="list">List of object to select a random one</param>
        public static T GetRandomOfList<T>(IList<T> list)
        {
            if (list.IsEmpty())
            {
                throw new ArgumentNullException(nameof(list));
            }
            return list[Next(0, list.Count)];
        }

        /// <summary>
        /// Generates a randomized list from given enumerable.
        /// </summary>
        /// <typeparam name="T">Type of items in the list</typeparam>
        /// <param name="items">items</param>
        public static List<T> GenerateRandomizedList<T>(IEnumerable<T> items)
        {
            if (items.IsEmpty())
            {
                throw new ArgumentNullException(nameof(items));
            }

            var currentList = new List<T>(items);
            var randomList = new List<T>();

            while (currentList.Any())
            {
                var randomIndex = Next(0, currentList.Count);
                randomList.Add(currentList[randomIndex]);
                currentList.RemoveAt(randomIndex);
            }

            return randomList;
        }


        /// <summary>
        /// Safes the next.
        /// </summary>
        /// <param name="minValue">The minimum value.</param>
        /// <param name="maxValue">The maximum value.</param>
        /// <returns>System.Int32.</returns>
        public static int SafeNext(int minValue, int maxValue)
        {
            lock (Rnd)
            {
                return new Random(Guid.NewGuid().GetHashCode()).Next(minValue, maxValue);
            }
        }
    }
}
