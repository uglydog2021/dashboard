using MES_ReportForms.Core.Models.System;
using MES_ReportForms.Core.Models;
using MES_ReportForms.Core.Repositories.Sql;
using Microsoft.AspNetCore.Mvc;
using System.Data;

namespace MES_ReportForms.API.Controllers.AdminControllers
{
    /// <summary>
    /// 
    /// </summary>
    [Route("api/execute"), EndpointGroupName(nameof(ApiVersionInfo.ReportFormAPI))]
    public class ExecuteController : ApiControllerBase
    {
        [HttpGet("QueryTable")]
        public async Task<ApiResult> QueryTable([FromQuery] ExecuteFilter query)
        {
            var executeRepository = new ExecuteRepository(query.ConnectionName);

            return await executeRepository.QueryTable(query.Sql);
        }

        [HttpGet("ExcuteSql")]
        public async Task<ApiResult> ExcuteSql([FromQuery] ExecuteFilter query)
        {
            var executeRepository = new ExecuteRepository(query.ConnectionName);

            return await executeRepository.ExecuteSql(query.Sql);
        }
         
        [HttpPost("QueryTablePost")]
        public async Task<ApiResult> QueryTablePost([FromBody] ExecuteFilter query)
        {
            var executeRepository = new ExecuteRepository(query.ConnectionName);

            return await executeRepository.QueryTable(query.Sql);
        }

        [HttpPost("ExcuteSqlPost")]
        public async Task<ApiResult> ExcuteSqlPost([FromBody] ExecuteFilter query)
        {
            var executeRepository = new ExecuteRepository(query.ConnectionName);

            return await executeRepository.ExecuteSql(query.Sql);
        }
    }
}
