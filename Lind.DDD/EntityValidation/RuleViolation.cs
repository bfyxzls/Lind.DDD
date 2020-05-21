using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Lind.DDD.EntityValidation
{
    /// <summary>
    /// 验证信息实体
    /// </summary>
    public class RuleViolation
    {
        /// <summary>
        /// 错误信息
        /// </summary>
        public string ErrorMessage { get; private set; }
        /// <summary>
        /// 引起错误的属性
        /// </summary>
        public string PropertyName { get; private set; }

        /// <summary>
        /// 初始化验证信息实体
        /// </summary>
        /// <param name="errorMessage">错误信息</param>
        public RuleViolation(string errorMessage)
        {
            ErrorMessage = errorMessage;
        }
        /// <summary>
        /// 初始化验证信息实体
        /// </summary>
        /// <param name="errorMessage">错误信息</param>
        /// <param name="propertyName">引起错误的属性</param>
        public RuleViolation(string errorMessage, string propertyName)
        {
            ErrorMessage = errorMessage;
            PropertyName = propertyName;
        }
        /// <summary>
        /// 重写方法
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return string.Format(ErrorMessage, PropertyName);
        }

    }
}
