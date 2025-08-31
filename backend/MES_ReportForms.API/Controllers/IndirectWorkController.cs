using Mapster;
using Microsoft.AspNetCore.Mvc;
using MES_ReportForms.Core.Entities.System;
using Microsoft.EntityFrameworkCore;
using MES_ReportForms.Core.Utils;
using MES_ReportForms.Core.Repositories.EF;
using MES_ReportForms.Core.Models;
using System.Linq.Expressions;
using System.Security.Policy;
using MES_ReportForms.Core.Repositories.Sql;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;
using MES_ReportForms.Core;
using Microsoft.IdentityModel.Tokens;

namespace MES_ReportForms.API.Controllers.AdminControllers
{
    /// <summary>
    /// 
    /// </summary>
    [Route("api/indirectWork"), EndpointGroupName(nameof(ApiVersionInfo.ReportFormAPI))]
    public class IndirectWorkController : AuthorizeApiControllerBase
    {
        private readonly IndirectWorkRepository _indirectWorkRepository;

        public IndirectWorkController(
            IndirectWorkRepository indirectWorkRepository)
        {
            _indirectWorkRepository = indirectWorkRepository; 
        }

        /// <summary>
        /// GetIndirectWork
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        /// <exception cref="BizException"></exception>
        [HttpGet("GetIndirectWork")]
        public PageData<T_IndirectWorkEntity> GetIndirectWork([FromQuery] IndirectWorkQueryDTO query)
        { 
            var indirectWorks = _indirectWorkRepository.AsQueryable()
                   .WhereIf(!query.GUID.IsNullOrEmpty(), e => e.GUID == query.GUID)
                   .WhereIf(!query.User_Name.IsNullOrEmpty(), e => e.User_Name.Contains(query.User_Name))
                   .WhereIf(!query.Description.IsNullOrEmpty(), e => e.Description.Contains(query.Description))
                   .WhereIf(query.Start_Time.HasValue, e => e.Start_Time >= query.Start_Time)
                   .WhereIf(query.End_Time.HasValue, e => e.End_Time <= query.End_Time)
                   .WhereIf(query.Non_OCR_Count.HasValue, e => e.Non_OCR_Count == query.Non_OCR_Count)
                   .WhereIf(query.Non_OCR_JJCount.HasValue, e => e.Non_OCR_JJCount >= query.Non_OCR_JJCount);

            return PageData<T_IndirectWorkEntity>.Build(query, indirectWorks.OrderByDescending(e => e.Create_Time).ApplyPageFilter(query).ToArray().Adapt<T_IndirectWorkEntity[]>(), indirectWorks.Count());
        }

        /// <summary>
        /// AddIndirectWork
        /// </summary>
        /// <param name="indirectWork"></param>
        /// <returns></returns>
        [HttpPost("AddIndirectWork")]
        public async Task<ApiResult<bool>> AddIndirectWork([FromBody] IndirectWorkAddDTO indirectWork)
        {
            var indirectWorkEntity = indirectWork.Adapt<T_IndirectWorkEntity>();

            var currentUser = CurrentUserSession();

            indirectWorkEntity.CreatorGUID = currentUser.GUID;
            indirectWorkEntity.Creator = currentUser.UserName;
             
            indirectWorkEntity.Create_Time = DateTime.Now;
            indirectWorkEntity.Update_Time = DateTime.Now;

            return await _indirectWorkRepository.InsertAsync(indirectWorkEntity);
        }

        /// <summary>
        /// RemoveIndirectWork
        /// </summary>
        /// <param name="IndirectWorkId"></param>
        /// <returns></returns>
        /// <exception cref="BizException"></exception>
        [HttpPost("RemoveIndirectWork")]
        public async Task<ApiResult<bool>> RemoveIndirectWork(int IndirectWorkId)
        {
            var indirectWorks = await _indirectWorkRepository.AsQueryable().FirstOrDefaultAsync(a => a.ID == IndirectWorkId);

            if(indirectWorks == null)
                throw new BizException(L($"数据不存在"));

            return await _indirectWorkRepository.RemoveAsync(indirectWorks);
        }

        /// <summary>
        /// UpdateIndirectWork
        /// </summary>
        /// <param name="indirectWork"></param>
        /// <returns></returns>
        /// <exception cref="BizException"></exception>
        [HttpPost("UpdateIndirectWork")]
        public async Task<ApiResult<bool>> UpdateIndirectWork([FromBody] IndirectWorkUpdateDTO indirectWork)
        { 
            var indirectWorks = await _indirectWorkRepository.AsQueryable().FirstOrDefaultAsync(a => a.ID == indirectWork.ID);

            if (indirectWorks == null)
                throw new BizException(L($"数据不存在"));
             
            var indirectWorkEntity = indirectWork.Adapt(indirectWorks);
             
            indirectWorkEntity.Update_Time = DateTime.Now;

            return await _indirectWorkRepository.UpdateAsync(indirectWorkEntity);
        }

    }
}
