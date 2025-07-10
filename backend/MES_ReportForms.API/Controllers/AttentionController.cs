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
    [Route("api/attention"), EndpointGroupName(nameof(ApiVersionInfo.ReportFormAPI))]
    public class AttentionController : AuthorizeApiControllerBase
    {
        private readonly AttentionRepository _attentionRepository;

        public AttentionController(
            AttentionRepository attentionRepository)
        {
            _attentionRepository = attentionRepository; 
        }

        /// <summary>
        /// GetAttention
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        /// <exception cref="BizException"></exception>
        [HttpGet("GetAttention")]
        public PageData<T_AttentionEntity> GetAttention([FromQuery] AttentionQueryDTO query)
        {
            //var attentionRepository = new AttentionSqlRepository();

            //return await attentionRepository.GetAttentionList(query);

            var attentions = _attentionRepository.AsQueryable()
                   .WhereIf(!query.User_Id.IsNullOrEmpty(), e => e.User_Id == query.User_Id)
                   .WhereIf(!query.OrganizationName.IsNullOrEmpty(), e => e.OrganizationName.Contains(query.OrganizationName))
                   .WhereIf(query.Start_Time.HasValue, e => e.Start_Time >= query.Start_Time)
                   .WhereIf(query.End_Time.HasValue, e => e.End_Time <= query.End_Time);

            return PageData<T_AttentionEntity>.Build(query, attentions.OrderByDescending(e => e.CreateTime).ApplyPageFilter(query).ToArray().Adapt<T_AttentionEntity[]>(), attentions.Count());
              
        }

        /// <summary>
        /// AddAttention
        /// </summary>
        /// <param name="attention"></param>
        /// <returns></returns>
        [HttpPost("AddAttention")]
        public async Task<ApiResult<bool>> AddAttention([FromBody] AttentionAddDTO attention)
        {
            //var attentionEntity = new T_AttentionEntity();
            //attentionEntity.OrganizationId = attention.OrganizationId;
            //attentionEntity.OrganizationName = attention.OrganizationName;
            //attentionEntity.StartTime = attention.StartTime;
            //attentionEntity.EndTime = attention.EndTime;
            //attentionEntity.UserId = attention.UserId;
            //attentionEntity.Subject = attention.Subject;
            //attentionEntity.Message = attention.Message;
             
            var attentionEntity = attention.Adapt<T_AttentionEntity>();

            attentionEntity.CreateTime = DateTime.Now;
            attentionEntity.UpdateTime = DateTime.Now;

            return await _attentionRepository.InsertAsync(attentionEntity);
        }

        /// <summary>
        /// RemoveAttention
        /// </summary>
        /// <param name="AttentionId"></param>
        /// <returns></returns>
        /// <exception cref="BizException"></exception>
        [HttpPost("RemoveAttention")]
        public async Task<ApiResult<bool>> RemoveAttention(int AttentionId)
        {
            var attentions = await _attentionRepository.AsQueryable().FirstOrDefaultAsync(a => a.Attention_Id == AttentionId);

            if(attentions == null)
                throw new BizException(L($"数据不存在"));

            return await _attentionRepository.RemoveAsync(attentions);
        }

        /// <summary>
        /// UpdateAttention
        /// </summary>
        /// <param name="attention"></param>
        /// <returns></returns>
        /// <exception cref="BizException"></exception>
        [HttpPost("UpdateAttention")]
        public async Task<ApiResult<bool>> UpdateAttention([FromBody] AttentionUpdateDTO attention)
        { 
            var attentions = await _attentionRepository.AsQueryable().FirstOrDefaultAsync(a => a.Attention_Id == attention.Attention_Id);

            if (attentions == null)
                throw new BizException(L($"数据不存在"));
             

            var attentionEntity = attention.Adapt(attentions);
            attentionEntity.UpdateTime = DateTime.Now;

            return await _attentionRepository.UpdateAsync(attentionEntity);
        }

    }
}
