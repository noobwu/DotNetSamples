// ***********************************************************************
// Assembly         : NoobCore.Tests
// Author           : Administrator
// Created          : 05-07-2022
//
// Last Modified By : Administrator
// Last Modified On : 05-07-2022
// ***********************************************************************
// <copyright file="BitwiseTests.cs" company="NoobCore.Tests">
//     Copyright (c) . All rights reserved.
// </copyright>
// <summary></summary>
// ***********************************************************************
using NUnit.Framework;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NoobCore.Tests.Enums
{
    /// <summary>
    /// Defines test class BitwiseTests.
    /// </summary>
    [TestFixture]
    public class BitwiseTests
    {
        /// <summary>
        /// Ands this instance.
        /// </summary>
        /// <param name="useScopeA">The use scope a.</param>
        /// <param name="useScopeB">The use scope b.</param>
        /// <param name="expectedUseScope">The expected use scope.</param>
        [TestCaseSource(nameof(AndSource))]
        public void And(byte useScopeA, byte useScopeB, byte expectedUseScope)
        {
            var andResult = useScopeA & useScopeB;
            Console.WriteLine($"useScopeA:{useScopeA},useScopeB:{useScopeB},andResult:{andResult},expectedUseScope:{expectedUseScope}");
            Assert.AreEqual(expectedUseScope, andResult);
        }

        /// <summary>
        /// Ands the source.
        /// </summary>
        /// <returns>IEnumerable.</returns>
        public static IEnumerable AndSource()
        {
            yield return new TestCaseData((byte)TicketUseScope.Limitless, (byte)TicketUseScope.Online, (byte)TicketUseScope.Online);
            yield return new TestCaseData((byte)TicketUseScope.Limitless, (byte)TicketUseScope.Offline, (byte)TicketUseScope.Offline);
            yield return new TestCaseData((byte)TicketUseScope.Limitless, (byte)TicketUseScope.Limitless, (byte)TicketUseScope.Limitless);
            yield return new TestCaseData((byte)TicketUseScope.Online, (byte)TicketUseScope.Online, (byte)TicketUseScope.Online);
            yield return new TestCaseData((byte)TicketUseScope.Online, (byte)TicketUseScope.Offline, byte.MinValue);
            yield return new TestCaseData((byte)TicketUseScope.Online, (byte)TicketUseScope.Limitless, (byte)TicketUseScope.Online);
            yield return new TestCaseData((byte)TicketUseScope.Offline, (byte)TicketUseScope.Online, byte.MinValue);
            yield return new TestCaseData((byte)TicketUseScope.Offline, (byte)TicketUseScope.Offline, (byte)TicketUseScope.Offline);
            yield return new TestCaseData((byte)TicketUseScope.Offline, (byte)TicketUseScope.Limitless, (byte)TicketUseScope.Offline);
        }

        /// <summary>
        /// Ors the specified gift types.
        /// </summary>
        /// <param name="giftTypes">The gift types.</param>
        /// <param name="expected">The expected.</param>
        [TestCaseSource(nameof(OrSource))]
        public void Or(GiftType[] giftTypes, int expected) {
            int orResult = (int)GiftType.None;
            foreach (var item in giftTypes)
            {
                orResult |=(int)item;
            }
            Console.WriteLine($"giftTypes:{string.Join(",",giftTypes)},expected:{expected},orResult:{orResult}");
            Assert.AreEqual(expected, orResult);
        }

        /// <summary>
        /// Ors the source.
        /// </summary>
        /// <returns>IEnumerable.</returns>
        public static IEnumerable OrSource()
        {
            yield return new TestCaseData(new GiftType[] { GiftType.Ticket },(int)GiftType.Ticket);
            yield return new TestCaseData(new GiftType[] { GiftType.Balance }, (int)GiftType.Balance);
            yield return new TestCaseData(new GiftType[] { GiftType.Integral}, (int)GiftType.Integral);
            yield return new TestCaseData(new GiftType[] { GiftType.Ticket, GiftType.Balance },3);
            yield return new TestCaseData(new GiftType[] { GiftType.Ticket, GiftType.Integral }, 5);
            yield return new TestCaseData(new GiftType[] { GiftType.Balance, GiftType.Integral }, 6);
            yield return new TestCaseData(new GiftType[] { GiftType.Ticket,GiftType.Balance, GiftType.Integral }, 7);
        }
    }
}
