
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MES_ReportForms.Core.Entities.System
{
    /// <summary>
    /// 
    /// </summary>

    [Table("T_EmployeeAttendance")]
    public class T_EmployeeAttendanceEntity
    {
        /// <summary>
        /// 自增长主键
        /// </summary>
        [Key]
        public int Id { get; set; }

        /// <summary>
        /// GUID
        /// </summary>
        public string GUID { get; set; }

        /// <summary>
        /// Employee ID
        /// </summary>
        public string EmployeeId { get; set; }

        /// <summary>
        /// 姓名
        /// </summary>
        public string UserName { get; set; }

        /// <summary>
        /// 休假类型（1.AnnualLeave年假 2. SickLeave 病假 3.OtherSpecialLeave 其他 4.OvertimeShift 串休）
        /// </summary>
        public int LeaveType { get; set; }

        /// <summary>
        /// 考勤年份
        /// </summary>
        public int AttendanceYear { get; set; }

        /// <summary>
        /// 考勤月份（用数字 1 - 12 表示）
        /// </summary>
        public int AttendanceMonth { get; set; }

        /// <summary>
        /// 考勤日期
        /// </summary>
        public int AttendanceDay { get; set; }

        /// <summary>
        /// 休假时长（串休以外的假期单位为天，表示为1或者0.5，0.5代表半天，其他数是串休）
        /// </summary>
        public decimal LeaveDays { get; set; }
    }
}
