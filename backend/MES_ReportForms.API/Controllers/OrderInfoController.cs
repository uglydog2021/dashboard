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
    [Route("api/orderInfo"), EndpointGroupName(nameof(ApiVersionInfo.ReportFormAPI))]
    public class OrderInfoController : AuthorizeApiControllerBase
    {
        /// <summary>
        /// 获取OrderInfo
        /// </summary>
        /// <returns></returns>
        [HttpGet("GetOrderInfo")]
        public async Task<ApiResult> GetOrderInfo([FromQuery] OrderInfoFilter query)
        {
            var orderInfoRepository = new OrderInfoRepository(query.ConnectionName);

            return await orderInfoRepository.GetOrderInfoList(query);
        }
         
        /// <summary>
        /// 导出Excel
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        [HttpPost("exportExcel")]
        public IActionResult ExportExcel([FromBody] OrderInfoExport query)
        {
            var orderInfoRepository = new OrderInfoRepository(query.ConnectionName);

            var orderInfoFilter = new OrderInfoFilter();

            var filter = query.Adapt(orderInfoFilter);
            filter.PageNumber = 1;
            filter.PageSize = 10000;

            var result = orderInfoRepository.ExportOrderInfo(filter);

            string fileName = HttpUtility.HtmlEncode("作业数据");

            return File(result, "application/vnd.ms-excel", $"{fileName}.xlsx");
        }

    }
}
