using Dapper;
using MES_ReportForms.Core.Entities.System;
using MES_ReportForms.Core.Models.System;
using MES_ReportForms.Core.Models;
using MES_ReportForms.Utilities;
using System.Data;
using System.Drawing;
using System.Text;
using MES_ReportForms.Core.Repositories.Sql;

namespace MES_ReportForms.Core.Repositories.EF
{
    public class AttentionRepository : RepositoryBase<T_AttentionEntity>
    {
        public AttentionRepository(ApplicationDbContext context) : base(context) { } 
    }

    public class AttentionSqlRepository : ProcessRepositoryBase
    {
        public AttentionSqlRepository(string connectionName ="") : base(connectionName)
        {

        }

        public async Task<ApiResult<IEnumerable<dynamic>>> GetAttentionList(AttentionQueryDTO query)
        {
            var sqlBuilder = new StringBuilder();

            sqlBuilder.Append($@" SELECT *  FROM  [T_Attention] where 1=1 ");

            var sqlParams = new DynamicParameters();

            if (query.Start_Time.HasValue)
            {
                sqlBuilder.Append($@" AND CONVERT(DATE,Start_Time) <= @Start_Time");
                sqlParams.Add($"Start_Time", query.Start_Time, DbType.DateTime, ParameterDirection.Input);
            }

            if (query.End_Time.HasValue)
            {
                sqlBuilder.Append($@" AND CONVERT(DATE,End_Time) >= @End_Time");
                sqlParams.Add($"End_Time", query.End_Time, DbType.DateTime, ParameterDirection.Input);
            }

            if (!string.IsNullOrEmpty(query.OrganizationName))
            {
                sqlBuilder.Append($@" AND EndTime = @EndTime");
                sqlParams.Add($"OrganizationName", query.OrganizationName, DbType.String, ParameterDirection.Input);
            }

            if (!string.IsNullOrEmpty(query.User_Id))
            {
                sqlBuilder.Append($@" AND User_Id = @User_Id");
                sqlParams.Add($"User_Id", query.User_Id, DbType.String, ParameterDirection.Input);
            }

            var total = DBCon.ExecuteTotal(sqlBuilder?.ToString(), sqlParams);

            var sqlStr = SqlHelper.SqlSplitPage(sqlBuilder?.ToString(),
                query.PageNumber,
                query.PageSize,
                " CreateDate ");

            var returnData = DBCon.Query(sqlStr, sqlParams);

            return ApiResult<IEnumerable<dynamic>>.OkCount(returnData, total);

        }
    }
}
