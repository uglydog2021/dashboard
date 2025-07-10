using MES_ReportForms.Core;
using System.ComponentModel.DataAnnotations;

namespace MES_ReportForms.Core.Models.System
{
    public class ActionHistoryFilter : PageFilter
    {
        /// <summary>
        /// 
        /// </summary>
        public string ConnectionName { get; set; }

        /// <summary>
        /// 可为空，不传则全部
        /// </summary>
        public string FileName { get; set; }

        /// <summary>
        /// 可为空，不传则全部
        /// </summary>
        public DateTime? ActionStartDate { get; set; }

        /// <summary>
        /// 可为空，不传则全部
        /// </summary>
        public DateTime? ActionEndDate { get; set; }

        /// <summary>
        /// 可为空，不传则全部
        /// </summary>
        public string TaskUser { get; set; }

    }

    public class ActionGroupUserHistoryFilter : PageFilter
    {
        /// <summary>
        /// 
        /// </summary>
        public string ConnectionName { get; set; }
         
        /// <summary>
        /// 
        /// </summary>
        public DateTime? ActionStartDate { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public DateTime? ActionEndDate { get; set; }
          
    }
    public class ActionHistoryExport
    {
        /// <summary>
        /// 
        /// </summary>
        public string ConnectionName { get; set; }
         
        /// <summary>
        /// 
        /// </summary>
        public string FileName { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public DateTime? ActionStartDate { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public DateTime? ActionEndDate { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string TaskUser { get; set; }

    }

}
