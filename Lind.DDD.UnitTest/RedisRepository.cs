using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Lind.DDD.Repositories.Redis;
using Lind.DDD.Domain;
using System.Linq;
namespace Lind.DDD.UnitTest
{
    [Serializable]
    public class area : NoSqlEntity
    {
        public string Name { get; set; }
    }
    [TestClass]
    public class RedisRepository
    {
        RedisRepository<area> redis = new RedisRepository<area>();

        [TestMethod]
        public void Insert()
        {
            redis.Insert(new area { Name = "zzl" });
            redis.Insert(new area { Name = "大叔" });
        }
        [TestMethod]
        public void Get()
        {
            foreach (var item in redis.GetModel())
            {
                Console.WriteLine(item.Id + ":" + item.Name);
            }
        }
        [TestMethod]
        public void Update()
        {
            var old = redis.Find("57d8b111ebb91924a03f84b0");
            if (old != null)
            {
                old.Name = "占岭";
                redis.Update(old);
            }
        }
    }
}
