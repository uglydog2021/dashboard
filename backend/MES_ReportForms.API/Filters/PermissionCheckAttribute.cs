using System.Diagnostics.CodeAnalysis;
using System.Net;
using MES_ReportForms.Core.Models; 
using MES_ReportForms.Core.Utils;
using MES_ReportForms.Core;
using MES_ReportForms.Core.Repositories;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;


namespace MES_ReportForms.API.Filters
{
    /// <summary>
    /// 功能权限验证
    /// </summary>
    public class PermissionCheckAttribute : ActionFilterAttribute
    {
        public const string CACHE_KEY = "Group_PermissionCodeList";
        const int CACHE_DURATION_MINS = 60; //缓存60分钟
        private ApplicationDbContext _db;

        public string _requiredPermissionCode;

        /// <summary>
        /// PermissionCheckAttribute
        /// </summary>
        /// <param name="requiredMenuValue"></param>
        public PermissionCheckAttribute(string requiredMenuValue)
        {
            _requiredPermissionCode = requiredMenuValue;
        }
         
        record GroupPermission(Guid GroupId, string PermissionCode);

        /// <summary>
        /// 清除缓存
        /// </summary>
        public static void CleanCache()
        {
            CacheHelper.RemoveCache(CACHE_KEY);
        }

    }
}