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
    [TestFixture]
    public class RecurseTests
    {
        /// <summary>
        /// 斐波那契数列的循环解法
        /// </summary>
        /// <param name="n"></param>
        /// <returns></returns>
        private int FibonacciForWhile(int n)
        {
            int[] resolution = { 0, 1 }; // 解的数组
            if (n < 2) { return resolution[n]; }

            int i = 1;
            int fib1 = 0, fib2 = 1, fib = 0;
            while (i < n)
            {
                fib = fib1 + fib2;
                fib1 = fib2;
                fib2 = fib;
                i++;
            }
            return fib; // 输出答案
        }

        /// <summary>
        /// 
        /// </summary>
        [TestCaseSource("FibonacciSource")]

        public void FibonacciForWhileTest(int n, int result)
        {
            var fib = FibonacciForWhile(n);
            Console.WriteLine($"FibonacciForWhileTest,n:{n},fib:{fib},result;{result}");
            Assert.AreEqual(result, fib);
        }

      

        /// <summary>
        /// 
        /// </summary>
        /// <param name="n"></param>
        /// <returns></returns>
        public int Fibonacci(int n)
        {
            if (0 == n || 1 == n) { return n; }
            if (n > 1) { return Fibonacci(n - 1) + Fibonacci(n - 2); }

            return 0; // 如果输入n有误，则返回默认值
        }
        /// <summary>
        /// 
        /// </summary>
        [TestCaseSource("FibonacciSource")]

        public void FibonacciTest(int n, int result)
        {
            var fib = Fibonacci(n);
            Console.WriteLine($"FibonacciTest,n:{n},fib:{fib},result;{result}");
            Assert.AreEqual(result, fib);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="n"></param>
        /// <param name="memo"></param>
        /// <returns></returns>
        public int Fibonacci(int n, int[] memo)
        {
            if (0 == n || 1 == n) { return n; }
            if (memo[n] != 0) { return memo[n]; } // 看来备忘录中找到了之前计算的结果，既然找到了，直接返回，避免重复计算

            if (n > 1)
            {
                memo[n] = Fibonacci(n - 1, memo) + Fibonacci(n - 2, memo);
                return memo[n];
            }

            return 0; // 如果数值无效(比如 < 0)，则返回0
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="n"></param>
        [TestCaseSource("FibonacciSource")]
        public void FibonacciAdvance(int n, int result)
        {
            int[] memo = new int[n + 1];
            var fib = Fibonacci(n,memo);
            Console.WriteLine($"FibonacciAdvance,n:{n},fib:{fib},result;{result}");
            Assert.AreEqual(result, fib);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public static IEnumerable FibonacciSource()
        {
            yield return new TestCaseData(0, 0);
            yield return new TestCaseData(1, 1);
            yield return new TestCaseData(2, 1);
            yield return new TestCaseData(3, 2);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="total"></param>
        /// <param name="values"></param>
        /// <param name="currentCounts"></param>
        /// <param name="combinations"></param>
        public void GetMinCountsHelper(int total, int[] values, List<int> currentCounts, List<List<int>> combinations)
        {
            if (0 == total)
            {
                // 如果余额为0，说明当前组合成立，将组合加入到待选数组中
                combinations.Add(new List<int>(currentCounts));
                return;
            }

            int valueLength = values.Length;
            for (int i = 0; i < valueLength; i++)
            {
                // 遍历所有面值
                int currentValue = values[i];
                if (currentValue > total)
                {
                    // 如果面值大于当前总额，直接跳过
                    continue;
                }

                // 否则在当前面值数量组合上的对应位置加1
                List<int> newCounts = new List<int>(currentCounts);
                newCounts[i] = newCounts[i] + 1;
                int rest = total - currentValue;

                GetMinCountsHelper(rest, values, newCounts, combinations); // 求解剩余额度所需硬币数量
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="combinations"></param>
        /// <returns></returns>
        public int GetMinimumHelper(List<List<int>> combinations)
        {
            // 如果没有可用组合，返回-1
            if (combinations == null || 0 == combinations.Count) { return -1; }

            int minCount = int.MaxValue;
            foreach (var counts in combinations)
            {
                int total = 0; // 求当前组合的硬币总数
                foreach (var count in counts)
                {
                    total += count;
                }
                // 保留最小的
                if (total < minCount) { minCount = total; }
            }
            return minCount;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="values">硬币面值的数组</param>
        /// <param name="total">总值</param>
        /// <param name="expectedMinCount">最少银币数量</param>
        [TestCaseSource("GetMinCoinCountSource")]

        public void GetMinCountOfCoins(int[] values,int total,int expectedMinCount)
        {
            values = values.OrderByDescending(x => x).ToArray(); // 硬币面值的数组
            //int total = 11; // 总值

            List<int> initialCounts = new List<int>(new int[values.Length]); // 初始值(0,0)
            

            List<List<int>> coinCombinations = new List<List<int>>(); // 存储所有组合
            GetMinCountsHelper(total, values, initialCounts, coinCombinations); // 求解所有组合（不去重）

            var minCount = GetMinimumHelper(coinCombinations); // 输出答案
            Console.WriteLine($"GetMinCountOfCoins,minCount:{minCount},expectedMinCount:{expectedMinCount}");
            Assert.AreEqual(expectedMinCount,minCount);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="total"></param>
        /// <param name="values"></param>
        /// <returns></returns>
        public int GetMinCountsHelper(int total, int[] values)
        {
            // 如果余额为0，说明当前组合成立，将组合加入到待选数组中
            if (0 == total) { return 0; }

            int valueLength = values.Length;
            int minCount = int.MaxValue;
            for (int i = 0; i < valueLength; i++)
            { 
                // 遍历所有面值
                int currentValue = values[i];

                // 如果当前面值大于硬币总额，那么跳过
                if (currentValue > total) { continue; }

                int rest = total - currentValue; // 使用当前面值，得到剩余硬币总额
                int restCount = GetMinCountsHelper(rest, values);

                // 如果返回-1，说明组合不可信，跳过
                if (restCount == -1) { continue; }

                int totalCount = 1 + restCount; // 保留最小总额
                if (totalCount < minCount) { minCount = totalCount; }
            }

            // 如果没有可用组合，返回-1
            if (minCount == int.MaxValue) { return -1; }

            return minCount; // 返回最小硬币数量
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="values">硬币面值的数组</param>
        /// <param name="total">总值</param>
        /// <param name="expectedMinCount">最少银币数量</param>
        [TestCaseSource("GetMinCoinCountSource")]

        public void GetMinCountOfCoinsAdvance(int[] values, int total, int expectedMinCount)
        {
            values = values.OrderByDescending(x => x).ToArray(); // 硬币面值的数组

            var minCount = GetMinCountsHelper(total,values); // 输出答案
            Console.WriteLine($"GetMinCountOfCoinsAdvance,minCount:{minCount},expectedMinCount:{expectedMinCount}");
            Assert.AreEqual(expectedMinCount, minCount);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="total"></param>
        /// <param name="values"></param>
        /// <param name="memo"></param>
        /// <returns></returns>
        public int GetMinCountsHelper(int total, int[] values, int[] memo)
        {
            int savedMinCount = memo[total];
            if (savedMinCount != -2) { return savedMinCount; }


            int valueLength = values.Length;
            int minCount = int.MaxValue;
            for (int i = 0; i < valueLength; i++)
            { 
                // 遍历所有面值
                int currentValue = values[i];
                // 如果当前面值大于硬币总额，那么跳过
                if (currentValue > total) { continue; }

                // 使用当前面值，得到剩余硬币总额
                int rest = total - currentValue;
                int restCount = GetMinCountsHelper(rest, values, memo);
                // 如果返回-1，说明组合不可信，跳过
                if (restCount == -1) { continue; }

                // 保留最小总额
                int totalCount = 1 + restCount;
                if (totalCount < minCount) { minCount = totalCount; }
            }

            // 如果没有可用组合，返回-1
            if (minCount == int.MaxValue)
            {
                memo[total] = -1;
                return -1;
            }

            memo[total] = minCount; // 记录到备忘录
            return minCount; // 返回最小硬币数量
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="values">硬币面值</param>
        /// <param name="total">总值</param>
        /// <param name="expectedMinCount"></param>
        /// <returns></returns>
        [TestCaseSource("GetMinCoinCountSource")]
        public void GetMinCountsSol(int[] values, int total, int expectedMinCount)
        {
            //values = values.OrderByDescending(x => x).ToArray(); // 硬币面值的数组
            values = values.OrderBy(x => x).ToArray(); // 硬币面值的数组
            int[] memo = Enumerable.Range(0, total + 1).Select(i => -2).ToArray(); ;// , -2); // 备忘录，没有缓存的元素为-2
            memo[0] = 0; // 其中0对应的结果也是0，首先存在备忘录中

            // 求得最小的硬币数量，并输出结果
            var minCount= GetMinCountsHelper(total, values, memo); // 输出结果
            Console.WriteLine($"GetMinCountsSol,minCount:{minCount},expectedMinCount:{expectedMinCount}");
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
        }
    }
}
