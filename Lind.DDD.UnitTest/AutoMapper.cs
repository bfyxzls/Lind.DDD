using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Lind.DDD.Paging;
using System.Collections.Generic;
using System.Linq;
namespace Lind.DDD.UnitTest
{
    [TestClass]
    public class AutoMapper
    {
        class TestDB
        {
            public string Name { get; set; }
            public string Tel { get; set; }
            public string Email { get; set; }
            public int Age { get; set; }
        }
        class TestDTO
        {
            public string Name { get; set; }
            public string Tel { get; set; }

        }

        [TestMethod]
        public void EntityToEntity()
        {

            var form = new TestDB { Name = "modify", Tel = "132" };
            var db = new TestDB { Name = "zzl", Tel = "13521", Email = "bfyxzls@sina.com", Age = 33 };
            form.MapTo<TestDB>(db);

            var c = db.ToDictionary();

        }

        [TestMethod]
        public void PagedList()
        {
            PagedList<TestDB> list = new PagedList<TestDB>
            {
                new TestDB{Name="zzl",Tel="13521"},
                new TestDB{Name="zhz",Tel="13621"},
            };
            list.PageIndex = 1;
            list.PageSize = 10;
            var old = list.MapToPaged<TestDTO>();
            foreach (var item in old)
            {
                Console.WriteLine(item.Name);
            }
            Console.WriteLine("pageIndex:{0},pageSize:{1}", old.PageIndex, old.PageSize);
        }

        [TestMethod]
        public void List()
        {
            List<TestDB> list = new List<TestDB>
            {
                new TestDB{Name="zzl",Tel="13521"},
                new TestDB{Name="zhz",Tel="13621"},
            };

            var old = list.MapTo<TestDB>();
        }

        [TestMethod]
        public void entity()
        {
            #region DTO以数据实体赋值(从表单拿到数据更新到数据库)
            var form = new TestDTO { Name = "zzl", Tel = "132" };
            var db = new TestDB { Name = "zzl", Tel = "13521", Email = "bfyxzls@sina.com", Age = 33 };
            db = form.MapTo<TestDB>(db);
            #endregion

            #region 数据实体为DTO赋值（从数据库把数据给DTO，并返回）
            var db2 = new TestDB { Name = "zzl", Tel = "13521972991" };
            form = db2.MapTo<TestDTO>();
            #endregion
        }
    }
}
