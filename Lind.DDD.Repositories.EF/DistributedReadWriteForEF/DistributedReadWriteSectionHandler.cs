using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace Lind.DDD.Repositories.EF.DistributedReadWriteForEF
{
    /// <summary>
    /// Section处理程序
    /// </summary>
    internal class DistributedReadWriteSectionHandler : IConfigurationSectionHandler
    {
        #region IConfigurationSectionHandler 成员

        public object Create(object parent, object configContext, System.Xml.XmlNode section)
        {
            Dictionary<string, DistributedReadWriteSection> names = new Dictionary<string, DistributedReadWriteSection>();

            string _key = string.Empty;
            string _ip = string.Empty;
            string _dbName = string.Empty;
            string _userId = string.Empty;
            string _password = string.Empty;
            int _port = 1433;

            foreach (XmlNode childNode in section.ChildNodes)
            {
                if (childNode.Attributes["key"] != null)
                {
                    _key = childNode.Attributes["key"].Value;

                    if (childNode.Attributes["Ip"] != null)
                    {
                        _ip = childNode.Attributes["Ip"].Value;
                    }
                    if (childNode.Attributes["Port"] != null)
                    {
                        _port = Convert.ToInt32(childNode.Attributes["Port"].Value);
                    }
                    if (childNode.Attributes["DbName"] != null)
                    {
                        _dbName = childNode.Attributes["DbName"].Value;
                    }
                    if (childNode.Attributes["UserId"] != null)
                    {
                        _userId = childNode.Attributes["UserId"].Value;
                    }
                    if (childNode.Attributes["Password"] != null)
                    {
                        _password = childNode.Attributes["Password"].Value;
                    }
                    names.Add(_key, new DistributedReadWriteSection { Ip = _ip, Port = _port, DbName = _dbName, UserId = _userId, Password = _password });
                }
            }
            return names;
        }

        #endregion
    }
}
