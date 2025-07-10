using Azure;
using Dapper;
using Dapper.Contrib.Extensions;
using MES_ReportForms.Core.Models;
using Microsoft.Extensions.Primitives;
using System.Collections.Concurrent;
using System.Data;
using System.Data.Common;
using System.Data.SqlTypes;
using System.Diagnostics;
using System.Text;

namespace MES_ReportForms.Core.Repositories.Sql
{
    public class ExecuteRepository : ProcessRepositoryBase
    {
        public ExecuteRepository(string connectionName = "") : base(connectionName)
        {

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sql"></param>
        /// <returns></returns>
        public async Task<ApiResult<IEnumerable<dynamic>>> QueryTable(string sql)
        {
            var filesData = await DBCon.QueryAsync(sql);

            return ApiResult<IEnumerable<dynamic>>.Ok(filesData);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="conn"></param>
        /// <param name="sql"></param>
        /// <returns></returns>
        public async Task<ApiResult> ExecuteSql(string sql)
        {
            var rowsAffected = await DBCon.ExecuteAsync(sql);

            return rowsAffected > 0 ? ApiResult.Ok() : ApiResult.ToError("数据操作失败");
        }
    }
}
