using Dapper; 
using MES_ReportForms.Core.Utils;
using MES_ReportForms.Core.Entities.System;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System.Data;
using System.Xml.Linq;

namespace MES_ReportForms.Utilities
{
    /// <summary>
    /// 创建数据库连接的工厂
    /// </summary>
    public class DbConnectionFactory
    {
        // 缓存 Process Connection String 时长
        private const int PROCESS_CONNECTION_STRING_CACHE_SECONDS = 60 * 5;

        private readonly static string defaultConnectionString = new ConfigurationBuilder()
            .AddJsonFile("appsettings.json")
            .Build()
            .GetConnectionString("db.connectionString");

        public static IDbConnection CreateDefaultDbConnection()
        {
            return CreateDbConnection(defaultConnectionString);
        }

        public static IDbConnection CreateDbConnection(string connectionString)
        {
            if (!connectionString.Contains("TrustServerCertificate"))
                connectionString += ";TrustServerCertificate=True";
            return new SqlConnection(connectionString);
        }

        public static IDbConnection CreateProcessDbConnection(string connectionName)
        {
            string connectionString = GetConnectionStringWithCache(connectionName);
            return CreateDbConnection(connectionString);
        }

        public static string GetConnectionStringWithCache(string connectionName)
        {
            var func = () => FindConnectionString(connectionName);

            var connectionString = CacheHelper.CacheFunc(
                func,
                "ConnectionNameConn" + connectionName,
                PROCESS_CONNECTION_STRING_CACHE_SECONDS);

            return connectionString;
        }

        public static Task<string> FindConnectionString(string connectionName)
        {
            using var connection = CreateDefaultDbConnection();

            var dBConnection = connection.QueryFirstOrDefault<T_DBConnectionEntity>(
                "SELECT * FROM T_DBConnection WHERE DBName = @DBName",
                new { DBName = connectionName }
            );

            if (dBConnection == null)
                throw new BizException("数据库连接名有误，请检查是否有配置此连接名！");

            var sqlConnection = $"Data Source=.;Initial Catalog={dBConnection.DBName};Persist Security Info=True;User ID={dBConnection.Account};Password={dBConnection.PassWord};Min Pool Size=10;Max Pool Size=50;TrustServerCertificate=True;";

            return Task.FromResult(sqlConnection);
        }
    }
}
