
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MES_ReportForms.Core.Entities.System
{
    /// <summary>
    /// 
    /// </summary>

    [Table("T_Organization")]
     
    public class T_OrganizationEntity
    {
        /// <summary>
        /// 部门ID
        /// </summary>
        [Key]
        public int OrganizationID { get; set; }

        /// <summary>
        /// 部门名称。最大长度为200个字符，不能为空。
        /// </summary>
        public string OrganizationName { get; set; }

        /// <summary>
        /// Maker每次获取任务的件数，不能为空。
        /// </summary>
        public int MakerTaskCount { get; set; }

        /// <summary>
        /// Checker每次获取任务的件数，不能为空。
        /// </summary>
        public int CheckerTaskCount { get; set; }

        /// <summary>
        /// 组织的创建时间。默认值为当前时间（GETDATE()）。
        /// </summary>
        public DateTime? CreateTime { get; set; }

        /// <summary>
        /// 组织的更新时间。此字段为可空类型。
        /// </summary>
        public DateTime? UpdateTime { get; set; }
    }

}
