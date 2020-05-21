using Lind.DDD.Authorization;
using Lind.DDD.Filters;
using Lind.DDD.Manager.Filters;
using Lind.DDD.Manager.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Lind.DDD.Manager.Controllers
{
    /// <summary>
    /// 数据集控制
    /// </summary>
    public class WebDataCtrlController : BaseController
    {
        [ActionAuthority(Authority.Detail), ManagerActionLoggerAttribute("数据集类型列表")]
        public ActionResult Index(int page = 1)
        {
            var model = webDataCtrlRepository.GetModel()
                                             .OrderByDescending(i => i.Id)
                                             .ToPagedList(new Paging.PageParameters(page, 10));

            int totalRecord = Convert.ToInt32(ViewBag.totalRecord);

            return View(model);
        }


        public PartialViewResult Details(int id)
        {
            return PartialView(webDataCtrlRepository.Find(id));
        }

        public ActionResult Create()
        {
            return View();
        }

        [ActionAuthority(Authority.Create), ManagerActionLoggerAttribute("添加数据集类型")]
        [HttpPost]
        public ActionResult Create(WebDataCtrl entity)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    webDataCtrlRepository.Insert(entity);
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
            return View(webDataCtrlRepository.Find(id));
        }


        [ActionAuthority(Authority.Edit), ManagerActionLoggerAttribute("编辑数据集类型")]
        [HttpPost]
        public ActionResult Edit(int id, WebDataCtrl entity)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var old = webDataCtrlRepository.Find(id);
                    old.DataCtrlApi = entity.DataCtrlApi;
                    old.DataCtrlField = entity.DataCtrlField;
                    old.DataCtrlType = entity.DataCtrlType;
                    webDataCtrlRepository.Update(old);

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


        [ActionAuthority(Authority.Delete), ManagerActionLoggerAttribute("删除数据集类型")]
        public ActionResult Delete(int id)
        {
            var old = webDataCtrlRepository.Find(id);
            old.DataStatus = Lind.DDD.Domain.Status.Deleted;
            webDataCtrlRepository.Update(old);
            return RedirectToAction("Index");
        }
    }
}
