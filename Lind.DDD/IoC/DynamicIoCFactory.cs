using Microsoft.Practices.Unity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lind.DDD.IoC
{
    /// <summary>
    /// 动态IOC生产者，一般根据配置的（数据库，文件)来生产对象
    /// 非单例，每次使用会构建新的对象
    /// </summary>
    public class DynamicIoCFactory
    {

        /// <summary>
        /// 生产对象
        /// </summary>
        /// <typeparam name="I"></typeparam>
        /// <param name="class">类型完成名称</param>
        /// class类型完整名称说明
        /// var t = Type.GetType("Lind.DDD.ConfigConstants.ConfigModel,Lind.DDD");//一般接口
        /// var tGeneric = Type.GetType("Lind.DDD.Test.Hello`1,Lind.DDD.Test");//泛型接口
        /// tGeneric = Type.GetType("Lind.DDD.Repositories.Xml.XmlRepository`1,Lind.DDD.Repositories.Xml");//拿到泛型类型
        /// tGeneric = tGeneric.MakeGenericType(typeof(Lind.DDD.ConfigConstants.ConfigModel));//注册泛型叁数
        /// tGeneric = Type.GetType("Lind.DDD.Repositories.Xml.XmlRepository`1[[Lind.DDD.ConfigConstants.ConfigModel,Lind.DDD]],Lind.DDD.Repositories.Xml");//拿到泛型类型
        /// <returns></returns>
        public static I GetService<I>(string @class)
        {
            using (IUnityContainer container = new UnityContainer())
            {
                var tGeneric = Type.GetType(@class);//拿到泛型类型
                container.RegisterType(typeof(I), tGeneric);//注意类型与实现的关系
                return container.Resolve<I>();//生产对象
            }
        }
    }
}
