using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MES_ReportForms.Core.Models
{
    public class UserSession
    {
        public string GUID { get; set; }

        /// <summary>
        /// 获取或设置员工ID。最大长度为100个字符。
        /// </summary>
        public string EmployeeId { get; set; }

        /// <summary>
        /// 获取或设置用户名。此字段为必填项。
        /// </summary>
        public string UserName { get; set; }

        /// <summary>
        /// 获取或设置组织ID。此字段长度为2个字符。
        /// </summary>
        public int? OrganizationID { get; set; }
         
        /// <summary>
        /// 获取或设置用户的电子邮件地址。最大长度为255个字符。
        /// </summary>
        public string UserEmail { get; set; }

        /// <summary>
        /// 获取或设置用户的Web权限。
        /// </summary>
        public int? WebPermission { get; set; }

        /// <summary>
        /// 获取或设置用户的数据权限。
        /// </summary>
        public int? DataPermission { get; set; }

        /// <summary>
        /// 获取或设置用户的导入权限。默认值为0。
        /// </summary>
        public int? ImportPermission { get; set; }

    }
}
