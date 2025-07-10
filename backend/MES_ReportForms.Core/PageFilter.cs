namespace MES_ReportForms.Core
{
    /// <summary>
    /// 按页查询
    /// </summary>
    public class PageFilter
    {
        /// <summary>
        /// 每页记录数(默认20)
        /// </summary>
        public int PageSize { get; set; } = 20;

        /// <summary>
        /// 当前页码(默认1)
        /// </summary>
        public int PageNumber { get; set; } = 1;

        /// <summary>
        /// 排序字段
        /// </summary>
        public virtual string OrderBy { get; set; }

        /// <summary>
        /// 排序方式，选项为：ASC、DESC，分别为升序、降序
        /// </summary>
        public virtual string Order { get; set; }

        /// <summary>
        /// 排序方式是否为升序
        /// </summary>
        /// <returns></returns>
        public bool IsASC()
        {
            return string.Equals(Order, "ASC", StringComparison.OrdinalIgnoreCase);
        }
    }
}
