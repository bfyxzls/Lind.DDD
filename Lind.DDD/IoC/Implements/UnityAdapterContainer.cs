using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Practices.Unity;
using System.Configuration;
namespace Lind.DDD.IoC.Implements
{
    /// <summary>
    /// Unity实现的IoC容器
    /// 适配器模式，定义新的容器类按着IContainer的标准，对UnityContainer进行适配,从新实现 
    /// </summary>
    internal sealed class UnityAdapterContainer : UnityContainer, IContainer
    {

        /// <summary>
        /// 反射成对象
        /// </summary>
        /// <typeparam name="TService">接口类型</typeparam>
        /// <returns>具体类型</returns>
        public TService Resolve<TService>()
        {
            return UnityContainerExtensions.Resolve<TService>(this);
        }
        /// <summary>
        /// 反射成对象
        /// </summary>
        /// <typeparam name="TService">接口类型</typeparam>
        /// <returns>具体类型</returns>
        public object Resolve(Type type)
        {
            return UnityContainerExtensions.Resolve(this, type);
        }
        /// <summary>
        /// 反射成对象
        /// </summary>
        /// <typeparam name="TService">接口类型</typeparam>
        /// <param name="overridedArguments">参数</param>
        /// <returns>具体类型</returns>
        public TService Resolve<TService>(object overridedArguments)
        {
            var overrides = Utils.GetParameterOverrides(overridedArguments);
            return UnityContainerExtensions.Resolve<TService>(this, overrides.ToArray());
        }
        /// <summary>
        /// 反射成对象
        /// </summary>
        /// <typeparam name="TService">接口类型</typeparam>
        /// <param name="overridedArguments">参数</param>
        /// <returns>具体类型</returns>
        public object Resolve(Type serviceType, object overridedArguments)
        {
            var overrides = Utils.GetParameterOverrides(overridedArguments);
            return UnityContainerExtensions.Resolve(this, serviceType, overrides.ToArray());
        }

        /// <summary>
        /// 注册抽象类型与具体实现的类型
        /// </summary>
        /// <param name="from">接口类型</param>
        /// <param name="to">具体类型</param>
        public void RegisterType(Type from, Type to)
        {
            UnityContainerExtensions.RegisterType(this, from, to);
        }

        /// <summary>
        /// 类型是否被注册到IoC容器
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public bool IsRegistered(Type type)
        {
            return UnityContainerExtensions.IsRegistered(this, type);
        }
    }
}
