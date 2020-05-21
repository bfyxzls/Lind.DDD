using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Lind.DDD.Api.Controllers
{
    public class AboutController : Controller
    {
        //
        // GET: /About/
        public ActionResult MD5()
        {
            return View();
        }

        //
        // GET: /About/Details/5
        [HttpPost]
        public ActionResult KV()
        {
            return View();
        }

        //
        // GET: /About/Create
        public ActionResult HttpHelper()
        {
            return View();
        }
        public ActionResult Cors()
        {
            return View();
        }
        public ActionResult Authority()
        {
            return View();
        }


    }
}
