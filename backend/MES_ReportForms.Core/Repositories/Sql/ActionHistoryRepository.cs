using Dapper;
using MES_ReportForms.Core.Models.System;
using MES_ReportForms.Core.Models;
using MES_ReportForms.Core.Entities.System;
using MES_ReportForms.Utilities;
using Microsoft.IdentityModel.Tokens;
using System.Data;
using System.Drawing;
using System.Text;
using OfficeOpenXml;

namespace MES_ReportForms.Core.Repositories.Sql
{ 
    public class ActionHistoryRepository : ProcessRepositoryBase
    {
        public ActionHistoryRepository(string connectionName = "") : base(connectionName)
        {

        }

        public async Task<ApiResult<IEnumerable<dynamic>>> GetActionHistory(ActionHistoryFilter query)
        {
            var sqlBuilder = new StringBuilder();

            sqlBuilder.Append($@" SELECT *  FROM  [T_ActionHistory] where 1=1 ");

            var sqlParams = new DynamicParameters();
              
            if (!string.IsNullOrEmpty(query.FileName))
            {
                sqlBuilder.Append($@" AND FileName = @FileName");
                sqlParams.Add($"FileName", query.FileName, DbType.String, ParameterDirection.Input);
            }

            var total = DBCon.ExecuteTotal(sqlBuilder?.ToString(), sqlParams);
             
            var sqlStr = SqlHelper.SqlSplitPage(sqlBuilder?.ToString(),
                query.PageNumber,
                query.PageSize,
                " CreateDate DESC"); 

            var returnData = DBCon.Query(sqlStr, sqlParams);

            return ApiResult<IEnumerable<dynamic>>.OkCount(returnData, total);
        }

