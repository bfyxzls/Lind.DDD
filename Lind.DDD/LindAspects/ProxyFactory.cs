using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Lind.DDD.LindAspects
{

    /// <summary>
    /// 动态代理生产者
    /// </summary>
    public class ProxyFactory
    {
        public static T CreateProxy<T>(Type realProxyType)
        {
            var generator = new DynamicProxyGenerator(realProxyType, typeof(T));
            Type type = generator.GenerateType();

            return (T)Activator.CreateInstance(type);
        }
        public static object CreateProxy(Type from, Type realProxyType)
        {
            var generator = new DynamicProxyGenerator(realProxyType, from);
            Type type = generator.GenerateType();

            return Activator.CreateInstance(type);
        }

    }
}
