using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace Lind.DDD.IoC
{
    /// <summary>
    /// IoC容器规范
    /// 作者：仓储大叔
    /// </summary>
    public interface IContainer
    {
        /// <summary>
        /// 反射成对象
        /// </summary>
        /// <typeparam name="TService">接口类型</typeparam>
        /// <returns>具体类型</returns>
        TService Resolve<TService>();
        /// <summary>
        /// 反射成对象
        /// </summary>
        /// <typeparam name="TService">接口类型</typeparam>
        /// <returns>具体类型</returns>
        object Resolve(Type type);
        /// <summary>
        /// 反射成对象
        /// </summary>
        /// <typeparam name="TService">接口类型</typeparam>
        /// <param name="overridedArguments">参数</param>
        /// <returns>具体类型</returns>
        TService Resolve<TService>(object overridedArguments);
        /// <summary>
        /// 反射成对象
        /// </summary>
        /// <typeparam name="TService">接口类型</typeparam>
        /// <param name="overridedArguments">参数</param>
        /// <returns>具体类型</returns>
        object Resolve(Type serviceType, object overridedArguments);
        /// <summary>
        /// 注册抽象类型与具体实现的类型
        /// </summary>
        /// <param name="from">接口类型</param>
        /// <param name="to">具体类型</param>
        void RegisterType(Type from, Type to);
        /// <summary>
        /// 类型是否被注册到IoC容器
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        bool IsRegistered(Type type);
    }
}
