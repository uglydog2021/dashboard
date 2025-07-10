using MES_ReportForms.Core.Entities.System;
using Microsoft.EntityFrameworkCore;
using System;
using System.Text.RegularExpressions;

namespace MES_ReportForms.Core.Repositories.EF
{
    /// <summary>
    /// 
    /// </summary>
    public class UserMasterRepository : RepositoryBase<T_UserMasterEntity>
    {
        private readonly ApplicationDbContext _dbContext;
        public UserMasterRepository(ApplicationDbContext context) : base(context) {

            _dbContext = context;
        }
         
    }
}
