using MES_ReportForms.API;
using Microsoft.EntityFrameworkCore;
using MES_ReportForms.Core.Utils;
using MES_ReportForms.Core.Models;

namespace MES_ReportForms.API.Middlewares
{
    /// <summary>
    /// 全局异常过滤器
    /// 调用示例：throw new NotImplementedException();
    /// </summary>
    public class GlobalExceptionMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<GlobalExceptionMiddleware> _logger;
        private readonly bool _alwaysMessageToResponse;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="next"></param>
        /// <param name="logger"></param>
        /// <param name="alwaysMessageToResponse"></param>
        public GlobalExceptionMiddleware(RequestDelegate next, ILogger<GlobalExceptionMiddleware> logger, bool alwaysMessageToResponse = false)
        {
            _next = next;
            _logger = logger;
            _alwaysMessageToResponse = alwaysMessageToResponse;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                var originalBodyStream = context.Response.Body;
                try
                {
                    using (var memoryStream = new MemoryStream())
                    {
                        context.Response.Body = memoryStream;

                        await _next(context);
                         
                        memoryStream.Seek(0, SeekOrigin.Begin);
                        await memoryStream.CopyToAsync(originalBodyStream);
                    }
                }
                finally {
                    // 恢复原始响应流
                    context.Response.Body = originalBodyStream;
                }
            }
            catch (BizException ex)
            {
                _logger.LogDebug(ex, "全局异常拦截中间件捕获到未处理的异常: " + ex.Message);
                context.Response.StatusCode = StatusCodes.Status200OK;
                await context.Response.WriteAsJsonAsync(
                    ApiResult.ToError(
                        _alwaysMessageToResponse ? ex.Message :
                                ex.IsThrowToFrontend ? ex.Message : ApiResult.MESSAGE_ERROR));
            }
            catch (DbUpdateException ex)
            {
                _logger.LogError(ex, "全局异常拦截中间件捕获到未处理的异常: " + ex.Message);
                context.Response.StatusCode = StatusCodes.Status500InternalServerError;
                await context.Response.WriteAsJsonAsync(ApiResult.ToError(_alwaysMessageToResponse ? ex.Message : "A database error occurred. Please try again later."));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "全局异常拦截中间件捕获到未处理的异常: " + ex.GetBaseException().Message);

                // 设置HTTP状态码
                context.Response.StatusCode = StatusCodes.Status500InternalServerError;

                // 返回一个自定义的错误响应
                await context.Response.WriteAsJsonAsync(ApiResult.ToError(_alwaysMessageToResponse ? ex.Message : "An unexpected error occurred. Please try again later."));
            }
        }
    }
}
