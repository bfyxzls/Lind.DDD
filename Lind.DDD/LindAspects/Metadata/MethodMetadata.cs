using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace Lind.DDD.LindAspects.Metadata
{
    /// <summary>
    /// 方法相关信息
    /// </summary>
    public class MethodMetadata
    {
        /// <summary>
        /// 方法名
        /// </summary>
        private string _methodName;

        public MethodMetadata(string methodName)
        {
            _methodName = methodName;
        }
        /// <summary>
        /// 方法名称
        /// </summary>
        public virtual string MethodName
        {
            get { return _methodName; }
            set { _methodName = value; }
        }
    }
}