        /// <summary>
        /// 返回件管理-分页查询
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        public async Task<ApiResult<IEnumerable<dynamic>>> GetActionGroupUserHistory(ActionGroupUserHistoryFilter query)
        {
            var sqlBuilder = new StringBuilder();

            sqlBuilder.Append($@" SELECT AH.taskUser,  U.User_Name, COUNT(*) AS TotalMarkAll 
                                FROM 
                                    [dbo].[T_ActionHistory] AH WITH (NOLOCK) 
                                left JOIN 
                                    [dbo].[T_UserMaster] U WITH (NOLOCK)
                                ON 
                                    AH.taskUser = U.User_Name
                                WHERE 
                                    status = 49 
                                ");

            var sqlParams = new DynamicParameters();

            if (query.ActionStartDate.HasValue)
            {
                sqlBuilder.Append($@" AND CONVERT(DATE,AH.ActionStartDate) >= @ActionStartDate");
                sqlParams.Add($"ActionStartDate", query.ActionStartDate, DbType.DateTime, ParameterDirection.Input);
            }

            if (query.ActionEndDate.HasValue)
            {
                sqlBuilder.Append($@" AND CONVERT(DATE,AH.ActionStartDate) <= @ActionEndDate");
                sqlParams.Add($"ActionEndDate", query.ActionEndDate, DbType.DateTime, ParameterDirection.Input);
            }
             
            sqlBuilder.Append($@" GROUP BY  AH.taskUser, U.User_Name ");

            var total = DBCon.ExecuteTotal(sqlBuilder?.ToString(), sqlParams);

            var sqlStr = SqlHelper.SqlSplitPage(sqlBuilder?.ToString(),
                query.PageNumber,
                query.PageSize,
                " TotalMarkAll DESC ");

            var returnData = await DBCon.QueryAsync(sqlStr, sqlParams);

            return ApiResult<IEnumerable<dynamic>>.OkCount(returnData, total);
        }

        /// <summary>
        /// GetActionHistoryList
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        public async Task<ApiResult<IEnumerable<dynamic>>> GetActionHistoryList(ActionHistoryFilter query)
        {
            var sqlBuilder = new StringBuilder();

            sqlBuilder.Append($@" SELECT *  FROM  [T_ActionHistory] where 1=1 ");

            var sqlParams = new DynamicParameters();

            if (query.ActionStartDate.HasValue)
            {
                sqlBuilder.Append($@" AND CONVERT(DATE,ActionStartDate) >= @ActionStartDate");
                sqlParams.Add($"ActionStartDate", query.ActionStartDate, DbType.DateTime, ParameterDirection.Input);
            }

            if (query.ActionEndDate.HasValue)
            {
                sqlBuilder.Append($@" AND CONVERT(DATE,ActionStartDate) <= @ActionEndDate");
                sqlParams.Add($"ActionEndDate", query.ActionEndDate, DbType.DateTime, ParameterDirection.Input);
            }

            if (!string.IsNullOrEmpty(query.FileName))
            {
                sqlBuilder.Append($@" AND FileName = @FileName");
                sqlParams.Add($"FileName", query.FileName, DbType.String, ParameterDirection.Input);
            }

            if (!string.IsNullOrEmpty(query.TaskUser))
            {
                sqlBuilder.Append($@" AND TaskUser = @TaskUser");
                sqlParams.Add($"TaskUser", query.TaskUser, DbType.String, ParameterDirection.Input);
            }

            var total = DBCon.ExecuteTotal(sqlBuilder?.ToString(), sqlParams);

            var sqlStr = SqlHelper.SqlSplitPage(sqlBuilder?.ToString(),
                query.PageNumber,
                query.PageSize,
                " CreateDate ");

            var returnData = await DBCon.QueryAsync(sqlStr, sqlParams);

            return ApiResult<IEnumerable<dynamic>>.OkCount(returnData, total);

        }
        public byte[] ExportActionHistory(ActionHistoryFilter taskQuery)
        {
            var actionHistoryList = GetActionHistory(taskQuery).Result.Result.ToList();
             
            using (var package = new ExcelPackage())
            {
                var worksheet = package.Workbook.Worksheets.Add("Sheet1");

                worksheet.Cells[1, 1].Value = "Status";
                worksheet.Cells[1, 2].Value = "FileName";
                worksheet.Cells[1, 3].Value = "TaskUser";
                worksheet.Cells[1, 4].Value = "HistoryFilePath";
                worksheet.Cells[1, 5].Value = "ActionStartDate";
                worksheet.Cells[1, 6].Value = "ActionEndDate";
                worksheet.Cells[1, 7].Value = "CreateDate";
                worksheet.Cells[1, 8].Value = "WorkTimeH";
                worksheet.Cells[1, 9].Value = "WorkTimeM";
                worksheet.Cells[1, 10].Value = "WorkTimeS";

                for (int i = 0; i < actionHistoryList.Count; i++)
                {
                    worksheet.Cells[i + 2, 1].Value = actionHistoryList[i].status;
                    worksheet.Cells[i + 2, 2].Value = actionHistoryList[i].fileName;
                    worksheet.Cells[i + 2, 3].Value = actionHistoryList[i].taskUser;
                    worksheet.Cells[i + 2, 4].Value = actionHistoryList[i].HistoryFilePath;
                    worksheet.Cells[i + 2, 5].Value = actionHistoryList[i].ActionStartDate != null ? Convert.ToString(actionHistoryList[i].ActionStartDate) : "";
                    worksheet.Cells[i + 2, 6].Value = actionHistoryList[i].ActionEndDate != null ? Convert.ToString(actionHistoryList[i].ActionEndDate) : "";
                    worksheet.Cells[i + 2, 7].Value = actionHistoryList[i].createDate != null ? Convert.ToString(actionHistoryList[i].createDate) : "";
                    worksheet.Cells[i + 2, 8].Value = actionHistoryList[i].WorkTimeH;
                    worksheet.Cells[i + 2, 9].Value = actionHistoryList[i].WorkTimeM;
                    worksheet.Cells[i + 2, 10].Value = actionHistoryList[i].WorkTimeS;
                }

                byte[] byteArr = package.GetAsByteArray();
                package.Dispose();
                return byteArr;
            }

        }


    }
}
