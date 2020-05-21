using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Lind.DDD.Utils;
namespace Lind.DDD.DTO
{
    /// <summary>
    /// 返回的相应对象
    /// Result: 分页返回
    /// Result: 集合返回
    /// Result: 实体返回
    /// </summary>
    public class ResponseMessage
    {
        string _serializableFields;
        /// <summary>
        /// 初始化ResponseMessage
        /// </summary>
        public ResponseMessage()
            : this(string.Empty)
        { }
        /// <summary>
        /// 直接初始化ResponseMessage
        /// </summary>
        /// <param name="serializableFields">希望返回的字段</param>
        public ResponseMessage(string serializableFields)
        {
            this._serializableFields = serializableFields;
            this.Status = 1;
            this.GuidKey = Guid.NewGuid().ToString();
        }

        /// <summary>
        /// 根据ResponseBase初始化ResponseMessage
        /// </summary>
        /// <param name="responseBase"></param>
        public ResponseMessage(ResponseBase responseBase)
            : this(responseBase.SerializableFields)
        { }
        /// <summary>
        /// 标示码，与RequestBase里的GuidKey对应
        /// </summary>
        public virtual string GuidKey { get; set; }
        /// <summary>
        /// 状态码
        /// 1，0失败
        /// </summary>
        public int Status { get; set; }
        /// <summary>
        /// 业务错误代码
        /// </summary>
        public string ErrorCode { get; set; }
        /// <summary>
        /// 错误消息
        /// </summary>
        public string ErrorMessage { get; set; }
        /// <summary>
        /// 只写属性，返回的对象，它不被序列化，只在服务端内存临时存储
        /// 它通常是一个ReponseBase对象或者集合
        /// </summary>
        [JsonIgnore]
        public Object Body
        {
            set
            {
                body = value;
                //自定义的Json序列化，支持按需序列化
                if (string.IsNullOrWhiteSpace(_serializableFields))
                    Result = body.ToJson();
                else
                    Result = body.ToJson(_serializableFields.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries).ToList());
            }
        }
        private object body;
        /// <summary>
        /// 返回的实体
        /// 可以根据Body，返回的JSON对象；也可以自定义返回的内容
        /// </summary>
        public string Result
        {
            get;
            set;
        }

    }
}
