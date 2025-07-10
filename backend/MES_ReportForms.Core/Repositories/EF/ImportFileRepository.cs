using Dapper;
using MES_ReportForms.Core.Entities;
using MES_ReportForms.Core.Entities.System;
using MES_ReportForms.Core.Models;
using MES_ReportForms.Core.Repositories.Sql;
using MES_ReportForms.Utilities;
using Microsoft.Data.SqlClient;
using OfficeOpenXml;
using System;
using System.Data;

namespace MES_ReportForms.Core.Repositories.EF
{
    public class ImportFileRepository : ProcessRepositoryBase
    {
        public ImportFileRepository(string connectionName = "") : base(connectionName)
        {

        }

        /// <summary>
        /// 批量新增
        /// </summary>
        /// <param name="insertEntity"></param>
        /// <param name="currentMonth"></param>
        /// <returns></returns>
        public ApiResult ImportEmployeeAttendanceDataToDb(List<T_EmployeeAttendanceEntity> insertEntity, int currentMonth)
        { 
            //清空数据
            var executeCount = DBCon.Execute(" DELETE FROM T_EmployeeAttendance WHERE AttendanceYear = @AttendanceYear AND AttendanceMonth = @AttendanceMonth ", new { AttendanceYear = DateTime.Now.Year, AttendanceMonth = currentMonth });

            var dataTable = ConvertToDataTable(insertEntity, "T_EmployeeAttendance");

            var rowsAffected = SqlBulkCopyInsert(dataTable);

            return rowsAffected ? ApiResult.Ok() : ApiResult.ToError("数据操作失败");
        }

