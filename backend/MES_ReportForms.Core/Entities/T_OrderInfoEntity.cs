
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MES_ReportForms.Core.Entities.System
{
    /// <summary>
    /// 
    /// </summary>

    [Table("T_OrderInfo")]
    public class T_OrderInfoEntity
    {
        /// <summary>
        /// 
        /// </summary>
        [Key]
        public int Id { get; set; } 

        /// <summary>
        /// 
        /// </summary>
        public string FileName { get; set; }
         
        /// <summary>
        /// 
        /// </summary>
        public string LastUpdateUser { get; set; }
         
        /// <summary>
        /// 
        /// </summary>
        public int LastUpdateFlag { get; set; }
         
        /// <summary>
        /// 
        /// </summary>
        public DateTime? CreateTime { get; set; }
         
        /// <summary>
        /// 
        /// </summary>
        public DateTime? UpdateTime { get; set; }
         
        /// <summary>
        /// 
        /// </summary>
        public string FileContent { get; set; } 
    }
}
