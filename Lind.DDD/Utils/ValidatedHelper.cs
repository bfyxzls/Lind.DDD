using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using System.Linq;

namespace Lind.DDD.Utils
{
    /// <summary>
    /// 验证 助手类
    /// </summary>
    public class ValidatedHelper
    {
        #region 检测是否符合email格式
        /// <summary>
        /// 检测是否符合email格式
        /// </summary>
        /// <param name="strEmail">要判断的email字符串</param>
        /// <returns>判断结果</returns>
        public static bool IsValidEmail(string strEmail)
        {
            return Regex.IsMatch(strEmail, @"^([\w-\.]+)@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.)|(([\w-]+\.)+))([a-zA-Z]{2,4}|[0-9]{1,3})(\]?)$");
        }
        #endregion

        #region 检测是否是正确的Url
        /// <summary>
        /// 检测是否是正确的Url
        /// </summary>
        /// <param name="strUrl">要验证的Url</param>
        /// <returns>判断结果</returns>
        public static bool IsURL(string strUrl)
        {
            return Regex.IsMatch(strUrl, @"^(http|https)\://([a-zA-Z0-9\.\-]+(\:[a-zA-Z0-9\.&%\$\-]+)*@)*((25[0-5]|2[0-4][0-9]|[0-1]{1}[0-9]{2}|[1-9]{1}[0-9]{1}|[1-9])\.(25[0-5]|2[0-4][0-9]|[0-1]{1}[0-9]{2}|[1-9]{1}[0-9]{1}|[1-9]|0)\.(25[0-5]|2[0-4][0-9]|[0-1]{1}[0-9]{2}|[1-9]{1}[0-9]{1}|[1-9]|0)\.(25[0-5]|2[0-4][0-9]|[0-1]{1}[0-9]{2}|[1-9]{1}[0-9]{1}|[0-9])|localhost|([a-zA-Z0-9\-]+\.)*[a-zA-Z0-9\-]+\.(com|edu|gov|int|mil|net|org|biz|arpa|info|name|pro|aero|coop|museum|[a-zA-Z]{1,10}))(\:[0-9]+)*(/($|[a-zA-Z0-9\.\,\?\'\\\+&%\$#\=~_\-]+))*$");
        }
        #endregion

        #region 是否为ip
        /// <summary>
        /// 是否为ip
        /// </summary>
        /// <param name="ip">IP 字符串</param>
        /// <returns>bool</returns>
        public static bool IsIP(string ip)
        {
            return Regex.IsMatch(ip, @"^((2[0-4]\d|25[0-5]|[01]?\d\d?)\.){3}(2[0-4]\d|25[0-5]|[01]?\d\d?)$");

        }

        /// <summary>
        /// 是否为ip
        /// </summary>
        /// <param name="ip">IPSect 字符串</param>
        /// <returns>bool</returns>
        public static bool IsIPSect(string ip)
        {
            return Regex.IsMatch(ip, @"^((2[0-4]\d|25[0-5]|[01]?\d\d?)\.){2}((2[0-4]\d|25[0-5]|[01]?\d\d?|\*)\.)(2[0-4]\d|25[0-5]|[01]?\d\d?|\*)$");

        }
        #endregion

        #region 检测是否有Sql危险字符
        /// <summary>
        /// 检测是否有Sql危险字符
        /// </summary>
        /// <param name="str">要判断字符串</param>
        /// <returns>判断结果</returns>
        public static bool IsSafeSqlString(string str)
        {

            return !Regex.IsMatch(str, @"[-|;|,|\/|\(|\)|\[|\]|\}|\{|%|@|\*|!|\']");
        }
        #endregion

        #region 检测用户名称是否有危险
        /// <summary>
        /// 检测用户名称是否有危险
        /// </summary>
        /// <param name="str">要判断字符串</param>
        /// <returns>判断结果</returns>
        public static bool IsSafeUserInfoString(string str)
        {
            return !Regex.IsMatch(str, @"^\s*$|^c:\\con\\con$|[%,\*" + "\"" + @"\s\t\<\>\&]|游客|^Guest");
        }
        #endregion

        #region 验证数组中的值是否可以转化为指定的类型

