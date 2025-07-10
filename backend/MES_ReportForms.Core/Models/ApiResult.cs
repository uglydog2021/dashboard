using System.ComponentModel.DataAnnotations;

namespace MES_ReportForms.Core.Models
{
    /// <summary>
    /// API响应体
    /// </summary>
    public class ApiResult
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
        /// 
        /// </summary>
        public int Total { get; set; }

        /// <summary>
        /// 错误
        /// </summary>
        /// <param name="errorMessage"></param>
        /// <returns></returns>
        public static ApiResult ToError(string errorMessage)
        {
            return new ApiResult
            {
                Code = CODE_ERROR,
                Message = errorMessage, 
                Success = false
            };
        }

        /// <summary>
        /// 成功
        /// </summary>
        /// <returns></returns>
        public static ApiResult Ok(string message = "Success")
        {
            return new ApiResult
            {
                Code = CODE_SUCCESS,
                Message = message,
                Success = true,
            };
        }

    }

    /// <summary>
    /// API响应体
    /// </summary>
    /// <typeparam name="T">返回值类型</typeparam>
    public class ApiResult<T> : ApiResult
    {
        /// <summary>
        /// 返回的数据
        /// </summary>
        public required T Result { get; set; }

        /// <summary>
        /// 响应：成功（隐式转换）
        /// </summary>
        /// <param name="result"></param>
        public static implicit operator ApiResult<T>(T result)
        {
            return new ApiResult<T>
            {
                Code = 0,
                Message = MESSAGE_SUCCESS,
                Success = true,
                Result = result
            };
        }

        /// <summary>
        /// 成功
        /// </summary>
        /// <returns></returns>
        public new static ApiResult<T> Ok(T result)
        {
            return new ApiResult<T>
            {
                Code = CODE_SUCCESS,
                Message = "Success",
                Success = true,
                Result = result
            };
        }

        /// <summary>
        /// 成功
        /// </summary>
        /// <param name="result"></param>
        /// <returns></returns>
        public static ApiResult<T> OkCount(T result, int? total)
        {
            return new ApiResult<T>
            {
                Code = CODE_SUCCESS,
                Message = "Success",
                Success = true,
                Result = result,
                Total = total == null ? 0 : total.Value
            };
        }

        /// <summary>
        /// 错误
        /// </summary>
        /// <param name="errorMessage"></param>
        /// <returns></returns>
        public new static ApiResult<T> ToError(string errorMessage)
        {
            return new ApiResult<T>
            {
                Code = CODE_ERROR,
                Message = errorMessage,
                Success = false,
                Result = default
            };
        }
    }
}
