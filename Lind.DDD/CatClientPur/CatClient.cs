using PureCat.Configuration;
using PureCat.Context;
using PureCat.Message;
using PureCat.Message.Spi;
using PureCat.Message.Spi.Internals;
using PureCat.Util;
using System;
using System.Collections.Generic;
using System.Collections;
using System.IO;
using System.Net.Http;
using System.Web;

namespace PureCat
{
    /// <summary>
    /// Cat客户端
    /// </summary>
    public class CatClient
    {
        private static readonly CatClient _instance = null;
        private static readonly object _lock = new object();

        public bool Initialized { get; private set; }

        public IMessageManager MessageManager { get; private set; }

        public IMessageProducer MessageProducer { get; private set; }

        static CatClient()
        {
            if (_instance == null)
            {
                lock (_lock)
                {
                    if (_instance == null)
                    {
                        _instance = new CatClient();
                    }
                }
            }
        }
        /// <summary>
        /// Cat初始化
        /// 1 加载客户端配置
        /// 2 拿到客户端的server列表
        /// 3 初始化客户端的server对象，返回ClientConfig对象
        /// 4 从将客户端的server地址进行http的发送，并返回服务端的路由列表，并覆盖ClientConfig的server属性
        /// 5 最后就是随机取一个可用的服务器
        /// </summary>
        public static void Initialize()
        {
            if (_instance == null)
                throw new Exception("_instance属性为null");

            if (_instance.Initialized)
                return;

            var configManager = new ClientConfigManager();
            DefaultMessageManager manager;
            manager = new DefaultMessageManager();
            manager.InitializeClient(configManager.ClientConfig);
            _instance.MessageManager = manager;
            _instance.MessageProducer = new DefaultMessageProducer(manager);
            _instance.Initialized = true;
            Logger.Info("Cat .Net Client initialized.");
        }

        public static IMessageManager GetManager()
        {
            return _instance.MessageManager;
        }

        public static IMessageProducer GetProducer()
        {
            return _instance.MessageProducer;
        }

        public static bool IsInitialized()
        {
            bool isInitialized = _instance.Initialized;
            if (isInitialized && !_instance.MessageManager.HasContext())
            {
                _instance.MessageManager.Setup();
            }
            return isInitialized;
        }

        /// <summary>
        /// 运行一个已有事务的代码段
        /// </summary>
        /// <param name="type"></param>
        /// <param name="name"></param>
        /// <param name="context"></param>
        /// <param name="action"></param>
        /// <returns></returns>
        public static CatContext DoTransaction(string type, string name, CatContext context, Action action)
        {
            var tran = NewTransaction(type, name);
            try
            {
                action();
                return context;

            }
            catch (Exception ex)
            {
                LogError(ex);
                tran.SetStatus(ex);
                throw;
            }
            finally
            {
                tran.Complete();
            }

        }

        /// <summary>
        /// 发布分布式事务，返回上下文
        /// </summary>
        /// <param name="type"></param>
        /// <param name="name"></param>
        /// <param name="action"></param>
        /// <returns></returns>
        public static CatContext DoTransaction(string type, string name, Action action)
        {
            var tran = NewTransaction(type, name);
            try
            {
                action();
                return PureCat.CatClient.LogRemoteCallClient(Guid.NewGuid().ToString());

            }
            catch (Exception ex)
            {
                LogError(ex);
                tran.SetStatus(ex);
                throw;
            }
            finally
            {
                tran.Complete();
            }

        }
        /// <summary>
        /// 发布分布式事务，不返回上下文
        /// </summary>
        /// <param name="type"></param>
        /// <param name="name"></param>
        /// <param name="action"></param>
        public static void DoTransactionAction(string type, string name, Action action)
        {
            var tran = NewTransaction(type, name);
            try
            {
                action();

            }
            catch (Exception ex)
            {
                LogError(ex);
                tran.SetStatus(ex);
                throw;
            }
            finally
            {
                tran.Complete();
            }

        }
        public static ITaggedTransaction NewTaggedTransaction(string type, string name, string tag)
        {
            return GetProducer().NewTaggedTransaction(type, name, tag);
        }

        public static IForkedTransaction NewForkedTransaction(string type, string name)
        {
            return GetProducer().NewForkedTransaction(type, name);
        }

        public static ITransaction NewTransaction(string type, string name)
        {
            return GetProducer().NewTransaction(type, name);
        }

        public static IEvent NewEvent(string type, string name)
        {
            return GetProducer().NewEvent(type, name);
        }

        public static ITrace NewTrace(string type, string name)
        {
            return GetProducer().NewTrace(type, name);
        }

        public static string GetCurrentMessageId()
        {
            var tree = GetManager().ThreadLocalMessageTree;
            if (tree != null)
            {
                if (tree.MessageId == null)
                {
                    tree.MessageId = CreateMessageId();
                }
                return tree.MessageId;
            }
            else
            {
                return null;
            }
        }

        public static string CreateMessageId()
        {
            return GetProducer().CreateMessageId();
        }

        public static CatContext LogRemoteCallClient(string contextName)
        {
            var ctx = new CatContext(contextName);

            var tree = GetManager().ThreadLocalMessageTree;

            if (tree.MessageId == null)
            {
                tree.MessageId = CreateMessageId();
            }

            var messageId = tree.MessageId;

            var childId = CreateMessageId();
            LogEvent(PureCatConstants.TYPE_REMOTE_CALL, ctx.ContextName, PureCatConstants.SUCCESS, childId);

            var rootId = tree.RootMessageId;

            if (rootId == null)
            {
                rootId = tree.MessageId;
            }

            ctx.CatRootId = rootId;
            ctx.CatParentId = messageId;
            ctx.CatChildId = childId;

            return ctx;
        }

