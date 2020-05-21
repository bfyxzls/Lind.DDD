using System;
using System.Collections.Generic;
using System.Text;
using System.Configuration;
using System.Xml;
namespace Lind.DDD.FastDFS
{
    public class TrackerSection : ConfigurationSection
    {
        [ConfigurationProperty("trackers", IsDefaultCollection = false)]
        public trackers Trackers { get { return (trackers)base["trackers"]; } }
    }

    public class trackers : ConfigurationElementCollection
    {
        protected override ConfigurationElement CreateNewElement()
        {
            return new tracker();
        }
        protected override object GetElementKey(ConfigurationElement element)
        {
            return ((tracker)element).Host;
        }
    }

    public class tracker : ConfigurationElement
    {
        #region 配置節設置，設定檔中有不能識別的元素、屬性時，使其不報錯

        protected override bool OnDeserializeUnrecognizedAttribute(string name, string value)
        {
            return base.OnDeserializeUnrecognizedAttribute(name, value);

        }

        protected override bool OnDeserializeUnrecognizedElement(string elementName, System.Xml.XmlReader reader)
        {
            return base.OnDeserializeUnrecognizedElement(elementName, reader);

        }
        #endregion

        [ConfigurationProperty("Host", DefaultValue = "localhost", IsRequired = true)]
        public string Host { get { return this["Host"].ToString(); } }

        [ConfigurationProperty("Port", DefaultValue = "22122", IsRequired = true)]
        public int Port { get { return (int)this["Port"]; } }

        [ConfigurationProperty("Weight", DefaultValue = "1", IsRequired = false)]
        public int Weight { get { return (int)this["Weight"]; } }

    }
}
