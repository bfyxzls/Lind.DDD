using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Lind.DDD.ConfigConstants.Models
{
    /// <summary>
    /// 配置信息实体
    /// </summary>
    public class ConfigModel
    {
        public ConfigModel()
        {
            Caching = new Caching();
            Queue = new Queue();
            Logger = new Logger();
            Pub_Sub = new Pub_Sub();
            MongoDB = new MongoDB();
            Redis = new Redis();
            Messaging = new Messaging();
            DomainEvent = new DomainEvent();
            Socket = new Socket();
            Cat = new Cat();
            SSO = new SSO();
            Versions = new List<Version>();
            IocContaion = new IocContainer();
            LindMQ = new LindMQ();
        }

        /// <summary>
        /// 启用属性变化跟踪
        /// </summary>
        [System.Xml.Serialization.XmlElementAttribute(Order = 0)]
        public int PropertyChanged { get; set; }

        /// <summary>
        /// 缓存相关配置
        /// </summary>
        [System.Xml.Serialization.XmlElementAttribute(Order = 1)]
        public Caching Caching { get; set; }

        /// <summary>
        /// 队列相关配置
        /// </summary>
        [System.Xml.Serialization.XmlElementAttribute(Order = 2)]
        public Queue Queue { get; set; }

        /// <summary>
        /// 日志相关
        /// </summary>
        [System.Xml.Serialization.XmlElementAttribute(Order = 3)]
        public Logger Logger { get; set; }

        /// <summary>
        /// Pub_Sub相关
        /// </summary>
        [System.Xml.Serialization.XmlElementAttribute(Order = 4)]
        public Pub_Sub Pub_Sub { get; set; }

        /// <summary>
        /// MongoDB相关
        /// </summary>
        [System.Xml.Serialization.XmlElementAttribute(Order = 5)]
        public MongoDB MongoDB { get; set; }

        /// <summary>
        /// redis相关
        /// </summary>
        [System.Xml.Serialization.XmlElementAttribute(Order = 6)]
        public Redis Redis { get; set; }

        /// <summary>
        /// Messaging消息相关
        /// </summary>
        [System.Xml.Serialization.XmlElementAttribute(Order = 7)]
        public Messaging Messaging { get; set; }

        /// <summary>
        /// 领域事件相关
        /// </summary>
        [System.Xml.Serialization.XmlElementAttribute(Order = 8)]
        public DomainEvent DomainEvent { get; set; }

        /// <summary>
        /// Socket通讯配置 
        /// </summary>
        [System.Xml.Serialization.XmlElementAttribute(Order = 10)]
        public Socket Socket { get; set; }

        /// <summary>
        /// Cat实时监控配置 
        /// </summary>
        [System.Xml.Serialization.XmlElementAttribute(Order = 11)]
        public Cat Cat { get; set; }

        /// <summary>
        /// Cat实时监控配置 
        /// XmlArray表示以数组的形式
        /// </summary>
        [XmlArray(Order = 12)]
        public List<Version> Versions { get; set; }

        /// <summary>
        /// Ioc容器配置
        /// </summary>
        [System.Xml.Serialization.XmlElementAttribute(Order = 13)]
        public IocContainer IocContaion { get; set; }

        /// <summary>
        /// SSO单点登陆相关
        /// </summary>
        [System.Xml.Serialization.XmlElementAttribute(Order = 14)]
        public SSO SSO { get; set; }
        /// <summary>
        /// 自动加载的DLL的黑名单，使用逗号分开，黑名单的程序集不会被装载
        /// </summary>
        [System.Xml.Serialization.XmlElementAttribute(Order = 15)]
        public string AutoLoadDLL_BlackList { get; set; }
        /// <summary>
        /// LindMQ配置
        /// </summary>
        [System.Xml.Serialization.XmlElementAttribute(Order = 16)]
        public LindMQ LindMQ { get; set; }
    }

    /// <summary>
    /// 缓存Caching(Redis,RunTime)
    /// </summary>
    public class Caching
    {
        #region 缓存Caching(Redis,RunTime)
        /// <summary>
        /// 缓存提供者:RuntimeCache,RedisCache
        /// </summary>
        [DisplayName("缓存提供者:RuntimeCache,RedisCache")]
        [System.Xml.Serialization.XmlElementAttribute(Order = 1)]
        public string Provider { get; set; }
        /// <summary>
        /// 缓存过期时间(minutes)
        /// </summary>
        [DisplayName("缓存过期时间(minutes)")]
        [System.Xml.Serialization.XmlElementAttribute(Order = 2)]
        public int ExpireMinutes { get; set; }
        #endregion
    }
    /// <summary>
    /// Socket数据传递的配置
    /// </summary>
    public class Socket
    {
        #region Socket数据传递的配置
        /// <summary>
        /// Socket通讯地址
        /// </summary>
        [DisplayName("Socket通讯地址")]
        [System.Xml.Serialization.XmlElementAttribute(Order = 0)]
        public string ServerHost { get; set; }
        /// <summary>
        /// Socket数据传输的端口
        /// </summary>
        [DisplayName("Socket数据传输的端口")]
        [System.Xml.Serialization.XmlElementAttribute(Order = 1)]
        public int DataPort { get; set; }
        /// <summary>
        /// Socket远程命令调用（RPC）的端口
        /// </summary>
        [DisplayName("Socket远程命令调用（RPC）的端口")]
        [System.Xml.Serialization.XmlElementAttribute(Order = 2)]
        public int CommandPort { get; set; }

        #endregion
    }
    /// <summary>
    /// 领域事件相关
    /// </summary>
    public class DomainEvent
    {
        #region 领域事件存储的介绍
        /// <summary>
        /// 领域事件存储的介绍:Memory,Redis
        /// </summary>
        [DisplayName("领域事件存储的介绍:Memory,Redis")]
        [System.Xml.Serialization.XmlElementAttribute(Order = 0)]
        public string Type { get; set; }
        /// <summary>
        /// 存储在redis里的领域事件键
        /// </summary>
        [DisplayName("存储在redis里的领域事件键")]
        [System.Xml.Serialization.XmlElementAttribute(Order = 1)]
        public string RedisKey { get; set; }
        #endregion
    }
    /// <summary>
    /// 队列Queue(Memory,File,Redis)
    /// </summary>
    public class Queue
    {
        #region 队列Queue(Memory,File,Redis)
        /// <summary>
        /// 队列类型：Memory,File,Redis
        /// </summary>
        [DisplayName("队列类型：Memory,File,Redis")]
        [System.Xml.Serialization.XmlElementAttribute(Order = 0)]
        public string Type { get; set; }
        /// <summary>
        /// 文件队列的相对目录名
        /// </summary>
        [DisplayName("文件队列的相对目录名")]
        [System.Xml.Serialization.XmlElementAttribute(Order = 1)]
        public string FilePath { get; set; }
        #endregion
    }
    /// <summary>
    /// 分布式Pub/Sub
    /// </summary>
    public class Pub_Sub
    {
        #region 分布式Pub/Sub
        /// <summary>
        /// pub端重发的时间间隔
        /// </summary>
        [DisplayName("pub端重发的时间间隔")]
        [System.Xml.Serialization.XmlElementAttribute(Order = 0)]
        public int Interval { get; set; }
        /// <summary>
        /// pub端的重发次数
        /// </summary>
        [DisplayName("pub端的重发次数")]
        [System.Xml.Serialization.XmlElementAttribute(Order = 1)]
        public int RepeatNum { get; set; }
        #endregion
    }
    /// <summary>
    /// 日志相关
    /// 日志Logger(File,Log4net,MongoDB)
    /// </summary>
    public class Logger
    {
        #region 日志Logger(File,Log4net,MongoDB)
        /// <summary>
        /// 日志实现方式：File,Log4net,MongoDB
        /// </summary>
        [DisplayName("日志实现方式：File,Log4net,MongoDB")]
        [System.Xml.Serialization.XmlElementAttribute(Order = 0)]
        public string Type { get; set; }
        /// <summary>
        /// 日志级别：DEBUG|INFO|WARN|ERROR|FATAL|OFF
        /// </summary>
        [DisplayName("日志级别：DEBUG|INFO|WARN|ERROR|FATAL|OFF")]
        [System.Xml.Serialization.XmlElementAttribute(Order = 1)]
        public string Level { get; set; }
        /// <summary>
        /// 日志记录的项目名称
        /// </summary>
        [DisplayName("日志记录的项目名称")]
        [System.Xml.Serialization.XmlElementAttribute(Order = 2)]
        public string ProjectName { get; set; }
        #endregion
    }
    /// <summary>
    /// 消息机制相关配置
    /// </summary>
    public class Messaging
    {
        #region 消息Messaging(Email,SMS,RTX)
        /// <summary>
        /// 消息机制－Email账号
        /// </summary>
        [DisplayName("消息机制－Email账号")]
        [System.Xml.Serialization.XmlElementAttribute(Order = 1)]
        public string Email_UserName { get; set; }
        /// <summary>
        /// 消息机制－Email登陆密码
        /// </summary>
        [DisplayName("消息机制－Email登陆密码")]
        [System.Xml.Serialization.XmlElementAttribute(Order = 2)]
        public string Email_Password { get; set; }
        /// <summary>
        /// 消息机制－Email主机头
        /// </summary>
        [DisplayName("消息机制－Email主机头")]
        [System.Xml.Serialization.XmlElementAttribute(Order = 3)]
        public string Email_Host { get; set; }
        /// <summary>
        /// 消息机制－Email端口
        /// </summary>
        [DisplayName("消息机制－Email端口")]
        [System.Xml.Serialization.XmlElementAttribute(Order = 4)]
        public int Email_Port { get; set; }
        /// <summary>
        /// 消息机制-Email地址
        /// </summary>
        [DisplayName("消息机制-Email地址")]
        [System.Xml.Serialization.XmlElementAttribute(Order = 5)]
        public string Email_Address { get; set; }
        /// <summary>
        /// 消息机制-Email显示的名称
        /// </summary>
        [DisplayName("消息机制-Email显示的名称")]
        [System.Xml.Serialization.XmlElementAttribute(Order = 6)]
        public string Email_DisplayName { get; set; }
        /// <summary>
        /// 消息机制－Rtx-发送消息的Api
        /// </summary>
        [DisplayName("消息机制－Rtx-发送消息的Api")]
        [System.Xml.Serialization.XmlElementAttribute(Order = 7)]
        public string RtxApi { get; set; }
        /// <summary>
        /// 消息机制－SMS－网关
        /// </summary>
        [DisplayName("消息机制－SMS－网关")]
        [System.Xml.Serialization.XmlElementAttribute(Order = 28)]
        public string SMSGateway { get; set; }
        /// <summary>
        /// 消息机制－SMS－加密方式
        /// </summary>
        [DisplayName("消息机制－SMS－加密方式")]
        [System.Xml.Serialization.XmlElementAttribute(Order = 29)]
        public string SMSSignType { get; set; }
        /// <summary>
        /// 消息机制－SMS－字符编码
        /// </summary>
        [DisplayName("消息机制－SMS－字符编码")]
        [System.Xml.Serialization.XmlElementAttribute(Order = 30)]
        public string SMSCharset { get; set; }
        /// <summary>
        /// 消息机制－SMS－短信密钥
        /// </summary>
        [DisplayName("消息机制－SMS－短信密钥")]
        [System.Xml.Serialization.XmlElementAttribute(Order = 31)]
        public string SMSKey { get; set; }
        /// <summary>
        /// 消息机制－SMS－项目ID
        /// </summary>
        [DisplayName("消息机制－SMS－项目ID")]
        [System.Xml.Serialization.XmlElementAttribute(Order = 32)]
        public int SMSItemID { get; set; }

        #endregion
    }
    /// <summary>
    /// Redis相关配置
    /// </summary>
    public class Redis
    {
        #region Redis
        /// <summary>
        /// redis缓存的连接串
        /// var conn = ConnectionMultiplexer.Connect("contoso5.redis.cache.windows.net,password=...");
        /// </summary>
        [DisplayName("StackExchange.redis缓存的连接串")]
        [System.Xml.Serialization.XmlElementAttribute(Order = 0)]
        public string Host { get; set; }
        [DisplayName("StackExchange.redis代理模式（可选0:无，1：TW")]
        [System.Xml.Serialization.XmlElementAttribute(Order = 1)]
        public int Proxy { get; set; }
        #endregion
    }
    /// <summary>
    /// MongoDB相关配置
    /// </summary>
    public class MongoDB
    {
        #region MongoDB
        /// <summary>
        /// Mongo连接串，支持多路由localhost:27017,localhost:27018,localhost:27018
        /// </summary>
        [DisplayName("Mongo连接串，支持多路由localhost:27017,localhost:27018,localhost:27018")]
        [System.Xml.Serialization.XmlElementAttribute(Order = 0)]
        public string Host { get; set; }
        /// <summary>
        /// Mongo-数据库名称
        /// </summary>
        [DisplayName("Mongo-数据库名称")]
        [System.Xml.Serialization.XmlElementAttribute(Order = 1)]
        public string DbName { get; set; }
        /// <summary>
        /// Mongo-登陆名
        /// </summary>
        [DisplayName("Mongo-登陆名")]
        [System.Xml.Serialization.XmlElementAttribute(Order = 2)]
        public string UserName { get; set; }
        /// <summary>
        /// Mongo-密码
        /// </summary>
        [DisplayName("Mongo-密码")]
        [System.Xml.Serialization.XmlElementAttribute(Order = 3)]
        public string Password { get; set; }
        #endregion
    }

    /// <summary>
    /// Cat实时监控配置
    /// </summary>
    public class Cat
    {
        public Cat()
        {
            CatDomain = new CatDomain();
            CatServers = new List<CatServer>();
        }

        [System.Xml.Serialization.XmlElementAttribute(Order = 0)]
        public CatDomain CatDomain { get; set; }
        [XmlArray(Order = 1)]
        public List<CatServer> CatServers { get; set; }
    }
    /// <summary>
    /// Cat服务器
    /// </summary>
    public class CatServer
    {
        public CatServer(string ip, int port = 2280, int webport = 8080)
        {
            Ip = ip;
            Port = port;
            WebPort = webport;
            Enabled = true;
        }
        public CatServer()
            : this("localhost", 2280, 8080)
        {

        }

        /// <summary>
        ///   Cat服务器IP
        /// </summary>
        [System.Xml.Serialization.XmlElementAttribute(Order = 0)]
        public string Ip { get; set; }
        /// <summary>
        ///   Cat服务器端口
        /// </summary>
        [System.Xml.Serialization.XmlElementAttribute(Order = 1)]
        public int Port { get; set; }
        /// <summary>
        /// WEB后台端口
        /// </summary>
        [System.Xml.Serialization.XmlElementAttribute(Order = 2)]
        public int WebPort { get; set; }
        /// <summary>
        ///   Cat服务器是否有效，默认有效
        /// </summary>
        [System.Xml.Serialization.XmlElementAttribute(Order = 3)]
        public bool Enabled { get; set; }
    }

    /// <summary>
    ///  Cat域名
    /// </summary>
    public class CatDomain
    {
        private string _id;
        private bool _mEnabled;

        public CatDomain(string id = null, bool enabled = true)
        {
            _id = string.IsNullOrWhiteSpace(id) ? "Unknown" : id;
            _mEnabled = enabled;
        }
        public CatDomain()
            : this("test", true)
        {

        }
        /// <summary>
        ///   当前系统的标识
        /// </summary>

        [XmlElementAttribute(Order = 0)]
        public string Id
        {
            get { return _id; }
            set { _id = value; }
        }

        /// <summary>
        ///   Cat日志是否开启，默认关闭
        /// </summary>
        [XmlElementAttribute(Order = 1)]
        public bool Enabled
        {
            get { return _mEnabled; }
            set { _mEnabled = value; }
        }
    }

    /// <summary>
    /// 版本配置
    /// </summary>
    public class Version
    {
        [XmlElementAttribute(Order = 0)]
        public string Code { get; set; }
        [XmlElementAttribute(Order = 1)]
        public string Info { get; set; }
    }

    /// <summary>
    /// IOC容器相关
    /// </summary>
    public class IocContainer
    {
        /// <summary>
        /// 容器类型，0:unity,1:autofac
        /// 需要在config中配置对象的容器声明
        /// </summary>
        [System.Xml.Serialization.XmlElementAttribute(Order = 0)]
        public int IoCType { get; set; }
        /// 数据集缓存策略：EntLib,Redis
        /// </summary>
        [System.Xml.Serialization.XmlElementAttribute(Order = 1)]
        public string AoP_CacheStrategy { get; set; }
    }

    /// <summary>
    /// SSO单点登陆
    /// </summary>
    public class SSO
    {

        /// <summary>
        /// sso的域名
        /// </summary>
        [System.Xml.Serialization.XmlElementAttribute(Order = 0)]
        public string Domain { get; set; }
        /// <summary>
        /// sso数据集存储的名称(redis/key,cache/key)
        /// </summary>
        [System.Xml.Serialization.XmlElementAttribute(Order = 3)]
        public string SSOKey { get; set; }
        /// token key
        /// </summary>
        [System.Xml.Serialization.XmlElementAttribute(Order = 1)]
        public string TokenKey { get; set; }
        /// <summary>
        /// sso提供者：Cache,Redis
        /// </summary>
        [System.Xml.Serialization.XmlElementAttribute(Order = 2)]
        public string Provider { get; set; }
    }

    /// <summary>
    /// LindMQ消息队列配置
    /// </summary>
    public class LindMQ
    {
        /// <summary>
        ///负载均衡的取模数,N表示N+1个queue管道 
        /// </summary>
        [System.Xml.Serialization.XmlElementAttribute(Order = 0)]
        public int Config_QueueCount { get; set; }
        /// <summary>
        /// LindMQ统一键前缀
        /// </summary>
        [System.Xml.Serialization.XmlElementAttribute(Order = 1)]
        public string LindMqKey { get; set; }
        /// <summary>
        /// LindMQ所有Topic需要存储到这个键里
        /// </summary>
        [System.Xml.Serialization.XmlElementAttribute(Order = 2)]
        public string LindMq_TopicKey { get; set; }
        /// <summary>
        /// 每个消费者的消费进度
        /// </summary>
        [System.Xml.Serialization.XmlElementAttribute(Order = 3)]
        public string QueueOffsetKey { get; set; }
        /// <summary>
        /// 消息自动回收的周期（天）
        /// </summary>
        [System.Xml.Serialization.XmlElementAttribute(Order = 4)]
        public int AutoEmptyForDay { get; set; }
    }

}
