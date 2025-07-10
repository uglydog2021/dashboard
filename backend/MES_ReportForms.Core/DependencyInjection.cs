using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace MES_ReportForms.Core
{
    /// <summary>
    /// 注入Core中的依赖。主程序引用Core，需要调整此方法，否则ApplicationDbContext和Repositories可能无法使用。
    /// </summary>
    public static class DependencyInjection
    {
        public static IServiceCollection AddCoreServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<ApplicationDbContext>(options =>
            {
                options.UseSqlServer(configuration.GetConnectionString("db.connectionString"));

                var isDvelopment = "Development".Equals(Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT"), StringComparison.OrdinalIgnoreCase);
                if (isDvelopment)
                {
                    options.EnableSensitiveDataLogging();
                }
            });

            // 注入 MES_ReportForms.Core.Repositories 命名空间下的所有类
            services.AddScopedAssemblyServices(Assembly.GetExecutingAssembly(), "MES_ReportForms.Core.Repositories.EF", "Repository");

            return services;
        }
    }
}
