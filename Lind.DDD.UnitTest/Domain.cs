using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Lind.DDD.Domain;
using System.Linq;
using Lind.DDD.Utils;
using System.Collections.Generic;
namespace Lind.DDD.UnitTest
{


    #region  关于聚合根，实体，值对象的说明
    /// <summary>
    /// 值对象
    /// </summary>
    public class Address
    {
        /// <summary>
        /// 值对象
        /// </summary>
        public string Province { get; set; }
        public string City { get; set; }
        public string County { get; set; }
    }
    /// <summary>
    /// 实体
    /// </summary>
    public class OrderItem : IModifyBehavor
    {
        /// <summary>
        /// 实体主键，聚合根内(order)内唯一
        /// </summary>
        public string ProductId { get; set; }
        /// <summary>
        /// 值对象
        /// </summary>
        public string ProductName { get; set; }
        public decimal Price { get; set; }
        public int Count { get; set; }

        #region IModifyBehavor 成员

        public DateTime LastModifyTime
        {
            get;
            set;
        }

        public int LastModifyUserId
        {
            get;
            set;
        }

        public string LastModifyUserName
        {
            get;
            set;
        }

        #endregion
    }
    /// <summary>
    /// 聚合根
    /// </summary>
    public class Order
    {
        /// <summary>
        /// 聚合根主键，全局唯一
        /// </summary>
        public string Id { get; set; }
        /// <summary>
        /// 值对象
        /// </summary>
        public string OrderNum { get; set; }
        /// <summary>
        /// 值对象
        /// </summary>
        public Address UserAddress { get; set; }
        /// <summary>
        /// 实体集合
        /// </summary>
        public IList<OrderItem> OrderItems { get; set; }
    }
    #endregion

    public class UserInfo : Lind.DDD.Domain.Entity, IOwnerBehavor, ILogicDeleteBehavor
    {
        public string Name { get; set; }

        #region IOwnerBehavor 成员

        public int OwnerId
        {
            get;
            set;
        }

        #endregion

        #region ILogicDeleteBehavor 成员

        public bool IsDeleted
        {
            get;
            set;
        }

        #endregion

        #region IOwnerBehavor 成员


        public string OwnerName
        {
            get;
            set;
        }

        #endregion
    }

    public class AD : Lind.DDD.Domain.Entity
    {
        public string Title { get; set; }
    }

    [TestClass]
    public class Domain
    {
        [TestMethod]
        public void GetOwner()
        {
            //拥有者接口
            var types1 = AssemblyHelper.GetTypesByInterfaces(typeof(IOwnerBehavor));
            //修改者接口
            var types2 = AssemblyHelper.GetTypesByInterfaces(typeof(IModifyBehavor));
            //邏輯刪除接口
            var types3 = AssemblyHelper.GetTypesByInterfaces(typeof(ILogicDeleteBehavor));

            foreach (var type in types1)
                Console.WriteLine("拥有者接口：" + type.Name);

            foreach (var type in types2)
                Console.WriteLine("修改者接口：" + type.Name);

            foreach (var type in types3)
                Console.WriteLine("邏輯刪除接口：" + type.Name);
        }
    }
}
