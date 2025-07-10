namespace MES_ReportForms.Core.Entities
{
    /// <summary>
    /// 数据库表公共字段
    /// </summary>
    public class BaseEntity
    {
        /// <summary>
        /// 是否删除
        /// </summary>
        public bool IsDelete { get; set; }

        /// <summary>
        /// 创建用户ID
        /// </summary>
        public string CreateUser { get; set; }

        /// <summary>
        /// 创建日期
        /// </summary>
        public DateTime CreateDate { get; set; }

        /// <summary>
        /// 修改用户ID
        /// </summary>
        public string ModifierUser { get; set; }

        /// <summary>
        /// 修改日期
        /// </summary>
        public DateTime? ModifierDate { get; set; }
    }
}
