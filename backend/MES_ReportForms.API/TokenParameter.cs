namespace MES_ReportForms.API
{
    public class TokenParameter
    {
        /// <summary>
        /// 颁发者
        /// </summary>
        public static string Issuer;
        /// <summary>
        /// 接收者
        /// </summary>
        public static string Audience;
        /// <summary>
        /// 签名秘钥
        /// </summary>
        public static string Secret;
        /// <summary>
        /// AccessToken过期时间（分钟）
        /// </summary>
        public static int AccessExpiration;
        /// <summary>
        /// 密码Salt
        /// </summary>
        public static string PasswordSalt;

        static TokenParameter()
        {
            IConfigurationRoot settings = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build();

            var tokenParameterSelection = settings.GetSection("TokenParameter");

            Issuer = tokenParameterSelection.GetValue<string>("Issuer");
            Audience = tokenParameterSelection.GetValue<string>("Audience");
            Secret = tokenParameterSelection.GetValue<string>("Secret");
            AccessExpiration = tokenParameterSelection.GetValue<int>("AccessExpiration");
            PasswordSalt = tokenParameterSelection.GetValue<string>("PasswordSalt");
        }
    }
}