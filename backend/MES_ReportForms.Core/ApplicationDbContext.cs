using System.Reflection;
using Microsoft.EntityFrameworkCore;
using MES_ReportForms.Core.Entities;
using MES_ReportForms.Core.Entities.System;
using System.Linq.Expressions;

namespace MES_ReportForms.Core
{
    public class ApplicationDbContext : DbContext
    {
        public required DbSet<T_UserMasterEntity> UserMasterEntity { get; set; }

        public required DbSet<T_DBConnectionEntity> DBConnectionEntity { get; set; }

        public required DbSet<T_FilesManagementEntity> FilesManagementEntity { get; set; }
          
        public required DbSet<T_AttentionEntity> T_AttentionEntity { get; set; }

        public required DbSet<T_OrganizationEntity> T_OrganizationEntity { get; set; }

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            //.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());

            // 为所有查询全局加入IsDelete过滤条件
            // 为所有继承自BaseEntity的实体加上该条件
            foreach (var entityType in modelBuilder.Model.GetEntityTypes()
               .Where(e => typeof(BaseEntity).IsAssignableFrom(e.ClrType)))
            {
                modelBuilder.Entity(entityType.ClrType).Property<Boolean>(nameof(BaseEntity.IsDelete));
                var parameter = Expression.Parameter(entityType.ClrType, "e");
                var body = Expression.Equal(
                    Expression.Call(typeof(EF), nameof(EF.Property), new[] { typeof(bool) }, parameter, Expression.Constant(nameof(BaseEntity.IsDelete))),
                    Expression.Constant(false));
                modelBuilder.Entity(entityType.ClrType).HasQueryFilter(Expression.Lambda(body, parameter));
            }

            base.OnModelCreating(modelBuilder);
        }
    }
}