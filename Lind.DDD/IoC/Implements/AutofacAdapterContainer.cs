using Autofac;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lind.DDD.IoC.Implements
{
    /// <summary>
    /// autofac提供的IoC容器
    /// </summary>
    internal class AutofacAdapterContainer : IContainer
    {

        ContainerBuilder builder;
        /// <summary>
        /// 配置文件注入时，使用服务定位器为它进行赋值
        /// </summary>
        internal Autofac.IContainer container { get; set; }
        public AutofacAdapterContainer() : this(new ContainerBuilder()) { }

        public AutofacAdapterContainer(ContainerBuilder containerBuilder)
        {
            builder = containerBuilder;
        }
        public TService Resolve<TService>()
        {
            return container.Resolve<TService>();
        }

        public object Resolve(Type type)
        {
            return container.Resolve(type);
        }

        public TService Resolve<TService>(object overridedArguments)
        {
            return container.Resolve<TService>(Utils.GetParameter(overridedArguments));
        }

        public object Resolve(Type serviceType, object overridedArguments)
        {
            return container.Resolve(serviceType, Utils.GetParameter(overridedArguments));
        }

        public void RegisterType(Type from, Type to)
        {
            builder.RegisterType(to).As(from);
            container = builder.Build();
        }

        public ILifetimeScope BeginLifetimeScope(object tag, Action<ContainerBuilder> configurationAction)
        {
            return builder.Build().BeginLifetimeScope(tag, configurationAction);
        }

        public ILifetimeScope BeginLifetimeScope(Action<ContainerBuilder> configurationAction)
        {
            return builder.Build().BeginLifetimeScope(configurationAction);
        }

        public ILifetimeScope BeginLifetimeScope(object tag)
        {
            return builder.Build().BeginLifetimeScope(tag);
        }

        public ILifetimeScope BeginLifetimeScope()
        {
            return builder.Build().BeginLifetimeScope();
        }

        public event EventHandler<Autofac.Core.Lifetime.LifetimeScopeBeginningEventArgs> ChildLifetimeScopeBeginning;

        public event EventHandler<Autofac.Core.Lifetime.LifetimeScopeEndingEventArgs> CurrentScopeEnding;

        public Autofac.Core.IDisposer Disposer
        {
            get { return builder.Build().Disposer; }
        }

        public event EventHandler<Autofac.Core.Resolving.ResolveOperationBeginningEventArgs> ResolveOperationBeginning;

        public object Tag
        {
            get { return builder.Build().Tag; }
        }

        public Autofac.Core.IComponentRegistry ComponentRegistry
        {
            get { return builder.Build().ComponentRegistry; }
        }

        public object ResolveComponent(Autofac.Core.IComponentRegistration registration, IEnumerable<Autofac.Core.Parameter> parameters)
        {
            return builder.Build().ResolveComponent(registration, parameters);
        }

        public void Dispose()
        {
            builder.Build().Dispose();
        }

        public bool IsRegistered(Type type)
        {
            return builder.Build().IsRegistered(type);
        }
    }
}
