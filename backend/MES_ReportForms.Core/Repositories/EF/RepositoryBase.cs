using Microsoft.EntityFrameworkCore;
using System.Data.Common;
using System.Data;
using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using System.IdentityModel.Tokens.Jwt;
using System.Reflection;
using System.Collections.Generic;

namespace MES_ReportForms.Core.Repositories.EF
{
    /// <summary>
    /// 数据存储库基类
    /// </summary>
    /// <typeparam name="TEntity"></typeparam>
    public class RepositoryBase<TEntity> where TEntity : class
    {
        private readonly ApplicationDbContext _currentContext;

        /// <summary>
        /// 当前数据表实体的数据集
        /// </summary>
        public readonly DbSet<TEntity> _dbSet;

        /// <summary>
        /// 当前数据库EF上下文
        /// </summary>
        protected ApplicationDbContext DbContext => _currentContext;

        /// <summary>
        /// 数据存储库基类
        /// </summary>
        /// <param name="context"></param>
        public RepositoryBase(ApplicationDbContext context)
        {
            _currentContext = context;
            _dbSet = context.Set<TEntity>();
        }

        public IEnumerable<TEntity> Query(Func<TEntity, bool> predicate)
        {
            return _dbSet.Where(predicate);
        }

        public IQueryable<TEntity> AsQueryable()
        {
            return _dbSet.AsQueryable();
        }
         
        public async Task<int> SaveChangesAsync()
        {
            return await _currentContext.SaveChangesAsync();
        }
         
        #region 插入数据
         
        public async Task<bool> InsertAsync(TEntity entity, bool isSaveChange = true)
        {
            _dbSet.Add(entity);
            if (isSaveChange)
            {
                return await SaveChangesAsync() > 0;
            }
            return false;
        }
         
        public virtual async Task<bool> DeleteAsync(TEntity entity, bool isSaveChange = true)
        { 
            _dbSet.Update(entity);

            return isSaveChange ? await SaveChangesAsync() > 0 : false;
        }

        public virtual async Task<bool> UpdateAsync(TEntity entity ,bool isSaveChange = true)
        { 
            _currentContext.Update(entity);

            return isSaveChange ? await SaveChangesAsync() > 0 : false;
        }  
        #endregion

        #region 删除  

        public virtual async Task<bool> RemoveAsync(TEntity entity , bool isSaveChange = true)
        {
            _dbSet.Remove(entity);

            return isSaveChange ? await SaveChangesAsync() > 0 : false;
        }
         
        public virtual async Task<bool> RemoveAsync(List<TEntity> entitys , bool isSaveChange = true)
        {
            _dbSet.RemoveRange(entitys);

            return isSaveChange ? await SaveChangesAsync() > 0 : false;
        }
        #endregion 

        #region 查找

        public async Task<bool> AnyAsync(Expression<Func<TEntity, bool>> predicate)
        {
            if (predicate == null)
            {
                predicate = c => false;
            }
            return await _dbSet.AnyAsync(predicate);
        }

        public long Count(Expression<Func<TEntity, bool>> predicate = null)
        {
            if (predicate == null)
            {
                predicate = c => true;
            }
            return _dbSet.LongCount(predicate);
        }

        public async Task<long> CountAsync(Expression<Func<TEntity, bool>> predicate = null)
        {
            if (predicate == null)
            {
                predicate = c => true;
            }
            return await _dbSet.LongCountAsync(predicate);
        }

        public TEntity Get(params object[] id)
        {
            if (id == null)
            {
                return default;
            }
            return _dbSet.Find(id);
        }

        public TEntity Get(Expression<Func<TEntity, bool>> predicate = null, bool isNoTracking = true)
        {
            var data = isNoTracking ? _dbSet.Where(predicate).AsNoTracking() : _dbSet.Where(predicate);

            return data.FirstOrDefault();
        }

        public async Task<TEntity> GetAsync(params object[] id)
        {
            if (id == null)
            {
                return default;
            }
            return await _dbSet.FindAsync(id);
        }

        public async Task<TEntity> GetAsync(Expression<Func<TEntity, bool>> predicate = null, bool isNoTracking = true)
        {
            var data = isNoTracking ? _dbSet.Where(predicate).AsNoTracking() : _dbSet.Where(predicate);

            return await data.FirstOrDefaultAsync();
        }

        public async Task<List<TEntity>> GetListAsync(Expression<Func<TEntity, bool>> predicate = null, string ordering = "", bool isNoTracking = true)
        {
            var data = isNoTracking ? _dbSet.Where(predicate).AsNoTracking() : _dbSet.Where(predicate);
            if (!string.IsNullOrEmpty(ordering))
            {
                data = data.OrderByDynamic(ordering);
            }
            return await data.ToListAsync();
        }

        public List<TEntity> GetList(Expression<Func<TEntity, bool>> predicate = null, string ordering = "", bool isNoTracking = true)
        {
            var data = isNoTracking ? _dbSet.Where(predicate).AsNoTracking() : _dbSet.Where(predicate);
            if (!string.IsNullOrEmpty(ordering))
            {
                data = data.OrderByDynamic(ordering);
            }
            return data.ToList();
        }
        public async Task<IQueryable<TEntity>> LoadAsync(Expression<Func<TEntity, bool>> predicate = null, bool isNoTracking = true)
        {
            if (predicate == null)
            {
                predicate = c => true;
            }
            return await Task.Run(() => isNoTracking ? _dbSet.Where(predicate).AsNoTracking() : _dbSet.Where(predicate));
        }
        public IQueryable<TEntity> Load(Expression<Func<TEntity, bool>> predicate = null, bool isNoTracking = true)
        {
            if (predicate == null)
            {
                predicate = c => true;
            }
            return isNoTracking ? _dbSet.Where(predicate).AsNoTracking() : _dbSet.Where(predicate);
        }
         
        #endregion

        #region SQL语句
        public virtual void BulkInsert<T>(List<T> entities)
        { }
        public int ExecuteSql(string sql)
        {
            return _currentContext.Database.ExecuteSqlRaw(sql);
        }

        public Task<int> ExecuteSqlAsync(string sql)
        {
            return _currentContext.Database.ExecuteSqlRawAsync(sql);
        }

        public int ExecuteSql(string sql, List<DbParameter> spList)
        {
            return _currentContext.Database.ExecuteSqlRaw(sql, spList.ToArray());
        }

        public Task<int> ExecuteSqlAsync(string sql, List<DbParameter> spList)
        {
            return _currentContext.Database.ExecuteSqlRawAsync(sql, spList.ToArray());
        }

        public virtual DataTable GetDataTableWithSql(string sql)
        {
            throw new NotImplementedException();
        }

        public virtual DataTable GetDataTableWithSql(string sql, List<DbParameter> spList)
        {
            throw new NotImplementedException();
        }

        #endregion

    }
}
