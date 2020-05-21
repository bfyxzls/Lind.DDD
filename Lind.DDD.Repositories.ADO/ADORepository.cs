using Lind.DDD.Domain;
using Lind.DDD.IRepositories;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Lind.DDD.Repositories.ADO
{
    public class ADORepository<T> : IRepository<T> where T : Entity, new()
    {
        #region Fields
        /// <summary>
        /// 数据库提供者
        /// </summary>
        private static readonly string dbProviderName = ConfigurationManager.AppSettings["DbProvider"];
        /// <summary>
        /// 数据库连接串
        /// </summary>
        private static readonly string dbConnectionString = ConfigurationManager.AppSettings["DbConnectionString"];
        /// <summary>
        /// 数据库连接对象
        /// </summary>
        private DbConnection dbConnection;
        #endregion


        #region 架构方法和析构方法
        public ADORepository()
        {
            this.dbConnection = CreateConnection();
        }

        ~ADORepository()
        {
            if (this.dbConnection != null)
            {
                this.dbConnection.Close();
                this.dbConnection.Dispose();
            }
        }
        #endregion

        /// <summary>
        /// 构建SQL链接
        /// </summary>
        /// <returns></returns>
        static DbConnection CreateConnection()
        {
            DbProviderFactory dbfactory = DbProviderFactories.GetFactory(dbProviderName);
            DbConnection dbconn = dbfactory.CreateConnection();
            dbconn.ConnectionString = dbConnectionString;
            return dbconn;
        }

        #region IRepository<T> 成员

        public T Find(params object[] id)
        {
            DbProviderFactory dbfactory = DbProviderFactories.GetFactory(dbProviderName);
            DbDataAdapter dbDataAdapter = dbfactory.CreateDataAdapter();

            dbDataAdapter.SelectCommand = new SqlCommand
            {
                CommandTimeout = 0,
                CommandType = CommandType.Text,
                CommandText = "select * from " + typeof(T).Name + " where  where id=@id"
            };
            SqlParameter _userid = new SqlParameter("id", SqlDbType.Int);
            _userid.SqlValue = id;
            dbDataAdapter.SelectCommand.Parameters.Add(_userid);

            DataTable dataTable = new DataTable();
            dbDataAdapter.Fill(dataTable);
            return new TBToList<T>().ToList(dataTable).FirstOrDefault();
        }

        public IQueryable<T> GetModel()
        {
            DbProviderFactory dbfactory = DbProviderFactories.GetFactory(dbProviderName);
            DbDataAdapter dbDataAdapter = dbfactory.CreateDataAdapter();
            dbDataAdapter.SelectCommand = new SqlCommand
            {
                CommandTimeout = 0,
                CommandType = CommandType.Text,
                CommandText = "select * from " + typeof(T).Name
            };


            DataTable dataTable = new DataTable();
            dbDataAdapter.Fill(dataTable);
            return new TBToList<T>().ToList(dataTable) as IQueryable<T>;
        }

        public void SetDataContext(object db)
        {
            throw new NotImplementedException();
        }

        public void Insert(T item)
        {
            if (item == null)
                throw new ArgumentException("The database entity can not be null.");

            Type entityType = item.GetType();
            var table = entityType.GetProperties().Where(i =>
                  i.Name != "IsValid"
                 && i.Name.ToLower() != "id"
                 && i.GetValue(item, null) != null
                 && !i.PropertyType.IsEnum
                 && (i.PropertyType.IsValueType || i.PropertyType == typeof(string))).ToArray();//过滤主键，航行属性，状态属性等

            var pkList = new List<string>() { "id" };

            var arguments = new List<object>();
            var fieldbuilder = new StringBuilder();
            var valuebuilder = new StringBuilder();

            fieldbuilder.Append(" INSERT INTO " + string.Format("[{0}]", entityType.Name) + " (");

            foreach (var member in table)
            {
                if (pkList.Contains(member.Name) && Convert.ToString(member.GetValue(item, null)) == "0")
                    continue;
                object value = member.GetValue(item, null);
                if (value != null)
                {
                    if (arguments.Count != 0)
                    {
                        fieldbuilder.Append(", ");
                        valuebuilder.Append(", ");
                    }

                    fieldbuilder.Append(member.Name);
                    if (member.PropertyType == typeof(string)
                        || member.PropertyType == typeof(DateTime)
                        || member.PropertyType == typeof(DateTime?)
                        || member.PropertyType == typeof(Boolean?)
                        || member.PropertyType == typeof(Boolean)
                        || member.PropertyType == typeof(Guid)
                        || member.PropertyType == typeof(Guid?)
                        )
                        valuebuilder.Append("'{" + arguments.Count + "}'");
                    else
                        valuebuilder.Append("{" + arguments.Count + "}");
                    if (value is string)
                        value = value.ToString().Replace("'", "char(39)");
                    arguments.Add(value);

                }
            }


            fieldbuilder.Append(") Values (");

            fieldbuilder.Append(valuebuilder.ToString());
            fieldbuilder.Append(");");
            var cmd = new SqlCommand
            {
                CommandTimeout = 0,
                CommandType = CommandType.Text,
                CommandText = fieldbuilder.ToString()
            };
            cmd.Connection.Open();
            cmd.ExecuteNonQuery();
        }

        public void Update(T item)
        {
            throw new NotImplementedException();
        }

        public void Delete(T item)
        {
            var cmd = new SqlCommand
            {
                CommandTimeout = 0,
                CommandType = CommandType.Text,
                CommandText = string.Format("delete from {0} where id={1}", typeof(T).Name, item.Id)
            };
            cmd.Connection.Open();
            cmd.ExecuteNonQuery();
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


    public class TBToList<T> where T : new()
    {
        /// <summary>
        /// 获取列名集合
        /// </summary>
        private IList<string> GetColumnNames(DataColumnCollection dcc)
        {
            IList<string> list = new List<string>();
            foreach (DataColumn dc in dcc)
            {
                list.Add(dc.ColumnName);
            }
            return list;
        }

        /// <summary>
        ///属性名称和类型名的键值对集合
        /// </summary>
        private Hashtable GetColumnType(DataColumnCollection dcc)
        {
            if (dcc == null || dcc.Count == 0)
            {
                return null;
            }
            IList<string> colNameList = GetColumnNames(dcc);

            Type t = typeof(T);
            PropertyInfo[] properties = t.GetProperties();
            Hashtable hashtable = new Hashtable();
            int i = 0;
            foreach (PropertyInfo p in properties)
            {
                foreach (string col in colNameList)
                {
                    if (col.ToLower().Contains(p.Name.ToLower()))
                    {
                        hashtable.Add(col, p.PropertyType.ToString() + i++);
                    }
                }
            }

            return hashtable;
        }

        /// <summary>
        /// DataTable转换成IList
        /// </summary>
        /// <param name="dt"></param>
        /// <returns></returns>
        public IList<T> ToList(DataTable dt)
        {
            if (dt == null || dt.Rows.Count == 0)
            {
                return null;
            }

            PropertyInfo[] properties = typeof(T).GetProperties();//获取实体类型的属性集合
            Hashtable hh = GetColumnType(dt.Columns);//属性名称和类型名的键值对集合
            IList<string> colNames = GetColumnNames(hh);//按照属性顺序的列名集合
            List<T> list = new List<T>();
            T model = default(T);
            foreach (DataRow dr in dt.Rows)
            {
                model = new T();//创建实体
                int i = 0;
                foreach (PropertyInfo p in properties)
                {
                    if (p.PropertyType == typeof(string))
                    {
                        p.SetValue(model, dr[colNames[i++]], null);
                    }
                    else if (p.PropertyType == typeof(int))
                    {
                        p.SetValue(model, int.Parse(dr[colNames[i++]].ToString()), null);
                    }
                    else if (p.PropertyType == typeof(DateTime))
                    {
                        p.SetValue(model, DateTime.Parse(dr[colNames[i++]].ToString()), null);
                    }
                    else if (p.PropertyType == typeof(float))
                    {
                        p.SetValue(model, float.Parse(dr[colNames[i++]].ToString()), null);
                    }
                    else if (p.PropertyType == typeof(double))
                    {
                        p.SetValue(model, double.Parse(dr[colNames[i++]].ToString()), null);
                    }
                }

                list.Add(model);
            }

            return list;
        }

        /// <summary>
        /// 按照属性顺序的列名集合
        /// </summary>
        private IList<string> GetColumnNames(Hashtable hh)
        {
            PropertyInfo[] properties = typeof(T).GetProperties();//获取实体类型的属性集合
            IList<string> ilist = new List<string>();
            int i = 0;
            foreach (PropertyInfo p in properties)
            {
                ilist.Add(GetKey(p.PropertyType.ToString() + i++, hh));
            }
            return ilist;
        }

        /// <summary>
        /// 根据Value查找Key
        /// </summary>
        private string GetKey(string val, Hashtable tb)
        {
            foreach (DictionaryEntry de in tb)
            {
                if (de.Value.ToString() == val)
                {
                    return de.Key.ToString();
                }
            }
            return null;
        }

    }
}



