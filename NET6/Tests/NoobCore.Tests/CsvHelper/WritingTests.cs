using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CsvHelper;
using CsvHelper.Configuration;
using Microsoft.VisualStudio.TestPlatform.PlatformAbstractions.Interfaces;
using NUnit.Framework;
using NUnit.Framework.Internal;

namespace NoobCore.Tests.CsvHelper
{
    [TestFixture]
    public class WritingTests
    {
        /// <summary>
        /// Defines the test method ContainsDelimiterTest.
        /// carl.wu
        /// </summary>
        [TestCase]
        public void ContainsDelimiter()
        {
            using (var writer = new StringWriter())
            using (var csv = new CsvWriter(writer, CultureInfo.InvariantCulture))
            {
                csv.WriteField($"o{csv.Configuration.Delimiter}e");
                csv.Flush();
                Assert.AreEqual($"\"o{csv.Configuration.Delimiter}e\"", writer.ToString());
            }
        }
        /// <summary>
        /// Defines the test method WriteFieldFormatTest.
        /// carl.wu
        /// </summary>
        [TestCase]
        public void WriteFieldFormat()
        {
            var records = new List<TestRecord>{
                new TestRecord
                {
                    IntColumn = 1,
                    DateColumn = new DateTime(2012, 10, 1, 12, 12, 12),
                    DecimalColumn = 150.99m,
                    FirstColumn = "first column",
                    Mobile="\t13788888888\t"
                }
            };
            string csvFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, $@"WriteFieldFormat-{DateTime.Now.ToString("yyyyMMddHHmmss")}.csv");
            using (var writer = new StreamWriter(csvFilePath) { AutoFlush = true })
            using (var csv = new CsvWriter(writer, CultureInfo.InvariantCulture))
            {
                csv.Context.RegisterClassMap<TestRecordMap>();
                csv.WriteRecords(records);
            }
            Assert.IsTrue(File.Exists(csvFilePath));
        }

        /// <summary>
        /// Class TestRecord.
        /// </summary>
        private class TestRecord
        {
            /// <summary>
            /// Gets or sets the int column.
            /// </summary>
            /// <value>The int column.</value>
            public int IntColumn { get; set; }

            /// <summary>
            /// Gets or sets the date column.
            /// </summary>
            /// <value>The date column.</value>
            public DateTime DateColumn { get; set; }

            /// <summary>
            /// Gets or sets the decimal column.
            /// </summary>
            /// <value>The decimal column.</value>
            public decimal DecimalColumn { get; set; }

            /// <summary>
            /// Gets or sets the first column.
            /// </summary>
            /// <value>The first column.</value>
            public string FirstColumn { get; set; }
            /// <summary>
            /// Gets or sets the mobile.
            /// </summary>
            /// <value>The mobile.</value>
            public string Mobile { get; set; }
        }

        /// <summary>
        /// Class TestRecordMap. This class cannot be inherited.
        /// Implements the <see cref="CsvHelper.Configuration.ClassMap{Kmmp.Core.NUnitTests.CsvHelperTests.WritingTests.TestRecord}" />
        /// </summary>
        /// <seealso cref="CsvHelper.Configuration.ClassMap{Kmmp.Core.NUnitTests.CsvHelperTests.WritingTests.TestRecord}" />
        private sealed class TestRecordMap : ClassMap<TestRecord>
        {
            /// <summary>
            /// Initializes a new instance of the <see cref="TestRecordMap" /> class.
            /// </summary>
            public TestRecordMap()
            {
                Map(m => m.FirstColumn).Name("FirstColumn").Index(0);
                Map(m => m.IntColumn).Name("Int Column").Index(1).TypeConverterOption.Format("0000");
                Map(m => m.DateColumn).Name("DateColumn").Index(2).TypeConverterOption.Format("yyyy-MM-dd HH:mm:ss");
                Map(m => m.DecimalColumn).Name("DecimalColumn").Index(3).TypeConverterOption.Format("F2");
                Map(m => m.Mobile).Name("MobileColumn").Index(4);
            }
        }


        [TestCase]
        public void AppendFile()
        {
            string csvFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, $@"AppendFile-{DateTime.Now.ToString("yyyyMMddHHmmss")}.csv");
            var records = new List<Foo>{
                new Foo { Id = 1, Name = "one" },
            };

            // Write to a file.
            using (var writer = new StreamWriter(csvFilePath))
            using (var csv = new CsvWriter(writer, CultureInfo.InvariantCulture))
            {
                csv.WriteRecords(records);
                csv.NextRecord();//空一行


            }


