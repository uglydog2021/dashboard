
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MES_ReportForms.Core.Entities.System
{
    /// <summary>
    /// 
    /// </summary>

    [Table("T_Attention")]
    public class T_AttentionEntity
    {
        /// <summary>
        /// 注意事项的唯一标识符。该字段为主键且自动增长。
        /// </summary>
        [Key]
        public int Attention_Id { get; set; }

        /// <summary>
        /// 组织的唯一标识符。
        /// </summary>
        public int? OrganizationId { get; set; }

        /// <summary>
        /// 组织的名称。
        /// </summary>
        public string OrganizationName { get; set; }

        /// <summary>
        /// 注意事项的开始时间。
        /// </summary>
        public DateTime Start_Time { get; set; }

        /// <summary>
        /// 注意事项的结束时间。
        /// </summary>
        public DateTime End_Time { get; set; }

        /// <summary>
        /// 发布人（用户）的唯一标识符。
        /// </summary>
        public string User_Id { get; set; }

        /// <summary>
        /// 注意事项的简短标题或主题。
        /// </summary>
        public string Subject { get; set; }

        /// <summary>
        /// 注意事项的详细内容。
        /// </summary>
        public string Message { get; set; }

        /// <summary>
        /// 记录创建的时间。
        /// </summary>
        public DateTime CreateTime { get; set; }

        /// <summary>
        /// 记录最后更新时间。
        /// </summary>
        public DateTime? UpdateTime { get; set; }
    }
}
