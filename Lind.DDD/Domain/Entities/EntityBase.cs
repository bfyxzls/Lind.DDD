using Dapper.Contrib.Extensions;
using Lind.DDD.EntityValidation;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using System.Xml.Serialization;

namespace Lind.DDD.Domain
{
    /// <summary>
    /// 领域模型，实体模型基类，它可能有多种持久化方式，如DB,File,Redis,Mongodb,XML等
    /// Lind.DDD框架的领域模型与数据库实体合二为一
    /// </summary>
    [Serializable]
    public abstract class EntityBase : IEntity
    {
        #region Contructors
        /// <summary>
        /// 实体初始化
        /// </summary>
        public EntityBase()
            : this(Status.Normal, 0, DateTime.Now, DateTime.Now, 0)
        { }
        /// <summary>
        /// 带参数的初始化
        /// </summary>
        /// <param name="status">状态</param>
        /// <param name="updateDateTime">更新日期</param>
        /// <param name="createDateTime">插入日期</param>
        public EntityBase(Status status, short isDeleted, DateTime updateDateTime, DateTime createDateTime, int lastModifyUserId)
        {
            this.DataStatus = Status.Normal;
            this.IsDeleted = isDeleted;
            this.LastModifyTime = DateTime.Now;
            this.DataCreateDateTime = DateTime.Now;
            this.LastModifyUserId = lastModifyUserId;
        }
        #endregion

        #region Properties
        /// <summary>
        /// 建立时间
        /// </summary>
        [XmlIgnore, DataMember(Order = 3), XmlElement(Order = 3), DisplayName("建立时间"), Required]
        public DateTime DataCreateDateTime { get; set; }
        /// <summary>
        /// 更新时间
        /// </summary>
        [XmlIgnore, DataMember(Order = 2), XmlElement(Order = 2), DisplayName("更新时间"), Required, Column("LastModifyTime")]
        public DateTime LastModifyTime { get; set; }
        /// <summary>
        /// 更新人
        /// </summary>
        [XmlIgnore, DataMember(Order = 2), XmlElement(Order = 2), DisplayName("更新人"), Required, Column("LastModifyUserId")]
        public int LastModifyUserId { get; set; }
        /// <summary>
        /// 实体状态
        /// </summary>
        [XmlIgnore, DataMember(Order = 1), XmlElement(Order = 1), DisplayName("状态"), Required]
        public Status DataStatus { get; set; }
        /// <summary>
        /// 实体状态
        /// </summary>
        [XmlIgnore, DataMember(Order = 1), XmlElement(Order = 4), DisplayName("是否删除"), Required]
        public short IsDeleted { get; set; }
        #endregion

        #region Methods

        /// <summary>
        /// 数据校验是否成功
        /// 不需要被序列化到库里
        /// </summary>
        [NotMapped]
        [XmlIgnore]
        [Computed]
        [MongoDB.Bson.Serialization.Attributes.BsonIgnore]
        public bool IsValid
        {
            get
            {
                return GetRuleViolations().Count() == 0;
            }
        }
        /// <summary>
        /// 拿到实体验证的结果列表
        /// 结果为null或者Enumerable.Count()==0表达验证成功
        /// </summary>
        /// <returns></returns>
        public IEnumerable<RuleViolation> GetRuleViolations()
        {
            var properties = this.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance).ToArray();

            foreach (var i in properties)
            {
                var attr = i.GetCustomAttributes();
                foreach (var a in attr)
                {
                    var val = (a as ValidationAttribute);
                    if (val != null)
                        if (!val.IsValid(i.GetValue(this)))
                        {
                            string info = val.ErrorMessage;
                            if (string.IsNullOrWhiteSpace(info))
                                info = val.FormatErrorMessage(i.Name);
                            yield return new RuleViolation(info, i.Name);
                        }
                }
            }

        }
        #endregion

    }
}
