using Lind.DDD.Authorization.Api;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Lind.DDD.Web.Controllers
{
    [AllowAnonymous]
    public class OAuthController : Controller
    {
        public ActionResult Test(string appKey = "zzl")
        {
            string token = new OAuthClient().GetRequestToken(appKey);
            ViewBag.requestToken = token;
            if (!string.IsNullOrWhiteSpace(token))
                ViewBag.accessToken = new OAuthClient().GetAccessToken();
            return View();
        }

        OAuthApi api = new OAuthApi();
        public void GetRequestToken()
        {
            api.GetRequestToken();
        }

        public void GetAccessToken()
        {
            api.GetAccessToken();
        }
        [OAuthFilter]
        public ActionResult Index()
        {
            return View();
        }
    }
}
