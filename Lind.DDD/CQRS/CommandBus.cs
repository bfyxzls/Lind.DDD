using Lind.DDD.LindPlugins;
using Lind.DDD.RedisUtils;
using Lind.DDD.Utils;
using RedisUtils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lind.DDD.CQRS
{

    /// <summary>
    /// 命令总线
    /// </summary>
    public class CommandBus
    {
        /// <summary>
        /// 发送到ES
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="command"></param>
        public static void Send<T>(T command) where T : Command
        {
            //var handlers = Lind.DDD.Utils.AssemblyHelper.GetTypesByInterfaces(typeof(ICommandHandler<T>));
            //foreach (var item in handlers)
            //{
            //    var handler = Lind.DDD.LindPlugins.PluginManager.Resolve<ICommandHandler<T>>(item.FullName);
            //    handler.Execute(command);
            //}

            lock (_objLock)
            {
                var _eventHandlers = RedisManager.Instance.GetDatabase().SetMembers(GetCurrentRedisKey(typeof(T)));
                if (command == null)
                    throw new ArgumentNullException("command event");
                var eventType = command.GetType();
                if (_eventHandlers.Count() > 0)
                {

                    List<Task> tasks = new List<Task>();

                    foreach (var handler in _eventHandlers)
                    {

                        var eventHandler = Utils.SerializeMemoryHelper.DeserializeFromBinary(handler) as ICommandHandler<T>;//显示处理程序

                        if (eventHandler != null)//非正常处理程序
                        {
                            eventHandler.Execute(command);
                        }
                    }
                }
            }

        }
        static object _objLock = new object();


        static string GetCurrentRedisKey(Type tEvent)
        {
            return "Lind_CQRS_" + tEvent.FullName;
        }

        public static void RegisterAll()
        {
            var types = AssemblyHelper.GetTypesByInterfaces(typeof(ICommandHandler<>)).Where(i => i.IsPublic);
            foreach (var item in types)
            {
                foreach (var handler in item.GetInterfaces())
                {
                    var eventHandler = PluginManager.Resolve(item.FullName, handler);
                    lock (_objLock)
                    {
                        foreach (var methodParam in eventHandler.GetType().GetMethods().Where(i => i.Name == "Execute"))
                        {
                            var newVal = Utils.SerializeMemoryHelper.SerializeToBinary(eventHandler);
                            RedisManager.Instance.GetDatabase().SetAdd(GetCurrentRedisKey(methodParam.GetParameters().First().ParameterType), newVal);
                        }

                    }
                }

            }
        }



    }
}
