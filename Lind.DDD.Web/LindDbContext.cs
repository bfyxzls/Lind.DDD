using Lind.DDD.IRepositories;
using Lind.DDD.Web.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure.Interception;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lind.DDD.Web
{
    /// <summary>
    /// LindDb这个数据库的上下文对象
    /// </summary>
    public class LindDbContext : DbContext
    {
        public LindDbContext()
            : base("LindDb")
        {
            Database.SetInitializer<LindDbContext>(new DropCreateDatabaseIfModelChanges<LindDbContext>());
            this.Configuration.AutoDetectChangesEnabled = false;
            this.Configuration.LazyLoadingEnabled = false;
        }
        public DbSet<UserInfo> UserInfo { get; set; }
        public DbSet<UserExtension> UserExtension { get; set; }
        public DbSet<OrderInfo> OrderInfo { get; set; }
        public DbSet<Product> Product { get; set; }
        public DbSet<Category> Category { get; set; }
        public DbSet<UserAccount> UserAccount { get; set; }
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<UserInfo>().HasRequired(p => p.UserExtension).WithOptional(p => p.UserInfo);//一对一关系
        }
    }


}
