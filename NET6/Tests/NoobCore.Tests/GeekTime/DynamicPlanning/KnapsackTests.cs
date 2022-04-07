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
    public class KnapsackTests
    {
        /// <summary>
        /// 0-1 背包问题（针对当前物品，是放入背包，还是不放入背包时的价值最大）
        /// </summary>
        /// <param name="wtItems">物品的重量</param>
        /// <param name="valItems">物品的价值</param>
        /// <param name="total">物品的总数</param>
        /// <param name="maxWeight">背包能容纳的总重量</param>
        /// <returns></returns>
        public int KnapsackDP(int[] wtItems, int[] valItems, int total, int maxWeight)
        {
            // 创建备忘录
            int[,] dpItems = new int[total + 1, maxWeight + 1];

            // 初始化状态
            //for (int i = 0; i < N + 1; i++) { dpItems[i,0] = 0; }
            //for (int j = 0; j < W + 1; j++) { dpItems[0,j] = 0; }

            for (int tn = 1; tn < total + 1; tn++)  // 遍历每一件物品
            {

                for (int rw = 1; rw < maxWeight + 1; rw++)   // 背包容量有多大就还要计算多少次
                {

                    if (rw < wtItems[tn])  // 当背包容量小于第 tn 件物品重量时，只能放入前tn-1件
                    {

                        dpItems[tn, rw] = dpItems[tn - 1, rw];
                    }
                    else
                    {
                        // 当背包容量还大于第tn件物品重量时，进一步作出决策
                        dpItems[tn, rw] = Math.Max(dpItems[tn - 1, rw], dpItems[tn - 1, rw - wtItems[tn]] + valItems[tn]);
                    }
                }
            }

            return dpItems[total, maxWeight];
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="wtItems">物品的重量</param>
        /// <param name="valItems">物品的价值</param>
        /// <param name="total">物品的总数</param>
        /// <param name="maxWeight">背包能容纳的总重量</param>
        /// <param name="expectedMaxVal">物品价值总和最大值</param>
        [TestCaseSource("KnapsackSource")]
        public void KnapsackDPTest(int[] wtItems, int[] valItems, int total, int maxWeight, int expectedMaxVal)
        {
            var maxVal = KnapsackDP(wtItems, valItems, total, maxWeight); // 输出答案
            Console.WriteLine($"KnapsackDPTest,maxVal:{maxVal},expectedMaxVal:{expectedMaxVal}");
            Assert.AreEqual(expectedMaxVal, maxVal);
        }
        /// <summary>
        /// 
        /// 
        /// </summary>
        /// <returns></returns>
        public static IEnumerable KnapsackSource()
        {
            yield return new TestCaseData(new int[] { 0, 3, 2, 1 }, new int[] { 0, 5, 2, 3 }, 3, 5, 8);
        }

        /// <summary>
        /// 完全背包问题（针对当前物品，应放入多少件当前物品，价值最大。）
        /// </summary>
        /// <param name="wtItems">物品的重量</param>
        /// <param name="valItems">物品的价值</param>
        /// <param name="total">物品的总数</param>
        /// <param name="maxWeight">背包能容纳的总重量</param>
        /// <returns></returns>
        public int Bag(int[] wtItems, int[] valItems, int total, int maxWeight)
        {
            // 创建备忘录
            int[,] dpItems = new int[total + 1, maxWeight + 1];

            // 初始化状态
            //for (int i = 0; i < total + 1; i++) { dpItems[i,0] = 0; }
            //for (int j = 0; j < maxWeight + 1; j++) { dpItems[0,j] = 0; }

            // 遍历每一件物品
            for (int tn = 1; tn < total + 1; tn++)
            {
                // 背包容量有多大就还要计算多少次
                for (int rw = 1; rw < maxWeight + 1; rw++)
                {
                    // tn % 2代表当前行的缓存索引
                    int ctn = tn % 2;
                    // 1 - ctn代表上一行的缓存索引
                    int ptn = 1 - ctn;

                    dpItems[ctn, rw] = dpItems[ptn, rw];
                    // 如果可以放入则尝试放入第tn件物品
                    if (wtItems[tn] <= rw)
                    {
                        dpItems[ctn, rw] = Math.Max(dpItems[ctn, rw], dpItems[ctn, rw - wtItems[tn]] + valItems[tn]);
                    }
                }
            }
            return dpItems[total % 2, maxWeight];
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="wtItems">物品的重量</param>
        /// <param name="valItems">物品的价值</param>
        /// <param name="total">物品的总数</param>
        /// <param name="maxWeight">背包能容纳的总重量</param>
        /// <param name="expectedMaxVal">物品价值总和最大值</param>
        [TestCaseSource("BagSource")]
        public void BagTest(int[] wtItems, int[] valItems, int total, int maxWeight, int expectedMaxVal)
        {
            var maxVal = Bag(wtItems, valItems, total, maxWeight); // 输出答案
            Console.WriteLine($"BagTest,maxVal:{maxVal},expectedMaxVal:{expectedMaxVal}");
            Assert.AreEqual(expectedMaxVal, maxVal);
        }

        /// <summary>
        /// 
        /// 
        /// </summary>
        /// <returns></returns>
        public static IEnumerable BagSource()
        {
            yield return new TestCaseData(new int[] { 0, 3, 2, 1 }, new int[] { 0, 5, 2, 3 }, 3, 5, 15);
        }

    }
}
