using Lind.DDD.IRepositories;
using Lind.DDD.IRepositories.Commons;
using Lind.DDD.Paging;
using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Lind.DDD.Repositories.Mongo
{
    /// <summary>
    /// Mongodb实现数据持久化
    /// Author:Lind.zhang
    /// </summary>
    /// <typeparam name="TEntity"></typeparam>
    public class MongoRepository<TEntity> : IMongoRepository<TEntity>
      where TEntity : Lind.DDD.Domain.NoSqlEntity
    {

        #region Fields & consts
        /// <summary>
        /// 创建数据库链接
        /// </summary>
        private MongoClient _server;
        /// <summary>
        /// 获得数据库
        /// </summary>
        private IMongoDatabase _database;
        /// <summary>
        /// 操作的集合（数据表）
        /// </summary>
        private IMongoCollection<TEntity> _table;
        /// <summary>
        /// 实体键，对应MongoDB的_id
        /// </summary>
        private const string EntityKey = "Id";

        /// <summary>
        /// 服务器地址和端口
        /// </summary>
        private static readonly string _connectionStringHost = ConfigConstants.ConfigManager.Config.MongoDB.Host;
        /// <summary>
        /// 数据库名称
        /// </summary>
        private static readonly string _dbName = ConfigConstants.ConfigManager.Config.MongoDB.DbName;
        /// <summary>
        /// 用户名
        /// </summary>
        private static readonly string _userName = ConfigConstants.ConfigManager.Config.MongoDB.UserName;
        /// <summary>
        /// 密码
        /// </summary>
        private static readonly string _password = ConfigConstants.ConfigManager.Config.MongoDB.Password;

        #endregion

        #region Constructors
        public MongoRepository()
        {
            _server = new MongoClient(ConnectionString());
            _database = _server.GetDatabase(_dbName);
            _table = _database.GetCollection<TEntity>(typeof(TEntity).Name);
        }

        #endregion

        #region Private Methods
        /// <summary>
        /// 组织Mongo连接串
        /// </summary>
        /// <returns></returns>
        private static string ConnectionString()
        {
            var database = _dbName;
            var userName = _userName;
            var password = _password;
            var authentication = string.Empty;
            var host = string.Empty;
            if (!string.IsNullOrWhiteSpace(userName))
            {
                authentication = string.Concat(userName, ':', password, '@');
            }
            database = database ?? "Test";
            if (string.IsNullOrWhiteSpace(_connectionStringHost))
            {
                throw new ArgumentNullException("请配置MongoDB_Host节点");
            }
            //mongodb://[username:password@]host1[:port1][,host2[:port2],…[,hostN[:portN]]][/[database][?options]]
            return string.Format("mongodb://{0}{1}/{2}", authentication, _connectionStringHost, database);
        }

        /// <summary>
        /// 构建Mongo的查询表达式，通过一个匿名对象
        /// 最多支持三层嵌套
        /// </summary>
        /// <typeparam name="U"></typeparam>
        /// <param name="template"></param>
        /// <returns></returns>
        private BsonDocumentFilterDefinition<TEntity> GeneratorMongoQuery<U>(U template)
        {
            var qType = typeof(U);
            var outter = new BsonDocument();
            var simpleQuery = new BsonDocument();
            foreach (var item in qType.GetProperties(BindingFlags.Instance | BindingFlags.Public))
            {
                if (item.PropertyType.IsClass && item.PropertyType != typeof(string))
                {
                    //复杂类型，导航属性，类对象和集合对象 
                    foreach (var sub in item.PropertyType.GetProperties(BindingFlags.Instance | BindingFlags.Public))
                    {

                        if (sub.PropertyType.IsClass && sub.PropertyType != typeof(string))
                        {
                            foreach (var subInner in sub.PropertyType.GetProperties(BindingFlags.Instance | BindingFlags.Public))
                            {
                                if (subInner.PropertyType.IsClass && subInner.PropertyType != typeof(string))
                                {
                                    foreach (var subItemInner in subInner.PropertyType.GetProperties(BindingFlags.Instance | BindingFlags.Public))
                                    {
                                        simpleQuery.Add(new BsonElement(item.Name + "." + sub.Name + "." + subInner.Name + "." + subItemInner.Name,
                                           BsonValue.Create(subItemInner.GetValue(subInner.GetValue(sub.GetValue(item.GetValue(template)))))));
                                    }

                                }
                                else
                                {
                                    simpleQuery.Add(new BsonElement(item.Name + "." + sub.Name + "." + subInner.Name,
                                        BsonValue.Create(subInner.GetValue(sub.GetValue(item.GetValue(template))))));
                                }
                            }
                        }
                        else
                        {
                            simpleQuery.Add(new BsonElement(item.Name + "." + sub.Name,
                               BsonValue.Create(sub.GetValue(item.GetValue(template)))
                           ));

                        }


                    }
                }
                else
                {
                    //简单类型,ValueType和string
                    simpleQuery.Add(new BsonElement(item.Name,
                      BsonValue.Create(item.GetValue(template))
                        ));
                }
            }
            return new BsonDocumentFilterDefinition<TEntity>(simpleQuery);
        }

        /// <summary>
        /// 递归构建Update操作串
        /// 注意：如果子属性为集合，只更新原集，不能添加push和删除pull集合元素
        /// </summary>
        /// <param name="fieldList">要更新的字段列表</param>
        /// <param name="property">当前属性</param>
        /// <param name="propertyValue">当前属性值</param>
        /// <param name="item">原对象</param>
        /// <param name="father">父属性</param>
        private void GenerateRecursion(
                 List<UpdateDefinition<TEntity>> fieldList,
                 PropertyInfo property,
                 object propertyValue,
                 TEntity item,
                 string father)
        {
            //复杂类型
            if (property.PropertyType.IsClass && property.PropertyType != typeof(string) && propertyValue != null)
            {
                //集合
                if (typeof(IList).IsAssignableFrom(propertyValue.GetType()))
                {
                    foreach (var sub in property.PropertyType.GetProperties(BindingFlags.Instance | BindingFlags.Public))
                    {
                        if (sub.PropertyType.IsClass && sub.PropertyType != typeof(string))
                        {
                            var arr = propertyValue as IList;
                            if (arr != null && arr.Count > 0)
                            {
                                for (int index = 0; index < arr.Count; index++)
                                {
                                    foreach (var subInner in sub.PropertyType.GetProperties(BindingFlags.Instance | BindingFlags.Public))
                                    {
                                        if (string.IsNullOrWhiteSpace(father))
                                            GenerateRecursion(fieldList, subInner, subInner.GetValue(arr[index]), item, property.Name + "." + index);
                                        else
                                            GenerateRecursion(fieldList, subInner, subInner.GetValue(arr[index]), item, father + "." + property.Name + "." + index);
                                    }
                                }
                            }
                        }
                    }
                }
                //实体
                else
                {
                    foreach (var sub in property.PropertyType.GetProperties(BindingFlags.Instance | BindingFlags.Public))
                    {
                        if (string.IsNullOrWhiteSpace(father))
                            GenerateRecursion(fieldList, sub, sub.GetValue(propertyValue), item, property.Name);
                        else
                            GenerateRecursion(fieldList, sub, sub.GetValue(propertyValue), item, father + "." + property.Name);
                    }
                }
            }
            //简单类型
            else
            {
                if (property.Name != EntityKey)//更新集中不能有实体键_id
                {
                    if (string.IsNullOrWhiteSpace(father))
                        fieldList.Add(Builders<TEntity>.Update.Set(property.Name, propertyValue));
                    else
                        fieldList.Add(Builders<TEntity>.Update.Set(father + "." + property.Name, propertyValue));

                }
            }
        }

        /// <summary>
        /// 版本二：递归构建Update操作串
        /// 主要功能：实现List子属性的push操作
        ///           更新时，添加了unset动作，将需要更新的元素先移除，再更新
        /// </summary>
        /// <param name="fieldList"></param>
        /// <param name="property"></param>
        /// <param name="propertyValue"></param>
        /// <param name="item"></param>
        /// <param name="father"></param>
        private void GenerateRecursionSet(
                  List<UpdateDefinition<TEntity>> fieldList,
                  PropertyInfo property,
                  object propertyValue,
                  TEntity item,
                  string father,
                  FilterDefinition<TEntity> filter
           )
        {
            //复杂类型
            if (property.PropertyType.IsClass && property.PropertyType != typeof(string) && propertyValue != null)
            {
                //对于复杂类型的更新：移除属性，避免为赋值为NULL类型复杂字段无法set的问题
                _table.UpdateOne(filter, Builders<TEntity>.Update.Unset(string.IsNullOrWhiteSpace(father) ? property.Name : father + "." + property.Name));

                //集合
                if (typeof(IList).IsAssignableFrom(propertyValue.GetType()))
                {
                    var arr = propertyValue as IList;
                    if (arr != null && arr.Count > 0)
                    {
                        fieldList.Add(Builders<TEntity>.Update.Set(string.IsNullOrWhiteSpace(father) ? property.Name : father + "." + property.Name, arr));
                    }
                }
                //实体
                else
                {
                    foreach (var sub in property.PropertyType.GetProperties(BindingFlags.Instance | BindingFlags.Public))
                    {
                        GenerateRecursionSet(fieldList, sub, sub.GetValue(propertyValue), item, string.IsNullOrWhiteSpace(father) ? property.Name : father + "." + property.Name, filter);
                    }
                }
            }
            //简单类型
            else
            {
                if (property.Name != EntityKey)//更新集中不能有实体键_id
                {
                    fieldList.Add(Builders<TEntity>.Update.Set(string.IsNullOrWhiteSpace(father) ? property.Name : father + "." + property.Name, propertyValue));
                }
            }
        }


        /// <summary>
        /// 构建Mongo的更新表达式
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        private List<UpdateDefinition<TEntity>> GeneratorMongoUpdate(TEntity item, FilterDefinition<TEntity> filter)
        {
            var fieldList = new List<UpdateDefinition<TEntity>>();

            var properties = typeof(TEntity).GetProperties(BindingFlags.Instance | BindingFlags.Public)
                .Where(i => i.GetCustomAttribute(typeof(MongoDB.Bson.Serialization.Attributes.BsonIgnoreAttribute)) == null);
            foreach (var property in properties)
            {
                GenerateRecursionSet(fieldList, property, property.GetValue(item), item, string.Empty, filter);
            }
            return fieldList;
        }

        /// <summary>
        /// 按需要更新的构建者
        /// 递归构建Update操作串
        /// </summary>
        /// <param name="fieldList"></param>
        /// <param name="property"></param>
        /// <param name="propertyValue"></param>
        /// <param name="item"></param>
        /// <param name="fatherValue"></param>
        /// <param name="father"></param>
        private void GenerateRecursionExpress(
          List<UpdateDefinition<TEntity>> fieldList,
          PropertyInfo property,
          object propertyValue,
          TEntity item,
          object fatherValue,
          string father)
        {
            //复杂类型
            if (property.PropertyType.IsClass && property.PropertyType != typeof(string) && propertyValue != null)
            {
                //集合
                if (typeof(IList).IsAssignableFrom(propertyValue.GetType()))
                {
                    var modifyIndex = 0;//要更新的记录索引
                    foreach (var sub in property.PropertyType.GetProperties(BindingFlags.Instance | BindingFlags.Public))
                    {
                        if (sub.PropertyType.IsClass && sub.PropertyType != typeof(string))
                        {
                            var arr = propertyValue as IList;
                            if (arr != null && arr.Count > 0)
                            {

                                var oldValue = property.GetValue(fatherValue ?? item) as IList;
                                if (oldValue != null)
                                {
                                    for (int index = 0; index < arr.Count; index++)
                                    {
                                        for (modifyIndex = 0; modifyIndex < oldValue.Count; modifyIndex++)
                                            if (sub.PropertyType.GetProperty(EntityKey).GetValue(oldValue[modifyIndex]).ToString()
                                                == sub.PropertyType.GetProperty(EntityKey).GetValue(arr[index]).ToString())//比较_id是否相等
                                                break;
                                        foreach (var subInner in sub.PropertyType.GetProperties(BindingFlags.Instance | BindingFlags.Public))
                                        {
                                            if (string.IsNullOrWhiteSpace(father))
                                                GenerateRecursionExpress(fieldList, subInner, subInner.GetValue(arr[index]), item, arr[index], property.Name + "." + modifyIndex);
                                            else
                                                GenerateRecursionExpress(fieldList, subInner, subInner.GetValue(arr[index]), item, arr[index], father + "." + property.Name + "." + modifyIndex);
                                        }
                                    }
                                }

                            }
                        }
                    }
                }
                //实体
                else
                {
                    foreach (var sub in property.PropertyType.GetProperties(BindingFlags.Instance | BindingFlags.Public))
                    {

                        if (string.IsNullOrWhiteSpace(father))
                            GenerateRecursionExpress(fieldList, sub, sub.GetValue(propertyValue), item, property.GetValue(fatherValue), property.Name);
                        else
                            GenerateRecursionExpress(fieldList, sub, sub.GetValue(propertyValue), item, property.GetValue(fatherValue), father + "." + property.Name);
                    }
                }
            }
            //简单类型
            else
            {
                if (property.Name != EntityKey)//更新集中不能有实体键_id
                {
                    if (string.IsNullOrWhiteSpace(father))
                        fieldList.Add(Builders<TEntity>.Update.Set(property.Name, propertyValue));
                    else
                        fieldList.Add(Builders<TEntity>.Update.Set(father + "." + property.Name, propertyValue));
                }
            }
        }

        /// <summary>
        /// 等待Task执行完成后再返回
        /// </summary>
        /// <param name="func"></param>
        /// <returns></returns>
        private Task ForWait(Func<Task> func)
        {
            var t = func();
            t.Wait();
            return t;
        }
        #endregion

        #region IRepository<TEntity> 成员

        public TEntity Find(params object[] id)
        {
            if (id == null || id.Count() < 0 || id[0] == null)
                return null;
            var condition = Builders<TEntity>.Filter.Eq("_id", new ObjectId(id[0].ToString()));
            return _table.Find(condition).FirstOrDefaultAsync().Result;
        }

        public IQueryable<TEntity> GetModel()
        {
            return _table.AsQueryable();
        }

        public void SetDataContext(object db)
        {
            throw new NotImplementedException();
        }

        public void Insert(TEntity item)
        {
            _table.InsertOne(item);
        }

        public void Update(TEntity item)
        {
            var query = Builders<TEntity>.Filter.Eq("_id", new ObjectId(typeof(TEntity).GetProperty(EntityKey)
                                                           .GetValue(item)
                                                           .ToString()));
            _table.UpdateOne(
              query,
              Builders<TEntity>.Update.Combine(GeneratorMongoUpdate(item, query)));
        }

        public void Delete(TEntity item)
        {
            var query = Builders<TEntity>.Filter.Eq("_id", new ObjectId(typeof(TEntity).GetProperty(EntityKey)
                                                            .GetValue(item)
                                                            .ToString()));
            _table.DeleteOne(query);
        }

        public void Insert(IEnumerable<TEntity> item)
        {
            if (item != null && item.Any())
            {
                var list = new List<WriteModel<TEntity>>();
                foreach (var iitem in item)
                {
                    list.Add(new InsertOneModel<TEntity>(iitem));
                }
                _table.BulkWrite(list);
            }
        }

        public void Update(IEnumerable<TEntity> item)
        {
            if (item != null && item.Any())
            {
                var list = new List<WriteModel<TEntity>>();
                foreach (var iitem in item)
                {
                    var query = Builders<TEntity>.Filter.Eq("_id", new ObjectId(typeof(TEntity).GetProperty(EntityKey)
                                                           .GetValue(iitem)
                                                           .ToString()));
                    list.Add(new UpdateOneModel<TEntity>(query, Builders<TEntity>.Update.Combine(GeneratorMongoUpdate(iitem as TEntity, query))));
                }
                _table.BulkWrite(list);
            }
        }

        public void Delete(IEnumerable<TEntity> item)
        {
            if (item != null && item.Any())
            {
                var list = new List<WriteModel<TEntity>>();
                foreach (var iitem in item)
                {
                    var query = Builders<TEntity>.Filter.Eq("_id", new ObjectId(typeof(TEntity).GetProperty(EntityKey)
                                                           .GetValue(iitem)
                                                           .ToString()));
                    list.Add(new DeleteOneModel<TEntity>(query));
                }
                _table.BulkWrite(list);
            }
        }

        #endregion

        #region IMongoRepository<TEntity> 成员


        public TEntity MapReduce(string map, string reduce)
        {
            throw new NotImplementedException();
        }
        public IEnumerable<TEntity> GetModel<U>(U template)
        {

            return _table.Find(GeneratorMongoQuery(template)).ToListAsync().Result;
        }

        public PagedResult<TEntity> GetModel<U>(int pageIndex, int pageSize)
        {
            return this.GetModel(new { }, pageIndex, pageSize);

        }

        public PagedResult<TEntity> GetModel<U>(U template, int pageIndex, int pageSize)
        {
            return this.GetModel(template, new { }, pageIndex, pageSize);
        }

        public PagedResult<TEntity> GetModel<U, O>(U template, O orderby, int pageIndex, int pageSize)
        {
            #region 条件过滤
            BsonDocumentFilterDefinition<TEntity> filterDefinition = GeneratorMongoQuery(template);
            #endregion

            #region 排序处理
            SortDefinition<TEntity> sorts = new ObjectSortDefinition<TEntity>(new { });
            foreach (var item in typeof(O).GetProperties())
            {
                if ((OrderType)item.GetValue(orderby) == OrderType.Asc)
                    sorts = sorts.Ascending(item.Name);
                else
                    sorts = sorts.Descending(item.Name);
            }
            #endregion

            #region 分页处理
            var skip = (pageIndex - 1) * pageSize;

            var recordCount = _table.Find(filterDefinition).CountAsync(new CancellationToken()).Result;
            var limit = pageSize;
            return new PagedResult<TEntity>(
                recordCount,
                (int)(recordCount + pageSize - 1) / pageSize,
                pageSize,
                pageIndex,
                _table.Find(filterDefinition)
                      .Sort(sorts)
                      .Skip(skip)
                      .Limit(limit)
                      .ToListAsync().Result);
            #endregion


        }

        public PagedResult<TEntity> GetModel(int pageIndex, int pageSize)
        {
            return GetModel(i => true, pageIndex, pageSize);
        }

        public PagedResult<TEntity> GetModel(System.Linq.Expressions.Expression<Func<TEntity, bool>> expression, int pageIndex, int pageSize)
        {
            return GetModel(expression, new Dictionary<Expression<Func<TEntity, object>>, bool>(), pageIndex, pageSize);
        }

        public PagedResult<TEntity> GetModel(
            Dictionary<Expression<Func<TEntity, object>>, bool> fields,
            int pageIndex,
            int pageSize)
        {
            return GetModel(i => true, fields, pageIndex, pageSize);
        }

        public PagedResult<TEntity> GetModel(
            Expression<Func<TEntity, bool>> expression,
            Dictionary<Expression<Func<TEntity, object>>, bool> fields,
            int pageIndex,
            int pageSize)
        {

            SortDefinition<TEntity> sorts = new ObjectSortDefinition<TEntity>(new { });
            foreach (var item in fields)
            {
                if (item.Value)
                    sorts = sorts.Ascending(item.Key);
                else
                    sorts = sorts.Descending(item.Key);
            }
            var skip = (pageIndex - 1) * pageSize;
            var limit = pageSize;
            var recordCount = _table.CountAsync<TEntity>(expression).Result;
            return new PagedResult<TEntity>(
                recordCount,
                (int)(recordCount + pageSize - 1) / pageSize,
                pageSize,
                pageIndex,
                _table.Find(expression)
                      .Sort(sorts)
                      .Skip(skip)
                      .Limit(limit)
                      .ToListAsync().Result);
        }

        public long Count(Expression<Func<TEntity, bool>> expression)
        {
            return _table.CountAsync(expression).Result;
        }

        public void Update<T>(IEnumerable<Expression<Action<T>>> list) where T : class
        {
            list.ToList().ForEach(entity =>
            {
                Update<T>(entity);
            });
        }

        public void Update<T>(System.Linq.Expressions.Expression<Action<T>> entity) where T : class
        {
            var query = Builders<TEntity>.Filter.Empty;

            var fieldList = new List<UpdateDefinition<TEntity>>();
            var objID = string.Empty;

            TEntity newEntity = typeof(TEntity).GetConstructor(Type.EmptyTypes).Invoke(null) as TEntity;//建立指定类型的实例
            List<string> propertyNameList = new List<string>();
            MemberInitExpression param = entity.Body as MemberInitExpression;
            var updateFields = param.Bindings.Select(i => i.Member.Name);
            foreach (var item in param.Bindings)
            {
                string propertyName = item.Member.Name;
                object propertyValue;
                var memberAssignment = item as MemberAssignment;
                if (memberAssignment.Expression.NodeType == ExpressionType.Constant)
                {

                    propertyValue = (memberAssignment.Expression as ConstantExpression).Value;
                    if (propertyName == EntityKey)
                        query = Builders<TEntity>.Filter.Eq("_id", new ObjectId(propertyValue.ToString()));
                }

                else
                {
                    propertyValue = Expression.Lambda(memberAssignment.Expression, null).Compile().DynamicInvoke();
                    if (propertyName == EntityKey)
                        query = Builders<TEntity>.Filter.Eq("_id", new ObjectId(propertyValue.ToString()));
                }
                typeof(T).GetProperty(propertyName).SetValue(newEntity, propertyValue, null);
                propertyNameList.Add(propertyName);
            }

            foreach (var property in newEntity.GetType().GetProperties(BindingFlags.Instance | BindingFlags.Public).Where(i => updateFields.Contains(i.Name)))
            {
                GenerateRecursionExpress(fieldList, property, property.GetValue(newEntity), newEntity, newEntity, string.Empty);
            }


            ForWait(() => _table.UpdateOneAsync(query, Builders<TEntity>.Update.Combine(fieldList)));
        }

        #endregion

        #region IExtensionRepository<TEntity> 成员

        public void BulkInsert(IEnumerable<TEntity> item, bool isRemoveIdentity)
        {
            throw new NotImplementedException();
        }

        public void BulkInsert(IEnumerable<TEntity> item)
        {
            throw new NotImplementedException();
        }

        public void BulkUpdate(IEnumerable<TEntity> item, params string[] fieldParams)
        {
            throw new NotImplementedException();
        }

        public void BulkDelete(IEnumerable<TEntity> item)
        {
            throw new NotImplementedException();
        }

        public IQueryable<TEntity> GetModel(System.Linq.Expressions.Expression<Func<TEntity, bool>> predicate)
        {
            return _table.Find(predicate).ToList().AsQueryable();
        }

        public TEntity Find(System.Linq.Expressions.Expression<Func<TEntity, bool>> predicate)
        {
            return GetModel(predicate).FirstOrDefault();
        }

        public IQueryable<TEntity> GetModel(Specification.ISpecification<TEntity> specification)
        {
            return GetModel(specification.SatisfiedBy());
        }

        public TEntity Find(Specification.ISpecification<TEntity> specification)
        {
            return Find(specification.SatisfiedBy());
        }

        public IQueryable<TEntity> GetModel(Action<IRepositories.Commons.IOrderable<TEntity>> orderBy, Specification.ISpecification<TEntity> specification)
        {
            var linq = new Orderable<TEntity>(GetModel(specification));
            orderBy(linq);
            return linq.Queryable;
        }

        #endregion

        #region IOrderableRepository<TEntity> 成员

        public IQueryable<TEntity> GetModel(Action<IRepositories.Commons.IOrderable<TEntity>> orderBy)
        {
            var linq = new Orderable<TEntity>(GetModel());
            orderBy(linq);
            return linq.Queryable;
        }

        public IQueryable<TEntity> GetModel(Action<IRepositories.Commons.IOrderable<TEntity>> orderBy, System.Linq.Expressions.Expression<Func<TEntity, bool>> predicate)
        {
            var linq = new Orderable<TEntity>(GetModel(predicate));
            orderBy(linq);
            return linq.Queryable;
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
