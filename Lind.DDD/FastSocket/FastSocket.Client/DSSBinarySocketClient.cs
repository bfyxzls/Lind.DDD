using System;
using System.Text;
using System.Threading.Tasks;
using Lind.DDD.FastSocket.Client.Response;
using Lind.DDD.FastSocket.SocketBase.Utils;
using Lind.DDD.FastSocket.Client.Protocol;

namespace Lind.DDD.FastSocket.Client
{
    /// <summary>
    /// 异步socket客户端
    /// </summary>
    public class DSSBinarySocketClient : PooledSocketClient<DSSBinaryResponse>
    {
        #region Constructors
        /// <summary>
        /// new
        /// </summary>
        public DSSBinarySocketClient()
            : base(new DSSBinaryProtocol())
        {
        }
        /// <summary>
        /// new
        /// </summary>
        /// <param name="socketBufferSize"></param>
        /// <param name="messageBufferSize"></param>
        public DSSBinarySocketClient(int socketBufferSize, int messageBufferSize)
            : base(new DSSBinaryProtocol(), socketBufferSize, messageBufferSize, 3000, 3000)
        {
        }
        /// <summary>
        /// new
        /// </summary>
        /// <param name="socketBufferSize">buffer存储数据大叔，默认8192</param>
        /// <param name="messageBufferSize"></param>
        /// <param name="millisecondsSendTimeout"></param>
        /// <param name="millisecondsReceiveTimeout"></param>
        public DSSBinarySocketClient(int socketBufferSize,
            int messageBufferSize,
            int millisecondsSendTimeout,
            int millisecondsReceiveTimeout)
            : base(new DSSBinaryProtocol(),
            socketBufferSize,
            messageBufferSize,
            millisecondsSendTimeout,
            millisecondsReceiveTimeout)
        {
        }
        #endregion

