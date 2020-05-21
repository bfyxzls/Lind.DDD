using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lind.DDD.EntityValidation
{
    /// <summary>
    /// 校验提供者
    /// 如果实体继承了EntityBase，这个提供者没有用
    /// </summary>
    public class ValidateProvider
    {
        /// <summary>
        /// 需要被校验的实体对象
        /// </summary>
        private object entity;
        /// <summary>
        /// 初始化
        /// </summary>
        /// <param name="entity"></param>
        public ValidateProvider(object entity)
        {
            this.entity = entity;
        }

        /// <summary>
        /// 是否校验通过
        /// </summary>
        public bool IsValid
        {
            get  
            {
                return GetErrorMessages().Count() == 0;
            }
        }

        /// <summary>
        /// 得到校验的错误列表
        /// </summary>
        /// <returns></returns>
        public IEnumerable<RuleViolation> GetErrorMessages()
        {
            foreach (var i in entity.GetType().GetProperties())
            {
                var validArr = i.GetCustomAttributes(typeof(ValidationAttribute), false);
                foreach (var validItem in validArr)
                {
                    var val = (validItem as ValidationAttribute);
                    if (val != null)
                        if (!val.IsValid(i.GetValue(entity)))
                        {
                            string info = val.ErrorMessage;
                            if (string.IsNullOrWhiteSpace(info))
                                info = val.FormatErrorMessage(i.Name);
                            yield return new RuleViolation(info, i.Name);
                        }
                }
            }
        }
    }
}
