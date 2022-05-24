// ***********************************************************************
// Assembly         : NoobCore.Tests
// Author           : Administrator
// Created          : 05-24-2022
//
// Last Modified By : Administrator
// Last Modified On : 05-24-2022
// ***********************************************************************
// <copyright file="TaskManagerTests.cs" company="NoobCore.Tests">
//     Copyright (c) . All rights reserved.
// </copyright>
// <summary></summary>
// ***********************************************************************
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NoobCore.Tests.BeautyOfProgramming
{
    /// <summary>
    /// Class TaskManagerTests.
    /// </summary>
    [TestFixture]
    public class TaskManagerTests
    {
        [TestCase]
        public void Sleep() {
            for (; ; ) {
                for (int i = 0; i < 96000000; i++) ;
                 Thread.Sleep(10);
            }
        }
        /// <summary>
        /// Defines the test method Sleep.
        /// </summary>
        [TestCase]
        public void TickCount()
        {
            const int busyTime = 10; //10 ms
            const int idleTime = busyTime; //10 ms
            int startTime = 0;
            while (true)
            {
                startTime = Environment.TickCount;
                while ((Environment.TickCount - startTime) <= busyTime) ;
                Thread.Sleep(idleTime);
            }
        }
    }
}
