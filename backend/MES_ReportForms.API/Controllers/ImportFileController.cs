using Mapster;
using Microsoft.AspNetCore.Mvc;
using MES_ReportForms.Core.Entities.System;
using Microsoft.EntityFrameworkCore;
using MES_ReportForms.Core.Utils;
using MES_ReportForms.Core.Repositories.EF;
using MES_ReportForms.Core.Models;
using System.Linq.Expressions;
using System.Security.Policy;
using OfficeOpenXml;
using MES_ReportForms.Core.Entities;
using System.Globalization;

namespace MES_ReportForms.API.Controllers.AdminControllers
{
    /// <summary>
    /// 
    /// </summary>
    [Route("api/importFile"), EndpointGroupName(nameof(ApiVersionInfo.ReportFormAPI))]
    public class ImportFileController : ApiControllerBase
    {
        private readonly ImportFileRepository _importFileRepository;

        public ImportFileController(
            ImportFileRepository importFileRepository)
        {
            _importFileRepository = importFileRepository; 
        }

        /// <summary>
        /// 导入 Sasu出勤表
        /// </summary>
        /// <param name="file"></param>
        /// <returns></returns>
        [HttpPost("ImportEmployeeAttendanceData")]
        public ApiResult<List<T_EmployeeAttendanceEntity>> ImportEmployeeAttendanceData(IFormFile file)
        { 
            if (file == null || file.Length == 0)
            { 
                throw new BizException(L($"未找到任何文件")); 
            }

            // 获取当前月份
            int currentMonth = DateTime.Now.Month;
            var employeeAttendanceList = new List<T_EmployeeAttendanceEntity>();

            using (var stream = new MemoryStream())
            {
                file.CopyTo(stream);

                stream.Position = 0;
                using (var package = new ExcelPackage(stream))
                {
                    ExcelWorksheet worksheet = package.Workbook.Worksheets[currentMonth + 1];

                    employeeAttendanceList.AddRange(_importFileRepository.ImportEmployeeAttendanceData(worksheet, currentMonth));
                }
            }

            var result = _importFileRepository.ImportEmployeeAttendanceDataToDb(employeeAttendanceList, currentMonth);

            return ApiResult<List<T_EmployeeAttendanceEntity>>.Ok(employeeAttendanceList);
        }

        /// <summary>
        /// 导入 SaSu Skill Matrix 表
        /// </summary>
        /// <param name="file"></param>
        /// <returns></returns>
        [HttpPost("ImportSkillMatrixData")]
        public ApiResult<List<T_SkillMatrixEntity>> ImportSkillMatrixData(IFormFile file)
        {
            if (file == null || file.Length == 0)
            {
                throw new BizException(L($"未找到任何文件"));
            }

            var skillMatrixList = new List<T_SkillMatrixEntity>();

            using (var stream = new MemoryStream())
            {
                file.CopyTo(stream);

                stream.Position = 0;
                using (var package = new ExcelPackage(stream))
                {
                    ExcelWorksheet worksheet = package.Workbook.Worksheets[0];

                    skillMatrixList.AddRange(_importFileRepository.ImportSkillMatrixData(worksheet));
                }
            }

            var result = _importFileRepository.ImportSkillMatrixDataToDb(skillMatrixList);

            return ApiResult<List<T_SkillMatrixEntity>>.Ok(skillMatrixList);
        }

        /// <summary>
        /// 导入 T_JapanHolidays数据
        /// </summary>
        /// <param name="file"></param>
        /// <returns></returns>
        /// <exception cref="BizException"></exception>
        [HttpPost("ImportJapanHolidaysData")]
        public ApiResult<List<T_JapanHolidaysEntity>> ImportJapanHolidaysData(IFormFile file)
        {
            if (file == null || file.Length == 0)
            {
                throw new BizException(L($"未找到任何文件"));
            }

            var holidays = new List<T_JapanHolidaysEntity>();

            using (var stream = file.OpenReadStream()) // 获取文件流
            using (var reader = new StreamReader(stream))
            {
                string line;
                bool isFirstLine = true;  // 用于跳过表头
                while ((line = reader.ReadLine()) != null)
                {
                    if (isFirstLine)
                    {
                        isFirstLine = false;
                        continue;
                    }

                    var columns = line.Split(',');

                    if (columns.Length != 3) continue;

                    // 解析字段
                    if (!int.TryParse(columns[0], out int year))
                        continue;

                    if (!DateTime.TryParseExact(columns[1], "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime holidayDate))
                        continue;

                    string holidayName = columns[2];

                    holidays.Add(new T_JapanHolidaysEntity
                    {
                        Year = year,
                        HolidayDate = holidayDate,
                        HolidayName = holidayName
                    });
                }
            } 

            var result = _importFileRepository.ImportJapanHolidaysDataToDb(holidays);

            return ApiResult<List<T_JapanHolidaysEntity>>.Ok(holidays);
        }
    }
}
