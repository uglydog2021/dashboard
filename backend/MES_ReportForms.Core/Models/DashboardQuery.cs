using System.ComponentModel.DataAnnotations;

namespace MES_ReportForms.Core.Models
{  
    public class AttendanceRecordQuery
    {
        /// <summary>
        /// 可不传
        /// </summary>
        public string ConnectionName { get; set; }
        /// <summary>
        /// 实际部门ID，必传
        /// </summary>
        [Required]
        public int OrganizationID { get; set; }

        ///// <summary>
        ///// 实际日期，可为空，默认当天
        ///// </summary>
        //public DateTime? LoginTime { get; set; }

        ///// <summary>
        ///// 当前年份，可为空，默认当前年份
        ///// </summary>
        //public int? AttendanceYear { get; set; }

        ///// <summary>
        ///// 当前月份，可为空，默认当前月份
        ///// </summary>
        //public int? AttendanceMonth { get; set; }

        ///// <summary>
        ///// 当前天，可为空，默认当前天
        ///// </summary>
        //public int? AttendanceDay { get; set; }
    }

    public class Dashboard6Query
    {
        /// <summary>
        /// 可不传
        /// </summary>
        public string ConnectionName { get; set; }

        /// <summary>
        /// 实际部门ID，可为空，不传则全部
        /// </summary>
        public int? OrganizationID { get; set; }

    }

    public class MissCountQuery
    {
        /// <summary>
        /// 可不传
        /// </summary>
        public string ConnectionName { get; set; }

        /// <summary>
        /// 实际部门ID，可为空，不传则全部
        /// </summary>
        public int? OrganizationID { get; set; }
        
        ///// <summary>
        ///// 可为空，不传则为全部
        ///// </summary>
        //public DateTime? CreateDate { get; set; }

        /// <summary>
        /// 类型（Day:日[默认]；Week：周；Month：月）
        /// </summary>
        public string Type { get; set; } = "Day";

    }
    public class DepartmentMonthCurrentYearQuery
    {
        /// <summary>
        /// 可不传
        /// </summary>
        public string ConnectionName { get; set; }

        ///// <summary>
        ///// 实际部门ID，可为空，不传则全部
        ///// </summary>
        //public int? OrganizationID { get; set; }
        
        ///// <summary>
        ///// 可为空，不传则为全部
        ///// </summary>
        //public DateTime? CreateDate { get; set; } 

    }
    
    public class OrganizationalFormQuery
    {
        /// <summary>
        /// 可不传
        /// </summary>
        public string ConnectionName { get; set; }

        /// <summary>
        /// 实际部门ID，可为空，不传则全部
        /// </summary>
        public int? OrganizationID { get; set; }
         
        /// <summary>
        /// 类型（Day:日[默认]；Week：周；Month：月）
        /// </summary>
        public string Type { get; set; } = "Day";

    }
     
    public class DailyPendingQuery
    {
        /// <summary>
        /// 可不传
        /// </summary>
        public string ConnectionName { get; set; }

        /// <summary>
        /// 可为空，不传则为全部
        /// </summary>
        public DateTime? CreateDate { get; set; }
    }
     
    public class DailyAttentionQuery
    {
        /// <summary>
        /// 可不传
        /// </summary>
        public string ConnectionName { get; set; }
        /// <summary>
        /// 实际部门ID，可为空，不传则全部
        /// </summary>
        public int? OrganizationID { get; set; }
    }

}
