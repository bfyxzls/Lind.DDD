using Lind.DDD.Authorization;
using Lind.DDD.Manager.Filters;
using Lind.DDD.Manager.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Lind.DDD.Filters;

namespace Lind.DDD.Manager.Controllers
{
    public class WebMenuController : BaseController
    {
        [ActionAuthority(Authority.Detail), ManagerActionLoggerAttribute("菜单列表")]
        public ActionResult Index()
        {
            return View(menuRepository.GetModel().ToList());
        }

        public ActionResult Details(int id)
        {
            return View();
        }

        public ActionResult Create(int id = 1)
        {
            var entity = menuRepository.Find(id);
            return View(new WebManageMenus { ParentID = entity.Id, Level = entity.Level + 1 });
        }

        [ActionAuthority(Authority.Create), ManagerActionLoggerAttribute("添加菜单")]
        [HttpPost]
        public ActionResult Create(WebManageMenus entity)
        {
            try
            {
                var oldlist = menuRepository.GetModel(i => i.ParentID == entity.ParentID);
                if (oldlist.FirstOrDefault(i => i.Name == entity.Name) != null)
                {
                    ModelState.AddModelError("", "同级菜单不能重名...");
                    return View();
                }
                ModelState.Remove("IsDisplayMenuTree");
                if (ModelState.IsValid)
                {
                    //菜单上的操作按钮
                    var menu_authority = Array.ConvertAll(Request.Form.GetValues("Authority"), i => Convert.ToInt64(i));
                    entity.Id = -1;//自关联表要主动赋值
                    entity.Authority = menu_authority.BinaryOr(i => i);
                    entity.About = "";
                    entity.Operator = "";
                    entity.IsDisplayMenuTree = Request["IsDisplayMenuTree"] == "1";
                    menuRepository.Insert(entity);
                    return RedirectToAction("Index");
                }
                else
                {
                    ModelState.AddModelError("", "请把表单填写完整...");
                    return View();
                }


            }
            catch
            {
                return View();
            }
        }

        public ActionResult Edit(int id)
        {
            return View(menuRepository.Find(id));

        }

        [ActionAuthority(Authority.Edit), ManagerActionLoggerAttribute("编辑菜单")]
        [HttpPost]
        public ActionResult Edit(int id, WebManageMenus entity)
        {
            try
            {
                var oldlist = menuRepository.GetModel(i => i.ParentID == entity.ParentID);
                if (oldlist.FirstOrDefault(i => i.Name == entity.Name && i.Id != entity.Id) != null)
                {
                    ModelState.AddModelError("", "同级菜单不能重名...");
                    return View();
                }

                ModelState.Remove("IsDisplayMenuTree");
                if (ModelState.IsValid)
                {
                    var old = menuRepository.Find(id);
                    var menu_authority = Array.ConvertAll(Request.Form.GetValues("Authority"), i => Convert.ToInt64(i));
                    old.Authority = menu_authority.BinaryOr(i => i);
                    old.LinkUrl = entity.LinkUrl;
                    old.Level = entity.Level;
                    old.IsDisplayMenuTree = Request["IsDisplayMenuTree"] == "1";
                    old.SortNumber = entity.SortNumber;
                    old.Name = entity.Name;
                    menuRepository.Update(old);
                    return RedirectToAction("Index");
                }
                else
                {
                    ModelState.AddModelError("", "请把表单填写完整...");
                    return View();
                }

            }
            catch
            {
                return View();
            }
        }

        [ActionAuthority(Authority.Delete), ManagerActionLoggerAttribute("删除菜单")]
        public ActionResult Delete(int id)
        {
            var del = menuRepository.Find(id);
            var delList = new TreeHelper.DataTree<WebManageMenus>(
                menuRepository.GetModel().ToList()).GetDeleteTree(del);
            foreach (var item in delList)
            {
                item.DataStatus = Lind.DDD.Domain.Status.Deleted;
                menuRepository.Update(item);
            }
            return RedirectToAction("Index");
        }

    }
}
