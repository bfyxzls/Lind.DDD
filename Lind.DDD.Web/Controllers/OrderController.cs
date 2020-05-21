using Lind.DDD.IoC;
using Lind.DDD.IRepositories;
using Lind.DDD.Web.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Lind.DDD.Authorization;
using Lind.DDD.Web.Enums;
using Lind.DDD.Authorization.Mvc;
using Lind.DDD.OnlinePay.Weixin;
using Lind.DDD.Utils;
namespace Lind.DDD.Web.Controllers
{
    /// <summary>
    /// 订单控制器
    /// </summary>
    public class OrderController : BaseController
    {
        IExtensionRepository<Product> productRepository = ServiceLocator.Instance.GetService<IExtensionRepository<Product>>();
        IExtensionRepository<OrderInfo> orderInfoRepository = ServiceLocator.Instance.GetService<IExtensionRepository<OrderInfo>>();

        #region 订单列表
        /// <summary>
        /// 订单列表
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult Index(int id)
        {
            return View(productRepository.GetModel().Include(i => i.Category).FirstOrDefault(i => i.Id == id));
        }
        [HttpPost]
        public ActionResult Index(FormCollection form)
        {
            int productId;
            int.TryParse(form["Id"], out productId);
            int saleCount;
            int.TryParse(form["SaleCount"], out saleCount);

            var product = productRepository.Find(productId);
            var orderDetail = new OrderDetail()
            {
                Price = product.Price,
                ProductId = productId,
                ProductName = product.Name,
                SaleCount = saleCount,
                UserInfoId = product.UserInfoId,
                UserInfoUserName = product.UserInfoUserName,
            };
            var order = new OrderInfo
            {
                OrderStatus = OrderStatus.Created,
                OrderDetail = new List<OrderDetail>() { orderDetail },
                UserInfoId = UserId,
                UserInfoName = CurrentUser.UserName
            };
            orderInfoRepository.Insert(order);


            return RedirectToAction("MyOrder", "User");
        }
        #endregion

        #region 付款，拣款，发货，签改
        /// <summary>
        /// 买家付款
        /// </summary>
        /// <returns></returns>
        public ActionResult Paid(int id)
        {

            //写日志
            Lind.DDD.Logger.LoggerFactory.Instance.Logger_Info("买家付款，订单号：" + id);

            #region 更新到数据库
            var entity = orderInfoRepository.Find(id);
            entity.OrderStatus = OrderStatus.Paid;
            orderInfoRepository.Update(entity);
            #endregion

            //写队列
            //Lind.DDD.CachingQueue.QueueManager.Instance.Push(Utils.SerializeMemoryHelper.SerializeToBinary("买家付款，订单号：" + id));

            //事件总线
            entity.Paid();
            return RedirectToAction("OrderList", "Shop");
        }
        /// <summary>
        /// 卖家拣款
        /// </summary>
        /// <returns></returns>
        public ActionResult Picked(int id)
        {
            var entity = orderInfoRepository.Find(id);
            entity.OrderStatus = OrderStatus.Picked;
            orderInfoRepository.Update(entity);
            entity.Picked();
            return RedirectToAction("OrderList", "Shop");
        }
        /// <summary>
        /// 卖家发货
        /// </summary>
        /// <returns></returns>
        public ActionResult Dispatched(int id)
        {
            var entity = orderInfoRepository.Find(id);
            entity.OrderStatus = OrderStatus.Dispatched;
            orderInfoRepository.Update(entity);
            entity.Dispatched();
            return RedirectToAction("OrderList", "Shop");
        }
        /// <summary>
        /// 买家签收
        /// </summary>
        /// <returns></returns>
        public ActionResult Signed(int id)
        {
            var entity = orderInfoRepository.Find(id);
            entity.OrderStatus = OrderStatus.Signed;
            orderInfoRepository.Update(entity);
            entity.Signed();
            return RedirectToAction("OrderList", "Shop");
        }
        #endregion

        #region 微信支付测试
        WxPayImpl wxPayImpl = new WxPayImpl();
        /// <summary>
        /// 发送支付请求
        /// </summary>
        [AllowAnonymous]
        public void Send()
        {
            WxPayImpl wxPayImpl = new WxPayImpl();
            //HttpResponseBase Response = this.Response;
            // QRCodeHelper.OutPutQRCodeImage(wxPayImpl.RechargeTo("order"+DateTime.Now.ToString("yyyyMMddHHmmss"), 500, 1), Response);
            Response.Redirect(wxPayImpl.RechargeTo("order" + DateTime.Now.ToString("yyyyMMddHHmmss"), 1, 1));
        }
        [AllowAnonymous]
        public ActionResult WeiXin()
        {
            return Content("weixin hello!");
        }
        public void Notify()
        {
            wxPayImpl.RecieveWxPayNotify((orderid) =>
            {
                //微信处理成功后的逻辑
            }, (msg) => { });
        }
        #endregion


    }
}
