using Newtonsoft.Json;
using System.Diagnostics;
using System.IO;
using System.Text;

namespace MES_ReportForms.API.Middlewares
{
    /// <summary>
    /// 修改响应数据内容
    /// </summary>
    public class ModifyReponseMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ModifyReponseMiddleware> _logger;

        public ModifyReponseMiddleware(RequestDelegate next, ILogger<ModifyReponseMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            if (!context.Request.Path.StartsWithSegments("/api", StringComparison.OrdinalIgnoreCase))
            {
                await _next(context);
                return;
            }

            // 保存原始响应流
            var originalBodyStream = context.Response.Body;
            try
            {
                // 创建新的内存流来替换响应流
                using (var memoryStream = new MemoryStream())
                {
                    context.Response.Body = memoryStream;
                     
                    // 调用下一个中间件
                    await _next(context);
                    // 将内存流指针重置到开始位置
                    memoryStream.Seek(0, SeekOrigin.Begin);

                    // 读取响应体内容
                    var responseBody = new StreamReader(memoryStream).ReadToEnd();

                    // 在此处可以修改响应体，例如删除某些 JSON 属性
                    dynamic responseObject = JsonConvert.DeserializeObject(responseBody);

                    //if (responseObject != null && responseObject["result"]["items"] != null)
                    //{
                    //    foreach (var item in responseObject["result"]["items"])
                    //    {
                    //        item.Remove("isDelete");
                    //        item.Remove("createUser");
                    //        item.Remove("createDate");
                    //        item.Remove("modifierUser");
                    //        item.Remove("modifierDate");
                    //    }
                    //}

                    var modifiedResponseBody = JsonConvert.SerializeObject(responseObject);


                    await memoryStream.CopyToAsync(originalBodyStream);

                    context.Response.Body = originalBodyStream;
                }
            }
            finally
            {
            }

        }
    }

}
