
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MES_ReportForms.Core.Entities.System
{
    /// <summary>
    /// 
    /// </summary>

    [Table("T_ActionHistory")]
    public class T_ActionHistoryEntity
    {
        /// <summary>
        /// 
        /// </summary>
        [Key]
        public int Id { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public int Status { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string FileName { get; set; } 

        /// <summary>
        /// 
        /// </summary>
        public string TaskUser { get; set; } 

        /// <summary>
        /// 
        /// </summary>
        public string HistoryFilePath { get; set; } 

        /// <summary>
        /// 
        /// </summary>
        public DateTime ActionStartDate { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public DateTime ActionEndDate { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public DateTime CreateDate { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public int WorkTimeH { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public int WorkTimeM { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public int WorkTimeS { get; set; }
         
    }
}
