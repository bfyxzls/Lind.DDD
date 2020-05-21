using Microsoft.Practices.Unity.InterceptionExtension;
using System;
using System.Collections.Generic;
using Microsoft.Practices.Unity.InterceptionExtension.Configuration;

namespace Lind.DDD.IoC.Interception
{
    /// <summary>
    /// 拦截器实例，具体拦截器可以自己去建立项目来实现，需要实现IInterceptionBehavior接口
    /// 表示用于异常日志记录的拦截行为。
    /// </summary>
    public class ExceptionLoggingBehavior : InterceptionBase
    {

        /// <summary>
        /// 通过实现此方法来拦截调用并执行所需的拦截行为。
        /// </summary>
        /// <param name="input">调用拦截目标时的输入信息。</param>
        /// <param name="getNext">通过行为链来获取下一个拦截行为的委托。</param>
        /// <returns>从拦截目标获得的返回信息。</returns>
        public override IMethodReturn Invoke(
            IMethodInvocation input,
            GetNextInterceptionBehaviorDelegate getNext)
        {
            //方法执行前
            var methodReturn = getNext().Invoke(input, getNext);//原方法被执行
            //方法执行后
            if (methodReturn.Exception != null)
            {
                Console.WriteLine(methodReturn.Exception.Message);
                Logger.LoggerFactory.Instance.Logger_Error(methodReturn.Exception);
            }
            return methodReturn;
        }

    }
}
