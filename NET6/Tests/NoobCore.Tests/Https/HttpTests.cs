// ***********************************************************************
// Assembly         : NoobCore.Tests
// Author           : Administrator
// Created          : 04-01-2022
//
// Last Modified By : Administrator
// Last Modified On : 04-01-2022
// ***********************************************************************
// <copyright file="HttpTests.cs" company="NoobCore.Tests">
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
using StackExchange.Utils;
using System.Net;

namespace NoobCore.Tests.Https
{
    /// <summary>
    /// Defines test class HttpTests.
    /// </summary>
    [TestFixture]
    public class HttpTests
    {
        /// <summary>
        /// Defines the test method BasicCreation.
        /// </summary>
        [TestCase]
        public async Task BasicCreation()
        {
            var request = Http.Request("https://example.com/")
                              .SendPlaintext("test")
                              .ExpectString();
            Assert.AreEqual("https://example.com/", request.Inner.Message.RequestUri.ToString());

            var content = request.Inner.Message.Content;
            Assert.IsInstanceOf<StringContent>(content);
            var stringContent = await content.ReadAsStringAsync();
            Assert.AreEqual("test", stringContent);
        }

        /// <summary>
        /// Defines the test method BasicDelete.
        /// </summary>
        [TestCase]
        public async Task BasicDelete()
        {
            var result = await Http.Request("https://httpbin.org/delete")
                                    .ExpectJson<HttpBinResponse>()
                                    .DeleteAsync();
            Assert.True(result.Success);
            Assert.AreEqual(HttpStatusCode.OK, result.StatusCode);
            Assert.AreEqual("https://httpbin.org/delete", result.Data.Url);
            Assert.AreEqual(Http.DefaultSettings.UserAgent, result.Data.Headers["User-Agent"]);
        }

    }
}
