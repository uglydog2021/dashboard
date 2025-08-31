using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MES_ReportForms.Core.Models
{
    public class IndirectWorkAddDTO
    {
        /// <summary>
        /// 不可为空
        /// </summary>
        [Required]
        public string GUID { get; set; }
        /// <summary>
        /// 不可为空
        /// </summary>
        [Required]
        public string User_Name { get; set; }
        /// <summary>
        /// 不可为空
        /// </summary>
        [Required]
        public string Description { get; set; }
        /// <summary>
        /// 开始时间，不可为空
        /// </summary>
        [Required]
        public DateTime Start_Time { get; set; }

        /// <summary>
        /// 结束时间，不可为空
        /// </summary>
        [Required]
        public DateTime End_Time { get; set; }

        /// <summary>
        /// 不可为空
        /// </summary>
        [Required]
        public int Non_OCR_Count { get; set; }

        /// <summary>
        /// 不可为空
        /// </summary>
        [Required]
        public int SPT { get; set; }

        /// <summary>
        /// 不可为空
        /// </summary>
        [Required]
        public int Non_OCR_JJCount { get; set; }

        /// <summary>
        /// 不可为空
        /// </summary>
        [Required]
        public int OCR_Count { get; set; }
        /// <summary>
        /// 不可为空
        /// </summary>
        [Required]
        public int OCR_JJCount { get; set; }
    }

    public class IndirectWorkUpdateDTO
    {
        /// <summary>
        /// 修改数据的ID，不可为空
        /// </summary>
        [Required]
        public int ID { get; set; }
        /// <summary>
        /// 不可为空
        /// </summary>
        [Required]
        public string GUID { get; set; }
        /// <summary>
        /// 不可为空
        /// </summary>
        [Required]
        public string User_Name { get; set; }
        /// <summary>
        /// 不可为空
        /// </summary>
        [Required]
        public string Description { get; set; }
        /// <summary>
        /// 开始时间，不可为空
        /// </summary>
        [Required]
        public DateTime Start_Time { get; set; }

        /// <summary>
        /// 结束时间，不可为空
        /// </summary>
        [Required]
        public DateTime End_Time { get; set; }

        /// <summary>
        /// 不可为空
        /// </summary>
        [Required]
        public int Non_OCR_Count { get; set; }
        /// <summary>
        /// 不可为空
        /// </summary>
        [Required]
        public int SPT { get; set; }
        /// <summary>
        /// 不可为空
        /// </summary>
        [Required]
        public int Non_OCR_JJCount { get; set; }
        /// <summary>
        /// 不可为空
        /// </summary>
        [Required]
        public int OCRCount { get; set; }
        /// <summary>
        /// 不可为空
        /// </summary>
        [Required]
        public int OCRJJCount { get; set; }
    }

    public class IndirectWorkQueryDTO : PageFilter
    {
        public string GUID { get; set; }
        public string User_Name { get; set; }
        public string Description { get; set; }
        public DateTime? Start_Time { get; set; }
        public DateTime? End_Time { get; set; }
        public int? Non_OCR_Count { get; set; }
        public int? Non_OCR_JJCount { get; set; }
    }
}
