using Microsoft.Practices.Unity.InterceptionExtension;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;

namespace Lind.DDD.CachingDataSet
{
    /// <summary>
    /// 表示用于方法缓存功能的拦截行为。
    /// </summary>
    public class CachingBehavior : IInterceptionBehavior
    {
        /// <summary>
        /// 缓存项目名称，每个项目有自己的名称
        /// 避免缓存键名重复
        /// </summary>
        static readonly string cacheProjectName = System.Configuration.ConfigurationManager.AppSettings["CacheProjectName"] ?? "DataSetCache";


        #region Private Methods
        /// <summary>
        /// 根据指定的<see cref="CachingAttribute"/>以及<see cref="IMethodInvocation"/>实例，
        /// 获取与某一特定参数值相关的键名。
        /// </summary>
        /// <param name="cachingAttribute"><see cref="CachingAttribute"/>实例。</param>
        /// <param name="input"><see cref="IMethodInvocation"/>实例。</param>
        /// <returns>与某一特定参数值相关的键名。</returns>
        private string GetValueKey(CachingAttribute cachingAttribute, IMethodInvocation input)
        {
            switch (cachingAttribute.Method)
            {
                // 如果是Remove，则不存在特定值键名，所有的以该方法名称相关的缓存都需要清除
                case CachingMethod.Remove:
                    return null;
                case CachingMethod.Get:// 如果是Get或者Put，则需要产生一个针对特定参数值的键名
                case CachingMethod.Put:
                    if (input.Arguments != null &&
                        input.Arguments.Count > 0)
                    {
                        var sb = new StringBuilder();
                        for (int i = 0; i < input.Arguments.Count; i++)
                        {
                            if (input.Arguments[i] == null)
                                break;

                            if (input.Arguments[i].GetType().BaseType == typeof(LambdaExpression))//lambda处理
                            {
                                string result = "";

                                try
                                {
                                    var exp = input.Arguments[i] as LambdaExpression;
                                    var arr = ((System.Runtime.CompilerServices.Closure)(((System.Delegate)(Expression.Lambda(exp).Compile().DynamicInvoke())).Target)).Constants;
                                    Type t = arr[0].GetType();
                                    foreach (var member in t.GetFields())
                                    {
                                        result += "_" + member.Name + "_" + t.GetField(member.Name).GetValue(arr[0]);
                                    }
                                    result = result.Remove(0, 1);
                                }
                                catch (NullReferenceException)
                                {
                                    //lambda表达式异常，可能是没有字段，如这种格式i=>true，会产生NullReferenceException异常.
                                }

                                sb.Append(result.ToString());
                            }
                            else if (input.Arguments[i].GetType() != typeof(string)//类和结构体处理
                                && input.Arguments[i].GetType().BaseType.IsClass)
                            {
                                var obj = input.Arguments[i];
                                Type t = obj.GetType();
                                StringBuilder result = new StringBuilder();
                                #region 提取类中的字段和属性
                                foreach (var member in t.GetProperties())//公开属性
                                {
                                    result.Append(member.Name)
                                          .Append("_")
                                          .Append(t.GetProperty(member.Name).GetValue(obj))
                                          .Append("_");
                                }
                                foreach (var member in t.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance))//私有和公用字段
                                {
                                    result.Append(member.Name)
                                          .Append("_")
                                          .Append(t.GetField(member.Name, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance).GetValue(obj))
                                          .Append("_");
                                }
                                #endregion
                                sb.Append(result.ToString().Remove(result.Length - 1));
                            }
                            else//简单值类型处理
                            {
                                sb.Append(input.Arguments[i].ToString());
                            }

                            if (i != input.Arguments.Count - 1)
                                sb.Append("_");
                        }
                        return sb.ToString();
                    }
                    else
                        return "NULL";
                default:
                    throw new InvalidOperationException("无效的缓存方式。");
            }
        }
        #endregion

        #region IInterceptionBehavior Members
        /// <summary>
        /// 获取当前行为需要拦截的对象类型接口。
        /// </summary>
        /// <returns>所有需要拦截的对象类型接口。</returns>
        public IEnumerable<Type> GetRequiredInterfaces()
        {
            return Type.EmptyTypes;
        }

        /// <summary>
        /// 通过实现此方法来拦截调用并执行所需的拦截行为。
        /// </summary>
        /// <param name="input">调用拦截目标时的输入信息。</param>
        /// <param name="getNext">通过行为链来获取下一个拦截行为的委托。</param>
        /// <returns>从拦截目标获得的返回信息。</returns>
        public IMethodReturn Invoke(IMethodInvocation input, GetNextInterceptionBehaviorDelegate getNext)
        {

            var method = input.MethodBase;
            //键值前缀
            string prefix = cacheProjectName + "_";
            var baseInterfaces = input.Target.GetType().GetInterfaces();
            if (baseInterfaces != null && baseInterfaces.Any())
            {
                foreach (var item in baseInterfaces)
                {
                    prefix += item.ToString() + "_";
                }
            }

            //键名，在put和get时使用
            var key = prefix + method.Name;

            if (method.IsDefined(typeof(CachingAttribute), false))
            {
                var cachingAttribute = (CachingAttribute)method.GetCustomAttributes(typeof(CachingAttribute), false)[0];
                var valKey = GetValueKey(cachingAttribute, input);
                switch (cachingAttribute.Method)
                {
                    case CachingMethod.Get:
                        try
                        {
                            if (CacheManager.Instance.Exists(key, valKey))
                            {
                                var obj = CacheManager.Instance.Get(key, valKey);
                                var arguments = new object[input.Arguments.Count];
                                input.Arguments.CopyTo(arguments, 0);
                                return new VirtualMethodReturn(input, obj, arguments);
                            }
                            else
                            {
                                var methodReturn = getNext().Invoke(input, getNext);
                                CacheManager.Instance.Add(key, valKey, methodReturn.ReturnValue);
                                return methodReturn;
                            }
                        }
                        catch (Exception ex)
                        {
                            return new VirtualMethodReturn(input, ex);
                        }
                    case CachingMethod.Put:
                        try
                        {
                            var methodReturn = getNext().Invoke(input, getNext);
                            if (CacheManager.Instance.Exists(key))
                            {
                                if (cachingAttribute.Force)
                                {
                                    CacheManager.Instance.Remove(key);
                                    CacheManager.Instance.Add(key, valKey, methodReturn.ReturnValue);
                                }
                                else
                                    CacheManager.Instance.Put(key, valKey, methodReturn.ReturnValue);
                            }
                            else
                                CacheManager.Instance.Add(key, valKey, methodReturn.ReturnValue);
                            return methodReturn;
                        }
                        catch (Exception ex)
                        {
                            return new VirtualMethodReturn(input, ex);
                        }
                    case CachingMethod.Remove:
                        try
                        {
                            var removeKeys = cachingAttribute.CorrespondingMethodNames;
                            foreach (var removeKey in removeKeys)
                            {
                                string delKey = prefix + removeKey;
                                if (CacheManager.Instance.Exists(delKey))
                                    CacheManager.Instance.Remove(delKey);
                            }
                            var methodReturn = getNext().Invoke(input, getNext);
                            return methodReturn;
                        }
                        catch (Exception ex)
                        {
                            return new VirtualMethodReturn(input, ex);
                        }
                    default: break;
                }
            }

            return getNext().Invoke(input, getNext);
        }

        /// <summary>
        /// 获取一个<see cref="Boolean"/>值，该值表示当前拦截行为被调用时，是否真的需要执行
        /// 某些操作。
        /// </summary>
        public bool WillExecute
        {
            get { return true; }
        }

        #endregion
    }
}
