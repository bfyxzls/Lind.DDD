using Lind.DDD.Logger;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Xml;

namespace Lind.DDD.Authorization.Api
{
    #region OAuth服务端

    /// <summary>
    /// 服务端认证模型
    /// </summary>
    public class OAuthModel
    {
        /// <summary>
        /// 客户项目ID
        /// </summary>
        public string oauth_consumer_key { get; set; }
        /// <summary>
        /// 客户项目私钥（双方约定）
        /// </summary>
        public string oauth_consumer_secret { get; set; }
        /// <summary>
        /// 第一次请求的token
        /// </summary>
        public string oauth_requestToken { get; set; }
        /// <summary>
        /// 第二次请求的token
        /// </summary>
        public string oauth_accessToken { get; set; }
        /// <summary>
        /// 开始授权时的时间戳，通常５分钟过期
        /// </summary>
        public double oauth_timestamp { get; set; }
    }

    /// <summary>
    /// OAuth认证数据库
    /// </summary>
    public class OAuthDb
    {
        public static readonly List<OAuthModel> Db;
        static OAuthDb()
        {
            Db = new List<OAuthModel>();
            Db.Add(new OAuthModel
            {
                oauth_consumer_key = "zzl",
                oauth_consumer_secret = "zzl1983"
            });

            Db.Add(new OAuthModel
            {
                oauth_consumer_key = "zhz",
                oauth_consumer_secret = "zhz2009"
            });
        }
    }

    /// <summary>
    /// OAuth过滤器，要求客户端带上accessToken
    /// </summary>
    public class OAuthFilter : System.Web.Mvc.ActionFilterAttribute
    {
        public override void OnActionExecuting(System.Web.Mvc.ActionExecutingContext filterContext)
        {
            var Request = filterContext.HttpContext.Request;
            var Response = filterContext.HttpContext.Response;

            string accessToken = Request.QueryString["accessToken"];
            //Oauth时间戳，服务器之间差不超时５分钟，URL授权大于５分钟自动实效
            var entity = OAuthDb.Db.FirstOrDefault(i =>
                (DateTime.Now.ToUniversalTime() - DateTime.MinValue).TotalSeconds - i.oauth_timestamp < 5 * 60 &&
                i.oauth_accessToken == accessToken);

            if (entity == null)
            {
                Response.Write("accessToken有问题或者认证超时，请重新认证");
                Response.StatusCode = 401;
                Response.End();
            }
            base.OnActionExecuting(filterContext);
        }
    }

    /// <summary>
    /// 开放的Api
    /// </summary>
    public class OAuthApi
    {

        HttpResponse Response = System.Web.HttpContext.Current.Response;
        HttpRequest Request = System.Web.HttpContext.Current.Request;

        /// <summary>
        /// 获取code,重定向到第三方
        /// 返回地址加上code
        /// </summary>
        public void GetRequestToken()
        {
            var oauth = new OAuthData();
            var appKey = Request.QueryString["appKey"];
            var redirect_uri = Request.QueryString["redirect_uri"];
            var sign = Request.QueryString["sign"];

            var entity = OAuthDb.Db.FirstOrDefault(i => i.oauth_consumer_key == appKey);
            if (entity == null)
            {
                Response.Write("oauth_consumer_key不是有效的！");
                Response.StatusCode = 401;
                Response.End();
                return;
            }
            oauth.SetValue("appKey", appKey);
            oauth.SetValue("redirect_uri", redirect_uri);

            if (Utils.Encryptor.MD5Encryptor.MD5((oauth.ToUrl() + "&key=" + entity.oauth_consumer_secret)).ToUpper() != sign)
            {
                Response.Write("签名不合并，可以传输过程中已经被篡改！");
                Response.StatusCode = 401;
                Response.End();
                return;
            }

            entity.oauth_requestToken = Utils.Encryptor.MD5Encryptor.MD5(entity.oauth_consumer_key + entity.oauth_consumer_secret);
            entity.oauth_timestamp = (DateTime.Now.ToUniversalTime() - DateTime.MinValue).TotalSeconds;
            Response.Redirect(redirect_uri + "?requestToken=" + entity.oauth_requestToken);
        }

        /// <summary>
        /// 获取access_token，重定向到第三方
        /// 返回地址加上access_token
        /// </summary>
        public void GetAccessToken()
        {
            string redirect_uri = Request.QueryString["redirect_uri"];
            string requestToken = Request.QueryString["requestToken"];
            var entity = OAuthDb.Db.FirstOrDefault(i => i.oauth_requestToken == requestToken);

            if (entity == null)
            {
                Response.Write("requestToken有问题！");
                Response.StatusCode = 401;
                Response.End();
            }
            else
            {
                entity.oauth_accessToken = Utils.Encryptor.MD5Encryptor.MD5(entity.oauth_consumer_key + entity.oauth_consumer_secret + entity.oauth_requestToken);
                Response.Redirect(redirect_uri + "&accessToken=" + entity.oauth_accessToken);
            }

        }
    }
    #endregion

