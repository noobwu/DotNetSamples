// ***********************************************************************
// Assembly         : NoobCore.Tests
// Author           : Administrator
// Created          : 06-29-2022
//
// Last Modified By : Administrator
// Last Modified On : 06-29-2022
// ***********************************************************************
// <copyright file="Log4NetTests.cs" company="NoobCore.Tests">
//     Copyright (c) . All rights reserved.
// </copyright>
// <summary></summary>
// ***********************************************************************
using log4net;
using log4net.Config;
using Microsoft.Extensions.Logging;
using NoobCore.Helper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NoobCore.Tests.Logging
{
    /// <summary>
    /// Class Log4NetTests.
    /// </summary>
    public class Log4NetTests
    {
        //文件日志记录器
        /// <summary>
        /// The file log
        /// </summary>
        private readonly ILog _fileLog = null;
        /// <summary>
        /// Initializes a new instance of the <see cref="Log4NetTests"/> class.
        /// </summary>
        public Log4NetTests() {
            _fileLog=LogManager.GetLogger("FileLogger");
            XmlConfigurator.ConfigureAndWatch(new FileInfo(Path.Combine(Thread.GetDomain().BaseDirectory, "Config/log4net.config")));
        }

        /// <summary>
        /// Gets the API log string.
        /// </summary>
        /// <param name="log">The log.</param>
        /// <param name="logLevel">The log level.</param>
        /// <returns>System.String.</returns>
        private  string GetApiLogString(ApiRequestLog log, LogLevel logLevel)
        {
            StringBuilder builder = new StringBuilder();
            builder.AppendLine($"[{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")}][{log.RequestId}]");
            builder.AppendLine($"******************************************************************");
            builder.AppendLine($"【日志类型】：{logLevel.ToString()}");
            builder.AppendLine($"【记录时间】：{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")}");
            builder.AppendLine($"【记录机器】：{GlobalHelper.GetMachineName()}");
            //builder.AppendLine($"【客户端IP】：{RequestHelper.GetRemoteIP()}");
            builder.AppendLine($"【请求地址】：{log.RequestUri}");
            builder.AppendLine($"【请求控制器】：{log.ControllerName}");
            builder.AppendLine($"【请求Action】：{log.ActionName}");
            builder.AppendLine($"【请求方式】：{log.RequestMethod}");
            builder.AppendLine($"【请求耗时】：{log.ExecuteTime}");
            builder.AppendLine($"【请求参数】");
            builder.AppendLine(log.RequestBody);
            builder.AppendLine($"【返回结果】");
            builder.AppendLine(log.ResponseResult);
            builder.AppendLine("******************************************************************");
            builder.AppendLine();
            return builder.ToString();
        }
    }

    /// <summary>
    /// Class ApiRequestLog.
    /// </summary>
    public class ApiRequestLog
    {
        /// <summary>
        /// 请求的地址
        /// </summary>
        /// <value>The request URI.</value>
        public string RequestUri { get; set; }

        /// <summary>
        /// 请求方式
        /// </summary>
        /// <value>The request method.</value>
        public string RequestMethod { get; set; }

        /// <summary>
        /// controller名字
        /// </summary>
        /// <value>The name of the controller.</value>
        public string ControllerName { get; set; }
        /// <summary>
        /// 操作的action
        /// </summary>
        /// <value>The name of the action.</value>
        public string ActionName { get; set; }
        /// <summary>
        /// Ip地址
        /// </summary>
        /// <value>The ip.</value>
        public string Ip { get; set; }

        /// <summary>
        /// Gets or sets the request identifier.
        /// </summary>
        /// <value>The request identifier.</value>
        public string RequestId { get; set; }

        /// <summary>
        /// Gets or sets the request enter time.
        /// </summary>
        /// <value>The request enter time.</value>
        public DateTime? RequestEnterTime { get; set; }

        /// <summary>
        /// Gets or sets the execute time.
        /// </summary>
        /// <value>The execute time.</value>
        public double? ExecuteTime { get; set; }

        /// <summary>
        /// 请求携带的参数
        /// </summary>
        /// <value>The request body.</value>
        public string RequestBody { get; set; }

        /// <summary>
        /// 执行结果
        /// </summary>
        /// <value>The response result.</value>
        public string ResponseResult { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="ApiRequestLog" /> class.
        /// </summary>
        public ApiRequestLog()
        {
        }
    }

    /// <summary>
    /// Class HttpRequestLog.
    /// </summary>
    public class HttpRequestLog
    {
        /// <summary>
        /// 请求的地址
        /// </summary>
        /// <value>The request URI.</value>
        public string RequestUri { get; set; }

        /// <summary>
        /// 请求方式
        /// </summary>
        /// <value>The request method.</value>
        public string RequestMethod { get; set; }
        /// <summary>
        /// Ip地址
        /// </summary>
        /// <value>The ip.</value>
        public string Ip { get; set; }
        /// <summary>
        /// Gets or sets the execute time.
        /// </summary>
        /// <value>The execute time.</value>
        public double? ExecuteTime { get; set; }

        /// <summary>
        /// 请求内容类型
        /// </summary>
        /// <value>The type of the request content.</value>
        public string RequestContentType { get; set; }

        /// <summary>
        /// 请求携带的参数
        /// </summary>
        /// <value>The request body.</value>
        public string RequestBody { get; set; }

        /// <summary>
        /// 响应内容类型
        /// </summary>
        /// <value>The type of the response content.</value>
        public string ResponseContentType { get; set; }

        /// <summary>
        /// 响应结果
        /// </summary>
        /// <value>The response result.</value>
        public string ResponseResult { get; set; }
    }
}
