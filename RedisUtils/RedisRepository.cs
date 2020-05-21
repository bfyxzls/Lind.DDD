using System;
using System.Collections.Generic;
using System.Linq;
using StackExchange.Redis;
namespace RedisUtils
{

    /// <summary>
    /// redis持久化机制
    /// Author:Lind.zhang
    /// 存储结构:Hashset
    /// </summary>
    /// <typeparam name="TEntity"></typeparam>
    public class RedisRepository<TEntity>
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
            return SerializeMemoryHelper.DeserializeFromJson<TEntity>(_db.HashGet(_tableName, (string)id[0]));
        }

        public IQueryable<TEntity> GetModel()
        {
            List<TEntity> list = new List<TEntity>();
            var hashVals = _db.HashValues(_tableName).ToArray();
            foreach (var item in hashVals)
            {
                list.Add(SerializeMemoryHelper.DeserializeFromJson<TEntity>(item));
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
                _db.HashSet(_tableName, GetId(item), SerializeMemoryHelper.SerializeToJson(item));
            }
        }

        public void Update(TEntity item)
        {
            if (item != null)
            {
                var old = Find(GetId(item));
                if (old != null)
                {
                    _db.HashDelete(_tableName, GetId(item));
                    _db.HashSet(_tableName, GetId(item), SerializeMemoryHelper.SerializeToJson(item));

                }
            }
        }

        public void Delete(TEntity item)
        {
            if (item != null)
            {
                _db.HashDelete(_tableName, GetId(item));
            }
        }

        private string GetId(TEntity entity)
        {
            return (string)typeof(TEntity).GetProperty("Id").GetValue(entity, null);
        }
        #endregion

    }
}