    #region OAuth客户端
    public class OAuthConfig
    {
        public static string appKey = "zzl";
        public static string appSecret = "zzl1983";
    }
    /// <summary>
    /// 提供OAuth授权的充血模型
    /// </summary>
    public class OAuthData
    {
        //采用排序的Dictionary的好处是方便对数据包进行签名，不用再签名之前再做一次排序
        private SortedDictionary<string, object> m_values = new SortedDictionary<string, object>();

        /// <summary>
        /// 设置某个字段的值
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        public void SetValue(string key, object value)
        {
            m_values[key] = value;
        }

        /// <summary>
        /// 根据字段名获取某个字段的值
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public object GetValue(string key)
        {
            object o = null;
            m_values.TryGetValue(key, out o);
            return o;
        }

        /// <summary>
        /// 判断某个字段是否已设置
        /// <param name="key">字段名</param>
        /// <returns>若字段key已被设置，则返回true，否则返回false</returns>
        public bool IsSet(string key)
        {
            object o = null;
            m_values.TryGetValue(key, out o);
            if (null != o)
                return true;
            else
                return false;
        }

        /// <summary>
        /// 将Dictionary转成xml
        /// <returns>经转换得到的xml串</returns>
        public string ToXml()
        {
            //数据为空时不能转化为xml格式
            if (0 == m_values.Count)
            {
                LoggerFactory.Instance.Logger_Warn("OAuthModel数据为空!");
                throw new ArgumentException("OAuthModel数据为空!");
            }

            string xml = "<xml>";
            foreach (KeyValuePair<string, object> pair in m_values)
            {
                //字段值不能为null，会影响后续流程
                if (pair.Value == null)
                {
                    LoggerFactory.Instance.Logger_Warn("OAuthModel内部含有值为null的字段!");
                    throw new ArgumentException("WxPayData内部含有值为null的字段!");
                }

                if (pair.Value.GetType() == typeof(int))
                {
                    xml += "<" + pair.Key + ">" + pair.Value + "</" + pair.Key + ">";
                }
                else if (pair.Value.GetType() == typeof(string))
                {
                    xml += "<" + pair.Key + ">" + "<![CDATA[" + pair.Value + "]]></" + pair.Key + ">";
                }
                else//除了string和int类型不能含有其他数据类型
                {
                    LoggerFactory.Instance.Logger_Warn("OAuthModel字段数据类型错误");
                    throw new ArgumentException("OAuthModel字段数据类型错误");
                }
            }
            xml += "</xml>";
            return xml;
        }

        /// <summary>
        /// 将xml转为WxPayData对象并返回对象内部的数据
        /// <param name="xml">待转换的xml串</param>
        /// <returns>经转换得到的Dictionary</returns>
        public SortedDictionary<string, object> FromXml(string xml)
        {
            if (string.IsNullOrEmpty(xml))
            {
                LoggerFactory.Instance.Logger_Warn("将空的xml串转换为OAuthModel不合法!");
                throw new ArgumentException("将空的xml串转换为WxPayData不合法!");
            }

            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.LoadXml(xml);
            XmlNode xmlNode = xmlDoc.FirstChild;//获取到根节点<xml>
            XmlNodeList nodes = xmlNode.ChildNodes;
            foreach (XmlNode xn in nodes)
            {
                XmlElement xe = (XmlElement)xn;
                m_values[xe.Name] = xe.InnerText;//获取xml的键值对到WxPayData内部的数据中
            }

            try
            {
                //2015-06-29 错误是没有签名
                if (m_values["return_code"] != "SUCCESS")
                {
                    return m_values;
                }
                CheckSign();//验证签名,不通过会抛异常
            }
            catch (Exception ex)
            {
                LoggerFactory.Instance.Logger_Error(ex);
                throw new Exception(ex.Message);
            }

            return m_values;
        }

        /// <summary>
        ///  Dictionary格式转化成url参数格式
        /// <returns>url格式串, 该串不包含sign字段值</returns>
        public string ToUrl()
        {
            string buff = "";
            foreach (KeyValuePair<string, object> pair in m_values)
            {
                if (pair.Value == null)
                {
                    LoggerFactory.Instance.Logger_Warn("OAuthModel内部含有值为null的字段");
                    throw new ArgumentException("OAuthModel内部含有值为null的字段");
                }

                if (pair.Key != "sign" && pair.Value.ToString() != "")
                {
                    buff += pair.Key + "=" + pair.Value + "&";
                }
            }
            buff = buff.Trim('&');
            return buff;
        }

        /// <summary>
        /// Dictionary格式化成Json
        /// <returns>json串数据</returns>
        public string ToJson()
        {
            return Utils.SerializeMemoryHelper.SerializeToJson(m_values);
        }

