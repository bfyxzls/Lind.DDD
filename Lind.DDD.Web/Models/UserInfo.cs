using Lind.DDD.Domain;
using Lind.DDD.IoC;
using Lind.DDD.IRepositories;
using Lind.DDD.Utils.Encryptor;
using Lind.DDD.Web.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using System.Web.DynamicData;

namespace Lind.DDD.Web.Models
{
    /// <summary>
    /// 用户实体模式
    /// </summary>
    public class UserInfo : Entity
    {
        IExtensionRepository<UserInfo> userRepository;
        public UserInfo()
        {
        }
        #region 表元属性
        /// <summary>
        /// maxLength和stringLength都可以用来设置数据表字段的长度，stringLength不可以用来做ＭＶＣ验证
        /// </summary>
        [DisplayName("用户名"), Required]// StringLength(50, MinimumLength = 4, ErrorMessage = "用户名只能由~50个字符组成")
        public string UserName { get; set; }
        [DisplayName("真实姓名"), Required]//StringLength(30, MinimumLength = 6, ErrorMessage = "真实姓名只能由6~30个字符组成")
        public string RealName { get; set; }
        [DisplayName("密码"), Required]// StringLength(20, MinimumLength = 6, ErrorMessage = "密码由6~20个字符组成")
        public string Password { get; set; }
        [DisplayName("电子邮件"), Required, EmailAddress]
        public string Email { get; set; }
        [DisplayName("QQ号"), Required]
        public string QQ { get; set; }
        [DisplayName("角色"), Required]
        public UserRole Role { get; set; }
        #endregion

        #region 导航属性
        [DisplayName("确认密码"), NotMapped]
        public virtual string TruePassword { get; set; }
        public List<OrderInfo> OrderInfo { get; set; }
        public UserExtension UserExtension { get; set; }
        public IList<UserCollection> UserCollection { get; set; }
        public IList<UserAccount> UserAccount { get; set; }
        #endregion

        #region 充血模型的方法
        /// <summary>
        /// 生成md5密码
        /// </summary>
        /// <returns></returns>
        public string Md5Password()
        {
            return Utility.EncryptString(Password, Utility.EncryptorType.MD5);
        }
        /// <summary>
        /// 密码和确认密码的比较
        /// </summary>
        /// <returns></returns>
        public bool Password_TruePassword()
        {
            return this.Password.Equals(this.TruePassword, StringComparison.CurrentCulture);
        }
        /// <summary>
        /// 验证用户是否存在
        /// </summary>
        /// <returns></returns>
        public bool IsExistUser()
        {
            userRepository = ServiceLocator.Instance.GetService<IExtensionRepository<UserInfo>>();

            return userRepository.GetModel(i => i.UserName == this.UserName || i.RealName == this.RealName).Count() > 0;
        }
        /// <summary>
        /// 用户登陆
        /// </summary>
        /// <returns></returns>
        public UserInfo Login()
        {
            userRepository = ServiceLocator.Instance.GetService<IExtensionRepository<UserInfo>>();

            string md5 = Md5Password();
            return userRepository.Find(i => i.UserName == this.UserName && i.Password == md5);
        }
        #endregion

    }

}