        public static void LogRemoteCallServer(CatContext ctx)
        {
            if (ctx == null)
            {
                return;
            }

            var tree = GetManager().ThreadLocalMessageTree;
            var messageId = ctx.CatChildId;
            var rootId = ctx.CatRootId;
            var parentId = ctx.CatParentId;

            if (messageId != null)
            {
                tree.MessageId = messageId;
            }
            if (parentId != null)
            {
                tree.ParentMessageId = parentId;
            }
            if (rootId != null)
            {
                tree.RootMessageId = rootId;
            }
        }

        public static void LogEvent(string type, string name, string status = PureCatConstants.SUCCESS, string nameValuePairs = null)
        {
            GetProducer().LogEvent(type, name, status, nameValuePairs);
        }

        public static void LogHeartbeat(string type, string name, string status = PureCatConstants.SUCCESS, string nameValuePairs = null)
        {
            GetProducer().LogHeartbeat(type, name, status, nameValuePairs);
        }

        public static void LogError(Exception ex)
        {
            GetProducer().LogError(ex);
        }

        public static void LogMetricForCount(string name, int count = 1)
        {
            LogMetricInternal(name, "C", count.ToString());
        }

        public static void LogMetricForDuration(string name, double value)
        {
            LogMetricInternal(name, "T", string.Format("{0:F}", value));
        }

        public static void LogMetricForSum(string name, double value)
        {
            LogMetricInternal(name, "S", string.Format("{0:F}", value));
        }

        public static void LogMetricForSum(string name, double value, int count = 1)
        {
            LogMetricInternal(name, "S,C", string.Format("{0},{1:F}", count, value));
        }

        private static void LogMetricInternal(string name, string status, string keyValuePairs = null)
        {
            GetProducer().LogMetric(name, status, keyValuePairs);
        }

        #region 分布式消息树的封装（仓储大叔）

        #region 从请求头拿到CatContext上下文
        /// <summary>
        /// 从请求头中拿到当前的消息树对象
        /// client发布端：SetCatContextToServer
        /// server接收端：GetCatContextFromServer
        /// </summary>
        /// <returns></returns>
        public static CatContext GetCatContextFromServer()
        {
            var result = System.Web.HttpContext.Current.Request.Headers.GetValues("catContext");
            if (result != null && result.Length > 0)
            {
                var cat = Lind.DDD.Utils.SerializeMemoryHelper.DeserializeFromJson<CatContext>(result[0].ToString());
                return cat;
            }
            return null;
        }
        public static CatContext GetCatContextFromServer(HttpClient http)
        {
            IList<string> result = http.DefaultRequestHeaders.GetValues("catContext") as IList<string>;
            if (result != null && result.Count > 0)
            {
                var cat = Lind.DDD.Utils.SerializeMemoryHelper.DeserializeFromJson<CatContext>(result[0].ToString());
                return cat;
            }
            return null;
        }
        #endregion

        #region 设置catContext到请求头
        /// <summary>
        /// 设置消息树到当前请求头
        /// </summary>
        /// <returns></returns>
        public static void SetCatContextToRequestHeader(System.Web.HttpContext http, CatContext context)
        {


            if (http.Request.Headers.GetValues("catContext") != null && http.Request.Headers.GetValues("catContext").Length > 0)
            {
                http.Request.Headers.Remove("catContext");
            }
            http.Request.Headers.Add("catContext", Lind.DDD.Utils.SerializeMemoryHelper.SerializeToJson(context));
        }
        /// <summary>
        /// 设置消息树到当前请求头
        /// </summary>
        /// <param name="http"></param>
        /// <param name="context"></param>
        public static void SetCatContextToRequestHeader(HttpClient http, CatContext context)
        {
            http.DefaultRequestHeaders.Remove("catContext");
            http.DefaultRequestHeaders.Add("catContext", Lind.DDD.Utils.SerializeMemoryHelper.SerializeToJson(context));
        }
        /// <summary>
        /// 设置消息树到当前请求头
        /// </summary>
        /// <param name="http"></param>
        /// <param name="context"></param>
        public static void SetCatContextToRequestHeader(System.Web.HttpContextBase http, CatContext context)
        {
            if (http.Request.Headers.GetValues("catContext") != null && http.Request.Headers.GetValues("catContext").Length > 0)
            {
                http.Request.Headers.Remove("catContext");
            }
            http.Request.Headers.Add("catContext", Lind.DDD.Utils.SerializeMemoryHelper.SerializeToJson(context));

        }
        /// <summary>
        /// 设置请求头，它来自某个响应头
        /// </summary>
        /// <param name="response"></param>
        public static void SetCatContextToRequestHeader(HttpResponseMessage response)
        {

            IEnumerable<string> context = new List<string>();
            if (response.Headers.TryGetValues("catContext", out context))
            {
                if (context != null)
                {
                    var cat = Lind.DDD.Utils.SerializeMemoryHelper.DeserializeFromJson<CatContext>((context as string[])[0].ToString());

                    PureCat.CatClient.SetCatContextToRequestHeader(System.Web.HttpContext.Current, cat);
                }
            }


        }
        #endregion

        #region 设置catContext到响应头
        /// <summary>
        /// 设置catContext到响应头
        /// </summary>
        /// <param name="response"></param>
        /// <param name="context"></param>
        public static void SetCatContextToResponseHeader(HttpResponseBase response, CatContext context)
        {
            if (response.Headers.GetValues("catContext") != null
                && response.Headers.GetValues("catContext").Length > 0)
            {
                response.Headers.Remove("catContext");
            }
            response.Headers.Add("catContext", Lind.DDD.Utils.SerializeMemoryHelper.SerializeToJson(context));
        }

        #endregion
        #endregion

    }
}