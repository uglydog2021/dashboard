using MES_ReportForms.Core.Models;
using MES_ReportForms.API;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using MES_ReportForms.Core;
using MediatR;
using System.Text.Json.Nodes;
using MES_ReportForms.Core.Utils;
using MES_ReportForms.Core.Repositories.EF;
using System;

namespace MES_ReportForms.API.Controllers
{
    /// <summary>
    /// 身份授权
    /// </summary>
    [Route("api/auth"), EndpointGroupName(nameof(ApiVersionInfo.ReportFormAPI))]
    public class AuthController : ApiControllerBase
    { 
        UserMasterRepository _userRepository;
        ApplicationDbContext _dbContext;

        public AuthController(ApplicationDbContext dbContext, UserMasterRepository userRepository)
        { 
            _dbContext = dbContext;
            _userRepository = userRepository;
        }

        /// <summary>
        /// 获取 Token 
        /// </summary>
        /// <param name="guid">用户名</param>
        /// <param name="password">密码</param>
        /// <returns>JWT Token</returns>
        [HttpPost("token")]
        public ApiResult<string> GetToken([Required] string guid, [Required] string password)
        {
            var user = _userRepository.AsQueryable().Where(u => u.GUID == guid).FirstOrDefault()
               ?? throw new BizException(L("登录用户不存在"));

            var passwordCiphertext = CommonUtils.GenerateSaltedHash(password, TokenParameter.PasswordSalt);
            if (passwordCiphertext != user.Password)
            {
                throw new BizException(L("用户名或密码错误！"));
            }

            return JwtHelper.GenerateJsonWebToken(new UserSession()
            {
                GUID = user.GUID,
                EmployeeId = user.EmployeeId,
                UserName = user.User_Name,
                OrganizationID = user.OrganizationID,
                UserEmail = user.User_Email,
                WebPermission = user.Web_Permission,
                DataPermission = user.Data_Permission,
                ImportPermission = user.Import_Permission,
            });
        }

        /// <summary>
        /// 刷新token，以便产生新有效期的token
        /// </summary>
        /// <param name="token">新Token</param>
        /// <returns></returns>
        [HttpPost("refresh-token")]
        public ApiResult<string> RefreshToken([Required] string token)
        {
            try
            {
                // 校验传入的token，如果token有效将返回身份信息，否则异常将被捕获
                ClaimsPrincipal principal = JwtHelper.ValidateToken(token, false);
                
                // 刷新Token时，对携带的token允许过期时间限制不超过2小时
                if (JwtHelper.ValidateExpired(token, 60 * 120))
                    throw new BizException(L("刷新Token失败，传入的Token已经过期超过期限"));

                // 将身份信息转换为用户会话信息
                var session = JwtHelper.ClaimsToSession(principal);

                if (session == null)
                    throw new Exception("Failed to resolve the token because the content may be incorrect.");

                // 生成新的token
                return JwtHelper.GenerateJsonWebToken(session);
            }
            catch(BizException)
            {
                throw;
            }
            catch
            {
                return ApiResult<string>.ToError("无效的token");
            }
        }
        
        /// <summary>
        /// 获取当前登录用户信息
        /// </summary>
        private UserSession GetLoginUser(bool isValidateExpireTime = true)
        {
            try
            {
                var accessToken = Request.Headers.Authorization.FirstOrDefault();

                if (string.IsNullOrEmpty(accessToken))
                    return null;

                if (accessToken.StartsWith("Bearer "))
                    accessToken = accessToken.Substring(7);

                var claimsPrincipal = JwtHelper.ValidateToken(accessToken, false);
                return JwtHelper.ClaimsToSession(claimsPrincipal);
            }
            catch
            {
                return null;
            }
        }
    }
}

