using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lind.DDD.OnlinePay.Weixin.Results
{
    public class ResultBase
    {
        private ResultBuilder ResultBuilder { get; set; }
        public ResultBase(ResultBuilder resultBuilder)
        {
            ResultBuilder = resultBuilder;
            OriginResultXml = resultBuilder.OriginResultXml;
        }

        /// <summary>
        /// 是否成功
        /// </summary>
        public bool Success
        {
            get
            {
                ResultStatus status = (ResultStatus)GetEnumValue<ResultStatus>("result_code");
                if (status == ResultStatus.SUCCESS)
                    return true;
                else
                    return false;
            }
        }
        /// <summary>
        /// 错误代码
        /// </summary>
        public ErrorCode? ErrorCode { get { return (ErrorCode?)GetEnumValue<ErrorCode?>("err_code"); } }
        /// <summary>
        /// 错误代码描述
        /// </summary>
        public string ErrorMessage { get { return GetValue("err_code_des"); } }

        public string OriginResultXml { get; set; }

        protected string GetValue(string name)
        {
            return ResultBuilder.GetParam(name);
        }
        protected int GetIntValue(string name)
        {
            string value = ResultBuilder.GetParam(name);
            if (string.IsNullOrEmpty(value))
                return 0;
            return Convert.ToInt32(value);
        }
        protected bool GetBooleanValue(string name)
        {
            string value = ResultBuilder.GetParam(name);
            if (string.IsNullOrEmpty(value))
                return false;
            value = value.ToLower();
            if (value == "y" || value == "true" || value == "yes")
                return true;
            return false;
        }

        protected object GetEnumValue<T>(string name)
        {
            string valueString = GetValue(name);
            try
            {
                object enumValue = Enum.Parse(typeof(T), valueString);
                return enumValue;
            }
            catch { }
            return null;
        }
    }
}
