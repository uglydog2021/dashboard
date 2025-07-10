using Azure.Core;
using MES_ReportForms.Core;
using MES_ReportForms.Core.Models;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace MES_ReportForms.API
{
    /// <summary>
    /// JWT Helper
    /// </summary>
    public static class JwtHelper
    {
        /// <summary>
        /// 生成JWT令牌。
        /// </summary>
        /// <param name="session">用户信息</param>
        /// <returns></returns>
        public static string GenerateJsonWebToken(UserSession session)
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(TokenParameter.Secret));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);
            var claimsIdentity = new ClaimsIdentity(CookieAuthenticationDefaults.AuthenticationScheme);
            // 用户ID
            claimsIdentity.AddClaim(new Claim(JwtRegisteredClaimNames.Sub, session.UserName));
            // 签发时间（时间戳）
            claimsIdentity.AddClaim(new Claim(JwtRegisteredClaimNames.Iat, DateTimeOffset.Now.ToUnixTimeSeconds()
                                                                                             .ToString()));

            // 用户类型（自定义）
            claimsIdentity.AddClaim(new Claim("profile", JsonSerializer.Serialize(session, new JsonSerializerOptions() { DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull })));
             
            var token = new JwtSecurityToken(TokenParameter.Issuer,
              TokenParameter.Audience,
              claimsIdentity.Claims,
              expires: DateTime.Now.AddMinutes(TokenParameter.AccessExpiration),
              signingCredentials: credentials);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        /// <summary>
        /// 当前请求用户会话信息
        /// </summary>
        /// <param name="httpContext"></param>
        /// <returns></returns>
        public static UserSession CurrentUserSession(HttpContext httpContext)
        {
            var claimsPrincipal = httpContext.User;
            return ClaimsToSession(claimsPrincipal);
        }

        /// <summary>
        /// 当前请求用户会话信息
        /// </summary>
        /// <param name="claimsPrincipal"></param>
        /// <returns></returns>
        public static UserSession ClaimsToSession(ClaimsPrincipal claimsPrincipal)
        {
            if (claimsPrincipal == null || claimsPrincipal.Claims == null || claimsPrincipal.Claims.Count() == 0)
                return null;

            var sub = claimsPrincipal.Claims.FirstOrDefault(c => c.Type == JwtRegisteredClaimNames.Sub);
            var iat = claimsPrincipal.Claims.FirstOrDefault(c => c.Type == JwtRegisteredClaimNames.Iat);
            var profile = claimsPrincipal.Claims.FirstOrDefault(c => c.Type == "profile");

            if ((sub == null || sub.Value == null) && profile == null)
                return null;

            if (profile == null)
                return new UserSession { UserName = sub.Value };

            return JsonSerializer.Deserialize<UserSession>(profile.Value);
        }

        /// <summary>
        /// 检校Token
        /// </summary>
        /// <param name="token"></param>
        /// <param name="isValidateExpireTime"></param>
        /// <returns></returns>
        public static ClaimsPrincipal ValidateToken(string token, bool isValidateExpireTime = true)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var validationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = isValidateExpireTime,
                ValidateIssuerSigningKey = true,
                ValidIssuer = TokenParameter.Issuer,
                ValidAudience = TokenParameter.Audience,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(TokenParameter.Secret))
            };
            return tokenHandler.ValidateToken(token, validationParameters, out SecurityToken validatedToken);
        }

        /// <summary>
        /// 验证JWT Token过期时间是否超过阈值
        /// </summary>
        /// <param name="token"></param>
        /// <param name="allowExpiredMaxSeconds"></param>
        public static bool ValidateExpired(string token, int allowExpiredMaxSeconds)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var issuedAt = tokenHandler.ReadJwtToken(token).IssuedAt;

            if (token.StartsWith("Bearer "))
                token = token.Substring(7);

            // 校验时间戳是否过期，防止重复利用。而时间戳信息就存在IV中，即前8位字节。
            var tokenExpiredTime = issuedAt.AddMinutes( TokenParameter.AccessExpiration);

            return (DateTime.Now.ToUniversalTime() - tokenExpiredTime.ToUniversalTime()) > TimeSpan.FromMinutes(allowExpiredMaxSeconds);
        }
    }
}