using MES_ReportForms.Core;
using System.ComponentModel.DataAnnotations;

namespace MES_ReportForms.Core.Models.System
{
    public class ExecuteFilter
    {
        /// <summary>
        /// 
        /// </summary>
        public string ConnectionName { get; set; }
         
        /// <summary>
        /// 
        /// </summary>
        public string Sql { get; set; }
    }

}
