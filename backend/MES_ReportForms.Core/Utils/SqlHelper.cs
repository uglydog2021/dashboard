namespace MES_ReportForms.Utilities
{
    /// <summary>
    /// SQL语句辅助类
    /// </summary>
    public static class SqlHelper
    {
        /// <summary>
        /// SQL分页语句
        /// </summary>
        /// <param name="sqlClause"></param>
        /// <param name="pageNumber"></param>
        /// <param name="pageSize"></param>
        /// <param name="orderColumn"></param>
        /// <returns></returns>
        public static string SqlSplitPage(string sqlClause, int pageNumber, int pageSize, string orderColumn)
        {
            // 确保 pageIndex 是正数
            pageNumber = pageNumber <= 0 ? 1 : pageNumber;

            // 计算 OFFSET 值
            int offset = (pageNumber - 1) * pageSize;

            // 构建基础SQL语句
            string basicSql = sqlClause; // 假设 strSql 已经是一个有效的 SQL 语句

            // 构建最终的分页SQL语句
            string dataSql = pageSize == 0
                ? basicSql
                : $"SELECT * FROM ({basicSql}) AS TB ORDER BY {orderColumn} OFFSET {offset} ROWS FETCH NEXT {pageSize} ROWS ONLY";

            return dataSql;
        }

        public static Type GetType(string fieldType)
        {
            var typeMapping = new Dictionary<string, Type>
            {
                { "int", typeof(int) },
                { "bigint", typeof(long) },
                { "smallint", typeof(short) },
                { "varchar", typeof(string) },
                { "text", typeof(string) },
                { "nvarchar", typeof(string) },
                { "char", typeof(string) },
                { "datetime", typeof(DateTime) },
                { "decimal", typeof(decimal) },
                { "float", typeof(float) },
                { "bit", typeof(bool) },
                { "uniqueidentifier", typeof(Guid) }
            };

            if (typeMapping.TryGetValue(fieldType, out Type dataType))
            {
                return dataType;
            }

            return null;
        }
    }
}