            records = new List<Foo>{
                 new Foo { Id = 2, Name = "two" },
            };

            // Append to the file.
            var config = new CsvConfiguration(CultureInfo.InvariantCulture)
            {
                //Delimiter = ",",
                // Don't write the header again.
                HasHeaderRecord = false,

            };
            using (var stream = File.Open(csvFilePath, FileMode.Append))
            using (var writer = new StreamWriter(stream))
            using (var csv = new CsvWriter(writer, config))
            {
                csv.WriteRecords(records);
            }
        }

        [TestCase]
        public void WriteMultipleFieldsFromSingleProperty()
        {
            string csvFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, $@"MultipleFields-{DateTime.Now.ToString("yyyyMMddHHmmss")}.csv");
            using (var stream = new MemoryStream())
            using (var reader = new StreamReader(stream))
            using (var writer = new StreamWriter(stream))
            //using (var writer = new StreamWriter(csvFilePath))
            using (var csv = new CsvWriter(writer, new CultureInfo("en-US")))
            {
                var records = new List<Test>
                {
                    new Test { Dob = DateTime.Parse( "9/6/2017" ) }
                };
                csv.Context.RegisterClassMap<TestMap>();
                csv.WriteRecords(records);
                writer.Flush();
                stream.Position = 0;

                var expected = new TestStringBuilder(csv.Configuration.NewLine);
                expected.AppendLine("A,B,C");
                expected.AppendLine("9/6/2017 12:00:00 AM,9/6/2017 12:00:00 AM,9/6/2017 12:00:00 AM");

                Assert.AreEqual(expected.ToString(), reader.ReadToEnd());
            }
        }
        private class Test
        {
            public DateTime Dob { get; set; }
        }

        /// <summary>
        /// 
        /// </summary>
        private sealed class TestMap : ClassMap<Test>
        {
            /// <summary>
            /// 
            /// </summary>
            public TestMap()
            {
                Map(m => m.Dob, false).Name("A");
                Map(m => m.Dob, false).Name("B");
                Map(m => m.Dob, false).Name("C");
            }
        }
        /// <summary>
        /// 
        /// </summary>
        [Test]
        public void MultipleHeaders()
        {

            using (var writer = new StringWriter())
            using (var csv = new CsvWriter(writer, CultureInfo.InvariantCulture))
            {
                csv.WriteHeader<A>();
                csv.NextRecord();
                csv.WriteRecord(new A { Id = 1 });
                csv.NextRecord();

                csv.WriteHeader<B>();
                csv.NextRecord();
                csv.WriteRecord(new B { Name = "one" });
                csv.NextRecord();
                writer.Flush();


                var expected = new TestStringBuilder(csv.Configuration.NewLine);
                expected.AppendLine("Id");
                expected.AppendLine("1");
                expected.AppendLine("Name");
                expected.AppendLine("one");

                Assert.AreEqual(expected.ToString(), writer.ToString());
            }
        }


        /// <summary>
        /// 
        /// </summary>
        private class A
        {
            /// <summary>
            /// 
            /// </summary>
            public int Id { get; set; }
        }

        /// <summary>
        /// 
        /// </summary>
        private class B
        {
            /// <summary>
            /// 
            /// </summary>
            public string Name { get; set; }
        }

        /// <summary>
        /// 
        /// </summary>
        [Test]
        public void WriteMultipleHeaders()
        {
            string csvFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, $@"MultipleHeaders-{DateTime.Now.ToString("yyyyMMddHHmmss")}.csv");
            using (var writer = new StreamWriter(csvFilePath))
            using (var csv = new CsvWriter(writer, CultureInfo.InvariantCulture))
            {
                csv.WriteHeader<A>();
                csv.NextRecord();
                csv.WriteRecord(new A { Id = 1 });
                csv.NextRecord();

                csv.WriteHeader<B>();
                csv.NextRecord();
                csv.WriteRecord(new B { Name = "one" });
                csv.NextRecord();
                writer.Flush();
            }
            using (var reader = new StreamReader(csvFilePath))
            {
                var expected = new TestStringBuilder("\r\n");
                expected.AppendLine("Id");
                expected.AppendLine("1");
                expected.AppendLine("Name");
                expected.AppendLine("one");

                Assert.AreEqual(expected.ToString(), reader.ReadToEnd());
            }
        }
    }

    /// <summary>
    /// Class Foo.
    /// </summary>
    public class Foo
    {
        /// <summary>
        /// Gets or sets the identifier.
        /// </summary>
        /// <value>The identifier.</value>
        public int Id { get; set; }
        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        /// <value>The name.</value>
        public string Name { get; set; }
    }


}
