using Mapster;
using Microsoft.AspNetCore.Mvc;
using MES_ReportForms.Core.Entities.System;
using Microsoft.EntityFrameworkCore;
using MES_ReportForms.Core.Utils;
using MES_ReportForms.Core.Repositories.EF;
using MES_ReportForms.Core.Models;
using System.Linq.Expressions;

namespace MES_ReportForms.API.Controllers.AdminControllers
{
    /// <summary>
    /// 数据库备份配置
    /// </summary>
    [Route("api/dbConnection"), EndpointGroupName(nameof(ApiVersionInfo.ReportFormAPI))]
    public class DBConnectionController : AuthorizeApiControllerBase
    {
        private readonly DBConnectionRepository _dBConnectionRepository;

        public DBConnectionController(
            DBConnectionRepository dBConnectionRepository)
        {
            _dBConnectionRepository = dBConnectionRepository; 
        }

        /// <summary>
        /// 添加数据库备份配置
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public async Task<ApiResult<bool>> Create([FromBody] T_DBConnectionEntity entity)
        {
            var model = await _dBConnectionRepository.GetAsync(p => p.DBName == entity.DBName);
            if (model != null)
                throw new BizException(L($"数据重复: {entity.DBName}"));

            return await _dBConnectionRepository.InsertAsync(entity.Adapt<T_DBConnectionEntity>());
        }

        /// <summary>
        /// 获取数据库备份配置
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<ApiResult<List<string>>> Get()
        {
            var model = await _dBConnectionRepository.AsQueryable().ToListAsync();

            if (model == null)
                throw new BizException(L($"数据不存在")); 

            return model.Select(p => p.DBName).ToList();
        }
    }
}
