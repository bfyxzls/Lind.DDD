using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;
using System.Xml;

namespace Lind.DDD.IRepositories.DistributedReadWriteForEF
{
    /// <summary>
    /// DistributedReadWriteForEFSection块，在web.config中提供DistributedReadWriteForEFSection块定义
    /// </summary>
    internal class DistributedReadWriteSection : ConfigurationSection
    {

        /// <summary>
        /// 主机地址
        /// </summary>
        [ConfigurationProperty("Ip", DefaultValue = "127.0.0.1")]
        public string Ip
        {
            get { return (string)this["Ip"]; }
            set { this["Ip"] = value; }
        }
        /// <summary>
        /// 端口号
        /// </summary>
        [ConfigurationProperty("Port", DefaultValue = "1433")]
        public int Port
        {
            get { return (int)this["Port"]; }
            set { this["Port"] = value; }
        }

        /// <summary>
        /// 数据库名称
        /// </summary>
        [ConfigurationProperty("DbName", DefaultValue = "Test")]
        public string DbName
        {
            get { return (string)this["DbName"]; }
            set { this["DbName"] = value; }
        }

        /// <summary>
        /// 数据库账号
        /// </summary>
        [ConfigurationProperty("UserId", DefaultValue = "sa")]
        public string UserId
        {
            get { return (string)this["UserId"]; }
            set { this["UserId"] = value; }
        }

        /// <summary>
        /// 数据库账号
        /// </summary>
        [ConfigurationProperty("Password", DefaultValue = "sa")]
        public string Password
        {
            get { return (string)this["Password"]; }
            set { this["Password"] = value; }
        }
    }
}
