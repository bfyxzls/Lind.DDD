using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Reflection;

namespace Lind.DDD.Utils
{
    /// <summary>
    /// 枚举类型扩展方法
    /// </summary>
    public static class EnumExtensions
    {
        /// <summary>
        /// 得到Flags特性的枚举的集合
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        static List<Enum> GetEnumValuesFromFlagsEnum(Enum value)
        {
            List<Enum> values = Enum.GetValues(value.GetType()).Cast<Enum>().ToList();
            List<Enum> res = new List<Enum>();
            foreach (var itemValue in values)
            {
                if (value.GetHashCode() >= itemValue.GetHashCode())//防止一些左而数小，后面数大的情况，严格规定左而有大数，右面为小数
                    if ((value.GetHashCode() & itemValue.GetHashCode()) > 0
                       || (value.GetHashCode() == 0 && itemValue.GetHashCode() == 0))//输出为0的枚举元素
                        res.Add(itemValue);
            }
            return res;
        }

        /// <summary>  
        /// 获取枚举变量值的 Description 属性  
        /// </summary>  
        /// <param name="obj">枚举变量</param>  
        /// <returns>如果包含 Description 属性，则返回 Description 属性的值，否则返回枚举变量值的名称</returns>  
        public static string GetDescription(this Enum obj)
        {
            string description = string.Empty;
            try
            {
                Type _enumType = obj.GetType();
                DescriptionAttribute dna = null;
                FieldInfo fi = null;
                var fields = _enumType.GetCustomAttributesData();

                if (!fields.Where(i => i.Constructor.DeclaringType.Name == "FlagsAttribute").Any())
                {
                    fi = _enumType.GetField(Enum.GetName(_enumType, obj));
                    dna = (DescriptionAttribute)Attribute.GetCustomAttribute(fi, typeof(DescriptionAttribute));
                    if (dna != null && !string.IsNullOrEmpty(dna.Description))
                        return dna.Description;
                    return null;
                }

                GetEnumValuesFromFlagsEnum(obj).ToList().ForEach(i =>
                {
                    fi = _enumType.GetField(Enum.GetName(_enumType, i));
                    dna = (DescriptionAttribute)Attribute.GetCustomAttribute(fi, typeof(DescriptionAttribute));
                    if (dna != null && !string.IsNullOrEmpty(dna.Description))
                        description += dna.Description + ",";
                });

                return description.EndsWith(",")
                    ? description.Remove(description.LastIndexOf(','))
                    : description;
            }
            catch
            {
                throw;
            }

        }
    }
}
