using System;
using System.Collections.Generic;
using System.Text;
using Lind.DDD.LindAspects.Metadata;
using System.Reflection;

namespace Lind.DDD.LindAspects
{
    /// <summary>
    /// 拦截所需要的上下文对象
    /// </summary>
    public class InvokeContext
    {
 

        ResultMetadata _result;
        public ResultMetadata Result
        {
            get
            {
                return _result;
            }
        }

        ExceptionMetadata _ex;
        public ExceptionMetadata Ex
        {
            get
            {
                return _ex;
            }
        }

        List<ParameterMetadata> _parameters;
        public ParameterMetadata[] Parameters
        {
            get
            {
                if (_parameters != null)
                {
                    return _parameters.ToArray();
                }

                return null;
            }
        }

        MethodMetadata _method;
        public MethodMetadata Method
        {
            get
            {

                return _method;
            }
        }

        public void SetParameter(object parameter)
        {

            if (_parameters == null)
            {
                lock (this)
                {
                    if (_parameters == null)
                    {
                        _parameters = new List<ParameterMetadata>();
                    }
                }
            }

            _parameters.Add(new ParameterMetadata(parameter));
        }

        public void SetError(Exception e)
        {
            _ex = new ExceptionMetadata(e);
        }

        public void SetResult(object result)
        {
            _result = new ResultMetadata(result);
        }

        public void SetMethod(string methodName)
        {
            _method = new MethodMetadata(methodName);
        }
    }
}
