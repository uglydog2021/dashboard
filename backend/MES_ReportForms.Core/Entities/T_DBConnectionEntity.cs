
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MES_ReportForms.Core.Entities.System
{

    /// <summary>
    /// 
    /// </summary>
    [Table("T_DBConnection")]
    public class T_DBConnectionEntity
    {
        /// <summary>
        /// 
        /// </summary>
        [Key]
        public string DBName { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string Account { get; set; } 

        /// <summary>
        /// 
        /// </summary>
        public string PassWord { get; set; } 
    }
}