        /// <summary>
        /// 验证数组中的值是否可以转化为指定的类型
        /// </summary>
        /// <typeparam name="T">T</typeparam>
        /// <param name="strArray">要验证的字符串数组</param>
        /// <returns>验证者</returns>
        static public Validator<T> IsCorrectArray<T>(string[] strArray)
        {
            var linq = from str in strArray
                       let tempValue = System.Web.HttpUtility.UrlDecode(str)
                       let tempRealValue = Convert.ChangeType(tempValue, typeof(T))
                       select new
                       {
                           Value = tempValue,
                           IsVaild = (tempRealValue != null),
                           RealValue = tempRealValue
                       };
            Validator<T> v = new Validator<T>(linq.Where(item => !item.IsVaild).Select((item, _index) => new _Validator
            {
                Value = item.Value,
                Index = _index
            }));

            if (v.IsValid)
            {
                v.SetCorrectValue(linq.Select((item) => (T)item.RealValue).ToArray());
            }
            return v;
        }


        #endregion

        #region 长度验证
        /// <summary>
        /// 验证数组每一项的长度是否符合给定的标准
        /// </summary>
        /// <param name="strArray">要验证的字符串数组</param>
        /// <param name="minLength">最小长度</param>
        /// <param name="maxLength">最大长度</param>
        /// <returns>验证者</returns>
        static public Validator<string> StringArrayIsLength(string[] strArray, int minLength, int maxLength)
        {
            var linq = from str in strArray
                       let tempValue = System.Web.HttpUtility.UrlDecode(str)
                       select new
                       {
                           Value = tempValue,
                           IsVaild = tempValue.Length >= minLength && tempValue.Length <= maxLength,
                       };
            Validator<string> v = new Validator<string>(linq.Where(item => !item.IsVaild).Select((item, _index) => new _Validator
            {
                Value = item.Value,
                Index = _index
            }));
            v.SetCorrectValue(linq.Select((item) => item.Value).ToArray());
            return v;
        }


        #endregion

        #region 验证数值是否为NULL
        /// <summary>
        /// 验证数值是否为NULL
        /// </summary>
        /// <param name="objArr">参数数目可变处采用参数</param>
        /// <returns>验证者</returns>
        static public IsNullValidator IsNull(params  object[] objArr)
        {
            return new IsNullValidator(objArr.Where(item => item == null).Select((item, index) => index).ToArray());
        }

        #endregion

    }

    /// <summary>
    /// 验证器
    /// </summary>
    public class Validator<T>
    {
        /// <summary>
        /// 是否验证成功
        /// </summary>
        public bool IsValid { get { return this._Validator().Count() == 0; } }
        IEnumerable<_Validator> _validator { get; set; }
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="_validator"></param>
        public Validator(IEnumerable<_Validator> _validator)
        {
            this._validator = _validator;
        }
        /// <summary>
        /// 验证消息序列
        /// </summary>
        public IEnumerable<_Validator> _Validator()
        {
            return _validator;
        }

        T[] correctValue = null;
        /// <summary>
        /// 设置确的值
        /// </summary>
        /// <param name="array">数组</param>
        internal void SetCorrectValue(T[] array)
        {
            if (this.IsValid)
            {
                this.correctValue = array;
            }
            else
            {
                this.correctValue = null;
            }
        }
        /// <summary>
        /// 获取正确的数组
        /// </summary>
        /// <returns>如果验证没有完全通过返回 null</returns>
        public T[] GetCorrectValue()
        {
            return this.correctValue;
        }
    }
    /// <summary>
    /// 是否为空[专用]验证器
    /// </summary>
    public class IsNullValidator
    {
        /// <summary>
        /// 是否验证成功
        /// </summary>
        public bool IsVaild { get { return index.Length == 0; } }
        int[] index;
        /// <summary>
        /// 索引
        /// </summary>
        public int[] Index { get { return this.index; } }
        /// <summary>
        /// 设置 不成功的索引值数组
        /// </summary>
        /// <param name="index">不成功的索引值数组</param>
        internal void SetIndexArr(int[] index)
        {
            this.index = index;
        }
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="index">不成功的索引值数组</param>
        public IsNullValidator(int[] index)
        {
            this.SetIndexArr(index);
        }

    }

    /// <summary>
    /// 验证者
    /// </summary>
    public class _Validator
    {
        /// <summary>
        /// 值
        /// </summary>
        public string Value { get; set; }
        /// <summary>
        /// 不符合条件的值所在数组中的索引
        /// </summary>
        public int Index { get; set; }
    }
}
