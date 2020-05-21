using Lind.DDD.IRepositories;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Lind.DDD.Manager.Models;
using EntityFramework.DynamicFilters;

namespace Lind.DDD.Manager
{
    /// <summary>
    /// LindDb这个数据库的上下文对象
    /// </summary>
    public class ManagerContext : DbContext
    {
        public ManagerContext()
            : base("DefaultConnection")
        {

            //没有时就建立它：new CreateDatabaseIfNotExists<ManagerContext>();
            //模型变更就删除它：new DropCreateDatabaseIfModelChanges<ManagerContext>();
            Database.SetInitializer<ManagerContext>(new CreateDatabaseIfNotExists<ManagerContext>());
            this.Configuration.AutoDetectChangesEnabled = true;//对多对多，一对多进行curd操作时需要为true
            this.Configuration.LazyLoadingEnabled = false;
            this.Configuration.ProxyCreationEnabled = false;//禁止动态拦截System.Data.Entity.DynamicProxies.
        }
        public DbSet<WebDataSetting> WebDataSetting { get; set; }
        public DbSet<WebDataCtrl> WebDataCtrl { get; set; }
        public DbSet<WebManageUsers> WebManageUsers { get; set; }
        public DbSet<WebManageRoles> WebManageRoles { get; set; }
        public DbSet<WebManageMenus> WebManageMenus { get; set; }
        public DbSet<WebDepartments> WebDepartments { get; set; }
        public DbSet<WebConfirmRecord> WebConfirmRecord { get; set; }
        public DbSet<WebManageRoles_WebManageMenus_Authority_R> WebManageRoles_WebManageMenus_Authority_R { get; set; }
        public DbSet<WebLogger> WebLogger { get; set; }
        public DbSet<WebCommonAreas> WebCommonAreas { get; set; }
        public DbSet<Lind.DDD.Domain.WebAuthorityCommands> WebAuthorityCommands { get; set; }
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Filter("LogicDelete", (Lind.DDD.Domain.ILogicDeleteBehavor d) => !d.IsDeleted);

            #region Data Map
            //modelBuilder.Entity<UserInfo>()
            //           .HasRequired(p => p.UserExtension)
            //           .WithOptional(p => p.UserInfo);//一对一关系

            //WebManageMenus自关联一对多
            modelBuilder.Entity<WebManageMenus>()
                        .HasMany(t => t.Sons)
                        .WithOptional(t => t.Father)//可以为null
                        .HasForeignKey(t => t.ParentID);

            //WebDepartments自关联一对多
            modelBuilder.Entity<WebDepartments>()
                        .HasMany(t => t.Sons)
                        .WithOptional(t => t.Father)
                        .HasForeignKey(t => t.ParentID);

            //WebDepartments-WebManageRoles一对多
            modelBuilder.Entity<WebDepartments>()
                        .HasMany(t => t.WebManageRoles)
                        .WithRequired(i => i.WebDepartments)//不能为null
                        .HasForeignKey(t => t.DepartmentID)
                        .WillCascadeOnDelete(false);

            //WebDepartments-WebManageUsers多对多
            modelBuilder.Entity<WebDepartments>()
                     .HasMany(t => t.WebManageUsers)
                     .WithMany(i => i.WebDepartments)
                     .Map(m =>
                      {
                          m.ToTable("WebDepartments_WebManageUsers_R");    //中间关系表表名
                          m.MapLeftKey("DeptId");   //设置WebDepartments表在中间表的主键名
                          m.MapRightKey("UserId");    //设置WebManageUsers表在中间表的主键名
                      });
            /*-------------------------------这块添加了权限字段------------------------------------------------
             //WebManageRoles－WebManageMenus　多对多
             modelBuilder.Entity<WebManageRoles>()
                         .HasMany(t => t.WebManageMenus)
                         .WithMany(a => a.WebManageRoles)
                         .Map(m =>
                         {
                             m.ToTable("WebManageRoles_WebManageMenus_R");    //中间关系表表名
                             m.MapLeftKey("RoleId");   //设置Activity表在中间表的主键名
                             m.MapRightKey("MenuId");    //设置Trip表在中间表的主键名
                            
                         });
             ---------------------------------------------------------------------------------------*/

            //WebManageMenus-WebManageRoles_WebManageMenus_Authority_R一对多
            modelBuilder.Entity<WebManageMenus>()
                      .HasMany(t => t.WebManageRoles_WebManageMenus_Authority_R)
                      .WithRequired(i => i.WebManageMenus)//不能为null
                      .HasForeignKey(t => t.MenuId)
                      .WillCascadeOnDelete(false);
            //WebManageRoles-WebManageRoles_WebManageMenus_Authority_R一对多
            modelBuilder.Entity<WebManageRoles>()
                      .HasMany(t => t.WebManageRoles_WebManageMenus_Authority_R)
                      .WithRequired(i => i.WebManageRoles)//不能为null
                      .HasForeignKey(t => t.RoleId)
                      .WillCascadeOnDelete(false);

            //WebManageRoles-WebManageUsers 多对多
            modelBuilder.Entity<WebManageRoles>()
                     .HasMany(t => t.WebManageUsers)
                     .WithMany(a => a.WebManageRoles)
                     .Map(m =>
                     {
                         m.ToTable("WebManageRoles_WebManageUsers_R");    //中间关系表表名
                         m.MapLeftKey("RoleId");   //设置Activity表在中间表的主键名
                         m.MapRightKey("UserId");    //设置Trip表在中间表的主键名
                     });

            //WebManageRoles-WebDataSetting一对多
            modelBuilder.Entity<WebManageRoles>()
                   .HasMany(t => t.WebDataSetting)
                   .WithRequired(i => i.WebManageRoles)//不能为null
                   .HasForeignKey(t => t.WebManageRolesId)
                   .WillCascadeOnDelete(false);

            //WebDataCtrl-WebDataSetting一对多
            modelBuilder.Entity<WebDataCtrl>()
               .HasMany(t => t.WebDataSetting)
               .WithRequired(i => i.WebDataCtrl)//不能为null
               .HasForeignKey(t => t.WebDataCtrlId)
               .WillCascadeOnDelete(false);

            //WebCommonAreas自关联一对多
            modelBuilder.Entity<WebCommonAreas>()
                        .HasMany(t => t.Sons)
                        .WithOptional(t => t.Father)
                        .HasForeignKey(t => t.ParentId);

            #endregion


        }
    }


}
