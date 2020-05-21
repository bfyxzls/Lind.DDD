using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace Lind.DDD.SSO
{
    /// <summary>
    /// 关于SSO单点登陆的实现网站需要使用它
    /// 验证过程：获取凭证－＞通过凭证获令牌－＞通过令牌获资源
    /// </summary>
    public class SSOActionFilter : System.Web.Mvc.ActionFilterAttribute
    {

        #region Constructors & Fields

        /// <summary>
        /// 当前上下文服务对象
        /// </summary>
        private readonly HttpServerUtility Server = System.Web.HttpContext.Current.Server;
        /// <summary>
        /// SSO域名地址
        /// </summary>
        private string ssoUri;
        /// <summary>
        /// SSO统一登陆Uri
        /// </summary>
        private string passPortUri;
        /// <summary>
        /// 获取用户凭证的Uri
        /// </summary>
        private string getCredenceUri;
        /// <summary>
        /// 获取令牌Uri
        /// </summary>
        private string getTokenUri;
        /// <summary>
        /// 服务端存储的Cache的key
        /// </summary>
        readonly string TOKEN_KEY = Lind.DDD.ConfigConstants.ConfigManager.Config.SSO.TokenKey ?? "SSOToken";

        public SSOActionFilter()
            : this(Lind.DDD.ConfigConstants.ConfigManager.Config.SSO.Domain, null)
        { }
        /// <summary>
        ///  SSO客户端特性初始化
        /// </summary>
        /// <param name="ssoUri">sso服务端域名</param>
        /// <param name="ssoLoginAction">sso的统一登录地址/controller/action</param>
        public SSOActionFilter(string ssoUri, string ssoLoginAction = null)
        {
            this.passPortUri = ssoUri + (ssoLoginAction ?? "/Common/Login?BackURL=");
            this.getCredenceUri = ssoUri + "/SSOCore/TokenGetCredence?tokenValue=";
            this.getTokenUri = ssoUri + "/SSOCore/GetTokenUrl?BackURL=";
        }
        #endregion

        /// <summary>
        /// 进行Action之前进行校验
        /// </summary>
        /// <param name="filterContext"></param>
        public override void OnActionExecuting(System.Web.Mvc.ActionExecutingContext filterContext)
        {

            var Request = filterContext.HttpContext.Request;
            var Response = filterContext.HttpContext.Response;
            var Session = filterContext.HttpContext.Session;
            Session.Timeout = 30;

            //令牌存储在第三方Session,退出只退自己平台的账号
            if (Session[TOKEN_KEY] != null)
            {
                //分站凭证存在
                //恭喜，分站凭证存在，您被授权访问该页面！
                Lind.DDD.Logger.LoggerFactory.Instance.Logger_Debug("恭喜，分站凭证存在，您被授权访问该页面！");
            }
            else
            {
                //令牌验证结果
                if (Request.QueryString[TOKEN_KEY] != null)
                {
                    if (Request.QueryString[TOKEN_KEY] != "$Token$")
                    {
                        //持有令牌
                        string tokenValue = Request.QueryString[TOKEN_KEY];

                        //调用WebService获取主站凭证[3]
                        var o = new WebClient().DownloadString(getCredenceUri + tokenValue);
                        if (!string.IsNullOrWhiteSpace(o))
                        {
                            //令牌正确[5,结束]
                            Session[TOKEN_KEY] = o;
                            //序列化用户信息
                            var obj = o.Split(new char[] { '&' }, StringSplitOptions.RemoveEmptyEntries);
                            Lind.DDD.Authorization.CurrentUser.Serialize(obj[0], obj[1]);
                            //恭喜，令牌存在，您被授权访问该页面！
                            Lind.DDD.Logger.LoggerFactory.Instance.Logger_Debug("恭喜，令牌存在，您被授权访问该页面！");
                        }
                        else
                        {
                            //令牌错误[4]
                            filterContext.Result = new RedirectResult(this.replaceToken());
                        }
                    }
                    else
                    {
                        //未持有令牌[2],获取令牌
                        filterContext.Result = new RedirectResult(this.replaceToken());
                    }
                }
                //没能领取令牌，去主站领取[1]$Token$
                else
                {
                    filterContext.Result = new RedirectResult(this.getTokenURL());
                }
            }
            base.OnActionExecuting(filterContext);
        }
        /// <summary>
        /// 获取带令牌请求的URL
        /// 在当前URL中附加上令牌请求参数
        /// </summary>
        /// <returns></returns>
        private string getTokenURL()
        {
            string url = System.Web.HttpContext.Current.Request.Url.AbsoluteUri;
            Regex reg = new Regex(@"^.*\?.+=.+$");
            if (reg.IsMatch(url))
                url += "&" + TOKEN_KEY + "=$Token$";
            else
                url += "?" + TOKEN_KEY + "=$Token$";
            return getTokenUri + Server.UrlEncode(url);
        }
        /// <summary>
        /// 去掉URL中的凭证
        /// 在当前URL中去掉凭证参数
        /// </summary>
        /// <returns></returns>
        private string replaceToken()
        {
            string url = System.Web.HttpContext.Current.Request.Url.AbsoluteUri;
            url = Regex.Replace(url, @"(\?|&)" + TOKEN_KEY + "=.*", "", RegexOptions.IgnoreCase);
            return passPortUri + Server.UrlEncode(url);
        }
    }
}