        public List<T_EmployeeAttendanceEntity> ImportEmployeeAttendanceData(ExcelWorksheet worksheet,int currentMonth)
        {
            var result = new List<T_EmployeeAttendanceEntity>();
             
            // 遍历从第 6 行开始的数据
            for (int row = 6; row <= worksheet.Dimension.End.Row - 1; row++)
            {
                var guid = worksheet.Cells[row, 3].Text.Trim();
                if (string.IsNullOrEmpty(guid)) continue;  // 如果姓名为空，则跳过

                //根据姓名，查找相关人员信息
                var userMaster = DBCon.QueryFirstOrDefault<T_UserMasterEntity>(" SELECT * FROM T_UserMaster WHERE GUID = @GUID ", new { GUID = guid });
                 
                ////如果找不到此人信息，也跳过
                if (userMaster == null) continue;  // 如果姓名为空，则跳过

                // 判断每一列（从 C 列到 AG 列，代表 1 号到 31 号）
                for (int col = 4; col <= 33; col++) // C 列到 AG 列
                {
                    var attendanceData = worksheet.Cells[row, col].Text.Trim();
                    if (string.IsNullOrEmpty(attendanceData)) continue;  // 如果当天没有数据，则跳过

                    int leaveDays = 0;
                    var leaveType = GetLeaveType(attendanceData,out leaveDays);

                    if (leaveType == 0) continue;  // 如果匹配不到，则跳过
                    // 创建数据对象
                    var data = new T_EmployeeAttendanceEntity
                    {
                        GUID = guid,
                        EmployeeId = userMaster.EmployeeId,
                        UserName = userMaster.User_Name,
                        LeaveType = leaveType,
                        AttendanceYear = DateTime.Now.Year,
                        AttendanceMonth = currentMonth,
                        AttendanceDay = col - 2, // 列数从 C 列开始，1 号是 C 列
                        LeaveDays = leaveDays
                    };

                    result.Add(data);
                }
            }

            return result;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="attendanceData"></param>
        /// <param name="leaveDays"></param>
        /// <returns></returns>
        private int GetLeaveType(string attendanceData,out int leaveDays)
        {
            leaveDays = 0;
            if (Int32.TryParse(attendanceData, out int day) && day < 0)
            {
                leaveDays = day;

                return 4;
            }
            switch (attendanceData)
            {
                case "○":
                case "●":

                    return 1; // AnnualLeave
                case "☆":
                case "★":

                    return 2; // SickLeave
                case "△":
                case "▲":

                    return 3; // OtherSpecialLeave
                default:

                    return 0;
            }
        }

        /// <summary>
        /// 批量新增
        /// </summary>
        /// <param name="insertEntity"></param>
        /// <returns></returns>
        public ApiResult ImportSkillMatrixDataToDb(List<T_SkillMatrixEntity> insertEntity)
        {
            //清空数据
            var executeCount = DBCon.Execute(" DELETE FROM T_SkillMatrix ");

            var dataTable = ConvertToDataTable(insertEntity, "T_SkillMatrix");

            var rowsAffected = SqlBulkCopyInsert(dataTable);

            return rowsAffected ? ApiResult.Ok() : ApiResult.ToError("数据操作失败");
        }

        public List<T_SkillMatrixEntity> ImportSkillMatrixData(ExcelWorksheet worksheet)
        {
            var result = new List<T_SkillMatrixEntity>();

            for (int row = 4; row <= worksheet.Dimension.End.Row - 1; row++)
            {
                var team = worksheet.Cells[row, 2].Text.Trim();
                if (string.IsNullOrEmpty(team)) continue;

                var guid = worksheet.Cells[row, 6].Text.Trim();
                if (string.IsNullOrEmpty(guid)) continue;  // 如果GUID为空，则跳过

                var userMaster = DBCon.QueryFirstOrDefault<T_UserMasterEntity>(" SELECT * FROM T_UserMaster WHERE GUID =@GUID ", new { GUID = guid });

                if (userMaster == null) continue;

                // 创建数据对象
                var data = new T_SkillMatrixEntity
                {
                    GUID = guid,
                    Team = worksheet.Cells[row, 2].Text.Trim(),
                    Group1 = worksheet.Cells[row, 3].Text.Trim(),
                    Group2 = worksheet.Cells[row, 4].Text.Trim(),
                    TokyoSkill = GetCellValue(worksheet.Cells[row, 8].Text.Trim()),
                    OsakaSkill = GetCellValue(worksheet.Cells[row, 9].Text.Trim()),
                    Wave10Skill = GetCellValue(worksheet.Cells[row, 10].Text.Trim()),
                    InputASkill = GetCellValue(worksheet.Cells[row, 11].Text.Trim()),
                    InputBSkill = GetCellValue(worksheet.Cells[row, 12].Text.Trim()),
                    NewSCSkill = GetCellValue(worksheet.Cells[row, 13].Text.Trim()),
                    EFAXSkill = GetCellValue(worksheet.Cells[row, 14].Text.Trim()),
                    ProductQuoteSkill = GetCellValue(worksheet.Cells[row, 15].Text.Trim()),
                    GPOSecondQuoteSkill = GetCellValue(worksheet.Cells[row, 16].Text.Trim()),
                    ContractQuoteSkill = GetCellValue(worksheet.Cells[row, 17].Text.Trim()),
                    MasterRegistrationSkill = GetCellValue(worksheet.Cells[row, 18].Text.Trim()),

                    CertificateSkill = GetCellValue(worksheet.Cells[row, 19].Text.Trim()),
                    MSAPaymentSkill = GetCellValue(worksheet.Cells[row, 20].Text.Trim()),

                    SampleArrangementSkill = GetCellValue(worksheet.Cells[row, 21].Text.Trim()),
                    PostDiscountSkill = GetCellValue(worksheet.Cells[row, 22].Text.Trim()),
                    ShortTermLoanSkill = GetCellValue(worksheet.Cells[row, 23].Text.Trim()),
                    InventoryPromotionSkill = GetCellValue(worksheet.Cells[row, 24].Text.Trim()),
                    LoanerEquipmentSkill = GetCellValue(worksheet.Cells[row, 25].Text.Trim()),
                    EquipmentArrangementSkill = GetCellValue(worksheet.Cells[row, 26].Text.Trim()),
                    IndirectSalesCaseSkill = GetCellValue(worksheet.Cells[row, 27].Text.Trim()),
                    NewSCCaseSkill = GetCellValue(worksheet.Cells[row, 28].Text.Trim()),
                    DirectSalesCaseSkill = GetCellValue(worksheet.Cells[row, 29].Text.Trim()),

                    JapaneseCertificate = worksheet.Cells[row, 30].Text.Trim(),
                    JapaneseAbility = worksheet.Cells[row, 31].Text.Trim(),
                    StudyAbroadExperience = worksheet.Cells[row, 32].Text.Trim(),
                    EnglishLevel = worksheet.Cells[row, 33].Text.Trim(),
                    CallHandlingExperience = worksheet.Cells[row, 34].Text.Trim(),
                    KTExperience = worksheet.Cells[row, 35].Text.Trim(),
                    DomainExperience1 = worksheet.Cells[row, 36].Text.Trim(),
                    DomainExperience2 = worksheet.Cells[row, 37].Text.Trim(),
                    ExcelSkill = worksheet.Cells[row, 38].Text.Trim(),
                    OtherQualifications = worksheet.Cells[row, 39].Text.Trim(),
                    OtherSkills = worksheet.Cells[row, 40].Text.Trim()

                };

                result.Add(data);
            }

            return result;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        private string GetCellValue(string cellValue)
        {
            switch (cellValue)
            {
                case "◎":
                    return "1";
                case "〇":
                    return "2";
                case "△":
                    return "3";
                default:
                    return "0";
            }
        }


        /// <summary>
        /// 批量新增
        /// </summary>
        /// <param name="insertEntity"></param>
        /// <returns></returns>
        public ApiResult ImportJapanHolidaysDataToDb(List<T_JapanHolidaysEntity> insertEntity)
        {
            //清空数据
            var executeCount = DBCon.Execute(" DELETE FROM T_JapanHolidays ");

            var dataTable = ConvertToDataTable(insertEntity, "T_JapanHolidays");

            var rowsAffected = SqlBulkCopyInsert(dataTable);

            return rowsAffected ? ApiResult.Ok() : ApiResult.ToError("数据操作失败");
        }
    }
}