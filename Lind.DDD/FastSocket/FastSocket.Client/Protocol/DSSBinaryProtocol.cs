using Lind.DDD.FastSocket.SocketBase;
using System;
using System.Text;
using Lind.DDD.FastSocket.SocketBase.Utils;
using Lind.DDD.FastSocket.Client.Response;
namespace Lind.DDD.FastSocket.Client.Protocol
{
    /// <summary>
    /// 异步二进制协议
    /// 从服务器返回信息时，用来解析成DSSBinaryProtocol对象（自定义协议）
    /// 协议格式
    /// [Message Length(int32)][SeqID(int32)][ProjectID(int16)][ExtProperty(int16)][Cmd Length(int16)][VersionNumber Length(int16)][Cmd + VersonNumber + Body Buffer]
    /// 其中参数TableName和VersonNumber长度为40，不够自动在左侧补空格
    /// </summary>
    public sealed class DSSBinaryProtocol : IProtocol<DSSBinaryResponse>
    {

        #region IProtocol Members
        /// <summary>
        /// find response
        /// </summary>
        /// <param name="connection"></param>
        /// <param name="buffer"></param>
        /// <param name="readlength"></param>
        /// <returns></returns>
        /// <exception cref="BadProtocolException">bad async binary protocl</exception>
        public DSSBinaryResponse FindResponse(IConnection connection, ArraySegment<byte> buffer, out int readlength)
        {
            if (buffer.Count < 4) { readlength = 0; return null; }

            //获取message length
            var messageLength = NetworkBitConverter.ToInt32(buffer.Array, buffer.Offset);
            if (messageLength < 7) throw new BadProtocolException("bad async binary protocl");

            readlength = messageLength + 4;
            if (buffer.Count < readlength) { readlength = 0; return null; }

            var seqID = NetworkBitConverter.ToInt32(buffer.Array, buffer.Offset + 4);
            var projectID = NetworkBitConverter.ToInt16(buffer.Array, buffer.Offset + 8);
            var extProperty = NetworkBitConverter.ToInt16(buffer.Array, buffer.Offset + 10);
            var flagLength = NetworkBitConverter.ToInt16(buffer.Array, buffer.Offset + 12);
            var versonLength = NetworkBitConverter.ToInt16(buffer.Array, buffer.Offset + 14);
            var projectNameLength = NetworkBitConverter.ToInt16(buffer.Array, buffer.Offset + 16);
            var strName = Encoding.UTF8.GetString(buffer.Array, buffer.Offset + 18, flagLength);
            var versionNumber = Encoding.UTF8.GetString(buffer.Array, buffer.Offset + 18 + flagLength, versonLength);
            var projectName = Encoding.UTF8.GetString(buffer.Array, buffer.Offset + 18 + flagLength + versonLength, projectNameLength);
            var dataLength = messageLength - 16 - flagLength - versonLength - projectNameLength;
            byte[] data = null;
            if (dataLength > 0)
            {
                data = new byte[dataLength];
                Buffer.BlockCopy(buffer.Array, buffer.Offset + 18 + flagLength + versonLength + projectNameLength, data, 0, dataLength);
            }
            return new DSSBinaryResponse(seqID, projectID, projectName, extProperty, strName, versionNumber, data);
        }
        #endregion
    }
}