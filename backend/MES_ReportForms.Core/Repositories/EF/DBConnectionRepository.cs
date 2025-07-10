using MES_ReportForms.Core.Entities.System;

namespace MES_ReportForms.Core.Repositories.EF
{
    public class DBConnectionRepository : RepositoryBase<T_DBConnectionEntity>
    {
        public DBConnectionRepository(ApplicationDbContext context) : base(context) { }
    }
}
