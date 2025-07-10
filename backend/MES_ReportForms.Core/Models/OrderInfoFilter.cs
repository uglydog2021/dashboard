using MES_ReportForms.Core;
using System.ComponentModel.DataAnnotations;

namespace MES_ReportForms.Core.Models.System
{
    public class OrderInfoFilter : PageFilter
    {
        /// <summary>
        /// 
        /// </summary>
        public string ConnectionName { get; set; }

        /// <summary>
        /// 可为空，不传则全部
        /// </summary>
        public string FileName { get; set; }
    }


    public class OrderInfoExport
    {
        /// <summary>
        /// 
        /// </summary>
        public string ConnectionName { get; set; }
         
        /// <summary>
        /// 
        /// </summary>
        public string FileName { get; set; }
    }

}
