using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Lind.DDD.Modules
{
    /// <summary>
    /// 模块化的管理者
    /// </summary>
    public class LindModuleManager
    {

        private readonly LindModuleCollection _modules;
        private readonly IoC.IContainer _iocManager;

        public LindModuleManager(IoC.IContainer iocManager)
        {
            _modules = new LindModuleCollection();
            _iocManager = iocManager;
        }

        public virtual void InitializeModules()
        {
            LoadAll();

            var sortedModules = _modules.GetSortedModuleListByDependency();
            sortedModules.ForEach(module => module.Instance.PreInitialize());
            sortedModules.ForEach(module => module.Instance.Initialize());

        }

        public virtual void ShutdownModules()
        {
            var sortedModules = _modules.GetSortedModuleListByDependency();
            sortedModules.Reverse();
            sortedModules.ForEach(sm => sm.Instance.Dispose());
        }

        private void LoadAll()
        {
            //加载所有模型
            var moduleTypes = AddMissingDependedModules(GetAllTypes());

            //Register to IOC container.
            foreach (var moduleType in moduleTypes)
            {
                if (!LindModule.IsLindModule(moduleType))
                {
                    throw new ArgumentException("This type is not an LIND module: " + moduleType.AssemblyQualifiedName);
                }

                if (!_iocManager.IsRegistered(moduleType))
                {
                    _iocManager.RegisterType(moduleType, moduleType);
                }
            }

            //Add to module collection
            foreach (var moduleType in moduleTypes)
            {
                var moduleObject = (LindModule)_iocManager.Resolve(moduleType);

                moduleObject.IocManager = _iocManager;

                _modules.Add(new LindModuleInfo(moduleObject));

            }


            //添加DI依赖项
            SetDependencies();

        }
        /// <summary>
        /// 加载所有类型
        /// </summary>
        /// <returns></returns>
        private List<Type> GetAllTypes()
        {
            var allTypes = new List<Type>();

            foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies().ToList())
            {
                try
                {
                    Type[] typesInThisAssembly;

                    try
                    {
                        //所有模型
                        typesInThisAssembly = assembly.GetTypes().Where(i => LindModule.IsLindModule(i)).ToArray();
                    }
                    catch (ReflectionTypeLoadException ex)
                    {
                        typesInThisAssembly = ex.Types;
                    }
                    allTypes.AddRange(typesInThisAssembly.Where(type => type != null));
                }
                catch (Exception ex)
                {
                    throw new ArgumentException("程序集解析出错" + ex.Message);
                }
            }

            return allTypes;
        }


        /// <summary>
        /// 设置DI特性的模型依赖
        /// </summary>
        private void SetDependencies()
        {
            foreach (var moduleInfo in _modules)
            {
                foreach (var referencedAssemblyName in moduleInfo.Assembly.GetReferencedAssemblies())
                {
                    var referencedAssembly = Assembly.Load(referencedAssemblyName);
                    var dependedModuleList = _modules.Where(m => m.Assembly == referencedAssembly).ToList();
                    if (dependedModuleList.Count > 0)
                    {
                        moduleInfo.Dependencies.AddRange(dependedModuleList);
                    }
                }

                foreach (var dependedModuleType in LindModule.FindDependedModuleTypes(moduleInfo.Type))
                {
                    var dependedModuleInfo = _modules.FirstOrDefault(m => m.Type == dependedModuleType);
                    if (dependedModuleInfo == null)
                    {
                        throw new ArgumentException("Could not find a depended module " + dependedModuleType.AssemblyQualifiedName + " for " + moduleInfo.Type.AssemblyQualifiedName);
                    }

                    if ((moduleInfo.Dependencies.FirstOrDefault(dm => dm.Type == dependedModuleType) == null))
                    {
                        moduleInfo.Dependencies.Add(dependedModuleInfo);
                    }
                }
            }
        }

        /// <summary>
        /// 添加所依赖的模型
        /// </summary>
        /// <param name="allModules"></param>
        /// <returns></returns>
        private static ICollection<Type> AddMissingDependedModules(ICollection<Type> allModules)
        {
            var initialModules = allModules.ToList();
            foreach (var module in initialModules)
            {
                FillDependedModules(module, allModules);
            }

            return allModules;
        }
        /// <summary>
        /// 递归:添加本类型里DI特性的依赖类型
        /// </summary>
        /// <param name="module"></param>
        /// <param name="allModules"></param>
        private static void FillDependedModules(Type module, ICollection<Type> allModules)
        {
            foreach (var dependedModule in LindModule.FindDependedModuleTypes(module))
            {
                if (!allModules.Contains(dependedModule))
                {
                    allModules.Add(dependedModule);
                    FillDependedModules(dependedModule, allModules);
                }
            }
        }
    }

}
