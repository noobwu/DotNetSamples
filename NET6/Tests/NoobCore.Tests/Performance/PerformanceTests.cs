// ***********************************************************************
// Assembly         : NoobCore.Tests
// Author           : Administrator
// Created          : 2022-09-24
// Github           : https://github.com/Apress/pro-.net-perf/blob/master/Ch06/Ch06/Program.cs
//
// Last Modified By : Administrator
// Last Modified On : 2022-09-25

// ***********************************************************************
// <copyright file="PerformanceTests.cs" company="NoobCore.Tests">
//     Copyright (c) . All rights reserved.
// </copyright>
// <summary></summary>
// ***********************************************************************
using NoobCore.Helper;
using NUnit.Framework;
using System;
using System.Collections;
using System.Diagnostics;

/// <summary>
/// The Performance namespace.
/// </summary>
namespace NoobCore.Tests.Performance
{
    /// <summary>
    /// Class PerformanceTests.
    /// </summary>
    public class PerformanceTests
    {
        /// <summary>
        /// Returns all the prime numbers in the range [start, end)
        /// </summary>
        /// <param name="start">The start.</param>
        /// <param name="end">The end.</param>
        /// <returns>IEnumerable&lt;System.UInt32&gt;.</returns>
        public IEnumerable<uint> PrimesInRange(uint start, uint end)
        {
            List<uint> primes = new List<uint>();
            for (uint number = start; number < end; ++number)
            {
                if (IsPrime(number))
                {
                    primes.Add(number);
                }
            }
            return primes;
        }


        /// <summary>
        /// Determines whether the specified number is prime.
        /// </summary>
        /// <param name="number">The number.</param>
        /// <returns><c>true</c> if the specified number is prime; otherwise, <c>false</c>.</returns>
        private bool IsPrime(uint number)
        {
            //This is a very inefficient O(n) algorithm, but it will do for our expository purposes
            if (number == 2) return true;
            if (number % 2 == 0) return false;
            for (uint divisor = 3; divisor < number; divisor += 2)
            {
                if (number % divisor == 0) return false;
            }
            return true;
        }

        /// <summary>
        /// Primeses the in range threads.
        /// </summary>
        /// <param name="start">The start.</param>
        /// <param name="end">The end.</param>
        /// <returns>IEnumerable&lt;System.UInt32&gt;.</returns>
        public IEnumerable<uint> PrimesInRange_Threads(uint start, uint end)
        {
            List<uint> primes = new List<uint>();
            uint range = end - start;
            uint numThreads = (uint)Environment.ProcessorCount; //is this a good idea?
            uint chunk = range / numThreads; //hopefully, there is no remainder
            Thread[] threads = new Thread[numThreads];
            for (uint i = 0; i < numThreads; ++i)
            {
                uint chunkStart = start + i * chunk;
                uint chunkEnd = chunkStart + chunk;
                threads[i] = new Thread(() =>
                {
                    for (uint number = chunkStart; number < chunkEnd; ++number)
                    {
                        if (IsPrime(number))
                        {
                            lock (primes)
                            {
                                primes.Add(number);
                            }
                        }
                    }
                });
                threads[i].Start();
            }
            foreach (Thread thread in threads)
            {
                thread.Join();
            }
            return primes;
        }

        /// <summary>
        /// Primeses the in range threads test.
        /// </summary>
        /// <param name="start">The start.</param>
        /// <param name="end">The end.</param>
        [TestCaseSource(nameof(PrimesInRange_Test_Source))]
        public void PrimesInRange_Threads_Test(uint start, uint end)
        {
            Measure(() => PrimesInRange_Threads(start, end), $"PrimesInRange_Threads({start}, {end})");
        }