        /// <summary>
        /// values格式化成能在Web页面上显示的结果（因为web页面上不能直接输出xml格式的字符串）
        /// </summary>
        /// <returns></returns>
        public string ToPrintStr()
        {
            string str = "";
            foreach (KeyValuePair<string, object> pair in m_values)
            {
                if (pair.Value == null)
                {
                    LoggerFactory.Instance.Logger_Warn("OAuthModel内部含有值为null的字段");
                    throw new ArgumentException("OAuthModel内部含有值为null的字段!");
                }

                str += string.Format("{0}={1}<br>", pair.Key, pair.Value.ToString());
            }
            return str;
        }

        /// <summary>
        /// 生成签名，详见签名生成算法 
        /// </summary>
        /// <returns> 签名, sign字段不参加签名</returns>
        public string MakeSign()
        {
            string key = OAuthConfig.appSecret;//双方定义好的key
            //转url格式
            string str = ToUrl();
            //在string后加入API KEY
            str += "&key=" + key;
            //MD5加密
            var md5 = System.Security.Cryptography.MD5.Create();
            var bs = md5.ComputeHash(Encoding.UTF8.GetBytes(str));
            var sb = new StringBuilder();
            foreach (byte b in bs)
            {
                sb.Append(b.ToString("x2"));
            }
            //所有字符转为大写
            return sb.ToString().ToUpper();
        }

        /// <summary>
        /// 检测签名是否正确
        /// </summary>
        /// <returns>正确返回true，错误抛异常</returns>
        public bool CheckSign()
        {
            //如果没有设置签名，则跳过检测
            if (!IsSet("sign"))
            {
                LoggerFactory.Instance.Logger_Warn("OAuthData签名不存在!");
                throw new ArgumentException("OAuthData签名不存在!");
            }
            //如果设置了签名但是签名为空，则抛异常
            else if (GetValue("sign") == null || GetValue("sign").ToString() == "")
            {
                LoggerFactory.Instance.Logger_Warn("OAuthData签名存在但不合法!");
                throw new ArgumentException("OAuthData签名存在但不合法!");
            }

            //获取接收到的签名
            string return_sign = GetValue("sign").ToString().ToUpper();

            //在本地计算新的签名
            string cal_sign = MakeSign();

            if (cal_sign == return_sign)
            {
                return true;
            }
            LoggerFactory.Instance.Logger_Warn("OAuthModel签名验证错误!");
            throw new ArgumentException("OAuthModel签名验证错误!");
        }

        /// <summary>
        /// 获取Dictionary
        /// </summary>
        /// <returns></returns>
        public SortedDictionary<string, object> GetValues()
        {
            return m_values;
        }
    }

    /// <summary>
    /// 客户端
    /// </summary>
    public class OAuthClient
    {
        /// <summary>
        /// 获取requestToken
        /// </summary>
        public string GetRequestToken(string appKey)
        {
            if (string.IsNullOrWhiteSpace(System.Web.HttpContext.Current.Request.QueryString["requestToken"]))
            {
                var data = new OAuthData();
                data.SetValue("appKey", appKey);
                data.SetValue("redirect_uri", System.Web.HttpContext.Current.Request.Url.AbsoluteUri);
                data.SetValue("sign", data.MakeSign()); //所有参数加key，生成md5
                if (!data.CheckSign())
                {
                    System.Web.HttpContext.Current.Response.Write("sign签名格式不正确，请参考说明文档！");
                    System.Web.HttpContext.Current.Response.StatusCode = 401;
                    System.Web.HttpContext.Current.Response.End();
                }
                System.Web.HttpContext.Current.Response.Redirect("http://localhost:5766/OAuth/GetRequestToken?" + data.ToUrl() + "&sign=" + data.MakeSign());
                return null;
            }
            else
            {
                return System.Web.HttpContext.Current.Request.QueryString["requestToken"];
            }
        }
        /// <summary>
        /// 获取accessToken
        /// </summary>
        public string GetAccessToken()
        {
            if (string.IsNullOrWhiteSpace(System.Web.HttpContext.Current.Request.QueryString["accessToken"]))
            {

                string requestToken = System.Web.HttpContext.Current.Request.QueryString["requestToken"];
                var data = new OAuthData();
                data.SetValue("requestToken", requestToken);
                data.SetValue("redirect_uri", System.Web.HttpContext.Current.Request.Url.AbsoluteUri);
                System.Web.HttpContext.Current.Response.Redirect("http://localhost:5766/OAuth/GetAccessToken?" + data.ToUrl() + "&sign=" + data.MakeSign());
                return null;
            }
            else
            {
                return System.Web.HttpContext.Current.Request.QueryString["accessToken"];
            }
        }
    }
    #endregion
}
