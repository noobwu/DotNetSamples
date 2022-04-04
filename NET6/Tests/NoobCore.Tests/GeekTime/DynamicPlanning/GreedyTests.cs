// ***********************************************************************
// Assembly         : NoobCore.Tests
// Author           : Administrator
// Created          : 04-04-2022
//
// Last Modified By : Administrator
// Last Modified On : 04-04-2022
// ***********************************************************************
// <copyright file="GreedyTests.cs" company="NoobCore.Tests">
//     Copyright (c) . All rights reserved.
// </copyright>
// <summary></summary>
// ***********************************************************************
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using NUnit.Framework;

namespace NoobCore.Tests.GeekTime.DynamicPlanning
{
    /// <summary>
    /// Class GreedyTests.
    /// </summary>
    [TestFixture]
    public class GreedyTests
    {
        /// <summary>
        /// Gets the minimum coin count helper.
        /// </summary>
        /// <param name="total">The total.</param>
        /// <param name="values">The values.</param>
        /// <param name="valueCount">The value count.</param>
        /// <returns>System.Int32.</returns>
        private int GetMinCoinCountHelper(int total, int[] values, int valueCount)
        {
            int rest = total;
            int count = 0;

            // 从大到小遍历所有面值
            for (int i = 0; i < valueCount; ++i)
            {
                int currentCount = rest / values[i]; // 计算当前面值最多能用多少个
                rest -= currentCount * values[i]; // 计算使用完当前面值后的余额
                count += currentCount; // 增加当前面额用量

                if (rest == 0)
                {
                    return count;
                }
            }

            return -1; // 如果到这里说明无法凑出总价，返回-1
        }

        /// <summary>
        /// Gets the minimum coin count.
        /// </summary>
        /// <param name="values">硬币面值</param>
        /// <param name="total">总价</param>
        /// <param name="minCount"></param>
        /// <returns>System.Int32.</returns>
        [TestCaseSource("GetMinCoinCountSource")]
        public void GetMinCoinCount(int[] values, int total, int minCount)
        {
            var minCoinCount = GetMinCoinCountHelper(total, values, values.Length); // 输出结果
            Console.WriteLine($"GetMinCoinCount,expectedMinCount:{minCount},actualMinCount:{minCoinCount}");
            Assert.AreEqual(minCount, minCoinCount);
        }

        /// <summary>
        /// Gets the minimum coin count source.
        /// </summary>
        /// <returns>IEnumerable.</returns>
        public static IEnumerable GetMinCoinCountSource()
        {
            yield return new TestCaseData(new int[] { 5, 3 }, 11, -1);
            yield return new TestCaseData(new int[] { 5, 3 }, 13, 3);
            yield return new TestCaseData(new int[] { 5, 3 }, 10, 2);
        }

        /// <summary>
        /// Gets the minimum coin count of value.
        /// </summary>
        /// <param name="total">The total.</param>
        /// <param name="values">The values.</param>
        /// <param name="valueIndex">Index of the value.</param>
        /// <returns>System.Int32.</returns>
        private int GetMinCoinCountOfValue(int total, int[] values, int valueIndex)
        {
            int valueCount = values.Length;
            if (valueIndex == valueCount) { return int.MaxValue; }

            int minResult = int.MaxValue;
            int currentValue = values[valueIndex];
            int maxCount = total / currentValue;

            for (int count = maxCount; count >= 0; count--)
            {
                int rest = total - count * currentValue;

                // 如果rest为0，表示余额已除尽，组合完成
                if (rest == 0)
                {
                    minResult = Math.Min(minResult, count);
                    break;
                }

                // 否则尝试用剩余面值求当前余额的硬币总数
                int restCount = GetMinCoinCountOfValue(rest, values, valueIndex + 1);

                // 如果后续没有可用组合
                if (restCount == int.MaxValue)
                {
                    // 如果当前面值已经为0，返回-1表示尝试失败
                    if (count == 0) { break; }
                    // 否则尝试把当前面值-1
                    continue;
                }

                minResult = Math.Min(minResult, count + restCount);
            }

            return minResult;
        }

