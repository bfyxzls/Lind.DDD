using Lind.DDD.FastSocket.Server.Command;
using Lind.DDD.LindMQ;
using Lind.DDD.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lind.DDD.LindMQ
{
    /// <summary>
    /// 队列端的推命令
    /// </summary>
    public class BrokerPushCommand : ICommand<AsyncBinaryCommandInfo>
    {
        #region ICommand<AsyncBinaryCommandInfo> 成员

        public void ExecuteCommand(FastSocket.SocketBase.IConnection connection, AsyncBinaryCommandInfo commandInfo)
        {
            if (commandInfo.Buffer == null || commandInfo.Buffer.Length == 0)
            {
                Console.WriteLine("BrokenPush参数为空");
                connection.BeginDisconnect();
                return;
            }

            var message = SerializeMemoryHelper.DeserializeFromBinary(commandInfo.Buffer) as MessageBody;
            try
            {
                BrokerManager.Push(message);

                string result = string.Format("消息成功加入队列,Topic:{0},QueueId:{1},QueueCount:{2}",
                    message.Topic,
                    message.QueueId,
                    message.QueueOffset);
                Console.WriteLine(result);
                commandInfo.Reply(connection, SerializeMemoryHelper.SerializeToBinary("OK"));//返回到客户端..

            }
            catch (Exception)
            {

                throw;
            }

        }

        #endregion

        #region ICommand 成员

        public string Name
        {
            get { return "LindQueue_Push"; }
        }

        #endregion
    }

}
