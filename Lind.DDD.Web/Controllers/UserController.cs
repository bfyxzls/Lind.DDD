using Lind.DDD.Authorization;
using Lind.DDD.Authorization.Mvc;
using Lind.DDD.IoC;
using Lind.DDD.IRepositories;
using Lind.DDD.Specification;
using Lind.DDD.UoW;
using Lind.DDD.Web.Enums;
using Lind.DDD.Web.Filters;
using Lind.DDD.Web.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Web;
using System.Web.Mvc;

namespace Lind.DDD.Web.Controllers
{
    /// <summary>
    /// 用户中心
    /// </summary>
    public class UserController : BaseController
    {

        IExtensionRepository<OrderInfo> orderRepository;
        IExtensionRepository<UserAccount> userAccountRepository;
        IExtensionRepository<UserInfo> userRepository;
        IMongoRepository<ActionLog> logRepository;
        IExtensionRepository<UserExtension> userExtRepository;
        public UserController()
        {
            userAccountRepository = ServiceLocator.Instance.GetService<IExtensionRepository<UserAccount>>();
            orderRepository = ServiceLocator.Instance.GetService<IExtensionRepository<OrderInfo>>();
            userRepository = ServiceLocator.Instance.GetService<IExtensionRepository<UserInfo>>();
            userExtRepository = ServiceLocator.Instance.GetService<IExtensionRepository<UserExtension>>();
            logRepository = ServiceLocator.Instance.GetService<IMongoRepository<ActionLog>>();
            userExtRepository.SetDataContext(db);
            userRepository.SetDataContext(db);
            userAccountRepository.SetDataContext(db);
        }
        #region 当前用户相关
        /// <summary>
        /// 用户设置
        /// </summary>
        /// <returns></returns>
        public ActionResult Setting()
        {
            int userid;
            int.TryParse(CurrentUser.UserID, out userid);
            return View(userRepository.Find(userid));
        }

        /// <summary>
        /// 我的订单
        /// </summary>
        /// <param name="orderStatus"></param>
        /// <returns></returns>
        public ActionResult MyOrder(string orderStatus)
        {
            ViewBag.OrderStatus = orderStatus;
            int id = Convert.ToInt16(CurrentUser.UserID);
            var linq = orderRepository.GetModel(i => i.UserInfo.Id == id);
            if (Request.HttpMethod == "POST")
                if (!string.IsNullOrWhiteSpace(orderStatus))
                {
                    int status;
                    int.TryParse(orderStatus, out status);
                    linq = linq.Where(i => (int)i.OrderStatus == status);
                }
            return View(linq);
        }
        #endregion

        #region 管理员相关


        [ManagerFilter]
        public ActionResult Index(string keyword, int page = 1)
        {


            ViewBag.keyword = keyword;
            Expression<Func<UserInfo, bool>> predicate = i => true;
            if (!string.IsNullOrWhiteSpace(keyword))
                predicate = ExpressionBuilder.And(predicate, (i => i.UserName == keyword));

            //entity include
            var linq = userRepository.GetModel()
                                     .Include(i => i.UserExtension)
                                     .Include(i => i.UserAccount)
                                     .Where(predicate);

            return View(linq.OrderBy(i => i.Id).AsQueryable().ToPagedList(page, PageSize));
        }

        [ManagerFilter]
        public ActionResult Create()
        {
            return View();
        }
        [ManagerFilter]
        [HttpPost]
        public ActionResult Create(UserInfo entity)
        {
            entity.UserExtension = new UserExtension
            {
                School = "北京大学",
                NickName = entity.RealName,
            };
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
                entity.Password = entity.Md5Password();
                userRepository.Insert(entity);
                return RedirectToAction("Index");
            }
        }
        [ManagerFilter]
        public ActionResult Edit(int id)
        {
            var account = userAccountRepository.Find(i => i.UserInfoId == id);
            ViewBag.AccountMoney = account == null ? 0 : account.Money;
            return View(userRepository.GetModel()
                                      .Include(i => i.UserExtension)
                                      .Include(i => i.UserAccount)
                                      .FirstOrDefault(i => i.Id == id));
        }
        [HttpPost]
        [ManagerFilter]
        public ActionResult Edit(UserInfo entity)
        {
            decimal money;
            decimal.TryParse(Request.Form["AccountMoney"], out money);
            //UoW机制
            var account = userAccountRepository.Find(i => i.UserInfoId == entity.Id);
            account.Money = money;
            unitOfWork.RegisterChangeded(account, SqlType.Update, userAccountRepository);

            entity.UserExtension = null;
            unitOfWork.RegisterChangeded(entity, SqlType.Update, userRepository);

            var userExtension = userExtRepository.Find(entity.Id);
            userExtension.NickName = Request.Form["UserExtension.NickName"];
            userExtension.School = Request.Form["UserExtension.School"];
            unitOfWork.RegisterChangeded(userExtension, SqlType.Update, userExtRepository);

            unitOfWork.Commit();
            return RedirectToAction("Index");
        }
        public ActionResult Details(int id)
        {
            return View(userRepository.Find(id));
        }
        [ManagerFilter]
        public ActionResult Delete(int id)
        {
            userRepository.Delete(new UserInfo { Id = id });
            userExtRepository.Delete(new UserExtension { Id = id });
            return RedirectToAction("Index");
        }
        #endregion
    }
}
