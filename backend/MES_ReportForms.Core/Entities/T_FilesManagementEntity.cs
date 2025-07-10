
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MES_ReportForms.Core.Entities.System
{
    /// <summary>
    /// 
    /// </summary>
    [Table("T_FilesManagement")]
    public class T_FilesManagementEntity
    {
        /// <summary>
        /// 
        /// </summary>
        [Key]
        public string FileName { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string FilePath { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string ProcessMessage { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string TaskUser { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public int Status { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public int OrganizeID { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string ReleaseMessage { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public int Priority { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public int Priority2 { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public DateTime UpdateDate { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public DateTime CreateDate { get; set; }
         
    }
}
