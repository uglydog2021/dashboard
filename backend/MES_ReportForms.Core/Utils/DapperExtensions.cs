using Dapper;
using System.Data;

namespace MES_ReportForms.Utilities
{
    /// <summary>
    /// 补充 Dapper 扩展方法
    /// </summary>
    public static class DapperExtensions
    {
        public static int ExecuteTotal(this IDbConnection connection, string sql, object param = null)
        {
            return connection.ExecuteScalar<int>($"SELECT COUNT(*) FROM ({sql}) subQuery", param);
        }
    }
}
