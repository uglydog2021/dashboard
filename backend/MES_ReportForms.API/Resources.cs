using MES_ReportForms.Core.Utils;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.Extensions.Localization;
using System.Runtime.CompilerServices;

namespace MES_ReportForms.API
{
    /// <summary>
    /// 
    /// </summary>
    public class Resources
    {
        public const string NotFound = "Not Found";
    }

    /// <summary>
    /// 
    /// </summary>
    public static class ResourcesExtensions
    {

        /// <summary>
        /// 获取多语言资源，将文本抛出业务异常
        /// </summary>
        /// <param name="localizer">本地化对象</param>
        /// <param name="name">资源名</param>
        /// <param name="args">string format的参数，用于格式化资源文本中的{0}等</param>
        /// <returns>抛出业务异常</returns>
        /// <exception cref="BizException"></exception>
        public static void ThrowBizException(this IStringLocalizer<Resources> localizer, string name, params object[] args)
        {
            throw new BizException(localizer.GetString(name, args));
        }
    }
}
