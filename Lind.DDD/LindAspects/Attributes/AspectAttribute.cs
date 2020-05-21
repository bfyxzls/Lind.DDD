using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Lind.DDD.LindAspects
{
    /// <summary>
    /// 方法拦截基类
    /// 每种特性在实现类上只能应用一种，可以使用多个多种特性，不支持多个相同的特性
    /// </summary>
    public abstract class AspectAttribute : Attribute
    {
        /// <summary>
        /// 拦截行为，有返回值
        /// </summary>
        /// <param name="context"></param>
        /// <param name="methodInfo"></param>
        /// <returns></returns>
        public virtual object FuncInvoke(InvokeContext context, MethodInfo methodInfo) { return null; }

        /// <summary>
        /// 拦截行为，没有返回值
        /// </summary>
        /// <param name="context"></param>
        /// <param name="methodInfo"></param>
        public virtual void ActionInvoke(InvokeContext context, MethodInfo methodInfo) { }

    }
    /// <summary>
    /// 方法执行前拦截
    /// </summary>
    [AttributeUsage(AttributeTargets.Method)]
    public abstract class BeforeAspectAttribute : AspectAttribute
    { }
    /// <summary>
    /// 方法执行后拦截
    /// </summary>
    [AttributeUsage(AttributeTargets.Method)]
    public abstract class AfterAspectAttribute : AspectAttribute
    { }
    /// <summary>
    /// 属性拦截
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public abstract class PropertyAspectAttribute : AspectAttribute
    { }
    /// <summary>
    /// 出现异常时拦截
    /// </summary>
    [AttributeUsage(AttributeTargets.Method)]
    public abstract class ExceptionAspectAttribute : AspectAttribute
    { }
}
