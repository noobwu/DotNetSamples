using System;

namespace NoobCore.Tests
{
    public class Config
    {
        /// <summary>
        /// The rabbit mq connection string
        /// </summary>
        public static readonly string RabbitMQConnString = Environment.GetEnvironmentVariable("CI_RABBITMQ") ?? "localhost";
        /// <summary>
        /// The SQL server connection string
        /// </summary>
        public static readonly string SqlServerConnString = Environment.GetEnvironmentVariable("MSSQL_CONNECTION")
                                            ?? "Server=localhost;Database=test;User Id=test;Password=test;";
    }
}