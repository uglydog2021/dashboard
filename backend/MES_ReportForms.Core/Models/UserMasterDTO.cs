using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MES_ReportForms.Core.Models
{
    public class UserMasterAddDTO
    { 
        /// <summary>
        /// 主键。
        /// </summary> 
        public string GUID { get; set; }

        /// <summary>
        /// 获取或设置员工ID
        /// </summary>
        public string EmployeeId { get; set; }

        /// <summary>
        /// 用户名
        /// </summary>
        public string User_Name { get; set; }

        /// <summary>
        /// 组织ID
        /// </summary>
        public int? OrganizationID { get; set; }

        /// <summary>
        /// 用户的密码
        /// </summary>
        public string Password { get; set; }

        /// <summary>
        /// 用户的电子邮件地址
        /// </summary>
        public string User_Email { get; set; }

        /// <summary>
        /// 用户的Web权限，必填
        /// </summary>
        public int Web_Permission { get; set; }

        /// <summary>
        /// 用户的数据权限，必填
        /// </summary>
        public int Data_Permission { get; set; }

        /// <summary>
        /// 用户的导入权限。默认值为0。
        /// </summary>
        public int Import_Permission { get; set; }

        /// <summary>
        /// 业务类型编码表：
        /// 1 = Order processing（订单处理）
        /// 2 = Quotation Management（报价管理）
        /// 3 = Sales Support（销售支持）
        /// </summary>
        public int BusinessType { get; set; }
    }

    public class UserMasterUpdateDTO
    {  
       /// <summary>
       /// 主键。
       /// </summary> 
        public string GUID { get; set; }

        /// <summary>
        /// 获取或设置员工ID
        /// </summary>
        public string EmployeeId { get; set; }

         /// <summary>
        /// 用户名。此字段为必填项。
        /// </summary>
        public string User_Name { get; set; }

        /// <summary>
        /// 组织ID
        /// </summary>
        public int? OrganizationID { get; set; }
          
        /// <summary>
        /// 用户的电子邮件地址
        /// </summary>
        public string User_Email { get; set; }

        /// <summary>
        /// 用户的Web权限。
        /// </summary>
        public int Web_Permission { get; set; }

        /// <summary>
        /// 用户的数据权限。
        /// </summary>
        public int Data_Permission { get; set; }

        /// <summary>
        /// 用户的导入权限。默认值为0。
        /// </summary>
        public int Import_Permission { get; set; }

        /// <summary>
        /// 业务类型编码表：
        /// 1 = Order processing（订单处理）
        /// 2 = Quotation Management（报价管理）
        /// 3 = Sales Support（销售支持）
        /// </summary>
        public int BusinessType { get; set; }
    }
    public class UserMasterQueryDTO : PageFilter
    {
        /// <summary>
        /// 主键，可为空，不传则全部
        /// </summary> 
        public string GUID { get; set; }

        /// <summary>
        ///  员工ID，可为空，不传则全部
        /// </summary>
        public string EmployeeId { get; set; }

        /// <summary>
        ///  用户名，可为空，不传则全部
        /// </summary>
        public string User_Name { get; set; }

        /// <summary>
        /// 组织ID，可为空，不传则全部
        /// </summary>
        public int? OrganizationID { get; set; }

        /// <summary>
        /// 业务类型编码表：
        /// 1 = Order processing（订单处理）
        /// 2 = Quotation Management（报价管理）
        /// 3 = Sales Support（销售支持）
        /// </summary>
        public int? BusinessType { get; set; }

    }
}