        /// <summary>
        /// Gets the minimum coin count loop.
        /// </summary>
        /// <param name="total">The total.</param>
        /// <param name="values">The values.</param>
        /// <param name="k">The k.</param>
        /// <returns>System.Int32.</returns>
        private int GetMinCoinCountLoop(int total, int[] values, int k)
        {
            int minCount = int.MaxValue;
            int valueCount = values.Length;

            if (k == valueCount)
            {
                return Math.Min(minCount, GetMinCoinCountOfValue(total, values, 0));
            }

            for (int i = k; i <= valueCount - 1; i++)
            {
                // k位置已经排列好
                int t = values[k];
                values[k] = values[i];
                values[i] = t;
                minCount = Math.Min(minCount, GetMinCoinCountLoop(total, values, k + 1)); // 考虑后一位

                // 回溯
                t = values[k];
                values[k] = values[i];
                values[i] = t;
            }

            return minCount;
        }

        /// <summary>
        /// Gets the minimum coin count of value.
        /// </summary>
        /// <param name="values">硬币面值.</param>
        /// <param name="total"> 总价.</param>
        /// <param name="minCount">硬币最小数量</param>
        /// <returns>System.Int32.</returns>
        [TestCaseSource("GetMinCoinCountOfValueSource")]
        public void GetMinCoinCountOfValue(int[] values, int total, int minCount)
        {
            var minCoinCount = GetMinCoinCountLoop(total, values, values.Length); // 输出结果
            Console.WriteLine($"GetMinCoinCountOfValue,expectedMinCount:{minCount},actualMinCount:{minCoinCount}");
            Assert.AreEqual(minCount, minCoinCount);
        }

        /// <summary>
        /// Gets the minimum coin count of value source.
        /// </summary>
        /// <returns>IEnumerable.</returns>
        public static IEnumerable GetMinCoinCountOfValueSource()
        {
            yield return new TestCaseData(new int[] { 3 }, 3, 1);
            yield return new TestCaseData(new int[] { 5 }, 5, 1);
            yield return new TestCaseData(new int[] { 5, 3 }, 11, 3);
            yield return new TestCaseData(new int[] { 3, 5 }, 11, 3);
            yield return new TestCaseData(new int[] { 3, 5 }, 1, int.MaxValue);
        }

        /// <summary>
        /// 返回币数，如果返回-1表示无法凑够total
        /// </summary>
        /// <param name="total">金额</param>
        /// <param name="coins">币种数组，从大到小排序</param>
        /// <returns>System.Int32.</returns>
        private static int GetMinCoinCountOfValueHelper(int total, int[] coins)
        {
            if (coins == null || coins.Length == 0)
            {
                return -1;
            }

            //当前币值
            int currentCoin = coins[0];

            //使用当前币值数量
            int useCurrentCoinCount = total / currentCoin;

            int restTotal = total - useCurrentCoinCount * currentCoin;
            // 如果restTotal为0，表示余额已除尽，组合完成
            if (restTotal == 0)
            {
                return useCurrentCoinCount;
            }

            // 其他币种数量
            int coninCount = -1;
            // 剩余的币种
            int[] restCoins = new int[coins.Length];
            Array.Copy(coins,1, restCoins,0, coins.Length);
            while (useCurrentCoinCount >= 0)
            {
                // 否则尝试用剩余面值求当前余额的硬币总数
                coninCount = GetMinCoinCountOfValueHelper(restTotal, restCoins);

                // 如果后续没有有可用组合,退一步，当前useCurrentCoinCount币数减1
                if (coninCount == -1)
                {
                    // 否则尝试把当前面值数-1
                    useCurrentCoinCount--;
                    // 重新计算restTotal
                    restTotal = total - useCurrentCoinCount * currentCoin;

                }
                else
                {
                    return useCurrentCoinCount + coninCount;
                }
            }

            return -1;
        }
    }
}
