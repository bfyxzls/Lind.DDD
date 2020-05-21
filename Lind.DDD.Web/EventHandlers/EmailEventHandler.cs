using Lind.DDD.Events;
using Lind.DDD.IoC;
using Lind.DDD.IRepositories;
using Lind.DDD.Messaging;
using Lind.DDD.Web.Events;
using Lind.DDD.Web.Models;
namespace Lind.DDD.Web.EventHandlers
{
    /// <summary>
    /// 订单发货的通知
    /// </summary>
    public class EmailEventHandler : 
        IEventHandler<OrderDispatchedEvent>, 
        IEventHandler<OrderPaidEvent>,
        IEventHandler<OrderSignedEvent>,
        IEventHandler<OrderPickedEvent>
    {
        IExtensionRepository<UserInfo> repository = ServiceLocator.Instance.GetService<IExtensionRepository<UserInfo>>();

        #region IEventHandler<OrderDispatchedEvent> 成员

        public void Handle(OrderDispatchedEvent evt)
        {
            //发货通知
            var user = repository.Find(evt.UserId);
            MessageFactory.GetService(MessageType.Email).Send(user.Email, "发货通知", "卖家已经为您发货了，请注意查收，订单号：" + evt.OrderId + "，时间：" + evt.EventTime);
        }

        #endregion

        #region IEventHandler<OrderPaidEvent> 成员

        public void Handle(OrderPaidEvent evt)
        {
            //付款通知
            var user = repository.Find(evt.UserId);
            MessageFactory.GetService(MessageType.Email).Send(user.Email, "付款通知", "买家已经为您付款了，请注意查收，订单号：" + evt.OrderId + "，时间：" + evt.EventTime);
        }

        #endregion

        #region IEventHandler<OrderSignedEvent> 成员

        public void Handle(OrderSignedEvent evt)
        {
            //签收通过
            var user = repository.Find(evt.UserId);
            MessageFactory.GetService(MessageType.Email).Send(user.Email, "签收通过", "买家已经签收了，订单完成，订单号：" + evt.OrderId + "，时间：" + evt.EventTime);

        }

        #endregion

        #region IEventHandler<OrderPickedEvent> 成员

        public void Handle(OrderPickedEvent evt)
        {
            //拣货通知
            var user = repository.Find(evt.UserId);
            MessageFactory.GetService(MessageType.Email).Send(user.Email, "拣货通知", "卖家已经开始拣货了，你的宝宝马上上路，订单号：" + evt.OrderId + "，时间：" + evt.EventTime);

        }

        #endregion
    }
}
