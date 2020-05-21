using Lind.DDD.Manager.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Lind.DDD.Manager.Controllers
{
    public class WebLoggerController : BaseController
    {
        public ActionResult Index(string controllerName, DateTime? startDate, DateTime? endDate, int page = 1)
        {
            var linq = webLoggerRepository.GetModel();
            if (!string.IsNullOrWhiteSpace(controllerName))
            {
                linq = linq.Where(i => i.ControllerName == controllerName);
            }
            if (startDate.HasValue)
            {
                linq = linq.Where(i => i.DataCreateDateTime >= startDate);
            }
            if (endDate.HasValue)
            {
                var dt = endDate.Value.AddDays(1);
                var _end = new DateTime(dt.Year, dt.Month, dt.Day, 0, 0, 0);

                linq = linq.Where(i => i.DataCreateDateTime < _end);
            }
            ViewBag.TotalRecord = linq.Count();
            var model = linq.OrderByDescending(i => i.Id)
                            .ToPagedList(new Paging.PageParameters(page, 10));
            return View(model);
        }

        public PartialViewResult Details(int id)
        {
            return PartialView(webLoggerRepository.GetModel().FirstOrDefault(i => i.Id == id));
        }


        //
        // GET: /WebLogger/Delete/5

        public ActionResult Delete(int id)
        {
            webLoggerRepository.Delete(new WebLogger { Id = id });
            return RedirectToAction("Index");
        }
    }
}
