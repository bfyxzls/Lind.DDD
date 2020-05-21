using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lind.DDD.OnlinePay.WebPay
{
    /// <summary>
    /// 在线支付调用的实现
    /// </summary>
    public class Demo
    {
        /// <summary>
        /// 充值POST
        /// </summary>
        public void UserRecharge()
        {
            decimal price = 1;
            int userID = 1;
            var alipayImpl = new AlipayImpl();
            alipayImpl.RechargeTo(price, userID);
        }
        /// <summary>
        /// 订单POST
        /// </summary>
        public void UserOrder()
        {
            int userID = 0, sourceType = 0;
            decimal price = 1;
            string orderID = "OR001", name = "zzl";
            var alipayImpl = new AlipayImpl();
            alipayImpl.OnlineTo(orderID, sourceType, userID, price, name,"时间戳tradeid，它是传到支付宝的号");
        }

        #region 第三方回调与本地事件订阅相关
        /// <summary>
        /// 支付宝返回页面
        /// </summary>
        public void AlipayReturn()
        {
            var alipayImpl = new AlipayImpl();
            alipayImpl.Success += Impl_Success;
            alipayImpl.Fail += Impl_Fail;
            alipayImpl.Notify_Return(HttpMethod.Get);
            alipayImpl.Success -= Impl_Success;
            alipayImpl.Fail -= Impl_Fail;

        }
        /// <summary>
        /// 支付宝通知页面
        /// </summary>
        public void AlipayNotify()
        {
            var alipayImpl = new AlipayImpl();
            alipayImpl.Success += Impl_Success;
            alipayImpl.Fail += Impl_Fail;
            alipayImpl.Notify_Return(HttpMethod.Post);
            alipayImpl.Success -= Impl_Success;
            alipayImpl.Fail -= Impl_Fail;
        }
        /// <summary>
        /// 银联返回页面
        /// </summary>
        public void ChinaPayReturn()
        {
            var alipayImpl = new AlipayImpl();
            alipayImpl.Success += Impl_Success;
            alipayImpl.Fail += Impl_Fail;
            alipayImpl.Notify_Return(HttpMethod.Get);
            alipayImpl.Success -= Impl_Success;
            alipayImpl.Fail -= Impl_Fail;

        }
        /// <summary>
        /// 银联通知页面
        /// </summary>
        public void ChinaPayNotify()
        {
            var alipayImpl = new AlipayImpl();
            alipayImpl.Success += Impl_Success;
            alipayImpl.Fail += Impl_Fail;
            alipayImpl.Notify_Return(HttpMethod.Post);
            alipayImpl.Success -= Impl_Success;
            alipayImpl.Fail -= Impl_Fail;
        }
        /// <summary>
        /// 第三方支付如果成功，就解锁余额
        /// </summary>
        /// <param name="e"></param>
        void Impl_Success(OrderEventArgs e)
        {
            if (e.UseType == UseType.Order)
            {
                //订单交易成功
            }
            else
            {
                //充值交易成功
            }
        }
        /// <summary>
        /// 第三方返回失败的逻辑
        /// </summary>
        /// <param name="e"></param>
        void Impl_Fail(OrderEventArgs e)
        {
            if (e.UseType == UseType.Order)
            {
                //订单交易失败
            }
            else
            {
                //充值交易失败
            }
        }
        #endregion
    }
}
