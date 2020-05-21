using Lind.DDD.Domain;
using Lind.DDD.IRepositories;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lind.DDD.Repositories.Memory
{
    /// <summary>
    /// 内存数据库仓储
    /// </summary>
    /// <typeparam name="TEntity"></typeparam>
    public class MemoryRepository<TEntity> : IRepository<TEntity> where TEntity : class,IEntity
    {
        /// <summary>
        /// 内存数据库
        /// </summary>
        readonly static ConcurrentDictionary<string, List<TEntity>> db = new ConcurrentDictionary<string, List<TEntity>>();
        List<TEntity> tbl;
        public MemoryRepository()
        {
            var tblName = typeof(TEntity).Name;
            if (!db.ContainsKey(tblName))
            {
                db.TryAdd(tblName, new List<TEntity>());
            }

            db.TryGetValue(tblName, out tbl);
        }
        #region IRepository<TEntity> 成员

        public TEntity Find(params object[] id)
        {
            throw new NotImplementedException();
        }

        public IQueryable<TEntity> GetModel()
        {
            return db[typeof(TEntity).Name].AsQueryable();
        }

        public void SetDataContext(object db)
        {
            throw new NotImplementedException();
        }

        public void Insert(TEntity item)
        {
            tbl.Add(item);
        }

        public void Update(TEntity item)
        {
            tbl.Remove(item);
            tbl.Add(item);
        }

        public void Delete(TEntity item)
        {
            tbl.Remove(item);
        }

        #endregion

        #region IUnitOfWorkRepository 成员

        public void UoWInsert(Domain.IEntity item)
        {
            throw new NotImplementedException();
        }

        public void UoWUpdate(Domain.IEntity item)
        {
            throw new NotImplementedException();
        }

        public void UoWDelete(Domain.IEntity item)
        {
            throw new NotImplementedException();
        }

        public void UoWInsert(IEnumerable<Domain.IEntity> list)
        {
            throw new NotImplementedException();
        }

        public void UoWUpdate(IEnumerable<Domain.IEntity> list)
        {
            throw new NotImplementedException();
        }

        public void UoWDelete(IEnumerable<Domain.IEntity> list)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
