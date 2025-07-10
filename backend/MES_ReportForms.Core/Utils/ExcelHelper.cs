using Azure;
using MES_ReportForms.Core.Models;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MES_ReportForms.Core.Utils
{
    public class ExcelHelper
    {
        public static byte[] ExportExcel(List<ExcelRun> excelRun)
        {
            using (ExcelPackage package = new ExcelPackage())
            {
                foreach (var item in excelRun)
                {
                    string sheetName = item.SheetName;
                    ExcelWorkbook wb = package.Workbook;
                    ExcelWorksheet ws = wb.Worksheets.Add(sheetName);
                    ws.Cells.Style.Font.Size = 10;
                    ws.Cells.Style.Font.Name = "微软雅黑";
                    ws.Cells.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    ws.Cells.Style.VerticalAlignment = ExcelVerticalAlignment.Center;

                    //设置表头单元格格式
                    using (var range = ws.Cells[1, 1, 1, item.ColumnNameList.Count])
                    {
                        range.Style.Font.Bold = true;
                        range.Style.Fill.PatternType = ExcelFillStyle.Solid;
                        range.Style.Fill.BackgroundColor.SetColor(Color.LightBlue);
                        range.Style.Border.Left.Style = ExcelBorderStyle.Thin;
                        range.Style.Border.Right.Style = ExcelBorderStyle.Thin;
                        range.Style.Border.Top.Style = ExcelBorderStyle.Thin;
                        range.Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                        ws.Row(1).Height = 30;
                    }

                    for (int i = 0; i < item.ColumnNameList.Count; i++)
                    { 
                        ws.Cells[1, i + 1].Value = item.ColumnNameList[i];
                        ws.Column(i + 1).AutoFit();
                    }

                    ws.View.FreezePanes(2, 1);
                    int rowNum = 0;
                    int k = 1;
                    //从第二行开始Insert数据
                    for (int x = 0; x < item.ViewData.ToList().Count; x++)
                    {
                        var dr = (IDictionary<string, dynamic>)item.ViewData.ToList()[x];
                        if (rowNum == 1048575)
                        {
                            ws = wb.Worksheets.Add(sheetName + k);
                            rowNum = 0;
                            k++;
                        }
                        for (int y = 0; y < item.ColumnNameList.Count; y++)
                        {
                            string columnStr = item.ColumnNameList[y];
                            ws.Cells[x + 2, y + 1].Value = dr[columnStr]?.ToString();

                        }
                    }
                    //ws.Dispose();
                }

                byte[] byteArr = package.GetAsByteArray();
                package.Dispose();
                return byteArr;
            }

        }
    }
}
