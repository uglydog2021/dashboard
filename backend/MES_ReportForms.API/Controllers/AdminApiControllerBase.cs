using MES_ReportForms.Core;
using Microsoft.AspNetCore.Authorization;

namespace MES_ReportForms.API.Controllers
{
    /// <summary>
    /// 需要验证权限
    /// </summary>
    [Authorize]
    public abstract class AuthorizeApiControllerBase : ApiControllerBase {  }
}
