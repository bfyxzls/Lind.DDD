using Lind.DDD.FastSocket.SocketBase;
using System;

namespace Lind.DDD.FastSocket.Server.Command
{
    /// <summary>
    /// async binary command info.
    /// </summary>
    [Serializable]
    public class DSSBinaryCommandInfo : ICommandInfo
    {
        #region Constructors
        /// <summary>
        /// new
        /// </summary>
        /// <param name="cmdName"></param>
        /// <param name="seqID"></param>
        /// <param name="buffer"></param>
        /// <exception cref="ArgumentNullException">cmdName is null or empty.</exception>
        public DSSBinaryCommandInfo(int seqID,
            short projectID,
            string projectName,
            short extProperty,
            string cmdName,
            string versionNumber,
            byte[] buffer)
        {
            if (string.IsNullOrEmpty(cmdName)) throw new ArgumentNullException("cmdName");
            if (string.IsNullOrEmpty(versionNumber)) throw new ArgumentNullException("versonNumber");

            this.VersionNumber = versionNumber;
            this.CmdName = cmdName;
            this.SeqID = seqID;
            this.ProjectID = projectID;
            this.ProjectName = projectName;
            this.ExtProperty = extProperty;
            this.Buffer = buffer;
        }
        #endregion

        #region Public Properties
        /// <summary>
        /// 版本号
        /// </summary>
        public string VersionNumber
        {
            get;
            private set;
        }
        /// <summary>
        /// 项目号
        /// </summary>
        public short ProjectID
        {
            get;
            private set;
        }
        public string ProjectName
        {
            get;
            private set;
        }
        /// <summary>
        /// 扩展属性
        /// </summary>
        public short ExtProperty
        {
            get;
            private set;
        }
        /// <summary>
        /// get the current command name.
        /// </summary>
        public string CmdName
        {
            get;
            private set;
        }
        /// <summary>
        /// seq id.
        /// </summary>
        public int SeqID
        {
            get;
            private set;
        }
        /// <summary>
        /// 主体内容,存储的对象
        /// </summary>
        public byte[] Buffer
        {
            get;
            private set;
        }
        #endregion

        #region Public Methods
        /// <summary>
        /// 回调客户端
        /// </summary>
        /// <param name="connection"></param>
        /// <param name="payload"></param>
        public void Reply(IConnection connection, byte[] payload)
        {
            var packet = PacketBuilder.ToDSSBinary(this.SeqID, this.ProjectID, this.ProjectName, this.ExtProperty, this.CmdName, this.VersionNumber, payload);
            connection.BeginSend(packet);
        }
        #endregion

    }
}