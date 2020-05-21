using System;
using System.Collections.Generic;
using System.Reflection;

namespace Lind.DDD.Modules
{
    /// <summary>
    /// Ä£¿éÄÚÈÝ
    /// </summary>
    internal class LindModuleInfo
    {
        /// <summary>
        /// The assembly which contains the module definition.
        /// </summary>
        public Assembly Assembly { get; private set; }

        /// <summary>
        /// Type of the module.
        /// </summary>
        public Type Type { get; private set; }

        /// <summary>
        /// Instance of the module.
        /// </summary>
        public LindModule Instance { get; private set; }

        /// <summary>
        /// All dependent modules of this module.
        /// </summary>
        public List<LindModuleInfo> Dependencies { get; private set; }

        public LindModuleInfo(LindModule instance)
        {
            Dependencies = new List<LindModuleInfo>();
            Type = instance.GetType();
            Instance = instance;
            Assembly = Type.Assembly;
        }

        public override string ToString()
        {
            return string.Format("{0}", Type.AssemblyQualifiedName);
        }
    }
}