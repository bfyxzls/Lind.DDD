using Lind.DDD.Paging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Lind.DDD.IRepositories
{
    /// <summary>
    /// MongoDB操作规范
    /// </summary>
    public interface IMongoRepository<TEntity> :
        IExtensionRepository<TEntity>
        where TEntity : class,Lind.DDD.Domain.IEntity
    {
        /// <summary>
        /// 通过MapReduce处理查询，并得到一个实体
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="map">将大批量的工作（数据）分解（MAP）执行</param>
        /// <param name="reduce">然后再将结果合并成最终结果（REDUCE）</param>
        /// <returns></returns>
        TEntity MapReduce(string map, string reduce);

        /// <summary>
        /// Mongo大数据情况下，有分页时使用这个方法
        /// </summary>
        /// <typeparam name="U"></typeparam>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        PagedResult<TEntity> GetModel<U>(int pageIndex, int pageSize);

        /// <summary>
        /// MongoDB集成的查询方法，大数据情况下，有分页时使用这个方法
        /// </summary>
        /// <typeparam name="U">匿名对象，用来为条件赋值</typeparam>
        /// <param name="template">条件对象</param>
        /// <param name="limit"></param>
        /// <param name="skip"></param>
        /// <returns></returns>
        PagedResult<TEntity> GetModel<U>(U template, int pageIndex, int pageSize);

        /// <summary>
        /// MongoDB集成的查询方法，大数据情况下，有分页和排序时使用这个方法
        /// </summary>
        /// <typeparam name="U">条件对象，用来为条件赋值</typeparam>
        /// <typeparam name="O">排序对象，IRepository.Core.OrderType类型，OrderType.Asc表示升序，OrderType.Desc表示降序</typeparam>
        /// <param name="template">条件对象</param>
        /// <param name="orderby">排序对象</param>
        /// <param name="limit"></param>
        /// <param name="skip"></param>
        /// <returns></returns>
        PagedResult<TEntity> GetModel<U, O>(U template, O orderby, int pageIndex, int pageSize);

        /// <summary>
        /// 官方驱动，返回带分页的结果集
        /// </summary>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        PagedResult<TEntity> GetModel(int pageIndex, int pageSize);

        /// <summary>
        /// 官方驱动，返回带条件和分页的结果集
        /// </summary>
        /// <param name="expression"></param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        PagedResult<TEntity> GetModel(Expression<Func<TEntity, bool>> expression, int pageIndex, int pageSize);

        /// <summary>
        /// 官方驱动，返回带排序和分页的结果集
        /// </summary>
        /// <param name="fields"></param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        PagedResult<TEntity> GetModel(Dictionary<Expression<Func<TEntity, object>>, bool> fields, int pageIndex, int pageSize);
        /// <summary>
        /// 官方驱动，返回带条件和排序及分页的结果集
        /// </summary>
        /// <param name="expression"></param>
        /// <param name="fields"></param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        PagedResult<TEntity> GetModel(Expression<Func<TEntity, bool>> expression, Dictionary<Expression<Func<TEntity, object>>, bool> fields, int pageIndex, int pageSize);
        /// <summary>
        /// 集合数量
        /// </summary>
        /// <param name="expression"></param>
        /// <returns></returns>
        long Count(Expression<Func<TEntity, bool>> expression);
        /// <summary>
        /// 列表按需更新
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="entity"></param>
        void Update<T>(IEnumerable<Expression<Action<T>>> list) where T : class;
        /// <summary>
        /// 按需更新
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="entity"></param>
        void Update<T>(System.Linq.Expressions.Expression<Action<T>> entity) where T : class;
        /// <summary>
        /// 通过mongo条件得到结果集
        /// </summary>
        /// <typeparam name="U"></typeparam>
        /// <param name="template"></param>
        /// <returns></returns>
        IEnumerable<TEntity> GetModel<U>(U template);
    }
}
