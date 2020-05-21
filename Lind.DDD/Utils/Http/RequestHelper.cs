using System;
using System.Web;
using System.Text.RegularExpressions;
using System.Net;

namespace Lind.DDD.Utils.Http
{
    /// <summary>
    /// Request操作类
    /// </summary>
    public class RequestHelper
    {
        #region 判断当前页面是否接收到了Post请求或者Get请求
        /// <summary>
        /// 判断当前页面是否接收到了Post请求
        /// </summary>
        /// <returns>是否接收到了Post请求</returns>
        public static bool IsPost()
        {
            return HttpContext.Current.Request.HttpMethod.Equals("POST");
        }
        /// <summary>
        /// 判断当前页面是否接收到了Get请求
        /// </summary>
        /// <returns>是否接收到了Get请求</returns>
        public static bool IsGet()
        {
            return HttpContext.Current.Request.HttpMethod.Equals("GET");
        }
        #endregion

        #region 返回上一个页面的地址
        /// <summary>
        /// 返回上一个页面的地址
        /// </summary>
        /// <returns>上一个页面的地址</returns>
        public static string GetUrlReferrer()
        {
            string retVal = null;

            try
            {
                retVal = HttpContext.Current.Request.UrlReferrer.ToString();
            }
            catch { }

            if (retVal == null)
                return "";

            return retVal;

        }
        #endregion

        #region 得到当前完整主机头
        /// <summary>
        /// 得到当前完整主机头
        /// </summary>
        /// <returns></returns>
        public static string GetCurrentFullHost()
        {
            HttpRequest request = System.Web.HttpContext.Current.Request;
            if (!request.Url.IsDefaultPort)
            {
                return string.Format("{0}:{1}", request.Url.Host, request.Url.Port.ToString());
            }
            return request.Url.Host;
        }
        #endregion

        #region 得到主机头
        /// <summary>
        /// 得到主机头
        /// </summary>
        /// <returns></returns>
        public static string GetHost()
        {
            return HttpContext.Current.Request.Url.Host;
        }
        #endregion

        #region 获取当前请求的原始 URL(URL 中域信息之后的部分,包括查询字符串(如果存在))
        /// <summary>
        /// 获取当前请求的原始 URL(URL 中域信息之后的部分,包括查询字符串(如果存在))
        /// </summary>
        /// <returns>原始 URL</returns>
        public static string RawUrl()
        {
            return HttpContext.Current.Request.RawUrl;
        }
        #endregion

        #region 判断当前访问是否来自浏览器软件
        /// <summary>
        /// 判断当前访问是否来自浏览器软件
        /// </summary>
        /// <returns>当前访问是否来自浏览器软件</returns>
        public static bool IsFromBrowser()
        {
            string[] BrowserName = { "ie", "opera", "netscape", "mozilla", "konqueror", "firefox" };
            string curBrowser = HttpContext.Current.Request.Browser.Type.ToLower();
            for (int i = 0; i < BrowserName.Length; i++)
            {
                if (curBrowser.IndexOf(BrowserName[i]) >= 0)
                {
                    return true;
                }
            }
            return false;
        }
        #endregion

        #region 判断是否来自搜索引擎链接
        /// <summary>
        /// 判断是否来自搜索引擎链接
        /// </summary>
        /// <returns>是否来自搜索引擎链接</returns>
        public static bool IsFromSearchEngines()
        {
            if (HttpContext.Current.Request.UrlReferrer == null)
            {
                return false;
            }
            string[] SearchEngine = { "google", "yahoo", "msn", "baidu", "sogou", "sohu", "sina", "163", "lycos", "tom", "yisou", "iask", "soso", "gougou", "zhongsou" };
            string tmpReferrer = HttpContext.Current.Request.UrlReferrer.ToString().ToLower();
            for (int i = 0; i < SearchEngine.Length; i++)
            {
                if (tmpReferrer.IndexOf(SearchEngine[i]) >= 0)
                {
                    return true;
                }
            }
            return false;
        }
        #endregion

        #region 获得当前完整Url地址
        /// <summary>
        /// 获得当前完整Url地址
        /// </summary>
        /// <returns>当前完整Url地址</returns>
        public static string Url()
        {
            return HttpContext.Current.Request.Url.ToString();
        }
        #endregion

        #region 获得当前页面的名称
        /// <summary>
        /// 获得当前页面的名称
        /// </summary>
        /// <returns>当前页面的名称</returns>
        public static string GetPageName()
        {
            string[] urlArr = HttpContext.Current.Request.Url.AbsolutePath.Split('/');
            return urlArr[urlArr.Length - 1].ToLower();
        }
        #endregion

