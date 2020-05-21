using Lind.DDD.Domain;
using Lind.DDD.Events;
using Lind.DDD.IoC;
using Lind.DDD.IRepositories;
using Lind.DDD.Web.Enums;
using Lind.DDD.Web.EventHandlers;
using Lind.DDD.Web.Events;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lind.DDD.Web.Models
{
    /// <summary>
    /// 订单领域实体
    /// </summary>
    [Serializable]
    public class OrderInfo : Entity
    {
        IExtensionRepository<OrderDetail> orderDetailRepository;
        public OrderInfo()
        {
            orderDetailRepository = ServiceLocator.Instance.GetService<IExtensionRepository<OrderDetail>>();
        }

        #region 基元属性
        [DisplayName("订单状态"), Required]
        public OrderStatus OrderStatus { get; set; }
        [DisplayName("价格"), Required]
        public decimal OrderPrice { get; set; }
        /// <summary>
        /// 手动加外键，外键表名+主键
        /// </summary>
        [DisplayName("用户ID"), Required]
        public int UserInfoId { get; set; }
        [DisplayName("用户名"), Required]
        public string UserInfoName { get; set; }
        #endregion

        public UserInfo UserInfo { get; set; }
        private IList<OrderDetail> orderDetail;
        public IList<OrderDetail> OrderDetail
        {
            get { return orderDetail; }
            set
            {
                orderDetail = value;

                if (orderDetail != null && orderDetail.Count > 0) //同时为订单总价赋值,这个不应该在数据模型，而应该在领域模型
                    OrderPrice = orderDetail.Sum(i => i.Price * i.SaleCount);
            }
        }
        /// <summary>
        /// 发货
        /// </summary>
        public void Dispatched()
        {
            EventBus.Instance.Publish(new OrderDispatchedEvent
            {
                UserId = this.UserInfoId,
                OrderId = this.Id
            });
        }

        /// <summary>
        /// 付款
        /// </summary>
        public void Paid()
        {
            this.OrderDetail = orderDetailRepository.GetModel(i => i.Id == this.Id).ToList();
            foreach (var item in this.OrderDetail)
            {
                EventBus.Instance.Publish(new OrderPaidEvent
                {
                    UserId = item.UserInfoId,
                    OrderId = this.Id
                });
            }

        }

        /// <summary>
        /// 拣货
        /// </summary>
        public void Picked()
        {
            EventBus.Instance.Publish(new OrderPickedEvent
            {
                UserId = this.UserInfoId,
                OrderId = this.Id
            });
        }

        /// <summary>
        /// 签收
        /// </summary>
        public void Signed()
        {
            this.OrderDetail = orderDetailRepository.GetModel(i => i.Id == this.Id).ToList();
            foreach (var item in this.OrderDetail)
            {
                EventBus.Instance.Publish(new OrderSignedEvent
                {
                    UserId = item.UserInfoId,
                    OrderId = this.Id
                });
            }

        }
    }

}
