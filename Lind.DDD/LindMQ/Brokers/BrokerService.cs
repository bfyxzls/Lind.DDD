using Lind.DDD.FastSocket.Server;
using Lind.DDD.FastSocket.Server.Command;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lind.DDD.LindMQ
{
    /// <summary>
    /// 队列端公开的服务
    /// 对外提供IP和端口,公开所有AsyncBinaryCommandInfo类型的方法
    /// 提供了两个RPC命令：BrokerPushCommand和BrokerPullCommand
    /// </summary>
    public class BrokerService : CommandSocketService<AsyncBinaryCommandInfo>
    {
        public override void OnConnected(FastSocket.SocketBase.IConnection connection)
        {
            base.OnConnected(connection);
        }
        protected override void HandleUnKnowCommand(FastSocket.SocketBase.IConnection connection, AsyncBinaryCommandInfo commandInfo)
        {
            Console.WriteLine("命令没有被识别");
        }
    }
}
