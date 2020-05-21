using Lind.DDD.Domain;
using Lind.DDD.Filters;
using Lind.DDD.Manager.Filters;
using Lind.DDD.Manager.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Transactions;
using System.Web.Mvc;
using System.Web.Mvc.Html;

namespace Lind.DDD.Manager.Controllers
{
    public class WebAuthorityCommandController : BaseController
    {
        public ActionResult Index(int page = 1)
        {
            var linq = webAuthorityCommandsRepository.GetModel();
            ViewBag.TotalRecord = linq.Count();
            var model = linq.OrderByDescending(i => i.Id).ToPagedList(new Paging.PageParameters(page, 10));
            foreach (var i in model)
            {
                var list = webManageRoles_WebManageMenus_Authority_RRepository.GetModel(j => (j.Authority & i.Flag) == i.Flag);
                i.IsUsed = (list != null && list.Any());
            }
            return View(model);
        }
        public PartialViewResult Details(int id)
        {
            return PartialView(webAuthorityCommandsRepository.Find(id));
        }
        public ActionResult Create()
        {
            return View();
        }

        /// <summary>
        /// 从位集合中找到空位
        /// </summary>
        /// <param name="max"></param>
        /// <param name="he"></param>
        /// <returns>0表示没有空位</returns>
        long GetValidNumber(long he)
        {
            for (long i = 1; i < he; i = i << 1)
            {
                if ((he & i) != i)
                    return i;
            }
            return 0;
        }
        [HttpPost]
        public ActionResult Create(WebAuthorityCommands entity)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    if (webAuthorityCommandsRepository.Find(i => i.Name.ToLower() == entity.Name.ToLower()) != null)
                    {
                        ModelState.AddModelError("", "这个按钮按钮已经在数据库里存在，请重新填写！");
                        return View(entity);
                    }

