using Lind.DDD.HttpExtensions.HttpResponse;
using Lind.DDD.MongoDbClient;
using Lind.DDD.DTO;
using Lind.DDD.Utils;
using Lind.DDD.Utils.Encryptor;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Web;
using System.Web.Http;
using System.Web.Http.Filters;

namespace Lind.DDD.Authorization.Api
{
    /// <summary>
    /// 功能：api数据安全性验证
    /// 校验方式：ciphertext=md5(form键的值拼接+timestamp+passkey),服务端用接收到的表单数据与时间戳和自己的passkey进行md5生成，最后比较值是否一致
    /// passkey为私钥，不用于网络传递，你可以将它与appKey进行关联，appKey用来传递，服务器根据appKey去数据库里取对应的passkey然后进行比较
    /// 功能：请求唯一性，防伪造性
    /// timestamp:UTC时间戳，不用于网络传递，在客户端调用服务器时，服务器也生成yyyyMMddhh的时间戳，然后进行计算，看是否过期（即每小时的串是有效的）
    /// </summary>
    [AttributeUsage(AttributeTargets.Method)]
    public class ApiValiadateFilter : ActionFilterAttribute
    {
        /// <summary>
        /// 方法拦截
        /// </summary>
        /// <param name="actionContext"></param>
        public override void OnActionExecuting(System.Web.Http.Controllers.HttpActionContext actionContext)
        {

            #region 例外
            bool skipAuthorization = actionContext.ControllerContext.ControllerDescriptor.ControllerType.IsDefined(typeof(AllowAnonymousAttribute), inherit: true) ||
           actionContext.ControllerContext.ControllerDescriptor.ControllerType.IsDefined(typeof(AllowAnonymousAttribute), inherit: true);

            if (skipAuthorization)
                return;
            #endregion

            #region 初始化
            var context = (HttpContextBase)actionContext.Request.Properties["MS_HttpContext"];//获取传统context
            var request = context.Request;//定义传统request对象
            var paramStr = new StringBuilder();
            //校验码只从Url地址后取，不从表单取
            var coll = new NameValueCollection(request.QueryString);
            coll.Add(request.Form);

            //StringContent方式的提交
            //var task = actionContext.Request.Content.ReadAsByteArrayAsync().Result;
            //var result = Encoding.UTF8.GetString(task);
            //if (!string.IsNullOrWhiteSpace(result))
            //{
            //    foreach (var key in result.FromJson().Keys)
            //        coll.Add(key, (string)result.FromJson()[key]);
            //}


            #endregion

            #region 解析XML配置文件
            var config = CacheConfigFile.ConfigFactory.Instance.GetConfig<ApiValidateModelConfig>()
                                                               .ApiValidateModelList
                                                               .FirstOrDefault(i => i.AppKey == coll["AppKey"]);
            if (config == null)
            {
                actionContext.Response = new HttpResponseMessage(HttpStatusCode.Forbidden)
                {
                    Content = new StringContent(SerializeMemoryHelper.SerializeToJson(new ResponseMessage()
                    {
                        Status = 0,
                        ErrorCode = "001",
                        ErrorMessage = "AppKey不是合法的，请先去组织生成有效的Key"
                    }))
                };
                base.OnActionExecuting(actionContext);
                return;
            }
            if (config.ExpireDate < DateTime.Now)
            {
                actionContext.Response = new HttpResponseMessage(HttpStatusCode.Forbidden)
                {
                    Content = new StringContent(SerializeMemoryHelper.SerializeToJson(new ResponseMessage()
                    {
                        Status = 0,
                        ErrorCode = "002",
                        ErrorMessage = "AppKey不是合法的，密钥已过期"
                    }))
                };
                base.OnActionExecuting(actionContext);
                return;
            }
            #endregion

            #region 验证算法

            var keys = new List<string>();
            foreach (string param in coll.Keys)
            {
                if (!string.IsNullOrEmpty(param))
                {
                    keys.Add(param.ToLower());
                }

            }
            keys.Sort();
            foreach (string p in keys)
            {
                if (p != ApiValidateHelper.CipherText)
                {
                    if (!string.IsNullOrEmpty(coll[p]))
                    {
                        paramStr.Append(coll[p]);
                    }

                }
            }
            paramStr.Append(config.PassKey);
            #endregion

            double timeStamp;
            if (double.TryParse(coll["timeStamp"], out timeStamp))
            {
                //当前UTC时间与1700/01/01的时间戳
                if (((DateTime.UtcNow - new DateTime(1970, 1, 1)).TotalSeconds - timeStamp) / 60 > config.ValidateMinutes)
                {
                    actionContext.Response = new HttpResponseMessage(HttpStatusCode.Forbidden)
                    {
                        Content = new StringContent(SerializeMemoryHelper.SerializeToJson(new ResponseMessage()
                        {
                            Status = 0,
                            ErrorCode = "003",
                            ErrorMessage = "请求已过期"
                        }))
                    };
                }
            }
            else
            {
                actionContext.Response = new HttpResponseMessage(HttpStatusCode.Forbidden)
                {
                    Content = new StringContent(SerializeMemoryHelper.SerializeToJson(new ResponseMessage()
                    {
                        Status = 0,
                        ErrorCode = "003",
                        ErrorMessage = "时间戳异常"
                    }))
                };
            }
            if (Lind.DDD.Utils.Encryptor.Utility.EncryptString(paramStr.ToString(), Lind.DDD.Utils.Encryptor.Utility.EncryptorType.MD5) != coll[ApiValidateHelper.CipherText])
            {

                actionContext.Response = new HttpResponseMessage(HttpStatusCode.Forbidden)
                {
                    Content = new StringContent(SerializeMemoryHelper.SerializeToJson(new ResponseMessage()
                    {
                        Status = 0,
                        ErrorCode = "003",
                        ErrorMessage = "验证失败，请求非法"
                    }))
                };
            }

            base.OnActionExecuting(actionContext);
        }
    }

}



