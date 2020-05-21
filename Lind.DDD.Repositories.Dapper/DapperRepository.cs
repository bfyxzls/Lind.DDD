using Lind.DDD.Domain;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dapper;
using Dapper.Contrib.Extensions;
namespace Lind.DDD.Repositories.Dapper
{
    /// <summary>
    /// 使用Dapper实现的仓储
    /// </summary>
    /// <typeparam name="TEntity"></typeparam>
    public class DapperRepository<TEntity> : IRepositories.IRepository<TEntity> where TEntity : Entity
    {
        IDbConnection conn;
        string connString;
        string tableName;
        public DapperRepository(string connString)
        {
            this.connString = connString;
            conn = new SqlConnection(connString);
            tableName = typeof(TEntity).Name;
           
        }

        #region IRepository<TEntity> 成员

        public TEntity Find(params object[] id)
        {
            return conn.Get<TEntity>(id);
        }

        public IQueryable<TEntity> GetModel()
        {
            return conn.GetAll<TEntity>().AsQueryable();
        }

        public void SetDataContext(object db)
        {
            throw new NotImplementedException();
        }

        public void Insert(TEntity item)
        {
            conn.Insert(item);
        }

        public void Update(TEntity item)
        {
            conn.Update(item);
        }

        public void Delete(TEntity item)
        {
            conn.Delete(item);
        }

        #endregion

        #region IUnitOfWorkRepository 成员

        public void UoWInsert(Domain.IEntity item)
        {
            conn.Insert(item);
        }

        public void UoWUpdate(Domain.IEntity item)
        {
            conn.Update(item);
        }

        public void UoWDelete(Domain.IEntity item)
        {
            conn.Delete(item);
        }

        public void UoWInsert(IEnumerable<Domain.IEntity> list)
        {
            conn.Insert(list);
        }

        public void UoWUpdate(IEnumerable<Domain.IEntity> list)
        {
            conn.Update(list);
        }

        public void UoWDelete(IEnumerable<Domain.IEntity> list)
        {
            conn.Delete(list);
        }

        #endregion
    }
}
