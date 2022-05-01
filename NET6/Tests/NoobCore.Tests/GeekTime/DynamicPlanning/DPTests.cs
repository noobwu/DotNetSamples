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

        /// <summary>
        /// 
        /// </summary>
        /// <param name="n"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
        private int FibonacciRecurse(int n)
        {
            if (n < 0)
            {
                throw new ArgumentException(nameof(n));
            }
            if (n < 2)
            {
                return n;
            }
            if (n == 2)
            {
                return 1;
            }
            return FibonacciRecurse(n - 1) + FibonacciRecurse(n - 2);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="n"></param>
        /// <param name="expected"></param>
        [TestCaseSource("FibonacciSource")]
        public void FibonacciRecurse(int n, int expected)
        {
            var res = FibonacciRecurse(n);
            Console.WriteLine($"FibonacciRecurse,n:{n},expected:{expected}");
            Assert.AreEqual(res, expected);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="n"></param>
        /// <param name="memos"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
        private int FibonacciMemo(int n, int[] memos)
        {
            if (n < 0)
            {
                throw new ArgumentException(nameof(n));
            }
            if (n < 2)
            {
                return n;
            }
            if (n == 2)
            {
                return 1;
            }
            if (memos[n] != 0)
            {
                return memos[n];
            }
            return FibonacciMemo(n - 1, memos) + FibonacciMemo(n - 2, memos);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="n"></param>
        /// <param name="expected"></param>
        [TestCaseSource("FibonacciSource")]
        public void FibonacciMemo(int n, int expected)
        {
            int[] memos = Enumerable.Range(0, n + 1).Select(i => i = 0).ToArray();
            var res = FibonacciMemo(n, memos);

            Console.WriteLine($"FibonacciMemo,n:{n},expected:{expected}");
            Assert.AreEqual(res, expected);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="n"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
        private int FibonacciMemo(int n)
        {
            if (n < 0)
            {
                throw new ArgumentException(nameof(n));
            }
            if (n < 2)
            {
                return n;
            }
            if (n == 2)
            {
                return 1;
            }
            int[] memos = Enumerable.Range(0, n + 1).Select(i => i = 0).ToArray();
            memos[1] = memos[2] = 1;
            for (int i = 3; i <= n; i++)
            {
                memos[i] = memos[i - 1] + memos[i - 2];
            }
            return memos[n];
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="n"></param>
        /// <param name="expected"></param>
        [TestCaseSource("FibonacciSource")]
        public void FibonacciMemo1(int n, int expected)
        {
            int[] memos = Enumerable.Range(0, n + 1).Select(i => i = 0).ToArray();
            var res = FibonacciMemo(n);

            Console.WriteLine($"FibonacciMemo,n:{n},expected:{expected}");
            Assert.AreEqual(res, expected);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="n"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
        private int FibonacciRecurse1(int n)
        {
            if (n < 0)
            {
                throw new ArgumentException(nameof(n));
            }
            if (n < 2)
            {
                return n;
            }
            if (n == 2)
            {
                return 1;
            }
            int prev = 1, curr = 1;
            for (int i = 3; i <= n; i++)
            {
                int sum = prev + curr;
                prev = curr;
                curr = sum;
            }
            return curr;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="n"></param>
        /// <param name="expected"></param>
        [TestCaseSource("FibonacciSource")]
        public void FibonacciRecurse1(int n, int expected)
        {
            var res = FibonacciRecurse1(n);
            Console.WriteLine($"FibonacciRecurse1,n:{n},expected:{expected}");
            Assert.AreEqual(res, expected);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public static IEnumerable FibonacciSource()
        {
            yield return new TestCaseData(1, 1);
            yield return new TestCaseData(2, 1);
            yield return new TestCaseData(3, 2);
            yield return new TestCaseData(4, 3);
            yield return new TestCaseData(5, 5);
            yield return new TestCaseData(6, 8);
            yield return new TestCaseData(20, 6765);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="amount">目标金额</param>
        /// <param name="conins">可选硬币面值</param>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
        private int CoinChangeRecurse(int[] conins, int amount)
        {
            if (amount < 0 || conins == null || conins.Length == 0)
            {
                throw new ArgumentException(nameof(amount));
            }
            // 如果余额为0，说明当前组合成立，将组合加入到待选数组中
            if (amount == 0)
            {
                return 0;
            }

            int minCount = int.MaxValue;
            for (int i = 0; i < conins.Length; i++)
            {
                // 遍历所有面值
                int currentValue = conins[i];

                // 如果当前面值大于硬币总额，那么跳过
                if (currentValue > amount) { continue; }
                int rest = amount - currentValue; // 使用当前面值，得到剩余硬币总额
                int restCount = CoinChangeRecurse(conins, rest);

                // 如果返回-1，说明组合不可信，跳过
                if (restCount == -1) { continue; }

                int tmpCount = 1 + restCount; // 保留最小总额
                if (tmpCount < minCount) { minCount = tmpCount; }
            }

            // 如果没有可用组合，返回-1
            if (minCount == int.MaxValue) { return -1; }

            return minCount; // 返回最小硬币数量
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="conins"></param>
        /// <param name="amount"></param>
        /// <param name="expected"></param>
        [TestCaseSource("CoinChangeSource")]
        public void CoinChangeRecurse(int[] conins, int amount, int expected)
        {
            var minCount = CoinChangeRecurse(conins,amount);
            Console.WriteLine($"CoinChangeRecurse,conins:[{string.Join(",",conins)}],amount:{amount},minCount:{minCount},expected:{expected}");
            Assert.AreEqual(expected, minCount);
        }
        /// <summary>
        /// Gets the minimum coin count of value source.
        /// </summary>
        /// <returns>IEnumerable.</returns>
        public static IEnumerable CoinChangeSource()
        {
            yield return new TestCaseData(new int[] { 1, 2, 5 }, 1, 1);
            yield return new TestCaseData(new int[] { 1, 2, 5 }, 2, 1);
            yield return new TestCaseData(new int[] { 1, 2, 5 }, 5, 1);
            yield return new TestCaseData(new int[] { 1, 2, 5 }, 3, 2);
            yield return new TestCaseData(new int[] { 1, 2, 5 }, 11, 3);
        }
    }
}
