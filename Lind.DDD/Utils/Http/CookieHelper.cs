using System.Web;
using System;
using System.Collections.Specialized;
using System.Collections.Generic;

namespace Lind.DDD.Utils.Http
{

    /// <summary>
    /// Cookie 操作帮助类
    /// </summary>
    public class CookieHelper
    {

        #region 写cookie值
        /// <summary>
        /// 写cookie值
        /// </summary>
        /// <param name="strName">名称</param>
        /// <param name="strValue">值</param>
        public static void Write(string strName, string strValue)
        {
            HttpCookie cookie = HttpContext.Current.Request.Cookies[strName];
            if (cookie == null)
            {
                cookie = new HttpCookie(strName);
            }
            cookie.Value = strValue;
            HttpContext.Current.Response.AppendCookie(cookie);

        }
        /// <summary>
        /// 将复杂对象写cookies
        /// </summary>
        /// <param name="strName"></param>
        /// <param name="value"></param>
        public static void Write<T>(string strName, T value)
        {
            Write(strName, Utils.SerializeMemoryHelper.SerializeToJson<T>(value));
        }
        /// <summary>
        /// 写cookie值
        /// </summary>
        /// <param name="strName">名称</param>
        /// <param name="strValue">值</param>
        /// <param name="doMain">域  例如:contoso.com</param>
        public static void WriteWithDomain(string strName, string strValue, string doMain)
        {
            HttpCookie cookie = HttpContext.Current.Request.Cookies[strName];
            if (cookie == null)
            {
                cookie = new HttpCookie(strName);
            }
            cookie.Value = strValue;
            cookie.Domain = doMain;
            HttpContext.Current.Response.AppendCookie(cookie);

        }

        /// <summary>
        /// 写cookie值
        /// </summary>
        /// <param name="strName">名称</param>
        /// <param name="key">键名</param>
        /// <param name="strValue">值</param>
        public static void Write(string strName, string key, string strValue)
        {
            HttpCookie cookie = HttpContext.Current.Request.Cookies[strName];
            if (cookie == null)
            {
                cookie = new HttpCookie(strName);
            }
            cookie[key] = strValue;
            HttpContext.Current.Response.AppendCookie(cookie);

        }
        /// <summary>
        /// 写cookie值
        /// </summary>
        /// <param name="strName">名称</param>
        /// <param name="key">键名</param>
        /// <param name="strValue">值</param>
        /// <param name="doMain">域  例如:contoso.com</param>
        public static void WriteWithDomain(string strName, string key, string strValue, string doMain)
        {
            HttpCookie cookie = HttpContext.Current.Request.Cookies[strName];
            if (cookie == null)
            {
                cookie = new HttpCookie(strName);
            }
            cookie.Domain = doMain;
            cookie[key] = strValue;
            HttpContext.Current.Response.AppendCookie(cookie);

        }

