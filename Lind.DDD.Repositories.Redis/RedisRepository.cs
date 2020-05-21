using Lind.DDD.Domain;
using Lind.DDD.IRepositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Lind.DDD.RedisUtils;
using StackExchange.Redis;
using Lind.DDD.Utils;
namespace Lind.DDD.Repositories.Redis
{

    /// <summary>
    /// redis持久化机制
    /// Author:Lind.zhang
    /// 存储结构:Hashset
    /// </summary>
    /// <typeparam name="TEntity"></typeparam>
    public class RedisRepository<TEntity> : IRepository<TEntity> where TEntity : NoSqlEntity
    {
        #region Constructors & Fields
        IDatabase _db;
        string _tableName;
        /// <summary>
        /// redis仓储初始化
        /// </summary>
        public RedisRepository()
            : this(null)
        { }

        /// <summary>
        /// redis仓储初始化
        /// </summary>
        /// <param name="dbName">数据库名，表的前缀</param>
        public RedisRepository(string dbName)
        {
            this._db = RedisManager.Instance.GetDatabase();
            this._tableName = typeof(TEntity).Name;
            if (!string.IsNullOrWhiteSpace(dbName))
                this._tableName = dbName + "_" + this._tableName;
        }
        #endregion

        #region IRepository<TEntity> 成员

        public TEntity Find(params object[] id)
        {
            return SerializeMemoryHelper.DeserializeFromJson<TEntity>(_db.HashGet(_tableName, (string)id[0])) as TEntity;
        }

        public IQueryable<TEntity> GetModel()
        {
            List<TEntity> list = new List<TEntity>();
            var hashVals = _db.HashValues(_tableName).ToArray();
            foreach (var item in hashVals)
            {
                list.Add(SerializeMemoryHelper.DeserializeFromJson<TEntity>(item) as TEntity);
            }
            return list.AsQueryable();
        }

        public void SetDataContext(object db)
        {

            throw new NotImplementedException();
        }

        public void Insert(TEntity item)
        {
            if (item != null)
            {
                _db.HashSet(_tableName, item.Id, SerializeMemoryHelper.SerializeToJson(item));
            }
        }

        public void Update(TEntity item)
        {
            if (item != null)
            {
                var old = Find(item.Id);
                if (old != null)
                {
                    _db.HashDelete(_tableName, item.Id);
                    _db.HashSet(_tableName, item.Id, SerializeMemoryHelper.SerializeToJson(item));

                }
            }
        }

        public void Delete(TEntity item)
        {
            if (item != null)
            {
                _db.HashDelete(_tableName, item.Id);
            }
        }

        #endregion

        #region IUnitOfWorkRepository 成员

        public void UoWInsert(Domain.IEntity item)
        {
            this.Insert(item as TEntity);
        }

        public void UoWUpdate(Domain.IEntity item)
        {
            this.Update(item as TEntity);
        }

        public void UoWDelete(Domain.IEntity item)
        {
            this.Delete(item as TEntity);
        }

        public void UoWInsert(IEnumerable<Domain.IEntity> list)
        {
            foreach (var item in list)
            {
                this.Insert(item as TEntity);
            }
        }

        public void UoWUpdate(IEnumerable<Domain.IEntity> list)
        {
            foreach (var item in list)
            {
                this.Update(item as TEntity);
            }
        }

        public void UoWDelete(IEnumerable<Domain.IEntity> list)
        {
            foreach (var item in list)
            {
                this.Delete(item as TEntity);
            }
        }
        #endregion
    }
}
