using NUnit.Framework;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
//https://labuladong.github.io/algo/3/

namespace NoobCore.Tests.Algorithms
{
    /// <summary>
    /// 
    /// </summary>
    public class DynamicPlanningTests
    {

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
                throw new ArgumentException("CoinChangeRecurse");
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
            var minCount = CoinChangeRecurse(conins, amount);
            Console.WriteLine($"CoinChangeRecurse,conins:[{string.Join(",", conins)}],amount:{amount},minCount:{minCount},expected:{expected}");
            Assert.AreEqual(expected, minCount);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="amount">目标金额</param>
        /// <param name="conins">可选硬币面值</param>
        /// <param name="memos"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
        private int CoinChangeMemo(int[] conins, int amount, int[] memos)
        {
            if (amount < 0 || conins == null || conins.Length == 0
                || memos == null || memos.Length == 0)
            {
                throw new ArgumentException("CoinChangeMemo");
            }
            // 如果余额为0，说明当前组合成立，将组合加入到待选数组中
            if (amount == 0)
            {
                return 0;
            }
            //如果备忘录中直接返回(0:默认值,-1:没有可用组合)
            if (memos[amount] != 0) { return memos[amount]; }

            int minCount = int.MaxValue;
            for (int i = 0; i < conins.Length; i++)
            {
                // 遍历所有面值
                int currentAmount = conins[i];
                // 如果当前面值大于硬币总额，那么跳过
                if (currentAmount > amount) { continue; }

                // 使用当前面值，得到剩余硬币总额
                int restAmount = amount - currentAmount;
                int restCount = CoinChangeMemo(conins, restAmount, memos);
                // 如果返回-1，说明组合不可信，跳过
                if (restCount == -1) { continue; }

                // 保留最小总额
                int tmpCount = 1 + restCount;
                if (tmpCount < minCount) { minCount = tmpCount; }
            }

            // 如果没有可用组合，返回-1
            if (minCount == int.MaxValue)
            {
                memos[amount] = -1;
                return -1;
            }
            // 记录到备忘录
            memos[amount] = minCount;
            // 返回最小硬币数量
            return minCount;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="conins"></param>
        /// <param name="amount"></param>
        /// <param name="expected"></param>
        [TestCaseSource("CoinChangeSource")]
        public void CoinChangeMemo(int[] conins, int amount, int expected)
        {
            var memos = Enumerable.Range(0, amount + 1).Select(i => i = 0).ToArray();
            var minCount = CoinChangeMemo(conins, amount, memos);
            Console.WriteLine($"CoinChangeMemo,conins:[{string.Join(",", conins)}],amount:{amount},minCount:{minCount},expected:{expected}");
            Assert.AreEqual(expected, minCount);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="coins"></param>
        /// <param name="amount"></param>
        /// <returns></returns>
        public int CoinChangeArrayRecurse(int[] coins, int amount)
        {
            //数组大小为 amount + 1，初始值也为 int.MaxValue
            int[] memos = Enumerable.Range(0, amount + 1).Select(i => i = int.MaxValue).ToArray();

            // base case
            memos[0] = 0;
            // 外层 for 循环在遍历所有状态的所有取值
            for (int i = 0; i < memos.Length; i++)
            {
                // 内层 for 循环在求所有选择的最小值
                foreach (int coin in coins)
                {
                    // 子问题无解，跳过
                    if (i - coin < 0)
                    {
                        continue;
                    }
                    memos[i] = Math.Min(memos[i], 1 + memos[i - coin]);
                }
            }
            return (memos[amount] == amount + 1) ? -1 : memos[amount];
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="conins"></param>
        /// <param name="amount"></param>
        /// <param name="expected"></param>
        [TestCaseSource("CoinChangeSource")]
        public void CoinChangeArrayRecurse(int[] conins, int amount, int expected)
        {
            var minCount = CoinChangeArrayRecurse(conins, amount);
            Console.WriteLine($"CoinChangeArrayRecurse,conins:[{string.Join(",", conins)}],amount:{amount},minCount:{minCount},expected:{expected}");
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
