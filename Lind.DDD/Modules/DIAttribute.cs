using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lind.DDD.Modules
{
    /// <summary>
    /// 依靠注入的模块化
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
    class DIAttribute : Attribute
    {
        /// <summary>
        /// 所依赖的类型
        /// </summary>
        public Type[] DependedModuleTypes { get; private set; }

        /// <summary>
        /// Used to define dependencies of an ABP module to other modules.
        /// </summary>
        /// <param name="dependedModuleTypes">Types of depended modules</param>
        public DIAttribute(params Type[] dependedModuleTypes)
        {
            DependedModuleTypes = dependedModuleTypes;
        }
    }
}
