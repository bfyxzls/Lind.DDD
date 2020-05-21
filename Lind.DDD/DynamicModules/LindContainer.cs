using Autofac;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lind.DDD.DynamicModules
{
    /// <summary>
    /// Lind框架对象容器
    /// </summary>
    public class LindContainer
    {
        /// <summary>
        /// 容器的生产者
        /// </summary>
        public static ContainerBuilder Instance
        {
            get
            {
                if (instance == null)
                {
                    lock (lockObj)
                    {
                        if (instance == null)
                            instance = new ContainerBuilder();
                    }
                }
                return instance;
            }
        }
        /// <summary>
        ///容器的消费者
        /// </summary>
        static IContainer Container
        {
            get
            {
                if (container == null)
                {
                    lock (lockObj)
                    {
                        if (container == null)
                            container = instance.Build();
                    }
                }
                return container;
            }
        }

        /// <summary>
        /// 从容器中拿出接口对应的对象
        /// </summary>
        /// <typeparam name="TImpelment"></typeparam>
        /// <returns></returns>
        public static TImpelment Resolve<TImpelment>()
        {
            return Container.Resolve<TImpelment>();
        }
        static IContainer container;
        static ContainerBuilder instance;
        static object lockObj = new object();
    }
}
