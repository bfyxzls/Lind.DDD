using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lind.DDD.OnlinePay.Weixin
{
    public enum TradeType
    {
        /// <summary>
        /// JS API（网页内）支付,公众号内付款,微信浏览器
        /// </summary>
        [Display(Name = "JS API（网页内）支付")]
        JSAPI,
        /// <summary>
        /// Native（原生）支付，可以生成二次维
        /// </summary>
        [Display(Name = "Native（原生）支付")]
        NATIVE,
        /// <summary>
        /// App支付
        /// </summary>
        [Display(Name = "App支付")]
        APP
    }

    #region 付款状态
    public enum TradeState
    {
        /// <summary>
        /// 支付成功
        /// </summary>
        [Display(Name = "支付成功")]
        SUCCESS,
        /// <summary>
        /// 转入退款
        /// </summary>
        [Display(Name = "转入退款")]
        REFUND,
        /// <summary>
        /// 未支付
        /// </summary>
        [Display(Name = "未支付")]
        NOTPAY,
        /// <summary>
        /// 已关闭
        /// </summary>
        [Display(Name = "已关闭")]
        CLOSED,
        /// <summary>
        /// 已撤销
        /// </summary>
        [Display(Name = "已撤销")]
        REVOKED,
        /// <summary>
        /// 用户支付中
        /// </summary>
        [Display(Name = "用户支付中")]
        USERPAYING,
        /// <summary>
        /// 未支付(输入密码或确认支付超时)
        /// </summary>
        [Display(Name = "未支付(输入密码或确认支付超时)")]
        NOPAY,
        /// <summary>
        /// 支付失败(其他原因，如银行返回失败)
        /// </summary>
        [Display(Name = "支付失败(其他原因，如银行返回失败)")]
        PAYERROR,
    }
    #endregion

    public enum ResultStatus
    {
        [Display(Name = "成功")]
        SUCCESS,
        [Display(Name = "失败")]
        FAIL,
    }

    #region 错误代码
    public enum ErrorCode
    {
        /// <summary>
        /// 接口后台错误
        /// </summary>
        [Display(Name = "接口后台错误")]
        SYSTEMERROR,
        /// <summary>
        /// 无效 transaction_id
        /// </summary>
        [Display(Name = "无效 transaction_id")]
        INVALID_TRANSACTIONID,
        /// <summary>
        /// 提交参数错误
        /// </summary>
        [Display(Name = "提交参数错误")]
        PARAM_ERROR,
        /// <summary>
        /// 订单已支付
        /// </summary>
        [Display(Name = "订单已支付")]
        ORDERPAID,
        /// <summary>
        /// 商户订单号重复
        /// </summary>
        [Display(Name = "商户订单号重复")]
        OUT_TRADE_NO_USED,
        /// <summary>
        /// 商户无权限
        /// </summary>
        [Display(Name = "商户无权限")]
        NOAUTH,
        /// <summary>
        /// 余额不足
        /// </summary>
        [Display(Name = "余额不足")]
        NOTENOUGH,
        /// <summary>
        /// 不支持卡类型
        /// </summary>
        [Display(Name = "不支持卡类型")]
        NOTSUPORTCARD,
        /// <summary>
        /// 订单已关闭
        /// </summary>
        [Display(Name = "订单已关闭")]
        ORDERCLOSED,
        /// <summary>
        /// 银行系统异常
        /// </summary>
        [Display(Name = "银行系统异常")]
        BANKERROR,
        /// <summary>
        /// 退款金额大于支付金额
        /// </summary>
        [Display(Name = "退款金额大于支付金额")]
        REFUND_FEE_INVALID,
        /// <summary>
        /// 订单不存在
        /// </summary>
        [Display(Name = "订单不存在")]
        ORDERNOTEXIST
    }

    #endregion

}