                    List<WebAuthorityCommands> list = webAuthorityCommandsRepository.GetModel().ToList();
                    var total = list.BinaryOr(i => i.Flag);
                    var valid = GetValidNumber(total);
                    entity.Flag = valid;
                    if (valid == 0)
                    {
                        valid = webAuthorityCommandsRepository.GetModel().Max(i => i.Flag);
                        entity.Flag = valid << 1;
                    }
                    webAuthorityCommandsRepository.Insert(entity);
                    MvcExtensions.ReloadWebAuthorityCommands();
                    return RedirectToAction("Index");
                }
                else
                {
                    ModelState.AddModelError("", "请认真填写表单！");
                    return View(entity);
                }
            }
            catch (Exception)
            {
                ModelState.AddModelError("", "数据处理出错，请重新提交！");
                return View(entity);
            }
        }
        public ActionResult Edit(int id)
        {
            return View(webAuthorityCommandsRepository.Find(id));
        }
        [HttpPost]
        public ActionResult Edit(int id, WebAuthorityCommands entity)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var old = webAuthorityCommandsRepository.Find(id);
                    old.ActionName = entity.ActionName;
                    old.ClassName = entity.ClassName;
                    old.Name = entity.Name;
                    old.Feature = entity.Feature;
                    old.LastModifyTime = DateTime.Now;
                    webAuthorityCommandsRepository.Update(old);
                    MvcExtensions.ReloadWebAuthorityCommands();
                    return RedirectToAction("Index");
                }
                else
                {
                    ModelState.AddModelError("", "请认真填写表单！");
                    return View(entity);
                }
            }
            catch (Exception)
            {
                ModelState.AddModelError("", "数据处理出错，请重新提交！");
                return View(entity);
            }

        }

        /// <summary>
        /// 删除按钮
        /// 注意：当按钮已经被分配，则不提供删除
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult Delete(int id)
        {
            try
            {
                var entity = webAuthorityCommandsRepository.Find(id);
                var list = webManageRoles_WebManageMenus_Authority_RRepository.GetModel().Where(i => (i.Authority & entity.Flag) == entity.Flag);
                if (list == null || list.Count() == 0)
                {
                    //删除主表
                    webAuthorityCommandsRepository.Delete(entity);
                    MvcExtensions.ReloadWebAuthorityCommands();
                    return RedirectToAction("Index");
                }
                else
                {
                    return Content("<script>alert('本功能按钮【已被使用】，不能进行删除！');location.href='/WebAuthorityCommand/Index';</script>");
                }
            }
            catch (Exception)
            {
                return RedirectToAction("Index");
            }

        }

        static IEnumerable<Type> modelList;
        static WebAuthorityCommandController()
        {
            var dll = AppDomain.CurrentDomain.GetAssemblies().Where(i => !i.GetName().Name.Contains("System")
                && !i.GetName().Name.Contains("Redis")
                && i.GetName().Name != "Lind.DDD");
            modelList = dll.SelectMany(a => GetLoadableTypes(a)).Where(i => i.GetInterfaces().Contains(typeof(IController)));

        }
        static IEnumerable<Type> GetLoadableTypes(Assembly assembly)
        {

            try
            {
                return assembly.GetTypes();
            }
            catch (ReflectionTypeLoadException e)
            {
                return e.Types.Where(t => t != null);
            }
            catch (Exception ex)
            {
                Lind.DDD.Logger.LoggerFactory.Instance.Logger_Error(ex);
                return null;
            }
        }


        #region 授权Action列表
        void FillAuthorityAction(List<Tuple<string, string>> model)
        {
            foreach (var item in modelList)
            {
                var methods = item.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance)
                    .Where(i => i.GetCustomAttributes(typeof(ActionAuthorityAttribute), false).Count() > 0);
                if (methods != null && methods.Count() > 0)
                {
                    foreach (var method in methods)
                    {
                        string name = "<span style='color:red'>未设置</span>";
                        if (method.GetCustomAttributes(typeof(ManagerActionLoggerAttribute), false).Count() > 0)
                            name = method.GetCustomAttributes(typeof(ManagerActionLoggerAttribute), false).FirstOrDefault().ToString();
                        model.Add(new Tuple<string, string>(name, "/" + item.Name.Substring(0, item.Name.Length - 10) + "/" + method.Name));
                    }
                }
            }
        }
        /// <summary>
        /// 授权页面列表
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public ActionResult AuthorityAction(int key = 0)
        {
            var menuList = new ManagerEfRepository<WebManageMenus>().GetModel().ToList();

            List<Tuple<string, string>> model = new List<Tuple<string, string>>();
            FillAuthorityAction(model);
            //查询需要配置的菜单
            if (key == 1)
            {
                model = model.Where(i => !menuList.Select(j => j.LinkUrl).Contains(i.Item2)).ToList();
            }
            return View(model);

        }
        /// <summary>
        /// 填充缺失数据表的菜单
        /// </summary>
        /// <returns></returns>
        public ActionResult DoAuthorityAction()
        {
            var menuList = new ManagerEfRepository<WebManageMenus>().GetModel().ToList();

            List<Tuple<string, string>> model = new List<Tuple<string, string>>();
            FillAuthorityAction(model);
            model = model.Where(i => !menuList.Select(j => j.LinkUrl).Contains(i.Item2)).ToList();
            var menu = menuRepository.Find(i => i.Name == "未分配的授权页面");
            if (menu == null)
            {
                var root = menuRepository.Find(i => !i.ParentID.HasValue);
                if (root == null)
                    throw new ArgumentException("没有根节点!或者根节点的ParentID不是null");
                menu = new WebManageMenus
                {
                    About = "",
                    IsDisplayMenuTree = false,
                    Level = 1,
                    LinkUrl = "",
                    Name = "未分配的授权页面",
                    Operator = Lind.DDD.Authorization.CurrentUser.UserName,
                    ParentID = root.Id,
                    SortNumber = 0
                };
                menuRepository.Insert(menu);
            }
            List<WebManageMenus> newMenuList = new List<WebManageMenus>();

            model.ForEach(i =>
            {
                newMenuList.Add(new WebManageMenus
                {
                    About = "",
                    IsDisplayMenuTree = false,
                    Level = menu.Level + 1,
                    LinkUrl = i.Item2,
                    Name = i.Item1,
                    Operator = Lind.DDD.Authorization.CurrentUser.UserName,
                    ParentID = menu.Id,
                    SortNumber = 0
                });
            });
            menuRepository.Insert(newMenuList);
            return RedirectToAction("AuthorityAction", new { key = 1 });
        }
        #endregion

    }
}

