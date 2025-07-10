using System.ComponentModel.DataAnnotations;

namespace MES_ReportForms.Core
{
    /// <summary>
    /// 分页查询响应
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class PageData<T>
    {

        /// <summary>
        /// 响应代码：成功
        /// </summary>
        public const int CODE_SUCCESS = 0;
        /// <summary>
        /// 响应代码：错误
        /// </summary>
        public const int CODE_ERROR = -1;
        /// <summary>
        /// 响应代码：没有登录
        /// </summary>
        public const int CODE_NO_AUTH = -100;
        /// <summary>
        /// 响应代码：没有权限
        /// </summary>
        public const int CODE_NO_PERMISSION = -101;

        /// <summary>
        /// 响应消息：成功
        /// </summary>
        public const string MESSAGE_SUCCESS = "Success";

        /// <summary>
        /// 响应消息：错误
        /// </summary>
        public const string MESSAGE_ERROR = "Error";

        /// <summary>
        /// 响应代码（0:成功; -1:失败; -100:没有登录; -101;没有权限)
        /// </summary>
        [Required]
        public int Code { get; set; }

        /// <summary>
        /// 响应消息
        /// </summary>
        public string Message { get; set; }

        /// <summary>
        /// 是否成功
        /// </summary>
        [Required]
        public bool Success { get; set; }
        /// <summary>
        /// 数据列表
        /// </summary>
        public IEnumerable<T> Result { get; set; }

        /// <summary>
        /// 总记录数
        /// </summary>
        public int Total { get; set; }

        /// <summary>
        /// 总页数
        /// </summary>
        //public int TotalPages
        //{
        //    get
        //    {
        //        if (Total == 0) return 0;

        //        if (PageSize == 0) return 1;

        //        return (int)Math.Ceiling(Total / (double)PageSize);
        //    }
        //}

        /// <summary>
        /// 构建一个对象
        /// </summary>
        /// <param name="filter"></param>
        /// <param name="items"></param>
        /// <param name="totalCount"></param>
        /// <returns></returns>
        public static PageData<TModel> Build<TModel>(PageFilter filter, IEnumerable<TModel> items, int totalCount)
        {
            return new PageData<TModel>()
            {
                Code = 0,
                Message = MESSAGE_SUCCESS,
                Success = true,
                Result = items,
                Total = totalCount
            };
        }
    }
}
