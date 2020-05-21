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
    /// 队列端的拉命令
    /// </summary>
    public class BrokerPullCommand : ICommand<AsyncBinaryCommandInfo>
    {

        #region ICommand<AsyncBinaryCommandInfo> 成员

        public void ExecuteCommand(FastSocket.SocketBase.IConnection connection, AsyncBinaryCommandInfo commandInfo)
        {

            if (commandInfo.Buffer == null || commandInfo.Buffer.Length == 0)
            {
                Console.WriteLine("BrokenPull参数为空");
                connection.BeginDisconnect();
                return;
            }

            var topic = SerializeMemoryHelper.DeserializeFromBinary(commandInfo.Buffer).ToString();
            string topicQueueId = null;
            Action after = null;
            var offset = BrokerManager.GetConsumerQueueOffset(connection, topic, ref topicQueueId, ref after);
            var entity = BrokerManager.Pull(connection,topic, topicQueueId, offset);
            if (entity == null)
            {
                Console.WriteLine("服务端消息已被消费完！");
                commandInfo.Reply(connection, null);//返回到客户端..
            }
            else
            {
                if (after != null)
                    after();
                Console.WriteLine("向客户端返回消息对象：{0},IP:{1},Port:{2},connId:{3}", entity.ToString(), connection.RemoteEndPoint.Address.Address, connection.RemoteEndPoint.Port, connection.ConnectionID);
                commandInfo.Reply(connection, SerializeMemoryHelper.SerializeToBinary(entity));//返回到客户端..
            }
        }

        #endregion

        #region ICommand 成员

        public string Name
        {
            get { return "LindQueue_Pull"; }
        }

        #endregion
    }



}
