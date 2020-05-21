using Lind.DDD.IRepositories;
using Lind.DDD.Web.Enums;
using Lind.DDD.Web.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lind.DDD.Web
{
    /// <summary>
    /// 数据库初始化
    /// </summary>
    public class LindDbInitializer : DropCreateDatabaseIfModelChanges<LindDbContext>
    {
        protected override void Seed(LindDbContext context)
        {
            try
            {
                #region 用户表
                var user = new UserInfo
                {
                    RealName = "仓储大叔",
                    UserName = "admin",
                    Password = "admin",
                    Email = "bfyxzls@sina.com",
                    QQ = "853066980",
                    Role = UserRole.Manager,
                    UserExtension = new UserExtension
                    {
                        School = "北工大",
                        NickName = "占占"
                    },
                    UserAccount = new List<UserAccount>
                    {
                        new UserAccount{
                        FreezeMoney = 0,
                        Money = 0,
                        TotalMoney = 0,
                        }
                    }
                };
                user.Password = user.Md5Password();

                var user2 = new UserInfo
                {
                    RealName = "匿名用户",
                    UserName = "guest",
                    Password = "guest",
                    Email = "niming@sina.com",
                    QQ = "853066980",
                    Role = UserRole.User,
                    UserExtension = new UserExtension
                    {
                        School = "北工大",
                        NickName = "占占"
                    },
                    UserAccount = new List<UserAccount>
                    {
                        new UserAccount{
                        FreezeMoney = 0,
                        Money = 0,
                        TotalMoney = 0,
                        }
                    }
                };
                user2.Password = user2.Md5Password();
                context.UserInfo.Add(user);
                context.UserInfo.Add(user2);
                context.SaveChanges();
                #endregion

                #region 分类表
                var product1 = new Product
              {
                  Description = "企业级框架，包括所有Demo，主张分层开发，横切与解耦功能点",
                  Discount = 100,
                  Price = 1000,
                  Name = "大叔企业级框架集",
                  UserInfoId = user.Id,
                  UserInfoUserName = user.UserName
              };
                var product2 = new Product
                {
                    Description = "Lind.DDD敏捷框架，主张快速开发，包括Demo有mvc,api,soa,xamarin移动开发等",
                    Discount = 100,
                    Price = 800,
                    Name = "Lind.DDD敏捷框架集",
                    UserInfoId = user.Id,
                    UserInfoUserName = user.UserName
                };
                var product3 = new Product
                {
                    Description = "企业级框架配套的视频，帮助各位最快速的学习大叔框架",
                    Discount = 100,
                    Price = 500,
                    Name = "大叔讲框架",
                    UserInfoId = user.Id,
                    UserInfoUserName = user.UserName
                };
                var product4 = new Product
                {
                    Description = "大叔提供的永久的技术支持，有问题请找大叔",
                    Discount = 100,
                    Price = 200,
                    Name = "技术支持",
                    UserInfoId = user.Id,
                    UserInfoUserName = user.UserName
                };
                var product5 = new Product
                {
                    Description = "大叔定期发布最新的源代码和最新的Demo，各位可以在群里查收",
                    Discount = 100,
                    Price = 100,
                    Name = "源码升级",
                    UserInfoId = 1,
                    UserInfoUserName = "Lind.Zhang"
                };
                var product6 = new Product
                {
                    Description = "书籍包括：新手（大叔也说EF，大叔也说MVC系列，大叔也说ORM~EF，大叔也说WEB Api，大叔也说面向对象，大叔也说设计模式，基础才是重中之重文章系列），老手（大叔架构师之路，大叔企业级架构，架构改善程序复用性的设计）",
                    Discount = 100,
                    Price = 100,
                    Name = "大叔的书籍",
                    UserInfoId = user.Id,
                    UserInfoUserName = user.UserName
                };
                var category1 = new Category
                {
                    Name = "框架源码",
                    Description = "框架源码",
                    Level = 1,
                    ParentId = 0,
                    Product = new List<Product>() { product1, product2 }
                };
                var category2 = new Category
                {
                    Name = "教程和资料",
                    Description = "大叔自己录制的视频教程和总结的技术资料",
                    Level = 1,
                    ParentId = 0,
                    Product = new List<Product>() { product3, product6 }
                };
                var category3 = new Category
                {
                    Name = "技术支持与代码升级",
                    Description = "技术支持与代码升级，大叔定期为你们更新源代码，随时有问题随时问大叔",
                    Level = 1,
                    ParentId = 0,
                    Product = new List<Product>() { product4, product5 }
                };
                context.Category.Add(category1);
                context.Category.Add(category2);
                context.Category.Add(category3);
                context.SaveChanges();
                #endregion

                base.Seed(context);
            }
            catch (Exception)
            {

                throw;
            }


        }
    }
}
