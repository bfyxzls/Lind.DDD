using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lind.DDD.OnlinePay.Weixin
{
    public class SDKRuntimeException:Exception
    {
        public SDKRuntimeException(string message)
            :base(message)
        { 
        }
    }
    public class SignErrorException : Exception
    {
        public SignErrorException(string message)
            : base(message)
        { }
    }

    public class ReturnParamException : Exception
    {
        public ReturnParamException(string message) : base(message) { }
    }

    /// <summary>
    /// 业务出错异常  result_code="FAIL" 
    /// </summary>
    public class ResultErrorException : Exception
    { 
        public string ErrorCode{get;set;}
        public string ErrorMessage{get;set;}
        public ResultErrorException(string errorCode, string message)
            : base(message)
        {
            ErrorCode = errorCode;
            ErrorMessage = message;
        }
    }
    /// <summary>
    /// 通信出错异常   return_code="FAIL" 
    /// </summary>
    public class ReturnErrorException : Exception
    {
        public ReturnErrorException(string message)
            : base(message)
        { }
    }

}
