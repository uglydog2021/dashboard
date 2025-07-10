using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.EntityFrameworkCore;
using System;
using System.Data;
using MES_ReportForms.Core;
using Microsoft.Extensions.DependencyInjection;

namespace MES_ReportForms.API.Filters
{
    /// <summary>
    /// 为事务过滤器
    /// </summary>
    public class TransactionFilterAttribute : ActionFilterAttribute
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="context"></param>
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            // 获取数据库上下文
            var dbContext = context.HttpContext.RequestServices.GetService<ApplicationDbContext>();

            if (dbContext != null && dbContext.Database.CurrentTransaction == null)
            {
                // 开启事务
                dbContext.Database.BeginTransaction();
            }

            base.OnActionExecuting(context);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="context"></param>
        public override void OnActionExecuted(ActionExecutedContext context)
        {
            var dbContext = context.HttpContext.RequestServices.GetService<ApplicationDbContext>();

            if (dbContext.Database.CurrentTransaction != null)
            {
                // 检查操作是否成功
                if (context.Exception == null)
                {
                    // 提交事务
                    dbContext.Database.CurrentTransaction.Commit();
                }
                else
                {
                    // 发生异常时回滚事务
                    dbContext.Database.CurrentTransaction.Rollback();
                }
            }

            base.OnActionExecuted(context);
        }
    }
}