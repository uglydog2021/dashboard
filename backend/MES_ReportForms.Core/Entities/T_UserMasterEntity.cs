
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MES_ReportForms.Core.Entities.System
{
    [Table("T_UserMaster")]
    public class T_UserMasterEntity
    {
        /// <summary>
        /// 主键。
        /// </summary>
        [Key]
        public string GUID { get; set; }

        /// <summary>
        /// 员工ID。最大长度为100个字符。
        /// </summary>
        public string EmployeeId { get; set; }

        /// <summary>
        /// 用户名。此字段为必填项。
        /// </summary>
        public string User_Name { get; set; }

        /// <summary>
        /// 组织ID。此字段长度为2个字符。
        /// </summary>
        public int? OrganizationID { get; set; }

        /// <summary>
        /// 用户的密码。此字段为必填项，最大长度为255个字符。
        /// </summary>
        public string Password { get; set; }

        /// <summary>
        /// 用户的电子邮件地址。最大长度为255个字符。
        /// </summary>
        public string User_Email { get; set; }

        /// <summary>
        /// 用户的Web权限。
        /// </summary>
        public int? Web_Permission { get; set; }

        /// <summary>
        /// 用户的数据权限。
        /// </summary>
        public int? Data_Permission { get; set; }

        /// <summary>
        /// 用户的导入权限。默认值为0。
        /// </summary>
        public int? Import_Permission { get; set; }

        /// <summary>
        /// 业务类型编码表：
        /// 1 = Order processing（订单处理）
        /// 2 = Quotation Management（报价管理）
        /// 3 = Sales Support（销售支持）
        /// </summary>
        public int? BusinessType { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string BusinessTypeText
        {
            get
            {
                return BusinessType switch
                {
                    1 => "Order processing",
                    2 => "Quotation Management",
                    3 => "Sales Support",
                    _ => "Unknown"
                };
            }
        }
        /// <summary>
        /// 用户记录的创建时间。
        /// 默认值为当前时间（GETDATE()）。该字段为可空类型。
        /// </summary>
        public DateTime? CreateTime { get; set; }

        /// <summary>
        /// 用户记录的最后更新时间。
        /// 该字段为可空类型。
        /// </summary>
        public DateTime? UpdateTime { get; set; }
    } 
}
