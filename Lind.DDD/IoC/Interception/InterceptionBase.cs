using Microsoft.Practices.Unity.InterceptionExtension;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lind.DDD.IoC.Interception
{
    /// <summary>
    /// 拦截器抽象基类
    /// 实现拦截器的项目需要继承此类，只引用Microsoft.Practices.Unity.Interception.dll程序集
    /// </summary>
    public abstract class InterceptionBase : IInterceptionBehavior
    {
        /// <summary>
        /// 获取当前行为需要拦截的对象类型接口。
        /// </summary>
        /// <returns></returns>
        public IEnumerable<Type> GetRequiredInterfaces()
        {
            return Type.EmptyTypes;
        }

        /// <summary>
        ///通过实现此方法来拦截调用并执行所需的拦截行为。
        /// </summary>
        /// <param name="input">调用拦截目标时的输入信息</param>
        /// <param name="getNext">通过行为链来获取下一个拦截行为的委托</param>
        /// <returns>从拦截目标获得的返回信息</returns>
        public abstract IMethodReturn Invoke(IMethodInvocation input, GetNextInterceptionBehaviorDelegate getNext);

        /// <summary>
        /// 获取一个<see cref="Boolean"/>值，该值表示当前拦截行为被调用时，是否真的需要执行拦截动作
        /// </summary>
        public bool WillExecute
        {
            get { return true; }
        }
    }
}
