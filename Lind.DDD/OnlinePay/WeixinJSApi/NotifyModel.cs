using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lind.DDD.OnlinePay.WeixinJSApi
{
    /// <summary>
    /// 微信回调数据模型
    /// </summary>
    public class NotifyModel
    {
        /// <summary>
        /// 当次交易存储到微信平台的订单号
        /// </summary>
        public string Transaction_Id { get; set; }
        /// <summary>
        /// (业务系统本身的订单号，它会发到微信，并且微信回调时再返回来)系统本身生成的订单号
        /// </summary>
        public string Out_Trade_No { get; set; }
        /// <summary>
        /// 对应当前公众号的用户OpenId
        /// </summary>
        public string OpenId { get; set; }
        /// <summary>
        /// 微信用户唯一标识
        /// </summary>
        public string UniqueId { get; set; }
        /// <summary>
        /// 回调是否成功
        /// </summary>
        public bool IsSuccess { get; set; }
        /// <summary>
        /// 回调消息
        /// </summary>
        public string NotifyMessage { get; set; }
    }
}
