using System;
namespace NoobCore.Tests
{
    /// <summary>
    /// 
    /// </summary>
    public class TestsConfig
    {
        /// <summary>
        /// The rabbit mq host
        /// </summary>
        public static readonly string RabbitMqHost = Environment.GetEnvironmentVariable("CI_RABBITMQ") ?? "localhost";

        /// <summary>
        /// The SQL server connection string
        /// </summary>
        public static readonly string SqlServerConnString = Environment.GetEnvironmentVariable("MSSQL_CONNECTION") ?? "Server=localhost;Database=test;User Id=test;Password=test;";
        /// <summary>
        /// The postgre SQL connection string
        /// </summary>
        public static readonly string PostgreSqlConnString = Environment.GetEnvironmentVariable("PGSQL_CONNECTION") ?? "Server=localhost;Port=5432;User Id=test;Password=test;Database=test;Pooling=true;MinPoolSize=0;MaxPoolSize=200";
       
    }
}