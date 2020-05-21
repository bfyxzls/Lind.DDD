using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Lind.DDD.Modules
{
    /// <summary>
    /// Lind.DDD的模块化
    /// </summary>
    public abstract class LindModule
    {

        /// <summary>
        /// IoC容器
        /// </summary>
        internal protected IoC.IContainer IocManager = IoC.IoCFactory.Instance.CurrentContainer;

        /// <summary>
        /// 是否为Lind模块
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static bool IsLindModule(Type type)
        {
            return
                type.IsClass &&
                !type.IsAbstract &&
                typeof(LindModule).IsAssignableFrom(type);
        }

        /// <summary>
        /// Finds depended modules of a module.
        /// </summary>
        /// <param name="moduleType"></param>
        /// <returns></returns>
        public static List<Type> FindDependedModuleTypes(Type moduleType)
        {
            if (!IsLindModule(moduleType))
            {
                throw new ArgumentException("This type is not an ABP module: " + moduleType.AssemblyQualifiedName);
            }

            var list = new List<Type>();

            if (moduleType.IsDefined(typeof(DIAttribute), true))
            {
                var dependsOnAttributes = moduleType.GetCustomAttributes(typeof(DIAttribute), true).Cast<DIAttribute>();
                foreach (var dependsOnAttribute in dependsOnAttributes)
                {
                    foreach (var dependedModuleType in dependsOnAttribute.DependedModuleTypes)
                    {
                        list.Add(dependedModuleType);
                    }
                }
            }

            return list;
        }

        /// <summary>
        /// 模块预加载的程序集
        /// </summary>
        public virtual void PreInitialize()
        {
            Console.WriteLine("{0}预初始化", this.GetType());
            Logger.LoggerFactory.Instance.Logger_Debug(this.GetType() + ".PreInitialize被调用");
        }

        /// <summary>
        /// 模块自己的程序集
        /// </summary>
        public virtual void Initialize()
        {
            Console.WriteLine("{0}初始化", this.GetType());
            Logger.LoggerFactory.Instance.Logger_Debug(this.GetType() + ".Initialize被调用");
        }

        /// <summary>
        /// 卸载模块，非托管需要自己去实现，托管由CLR控制
        /// </summary>
        public virtual void Dispose()
        {
            Logger.LoggerFactory.Instance.Logger_Debug(this.GetType() + ".PreInitialize被调用");
        }
    }
}
