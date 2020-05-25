using System;
using System.Collections.Generic;
using System.Data.Entity.Infrastructure.Interception;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace System.Linq
{
    /// <summary>
    /// DbCommand拦截器扩展
    /// </summary>
    public static class DbCommandInterceptorExtensions
    {
        /// <summary>
        /// 将DbCommand的拦截器以单例的形式添加到DbInterception静态对象中
        /// </summary>
        /// <param name="action"></param>
        public static void UsingSingletonInterceptor(DbCommandInterceptor interceptor)
        {
            #region SQL语句拦截器，拦截器只加载一次
            var property = typeof(DbCommandDispatcher).GetProperty("InternalDispatcher", BindingFlags.Instance | BindingFlags.NonPublic);
            if (property != null)
            {
                var val = property.GetValue(System.Data.Entity.Infrastructure.Interception.DbInterception.Dispatch.Command);
                if (val != null)
                {
                    var list = val.GetType().GetField("_interceptors", BindingFlags.Instance | BindingFlags.NonPublic);
                    if (list != null)
                    {
                        var listVal = list.GetValue(val) as List<System.Data.Entity.Infrastructure.Interception.IDbCommandInterceptor>;
                        if (listVal != null)
                        {
                            if (listVal.FirstOrDefault(i => i.ToString() == interceptor.GetType().ToString()) == null)
                            {
                                System.Data.Entity.Infrastructure.Interception.DbInterception.Add(interceptor);
                            }
                        }
                    }
                }
            }
            #endregion
        }
    }
}
