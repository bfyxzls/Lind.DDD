using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Data.Entity;
using System.Collections.Generic;
using System.Linq;
namespace Lind.DDD.UnitTest
{
    public class User_Info_DTO
    {
        public int Id { get; set; }
        /// <summary>
        /// 手机
        /// </summary>
        public string Mobile { get; set; }
        /// <summary>
        /// 用户名
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 收货地址
        /// </summary>
        public string Address { get; set; }

        public ICollection<Topic> Topic { get; set; }
    }

    /// <summary>
    /// 用openID去好班取基础用户信息
    /// </summary>
    public class User_Info : Lind.DDD.Domain.Entity
    {
        /// <summary>
        /// 手机
        /// </summary>
        public string Mobile { get; set; }
        /// <summary>
        /// 用户名
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 收货地址
        /// </summary>
        public string Address { get; set; }

        public ICollection<Topic> Topic { get; set; }
    }
    /// <summary>
    /// 会话
    /// </summary>
    public class Topic : Lind.DDD.Domain.Entity
    {
        public Topic()
        {
            AddTime = DateTime.Now;
            EditTime = DateTime.Now;

        }
        public User_Info UserInfo { get; set; }

        public int UserId { get; set; }
        public string Title { get; set; }
        public string Img { get; set; }
        public string Link { get; set; }
        public DateTime AddTime { get; set; }
        public DateTime EditTime { get; set; }
        public int Status { get; set; }
    }

    /// <summary>
    /// LindDb这个数据库的上下文对象
    /// </summary>
    public class PrizeContext : DbContext
    {
        public PrizeContext()
            : base("TestConnection")
        {
            Database.SetInitializer<PrizeContext>(new CreateDatabaseIfNotExists<PrizeContext>());
            this.Configuration.AutoDetectChangesEnabled = true;//对多对多，一对多进行curd操作时需要为true
            this.Configuration.LazyLoadingEnabled = false;
            this.Configuration.ProxyCreationEnabled = false;//禁止动态拦截System.Data.Entity.DynamicProxies.
        }
        public DbSet<User_Info> User_Info { get; set; }
        public DbSet<Topic> Topic { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<User_Info>()
                      .HasMany(t => t.Topic)
                      .WithRequired(i => i.UserInfo)//不能为null
                      .HasForeignKey(t => t.UserId)
                      .WillCascadeOnDelete(false);
        }
    }


    [TestClass]
    public class EFTest
    {
        PrizeContext db = new PrizeContext();

        [TestMethod]
        public void Init()
        {
            var table = db.User_Info.ToListAsync().Result;
            Console.WriteLine("数据库初始化完成");
        }

        [TestMethod]
        public void Insert()
        {
            var user = new User_Info()
            {
                Address = "test",
                Mobile = "135",
                Name = "test",
                Topic = new List<Topic>() 
                { 
                    new Topic {Img = "", Status = 1, Title = "test1"　}, 
                    new Topic { Img = "", Status = 1, Title = "test2" } 
                }
            };
            db.User_Info.Add(user);
            db.SaveChanges();
            Console.WriteLine("添加用户完成");
        }


        [TestMethod]
        public void Update()
        {
            var user = db.User_Info
                         .Include(i => i.Topic)
                         .FirstOrDefaultAsync(i => i.Id == 1)
                         .Result;

            user.Name = "zzl_test";
            user.Topic.Add(new Topic { Img = "3", Status = 1, Title = "test3" });
            db.User_Info.Attach(user);
            db.Entry(user).State = EntityState.Modified;
            db.SaveChanges();
            Console.WriteLine("更新用户完成");
        }

        [TestMethod]
        public void Update_dto()
        {
            var redis = new User_Info()
             {
                 Address = "test",
                 Mobile = "135",
                 Name = "test",
                 Id = 1,
                 Topic = new List<Topic>() 
                 { 
                    new Topic {Id=1,Img = "", UserId=1,Status = 1, Title = "test11"　}, 
                    new Topic { Id=2,Img = "",UserId=1, Status = 1, Title = "test21" } 
                 }
             };
            User_Info_DTO dto = redis.MapTo<User_Info_DTO>();
            dto.Name = "dto";

            User_Info result = dto.MapTo<User_Info>();
            db.User_Info.Attach(result);
            foreach (var item in result.Topic)
            {
                db.Topic.Attach(item);
                db.Entry(item).State = EntityState.Modified;
                db.SaveChanges();
            }
            db.Entry(result).State = EntityState.Modified;
            db.SaveChanges();
        }


    }
}
