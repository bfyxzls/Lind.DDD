using Autofac;
using Lind.DDD.LindAspects;
using Lind.DDD.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Lind.DDD.LindPlugins
{
    /// <summary>
    /// 可插拔组件的管理者
    /// Author:Lind
    /// 依赖于Autofac
    /// </summary>
    public class PluginManager
    {
        /// <summary>
        /// 插件容器辅助字段
        /// </summary>
        private static IContainer _container = null;
        /// <summary>
        /// 互斥锁
        /// </summary>
        private static object lockObj = new object();
        /// <summary>
        /// 类的构造方法
        /// </summary>
        static PluginManager()
        {

            lock (lockObj)
            {
                if (_container == null)
                {
                    lock (lockObj)
                    {
                        try
                        {
                            var builder = new ContainerBuilder();
                            //装载的插件都是公共类型
                            var typeList = AssemblyHelper.GetTypesByInterfaces(typeof(IPlugins)).Where(i => i.IsPublic);
                            Lind.DDD.Logger.LoggerFactory.Instance.Logger_Info("开始注册(IPlugins)插件！");
                            foreach (var item in typeList)
                            {
                                foreach (var sub in item.GetInterfaces())
                                {
                                    builder.RegisterType(item).Named(item.FullName, sub);

                                    Lind.DDD.Logger.LoggerFactory.Instance.Logger_Info(
                                        item.FullName.PadRight(50, '-') + sub.FullName);
                                }
                            }
                            Lind.DDD.Logger.LoggerFactory.Instance.Logger_Info("成功注册(IPlugins)所有插件！");
                            _container = builder.Build();
                        }
                        catch (Exception)
                        {
                            throw new ArgumentException("PluginManager依赖于autofac包包...");
                        }

                    }
                }
            }

        }
        /// <summary>
        /// 从插件容器里返回对象
        /// </summary>
        /// <param name="serviceName">对象全名</param>
        /// <param name="serviceType">接口类型</param>
        /// <returns></returns>
        public static object Resolve(string serviceName, Type serviceType)
        {
            var obj = _container.ResolveNamed(serviceName, serviceType);
            if (typeof(IAspectProxy).IsAssignableFrom(serviceType))
            {
                obj = ProxyFactory.CreateProxy(serviceType, obj.GetType());
            }
            return obj;
        }
        /// <summary>
        /// 从插件容器里返回对象
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="serviceName"></param>
        /// <returns></returns>
        public static TService Resolve<TService>(string serviceName)
        {
            return (TService)Resolve(serviceName, typeof(TService));
        }
        /// <summary>
        /// 从插件容器里返回对象,第一个实现它的类型
        /// </summary>
        /// <typeparam name="TService"></typeparam>
        /// <returns></returns>
        public static TService Resolve<TService>()
        {
            var types = AppDomain.CurrentDomain.GetAssemblies()
                    .SelectMany(a => a.GetTypes()
                    .Where(t => t.GetInterfaces().Contains(typeof(TService))))
                    .ToArray();
            return (TService)Resolve(types.FirstOrDefault().FullName, typeof(TService));
        }

    }
}
