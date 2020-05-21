using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lind.DDD.UnitTest
{
    /// <summary>
    /// 实体
    /// </summary>
    class Entity
    {
        public string Id { get; set; }
        public string Des { get; set; }
    }
    /// <summary>
    /// 实体集合，并添加个性化方法
    /// </summary>
    class EntityCollection : List<Entity>
    {
        public void SortById()
        {
            this.OrderBy(i => i.Id)
                .ToList()
                .ForEach(i => Console.WriteLine(i.Id));
        }
    }

    /// <summary>
    /// 实体与实体集合
    /// </summary>
    [TestClass]
    public class Entity_ListEntity
    {
        [TestMethod]
        public void TestMethod1()
        {
            var list = new EntityCollection();
            list.Add(new Entity { Id = "a", Des = "OKa" });
            list.Add(new Entity { Id = "c", Des = "OKc" });
            list.Add(new Entity { Id = "b", Des = "OKb" });
            list.SortById();
        }
    }
}
