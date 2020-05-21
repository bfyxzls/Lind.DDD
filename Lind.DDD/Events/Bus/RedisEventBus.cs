using Lind.DDD.LindPlugins;
using Lind.DDD.RedisUtils;
using Lind.DDD.Utils;
using RedisUtils;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lind.DDD.Events
{
    internal class RedisEventBus : IEventBus
    {
        /// <summary>
        /// 模式锁
        /// </summary>
        private static object _objLock = new object();
        /// <summary>
        /// 对于事件数据的存储，目前采用内存字典
        /// </summary>
        private readonly IDatabase _RedisUtils = RedisManager.Instance.GetDatabase();
        /// <summary>
        /// redis事件总线的Key
        /// </summary>
        private string redisKey = ConfigConstants.ConfigManager.Config.DomainEvent.RedisKey;

        /// <summary>
        /// 得到当前redis-eventbus-key
        /// </summary>
        /// <typeparam name="TEvent"></typeparam>
        /// <returns></returns>
        private string GetCurrentRedisKey<TEvent>()
        {
            return redisKey + "_" + typeof(TEvent).FullName;
        }
        /// <summary>
        ///得到非泛型版本的值
        /// </summary>
        /// <param name="tEvent"></param>
        /// <returns></returns>
        private string GetCurrentRedisKey(Type tEvent)
        {
            return redisKey + "_" + tEvent.FullName;
        }

        #region IEventBus 成员

        #region 事件订阅 & 取消订阅，可以扩展
        /// <summary>
        /// 订阅事件列表
        /// </summary>
        /// <typeparam name="TEvent"></typeparam>
        /// <param name="eventHandler"></param>
        public void Subscribe<TEvent>(IEventHandler<TEvent> eventHandler)
            where TEvent : class, IEventData
        {
            lock (_objLock)
            {
                var newVal = Utils.SerializeMemoryHelper.SerializeToBinary(eventHandler);
                _RedisUtils.SetAdd(GetCurrentRedisKey<TEvent>(), newVal);
            }


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
            Subscribe<TEvent>(new ActionDelegatedEventHandler<TEvent>(eventHandlerFunc));
        }
        public void Subscribe<TEvent>(IEnumerable<IEventHandler<TEvent>> eventHandlers)
            where TEvent : class, IEventData
        {
            foreach (var eventHandler in eventHandlers)
                Subscribe<TEvent>(eventHandler);
        }
        /// <summary>
        /// 取消订阅事件
        /// </summary>
        /// <param name="type"></param>
        /// <param name="subType"></param>
        public void Unsubscribe<TEvent>(IEventHandler<TEvent> eventHandler)
            where TEvent : class, IEventData
        {

            lock (_objLock)
            {


                _RedisUtils.SetRemove(GetCurrentRedisKey<TEvent>(), Utils.SerializeMemoryHelper.SerializeToBinary((eventHandler)));
            }

        }
        public void Unsubscribe<TEvent>(IEnumerable<IEventHandler<TEvent>> eventHandlers)
          where TEvent : class, IEventData
        {
            foreach (var eventHandler in eventHandlers)
                Unsubscribe<TEvent>(eventHandler);
        }
        public void Unsubscribe<TEvent>(Action<TEvent> eventHandlerFunc)
            where TEvent : class, IEventData
        {
            Unsubscribe<TEvent>(new ActionDelegatedEventHandler<TEvent>(eventHandlerFunc));
        }
        /// <summary>
        /// 取消指定事件的所有订阅
        /// </summary>
        /// <typeparam name="TEvent"></typeparam>
        /// <param name="eventHandlerFunc"></param>
        public void UnsubscribeAll<TEvent>()
        where TEvent : class, IEventData
        {
            _RedisUtils.KeyDelete(GetCurrentRedisKey<TEvent>());

        }
        /// <summary>
        /// 取消所有事件的所有订阅
        /// </summary>
        public void UnsubscribeAll()
        {
            throw new NotImplementedException("本方法不被实现");
        }
        #endregion

        #region 事件发布
        /// <summary>
        /// 发布事件，支持异步事件
        /// </summary>
        /// <typeparam name="TEvent"></typeparam>
        /// <param name="evnt"></para m>
        public void Publish<TEvent>(TEvent @event)
           where TEvent : class, IEventData
        {

            lock (_objLock)
            {
                var _eventHandlers = _RedisUtils.SetMembers(GetCurrentRedisKey<TEvent>());
                if (@event == null)
                    throw new ArgumentNullException("event");

                if (_eventHandlers.Count() > 0)
                {
                    foreach (var handler in _eventHandlers)
                    {
                        var eventHandler = Utils.SerializeMemoryHelper.DeserializeFromBinary(handler) as IEventHandler<TEvent>;//显示处理程序
                        if (eventHandler == null)//匿名处理程序
                            eventHandler = Utils.SerializeMemoryHelper.DeserializeFromBinary(handler) as ActionDelegatedEventHandler<TEvent>;
                        if (eventHandler != null)//非正常处理程序
                        {
                            if (eventHandler.GetType().IsDefined(typeof(HandlesAsynchronouslyAttribute), false))
                            {
                                Task.Factory.StartNew((o) => eventHandler.Handle((TEvent)o), @event);
                            }
                            else
                            {
                                eventHandler.Handle(@event);
                            }
                        }
                    }

                }
            }

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

            lock (_objLock)
            {
                var _eventHandlers = _RedisUtils.SetMembers(GetCurrentRedisKey<TEvent>());
                if (@event == null)
                    throw new ArgumentNullException("event");
                var eventType = @event.GetType();
                if (_eventHandlers.Count() > 0)
                {

                    List<Task> tasks = new List<Task>();

                    foreach (var handler in _eventHandlers)
                    {

                        var eventHandler = Utils.SerializeMemoryHelper.DeserializeFromBinary(handler) as IEventHandler<TEvent>;//显示处理程序
                        if (eventHandler == null)//匿名处理程序
                            eventHandler = Utils.SerializeMemoryHelper.DeserializeFromBinary(handler) as ActionDelegatedEventHandler<TEvent>;
                        if (eventHandler != null)//非正常处理程序
                        {


                            if (eventHandler.GetType().IsDefined(typeof(HandlesAsynchronouslyAttribute), false))
                            {
                                tasks.Add(Task.Factory.StartNew((o) => eventHandler.Handle((TEvent)o), @event));
                            }
                            else
                            {
                                eventHandler.Handle(@event);
                            }
                        }
                        if (tasks.Count > 0)
                        {
                            if (timeout == null)
                                Task.WaitAll(tasks.ToArray());
                            else
                                Task.WaitAll(tasks.ToArray(), timeout.Value);
                        }
                        callback(@event, true, null);

                    }
                }
            }
        }
        /// <summary>
        /// 显式的异步发布事件,不需要为处理程序加HandlesAsynchronouslyAttribute
        /// </summary>
        /// <typeparam name="TEvent"></typeparam>
        /// <param name="event">具体事件执行者</param>
        public void PublishAsync<TEvent>(TEvent @event)
           where TEvent : class, IEventData
        {
            Task.Factory.StartNew(() => Publish(@event));
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
            Task.Factory.StartNew(() => Publish(@event, callback, timeout));
        }
        #endregion

        #region 订阅所有

        /// <summary>
        /// 全局统一注册所有事件处理程序，实现了IEventHandlers的
        /// 目前只支持注册显示实现IEventHandlers的处理程序，不支持匿名处理程序
        /// </summary>
        public void SubscribeAll()
        {
            var types = AssemblyHelper.GetTypesByInterfaces(typeof(IEventHandler<>)).Where(i => i.IsPublic);
            foreach (var item in types)
            {
                foreach (var handler in item.GetInterfaces())
                {
                    var eventHandler = PluginManager.Resolve(item.FullName, handler);
                    lock (_objLock)
                    {
                        foreach (var methodParam in eventHandler.GetType().GetMethods().Where(i => i.Name == "Handle"))
                        {
                            var newVal = Utils.SerializeMemoryHelper.SerializeToBinary(eventHandler);
                            _RedisUtils.SetAdd(GetCurrentRedisKey(methodParam.GetParameters().First().ParameterType), newVal);
                        }

                    }
                }

            }
        }

        #endregion

        #endregion
    }
}
