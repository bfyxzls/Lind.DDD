using Lind.DDD.Authorization;
using Lind.DDD.IoC;
using Lind.DDD.IRepositories;
using Lind.DDD.Utils.Http;
using Lind.DDD.Web.Enums;
using Lind.DDD.Web.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Web.Mvc;

namespace Lind.DDD.Web.Controllers
{
    /// <summary>
    /// 商城中心
    /// </summary>
    //[Lind.DDD.SSO.SSOActionFilter()]
    public class ShopController : BaseController
    {
        IExtensionRepository<Product> productRepository;
        IExtensionRepository<UserAccount> userAccountRepository;
        IExtensionRepository<Category> categoryRepository;
        IExtensionRepository<OrderInfo> orderInfoRepository;
        List<SelectListItem> cateList;
        public ShopController()
        {
            userAccountRepository = ServiceLocator.Instance.GetService<IExtensionRepository<UserAccount>>();
            productRepository = ServiceLocator.Instance.GetService<IExtensionRepository<Product>>();
            categoryRepository = ServiceLocator.Instance.GetService<IExtensionRepository<Category>>();
            orderInfoRepository = ServiceLocator.Instance.GetService<IExtensionRepository<OrderInfo>>();
            cateList = new List<SelectListItem>();
            foreach (var i in categoryRepository
                .GetModel()
                .ToDictionary(i => i.Id.ToString(), j => j.Name))
            {
                cateList.Add(new SelectListItem { Text = i.Value, Value = i.Key });
            }
        }

        /// <summary>
        /// 商品列表
        /// </summary>
        /// <returns></returns>
        [AllowAnonymous]
        public ActionResult Index()
        {
            return View(productRepository.GetModel().Include(i => i.Category));
        }

        /// <summary>
        /// 购买
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult Do(int id)
        {
            ViewBag.Money = (userAccountRepository.Find(i => i.UserInfoId == UserId) ?? new UserAccount()).Money;

            return View(productRepository.GetModel()
                                         .Include(i => i.Category)
                                         .FirstOrDefault(i => i.Id == id));
        }

        /// <summary>
        /// 购买提交
        /// </summary>
        /// <param name="form"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult Do(FormCollection form)
        {
            var orderDetail = new List<OrderDetail>();

            foreach (var id in form["Id"].Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
            {
                int productId;
                int.TryParse(id, out productId);

                var product = productRepository.Find(productId);

                orderDetail.Add(new OrderDetail()
                 {
                     Price = product.Price,
                     ProductId = productId,
                     ProductName = product.Name,
                     SaleCount = 1,
                     UserInfoId = product.UserInfoId,
                     UserInfoUserName = product.UserInfoUserName,
                     StartTime = DateTime.Now,
                     EndTime = DateTime.Now.AddYears(1)
                 });
            }

            var order = new OrderInfo
            {
                OrderStatus = OrderStatus.Created,
                OrderDetail = orderDetail,
                UserInfoId = Convert.ToInt32(CurrentUser.UserID),
                UserInfoName = CurrentUser.UserName
            };
            orderInfoRepository.Insert(order);
            var account = userAccountRepository.Find(i => i.UserInfoId == UserId);
            account.Money = account.Money - order.OrderPrice < 0
                ? 0
                : account.Money - order.OrderPrice;
            userAccountRepository.Update(account);

            CurrentUser.Serialize(
                CurrentUser.UserID,
                CurrentUser.UserName,
                extInfo: account.Money.ToString(),
                role: CurrentUser.Role);

            CookieHelper.Remove("MyCart");
            return RedirectToAction("OrderSuccess");
        }

        /// <summary>
        /// 订单成功
        /// </summary>
        /// <returns></returns>
        public ActionResult OrderSuccess()
        {
            return View();
        }

        /// <summary>
        /// 订单列表
        /// </summary>
        /// <param name="orderStatus"></param>
        /// <returns></returns>
        [AllowAnonymous]
        public ActionResult OrderList(string orderStatus)
        {
            ViewBag.OrderStatus = orderStatus;

            var linq = orderInfoRepository.GetModel()
                                          .Include(i => i.OrderDetail);

            if (Request.HttpMethod == "POST")
                if (!string.IsNullOrWhiteSpace(orderStatus))
                {
                    int status;
                    int.TryParse(orderStatus, out status);
                    linq = linq.Where(i => (int)i.OrderStatus == status);
                }
            return View(linq);
        }

        /// <summary>
        /// 加入购物车
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [AllowAnonymous]
        public ActionResult DoCart(int id)
        {
            var list = CookieHelper.Read<List<Cart>>("MyCart") ?? new List<Cart>();
            var product = productRepository.Find(id);
            list.Add(new Cart
            {
                ProductId = id,
                ProductName = Url.Encode(product.Name),
                Price = product.Price
            });
            CookieHelper.Write("MyCart", list);
            return Json(true);
        }

        /// <summary>
        /// 我的购物车
        /// </summary>
        /// <returns></returns>
        [AllowAnonymous]
        public ActionResult MyCart()
        {
            ViewBag.Money = (userAccountRepository.Find(i => i.UserInfoId == UserId) ?? new UserAccount()).Money;
            return PartialView("PartialMyCart", CookieHelper.Read<List<Cart>>("MyCart"));
        }

        /// <summary>
        /// 购物车内商品数量
        /// </summary>
        /// <returns></returns>
        [AllowAnonymous]
        public int GetCartCount()
        {
            return (CookieHelper.Read<List<Cart>>("MyCart") ?? new List<Cart>()).Count();
        }
        /// <summary>
        /// 清空购物车
        /// </summary>
        /// <returns></returns>
        [AllowAnonymous]
        public bool ClearCart()
        {
            CookieHelper.Remove("MyCart");
            return true;
        }
    }
}
