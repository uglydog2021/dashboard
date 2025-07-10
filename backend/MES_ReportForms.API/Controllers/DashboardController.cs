using Azure;
using MES_ReportForms.API;
using MES_ReportForms.API.Controllers;
using MES_ReportForms.Core.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using MES_ReportForms.Core.Repositories.Sql;
using static System.Runtime.InteropServices.JavaScript.JSType;
using Microsoft.AspNetCore.Http;

namespace MES_ReportForms.WebAPI.Controllers
{
    /// <summary>
    /// 获取Dashboard接口信息
    /// </summary>
    [Route("api/dashboard"), EndpointGroupName(nameof(ApiVersionInfo.ReportFormAPI))]
    [ApiController]
    public class DashboardController : ApiControllerBase
    {
        /// <summary>
        /// 1-出勤数据,休假数据
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        [HttpGet("getAttendanceRecord")] 
        public async Task<ApiResult> GetAttendanceRecord([FromQuery] AttendanceRecordQuery query)
        {
            var dashboardRepository = new DashboardRepository(query.ConnectionName);

            var vacationRecord = await dashboardRepository.GetVacationRecord(query);
              
            return vacationRecord;
        }
        
        /// <summary>
        /// 2-折线图，显示Miss率,件数
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        [HttpGet("getMissCountPerformanceVolume")]
        public async Task<ApiResult> GetMissCountPerformanceVolume([FromQuery] MissCountQuery query)
        {
            var dashboardRepository = new DashboardRepository(query.ConnectionName);

            var result = await dashboardRepository.GetMissCountPerformanceVolume(query);
              
            return result;
        }

        /// <summary>
        /// 3-Utilization、Productivity
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        [HttpGet("getOrganizationalForm")]
        public async Task<ApiResult> GetOrganizationalForm([FromQuery] OrganizationalFormQuery query)
        {
            var dashboardRepository = new DashboardRepository(query.ConnectionName);

            var utilization = await dashboardRepository.GetUtilization(query);
             
            var productivity = await dashboardRepository.GetProductivity(query);

            return ApiResult<dynamic>.Ok(new { Utilization = utilization.Result, Productivity = productivity.Result });
             
        }

        /// <summary>
        /// 4-获取BaseLine件数，本月总件数，BaseLine达成率
        /// </summary>
        /// <param name="connectionName">可不传</param>
        /// <returns></returns>
        [HttpGet("getMonthFileCount")]
        public async Task<ApiResult> GetMonthFileCount(string connectionName)
        {
            var dashboardRepository = new DashboardRepository(connectionName);

            return await dashboardRepository.GetMonthFileCount();
        }


        /// <summary>
        /// 5-当天受领件数
        /// </summary>
        /// <param name="connectionName">可不传</param>
        /// <returns></returns>
        [HttpGet("getDailyTotalFileCount")]
        public async Task<ApiResult> GetDailyTotalFileCount(string connectionName)
        {
            var dashboardRepository = new DashboardRepository(connectionName);

            return await dashboardRepository.GetDailyTotalFileCount();
        }


        /// <summary>
        /// 6
        /// </summary>
        /// <param name="connectionName">可不传</param>
        /// <returns></returns>
        [HttpGet("getDailyCountByStatus")]
        public async Task<ApiResult> GetDailyCountByStatus([FromQuery] Dashboard6Query query)
        {
            var dashboardRepository = new DashboardRepository(query.ConnectionName);

            return await dashboardRepository.GetDailyCountByStatus(query);
        }

        /// <summary>
        /// 7
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        [HttpGet("getDailyPendingList")]
        public async Task<ApiResult> GetDailyPendingList([FromQuery] DailyPendingQuery query)
        {
            var dashboardRepository = new DashboardRepository(query.ConnectionName);

            return await dashboardRepository.GetDailyPendingList(query);
        }

        /// <summary>
        /// 8-当前年度每个月每个部门的件数,当前年度每个月每个部门的miss率
        /// </summary>
        /// <param name="connectionName">可不传</param>
        /// <returns></returns>
        [HttpGet("getDepartmentMonthCurrentYear")]
        public async Task<ApiResult> GetDepartmentMonthCurrentYear([FromQuery] DepartmentMonthCurrentYearQuery query)
        {
            var dashboardRepository = new DashboardRepository(query.ConnectionName);

            var departmentMonthCurrentYear = await dashboardRepository.GetDepartmentMonthCurrentYear(query);

            var departmentMonthCurrentYearMissRate = await dashboardRepository.GetDepartmentMonthCurrentYearMissRate(query);

            return ApiResult<dynamic>.Ok(new { DepartmentMonthCurrentYear = departmentMonthCurrentYear.Result, MissRate = departmentMonthCurrentYearMissRate.Result });
        }

        /// <summary>
        /// 9-获取显去年同时期件数,当月各部门出勤率
        /// </summary>
        /// <param name="connectionName">可不传</param>
        /// <returns></returns>
        [HttpGet("getNumberItemsSamePeriodLastYear")]
        public async Task<ApiResult> GetNumberItemsSamePeriodLastYear(string connectionName)
        {
            var dashboardRepository = new DashboardRepository(connectionName);

            var attendanceRateDepartmentCurrentMonth = await dashboardRepository.GetAttendanceRateDepartmentCurrentMonth();

            int maxWeek = 0;
            int minWeek = 0;

            if (attendanceRateDepartmentCurrentMonth.Result.Count() > 0)
            {
                var weeks = attendanceRateDepartmentCurrentMonth.Result.Select(item => (int)item.Week).ToList();

                // 获取最大值和最小值
                maxWeek = weeks.Max();
                minWeek = weeks.Min();
            }

            var numberItemsSamePeriodLastYear = await dashboardRepository.GetNumberItemsSamePeriodLastYear(minWeek, maxWeek);
             
            return ApiResult<dynamic>.Ok(new { SamePeriod = numberItemsSamePeriodLastYear.Result, AttendanceRate = attendanceRateDepartmentCurrentMonth.Result });
        }

        /// <summary>
        /// 10
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        [HttpGet("getDailyAttentionList")]
        public async Task<ApiResult> GetDailyAttentionList([FromQuery] DailyAttentionQuery query)
        {
            var dashboardRepository = new DashboardRepository(query.ConnectionName);

            return await dashboardRepository.GetDailyAttentionList(query);
        }
    }
}
