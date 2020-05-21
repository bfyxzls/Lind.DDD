using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lind.DDD.Web.Enums
{
    /// <summary>
    /// 表示销售订单的状态。
    /// </summary>
    /// <remarks>此处使用枚举类型，是为了在Domain Model中简化编码，毕竟枚举类型要比整数类型更能表达领域含义。但在
    /// Entity Framework的4.3.1版本中，暂没有对枚举类型的支持。
    /// </remarks>
    public enum OrderStatus : int
    {
        /// <summary>
        /// 表示销售订单的已创建状态 - 表明销售订单已被创建（未用）。
        /// </summary>
        [Description("买家未付款")]
        Created = 0,
        /// <summary>
        /// 表示销售订单的已付款状态 - 表明客户已向销售订单付款。
        /// </summary>
        [Description("买家已付款")]
        Paid,
        /// <summary>
        /// 表示销售订单的已拣货状态 - 表明销售订单中包含的商品已从仓库拣货（未用）。
        /// </summary>
        [Description("卖家已拣货")]
        Picked,
        /// <summary>
        /// 表示销售订单的已发货状态。
        /// </summary>
        [Description("卖家已发货")]
        Dispatched,
        /// <summary>
        /// 表示销售订单的已派送状态。
        /// </summary>
        [Description("正在派送")]
        Delivered,
        /// <summary>
        /// 用户已经签收
        /// </summary>
        [Description("买家已签收")]
        Signed
    }
}
