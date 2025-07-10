using MES_ReportForms.API;
using MES_ReportForms.Core;
using MES_ReportForms.Core.Models;
using MES_ReportForms.Core.Utils;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;

namespace MES_ReportForms.API.Controllers
{
    /// <summary>
    /// API 抽象基类 
    /// </summary>
    [ApiController]
    [Produces("application/json")]
    public abstract class ApiControllerBase : ControllerBase
    {
        [FromServices]
        public IStringLocalizer<Resources> Localizer { get; set; }

        /// <summary>
        ///  
        /// </summary>
        /// <param name="name"></param>
        /// <param name="arguments"></param>
        /// <returns></returns>
        protected String L(string name, params object[] arguments)
        {
            return this.Localizer.GetString(name, arguments).ToString();
        }

        /// <summary>
        /// 获取当前登录人信息
        /// </summary>
        /// <returns></returns>
        protected UserSession CurrentUserSession()
        {
            if (HttpContext == null)
            {
                throw new BizException(L("User not logged in"));
            }

            return JwtHelper.CurrentUserSession(HttpContext);
        }
    }
}
