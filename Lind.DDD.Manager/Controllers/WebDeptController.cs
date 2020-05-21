using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Lind.DDD.Manager.Models;
using Lind.DDD.Manager.Filters;
using Lind.DDD.Authorization;
using Lind.DDD.Filters;

namespace Lind.DDD.Manager.Controllers
{
    public class WebDeptController : BaseController
    {
        [ActionAuthority(Authority.Detail), ManagerActionLoggerAttribute("部门列表")]
        public ActionResult Index(int page = 1)
        {
            return View(departmentsRepository.GetModel()
                                             .Include(i => i.Father)
                                             .OrderBy(i => i.Id)
                                             .ToPagedList(new Paging.PageParameters(page, 5)));
        }

        public ActionResult Details(int id)
        {
            return View();
        }

        public ActionResult Create(int id = 1)
        {
            var entity = menuRepository.Find(id);

            return View(new WebDepartments { ParentID = entity.Id, Level = entity.Level + 1 });
        }

        [ActionAuthority(Authority.Create), ManagerActionLoggerAttribute("添加部门")]
        [HttpPost]
        public ActionResult Create(WebDepartments entity)
        {
            try
            {
                var oldlist = departmentsRepository.GetModel(i => i.ParentID == entity.ParentID);
                if (oldlist.FirstOrDefault(i => i.Name == entity.Name) != null)
                {
                    ModelState.AddModelError("", "同级不能重名...");
                    return View();

                }
                if (ModelState.IsValid)
                {
                    entity.Id = -1;
                    entity.Operator = "";
                    entity.About = "";
                    departmentsRepository.Insert(entity);
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
            return View(departmentsRepository.Find(id));
        }

        [ActionAuthority(Authority.Edit), ManagerActionLoggerAttribute("编辑部门")]
        [HttpPost]
        public ActionResult Edit(int id, WebDepartments entity)
        {
            try
            {
                var oldlist = departmentsRepository.GetModel(i => i.ParentID == entity.ParentID);
                if (oldlist.FirstOrDefault(i => i.Name == entity.Name && i.Id != entity.Id) != null)
                {
                    ModelState.AddModelError("", "同级不能重名...");
                    return View();

                }

                if (ModelState.IsValid)
                {
                    var old = departmentsRepository.Find(id);
                    old.Name = entity.Name;
                    old.Level = entity.Level;
                    old.SortNumber = entity.SortNumber;
                    old.LinkUrl = entity.LinkUrl;
                    departmentsRepository.Update(old);
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

        [ActionAuthority(Authority.Delete), ManagerActionLoggerAttribute("删除部门")]
        public ActionResult Delete(int id)
        {
            var del = departmentsRepository.Find(id);
            var delList = new TreeHelper.DataTree<WebDepartments>(departmentsRepository.GetModel().ToList()).GetDeleteTree(del);
            foreach (var item in delList)
            {
                item.DataStatus = Lind.DDD.Domain.Status.Deleted;
                departmentsRepository.Delete(item);
            }

            return RedirectToAction("Index");
        }

    }
}
