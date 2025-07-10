using MES_ReportForms.Utilities;
using System.Data;

namespace MES_ReportForms.Domain.Services
{
    public abstract class ProcessServiceBase : ServiceBase
    {
        protected string ConnectionName { get; set; }
        protected string ConnectionString { get; private set; }

        public ProcessServiceBase(string connectionName = null)
        {
            if (!string.IsNullOrEmpty(connectionName))
            {
                ConnectionName = connectionName;
                ConnectionString = DbConnectionFactory.GetConnectionStringWithCache(connectionName);
            }
        }
          
        /// <summary>
        ///
        /// </summary>
        protected IDbConnection DBCon
        {
            get
            {
                if (!string.IsNullOrEmpty(ConnectionName))
                    return DbConnectionFactory.CreateDbConnection(ConnectionString);
                else
                    return DbConnectionFactory.CreateDefaultDbConnection();
            }
        }
    }
}
