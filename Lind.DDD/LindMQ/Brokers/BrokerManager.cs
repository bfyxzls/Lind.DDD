using Lind.DDD.FastSocket.Server;
using Lind.DDD.FastSocket.Server.Command;
using Lind.DDD.Utils;
using RedisUtils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lind.DDD.LindMQ
{
    /// <summary>
    /// 队列端消息管理者
    /// key/value:topic/queueId
    /// key/value:topic_queueId/MessageBody
    /// key/value:topic_consumerId/offset
    /// LindQueue中的broker负责消息的中转，即接收producer发送过来的消息，然后持久化消息到磁盘，
    /// 然后接收consumer发送过来的拉取消息的请求，然后根据请求拉取相应的消息给consumer。所以，broker可以理解为消息队列服务器，提供消息的接收、存储、拉取服务。可见，broker对于equeue来说是核心，它绝对不能挂，一旦挂了，那producer，consumer就无法实现publish-subscribe了。
    /// </summary>
    public class BrokerManager
    {
        #region consts
        /// <summary>
        ///负载均衡的取模数,N表示N+1个queue管道 
        /// </summary>
        static int CONFIG_QUEUECOUNT = ConfigConstants.ConfigManager.Config.LindMQ.Config_QueueCount;
        /// <summary>
        /// LindMQ统一键前缀
        /// </summary>
        static string LINDMQKEY = ConfigConstants.ConfigManager.Config.LindMQ.LindMqKey;
        /// <summary>
        /// LindMQ所有Topic需要存储到这个键里
        /// </summary>
        static string LINDMQ_TOPICKEY = ConfigConstants.ConfigManager.Config.LindMQ.LindMq_TopicKey;
        /// <summary>
        /// 每个消费者的消费进度
        /// </summary>
        static string QUEUEOFFSETKEY = ConfigConstants.ConfigManager.Config.LindMQ.QueueOffsetKey;
        /// <summary>
        /// 消息自动回收的周期（天）
        /// </summary>
        static int AutoEmptyForDay = ConfigConstants.ConfigManager.Config.LindMQ.AutoEmptyForDay;
        #endregion

        #region Public Methods
        /// <summary>
        /// 在队列中的消息
        /// </summary>
        /// <returns></returns>
        public static IEnumerable<MessageBody> GetMessageBody(string topic)
        {
            foreach (var item in RedisManager.Instance.GetDatabase().SetMembers(GetRedisKey(topic)))
            {
                foreach (var sub in RedisManager.Instance.GetDatabase().ListRange(item.ToString()))
                {
                    yield return Utils.SerializeMemoryHelper.DeserializeFromJson<MessageBody>(sub);
                }
            }

        }

        /// <summary>
        /// 得到有的Topic
        /// </summary>
        /// <returns></returns>
        public static List<Topic_Queue_Offset> GetAllTopic()
        {
            var result = new List<Topic_Queue_Offset>();

            //所有topic
            var ret = RedisManager
                                     .Instance
                                     .GetDatabase()
                                     .SetMembers(LINDMQ_TOPICKEY).ToList();

            foreach (var item in ret)
            {
                //每个topic下的queue列表
                var queueList = RedisManager
                                     .Instance
                                     .GetDatabase()
                                     .SetMembers(LINDMQKEY + item).ToList();

                var queueMessages = new List<Queue_Offset>();
                foreach (var sub in queueList)
                {

                    queueMessages.Add(new Queue_Offset
                    {
                        QueueName = sub
                    });
                }
                result.Add(new Topic_Queue_Offset
                {
                    TopicName = item,
                    Queue_Offset = queueMessages
                });
            }
            return result;
        }

        /// <summary>
        /// 拿到消息进度
        /// </summary>
        /// <returns></returns>
        public static IDictionary<string, int> GetQueueOffset()
        {
            var ret = RedisManager
                                 .Instance
                                 .GetDatabase()
                                 .HashGetAll(QUEUEOFFSETKEY)
                                 .ToDictionary(i => (string)i.Name, i => (int)i.Value);

            return ret;
        }

        /// <summary>
        /// 开始拉消息的服务,从FastSocket的配置文件里拿数据
        /// 服务端配置信息在Broken的宿主app.config
        /// </summary>
        public static void Start()
        {
            SocketServerManager.Init();
            SocketServerManager.Start();
            Console.ReadLine();
        }

        /// <summary>
        /// 自动清除过期的消息，清楚昨天的任务
        /// </summary>
        /// <returns></returns>
        public static void AutoRemoveQueue()
        {
            var topicList = RedisManager.Instance.GetDatabase().SetMembers(LINDMQ_TOPICKEY);
            foreach (var topic in topicList)
            {
                var queueList = RedisManager.Instance.GetDatabase().SetMembers(LINDMQKEY + topic);
                foreach (var queue in queueList)
                {
                    var removeKey = LINDMQKEY + queue + "_" + DateTime.Now.AddDays(-AutoEmptyForDay).ToString("yyyyMMdd");
                    RedisManager.Instance.GetDatabase().KeyDelete(removeKey);
                }
            }
        }

        #endregion

        #region Internal & Private Methods
        /// <summary>
        /// LindMQ在redis存储的键前缀
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        static string GetRedisKey(string key)
        {
            return LINDMQKEY + key;
        }
        /// <summary>
        /// LindMQ队列存储的Key
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        static string GetRedisDataKey(string key)
        {
            return GetRedisKey(key) + "_" + DateTime.Now.ToString("yyyyMMdd");
        }
        /// <summary>
        /// 返回IP地址的长整型格式
        /// </summary>
        /// <param name="connection"></param>
        /// <returns></returns>
        static long GetIpLong(FastSocket.SocketBase.IConnection connection)
        {
            return System.BitConverter.ToInt64(connection.RemoteEndPoint.Address.GetAddressBytes(), 0);
        }
        /// <summary>
        /// 持久化消息
        /// </summary>
        /// <param name="body">消息体</param>
        internal static void Push(MessageBody body)
        {
            //存储当前Topic
            RedisManager.Instance.GetDatabase().SetAdd(LINDMQ_TOPICKEY, body.Topic);

            //要存储到哪个队列
            body.QueueId = Math.Abs(body.Body.GetHashCode() % BrokerManager.CONFIG_QUEUECOUNT);
            var dataKey = body.Topic + body.QueueId;
            RedisManager.Instance.GetDatabase().SetAdd(GetRedisKey(body.Topic), dataKey);

            //记录偏移
            var offset = RedisManager.Instance.GetDatabase().SortedSetLength(GetRedisDataKey(dataKey));
            body.QueueOffset = offset + 1;

            //存储消息
            RedisManager.Instance.GetDatabase().SortedSetAdd(
                GetRedisDataKey(dataKey),
                Utils.SerializeMemoryHelper.SerializeToJson(body),
                score: body.QueueOffset);
        }

        /// <summary>
        /// 拉出消息
        /// </summary>
        /// <param name="connection">当前连接</param>
        /// <param name="topic">主题</param>
        /// <param name="topicQueueId">队列</param>
        /// <param name="offset">偏移量</param>
        /// <returns></returns>
        internal static MessageBody Pull(FastSocket.SocketBase.IConnection connection, string topic, string topicQueueId, int offset = 0)
        {
            if (topicQueueId == null)
                return null;

            //通过消费进度，拿指定进度的消息
            var entity = RedisManager.Instance.GetDatabase().SortedSetRangeByScore(GetRedisDataKey(topicQueueId), start: offset).FirstOrDefault();
            if (!entity.HasValue)
            {
                //当前客户端已经消费完成
                return null;
            }
            //消费日志
            RedisManager.Instance.GetDatabase().SetAdd(GetRedisKey(topicQueueId + "_" + GetIpLong(connection)), offset);
            return Utils.SerializeMemoryHelper.DeserializeFromJson<MessageBody>(entity);
        }

        /// <summary>
        /// 拉出来消息后，处理消息
        /// </summary>
        /// <param name="connection">当前连接</param>
        /// <param name="topic">主题</param>
        /// <param name="topicQueueId">指定队列</param>
        /// <param name="aciton">处理程序</param>
        /// <param name="offset">偏移量</param>
        internal static void Pull(FastSocket.SocketBase.IConnection connection, string topic, string topicQueueId, Action<MessageBody> aciton, int offset = 0)
        {
            var entity = Pull(connection, topic, topicQueueId, offset);
            if (entity != null)
                aciton(entity);
        }

        /// <summary>
        /// 返回当前消费者的消息偏移量
        /// </summary>
        /// <param name="connection">消费者连接</param>
        /// <param name="topic">主题</param>
        /// <param name="topicQueueId">当前队列</param>
        /// <param name="after">回调</param>
        /// <returns></returns>
        internal static int GetConsumerQueueOffset(
            FastSocket.SocketBase.IConnection connection,
            string topic,
            ref string topicQueueId,
            ref Action after)
        {
            int offset = 0;
            var queueList = RedisManager.Instance.GetDatabase().SetMembers(GetRedisKey(topic));
            topicQueueId = queueList.OrderByNewId().FirstOrDefault().ToString();
            if (topicQueueId == null)
                return 0;

            //消费者标识

            string connectionId = GetRedisKey(topicQueueId + "_" + GetIpLong(connection));
            if (!RedisManager.Instance.GetDatabase().HashExists(QUEUEOFFSETKEY, connectionId))
            {
                RedisManager.Instance.GetDatabase().HashSet(QUEUEOFFSETKEY, connectionId, 1);
            }

             //业务层的回调,算出当前队列的偏移量
            after = () =>
            {
                //更新消费端的消费量
                RedisManager.Instance.GetDatabase().HashIncrement(QUEUEOFFSETKEY, connectionId, 1);
            };

            return offset;
        }
        #endregion

    }




}