        #region Public Methods
        /// <summary>
        /// 发送数据到服务端
        /// </summary>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="cmdName"></param>
        /// <param name="projectID"></param>
        /// <param name="versionNumber"></param>
        /// <param name="payload"></param>
        /// <param name="funcResultFactory"></param>
        /// <param name="asyncState"></param>
        /// <returns></returns>
        public Task<TResult> Send<TResult>(string cmdName,
            short projectID,
            string versionNumber,
            byte[] payload,
            Func<DSSBinaryResponse, TResult> funcResultFactory, object asyncState = null)
        {
            return this.Send(null, cmdName, projectID, string.Empty, 0, versionNumber, payload, funcResultFactory, asyncState);
        }
        /// <summary>
        /// 发送数据到服务端
        /// 数据包由方法参数组成
        /// </summary>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="cmdName"></param>
        /// <param name="projectID"></param>
        /// <param name="extProperty"></param>
        /// <param name="versionNumber"></param>
        /// <param name="payload"></param>
        /// <param name="funcResultFactory"></param>
        /// <param name="asyncState"></param>
        /// <returns></returns>
        public Task<TResult> Send<TResult>(string cmdName,
            short projectID,
            short extProperty,
            string versionNumber,
            byte[] payload,
          Func<DSSBinaryResponse, TResult> funcResultFactory, object asyncState = null)
        {
            return this.Send(null, cmdName, projectID, string.Empty, extProperty, versionNumber, payload, funcResultFactory, asyncState);
        }
        /// <summary>
        /// 发送数据到服务端
        /// 数据包由方法参数组成
        /// </summary>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="consistentKey"></param>
        /// <param name="cmdName"></param>
        /// <param name="projectID"></param>
        /// <param name="extProperty"></param>
        /// <param name="versionNumber"></param>
        /// <param name="payload"></param>
        /// <param name="funcResultFactory"></param>
        /// <param name="asyncState"></param>
        /// <returns></returns>
        public Task<TResult> Send<TResult>(byte[] consistentKey,
         string cmdName,
         short projectID,
         short extProperty,
         string versionNumber,
         byte[] payload,
         Func<DSSBinaryResponse, TResult> funcResultFactory, object asyncState = null)
        {
            return this.Send(consistentKey, cmdName, projectID, null, extProperty, versionNumber, payload, funcResultFactory, asyncState);
        }
        /// <summary>
        /// 发送数据到服务端
        /// 数据包由方法参数组成
        /// </summary>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="cmdName"></param>
        /// <param name="projectID"></param>
        /// <param name="projectName"></param>
        /// <param name="extProperty"></param>
        /// <param name="versionNumber"></param>
        /// <param name="payload"></param>
        /// <param name="funcResultFactory"></param>
        /// <param name="asyncState"></param>
        /// <returns></returns>
        public Task<TResult> Send<TResult>(
        string cmdName,
        short projectID,
        string projectName,
        short extProperty,
        string versionNumber,
        byte[] payload,
        Func<DSSBinaryResponse, TResult> funcResultFactory, object asyncState = null)
        {
            return this.Send(null, cmdName, projectID, projectName, extProperty, versionNumber, payload, funcResultFactory, asyncState);
        }
        /// <summary>
        /// 发送数据到服务端
        /// 数据包由方法参数组成
        /// </summary>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="consistentKey"></param>
        /// <param name="cmdName"></param>
        /// <param name="projectID"></param>
        /// <param name="extProperty"></param>
        /// <param name="versionNumber"></param>
        /// <param name="payload"></param>
        /// <param name="funcResultFactory"></param>
        /// <param name="asyncState"></param>
        /// <returns></returns>
        public Task<TResult> Send<TResult>(byte[] consistentKey,
            string cmdName,
            short projectID,
            string projectName,
            short extProperty,
            string versionNumber,
            byte[] payload,
            Func<DSSBinaryResponse, TResult> funcResultFactory, object asyncState = null)
        {
            if (string.IsNullOrEmpty(cmdName)) throw new ArgumentNullException("cmdName");
            if (funcResultFactory == null) throw new ArgumentNullException("funcResultFactory");

            var seqID = base.NextRequestSeqID();
            var cmdLength = cmdName.Length;
            var versonNumberLength = versionNumber.Length;
            var projectNameLength = projectName.Length;

            var messageLength = (payload == null ? 0 : payload.Length) + cmdLength + versonNumberLength + projectNameLength + 18;
            var sendBuffer = new byte[messageLength + 4];

            //write message length
            Buffer.BlockCopy(NetworkBitConverter.GetBytes(messageLength), 0, sendBuffer, 0, 4);
            //write seqID.
            Buffer.BlockCopy(NetworkBitConverter.GetBytes(seqID), 0, sendBuffer, 4, 4);
            //write proejctID
            Buffer.BlockCopy(NetworkBitConverter.GetBytes(projectID), 0, sendBuffer, 8, 2);
            //write extProperty
            Buffer.BlockCopy(NetworkBitConverter.GetBytes(extProperty), 0, sendBuffer, 10, 2);
            //write response flag length.
            Buffer.BlockCopy(NetworkBitConverter.GetBytes((short)cmdLength), 0, sendBuffer, 12, 2);
            //write verson length
            Buffer.BlockCopy(NetworkBitConverter.GetBytes((short)versonNumberLength), 0, sendBuffer, 14, 2);
            //write projectName length
            Buffer.BlockCopy(NetworkBitConverter.GetBytes((short)projectNameLength), 0, sendBuffer, 16, 2);
            //write response cmd
            Buffer.BlockCopy(Encoding.ASCII.GetBytes(cmdName), 0, sendBuffer, 18, cmdLength);
            //write response versonNumber
            Buffer.BlockCopy(Encoding.ASCII.GetBytes(versionNumber), 0, sendBuffer, 18 + cmdLength, versonNumberLength);
            //write response projectName
            Buffer.BlockCopy(Encoding.ASCII.GetBytes(projectName), 0, sendBuffer, 18 + cmdLength + versonNumberLength, projectNameLength);
            //write body buffer
            if (payload != null && payload.Length > 0)
                Buffer.BlockCopy(payload, 0, sendBuffer, 18 + cmdLength + versonNumberLength + projectNameLength, payload.Length);

            var source = new TaskCompletionSource<TResult>(asyncState);
            base.Send(new Request<DSSBinaryResponse>(consistentKey, seqID, cmdName, sendBuffer,
                ex => source.TrySetException(ex),
                response =>
                {
                    TResult result;
                    try { result = funcResultFactory(response); }
                    catch (Exception ex) { source.TrySetException(ex); return; }

                    source.TrySetResult(result);
                }));
            return source.Task;
        }
        #endregion
    }
}