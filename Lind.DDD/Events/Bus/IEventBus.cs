using System;
namespace Lind.DDD.Events
{
    /// <summary>
    /// 事件总线，生产者接口
    /// </summary>
    public interface IEventBus
    {
        /// <summary>
        ///  发布事件，支持异步事件
        /// </summary>
        /// <typeparam name="TEvent"></typeparam>
        /// <param name="event"></param>
        void Publish<TEvent>(TEvent @event) where TEvent : class, IEventData;
        /// <summary>
        ///  发布事件
        /// event参数为关键字,所以加了@符
        /// </summary>
        /// <typeparam name="TEvent"></typeparam>
        /// <param name="event"></param>
        /// <param name="callback"></param>
        /// <param name="timeout"></param>
        void Publish<TEvent>(TEvent @event, Action<TEvent, bool, Exception> callback, TimeSpan? timeout = null) where TEvent : class, IEventData;
        /// <summary>
        ///显式的异步发布事件,不需要为处理程序加HandlesAsynchronouslyAttribute
        /// </summary>
        /// <typeparam name="TEvent"></typeparam>
        /// <param name="event"></param>
        void PublishAsync<TEvent>(TEvent @event) where TEvent : class, IEventData;
        /// <summary>
        /// 显式的异步发布事件,不需要为处理程序加HandlesAsynchronouslyAttribute
        /// event参数为关键字,所以加了@符
        /// </summary>
        /// <typeparam name="TEvent"></typeparam>
        /// <param name="event"></param>
        /// <param name="callback"></param>
        /// <param name="timeout"></param>
        void PublishAsync<TEvent>(TEvent @event, Action<TEvent, bool, Exception> callback, TimeSpan? timeout = null) where TEvent : class, IEventData;
        /// <summary>
        ///  订阅事件列表
        /// </summary>
        /// <typeparam name="TEvent"></typeparam>
        /// <param name="eventHandler"></param>
        void Subscribe<TEvent>(IEventHandler<TEvent> eventHandler) where TEvent : class, IEventData;
        /// <summary>
        /// 订阅事件实体
        /// 装饰模式
        /// </summary>
        /// <typeparam name="TEvent"></typeparam>
        /// <param name="eventHandlerFunc"></param>
        void Subscribe<TEvent>(Action<TEvent> eventHandlerFunc) where TEvent : class, IEventData;
        /// <summary>
        /// 订阅事件集合
        /// </summary>
        /// <typeparam name="TEvent"></typeparam>
        /// <param name="eventHandlers"></param>
        void Subscribe<TEvent>(System.Collections.Generic.IEnumerable<IEventHandler<TEvent>> eventHandlers) where TEvent : class, IEventData;
        /// <summary>
        /// 取消订阅
        /// </summary>
        /// <typeparam name="TEvent"></typeparam>
        /// <param name="eventHandler"></param>
        void Unsubscribe<TEvent>(IEventHandler<TEvent> eventHandler) where TEvent : class, IEventData;
        /// <summary>
        /// 取消订阅
        /// </summary>
        /// <typeparam name="TEvent"></typeparam>
        /// <param name="eventHandlerFunc"></param>
        void Unsubscribe<TEvent>(Action<TEvent> eventHandlerFunc) where TEvent : class, IEventData;
        /// <summary>
        /// 取消订阅
        /// </summary>
        /// <typeparam name="TEvent"></typeparam>
        /// <param name="eventHandlers"></param>
        void Unsubscribe<TEvent>(System.Collections.Generic.IEnumerable<IEventHandler<TEvent>> eventHandlers) where TEvent : class, IEventData;
        /// <summary>
        /// 取消订阅全部事件
        /// </summary>
        void UnsubscribeAll();
        /// <summary>
        /// 取消订阅全部事件
        /// </summary>
        /// <typeparam name="TEvent"></typeparam>
        void UnsubscribeAll<TEvent>() where TEvent : class, IEventData;
        /// <summary>
        /// 订阅全部事件，实现了IEventHandler的类型
        /// </summary>
        void SubscribeAll();
    }
}
