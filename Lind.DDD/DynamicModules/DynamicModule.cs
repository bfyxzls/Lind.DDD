using Autofac;
using Lind.DDD.Logger;
using Lind.DDD.Logger.Implements;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lind.DDD.DynamicModules
{
    /// <summary>
    /// 设计一个模块化注册机制
    /// </summary>
    public class DynamicModule
    {

        public static DynamicModule Instance { get; private set; }
        private DynamicModule() { }
        static ContainerBuilder builder;
        public static DynamicModule Create()
        {
            Instance = new DynamicModule();
            return Instance;
        }

        /// <summary>
        /// 注册全局组件
        /// </summary>
        /// <returns></returns>
        public DynamicModule RegisterGlobalModule()
        {
            this.RegisterModule<ILogger, NormalLogger>();
            return this;
        }
        /// <summary>
        /// 注册泛型类型
        /// </summary>
        /// <typeparam name="TService">接口</typeparam>
        /// <typeparam name="TImplement">实现</typeparam>
        /// <returns></returns>
        public DynamicModule RegisterGenericModule(Type service, Type implement)
        {
            LindContainer.Instance.RegisterGeneric(implement).As(service);
            return this;
        }
        /// <summary>
        /// 注册普通类型
        /// </summary>
        /// <typeparam name="TService"></typeparam>
        /// <typeparam name="TImplement"></typeparam>
        /// <returns></returns>
        public DynamicModule RegisterModule<TService, TImplement>()
            where TService : class
            where TImplement : TService
        {
            LindContainer.Instance.RegisterType(typeof(TImplement)).As(typeof(TService));
            return this;
        }

    }
}
