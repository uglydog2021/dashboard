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
    public class OrderInfoRepository : ProcessRepositoryBase
    {
        public OrderInfoRepository(string connectionName = "") : base(connectionName)
        {

        }
         
        public async Task<ApiResult<IEnumerable<dynamic>>> GetOrderInfoList(OrderInfoFilter query)
        {
            var sqlBuilder = new StringBuilder();

            sqlBuilder.Append($@" SELECT *  FROM  [T_OrderInfo] where 1=1 ");

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
                " UpdateTime ");

            var returnData = DBCon.Query(sqlStr, sqlParams);

            return ApiResult<IEnumerable<dynamic>>.OkCount(returnData, total);

        }

        public byte[] ExportOrderInfo(OrderInfoFilter taskQuery)
        {
            var orderInfoList = GetOrderInfoList(taskQuery).Result.Result.ToList();

            using (var package = new ExcelPackage())
            {
                var worksheet = package.Workbook.Worksheets.Add("Sheet1");

                worksheet.Cells[1, 1].Value = "FileName";
                worksheet.Cells[1, 2].Value = "LastUpdateUser";
                worksheet.Cells[1, 3].Value = "LastUpdateFlag";
                worksheet.Cells[1, 4].Value = "CreateTime";
                worksheet.Cells[1, 5].Value = "UpdateTime";
                worksheet.Cells[1, 6].Value = "FileContent";

                for (int i = 0; i < orderInfoList.Count; i++)
                {
                    worksheet.Cells[i + 2, 1].Value = orderInfoList[i].fileName;
                    worksheet.Cells[i + 2, 2].Value = orderInfoList[i].lastUpdateUser;
                    worksheet.Cells[i + 2, 3].Value = orderInfoList[i].lastUpdateFlag;
                    worksheet.Cells[i + 2, 4].Value = orderInfoList[i].createTime != null ? Convert.ToString(orderInfoList[i].createTime) : "";
                    worksheet.Cells[i + 2, 5].Value = orderInfoList[i].updateTime != null ? Convert.ToString(orderInfoList[i].updateTime) : "";
                    worksheet.Cells[i + 2, 6].Value = orderInfoList[i].FileContent;
                }

                byte[] byteArr = package.GetAsByteArray();
                package.Dispose();
                return byteArr;
            }
        }
    }
}
