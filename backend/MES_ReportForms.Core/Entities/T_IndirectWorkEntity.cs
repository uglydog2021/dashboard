
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MES_ReportForms.Core.Entities.System
{
    /// <summary>
    /// 
    /// </summary>
    [Table("T_IndirectWork")]
    public class T_IndirectWorkEntity
    {
        [Key]
        public int ID { get; set; }
        public string GUID { get; set; }
        public string User_Name { get; set; }
        public string Description { get; set; }
        public DateTime Start_Time { get; set; }
        public DateTime End_Time { get; set; }
        public int Non_OCR_Count { get; set; }

        public int SPT { get; set; }

        public int Non_OCR_JJCount { get; set; }
        public int OCR_Count { get; set; }
        public int OCR_JJCount { get; set; }
        public string CreatorGUID { get; set; }
        public string Creator { get; set; }
        public DateTime Create_Time { get; set; } = DateTime.Now;
        public DateTime? Update_Time { get; set; }

        // Duration as a calculated field in the database (in hours)
        [NotMapped]
        public decimal Duration { get; set; }
    }
}
