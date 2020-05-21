using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Lind.DDD.Repositories.Dapper;
using Dapper.Contrib.Extensions;

namespace Lind.DDD.UnitTest.Dapper
{
    [Table("UserInfo")]
    public class UserInfo : Lind.DDD.Domain.Entity
    {
       
        public string UserName { get; set; }
        public DateTime CreateDateTime { get; set; }
    }
    [TestClass]
    public class DapperTest
    {
        static string conn = System.Configuration.ConfigurationManager.ConnectionStrings["conn"].ConnectionString;
        DapperRepository<UserInfo> dapper = new DapperRepository<UserInfo>(conn);

        [TestMethod]
        public void Insert()
        {
            dapper.Insert(new UserInfo
            {
                UserName = "zzltest",
                CreateDateTime = DateTime.Now
            });
        }

        [TestMethod]
        public void Update()
        {
            var entity = dapper.Find(1);
            entity.DataUpdateDateTime = DateTime.Now;
            dapper.Update(entity);
        }

        [TestMethod]
        public void Get()
        {
            foreach (var item in dapper.GetModel())
            {
                Console.WriteLine(item.UserName);
            }
        }
    }
}
