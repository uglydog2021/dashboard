using Dapper;
using MES_ReportForms.Core.Entities.System;
using MES_ReportForms.Core.Models.System;
using MES_ReportForms.Core.Models;
using MES_ReportForms.Utilities;
using System.Data;
using System.Drawing;
using System.Text;
using MES_ReportForms.Core.Repositories.Sql;

namespace MES_ReportForms.Core.Repositories.EF
{
    public class OrganizationRepository : RepositoryBase<T_OrganizationEntity>
    {
        public OrganizationRepository(ApplicationDbContext context) : base(context) { } 
    }
     
}
