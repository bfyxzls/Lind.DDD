using Lind.DDD.IRepositories;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Lind.DDD.Manager.Models;
using Lind.DDD.Domain;
using System.Data.Entity.Validation;
using Lind.DDD.Authorization;
namespace Lind.DDD.Manager
{
    /// <summary>
    /// 数据库初始化
    /// </summary>
    public class ManagerInitializer : CreateDatabaseIfNotExists<ManagerContext>
    {
        protected override void Seed(ManagerContext context)
        {
            try
            {
                #region 操作命令字典表(操作权限的按钮)
                context.WebAuthorityCommands.Add(new WebAuthorityCommands
                {
                    Flag = 1,
                    ActionName = "Details",
                    Name = "查看",
                    Feature = WebAuthorityCommandFeature.Dialog
                });
                context.WebAuthorityCommands.Add(new WebAuthorityCommands
                {
                    Flag = 2,
                    ActionName = "Add",
                    Name = "添加",
                    Feature = WebAuthorityCommandFeature.None
                });
                context.WebAuthorityCommands.Add(new WebAuthorityCommands
                {
                    Flag = 4,
                    ActionName = "Edit",
                    Name = "编辑",
                    Feature = WebAuthorityCommandFeature.None
                });

                context.WebAuthorityCommands.Add(new WebAuthorityCommands
                {
                    Flag = 8,
                    ActionName = "Deleted",
                    Name = "删除",
                    Feature = WebAuthorityCommandFeature.Warn
                });

                context.WebAuthorityCommands.Add(new WebAuthorityCommands
                {
                    Flag = 16,
                    ActionName = "Freeze",
                    Name = "冻结",
                    Feature = WebAuthorityCommandFeature.Warn
                });
                context.WebAuthorityCommands.Add(new WebAuthorityCommands
                {
                    Flag = 32,
                    ActionName = "Approve",
                    Name = "审核",
                    Feature = WebAuthorityCommandFeature.Warn
                });
                context.SaveChanges();
                #endregion

                #region 部门表
                var department = new WebDepartments
                {
                    About = "",
                    Name = "根",
                    Level = 0,
                    Operator = "admin",
                    ParentID = null,
                    SortNumber = 0,
                    Sons = new List<WebDepartments> { 
                         new WebDepartments
                    { 
                        About = "",
                        Name = "总裁办",
                        Level = 1,
                        Operator = "admin",
                        SortNumber = 0},
                    new WebDepartments
                    { 
                        About = "",
                        Name = "研发部",
                        Level = 1,
                        Operator = "admin",
                        SortNumber = 0,
                        Sons=new  List<WebDepartments>
                        {
                          new WebDepartments
                             { 
                                 About = "",
                                 Name = "设计部",
                                 Level = 2,
                                 Operator = "admin",
                                 SortNumber = 0
                             },
                              new WebDepartments
                             { 
                                 About = "",
                                 Name = "产品部",
                                 Level = 2,
                                 Operator = "admin",
                                 SortNumber = 0
                             },
                              new WebDepartments
                             { 
                                 About = "",
                                 Name = "开发部",
                                 Level = 2,
                                 Operator = "admin",
                                 SortNumber = 0,
                                 Sons=new  List<WebDepartments>{
                                      new WebDepartments
                                      { 
                                          About = "",
                                          Name = "C#组",
                                          Level = 3,
                                          Operator = "admin",
                                          SortNumber = 0
                                      },
                                      new WebDepartments
                                      { 
                                          About = "",
                                          Name = "JAVA组",
                                          Level = 3,
                                          Operator = "admin",
                                          SortNumber = 0
                                      },
                                       new WebDepartments
                                       { 
                                          About = "",
                                          Name = "IOS组",
                                          Level = 3,
                                          Operator = "admin",
                                          SortNumber = 0
                                      },
                                       new WebDepartments
                                       { 
                                          About = "",
                                          Name = "Android组",
                                          Level = 3,
                                          Operator = "admin",
                                          SortNumber = 0
                                      },
                                 }
                             }
                         }
　                   },
                    new WebDepartments
                    { 
                        About = "",
                        Name = "人力部",
                        Level = 1,
                        Operator = "admin",
                        SortNumber = 0,
                    },
                    new WebDepartments
                    { 
                        About = "",
                        Name = "财物部",
                        Level = 1,
                        Operator = "admin",
                        SortNumber = 0,
                    }
               　 }
                };
                context.WebDepartments.Add(department);
                context.SaveChanges();
                #endregion

                #region 菜单表
                var menu = new WebManageMenus
                {
                    About = "",
                    LinkUrl = "",
                    Level = 0,
                    Name = "根",
                    Operator = "admin",
                    ParentID = null,
                    SortNumber = 0
                };

                #region 系统菜单
                var systemMenu = new WebManageMenus
                {
                    About = "",
                    LinkUrl = "",
                    Level = 1,
                    Name = "系统管理",
                    Operator = "admin",
                    SortNumber = 0
                };

                systemMenu.Sons.Add(new WebManageMenus
                {
                    About = "",
                    LinkUrl = "",
                    Level = 2,
                    Name = "菜单管理",
                    Operator = "admin",
                    ParentID = null,
                    SortNumber = 0,
                    Sons = new List<WebManageMenus> { 
                       new WebManageMenus{
                        About = "",
                        LinkUrl = "/WebMenu/Create",
                        Level = 3,
                        Name = "添加菜单",
                        Operator = "admin",
                        ParentID = null,
                        SortNumber = 0,
                        IsDisplayMenuTree=false,
                        Authority=(long)(Authority.Create|Authority.Detail)
                        },
                       new WebManageMenus{
                        About = "",
                        LinkUrl = "/WebMenu/Index",
                        Level = 3,
                        Name = "管理菜单",
                        Operator = "admin",
                        ParentID = null,
                        SortNumber = 0,
                        Authority=(long)(Authority.Edit|Authority.Detail|Authority.Delete)
                        }
                        ,
                       new WebManageMenus{
                        About = "",
                        LinkUrl = "/WebMenu/Edit",
                        Level = 3,
                        Name = "编辑菜单",
                        Operator = "admin",
                        ParentID = null,
                        SortNumber = 0,
                        IsDisplayMenuTree=false,
                        Authority=(long)(Authority.Edit|Authority.Detail)
                        }
                         ,
                       new WebManageMenus{
                        About = "",
                        LinkUrl = "/WebMenu/Delete",
                        Level = 3,
                        Name = "删除菜单",
                        Operator = "admin",
                        ParentID = null,
                        SortNumber = 0,
                        IsDisplayMenuTree=false,
                        Authority=(long)(Authority.Delete)
                        }
             　　   }
                });
                systemMenu.Sons.Add(new WebManageMenus
                {
                    About = "",
                    LinkUrl = "",
                    Level = 2,
                    Name = "部门管理",
                    Operator = "admin",
                    ParentID = null,
                    SortNumber = 0,
                    Sons = new List<WebManageMenus> { 
                       new WebManageMenus{
                        About = "",
                        LinkUrl = "/WebDept/Create",
                        Level =3,
                        Name = "添加部门",
                        Operator = "admin",
                        ParentID = null,
                        SortNumber = 0,
                        IsDisplayMenuTree=false,
                        Authority=(long)(Authority.Create|Authority.Detail)
                        },
                       new WebManageMenus{
                        About = "",
                        LinkUrl = "/WebDept/Index",
                        Level = 3,
                        Name = "管理部门",
                        Operator = "admin",
                        ParentID = null,
                        SortNumber = 0,
                        Authority=(long)(Authority.Edit|Authority.Detail|Authority.Delete)
                        }
                        ,
                       new WebManageMenus{
                        About = "",
                        LinkUrl = "/WebDept/Edit",
                        Level = 3,
                        Name = "编辑部门",
                        Operator = "admin",
                        ParentID = null,
                        SortNumber = 0,
                        IsDisplayMenuTree=false,
                        Authority=(long)(Authority.Edit|Authority.Detail)
                        }
                         ,
                        new WebManageMenus{
                        About = "",
                        LinkUrl = "/WebDept/Delete",
                        Level = 3,
                        Name = "删除部门",
                        Operator = "admin",
                        ParentID = null,
                        SortNumber = 0,
                        IsDisplayMenuTree=false,
                        Authority=(long)(Authority.Delete)
                        }
             　　   }
                });
                systemMenu.Sons.Add(new WebManageMenus
                {
                    About = "后台管理默认的主页",
                    LinkUrl = "",
                    Level = 2,
                    Name = "用户管理",
                    Operator = "admin",
                    ParentID = null,
                    SortNumber = 0,
                    Sons = new List<WebManageMenus> { 
                       new WebManageMenus{
                        About = "",
                        LinkUrl = "/WebUser/Create",
                        Level =3,
                        Name = "添加用户",
                        Operator = "admin",
                        ParentID = null,
                        SortNumber = 0,
                          Authority=(long)(Authority.Detail|Authority.Create)
                        },
                       new WebManageMenus{
                        About = "",
                        LinkUrl = "/WebUser/Index",
                        Level =3,
                        Name = "管理用户",
                        Operator = "admin",
                        ParentID = null,
                        SortNumber = 0,
                          Authority=(long)(Authority.Edit|Authority.Detail|Authority.Delete|Authority.Freeze)
                        },
                       new WebManageMenus{
                        About = "",
                        LinkUrl = "/WebUser/Edit",
                        Level =3,
                        Name = "编辑用户",
                        Operator = "admin",
                        ParentID = null,
                        SortNumber = 0,
                        IsDisplayMenuTree=false,
                        Authority=(long)(Authority.Edit|Authority.Detail)
                        }
                        ,
                        new WebManageMenus{
                        About = "",
                        LinkUrl = "/WebUser/Delete",
                        Level =3,
                        Name = "删除用户",
                        Operator = "admin",
                        ParentID = null,
                        SortNumber = 0,
                        IsDisplayMenuTree=false,
                        Authority=(long)(Authority.Delete)
                        }
             　　   }
                });
                systemMenu.Sons.Add(new WebManageMenus
                {
                    About = "",
                    LinkUrl = "",
                    Level = 2,
                    Name = "角色管理",
                    Operator = "admin",
                    ParentID = null,
                    SortNumber = 0,
                    Sons = new List<WebManageMenus> { 
                       new WebManageMenus{
                        About = "",
                        LinkUrl = "/WebRole/Create",
                        Level = 3,
                        Name = "添加角色",
                        Operator = "admin",
                        ParentID = null,
                        SortNumber = 0,
                         Authority=(long)(Authority.Detail|Authority.Create)
                        },
                       new WebManageMenus{
                        About = "",
                        LinkUrl = "/WebRole/Index",
                        Level = 3,
                        Name = "管理角色",
                        Operator = "admin",
                        ParentID = null,
                        SortNumber = 0,
                         Authority=(long)(Authority.Edit|Authority.Detail|Authority.Delete)
                        },
                       new WebManageMenus{
                        About = "",
                        LinkUrl = "/WebRole/Edit",
                        Level = 3,
                        Name = "编辑角色",
                        Operator = "admin",
                        ParentID = null,
                        SortNumber = 0,
                        IsDisplayMenuTree=false,
                        Authority=(long)(Authority.Edit|Authority.Detail)
                        }
                        ,
                       new WebManageMenus{
                        About = "",
                        LinkUrl = "/WebRole/Delete",
                        Level = 3,
                        Name = "删除角色",
                        Operator = "admin",
                        ParentID = null,
                        SortNumber = 0,
                        IsDisplayMenuTree=false,
                        Authority=(long)(Authority.Delete)
                        }
             　　   }
                });
                systemMenu.Sons.Add(new WebManageMenus
                {
                    About = "",
                    LinkUrl = "",
                    Level = 2,
                    Name = "数据集管理",
                    Operator = "admin",
                    ParentID = null,
                    SortNumber = 0,
                    Sons = new List<WebManageMenus> { 
                       new WebManageMenus{
                        About = "",
                        LinkUrl = "/WebDataCtrl/Create",
                        Level = 3,
                        Name = "添加类型",
                        Operator = "admin",
                        ParentID = null,
                        SortNumber = 0,
                         Authority=(long)(Authority.Create|Authority.Detail)
                        },
                       new WebManageMenus{
                        About = "",
                        LinkUrl = "/WebDataCtrl/Index",
                        Level = 3,
                        Name = "管理类型",
                        Operator = "admin",
                        ParentID = null,
                        SortNumber = 0,
                        Authority=(long)(Authority.Edit|Authority.Detail|Authority.Delete)
                        }
                        ,
                       new WebManageMenus{
                        About = "",
                        LinkUrl = "/WebDataCtrl/Delete",
                        Level = 3,
                        Name = "删除类型",
                        Operator = "admin",
                        ParentID = null,
                        SortNumber = 0,
                        Authority=(long)(Authority.Delete)
                        }
                        ,
                         new WebManageMenus{
                        About = "",
                        LinkUrl = "/WebDataCtrl/Edit",
                        Level = 3,
                        Name = "编辑类型",
                        Operator = "admin",
                        ParentID = null,
                        SortNumber = 0,
                        IsDisplayMenuTree=false,
                         Authority=(long)(Authority.Edit|Authority.Detail)
                        },
                         new WebManageMenus{
                        About = "",
                        LinkUrl = "/WebDataSetting/Create",
                        Level = 3,
                        Name = "添加数据集",
                        Operator = "admin",
                        ParentID = null,
                        SortNumber = 0,
                        Authority=(long)(Authority.Create|Authority.Detail)
                        },
                       new WebManageMenus{
                        About = "",
                        LinkUrl = "/WebDataSetting/Index",
                        Level =3,
                        Name = "管理数据集",
                        Operator = "admin",
                        ParentID = null,
                        SortNumber = 0,
                         Authority=(long)(Authority.Edit|Authority.Detail|Authority.Delete)
                        },
                         new WebManageMenus{
                        About = "",
                        LinkUrl = "/WebDataSetting/Edit",
                        Level = 3,
                        Name = "编辑数据集",
                        Operator = "admin",
                        ParentID = null,
                        SortNumber = 0,
                        IsDisplayMenuTree=false,
                        Authority=(long)(Authority.Edit|Authority.Detail)
                         }
                         ,
                         new WebManageMenus{
                        About = "",
                        LinkUrl = "/WebDataSetting/Delete",
                        Level = 3,
                        Name = "删除数据集",
                        Operator = "admin",
                        ParentID = null,
                        SortNumber = 0,
                        IsDisplayMenuTree=false,
                        Authority=(long)(Authority.Delete)
                         }
             　　   }
                });

                systemMenu.Sons.Add(new WebManageMenus
                {
                    About = "",
                    LinkUrl = "",
                    Level = 2,
                    Name = "操作日志管理",
                    Operator = "admin",
                    ParentID = null,
                    SortNumber = 0,
                    Sons = new List<WebManageMenus>
                    {
                        new WebManageMenus{
                           About = "",
                           LinkUrl = "/WebLogger/Index",
                           Level = 3,
                           Name = "日志列表",
                           Operator = "admin",
                           ParentID = null,
                           SortNumber = 0,
                           Authority=(long)(Authority.Detail)
                        }
                    }
                });
                menu.Sons.Add(systemMenu);

                systemMenu.Sons.Add(new WebManageMenus
                {
                    About = "",
                    LinkUrl = "",
                    Level = 2,
                    Name = "权限按钮管理",
                    Operator = "admin",
                    ParentID = null,
                    SortNumber = 0,
                    Sons = new List<WebManageMenus>
                    {
                        new WebManageMenus{
                           About = "",
                           LinkUrl = "/WebAuthorityCommand/Index",
                           Level = 3,
                           Name = "权限按钮列表",
                           Operator = "admin",
                           ParentID = null,
                           SortNumber = 0,
                           Authority=(long)(Authority.Create|Authority.Detail)
                        },
                        new WebManageMenus{
                           About = "",
                           LinkUrl = "/WebAuthorityCommand/Create",
                           Level = 3,
                           Name = "添加权限按钮",
                           Operator = "admin",
                           ParentID = null,
                           SortNumber = 1,
                           Authority=(long)(Authority.Edit|Authority.Detail|Authority.Delete)
                        },
                         new WebManageMenus{
                           About = "",
                           LinkUrl = "/WebAuthorityCommand/Edit",
                           Level = 3,
                           Name = "编辑权限按钮",
                           Operator = "admin",
                           ParentID = null,
                           SortNumber = 1,
                           IsDisplayMenuTree=false,
                           Authority=(long)(Authority.Edit|Authority.Detail)
                        },
                        new WebManageMenus{
                           About = "",
                           LinkUrl = "/WebAuthorityCommand/AuthorityAction",
                           Level = 3,
                           Name = "权限页面管理",
                           Operator = "admin",
                           ParentID = null,
                           SortNumber = 1,
                        }
                    }
                });
                menu.Sons.Add(systemMenu);

                systemMenu.Sons.Add(new WebManageMenus
                {
                    About = "",
                    LinkUrl = "",
                    Level = 2,
                    Name = "拥有者(数据建立者)管理",
                    Operator = "admin",
                    ParentID = null,
                    SortNumber = 0,
                    Sons = new List<WebManageMenus>
                    {
                        new WebManageMenus{
                           About = "",
                           LinkUrl = "/Owner/Index",
                           Level = 3,
                           Name = "拥有者列表",
                           Operator = "admin",
                           ParentID = null,
                           SortNumber = 0,
                           Authority=(long)(Authority.Edit|Authority.Detail)
                        },
                        new WebManageMenus{
                           About = "",
                           LinkUrl = "/Owner/Edit",
                           Level = 3,
                           Name = "编辑拥有者",
                           Operator = "admin",
                           ParentID = null,
                           SortNumber = 1,
                           IsDisplayMenuTree=false,
                           Authority=(long)(Authority.Edit|Authority.Detail)
                        }
                    }
                });
                menu.Sons.Add(systemMenu);
                #endregion


                context.WebManageMenus.Add(menu);
                context.SaveChanges();
                #endregion

                #region 角色表
                var role = new WebManageRoles
                {
                    About = "",
                    DepartmentID = department.Sons.FirstOrDefault().Id,
                    Operator = "admin",
                    //OperatorAuthority = 1 | 2 | 4 | 8,
                    RoleName = "管理员",
                    SortNumber = 0,
                };
                context.WebManageRoles.Add(role);
                context.SaveChanges();
                #endregion

                #region 角色与菜单授权
                List<WebManageRoles_WebManageMenus_Authority_R> webManageRoles_WebManageMenus_Authority_R = new List<WebManageRoles_WebManageMenus_Authority_R>();
                context.WebManageMenus.ToList().ForEach(item =>
                {
                    webManageRoles_WebManageMenus_Authority_R.Add(new WebManageRoles_WebManageMenus_Authority_R
                    {
                        MenuId = item.Id,
                        Authority = 1 | 2 | 4 | 8 | 16 | 32,
                        RoleId = role.Id,
                    });
                });
                context.WebManageRoles_WebManageMenus_Authority_R.AddRange(webManageRoles_WebManageMenus_Authority_R);
                context.SaveChanges();
                #endregion

                #region 用户表
                var user1 = new WebManageUsers
                {

                    Description = "",
                    Email = "test@sina.com",
                    LoginName = "admin",
                    Mobile = "13800138000",
                    Operator = "",
                    Password = Lind.DDD.Utils.Encryptor.Utility.EncryptString("admin", Utils.Encryptor.Utility.EncryptorType.MD5),
                    RealName = "管理员",
                    WebSystemID = 1,
                    ThridUserId = "",
                    DataCreateDateTime = DateTime.Now,
                    DataStatus = Status.Normal,
                    LastModifyTime = DateTime.Now,
                    WebDepartments = new List<WebDepartments> { department },
                    WebManageRoles = new List<WebManageRoles>() { role },
                };
                var it = department.Sons.Where(i => i.Name == "研发部").FirstOrDefault();

                var user2 = new WebManageUsers
                {
                    Description = "",
                    Email = "test2@sina.com",
                    LoginName = "it",
                    Mobile = "13800138000",
                    Operator = "",
                    Password = Lind.DDD.Utils.Encryptor.Utility.EncryptString("it", Utils.Encryptor.Utility.EncryptorType.MD5),
                    RealName = "技术人员",
                    WebSystemID = 1,
                    DataCreateDateTime = DateTime.Now,
                    DataStatus = Status.Normal,
                    LastModifyTime = DateTime.Now,
                    WebDepartments = new List<WebDepartments> { 
                        it.Sons.FirstOrDefault(i => i.Name == "开发部"),
                        it.Sons.FirstOrDefault(i => i.Name == "设计部")
                    },
                    WebManageRoles = new List<WebManageRoles>() { role },
                };
                context.WebManageUsers.AddRange(new List<WebManageUsers> { user1, user2 });
                context.SaveChanges();

                #endregion

                base.Seed(context);
            }
            catch (DbEntityValidationException db)
            {
                throw db;
            }
            catch (Exception ex)
            {

                throw ex;
            }


        }
    }
}
