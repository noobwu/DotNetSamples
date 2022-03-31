using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NoobCore.Tests.Algorithms
{
    /// <summary>
    /// 
    /// </summary>
    public class FinancialCalculator
    {
        /// <summary>
        /// Compoundings the interest.
        /// </summary>
        /// <param name="initAmount">初始投资金额</param>
        /// <param name="interest">年利率</param>
        /// <param name="years">投资年限</param>
        /// <param name="timesPerYear">每年投资次数</param>
        /// <returns></returns>
        public static double CompoundingInterest(double initAmount, double interest, int years, int timesPerYear)
        {
            // Using the formula P(1+r/n)(nt) from TheCalculatorSite.com
            // https://www.thecalculatorsite.com/articles/finance/compound-interest-formula.php?page=2

            double returnResult = 0;
            try
            {
                returnResult = initAmount * Math.Pow((1 + (interest / timesPerYear)), (timesPerYear * years));
            }
            catch
            {
                throw;
            }

            return returnResult;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="initAmount"></param>
        /// <param name="monthlyDeposit"></param>
        /// <param name="interest"></param>
        /// <param name="years"></param>
        /// <param name="timesPerYear"></param>
        /// <returns></returns>
        public static double CompoundingInterestWithDeposits(double initAmount, double monthlyDeposit, double interest, int years, int timesPerYear)
        {
            // Using the formula P(1+r/n)(nt) + PMT × {[(1 + r/n)(nt) - 1] / (r/n)} from TheCalculatorSite.com
            // https://www.thecalculatorsite.com/articles/finance/compound-interest-formula.php?page=2

            double tmpInitAmount = 0;
            double pmtAmount = 0;
            double periodInterest = 0;
            double exponent = 0;

            try
            {
                tmpInitAmount = CompoundingInterest(initAmount, interest, years, timesPerYear);
                periodInterest = interest / timesPerYear;
                exponent = timesPerYear * years;
                pmtAmount = monthlyDeposit * (((Math.Pow((1 + periodInterest), exponent)) - 1) / periodInterest);
            }
            catch (Exception)
            {
                throw;
            }
            return tmpInitAmount + pmtAmount;
        }
        /// <summary>
        /// 按揭
        /// </summary>
        public class MortgageAnalysis
        {
            /// <summary>
            /// 
            /// </summary>
            double _OriginalAmt;
            /// <summary>
            /// 
            /// </summary>
            double _downPaymentPct;
            /// <summary>
            /// 
            /// </summary>
            int _loanInMonths;
            /// <summary>
            /// 
            /// </summary>
            double _interestRate;
            /// <summary>
            /// 
            /// </summary>
            DateTime _startDate;
            /// <summary>
            /// 
            /// </summary>
            public double OriginalAmount { get; set; }

            /// <summary>
            /// 
            /// </summary>
            public double DownPaymentPercent
            {
                get { return _downPaymentPct; }
                set { if (value > 1) { _downPaymentPct = (value / 100); } else { _downPaymentPct = value; } }
            }

            /// <summary>
            /// 
            /// </summary>
            public int LoanPeriodInMonths
            {
                get { return _loanInMonths; }
                set { _loanInMonths = value; }
            }

            /// <summary>
            /// 
            /// </summary>
            public double InterestRate
            {
                get { return _interestRate; }
                set { if (value >= 1) { _interestRate = (value / 100); } else { _interestRate = value; } }
            }

            /// <summary>
            /// 
            /// </summary>
            public DateTime LoanStartDate
            {
                get { return _startDate; }
                set { _startDate = value; }
            }

            /// <summary>
            /// 
            /// </summary>
            public MortgageAnalysis()
            {
                // Basic constructor defining start date of the loan as today.
                _OriginalAmt = 0;
                _downPaymentPct = 0;
                _loanInMonths = 0;
                _interestRate = 0;
                _startDate = DateTime.Today;
            }

            /// <summary>
            /// 
            /// </summary>
            /// <param name="SalePrice"></param>
            /// <param name="DownPaymentPct"></param>
            /// <param name="LoanInMonths"></param>
            /// <param name="InterestRate"></param>
            /// <param name="StartDate"></param>
            public MortgageAnalysis(double SalePrice, double DownPaymentPct, int LoanInMonths, double InterestRate, DateTime StartDate)
            {
                _OriginalAmt = SalePrice;
                _downPaymentPct = DownPaymentPct;
                _loanInMonths = LoanInMonths;
                _interestRate = (InterestRate / 100);
                _startDate = StartDate;
            }
            /// <summary>
            /// 
            /// </summary>
            /// <param name="SalePrice"></param>
            /// <param name="DownPaymentPct"></param>
            /// <param name="LoanInMonths"></param>
            /// <param name="InterestRate"></param>

            public MortgageAnalysis(double SalePrice, double DownPaymentPct, int LoanInMonths, double InterestRate)
            {
                _OriginalAmt = SalePrice;
                _downPaymentPct = DownPaymentPct;
                _loanInMonths = LoanInMonths;
                _interestRate = (InterestRate / 100);
                _startDate = DateTime.Today;
            }

            /// <summary>
            /// 
            /// </summary>
            /// <returns></returns>
            public double LoanPrincipal()
            {
                double returnValue;

                // Return loan principal as total of loan minus interest.

                try
                {
                    if (_downPaymentPct == 0)
                        returnValue = _OriginalAmt;
                    else if (_OriginalAmt > 0 && _downPaymentPct > 0)
                        returnValue = (_OriginalAmt - (_OriginalAmt * _downPaymentPct));
                    else
                        returnValue = 0;
                }
                catch (Exception)
                {
                    throw;
                } 

                return returnValue;
            }

            /// <summary>
            /// 
            /// </summary>
            /// <returns></returns>
            public double TotalLoanAmount()
            {
                double returnValue;

                try
                {
                    // Return total of loan as monthly payment multiplied by months.
                    if (_loanInMonths > 0)
                        returnValue = (MonthlyPayment() * _loanInMonths);
                    else
                        returnValue = 0;
                }
                catch (Exception ex)
                {
                    throw ex;
                }

                return returnValue;
            }

            /// <summary>
            /// 
            /// </summary>
            /// <returns></returns>
            public double TotalInterest()
            {
                double returnValue;

                // Return total interest as total of loan minus the principal.
                // Return 0 if the monthly payment has not been set.
                if (MonthlyPayment() > 0)
                {
                    returnValue = Math.Round((TotalLoanAmount() - LoanPrincipal()), 2);
                }
                else
                {
                    returnValue = 0;
                }

                return returnValue;
            }

            /// <summary>
            /// 
            /// </summary>
            /// <returns></returns>
            public double MonthlyPayment()
            {
                // Calculating the monthly mortgage payment from formula at:
                // https://www.wikihow.com/Calculate-Mortgage-Payments
                // P *((r * (Math.Pow(1 + r, n))) / (Math.Pow(1 + r, n)–1))

                double returnPayment = 0;
                double monthlyInterest = _interestRate / 12;
                double loanAmt = LoanPrincipal();

                try
                {
                    if (loanAmt > 0 && _interestRate > 0)
                    {
                        returnPayment = loanAmt * ((monthlyInterest * Math.Pow((1 + monthlyInterest), _loanInMonths))
                            / (Math.Pow(1 + monthlyInterest, _loanInMonths) - 1));
                    }
                    else
                        returnPayment = 0;
                }
                catch (Exception)
                {
                    throw;
                }

                return returnPayment;
            }

            /// <summary>
            /// 
            /// </summary>
            /// <returns></returns>
            public DateTime LoanCompletionDate()
            {
                DateTime returnDate;

                try
                {
                    // Return loan completion date.
                    if (_loanInMonths > 0)
                    {
                        returnDate = _startDate.AddMonths(_loanInMonths);
                    }
                    else
                    {
                        returnDate = DateTime.Today;
                    }
                }
                catch (Exception)
                {
                    throw;
                }

                return returnDate;
            }
        }
    }
}
