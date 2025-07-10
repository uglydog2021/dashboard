using Dapper;
using MES_ReportForms.Utilities;
using Microsoft.Data.SqlClient;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq.Expressions;
using System.Reflection;
using System.Security.Cryptography;

namespace MES_ReportForms.Core.Repositories.Sql
{
    public class ProcessRepositoryBase
    {
        protected string ConnectionName { get; set; }
        protected string ConnectionString { get; init; }

        public ProcessRepositoryBase(string connectionName)
        {
            if (!string.IsNullOrWhiteSpace(connectionName))
            {
                ConnectionName = connectionName;
                ConnectionString = DbConnectionFactory.GetConnectionStringWithCache(connectionName);
            }
        }
         
        /// <summary>
        /// Process 数据库（配置在主数据库 Process 表中的 ConnectionString 列）
        /// </summary>
        protected IDbConnection DBCon
        {
            get
            {
                if (!string.IsNullOrEmpty(ConnectionName))
                    return DbConnectionFactory.CreateDbConnection(ConnectionString);
                else
                    return DbConnectionFactory.CreateDefaultDbConnection();
            }
        }

        public bool SqlBulkCopyInsert(DataTable TBData)
        {
            var sqlConnection = DBCon as SqlConnection;
            if (sqlConnection == null)
            {
                throw new InvalidOperationException("IDbConnection must be of type SqlConnection.");
            }
            sqlConnection.Open();
            using (var transaction = sqlConnection.BeginTransaction())
            {
                try
                {
                    using (var bulkCopy = new SqlBulkCopy(sqlConnection, SqlBulkCopyOptions.Default, transaction))
                    {
                        bulkCopy.DestinationTableName = TBData.TableName;
                        bulkCopy.NotifyAfter = TBData.Rows.Count;

                        bulkCopy.WriteToServer(TBData);

                        transaction.Commit();
                    }
                }
                catch (Exception)
                {
                    transaction.Rollback();
                    sqlConnection.Close();

                    return false;
                }
                finally
                {
                    sqlConnection.Close();
                }
            }

            return true;
        }

        // 将 List 转换为 DataTable
        public DataTable ConvertToDataTable<T>(List<T> items,string tableName)
        {
            DataTable dataTable = new DataTable();

            // 获取类型的属性
            PropertyInfo[] properties = typeof(T).GetProperties();

            // 为 DataTable 添加列
            foreach (var property in properties)
            {
                dataTable.Columns.Add(property.Name, property.PropertyType);
            }

            // 为 DataTable 添加行
            foreach (var item in items)
            {
                DataRow row = dataTable.NewRow();
                foreach (var property in properties)
                {
                    row[property.Name] = property.GetValue(item);
                }
                dataTable.Rows.Add(row);
            }
            dataTable.TableName = tableName;

            return dataTable;
        }


        protected static string RemovePrefix(string str, string prefix, bool ignoreCase = true)
        {
            if (string.IsNullOrWhiteSpace(str) || string.IsNullOrWhiteSpace(prefix))
                return str;

            StringComparison stringComparison = ignoreCase ? StringComparison.InvariantCultureIgnoreCase
                : StringComparison.InvariantCulture;

            if (string.Equals(str, prefix, stringComparison))
                return "";

            if (str.StartsWith(prefix, stringComparison))
                return str.Substring(prefix.Length - 1);

            return str;
        }

        protected bool IsExistsTable(string tableName)
        {
            var sql = "SELECT OBJECT_ID(@TableName,N'U')";
            return DBCon.ExecuteScalar<int?>(sql, new { TableName = tableName }).HasValue;
        }
    }
}
