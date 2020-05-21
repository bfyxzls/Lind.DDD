using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MongoDB.Driver;
using System.Collections.Generic;
using Lind.DDD.IRepositories;
using Lind.DDD.Domain;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Lind.DDD.UnitTest
{
    public class Adderss
    {
        public string Province { get; set; }
        public string City { get; set; }
        public string District { get; set; }
        public Adderss()
        {

        }
        public Adderss(string p, string c, string d, string[] info = null)
        {
            Province = p;
            City = c;
            District = d;
            Info = info;
        }
        public string[] Info { get; set; }
    }
    public class Des
    {
        public int SortNum { get; set; }
        public string[] Worker { get; set; }
        public List<Adderss> Address { get; set; }
    }
    public class DogHistory
    {
        public List<Adderss> AddressList { get; set; }
        public bool IsHealth { get; set; }
        public string HistoryName { get; set; }
        public string[] Foods { get; set; }
        public int SortNum { get; set; }
    }
    public class Dog : NoSqlEntity
    {
        public Dog()
        {
            this.AddressHistory = new List<Adderss>();
            this.DogHistory = new List<DogHistory>();
            this.Foods = new string[] { };
        }
        public Adderss DefaultAdderss { get; set; }
        public Des Des { get; set; }
        public string Title { get; set; }
        public string Type { get; set; }
        public string[] Foods { get; set; }
        public List<DogHistory> DogHistory { get; set; }
        public List<Adderss> AddressHistory { get; set; }
    }


    public class Zoo : NoSqlEntity
    {
        public string title { get; set; }
        public double[] loc { get; set; }

    }

    [TestClass]
    public class MongodbTest
    {

        [TestMethod]
        public void LockMongodbTest()
        {
            Parallel.For(1, 1000, (o) =>
            {
                var filter = Builders<Dog>.Filter.Eq(i => i.Title, "钓竿");
                MongoDbClient.MongoManager<Dog>.Instance.UpdateOne(filter, Builders<Dog>.Update.Set("Type", o), options: new UpdateOptions
                {
                    IsUpsert = true
                });
            });
            Console.WriteLine("执行完成");
        }


        IExtensionRepository<Dog> repository = new Lind.DDD.Repositories.Mongo.MongoRepository<Dog>();

        #region  原生测试
        [TestMethod]
        public void Add()
        {

            MongoDbClient.MongoManager<Dog>.Instance.InsertOne(new Dog
            {
                Des = new Des
                {
                    SortNum = 3,
                    Worker = new string[] { "小工", "大工" },
                    Address = new List<Adderss>()
                    {
                      new Adderss("北京","房山","良乡",new string[]{"张谢","一区","70号"}),
                      new Adderss("北京","大兴","西红门",new string[]{"理想城","大满贯","4号楼"}),
                    }
                },

                DataStatus = Status.Deleted,
                Title = "仓储大叔",
                Type = "中国",
                DogHistory = new List<DogHistory> {
                    new DogHistory {SortNum=3, HistoryName = "大毛", IsHealth = true,  Foods = new string[] { "肉"},AddressList=new List<Adderss>{
                        new Adderss("北京","房山","良乡",new string[]{"张谢","一区","70号"}),
                         new Adderss("北京","大兴","西红门",new string[]{"理想城","大满贯","4号楼"}) }},
                    new DogHistory {SortNum=1, HistoryName = "毛仔", IsHealth = true, Foods = new string[] { "饲料"},AddressList=new List<Adderss>{
                        new Adderss("beijing","fangshan","liangxiang",new string[]{"zhangxie","Road1","No.70"}),
                        new Adderss("beijing","daxing","xihongmen",new string[]{"Lixiangcheng","Damanguan","No.4"}) }}
                }
            });
        }
        [TestMethod]
        public void Del()
        {
            MongoDbClient.MongoManager<Dog>.Instance.DeleteOne(Builders<Dog>.Filter.Eq(i => i.Title, "金毛"));
        }

        [TestMethod]
        public void Edit()
        {
            var filter = Builders<Dog>.Filter.Eq(i => i.Title, "金毛");
            MongoDbClient.MongoManager<Dog>.Instance.UpdateOne(filter, Builders<Dog>.Update.Set("Type", "中国"));
        }
        /// <summary>
        /// 推进来
        /// </summary>
        [TestMethod]
        public void Push()
        {
            var filter = Builders<Dog>.Filter.Eq(i => i.Id, "5850b0bdebb91a3184f90d3d");

            //更新所需要的字段
            var updateList = new List<UpdateDefinition<Dog>>();
            //更新需要集合类型的字段
            var dogHistoryList = new List<DogHistory>();
            //添加元素到集合属性
            dogHistoryList.Add(new DogHistory
            {
                HistoryName = "四虎子3",
                IsHealth = false,
            });
            dogHistoryList.Add(new DogHistory
            {
                HistoryName = "四虎子4",
                IsHealth = false,
            });
            //将需要更新集合对象添加到updateList里
            updateList.Add(Builders<Dog>.Update.PushEach(i => i.DogHistory, dogHistoryList));

            MongoDbClient.MongoManager<Dog>.Instance.UpdateOne(
                filter,
               Builders<Dog>.Update.Combine(updateList));

        }
        /// <summary>
        /// 拉出去
        /// </summary>
        [TestMethod]
        public void Pull()
        {
            var filter = Builders<Dog>.Filter.Eq(i => i.Title, "金毛");
            //拉简单类型
            MongoDbClient.MongoManager<Dog>.Instance.UpdateOne(filter, Builders<Dog>.Update.Pull(i => i.Foods, "甜品"));

            //拉复杂类型
            var filter2 = Builders<Dog>.Filter.Eq("DogHistory.HistoryName", "大毛");//找到一级对象
            var old = MongoDbClient.MongoManager<Dog>.Instance.Find(filter2).ToList();
            old.ForEach(item =>
            {
                var sub = item.DogHistory.FirstOrDefault(i => i.HistoryName == "大毛");//找到集合里的元素
                MongoDbClient.MongoManager<Dog>.Instance.UpdateOne(
                    filter2, Builders<Dog>.Update.Pull("DogHistory", sub));
            });



        }

        [TestMethod]
        public void GetAll()
        {
            foreach (var item in MongoDbClient.MongoManager<Dog>.Instance.Database.GetCollection<Dog>("Dog").AsQueryable())
            {
                Console.WriteLine(item.Title);
            }
        }


        [TestMethod]
        public void Get()
        {
            var model = MongoDbClient.MongoManager<Dog>.Instance.Find(Builders<Dog>.Filter.Eq(i => i.Type, "美国"));
            model.ToList().ForEach(i =>
            {
                Console.WriteLine(i.Title + "->" + i.Type);
            });


        }
        /// <summary>
        /// 根据子对象查询
        /// </summary>
        [TestMethod]
        public void GetInner()
        {
            var model = MongoDbClient.MongoManager<Dog>.Instance.Find(Builders<Dog>.Filter.Eq("DogHistory.HistoryName", "大毛"));
            model.ToList().ForEach(i =>
            {
                Console.WriteLine(i.Title + "->" + i.Type);
            });
        }

        [TestMethod]
        public void SubSort()
        {
            Console.WriteLine("--------------------子集合对象SortBy");

            var entityIndex = MongoDbClient.MongoManager<Dog>.Instance.Find(Builders<Dog>.Filter.Eq("DogHistory.HistoryName", "大毛")).FirstOrDefault();
            var index = entityIndex.DogHistory.FindIndex(i => i.HistoryName == "大毛");
            var model2 = MongoDbClient.MongoManager<Dog>.Instance
              .Find(Builders<Dog>.Filter.Eq("DogHistory.HistoryName", "大毛"))
              .SortBy(i => i.DogHistory[index].SortNum);
            model2.ToList().ForEach(i =>
            {
                Console.WriteLine(i.Title + ":" + i.DataStatus);
            });

            Console.WriteLine("--------------------子集合对象SortByDescending");
            var model = MongoDbClient.MongoManager<Dog>.Instance
                .Find(Builders<Dog>.Filter.Eq("DogHistory.HistoryName", "大毛"))
                .SortByDescending(i => i.DogHistory[index].SortNum);
            model.ToList().ForEach(i =>
            {
                Console.WriteLine(i.Title + ":" + i.DataStatus);
            });

        }
        [TestMethod]
        public void Sort()
        {

            Console.WriteLine("--------------------一级对象");
            //一级排序字段
            var model = MongoDbClient.MongoManager<Dog>.Instance
                .Find(Builders<Dog>.Filter.Eq("DogHistory.HistoryName", "大毛"))
                .SortBy(i => i.DataStatus).ToList();
            model.ToList().ForEach(i =>
            {
                Console.WriteLine(i.Title + ":" + i.DataStatus);
            });


            Console.WriteLine("--------------------子实体对象");
            var model3 = MongoDbClient.MongoManager<Dog>.Instance
              .Find(Builders<Dog>.Filter.Eq("DogHistory.HistoryName", "毛仔"))
              .SortBy(i => i.Des.SortNum).ToList();
            model3.ToList().ForEach(i =>
            {
                Console.WriteLine(i.Title + ":" + i.Title + ",sort:" + i.Des.SortNum);
            });


        }
        #endregion

        #region 仓储测试
        [TestMethod]
        public void RepositoryAdd()
        {
            repository.Insert(new Dog
            {
                Des = new Des { SortNum = 3 },
                DataStatus = Status.Deleted,
                Title = "哈巴狗1",
                Type = "美国",
                DogHistory = new List<DogHistory>(),
                Foods = new string[] { }
            });

        }

        [TestMethod]
        public void RepositoryAddList()
        {
            List<Dog> list = new List<Dog>();
            for (var i = 0; i < 10000; i++)
            {
                list.Add(new Dog
                {
                    Des = new Des { SortNum = 3 },
                    DataStatus = Status.Deleted,
                    Title = "哈巴狗1",
                    Type = "美国",
                    DogHistory = new List<DogHistory>(),
                    Foods = new string[] { }
                });
            }
            repository.Insert(list);
        }
        [TestMethod]
        public void RepositoryDel()
        {
            repository.Delete(new Dog
            {
                Id = "58087dd7ebb9191be8f144d9"
            });//只支持主键删除
        }

        [TestMethod]
        public void RepositoryEditList()
        {
            var dogList = repository.GetModel().ToList();
            dogList.ForEach(dog =>
            {
                dog.Type = "ddud";
                dog.Foods = new string[] { "1", "2", "3" };
                dog.AddressHistory = new List<Adderss>();
                dog.AddressHistory.Add(new Adderss("zzl", "lr", "123"));
                dog.AddressHistory.Add(new Adderss("zzl2", "lr2", "1232"));
                dog.AddressHistory.Add(new Adderss("zzl3", "lr3", "1233"));
                dog.DogHistory = new List<DogHistory>();
                dog.DogHistory.Add(new DogHistory
                {
                    SortNum = 3,
                    HistoryName = "大毛",
                    IsHealth = true,
                    Foods = new string[] { "肉" },
                    AddressList = new List<Adderss> { new Adderss("china", "beijing", "fangshan"), new Adderss("china", "shanghai", "pujiang") }

                });
                dog.DogHistory.Add(new DogHistory
                {
                    SortNum = 1,
                    HistoryName = "毛仔",
                    IsHealth = true,
                    Foods = new string[] { "饲料" },
                    AddressList = new List<Adderss> { new Adderss("usa", "jiazhou", "no.1") }
                });
                dog.Des.Worker = new string[] { "engineer", "coder" };
                dog.DefaultAdderss = new Adderss("1", "2", "3");
                dog.Des.Address = new List<Adderss>
            {
               new Adderss("beijing","fangshan","liangxiang",new string[]{"zhaojiaogan","Road100","No.300"}),
               new Adderss("美国","加州","1区",new string[]{"Road1","Room4","No.1"}),
             };
            });
            repository.Update(dogList);//没有被赋值的字段被null,建议先获取再更新
        }



        [TestMethod]
        public void RepositoryEdit()
        {
            var dog = repository.Find(i => i.Id == "58520492ebb91a2e74c6645a");
            dog.Type = "ddud";
            dog.Foods = new string[] { "1", "2", "3" };
            dog.AddressHistory = new List<Adderss>();
            dog.AddressHistory.Add(new Adderss("zzl", "lr", "123"));
            dog.AddressHistory.Add(new Adderss("zzl2", "lr2", "1232"));
            dog.AddressHistory.Add(new Adderss("zzl3", "lr3", "1233"));
            dog.DogHistory = new List<DogHistory>();
            dog.DogHistory.Add(new DogHistory
            {
                SortNum = 3,
                HistoryName = "大毛",
                IsHealth = true,
                Foods = new string[] { "肉" },
                AddressList = new List<Adderss> { new Adderss("china", "beijing", "fangshan"), new Adderss("china", "shanghai", "pujiang") }

            });
            dog.DogHistory.Add(new DogHistory
            {
                SortNum = 1,
                HistoryName = "毛仔",
                IsHealth = true,
                Foods = new string[] { "饲料" },
                AddressList = new List<Adderss> { new Adderss("usa", "jiazhou", "no.1") }
            });
            dog.Des.Worker = new string[] { "engineer", "coder" };
            dog.DefaultAdderss = new Adderss("1", "2", "3");
            dog.Des.Address = new List<Adderss>
            {
               new Adderss("beijing","fangshan","liangxiang",new string[]{"zhaojiaogan","Road100","No.300"}),
               new Adderss("美国","加州","1区",new string[]{"Road1","Room4","No.1"}),
             };

            repository.Update(dog);//没有被赋值的字段被null,建议先获取再更新
        }

        [TestMethod]
        public void RepositoryGet()
        {
            repository.GetModel(i => i.Type == "美国").ToList().ForEach(i =>
            {
                Console.WriteLine(i.Title + "->" + i.Type);
            });
        }

        [TestMethod]
        public void RepositoryGetInner()
        {
            var linq = repository.GetModel().Where(i => i.DogHistory.Where(j => j.HistoryName == "大毛").Any());
            linq.ToList().ForEach(i =>
            {
                Console.WriteLine(i.Title + "->" + i.Type);
            });
        }
        #endregion

        #region Mongodb坐标
        [TestMethod]
        public void Addloc()
        {
            for (int i = -180; i < 180; i += 5)
                MongoDbClient.MongoManager<Zoo>.Instance.InsertOne(new Zoo { title = "北京" + i, loc = new double[] { i, i } });
        }
        [TestMethod]
        public void Findloc()
        {
            var filter = Builders<Zoo>.Filter.Near(i => i.loc, 50, 50, maxDistance: 50);
            var result = MongoDbClient.MongoManager<Zoo>.Instance.Find(filter);
            foreach (var item in result.ToList())
            {
                Console.WriteLine(item.title);
            }
        }

        #endregion
    }
}