        /// <summary>
        /// Primeses the in range thread pool.
        /// </summary>
        /// <param name="start">The start.</param>
        /// <param name="end">The end.</param>
        /// <returns>IEnumerable&lt;System.UInt32&gt;.</returns>
        public IEnumerable<uint> PrimesInRange_ThreadPool(uint start, uint end)
        {
            List<uint> primes = new List<uint>();
            const uint ChunkSize = 100;
            int completed = 0;
            ManualResetEvent allDone = new ManualResetEvent(initialState: false);
            uint chunks = (end - start) / ChunkSize; //again, this should divide evenly
            for (uint i = 0; i < chunks; ++i)
            {
                uint chunkStart = start + i * ChunkSize;
                uint chunkEnd = chunkStart + ChunkSize;
                ThreadPool.QueueUserWorkItem(_ =>
                {
                    for (uint number = chunkStart; number < chunkEnd; ++number)
                    {
                        if (IsPrime(number))
                        {
                            lock (primes)
                            {
                                primes.Add(number);
                            }
                        }
                    }
                    if (Interlocked.Increment(ref completed) == chunks)
                    {
                        allDone.Set();
                    }
                });
            }
            allDone.WaitOne();
            return primes;
        }

        /// <summary>
        /// Primeses the in range threads test.
        /// </summary>
        /// <param name="start">The start.</param>
        /// <param name="end">The end.</param>
        [TestCaseSource(nameof(PrimesInRange_Test_Source))]
        public void PrimesInRange_ThreadPool_Test(uint start, uint end)
        {
            Measure(() => PrimesInRange_ThreadPool(start, end), $"PrimesInRange_ThreadPool({start}, {end})");
        }



        /// <summary>
        /// Primeses the in range parallel for.
        /// </summary>
        /// <param name="start">The start.</param>
        /// <param name="end">The end.</param>
        /// <returns>IEnumerable&lt;System.UInt32&gt;.</returns>
        public IEnumerable<uint> PrimesInRange_ParallelFor(uint start, uint end)
        {
            List<uint> primes = new List<uint>();
            Parallel.For((long)start, (long)end, number =>
            {
                if (IsPrime((uint)number))
                {
                    lock (primes)
                    {
                        primes.Add((uint)number);
                    }
                }
            });
            return primes;
        }

        /// <summary>
        /// Primeses the in range aggregation.
        /// </summary>
        /// <param name="start">The start.</param>
        /// <param name="end">The end.</param>
        /// <returns>IEnumerable&lt;System.UInt32&gt;.</returns>
        public IEnumerable<uint> PrimesInRange_Aggregation(uint start, uint end)
        {
            List<uint> primes = new List<uint>();
            Parallel.For(3, 200000,
              () => new List<uint>(),        //initialize the local copy
              (i, pls, localPrimes) =>
              {    //single computation step, returns new local state
                  if (IsPrime((uint)i))
                  {
                      localPrimes.Add((uint)i);       //no synchronization necessary, thread-local state
                  }
                  return localPrimes;
              },
              localPrimes =>
              {              //combine the local lists to the global one
                  lock (primes)
                  {              //synchronization is required
                      primes.AddRange(localPrimes);
                  }
              }
            );
            return primes;
        }
        /// <summary>
        /// Primeses the in range threads test.
        /// </summary>
        /// <param name="start">The start.</param>
        /// <param name="end">The end.</param>
        [TestCaseSource(nameof(PrimesInRange_Test_Source))]
        public void PrimesInRange_Tests(uint start, uint end)
        {
            Measure(() => PrimesInRange(start, end), $"PrimesInRange({start}, {end})");
            Measure(() => PrimesInRange_Threads(start, end), $"PrimesInRange_Threads({start}, {end})");
            Measure(() => PrimesInRange_ThreadPool(start, end), $"PrimesInRange_ThreadPool({start}, {end})");
            Measure(() => PrimesInRange_ParallelFor(start, end), $"PrimesInRange_ParallelFor({start}, {end})");
            Measure(() => PrimesInRange_Aggregation(start, end), $"PrimesInRange_Aggregation({start}, {end})");

        }

