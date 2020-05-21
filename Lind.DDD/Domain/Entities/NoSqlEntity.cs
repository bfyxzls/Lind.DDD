using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Lind.DDD.Domain
{
    /// <summary>
    /// mongodb,xml,redis实体基类
    /// 主键类型为string
    /// </summary>
    [Serializable]
    public abstract class NoSqlEntity : EntityBase
    {
        /// <summary>
        /// 初始化NoSql
        /// </summary>
        public NoSqlEntity()
        {
            this.Id = ObjectId.GenerateNewId().ToString();
        }
        /// <summary>
        /// 标识列
        /// </summary>
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        [DataMember(Order = 0), XmlElement(Order = 0), DisplayName("编号"), Column("ID"), Required]
        public string Id { get; set; }

        /// <summary>
        /// 返回mongodb实体的键值对
        /// </summary>
        public IEnumerable<KeyValuePair<string, object>> GetProperyiesDictionary()
        {
            var properties = this.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance)
                   .Where(i => i.Name != "Id")
                   .ToArray();

            foreach (var i in properties)
                yield return new KeyValuePair<string, object>(i.Name, i.GetValue(this));

        }
    }
}
