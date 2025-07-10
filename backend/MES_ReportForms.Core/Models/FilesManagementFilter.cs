using MES_ReportForms.Core;
using System.ComponentModel.DataAnnotations;

namespace MES_ReportForms.Core.Models.System
{
    public class FilesManagementFilter : PageFilter
    {
        /// <summary>
        /// 
        /// </summary>
        public string ConnectionName { get; set; }

        /// <summary>
        /// 可为空，不传则全部
        /// </summary>
        public DateTime? StartData { get; set; }

        /// <summary>
        /// 可为空，不传则全部
        /// </summary>
        public DateTime? EndData { get; set; }

        /// <summary>
        /// 可为空，不传则全部
        /// </summary>
        public string TaskUser { get; set; }
         
        /// <summary>
        /// 可为空，不传则全部
        /// </summary>
        public string FileName { get; set; }

        /// <summary>
        /// 可为空，不传则全部
        /// </summary>
        public int? Status { get; set; }

    }

    public class FilesManagementExport
    {
        /// <summary>
        /// 
        /// </summary>
        public string ConnectionName { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public DateTime? StartData { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public DateTime? EndData { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string TaskUser { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public int? Status { get; set; }

    }
}
