using MES_ReportForms.Core.Models.System;
using MES_ReportForms.Core.Models;
using MES_ReportForms.Core.Repositories.Sql;
using Microsoft.AspNetCore.Mvc;
using System.Data;
using MES_ReportForms.API.Filters;
using MES_ReportForms.Core.Utils;
using MES_ReportForms.Core;
using MES_ReportForms.Core.Repositories.EF;
using MES_ReportForms.Core.Entities.System;
using Mapster;
using Microsoft.IdentityModel.Tokens;
using Microsoft.EntityFrameworkCore;
using System;

namespace MES_ReportForms.API.Controllers.AdminControllers
{
    /// <summary>
    /// 
    /// </summary>
    [Route("api/userMaster"), EndpointGroupName(nameof(ApiVersionInfo.ReportFormAPI))]
    public class UserMasterController : AuthorizeApiControllerBase
    {
        private readonly UserMasterRepository _userRepository; 

        /// <summary>
        /// 
        /// </summary>
        /// <param name="userRepository"></param>
        public UserMasterController(
            UserMasterRepository userRepository)
        {
            _userRepository = userRepository;
        }

        /// <summary>
        /// GetUserMaster
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        /// <exception cref="BizException"></exception>
        [HttpGet("GetUserMaster")]
        public PageData<T_UserMasterEntity> GetUserMaster([FromQuery] UserMasterQueryDTO query)
        { 
            var attentions = _userRepository.AsQueryable()
                   .WhereIf(!query.GUID.IsNullOrEmpty(), e => e.GUID == query.GUID)
                   .WhereIf(!query.EmployeeId.IsNullOrEmpty(), e => e.EmployeeId.Contains(query.EmployeeId))
                   .WhereIf(!query.User_Name.IsNullOrEmpty(), e => e.User_Name.Contains(query.User_Name))
                   .WhereIf(query.OrganizationID.HasValue , e => e.OrganizationID == query.OrganizationID.Value)
                   .WhereIf(query.BusinessType.HasValue , e => e.BusinessType == query.BusinessType.Value);

            return PageData<T_UserMasterEntity>.Build(query, attentions.OrderByDescending(e => e.CreateTime).ApplyPageFilter(query).ToArray().Adapt<T_UserMasterEntity[]>(), attentions.Count());

        }

        /// <summary>
        /// CreateUserMaster
        /// </summary>
        /// <param name="user">用户信息</param>
        /// <returns></returns>
        [HttpPost("CreateUserMaster")]
        public async Task<ApiResult<string>> CreateUserMaster([FromBody] UserMasterAddDTO user)
        {
            var userModel = await _userRepository.GetAsync(p => p.GUID == user.GUID);
            if (userModel != null)
                throw new BizException($"用户账号添加重复: {user.GUID}");

            var userEntity = user.Adapt<T_UserMasterEntity>();

            var password = user.Password;

            if (string.IsNullOrEmpty(password))
            {
                password = CommonUtils.GenerateRandomString();
            }
            
            string newEncryptedPassword = CommonUtils.GenerateSaltedHash(password, TokenParameter.PasswordSalt);

            userEntity.Password = newEncryptedPassword;
            userEntity.CreateTime = DateTime.Now;
            userEntity.UpdateTime = DateTime.Now;
             
            var count = await _userRepository.InsertAsync(userEntity); 
            
            return count ?  $"创建成功!! 密码为：{password}" : "创建失败!";
        }

        /// <summary>
        /// RemoveUserMaster
        /// </summary>
        /// <param name="guid"></param>
        /// <returns></returns>
        /// <exception cref="BizException"></exception>
        [HttpPost("RemoveUserMaster")]
        public async Task<ApiResult<bool>> RemoveUserMaster(string guid)
        {
            var attentions = await _userRepository.AsQueryable().FirstOrDefaultAsync(a => a.GUID == guid);

            if (attentions == null)
                throw new BizException(L($"数据不存在"));

            return await _userRepository.RemoveAsync(attentions);
        }

        /// <summary>
        /// UpdateUserMaster
        /// </summary>
        /// <param name="attention"></param>
        /// <returns></returns>
        /// <exception cref="BizException"></exception>
        [HttpPost("UpdateUserMaster")]
        public async Task<ApiResult<bool>> UpdateUserMaster([FromBody] UserMasterUpdateDTO attention)
        {
            var attentions = await _userRepository.AsQueryable().FirstOrDefaultAsync(a => a.GUID == attention.GUID);

            if (attentions == null)
                throw new BizException(L($"数据不存在"));

            var attentionEntity = attention.Adapt(attentions);
            attentionEntity.UpdateTime = DateTime.Now;

            return await _userRepository.UpdateAsync(attentionEntity);
        }
    }
}
