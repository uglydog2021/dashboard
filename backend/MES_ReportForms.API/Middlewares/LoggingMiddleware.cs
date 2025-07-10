using System.Diagnostics;
using System.IO.Pipelines;

namespace MES_ReportForms.API.Middlewares
{
    /// <summary>
    /// 记录API调用日志
    /// </summary>
    public class LoggingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<LoggingMiddleware> _logger;

        public LoggingMiddleware(RequestDelegate next, ILogger<LoggingMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var request = context.Request;

            if (!context.Request.Path.StartsWithSegments("/api", StringComparison.OrdinalIgnoreCase))
            {
                await _next(context);
                return;
            }


            var clientIp = context.Connection.RemoteIpAddress;
            using var ms = new MemoryStream();

            var originalStream = context.Response.Body;
            try
            {
                context.Response.Body = ms;

                // 记录请求开始时间
                var stopwatch = Stopwatch.StartNew();
                // 调用管道中的下一个Middleware
                await _next(context);
                // 停止计时器并记录耗时
                stopwatch.Stop();
                var responseTime = stopwatch.ElapsedMilliseconds;

                ms.Seek(0, SeekOrigin.Begin);
                var responseContent = await new StreamReader(ms).ReadToEndAsync();
                var responseLength = responseContent == null ? 0 : responseContent.Length;
                ms.Seek(0, SeekOrigin.Begin);
                await ms.CopyToAsync(originalStream);

                // 记录响应状态码和耗时
                _logger.LogInformation(
                    "{Method} {Url}{QueryString} {ClientIP}\nStatus code: {StatusCode}, Response Length: {responseLength} Response time: {ResponseTime} ms",
                        request.Method,
                        request.Path,
                        request.QueryString.ToString(),
                        clientIp,
                        context.Response.StatusCode,
                        responseLength,
                        responseTime);
            }
            finally
            {
                context.Response.Body = originalStream;
            }
        }
    }

}
