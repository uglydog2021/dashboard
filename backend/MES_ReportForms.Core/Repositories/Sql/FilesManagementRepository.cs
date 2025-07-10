using Dapper;
using MES_ReportForms.Core.Models.System;
using MES_ReportForms.Core.Models;
using MES_ReportForms.Core.Entities.System;
using MES_ReportForms.Utilities;
using Microsoft.IdentityModel.Tokens;
using System.Data;
using System.Drawing;
using System.Text;
using System.Diagnostics;
using MES_ReportForms.Core.Utils;
using OfficeOpenXml;
using System.Collections.Generic;

namespace MES_ReportForms.Core.Repositories.Sql
{ 
    public class FilesManagementRepository : ProcessRepositoryBase
    {
        public FilesManagementRepository(string connectionName = "") : base(connectionName)
        {

        }

        public async Task<ApiResult<IEnumerable<dynamic>>> GetFilesManagement(FilesManagementFilter query)
        {
            var sqlBuilder = new StringBuilder();

            sqlBuilder.Append($@" SELECT *  FROM  [T_FilesManagement] where 1=1 ");

            var sqlParams = new DynamicParameters();

            if (query.StartData.HasValue)
            {
                sqlBuilder.Append($@" AND CONVERT(DATE,CreateDate) >= @StartData");
                sqlParams.Add($"StartData", query.StartData, DbType.DateTime, ParameterDirection.Input);
            }

            if (query.EndData.HasValue)
            {
                sqlBuilder.Append($@" AND CONVERT(DATE,CreateDate) <= @EndData");
                sqlParams.Add($"EndData", query.EndData, DbType.DateTime, ParameterDirection.Input);
            }

            if (!string.IsNullOrEmpty(query.TaskUser))
            {
                sqlBuilder.Append($@" AND TaskUser = @TaskUser");
                sqlParams.Add($"TaskUser", query.TaskUser, DbType.String, ParameterDirection.Input);
            }

            if (!string.IsNullOrEmpty(query.FileName))
            {
                sqlBuilder.Append($@" AND FileName Like @FileName");
                sqlParams.Add($"FileName", $"%{query.FileName}%" , DbType.String, ParameterDirection.Input);
            }

            if (query.Status.HasValue)
            {
                sqlBuilder.Append($@" AND Status = @Status");
                sqlParams.Add($"Status", query.Status, DbType.Int32, ParameterDirection.Input);
            } 

            var total = DBCon.ExecuteTotal(sqlBuilder?.ToString(), sqlParams);

            var sqlStr = SqlHelper.SqlSplitPage(sqlBuilder?.ToString(),
                query.PageNumber,
                query.PageSize,
                " CreateDate ");

            var returnData = DBCon.Query(sqlStr, sqlParams);

            return ApiResult<IEnumerable<dynamic>>.OkCount(returnData, total);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="taskQuery"></param>
        /// <returns></returns>
        public byte[] ExportFilesManagement(FilesManagementFilter taskQuery)
        {
            var filesManagementList = GetFilesManagement(taskQuery).Result.Result.ToList();
              
            using (var package = new ExcelPackage())
            { 
                var worksheet = package.Workbook.Worksheets.Add("Sheet1");
                 
                worksheet.Cells[1, 1].Value = "FileName";
                worksheet.Cells[1, 2].Value = "FilePath";
                worksheet.Cells[1, 3].Value = "ProcessMessage";
                worksheet.Cells[1, 4].Value = "TaskUser";
                worksheet.Cells[1, 5].Value = "Status";
                worksheet.Cells[1, 6].Value = "OrganizeID";
                worksheet.Cells[1, 7].Value = "ReleaseMessage";
                worksheet.Cells[1, 8].Value = "Priority";
                worksheet.Cells[1, 9].Value = "Priority2";
                worksheet.Cells[1, 10].Value = "UpdateDate";
                worksheet.Cells[1, 11].Value = "CreateDate";

                for (int i = 0; i < filesManagementList.Count; i++)
                {
                    worksheet.Cells[i + 2, 1].Value = filesManagementList[i].fileName;
                    worksheet.Cells[i + 2, 2].Value = filesManagementList[i].filePath;
                    worksheet.Cells[i + 2, 3].Value = filesManagementList[i].processMessage;
                    worksheet.Cells[i + 2, 4].Value = filesManagementList[i].taskUser;
                    worksheet.Cells[i + 2, 5].Value = filesManagementList[i].status;
                    worksheet.Cells[i + 2, 6].Value = filesManagementList[i].OrganizeID;
                    worksheet.Cells[i + 2, 7].Value = filesManagementList[i].releaseMessage;
                    worksheet.Cells[i + 2, 8].Value = filesManagementList[i].priority;
                    worksheet.Cells[i + 2, 9].Value = filesManagementList[i].priority2;
                    worksheet.Cells[i + 2, 10].Value = filesManagementList[i].updateDate != null ? Convert.ToString(filesManagementList[i].updateDate) : "";
                    worksheet.Cells[i + 2, 11].Value = filesManagementList[i].createDate != null ? Convert.ToString(filesManagementList[i].createDate) : "";
                }

                byte[] byteArr = package.GetAsByteArray();
                package.Dispose();
                return byteArr;
            }
             
        }

    }
}
