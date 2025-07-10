using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MES_ReportForms.Core.Models
{
    public class AttentionAddDTO
    { 
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
        public DateTime? Start_Time { get; set; }

        /// <summary>
        /// 注意事项的结束时间。
        /// </summary>
        public DateTime? End_Time { get; set; }

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
    }
    public class AttentionUpdateDTO
    {
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
        public DateTime? Start_Time { get; set; }

        /// <summary>
        /// 注意事项的结束时间。
        /// </summary>
        public DateTime? End_Time { get; set; }

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
    }

    public class AttentionQueryDTO : PageFilter
    {
        /// <summary>
        /// 组织的名称，可为空，不传则全部
        /// </summary>
        public string OrganizationName { get; set; }

        /// <summary>
        /// 注意事项的开始时间，可为空，不传则全部
        /// </summary>
        public DateTime? Start_Time { get; set; }

        /// <summary>
        /// 注意事项的结束时间，可为空，不传则全部
        /// </summary>
        public DateTime? End_Time { get; set; }

        /// <summary>
        /// 发布人（用户），可为空，不传则全部
        /// </summary>
        public string User_Id { get; set; }

    }
}
