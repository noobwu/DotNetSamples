using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using System.Collections;

namespace NoobCore.Tests.Algorithms
{
    /// <summary>
    /// 
    /// </summary>
    [TestFixture]
    public class FinancialCalculatorTests
    {

        /// <summary>
        /// Compoundings the interest.
        /// </summary>
        /// <param name="initAmount">The initialize amount.</param>
        /// <param name="interest">The interest.</param>
        /// <param name="years">The years.</param>
        /// <param name="timesPerYear">The times per year.</param>
        /// <returns></returns>
        [TestCaseSource("CompoundingInterestSource")]
        public static void CompoundingInterest(double initAmount, double interest, int years, int timesPerYear)
        {
           var result=  FinancialCalculator.CompoundingInterest(initAmount, interest, years, timesPerYear);
            Console.WriteLine($"result:{result}");

        }

        /// <summary>
        /// Compoundings the interest source.
        /// </summary>
        /// <returns></returns>
        public static IEnumerable CompoundingInterestSource()
        {
            yield return new TestCaseData(100000, 0.005,1,12);
        }
    }
}
