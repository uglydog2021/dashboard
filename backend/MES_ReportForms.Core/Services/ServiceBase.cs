using MES_ReportForms.Utilities;
using System.Data;

namespace MES_ReportForms.Domain.Services
{
    public abstract class ServiceBase
    {
        /// <summary>
        /// Main 主数据库
        /// </summary>
        protected IDbConnection MDB
        {
            get
            {
                return DbConnectionFactory.CreateDefaultDbConnection();
            }
        }
    }
}
