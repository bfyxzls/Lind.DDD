using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lind.DDD.UnitTest
{
    public class TestCFContext : DbContext
    {
        public TestCFContext()
            : base("TestCF")
        {

            Database.SetInitializer<TestCFContext>(new CreateDatabaseIfNotExists<TestCFContext>());
            this.Configuration.AutoDetectChangesEnabled = true;//对多对多，一对多进行curd操作时需要为true
            this.Configuration.LazyLoadingEnabled = false;
            this.Configuration.ProxyCreationEnabled = false;//禁止动态拦截System.Data.Entity.DynamicProxies.
        }
        public DbSet<TestOrder> TestOrder { get; set; }
        public DbSet<TestUser> TestUser { get; set; }

    }

    public partial class TestOrder : Lind.DDD.Domain.Entity
    {
        public int UserID { get; set; }
        public decimal Price { get; set; }
        public int OrderStatus { get; set; }
        public int Count { get; set; }
        public string Info { get; set; }
    }

    public partial class TestUser : Lind.DDD.Domain.Entity
    {

        public string UserName { get; set; }
        public string Email { get; set; }
    }
}
