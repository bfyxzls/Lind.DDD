using System;
using System.Text;

namespace Lind.DDD.FastSocket.Server
{
    /// <summary>
    /// <see cref="SocketBase.Packet"/> builder
    /// </summary>
    static public class PacketBuilder
    {
        #region ToAsyncBinary
        /// <summary>
        /// to async binary <see cref="SocketBase.Packet"/>
        /// </summary>
        /// <param name="responseFlag"></param>
        /// <param name="seqID"></param>
        /// <param name="buffer"></param>
        /// <returns></returns>
        static public SocketBase.Packet ToAsyncBinary(string responseFlag, int seqID, byte[] buffer)
        {
            var messageLength = (buffer == null ? 0 : buffer.Length) + responseFlag.Length + 6;
            var sendBuffer = new byte[messageLength + 4];

            //write message length
            Buffer.BlockCopy(SocketBase.Utils.NetworkBitConverter.GetBytes(messageLength), 0, sendBuffer, 0, 4);
            //write seqID.
            Buffer.BlockCopy(SocketBase.Utils.NetworkBitConverter.GetBytes(seqID), 0, sendBuffer, 4, 4);
            //write response flag length.
            Buffer.BlockCopy(SocketBase.Utils.NetworkBitConverter.GetBytes((short)responseFlag.Length), 0, sendBuffer, 8, 2);
            //write response flag
            Buffer.BlockCopy(Encoding.ASCII.GetBytes(responseFlag), 0, sendBuffer, 10, responseFlag.Length);
            //write body buffer
            if (buffer != null && buffer.Length > 0) Buffer.BlockCopy(buffer, 0, sendBuffer, 10 + responseFlag.Length, buffer.Length);

            return new SocketBase.Packet(sendBuffer);
        }
        #endregion

        #region DSSBinary
        /// <summary>
        ///  封装DSSBinary包
        /// </summary>
        /// <param name="seqID"></param>
        /// <param name="responseFlag"></param>
        /// <param name="versionNumber"></param>
        /// <param name="buffer"></param>
        /// <returns></returns>
        static public SocketBase.Packet ToDSSBinary(int seqID,
            short projectID,
            string projectName,
            short extProperty,
            string responseFlag,
            string versionNumber,
            byte[] buffer)
        {
            var messageLength = (buffer == null ? 0 : buffer.Length) + responseFlag.Length + versionNumber.Length + projectName.Length + 18;
            var sendBuffer = new byte[messageLength + 4];

            //write message length
            Buffer.BlockCopy(SocketBase.Utils.NetworkBitConverter.GetBytes(messageLength), 0, sendBuffer, 0, 4);
            //write seqID.
            Buffer.BlockCopy(SocketBase.Utils.NetworkBitConverter.GetBytes(seqID), 0, sendBuffer, 4, 4);
            //write projectID
            Buffer.BlockCopy(SocketBase.Utils.NetworkBitConverter.GetBytes(projectID), 0, sendBuffer, 8, 2);
            //write extProperty
            Buffer.BlockCopy(SocketBase.Utils.NetworkBitConverter.GetBytes(extProperty), 0, sendBuffer, 10, 2);
            //write response flag length.
            Buffer.BlockCopy(SocketBase.Utils.NetworkBitConverter.GetBytes((short)responseFlag.Length), 0, sendBuffer, 12, 2);
            Buffer.BlockCopy(SocketBase.Utils.NetworkBitConverter.GetBytes((short)versionNumber.Length), 0, sendBuffer, 14, 2);
            Buffer.BlockCopy(SocketBase.Utils.NetworkBitConverter.GetBytes((short)projectName.Length), 0, sendBuffer, 16, 2);

            //write response flag
            Buffer.BlockCopy(Encoding.ASCII.GetBytes(responseFlag), 0, sendBuffer, 18, responseFlag.Length);
            //write versonNumber
            Buffer.BlockCopy(Encoding.ASCII.GetBytes(versionNumber), 0, sendBuffer, 18 + responseFlag.Length, versionNumber.Length);
            //write projectName
            Buffer.BlockCopy(Encoding.ASCII.GetBytes(projectName), 0, sendBuffer, 18 + responseFlag.Length + versionNumber.Length, projectName.Length);

            //write body buffer
            if (buffer != null && buffer.Length > 0) Buffer.BlockCopy(buffer, 0, sendBuffer, 18 + responseFlag.Length + versionNumber.Length + projectName.Length, buffer.Length);

            return new SocketBase.Packet(sendBuffer);
        }

        #endregion

        #region ToCommandLine
        /// <summary>
        /// to command line <see cref="SocketBase.Packet"/>
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        static public SocketBase.Packet ToCommandLine(string value)
        {
            return new SocketBase.Packet(Encoding.UTF8.GetBytes(string.Concat(value, Environment.NewLine)));
        }
        #endregion
    }
}