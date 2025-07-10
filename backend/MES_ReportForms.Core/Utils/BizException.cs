namespace MES_ReportForms.Core.Utils
{
    /// <summary>
    /// 业务异常
    /// </summary>
    public class BizException : Exception
    {

        /// <summary>
        /// BizException
        /// </summary>
        public BizException()
        {
        }

        /// <summary>
        /// BizException
        /// </summary>
        /// <param name="message"></param>
        public BizException(string message) : base(message)
        {
            
        }


        /// <summary>
        /// 是否抛给前端(应用程序)界面端显示
        /// </summary>
        public bool IsThrowToFrontend { get; set; } = true;

        /// <summary>
        /// 业务异常码
        /// </summary>
        public string Code { get; set; }
    }
}
