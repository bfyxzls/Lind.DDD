using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
using Lind.DDD.Utils;
namespace Lind.DDD.DTO
{
    /// <summary>
    /// 响应体基类
    /// </summary>
    public abstract class ResponseBase
    {
        /// <summary>
        /// 相应的字段
        /// </summary>
        public string SerializableFields { get; set; }
        /// <summary>
        /// 初始化ResponseMessage
        /// </summary>
        public ResponseBase()
            : this(null)
        { }
        /// <summary>
        /// 初始化ResponseMessage
        /// </summary>
        /// <param name="serializableFields">希望返回的字段</param>
        public ResponseBase(string serializableFields)
        {
            this.SerializableFields = serializableFields;
        }

    }
}
