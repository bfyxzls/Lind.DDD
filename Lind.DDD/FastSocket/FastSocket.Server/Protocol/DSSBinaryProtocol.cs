using System;
using System.Text;
using Lind.DDD.FastSocket.SocketBase;
using Lind.DDD.FastSocket.Server.Command;
using Lind.DDD.FastSocket.SocketBase.Utils;


namespace Lind.DDD.FastSocket.Server.Protocol
{
    /// <summary>
    /// 数据中心二进制协议
    /// 协议的主体是命令，即你的协议去服务你的命令
    /// [Message Length(int32)][SeqID(int32)][ProjectID(int16)][ExtProperty(int16)][Cmd Length(int16)][VersonNumber Length(int16)][Cmd + VersonNumber + Body Buffer]
    /// </summary>
    public sealed class DSSBinaryProtocol : IProtocol<DSSBinaryCommandInfo>
    {
        #region IProtocol Members
        /// <summary>
        /// 将客户端转来的byte[]转换成指定对象（自定义协议）
        /// </summary>
        /// <param name="connection"></param>
        /// <param name="buffer"></param>
        /// <param name="maxMessageSize"></param>
        /// <param name="readlength"></param>
        /// <returns></returns>
        /// <exception cref="BadProtocolException">bad async binary protocl</exception>
        public DSSBinaryCommandInfo FindCommandInfo(IConnection connection, ArraySegment<byte> buffer,
            int maxMessageSize, out int readlength)
        {
            if (buffer.Count < 4) { readlength = 0; return null; }

            var payload = buffer.Array;

            //获取message length
            var messageLength = NetworkBitConverter.ToInt32(payload, buffer.Offset);
            if (messageLength < 7) throw new BadProtocolException("bad async binary protocl");
            if (messageLength > maxMessageSize) throw new BadProtocolException("message is too long");

            readlength = messageLength + 4;
            if (buffer.Count < readlength)
            {
                readlength = 0; return null;
            }

            var seqID = NetworkBitConverter.ToInt32(payload, buffer.Offset + 4);
            var projectID = NetworkBitConverter.ToInt16(payload, buffer.Offset + 8);
            var extProperty = NetworkBitConverter.ToInt16(payload, buffer.Offset + 10);
            var cmdNameLength = NetworkBitConverter.ToInt16(payload, buffer.Offset + 12);
            var versionNumberLength = NetworkBitConverter.ToInt16(payload, buffer.Offset + 14);
            var projectNameLength = NetworkBitConverter.ToInt16(payload, buffer.Offset + 16);
            var strName = Encoding.UTF8.GetString(payload, buffer.Offset + 18, cmdNameLength);
            var versionNumber = Encoding.UTF8.GetString(payload, buffer.Offset + 18 + cmdNameLength, versionNumberLength);
            var projectName = Encoding.UTF8.GetString(payload, buffer.Offset + 18 + cmdNameLength + versionNumberLength, projectNameLength);
            var dataLength = messageLength - 12 - cmdNameLength - versionNumberLength - projectNameLength;
            byte[] data = null;
            if (dataLength > 0)
            {
                data = new byte[dataLength];
                Buffer.BlockCopy(payload, buffer.Offset + 18 + cmdNameLength + versionNumberLength + projectNameLength, data, 0, dataLength);
            }
            return new DSSBinaryCommandInfo(seqID, projectID, projectName, extProperty, strName, versionNumber, data);
        }
        #endregion
    }
}