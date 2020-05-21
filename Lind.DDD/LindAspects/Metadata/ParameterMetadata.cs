using System;
using System.Collections.Generic;
using System.Text;

namespace Lind.DDD.LindAspects.Metadata
{
    /// <summary>
    /// 方法参数相关信息
    /// </summary>
    public class ParameterMetadata
    {
        private object _para;

        public ParameterMetadata(object para)
        {
            _para = para;
        }

        public virtual object Para
        {
            get { return _para; }
            set { _para = value; }
        }
    }
}
