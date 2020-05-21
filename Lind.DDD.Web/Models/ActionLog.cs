using Lind.DDD.Domain;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lind.DDD.Web.Models
{
    /// <summary>
    /// 日志表，不持久化到SQL
    /// </summary>
    [NotMapped]
    public class ActionLog : NoSqlEntity
    {
        public string Message { get; set; }
        public ActionLogType Type { get; set; }
        public string Operator { get; set; }
    }

    public enum ActionLogType
    {
        /// <summary>
        /// 成功
        /// </summary>
        Success,
        /// <summary>
        /// 失败
        /// </summary>
        Fail,
        /// <summary>
        /// 工作流
        /// </summary>
        Flow,
    }
}
