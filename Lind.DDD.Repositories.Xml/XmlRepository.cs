using Lind.DDD.Domain;
using Lind.DDD.IRepositories;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Serialization;

namespace Lind.DDD.Repositories.Xml
{

    /// <summary>
    /// XML简单类型仓储（没有子层级结构）
    /// XML文件数据仓储
    /// XML结构为Element
    /// </summary>
    /// <typeparam name="TEntity"></typeparam>
    public class XmlRepository<TEntity> :
       IRepository<TEntity>
       where TEntity : NoSqlEntity, new()
    {
        #region Fields
        private XDocument _doc;
        private string _filePath;
        private static object lockObj = new object();
        #endregion

        #region Constructors
        /// <summary>
        /// 初始化XML仓储
        /// </summary>
        /// <param name="dbName">XML文件名及根名称</param>
        public XmlRepository(string dbName)
        {
            var file = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, dbName + ".xml");
            if (!File.Exists(file))
            {
                using (System.IO.StreamWriter srFile = new System.IO.StreamWriter(file, true))
                {
                    srFile.WriteLine("<?xml version=\"1.0\"?><" + dbName + "></" + dbName + ">");
                }
            }
            _filePath = file;
            _doc = XDocument.Load(file);
        }
        //[Microsoft.Practices.Unity.InjectionConstructor]
        public XmlRepository() : this("XmlDocument") { }
        #endregion

        #region IRepository<TEntity> 成员

        public void Insert(TEntity item)
        {
            if (item == null)
                throw new ArgumentException("The database entity can not be null.");


            XElement db = new XElement(typeof(TEntity).Name);
            foreach (var member in item.GetType().GetProperties().OrderByDescending(i => i.Name == "Id")
                .Where(i => i.PropertyType.IsValueType || i.PropertyType == typeof(String)))//只找简单类型的属性
            {
                var value = member.GetValue(item, null);
                var description = (DisplayNameAttribute)member.GetCustomAttributes(typeof(DisplayNameAttribute), false).FirstOrDefault() ?? new DisplayNameAttribute();
                if (value == null)
                    db.Add(new XElement(member.Name, new XAttribute("value", ""), new XAttribute("description", description.DisplayName)));
                else
                    db.Add(new XElement(member.Name, new XAttribute("value", value), new XAttribute("description", description.DisplayName)));
            }

            _doc.Root.Add(db);
            lock (lockObj)
            {
                _doc.Save(_filePath);
            }

　        }

        public void Delete(TEntity item)
        {
            if (item == null)
                throw new ArgumentException("The database entity can not be null.");


            XElement xe = (from db in _doc.Root.Elements(typeof(TEntity).Name)
                           where db.Element("Id").Attribute("value").Value == item.Id
                           select db).Single() as XElement;
            xe.Remove();
            lock (lockObj)
            {
                _doc.Save(_filePath);
            }
        }

        public void Update(TEntity item)
        {
            if (item == null)
                throw new ArgumentException("The database entity can not be null.");

            XElement xe = (from db in _doc.Root.Elements(typeof(TEntity).Name)
                           where db.Element("Id").Attribute("value").Value == item.Id
                           select db).Single();
            try
            {
                foreach (var member in item.GetType()
                                           .GetProperties()
                                           .Where(i => i.PropertyType.IsValueType
                                               || i.PropertyType == typeof(String)))//只找简单类型的属性
                {
                    xe.Add(new XElement(member.Name, new XAttribute("value", member.GetValue(item, null))));
                }
                lock (lockObj)
                {
                    _doc.Save(_filePath);
                }
            }

            catch
            {
                throw;
            }

        }

        public IQueryable<TEntity> GetModel()
        {
            IEnumerable<XElement> list = _doc.Root.Elements(typeof(TEntity).Name);
            IList<TEntity> returnList = new List<TEntity>();
            foreach (var item in list)
            {
                TEntity entity = new TEntity();
                foreach (var member in entity.GetType()
                                             .GetProperties()
                                             .Where(i => i.PropertyType.IsValueType
                                                 || i.PropertyType == typeof(String)))//只找简单类型的属性,支持枚举
                {
                    if (item.Element(member.Name) != null)
                    {
                        if (member.PropertyType.IsEnum)
                        {
                            var obj = Enum.Parse(member.PropertyType, item.Element(member.Name).FirstAttribute.Value);
                            member.SetValue(entity, obj, null);
                        }
                        else
                        {
                            member.SetValue(entity, Convert.ChangeType(item.Element(member.Name).FirstAttribute.Value, member.PropertyType), null);
                        }
                    }
                }
                returnList.Add(entity);
            }
            return returnList.AsQueryable();
        }

        public TEntity Find(params object[] id)
        {
            return GetModel().FirstOrDefault(i => i.Id == Convert.ToString(id[0]));
        }

        public void SetDataContext(object db)
        {
            throw new NotImplementedException();
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