        /// <summary>
        /// 写cookie值
        /// </summary>
        /// <param name="strName">名称</param>
        /// <param name="strValue">值</param>
        /// <param name="strValue">过期时间(分钟)</param>
        public static void Write(string strName, string strValue, int expires)
        {
            HttpCookie cookie = HttpContext.Current.Request.Cookies[strName];
            if (cookie == null)
            {
                cookie = new HttpCookie(strName);
            }

            cookie.Value = strValue;
            cookie.Expires = System.DateTime.Now.AddMinutes(expires);
            HttpContext.Current.Response.AppendCookie(cookie);
        }
        /// <summary>
        /// 写cookie值
        /// </summary>
        /// <param name="strName">名称</param>
        /// <param name="strValue">值</param>
        /// <param name="strValue">过期时间(分钟)</param>
        /// <param name="doMain">域  例如:contoso.com</param>
        public static void WriteWithDomain(string strName, string strValue, int expires, string doMain)
        {
            HttpCookie cookie = HttpContext.Current.Request.Cookies[strName];
            if (cookie == null)
            {
                cookie = new HttpCookie(strName);
            }
            cookie.Domain = doMain;
            cookie.Value = strValue;
            cookie.Expires = System.DateTime.Now.AddMinutes(expires);
            HttpContext.Current.Response.AppendCookie(cookie);
        }
        /// <summary>
        /// 写cookie值
        /// </summary>
        /// <param name="strName">名称</param>
        /// <param name="key">键名</param>
        /// <param name="strValue">值</param>
        /// <param name="expires">过期时间(分钟)</param>
        public static void Write(string strName, string key, string strValue, int expires)
        {
            HttpCookie cookie = HttpContext.Current.Request.Cookies[strName];
            if (cookie == null)
            {
                cookie = new HttpCookie(strName);
            }
            cookie[key] = strValue;
            cookie.Expires = System.DateTime.Now.AddMinutes(expires);
            HttpContext.Current.Response.AppendCookie(cookie);
        }
        /// <summary>
        /// 写cookie值
        /// </summary>
        /// <param name="strName">名称</param>
        /// <param name="key">键名</param>
        /// <param name="strValue">值</param>
        /// <param name="expires">过期时间(分钟)</param>
        /// <param name="doMain">域  例如:contoso.com</param>
        public static void WriteWithDomain(string strName, string key, string strValue, int expires, string doMain)
        {
            HttpCookie cookie = HttpContext.Current.Request.Cookies[strName];
            if (cookie == null)
            {
                cookie = new HttpCookie(strName);
            }
            cookie.Domain = doMain;
            cookie[key] = strValue;
            cookie.Expires = System.DateTime.Now.AddMinutes(expires);
            HttpContext.Current.Response.AppendCookie(cookie);
        }

        #endregion

        #region 读cookie值
        /// <summary>
        /// 读cookies值并转为对象
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="strName"></param>
        /// <returns></returns>
        public static T Read<T>(string strName)
        {
            return Utils.SerializeMemoryHelper.DeserializeFromJson<T>(Read(strName));
        }
        /// <summary>
        /// 读cookie值
        /// </summary>
        /// <param name="strName">名称</param>
        /// <returns>cookie值</returns>
        public static string Read(string strName)
        {
            if (HttpContext.Current.Request.Cookies != null
                && HttpContext.Current.Request.Cookies[strName] != null)
            {
                return HttpContext.Current.Request.Cookies[strName].Value;
            }
            else
            {
                return string.Empty;
            }
        }
        /// <summary>
        /// 读cookie值
        /// </summary>
        /// <param name="strName">名称</param>
        /// <param name="key">键名</param>
        /// <returns>cookie值</returns>
        public static string Read(string strName, string key)
        {
            if (HttpContext.Current.Request.Cookies != null
                && HttpContext.Current.Request.Cookies[strName] != null)
            {
                return HttpContext.Current.Request.Cookies[strName][key];
            }
            else
            {
                return string.Empty;
            }
        }

        #endregion

        #region Cookie 删除
        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="name">名称</param>
        public static void Remove(string name)
        {
            if (HttpContext.Current.Request.Cookies != null
                && HttpContext.Current.Request.Cookies[name] != null)
            {
                HttpCookie myCookie = new HttpCookie(name);
                myCookie.Expires = DateTime.Now.AddMinutes(-1);
                HttpContext.Current.Response.Cookies.Add(myCookie);
            }
        }

        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="name">名称</param>
        /// <param name="key">二级建名称</param>
        public static void Remove(string name, string key)
        {
            if (HttpContext.Current.Request.Cookies != null
                && HttpContext.Current.Request.Cookies[name] != null
                && !string.IsNullOrEmpty(HttpContext.Current.Request.Cookies[name][key]))
            {
                string[] temp = HttpContext.Current.Request.Cookies[name].Value.Split(new char[] { '&' }, StringSplitOptions.RemoveEmptyEntries);
                List<string> list = new List<string>();
                foreach (string item in temp)
                {
                    if (item.StartsWith(key))
                    {
                        continue;
                    }
                    else
                    {
                        list.Add(item);
                    }
                }
                Write(name, string.Join("&", list.ToArray()));
            }
        }
        #endregion
    }
}
