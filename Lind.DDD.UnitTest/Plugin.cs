using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using Lind.DDD.LindPlugins;

namespace Lind.DDD.UnitTest
{
    public class News
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public int Type { get; set; }
        public string Info { get; set; }
    }
    public class NewsType
    {
        public int Id { get; set; }
        public string Title { get; set; }
    }
    public class PeopleChina
    {
        public string Name { get; set; }
        public int Age { get; set; }
    }

    public class PeopleUSA
    {
        public string Name { get; set; }
        public string Owner { get; set; }
        public string City { get; set; }
    }
    public interface IHello : IPlugins
    {
        void Hello();
    }
    public class China : IHello
    {
        #region IHello 成员

        public void Hello()
        {
            Console.WriteLine("世界你好！");
        }

        #endregion
    }
    public class US : IHello
    {
        #region IHello 成员

        public void Hello()
        {
            Console.WriteLine("Hello World!");
        }

        #endregion
    }
    [TestClass]
    public class Plugin
    {
        List<News> newList = new List<News>();
        static List<NewsType> newsType = new List<NewsType>();

        static Plugin()
        {
            newsType.Add(new NewsType { Id = 1, Title = "人" });
            newsType.Add(new NewsType { Id = 2, Title = "动物" });
        }
        [TestMethod]
        public void TestDll()
        {
            PluginManager.Resolve<IHello>("Lind.DDD.UnitTest.China").Hello();
        }

        [TestMethod]
        public void AddNews()
        {
            newList.Add(new News
            {

                Id = 1,
                Title = "people",
                Type = 1,
                Info = Lind.DDD.Utils.SerializeMemoryHelper.SerializeToJson(new PeopleChina { Age = 23, Name = "测试" })
            });
            newList.Add(new News
            {

                Id = 1,
                Title = "people",
                Type = 2,
                Info = Lind.DDD.Utils.SerializeMemoryHelper.SerializeToJson(new PeopleUSA { Owner = "usa", City = "meiguozhou", Name = "lind" })
            });
            foreach (var item in newList)
                Console.WriteLine(item.Info);

            newList.ForEach(i =>
            {
                if (i.Type == 1)
                {
                    var entity = Lind.DDD.Utils.SerializeMemoryHelper.DeserializeFromJson<PeopleChina>(i.Info);
                    Console.WriteLine(entity.ToString());
                }

                if (i.Type == 2)
                {
                    var entity = Lind.DDD.Utils.SerializeMemoryHelper.DeserializeFromJson<PeopleUSA>(i.Info);
                    Console.WriteLine(entity.ToString());
                }
            });
        }
    }
}
