namespace MES_ReportForms.API
{
    /// <summary>
    /// api版本号
    /// </summary>
    public class ApiVersionInfo
    {
        public record ApiVersionDetail(string Version, string Title, string Description);
         
        /// <summary>
        /// API
        /// </summary>
        public static ApiVersionDetail ReportFormAPI = new ApiVersionDetail("V1",
            "MES_ReportForms Service WebAPI","");
    }
}
