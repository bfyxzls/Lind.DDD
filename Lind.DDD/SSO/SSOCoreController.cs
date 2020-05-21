using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace Lind.DDD.SSO
{
    /// <summary>
    /// SSO服务端-控制器基类
    /// </summary>
    public class SSOCoreController : Controller
    {
        /// <summary>
        /// SSO产生的token的键名,它被存储到cookies里
        /// </summary>
        readonly string TOKEN_KEY = Lind.DDD.ConfigConstants.ConfigManager.Config.SSO.TokenKey ?? "SSOToken";

        /// <summary>
        /// 验证通过,进行授权
        /// </summary>
        public ActionResult DoCertification(string userId, string userName, string backURL)
        {
            //产生令牌
            string tokenValue = Guid.NewGuid().ToString();
            HttpCookie tokenCookie = new HttpCookie(TOKEN_KEY);
            tokenCookie.Values.Add("Value", tokenValue);
            tokenCookie.Domain = System.Web.HttpContext.Current.Request.Url.Host;//localhost域名只对火狐浏览器其它均不支持
            System.Web.HttpContext.Current.Response.AppendCookie(tokenCookie);

            //产生主站凭证
            string cert = string.Format("{0}&{1}", userId, userName);
            SSOManager.Instance.TokenInsert(tokenValue, cert, DateTime.Now.AddMinutes(10));

            //跳转回分站
            return GetTokenUrl(backURL);
        }

        /// <summary>
        /// 令牌验证
        /// 以URL参数方式返回
        /// </summary>
        /// <param name="backUrl"></param>
        /// <returns></returns>
        public ActionResult GetTokenUrl(string backUrl)
        {
            if (backUrl != null)
            {

                string backURL = System.Web.HttpUtility.UrlDecode(backUrl);

                //获取Cookie
                HttpCookie tokenCookie = System.Web.HttpContext.Current.Request.Cookies[TOKEN_KEY];
                if (tokenCookie != null)
                {
                    backURL = backURL.Replace("$Token$", tokenCookie.Values["Value"].ToString());
                }

                return Redirect(backURL);
            }
            return null;
        }

        /// <summary>
        /// 根据令牌获取用户凭证,Client调用它
        /// </summary>
        /// <param name="tokenValue"></param>
        /// <returns></returns>
        public object TokenGetCredence(string tokenValue)
        {
            object o = null;

            var sso = SSOManager.Instance.GetCacheTable();
            if (sso != null)
            {
                var entity = sso.FirstOrDefault(i => i.Token == tokenValue);
                if (entity != null)
                {
                    o = entity.Certificate;
                }
            }

            return o;
        }

        /// <summary>
        /// 清除凭证,Client调用它
        /// </summary>
        /// <param name="tokenValue"></param>
        public void ClearToken(string tokenValue)
        {
            SSOManager.Instance.DeleteToken(tokenValue);
        }
    }
}
