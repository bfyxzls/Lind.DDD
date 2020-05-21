using System;
using System.Collections.Generic;
using System.Text;

namespace Lind.DDD.LindAspects.Metadata
{
    /// <summary>
    /// 方法返回值，在AfterAspectAttribute拦截时才可以拿到
    /// </summary>
    public class ResultMetadata
    {
        private object _result;

        public ResultMetadata(object result)
        {
            _result = result;
        }

        public virtual object Result
        {
            get { return _result; }
            set { _result = value; }
        }
    }
}
