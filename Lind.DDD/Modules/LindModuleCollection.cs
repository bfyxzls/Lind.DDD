using System;
using System.Collections.Generic;
using System.Linq;
using Lind.DDD.LinqExtensions;
namespace Lind.DDD.Modules
{
    /// <summary>
    /// 模式的集合及集合的方法
    /// </summary>
    internal class LindModuleCollection : List<LindModuleInfo>
    {
        /// <summary>
        /// Gets a reference to a module instance.
        /// </summary>
        /// <typeparam name="TModule">Module type</typeparam>
        /// <returns>Reference to the module instance</returns>
        public TModule GetModule<TModule>() where TModule : LindModule
        {
            var module = this.FirstOrDefault(m => m.Type == typeof(TModule));
            if (module == null)
            {
                throw new ArgumentException("Can not find module for " + typeof(TModule).FullName);
            }

            return (TModule)module.Instance;
        }

        /// <summary>
        /// Sorts modules according to dependencies.
        /// If module A depends on module B, A comes after B in the returned List.
        /// </summary>
        /// <returns>Sorted list</returns>
        public List<LindModuleInfo> GetSortedModuleListByDependency()
        {
            return this.SortByDependencies(x => x.Dependencies).ToList();
        }
    }




}