        #region 返回表单或Url参数的总个数
        /// <summary>
        /// 返回表单或Url参数的总个数
        /// </summary>
        /// <returns></returns>
        public static int ParamCount()
        {
            return HttpContext.Current.Request.Form.Count + HttpContext.Current.Request.QueryString.Count;
        }
        #endregion

        #region 获得指定表单参数的值
        /// <summary>
        /// 获得指定表单参数的值
        /// </summary>
        /// <param name="strName">表单参数</param>
        /// <returns>表单参数的值</returns>
        public static string FormString(string strName)
        {
            if (HttpContext.Current.Request.Form[strName] == null)
            {
                return string.Empty;
            }
            return HttpContext.Current.Request.Form[strName];
        }

        #endregion

        #region 获得指定Url参数的值
        /// <summary>
        /// 获得指定Url参数的值
        /// </summary>
        /// <param name="strName">Url参数</param>
        /// <returns>Url参数的值</returns>
        public static string QueryStrings(string strName)
        {
            if (HttpContext.Current.Request.QueryString[strName] == null)
            {
                return string.Empty;
            }
            return HttpContext.Current.Request.QueryString[strName];
        }
        #endregion

        #region 获得当前页面客户端的IP
        /// <summary>
        /// 获得当前页面客户端的IP
        /// </summary>
        /// <returns>当前页面客户端的IP</returns>
        public static string IP()
        {
            string result = String.Empty;

            result = HttpContext.Current.Request.ServerVariables["HTTP_X_FORWARDED_FOR"];
            if (null == result || result == String.Empty)
            {
                result = HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];
            }

            if (null == result || result == String.Empty)
            {
                result = HttpContext.Current.Request.UserHostAddress;
            }

            if (null == result || result == String.Empty || !ValidatedHelper.IsIP(result))
            {
                return "0.0.0.0";
            }

            return result;

        }
        #endregion

        #region 获取Url文件名
        /// <summary>
        /// 获取Url文件名
        /// </summary>
        /// <param name="url">Url</param>
        /// <returns>Url文件名</returns>
        static public string GetUrlFileNameWithoutExtension(string url)
        {
            return System.IO.Path.GetFileNameWithoutExtension(url.Substring(url.LastIndexOf('/') + 1));
        }
        #endregion

        /// <summary>
        /// 过滤script和iframe
        /// </summary>
        /// <param name="content"></param>
        /// <returns></returns>
        public static string FilterScriptIFrame(string content)
        {
            return Regex.Replace(content, @"(script|frame)([\s|:|>])+", "$1.$2", RegexOptions.IgnoreCase);
        }

        /// <summary>
        /// 杩囨护鑴氭湰
        /// </summary>
        /// <param name="content"></param>
        /// <returns></returns>
        public static string FilterScript(string content)
        {
            if (content == null || content == "")
            {
                return content;
            }
            string regexstr = @"(?i)<script([^>])*>(\w|\W)*</script([^>])*>";//@"<script.*</script>";
            content = Regex.Replace(content, regexstr, string.Empty, RegexOptions.IgnoreCase);
            content = Regex.Replace(content, "<script([^>])*>", string.Empty, RegexOptions.IgnoreCase);
            return Regex.Replace(content, "</script>", string.Empty, RegexOptions.IgnoreCase);
        }

        /// <summary>
        /// 杩囨护妗嗘灦
        /// </summary>
        /// <param name="content"></param>
        /// <returns></returns>
        public static string FilterIFrame(string content)
        {
            if (content == null || content == "")
            {
                return content;
            }
            string regexstr = @"(?i)<iframe([^>])*>(\w|\W)*</iframe([^>])*>";//@"<script.*</script>";
            content = Regex.Replace(content, regexstr, string.Empty, RegexOptions.IgnoreCase);
            content = Regex.Replace(content, "<iframe([^>])*>", string.Empty, RegexOptions.IgnoreCase);
            return Regex.Replace(content, "</iframe>", string.Empty, RegexOptions.IgnoreCase);
        }

        /// <summary>
        /// 判断远程URL文件是否存在，通过判断文件头的方式
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        public static bool IsExistUrlFile(string url)
        {
            try
            {
                //注意：此方法需要引用Msxml2.dll
                //MSXML2.XMLHTTP _xmlhttp = new MSXML2.XMLHTTPClass();
                //_xmlhttp.open("HEAD", url, false, null, null);
                //_xmlhttp.send("");
                //return (_xmlhttp.status == 200);
                return true;
            }
            catch (Exception)
            {

                throw;
            }
         
        }



    }
}
