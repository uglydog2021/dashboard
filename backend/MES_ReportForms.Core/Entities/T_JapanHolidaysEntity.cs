
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MES_ReportForms.Core.Entities.System
{

    /// <summary>
    /// 
    /// </summary>
    [Table("T_JapanHolidays")]
    public class T_JapanHolidaysEntity
    {
        /// <summary>
        /// 
        /// </summary>
        [Key]
        public int Year { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public DateTime HolidayDate { get; set; } 

        /// <summary>
        /// 
        /// </summary>
        public string HolidayName { get; set; } 
    }
}
