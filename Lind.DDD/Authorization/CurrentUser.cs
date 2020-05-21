using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lind.DDD.Authorization
{
    /// <summary>
    /// 当前登陆的用户信息
    /// 可以有Redis Session和Session进行实现
    /// </summary>
    public class CurrentUser
    {
        #region Public Properties
        /// <summary>
        /// 当然登陆的用户ID
        /// </summary>
        public static string UserID
        {
            get
            {
                return (System.Web.HttpContext.Current.Session["UserID"] ?? string.Empty).ToString();
            }
        }
        /// <summary>
        /// 当前登陆的用户名
        /// </summary>
        public static string UserName
        {
            get
            {
                return (System.Web.HttpContext.Current.Session["UserName"] ?? string.Empty).ToString();
            }
        }
        /// <summary>
        /// 用户角色
        /// </summary>
        public static string Role
        {
            get
            {
                return (System.Web.HttpContext.Current.Session["Role"] ?? string.Empty).ToString();
            }
        }
        /// <summary>
        /// 用户权限
        /// 增，删，改，查，冻结，审批
        /// </summary>
        public static Authority Authority
        {
            get
            {
                int val = 0;
                int.TryParse((System.Web.HttpContext.Current.Session["Authority"] ?? string.Empty).ToString(), out val);
                return (Authority)val;
            }
        }
        /// <summary>
        /// 用户所在的部门（组织结构）
        /// </summary>
        public static string Department
        {
            get
            {
                return (System.Web.HttpContext.Current.Session["Department"] ?? string.Empty).ToString();
            }
        }
        /// <summary>
        /// 当前登陆用户存储的扩展信息
        /// </summary>
        public static string ExtInfo
        {
            get
            {
                return (System.Web.HttpContext.Current.Session["ExtInfo"] ?? string.Empty).ToString();
            }
        }
        /// <summary>
        /// 是否登陆
        /// </summary>
        public static bool IsLogin
        {
            get
            {
                return !string.IsNullOrWhiteSpace(UserID);
            }
        }
        #endregion

        #region Public Methods
        /// <summary>
        /// 退出登陆
        /// </summary>
        public static void Exit()
        {
            System.Web.HttpContext.Current.Session.Abandon();//清除全部Session
        }

        /// <summary>
        /// 将用户信息持久化到Session
        /// </summary>
        /// <param name="userID">用户编号</param>
        /// <param name="userName">用户名</param>
        /// <param name="extInfo">扩展信息</param>
        /// <param name="role">角色</param>
        /// <param name="department">部门</param>
        /// <param name="authority">权限</param>
        public static void Serialize(
            string userID,
            string userName,
            string extInfo = "",
            string role = "",
            string department = "",
            int authority = 0)
        {
            System.Web.HttpContext.Current.Session["UserID"] = userID;
            System.Web.HttpContext.Current.Session["UserName"] = userName;
            System.Web.HttpContext.Current.Session["ExtInfo"] = extInfo;
            System.Web.HttpContext.Current.Session["Role"] = role;
            System.Web.HttpContext.Current.Session["Authority"] = authority;
            System.Web.HttpContext.Current.Session["Department"] = department;
        }

        #endregion

    }
}
