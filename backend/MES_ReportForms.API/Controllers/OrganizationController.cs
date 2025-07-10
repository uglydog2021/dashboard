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
    [Route("api/organization"), EndpointGroupName(nameof(ApiVersionInfo.ReportFormAPI))]
    public class OrganizationController : AuthorizeApiControllerBase
    {
        private readonly OrganizationRepository _organizationRepository;

        public OrganizationController(
            OrganizationRepository organizationRepository)
        {
            _organizationRepository = organizationRepository; 
        }

        /// <summary>
        /// GetOrganization
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        /// <exception cref="BizException"></exception>
        [HttpGet("GetOrganization")]
        public PageData<T_OrganizationEntity> GetOrganization([FromQuery] OrganizationQueryDTO query)
        { 
            var organizations = _organizationRepository.AsQueryable()
                   .WhereIf(query.OrganizationID.HasValue, e => e.OrganizationID == query.OrganizationID.Value)
                   .WhereIf(!query.OrganizationName.IsNullOrEmpty(), e => e.OrganizationName.Contains(query.OrganizationName))
                   .WhereIf(query.MakerTaskCount.HasValue, e => e.MakerTaskCount == query.MakerTaskCount)
                   .WhereIf(query.CheckerTaskCount.HasValue, e => e.CheckerTaskCount == query.CheckerTaskCount);

            return PageData<T_OrganizationEntity>.Build(query, organizations.OrderByDescending(e => e.CreateTime).ApplyPageFilter(query).ToArray().Adapt<T_OrganizationEntity[]>(), organizations.Count());
        }

        /// <summary>
        /// AddOrganization
        /// </summary>
        /// <param name="organization"></param>
        /// <returns></returns>
        [HttpPost("AddOrganization")]
        public async Task<ApiResult<bool>> AddOrganization([FromBody] OrganizationAddDTO organization)
        { 
            var organizationEntity = organization.Adapt<T_OrganizationEntity>();

            organizationEntity.CreateTime = DateTime.Now;
            organizationEntity.UpdateTime = DateTime.Now;
             
            return await _organizationRepository.InsertAsync(organizationEntity);
        }

        /// <summary>
        /// RemoveOrganization
        /// </summary>
        /// <param name="OrganizationId"></param>
        /// <returns></returns>
        /// <exception cref="BizException"></exception>
        [HttpPost("RemoveOrganization")]
        public async Task<ApiResult<bool>> RemoveOrganization(int OrganizationId)
        {
            var organizations = await _organizationRepository.AsQueryable().FirstOrDefaultAsync(a => a.OrganizationID == OrganizationId);

            if(organizations == null)
                throw new BizException(L($"数据不存在"));

            return await _organizationRepository.RemoveAsync(organizations);
        }

        /// <summary>
        /// UpdateOrganization
        /// </summary>
        /// <param name="organization"></param>
        /// <returns></returns>
        /// <exception cref="BizException"></exception>
        [HttpPost("UpdateOrganization")]
        public async Task<ApiResult<bool>> UpdateOrganization([FromBody] OrganizationUpdateDTO organization)
        { 
            var organizations = await _organizationRepository.AsQueryable().FirstOrDefaultAsync(a => a.OrganizationID == organization.OrganizationID);

            if (organizations == null)
                throw new BizException(L($"数据不存在"));
             

            var organizationEntity = organization.Adapt(organizations);
            organizationEntity.UpdateTime = DateTime.Now;

            return await _organizationRepository.UpdateAsync(organizationEntity);
        }

    }
}
