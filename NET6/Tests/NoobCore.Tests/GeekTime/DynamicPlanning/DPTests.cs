using Newtonsoft.Json.Linq;
using NUnit.Framework;
using Sigil;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NoobCore.Tests.GeekTime.DynamicPlanning
{
    [TestFixture]
    public class DPTests
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="k"></param>
        /// <param name="values"></param>
        /// <returns></returns>
        public int GetMinCounts(int k, int[] values)
        {
            int[] memo = Enumerable.Range(0, k + 1).Select(i => -2).ToArray();
            memo[0] = 0; // 初始化状态

            for (int v = 1; v <= k; v++)
            {
                int minCount = k + 1; // 模拟无穷大
                for (int i = 0; i < values.Length; ++i)
                {
                    int currentValue = values[i];

                    // 如果当前面值大于硬币总额，那么跳过
                    if (currentValue > v) { continue; }

                    // 使用当前面值，得到剩余硬币总额
                    int rest = v - currentValue;
                    int restCount = memo[rest];

                    // 如果返回-1，说明组合不可信，跳过
                    if (restCount == -1) { continue; }

                    // 保留最小总额
                    int kCount = 1 + restCount;
                    if (kCount < minCount) { minCount = kCount; }
                }

                // 如果是可用组合，记录结果
                if (minCount != k + 1) { memo[v] = minCount; }
            }

            return memo[k];
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="values"> 硬币面值</param>
        /// <param name="total">总值</param>
        /// <param name="expectedMinCount"></param>
        [TestCaseSource("GetMinCoinCountSource")]
        public void GetMinCountsDPSol(int[] values, int total, int expectedMinCount)
        {
            // 求得最小的硬币数量
            var minCount = GetMinCounts(total, values); // 输出答案
            Console.WriteLine($"GetMinCountsDPSol,minCount:{minCount},expectedMinCount:{expectedMinCount}");
            Assert.AreEqual(expectedMinCount, minCount);
        }

        /// <summary>
        /// Gets the minimum coin count of value source.
        /// </summary>
        /// <returns>IEnumerable.</returns>
        public static IEnumerable GetMinCoinCountSource()
        {
            yield return new TestCaseData(new int[] { 3 }, 3, 1);
            yield return new TestCaseData(new int[] { 5 }, 5, 1);
            yield return new TestCaseData(new int[] { 5, 3 }, 11, 3);
            yield return new TestCaseData(new int[] { 3, 5 }, 11, 3);
            yield return new TestCaseData(new int[] { 3, 5 }, 1, -1);
            yield return new TestCaseData(new int[] { 3, 5 }, 14, 4);
            yield return new TestCaseData(new int[] { 3, 5 }, 22, 6);
        }

    }
}
