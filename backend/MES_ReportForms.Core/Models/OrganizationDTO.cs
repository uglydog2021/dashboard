using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MES_ReportForms.Core.Models
{
    public class OrganizationAddDTO
    {
        /// <summary>
        /// 部门ID。固定长度2个字符，为主键。
        /// </summary>
        public int OrganizationID { get; set; }

        /// <summary>
        /// 部门名称。最大长度为200个字符，不能为空。
        /// </summary>
        public string OrganizationName { get; set; }

        /// <summary>
        /// Maker每次获取任务的件数，不能为空。
        /// </summary>
        public int? MakerTaskCount { get; set; }

        /// <summary>
        /// Checker每次获取任务的件数，不能为空。
        /// </summary>
        public int? CheckerTaskCount { get; set; }
         
    }
    public class OrganizationUpdateDTO
    {
        /// <summary>
        ///  部门ID。固定长度2个字符，为主键。
        /// </summary>
        public int OrganizationID { get; set; }

        /// <summary>
        /// 部门名称。最大长度为200个字符，不能为空。
        /// </summary>
        public string OrganizationName { get; set; }

        /// <summary>
        /// Maker每次获取任务的件数，不能为空。
        /// </summary>
        public int? MakerTaskCount { get; set; }

        /// <summary>
        /// Checker每次获取任务的件数，不能为空。
        /// </summary>
        public int? CheckerTaskCount { get; set; }
          
    }

    public class OrganizationQueryDTO : PageFilter
    {
        /// <summary>
        ///  部门ID，可为空，不传则全部
        /// </summary>
        public int? OrganizationID { get; set; }

        /// <summary>
        /// 部门名称，可为空，不传则全部
        /// </summary>
        public string OrganizationName { get; set; }

        /// <summary>
        /// Maker每次获取任务的件数，可为空，不传则全部
        /// </summary>
        public int? MakerTaskCount { get; set; }

        /// <summary>
        /// Checker每次获取任务的件数，可为空，不传则全部
        /// </summary>
        public int? CheckerTaskCount { get; set; } 

    }
    
}
