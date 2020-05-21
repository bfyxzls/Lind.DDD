using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Reflection;
using System.Web;
using Microsoft.Practices.Unity;
using Microsoft.Practices.Unity.Configuration;
using System.IO;
using Microsoft.Practices.Unity.InterceptionExtension;
using Lind.DDD.Caching;
using Lind.DDD.CachingDataSet;
using Lind.DDD.IoC.Implements;
using Autofac;
using Autofac.Configuration;

namespace Lind.DDD.IoC
{

    /// <summary>
    /// 服务定位器
    /// 作者：仓储大叔
    /// 集成：unity & autofac，统一的容器接口IContainer
    /// </summary>
    public sealed class ServiceLocator : IServiceProvider
    {
        #region Private Fields
        private readonly IContainer _container;
        #endregion

        #region Private Static Fields
        private static readonly ServiceLocator instance = new ServiceLocator();
        #endregion

        #region Ctor
        /// <summary>
        /// Initializes a new instance of ServiceLocator class.
        /// </summary>
        private ServiceLocator()
        {

            #region Unity注册
            Action<IUnityContainer> unityAction = _container =>
            {
                UnityConfigurationSection section = (UnityConfigurationSection)ConfigurationManager.GetSection("unity");
                if (section == null)
                {
                    var unityConfig = System.AppDomain.CurrentDomain.BaseDirectory + @"\IoC.config";
                    var fileMap = new ExeConfigurationFileMap() { ExeConfigFilename = unityConfig };
                    var configuration = ConfigurationManager.OpenMappedExeConfiguration(fileMap, ConfigurationUserLevel.None);
                    section = (UnityConfigurationSection)configuration.GetSection("unity");
                }

                if (section == null)
                    throw new ArgumentException("请配置unity节点...");

                #region 装载config中的类型
                if (section != null)
                    section.Configure(_container);
                #endregion

                #region 注册动态类型
                LoadDynamicType(_container);
                #endregion
            };
            #endregion

            #region Autofac注册
            Action<AutofacAdapterContainer, ContainerBuilder> autofacAction = (_container, builder) =>
            {
                builder.RegisterModule(new ConfigurationSettingsReader("autofac"));
                _container.container = builder.Build();
            };
            #endregion

            switch (ConfigConstants.ConfigManager.Config.IocContaion.IoCType)
            {
                case 0:
                    /*
                     * unity实现比较容易，直接将config的对象装载到容器即可
                     */
                    _container = new UnityAdapterContainer();
                    unityAction((IUnityContainer)_container);
                    break;
                case 1:
                    /*
                     * autofac的实现，将先建立ContainerBuilder,然后注册config的对象，最后为它的AutofacAdapterContainer的contaion属性赋值
                     */
                    var builder = new ContainerBuilder();
                    _container = new AutofacAdapterContainer(builder);
                    autofacAction((AutofacAdapterContainer)_container, builder);
                    break;
                default:
                    throw new ArgumentException("不支持此IoC类型");
            }

        }


        #endregion

        #region Public Static Properties
        /// <summary>
        /// Gets the singleton instance of the ServiceLocator class.
        /// </summary>
        public static ServiceLocator Instance
        {
            get { return instance; }
        }

        #endregion

        #region Private Methods
        /// <summary>
        /// 装载一批动态的类型
        /// Author:zhangzhanling
        /// Date:2015-04-03
        /// </summary>
        private void LoadDynamicType(IUnityContainer _container)
        {
            //unity动态类型注入，各个程序集用,分开，支持*通配符号
            string unityDynamicAssembly = System.Configuration.ConfigurationManager.AppSettings["unityDynamicAssembly"];
            //是否同时启动数据集缓存策略
            string unityCachingDoing = System.Configuration.ConfigurationManager.AppSettings["unityCachingDoing"];
            InjectionMember[] injectionMembers = new InjectionMember[] { };
            if (unityCachingDoing == "1")
            {
                injectionMembers = new InjectionMember[] { new Interceptor<InterfaceInterceptor>(), new InterceptionBehavior<CachingBehavior>() };
            }
            if (!string.IsNullOrWhiteSpace(unityDynamicAssembly))
            {
                Array.ForEach(unityDynamicAssembly.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries), dllName =>
                {
                    var baseDir = AppDomain.CurrentDomain.BaseDirectory;
                    if (System.Web.HttpContext.Current != null)
                    {
                        baseDir += "bin";
                    }
                    var files = Directory.GetFiles(baseDir, dllName);
                    var iTypes = new List<Type>();
                    foreach (var file in files)
                    {
                        var interfaceASM = Assembly.LoadFrom(Path.Combine(baseDir, file));
                        var types = from t in interfaceASM.GetTypes()
                                    where !string.IsNullOrWhiteSpace(t.Namespace)
                                    select t;

                        foreach (var type in types)
                        {
                            if (type.GetInterfaces() != null && type.GetInterfaces().Any())
                                foreach (var father in type.GetInterfaces())
                                {
                                    _container.RegisterType(father
                                        , type
                                        , injectionMembers);
                                }
                        }
                    }
                });

            }

        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Gets the service instance with the given type.
        /// </summary>
        /// <typeparam name="T">The type of the service.</typeparam>
        /// <returns>The service instance.</returns>
        public T GetService<T>()
        {
            return _container.Resolve<T>();
        }
        /// <summary>
        /// Gets the service instance with the given type by using the overrided arguments.
        /// </summary>
        /// <typeparam name="T">The type of the service.</typeparam>
        /// <param name="overridedArguments">The overrided arguments.</param>
        /// <returns>The service instance.</returns>
        public T GetService<T>(object overridedArguments)
        {
            var overrides = Utils.GetParameterOverrides(overridedArguments);
            return _container.Resolve<T>(overrides.ToArray());
        }
        /// <summary>
        /// Gets the service instance with the given type by using the overrided arguments.
        /// </summary>
        /// <param name="serviceType">The type of the service.</param>
        /// <param name="overridedArguments">The overrided arguments.</param>
        /// <returns>The service instance.</returns>
        public object GetService(Type serviceType, object overridedArguments)
        {
            var overrides = Utils.GetParameterOverrides(overridedArguments);
            return _container.Resolve(serviceType, overrides.ToArray());
        }
        #endregion

        #region IServiceProvider Members
        /// <summary>
        /// Gets the service instance with the given type.
        /// </summary>
        /// <param name="serviceType">The type of the service.</param>
        /// <returns>The service instance.</returns>
        public object GetService(Type serviceType)
        {
            return _container.Resolve(serviceType);
        }

        #endregion
    }
}