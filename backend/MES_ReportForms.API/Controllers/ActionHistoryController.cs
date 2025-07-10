using MES_ReportForms.API; 
using MES_ReportForms.Core; 
using MES_ReportForms.Core.Repositories;
using Mapster;
using Microsoft.AspNetCore.Mvc;
using MES_ReportForms.Core.Models.System;
using MES_ReportForms.Domain.Services;
using MES_ReportForms.Core.Repositories.Sql;
using MES_ReportForms.Core.Models;
using System.Web;

namespace MES_ReportForms.API.Controllers.AdminControllers
{
    /// <summary>
    ///  
    /// </summary>
    [Route("api/actionHistory"), EndpointGroupName(nameof(ApiVersionInfo.ReportFormAPI))]
    public class ActionHistoryController : AuthorizeApiControllerBase
    {
        /// <summary>
        /// 根据单据号获取事件
        /// </summary>
        /// <returns></returns>
        [HttpGet("GetActionHistory")]
        public async Task<ApiResult> GetActionHistory([FromQuery] ActionHistoryFilter query)
        {
            var actionHistoryRepository = new ActionHistoryRepository(query.ConnectionName);

            return await actionHistoryRepository.GetActionHistory(query);
        }

        /// <summary>
        /// ActionHistory-分页查询
        /// </summary>
        /// <returns></returns>
        [HttpGet("GetActionHistoryList")]
        public async Task<ApiResult> GetActionHistoryList([FromQuery] ActionHistoryFilter query)
        {
            var actionHistoryRepository = new ActionHistoryRepository(query.ConnectionName);

            return await actionHistoryRepository.GetActionHistoryList(query);
        }

        /// <summary>
        /// 返回件管理-分页查询
        /// </summary>
        /// <returns></returns>
        [HttpGet("GetActionGroupUserHistory")]
        public async Task<ApiResult> GetActionGroupUserHistory([FromQuery] ActionGroupUserHistoryFilter query)
        {
            var actionHistoryRepository = new ActionHistoryRepository(query.ConnectionName);

            return await actionHistoryRepository.GetActionGroupUserHistory(query);
        }

        /// <summary>
        /// 导出Excel
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        [HttpPost("exportExcel")]
        public IActionResult ExportExcel([FromBody] ActionHistoryExport query)
        {
            var actionHistoryRepository = new ActionHistoryRepository(query.ConnectionName);

            var actionHistoryFilter = new ActionHistoryFilter();

            var filter = query.Adapt(actionHistoryFilter);
            filter.PageNumber = 1;
            filter.PageSize = 10000;

            var result = actionHistoryRepository.ExportActionHistory(filter);

            string fileName = HttpUtility.HtmlEncode("作业数据");

            return File(result, "application/vnd.ms-excel", $"{fileName}.xlsx");
        }

    }
}
