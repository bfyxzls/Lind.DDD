
namespace Lind.DDD.FastSocket.Client.Response
{
    /// <summary>
    /// 数据同步系统DSS使用的Socket协议，我们称为DSSBinary协议
    /// [Message Length(int32)][SeqID(int32)][ProjectID(int16)][ExtProperty(int16)][Cmd Length(int16)][VersonNumber Length(int16)][Cmd + VersonNumber + Body Buffer]
    /// </summary>
    public class DSSBinaryResponse : IResponse
    {
        /// <summary>
        /// 流水ID
        /// </summary>
        public int SeqID { get; private set; }
        /// <summary>
        /// 项目类型编号
        /// </summary>
        public short ProjectID { get; set; }
        /// <summary>
        /// 项目名称
        /// </summary>
        public string ProjectName { get; set; }
        /// <summary>
        /// 扩展属性，具体项目可以自已定义
        /// </summary>
        public short ExtProperty { get; set; }
        /// <summary>
        /// 本次传输的版本号，所有客户端唯一[项目名称(4字节)+guid(36字节)]
        /// </summary>
        public string VersionNumber { get; private set; }
        /// <summary>
        /// 命令名称
        /// </summary>
        public string Flag { get; private set; }

        /// <summary>
        /// 要操作的表对象，以字节数组形式进行传输
        /// </summary>
        public readonly byte[] Buffer = null;

        public DSSBinaryResponse(int seqID,
            short projectID,
            short extProperty,
            string flag,
            string versonNumber,
            byte[] buffer)
            : this(seqID, projectID, null, 0, flag, versonNumber, buffer)
        {

        }
        public DSSBinaryResponse(int seqID,
           short projectID,
           string projectName,
           short extProperty,
           string flag,
           string versonNumber,
           byte[] buffer)
        {
            this.SeqID = seqID;
            this.ExtProperty = extProperty;
            this.ProjectID = projectID;
            this.ProjectName = projectName;
            this.VersionNumber = versonNumber;
            this.Flag = flag;
            this.Buffer = buffer;
        }

        public DSSBinaryResponse(int seqID,
           short projectID,
           string flag,
           string versonNumber,
           byte[] buffer)
            : this(seqID, projectID, 0, flag, versonNumber, buffer)
        { }
    }
}