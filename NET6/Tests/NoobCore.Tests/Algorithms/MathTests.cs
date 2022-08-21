using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;
using Xunit.Sdk;

namespace NoobCore.Tests.Algorithms
{
    public class MathTests
    {
        private readonly ITestOutputHelper _testOutputHelper;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="testOutputHelper"></param>
        public MathTests(ITestOutputHelper testOutputHelper)
        {
            _testOutputHelper = testOutputHelper;
        }
        /// <summary>
        /// 
        /// </summary>
        [Fact]
        public void Log() {
            _testOutputHelper.WriteLine($"0x5bd1e995:{Convert.ToInt32("5bd1e995",16)}");
            var res = Math.Log(2.0);
            _testOutputHelper.WriteLine($" Math.Log(2.0):{res}");
            var res1= Math.Log(2.0,Math.E);
            Assert.Equal(res,res1);
            _testOutputHelper.WriteLine($" Math.Log(2.0,Math.E):{res1}");

            var res2 = Math.Log(2,2.0);
            _testOutputHelper.WriteLine($"Math.Log(1,2.0):{res2}");
        }
    }
}