        /// <summary>
        /// Primeses the in range test source.
        /// </summary>
        /// <returns>IEnumerable.</returns>
        public static IEnumerable PrimesInRange_Test_Source()
        {
            yield return new TestCaseData((uint)100, (uint)200000);
        }




        /// <summary>
        /// Quicks the sort sequential.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="items">The items.</param>
        public void QuickSort_Sequential<T>(T[] items) where T : IComparable<T>
        {
            QuickSort_Sequential(items, 0, items.Length);
        }

        /// <summary>
        /// Quicks the sort sequential.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="items">The items.</param>
        /// <param name="left">The left.</param>
        /// <param name="right">The right.</param>
        private void QuickSort_Sequential<T>(T[] items, int left, int right) where T : IComparable<T>
        {
            if (left == right) return;
            int pivot = Partition(items, left, right);
            QuickSort_Sequential(items, left, pivot);
            QuickSort_Sequential(items, pivot + 1, right);
        }

        /// <summary>
        /// Partitions the specified items.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="items">The items.</param>
        /// <param name="left">The left.</param>
        /// <param name="right">The right.</param>
        /// <returns>System.Int32.</returns>
        private int Partition<T>(T[] items, int left, int right) where T : IComparable<T>
        {
            int pivotPos = (right - left) / 2; //often a random index between left and right is used
            T pivotValue = items[pivotPos];
            Swap(ref items[right - 1], ref items[pivotPos]);
            int store = left;
            for (int i = left; i < right - 1; ++i)
            {
                if (items[i].CompareTo(pivotValue) < 0)
                {
                    Swap(ref items[i], ref items[store]);
                    ++store;
                }
            }
            Swap(ref items[right - 1], ref items[store]);
            return store;
        }

        /// <summary>
        /// Swaps the specified a.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="a">a.</param>
        /// <param name="b">The b.</param>
        private void Swap<T>(ref T a, ref T b)
        {
            T temp = a;
            a = b;
            b = temp;
        }

        /// <summary>
        /// Quicks the sort tests.
        /// </summary>
        /// <param name="start">The start.</param>
        /// <param name="end">The end.</param>
        [TestCaseSource(nameof(QuickSort_Test_Source))]
        public void QuickSort_Tests(int start, int end)
        {
            Random rnd = new Random();
            Measure(() => Enumerable.Range(start, end).Select(n => rnd.Next()).ToArray(), QuickSort_Sequential, $"QuickSort_Sequential ({end} including allocation)");
        }


        /// <summary>
        /// Primeses the in range test source.
        /// </summary>
        /// <returns>IEnumerable.</returns>
        public static IEnumerable QuickSort_Test_Source()
        {
            yield return new TestCaseData(0, 1000000);
        }

        /// <summary>
        /// Measures the specified what.
        /// </summary>
        /// <param name="what">The what.</param>
        /// <param name="description">The description.</param>
        private void Measure(Action what, string description)
        {
            const int ITERATIONS = 5;
            double[] elapsed = new double[ITERATIONS];
            for (int i = 0; i < ITERATIONS; ++i)
            {
                Stopwatch sw = Stopwatch.StartNew();
                what();
                elapsed[i] = sw.ElapsedMilliseconds;
            }
            Console.WriteLine($"{description} took {elapsed.Skip(1).Average()}ms on average");
        }
        /// <summary>
        /// Measures the specified setup.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="setup">The setup.</param>
        /// <param name="measurement">The measurement.</param>
        /// <param name="description">The description.</param>
        private void Measure<T>(Func<T> setup, Action<T> measurement, string description)
        {
            T state = setup();
            Measure(() => measurement(state), description);
        }

        /// <summary>
        /// Repeats the specified times.
        /// </summary>
        /// <param name="times">The times.</param>
        /// <param name="action">The action.</param>
        private void Repeat(int times, Action action)
        {
            for (int i = 0; i < times; ++i)
                action();
        }

    }
}
