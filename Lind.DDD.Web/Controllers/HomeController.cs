using Lind.DDD.Domain;
using Lind.DDD.Web.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data.Entity;
using Lind.DDD.Repositories.EF;
using Lind.DDD.IRepositories;
using Lind.DDD.Authorization;
using Lind.DDD.Paging;
using Lind.DDD.IoC;
using Lind.DDD.UoW;
using Lind.DDD.Authorization.Mvc;
using Lind.DDD.Specification;
using System.Linq.Expressions;
using Lind.DDD.Web.ViewModels;
using Lind.DDD.Web.Enums;
using System.Threading.Tasks;
using System.Threading;

namespace Lind.DDD.Web.Controllers
{

    /// <summary>
    /// 用户控制器
    /// </summary>
    public class HomeController : BaseController
    {

        #region 构造方法
        public HomeController()
        {

            userRepository = ServiceLocator.Instance.GetService<IExtensionRepository<UserInfo>>();
            userAccountRepository = ServiceLocator.Instance.GetService<IExtensionRepository<UserAccount>>();
            userExtRepository = ServiceLocator.Instance.GetService<IExtensionRepository<UserExtension>>();
            logRepository = ServiceLocator.Instance.GetService<IMongoRepository<ActionLog>>();
            userExtRepository.SetDataContext(db);
            userRepository.SetDataContext(db);

        }
        #endregion

        #region 变量 ThreadPool.QueueUserWorkItem
        IMongoRepository<ActionLog> logRepository;
        IExtensionRepository<UserInfo> userRepository;
        IExtensionRepository<UserAccount> userAccountRepository;
        IExtensionRepository<UserExtension> userExtRepository;
        #endregion

        [AllowAnonymous]
        public ActionResult Register()
        {
            return View();
        }
        [AllowAnonymous]
        [HttpPost]
        public ActionResult Register(UserInfo entity)
        {
            entity.UserExtension = new UserExtension
            {
                School = "不用填",
                NickName = entity.RealName,
            };
            if (!ModelState.IsValid)
            {
                ModelState.AddModelError("", "请把表单填写完整!");
                return View();
            }
            if (!entity.Password_TruePassword())
            {
                ModelState.AddModelError("", "两次密码需要一致!");
                return View();
            }
            else if (entity.IsExistUser())
            {
                ModelState.AddModelError("", "用户已经存在!");
                return View();
            }
            else
            {
                try
                {
                    entity.Role = UserRole.User;
                    entity.Password = entity.Md5Password();
                    userRepository.Insert(entity);

                    //送的虚拟货币
                    userAccountRepository.Insert(new UserAccount
                    {
                        UserInfoId = entity.Id,
                        FreezeMoney = 0,
                        Money = 50,
                        TotalMoney = 50,
                        UserAccountDetail = new List<UserAccountDetail>() {
                          new UserAccountDetail{
                              Memo="新用户注册送50元！",
                              Money=50,Type=UserAccountDetailType.In
                          }
                        }
                    });

                    //模拟登陆
                    CurrentUser.Serialize(entity.Id.ToString(), entity.UserName, extInfo: "50", role: entity.Role.ToString());
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", ex.Message);
                    return View();
                }

                return RedirectToAction("Index", "Shop");
            }
        }

        #region 登陆，登出，修改密码
        [AllowAnonymous]
        public ActionResult Login(string returnUrl)
        {
            ViewBag.returnUrl = returnUrl;
            return View();
        }
        [AllowAnonymous, HttpPost]
        public ActionResult Login(string returnUrl, FormCollection form)
        {
            var entity = new UserInfo { UserName = form["username"], Password = form["password"] }.Login();
            if (entity == null)
            {
                ModelState.AddModelError("", "用户名密码不正确");
                return View();
            }
            CurrentUser.Serialize(
                entity.Id.ToString(),
                entity.UserName,
                extInfo: userAccountRepository.Find(i => i.UserInfoId == entity.Id).Money.ToString(),
                role: entity.Role.ToString());

            if (string.IsNullOrWhiteSpace(returnUrl))
                return RedirectToAction("Index", "User");
            else
                return Redirect(returnUrl);
        }

        /// <summary>
        /// 登出
        /// </summary>
        /// <returns></returns>
        public ActionResult Logout()
        {
            Lind.DDD.Authorization.CurrentUser.Exit();
            return RedirectToAction("Login", "Home");
        }
        /// <summary>
        /// 修改密码
        /// </summary>
        /// <returns></returns>
        public ActionResult ModifyPassword()
        {
            return View();
        }
        [HttpPost]
        public ActionResult ModifyPassword(ChangePasswordModel model)
        {
            if (ModelState.IsValid)
            {

                // 在某些出错情况下，ChangePassword 将引发异常，
                // 而不是返回 false。
                bool changePasswordSucceeded = false;
                var oldpassword = Lind.DDD.Utils.Encryptor.Utility.EncryptString(model.OldPassword, Utils.Encryptor.Utility.EncryptorType.MD5);
                var old = userRepository.Find(i => i.UserName == CurrentUser.UserName && i.Password == oldpassword);
                if (old != null)
                {
                    old.Password = Lind.DDD.Utils.Encryptor.Utility.EncryptString(model.NewPassword, Utils.Encryptor.Utility.EncryptorType.MD5);
                    userRepository.Update(old);
                    changePasswordSucceeded = true;
                }
                if (changePasswordSucceeded)
                {
                    return RedirectToAction("ChangePasswordSuccess");
                }
                else
                {
                    ModelState.AddModelError("", "当前密码不正确或新密码无效。");
                }
            }

            // 如果我们进行到这一步时某个地方出错，则重新显示表单
            return View(model);
        }
        /// <summary>
        /// 密码修改成功
        /// </summary>
        /// <returns></returns>
        public ActionResult ChangePasswordSuccess()
        {
            return View();
        }


        #endregion


    }
}
