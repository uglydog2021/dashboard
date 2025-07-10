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
    [Route("api/filesManagement"), EndpointGroupName(nameof(ApiVersionInfo.ReportFormAPI))]
    public class FilesManagementController : AuthorizeApiControllerBase
    {
        /// <summary>
        /// 单据主表分页查询
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        [HttpGet("GetFilesManagement")]
        public async Task<ApiResult> GetFilesManagement([FromQuery] FilesManagementFilter query)
        {
            var filesManagementRepository = new FilesManagementRepository(query.ConnectionName);

            return await filesManagementRepository.GetFilesManagement(query);
        }

        /// <summary>
        /// 导出Excel
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        [HttpPost("exportExcel")]
        public IActionResult ExportExcel([FromBody] FilesManagementExport query)
        {
            var filesManagementRepository = new FilesManagementRepository(query.ConnectionName);

            var filesManagementFilter = new FilesManagementFilter();

            var filter = query.Adapt(filesManagementFilter);
            filter.PageNumber = 1;
            filter.PageSize = 10000;

            var result = filesManagementRepository.ExportFilesManagement(filter);

            string fileName = HttpUtility.HtmlEncode("作业数据");

            return File(result, "application/vnd.ms-excel", $"{fileName}.xlsx");
        }

    }
}
