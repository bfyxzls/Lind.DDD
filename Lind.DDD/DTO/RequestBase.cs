using Lind.DDD.EntityValidation;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Lind.DDD.DTO
{
    /// <summary>
    /// 请求体基类
    /// </summary>
    public abstract class RequestBase
    {
        /// <summary>
        /// 请求初始化
        /// </summary>
        public RequestBase()
        {
            GuidKey = Guid.NewGuid().ToString();
            queuePredicate = i => i.Name != "IsValid"
            && i.Name != "ContainFields"
            && i.Name != "Page"
            && i.Name != "Sort"
            && i.Name != "GuidKey"
            && i.GetValue(this) != null;
        }

        /// <summary>
        /// 以属性作为查询条件,去掉为空的属性和公用属性
        /// </summary>
        private Func<PropertyInfo, bool> queuePredicate;

        #region 公用属性，不进行参数过滤
        /// <summary>
        /// 本次请求唯一标示
        /// </summary>
        public string GuidKey { get; set; }
        /// <summary>
        /// 分页参数，页码和每页显示的记录数
        /// 例：Page=1,5，表示获取第一页，每页显示5条
        /// </summary>
        public string Page { get; set; }
        /// <summary>
        /// 排序相关 1 升序,-1 降序
        /// 例：Sort=email|1,username-1
        /// </summary>
        public string Sort { get; set; }
        /// <summary>
        /// 需要返回的字段，其它字段将不会被序列化，这些字段使用,分开
        /// 例：ContainFields=username,realname,email
        /// </summary>
        public string ContainFields { get; set; }
        /// <summary>
        /// 数据验证(是否成功)
        /// 虚属性，子类可以根据自己的逻辑去复写
        /// </summary>
        public virtual bool IsValid
        {
            get
            {
                return this.GetRuleViolations() == null ||
                       this.GetRuleViolations().Count() == 0;
            }
        }


        #endregion

        #region Methods
        /// <summary>
        /// 得到对象的属性，以键值对的方式返回
        /// </summary>
        /// <returns></returns>
        public IEnumerable<KeyValuePair<string, object>> GetProperyiesDictionary()
        {
            var properties = this.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance)
                     .Where(queuePredicate)
                     .ToArray();

            foreach (var i in properties)
                yield return new KeyValuePair<string, object>(i.Name, i.GetValue(this));

        }
        /// <summary>
        /// 拿到由客户端来的分页参数，构建成分页查询对象
        /// </summary>
        /// <returns></returns>
        public Paging.PageParameters GetPageParameters()
        {
            try
            {
                var linq = Page.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                int pageIndex, pageSize;
                var index = int.TryParse(linq[0], out pageIndex);
                var size = int.TryParse(linq[1], out pageSize);
                return new Paging.PageParameters(index ? pageIndex : 1, size ? pageSize : 10);
            }
            catch (Exception)
            {

                return new Paging.PageParameters(1, 10);
            }

        }
        /// <summary>
        /// 得到排序参数字典，以键值对的方式返回
        /// </summary>
        /// <returns></returns>
        public IEnumerable<KeyValuePair<string, object>> GetSortDictionary()
        {
            if (!string.IsNullOrWhiteSpace(Sort))
            {
                var sortFields = Sort.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                foreach (var item in sortFields)
                {
                    var values = item.Split(new char[] { '|' }, StringSplitOptions.RemoveEmptyEntries);
                    if (values.Length == 2 && (Convert.ToInt32(values[1]) == 1 || Convert.ToInt32(values[1]) == -1))
                    {
                        int val;
                        int.TryParse(values[1], out val);
                        if (val == 1 || val == -1)
                            yield return new KeyValuePair<string, object>(values[0], val);
                    }
                }
            }
        }
        /// <summary>
        /// 获取验证失败的信息枚举,默认提供了非空验证，派生类可以根据自己的需要去复写这个方法
        /// 个性化验证同样使用yield return返回到IEnumberable列表中
        /// 它使用了简单的迭代器,如果GetRuleViolations有错误则返回迭代列表
        /// </summary> 
        /// <returns></returns>
        public IEnumerable<RuleViolation> GetRuleViolations()
        {
            var properties = this.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance)
            .Where(queuePredicate)
            .ToArray();

            foreach (var i in properties)
            {
                var attr = i.GetCustomAttributes();
                foreach (var a in attr)
                {
                    var val = (a as ValidationAttribute);
                    if (val != null)
                        if (!val.IsValid(i.GetValue(this)))
                        {
                            yield return new RuleViolation(val.ErrorMessage, i.Name);
                        }
                }
            }

        }

        /// <summary>
        /// 得到由GetRuleViolations()方法产生的验证消息集合
        /// 实际项目中，可以自己去规定，本方法为虚方法，派生类可以重写
        /// </summary>
        /// <returns></returns>
        public virtual string GetRuleViolationMessages()
        {

            if (GetRuleViolations() != null && GetRuleViolations().Count() > 0)
            {
                return string.Join(",", GetRuleViolations().Select(i => i.ToString()));//json序列化时出现问题
            }
            return string.Empty;

        }
        #endregion
    }
}
