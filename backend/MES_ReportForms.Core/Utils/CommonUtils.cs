using System.Security.Cryptography;
using System.Text;

namespace MES_ReportForms.Core
{
    public class CommonUtils
    {
        /// <summary>
        /// 随机生成12位数字混合大小写字母，随机加入特殊字符!@#$%^&*，并排除iIl1o0O
        /// </summary>
        /// <returns></returns>
        public static string GenerateRandomString()
        {
            string allowedChars = "abcdefghjkmnpqrstuvwxyzABCDEFGHJKLMNPQRSTUVWXYZ23456789!@#$%^&*";
            int length = 12;
            var stringBuilder = new StringBuilder(length);

            for (int i = 0; i < length; i++)
            {
                int index = RandomNumberGenerator.GetInt32(allowedChars.Length);
                char randomChar = allowedChars[index];
                stringBuilder.Append(randomChar);
            }

            return stringBuilder.ToString();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="password"></param>
        /// <param name="salt"></param>
        /// <returns></returns>
        public static string GenerateSaltedHash(string password, string salt)
        {
            // 将密码和盐合并
            string combined = password + salt;

            
            // 创建SHA256哈希算法实例
            using (var sha256 = SHA256.Create())
            {
                // 对合并后的字符串进行编码
                byte[] combinedBytes = Encoding.UTF8.GetBytes(combined);

                // 计算哈希
                byte[] hashBytes = sha256.ComputeHash(combinedBytes);

                // 将哈希字节转换为十六进制字符串
                StringBuilder hashBuilder = new StringBuilder();
                foreach (byte b in hashBytes)
                {
                    hashBuilder.Append(b.ToString("x2"));
                }

                // 返回哈希值
                return hashBuilder.ToString();
            }
        }

        /// <summary>
        /// 动态获取列的名称集合
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static List<string> GetColumnNames(Type type)
        {
            // 获取类型的所有属性
            var properties = type.GetProperties();

            // 提取属性名称并返回
            return properties.Select(p => p.Name).ToList();
        }


        /// <summary>
        /// 获取对应条件的日期
        /// </summary>
        /// <param name="timeRange"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
        public static (DateTime, DateTime) GetDateRange(string timeRange)
        {
            DateTime now = DateTime.Now;
            DateTime startDate = DateTime.MinValue;
            DateTime endDate = DateTime.MinValue;

            switch (timeRange)
            {
                case "Day":
                    // 返回当天的起始和结束日期（只精确到年月日）
                    startDate = now.Date; // 当天的00:00:00
                    endDate = now.Date;   // 当天的日期（只需要到日期）
                    break;

                case "Week":
                    // 返回本周的起始和结束日期（只精确到年月日）
                    int diff = now.DayOfWeek - DayOfWeek.Monday;
                    startDate = now.AddDays(-diff).Date; // 本周一的日期
                    endDate = startDate.AddDays(6); // 本周日的日期
                    break;

                case "Month":
                    // 返回本月的第一天和最后一天（只精确到年月日）
                    startDate = new DateTime(now.Year, now.Month, 1); // 本月第一天
                    endDate = startDate.AddMonths(1).AddDays(-1); // 本月最后一天
                    break;

                default:
                    throw new ArgumentException("Invalid time range specified.");
            }

            return (startDate, endDate);
        }
    }
}
