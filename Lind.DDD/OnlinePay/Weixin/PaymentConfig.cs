using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lind.DDD.OnlinePay.Weixin
{
    public class PaymentConfig
    {
        /// <summary>
        /// 公众账号ID  微信公众号身份的唯一标识。审核通过后，在微信发送的邮件中查看
        /// </summary>
        public string AppId { get; set; }
        /// <summary>
        /// 受理商ID，身份标识
        /// </summary>
        public string Mch_Id { get; set; }
        /// <summary>
        /// 商户支付密钥Key。审核通过后，在微信发送的邮件中查看
        /// </summary>
        public string Key { get; set; }
        /// <summary>
        /// JSAPI接口中获取openid，审核后在公众平台开启开发模式后可查看
        /// </summary>
        public string AppSecret { get; set; }
        /// <summary>
        /// 获取access_token过程中的跳转uri，通过跳转将code传入jsapi支付页面
        /// </summary>
        public string JS_API_CALL_URL { get; set; }
        /// <summary>
        /// 证书路径,注意应该填写绝对路径
        /// </summary>
        public string CertPath { get; set; }
        public string SSLKEY_PATH { get; set; }
        public string CertPassword { get; set; }

        /// <summary>
        /// 异步通知url，商户根据实际开发过程设定
        /// </summary>
        public string NOTIFY_URL { get; set; }

        /// <summary>
        /// 设备号  (非必填)
        /// </summary>
        public string Device_Info { get; set; }

        public int TimeOut { get; set; }
        /// <summary>
        /// 统一支付接口
        /// </summary>
        public string UnifiedOrderUrl { get; set; }
        /// <summary>
        /// 订单查询接口
        /// </summary>
        public string OrderQueryUrl { get; set; }

        public static void Init(PaymentConfig config)
        {
            _instance = config;
        }

        public static PaymentConfig Instance {
            get
            {
                if (_instance == null)
                {
                    //TODO
                    //需要改成从配置文件读取
                    //_instance = new PaymentConfig();
                    //_instance.UnifiedOrderUrl = "https://api.mch.weixin.qq.com/pay/unifiedorder";
                    //_instance.TimeOut = 30;
                    //_instance.OrderQueryUrl = "https://api.mch.weixin.qq.com/pay/orderquery";
                    throw new Exception("微信支付配置没有初始化！");
                }
                return _instance;
            }
        }
        private static PaymentConfig _instance;

    }
}
