using Lind.DDD.Commons;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace Lind.DDD.Utils
{
    /// <summary>
    /// XML辅助对象
    /// </summary>
    public class XmlHelper
    {
        /// <summary>
        /// 通过结构化的XSD文件校验对应的XML文件
        /// </summary>
        /// <param name="xsd"></param>
        /// <param name="xml"></param>
        /// <returns></returns>
        public static ReturnMessage ValidateXml(string xsd, string xml)
        {
            var result = new ReturnMessage();
            XmlReaderSettings st = new XmlReaderSettings();
            st.ValidationType = ValidationType.Schema;
            st.Schemas.Add(null, xsd);

            st.ValidationEventHandler += (obj, e) =>
            {
                result.AddItem(e.Message);
            };

            XmlReader xr = XmlReader.Create(xml, st);
            while (xr.Read())
            {
                if (xr.IsStartElement())
                {
                    xr.Read();
                }
            }
            xr.Close();
            return result;
        }

    }
}
