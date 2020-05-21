
namespace Lind.DDD.FastSocket.Server.Protocol
{
    /// <summary>
    /// ProtocolNames
    /// </summary>
    static public class ProtocolNames
    {
        /// <summary>
        /// 异步binary协议
        /// </summary>
        public const string AsyncBinary = "asyncBinary";
        /// <summary>
        /// thrift协议
        /// </summary>
        public const string Thrift = "thrift";
        /// <summary>
        /// 命令行协议
        /// </summary>
        public const string CommandLine = "commandLine";
        /// <summary>
        /// 数据同步中心协议，支持版本号
        /// </summary>
        public const string DSSBinary = "dssBinary";
    }
}