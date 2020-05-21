using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.Xml.Linq;
using System.Threading.Tasks;

namespace Lind.DDD.Events
{
    /// <summary>
    /// 事件总线[事件功能核心代码]
    /// 发布与订阅处理逻辑
    /// 核心功能代码
    /// </summary>
    public class EventBus : IEventBus
    {
        #region Constructors
        
        protected EventBus()
        {
            if (_eventBusType.ToLower() == "memory")
                _iEventBus = new MemoryEventBus();
            else
                _iEventBus = new RedisEventBus();
        }

        #endregion

        private static object _objLock = new object();
        /// <summary>
        /// 事件总线对象
        /// </summary>
        private static EventBus _instance = null;

        /// <summary>
        /// 事件总线存储方式
        /// </summary>
        private static string _eventBusType = ConfigConstants.ConfigManager.Config.DomainEvent.Type;
        /// <summary>
        /// 事件生产者
        /// </summary>
        private IEventBus _iEventBus = null;

        #region Singleton Instance
        /// <summary>
        /// 初始化空的事件总件,单例模式,双重锁，解决并发和性能问题
        /// </summary>
        public static IEventBus Instance
        {
            get
            {
                if (_instance == null)
                {
                    lock (_objLock)
                    {
                        if (_instance == null)
                        {
                            _instance = new EventBus();
                        }
                    }
                }
                return _instance;
            }
        }
        #endregion

        #region 事件订阅 & 取消订阅，可以扩展
        /// <summary>
        /// 订阅事件列表
        /// </summary>
        /// <param name="type"></param>
        /// <param name="subTypeList"></param>
        public void SubscribeAll()
        {
            _iEventBus.SubscribeAll();
        }
        public void Subscribe<TEvent>(IEventHandler<TEvent> eventHandler)
            where TEvent : class, IEventData
        {
            _iEventBus.Subscribe<TEvent>(eventHandler);
        }
        /// <summary>
        /// 订阅事件实体
        /// 装饰模式
        /// </summary>
        /// <param name="type"></param>
        /// <param name="subTypeList"></param>
        public void Subscribe<TEvent>(Action<TEvent> eventHandlerFunc)
            where TEvent : class, IEventData
        {
            _iEventBus.Subscribe<TEvent>(eventHandlerFunc);
        }
        public void Subscribe<TEvent>(IEnumerable<IEventHandler<TEvent>> eventHandlers)
            where TEvent : class, IEventData
        {
            _iEventBus.Subscribe<TEvent>(eventHandlers);
        }
        /// <summary>
        /// 取消订阅事件
        /// </summary>
        /// <param name="type"></param>
        /// <param name="subType"></param>
        public void Unsubscribe<TEvent>(IEventHandler<TEvent> eventHandler)
            where TEvent : class, IEventData
        {
            _iEventBus.Unsubscribe<TEvent>(eventHandler);
        }
        public void Unsubscribe<TEvent>(IEnumerable<IEventHandler<TEvent>> eventHandlers)
          where TEvent : class, IEventData
        {
            _iEventBus.Unsubscribe<TEvent>(eventHandlers);
        }
        public void Unsubscribe<TEvent>(Action<TEvent> eventHandlerFunc)
            where TEvent : class, IEventData
        {
            _iEventBus.Unsubscribe<TEvent>(eventHandlerFunc);

        }
        /// <summary>
        /// 取消指定事件的所有订阅
        /// </summary>
        /// <typeparam name="TEvent"></typeparam>
        /// <param name="eventHandlerFunc"></param>
        public void UnsubscribeAll<TEvent>()
        where TEvent : class, IEventData
        {
            _iEventBus.UnsubscribeAll<TEvent>();
        }
        /// <summary>
        /// 取消所有事件的所有订阅
        /// </summary>
        public void UnsubscribeAll()
        {
            _iEventBus.UnsubscribeAll();
        }
        #endregion

        #region 事件发布
        /// <summary>
        /// 发布事件，支持异步事件
        /// </summary>
        /// <typeparam name="TEvent"></typeparam>
        /// <param name="evnt"></param>
        public void Publish<TEvent>(TEvent @event)
           where TEvent : class, IEventData
        {
            _iEventBus.Publish<TEvent>(@event);
        }
        /// <summary>
        /// 发布事件
        /// event参数为关键字,所以加了@符
        /// </summary>
        /// <typeparam name="TEvent"></typeparam>
        /// <param name="event"></param>
        /// <param name="callback"></param>
        /// <param name="timeout"></param>
        public void Publish<TEvent>(TEvent @event, Action<TEvent, bool, Exception> callback, TimeSpan? timeout = null)
           where TEvent : class, IEventData
        {
            _iEventBus.Publish<TEvent>(@event, callback, timeout);
        }
        /// <summary>
        /// 显式的异步发布事件,不需要为处理程序加HandlesAsynchronouslyAttribute
        /// </summary>
        /// <typeparam name="TEvent"></typeparam>
        /// <param name="event">具体事件执行者</param>
        public void PublishAsync<TEvent>(TEvent @event)
           where TEvent : class, IEventData
        {
            _iEventBus.PublishAsync<TEvent>(@event);

        }
        /// <summary>
        /// 显式的异步发布事件,不需要为处理程序加HandlesAsynchronouslyAttribute
        /// </summary>
        /// <typeparam name="TEvent"></typeparam>
        /// <param name="event">具体事件执行者</param>
        /// <param name="callback">回调</param>
        /// <param name="timeout">超时</param>
        public void PublishAsync<TEvent>(TEvent @event, Action<TEvent, bool, Exception> callback, TimeSpan? timeout = null)
          where TEvent : class, IEventData
        {
            _iEventBus.PublishAsync<TEvent>(@event, callback, timeout);
        }
        #endregion
    }
}
