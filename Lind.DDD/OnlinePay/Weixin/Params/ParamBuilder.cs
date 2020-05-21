using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Xml;

namespace Lind.DDD.OnlinePay.Weixin.Params
{
    public class ParamBuilder
    {
        protected Dictionary<string, string> ParamDict;
        protected Dictionary<string, int> IntParamDict;

        public ParamBuilder()
        {
            ParamDict = new Dictionary<string, string>();
            IntParamDict = new Dictionary<string, int>();
        }

        public void SetParam(string name, string value)
        {
            if (string.IsNullOrEmpty(name))
                return;
            if (value == null)
                return;
            if (!ParamDict.ContainsKey(name))
                ParamDict.Add(name.Trim(), value.Trim());
            else
                ParamDict[name] = value;
        }

        public void SetParam(string name, int value)
        {
            if (string.IsNullOrEmpty(name))
                return;
            if (!IntParamDict.ContainsKey(name))
                IntParamDict.Add(name.Trim(), value);
            else
                IntParamDict[name] = value;
        }

        public string GetParam(string name)
        {
            if (ParamDict.ContainsKey(name))
                return ParamDict[name];
            if (IntParamDict.ContainsKey(name))
                return IntParamDict[name].ToString();
            return null;
        }

        /// <summary>
        /// 生成Xml
        /// </summary>
        /// <returns></returns>
        public string ToXml()
        {
            //随机串
            string randomString=Guid.NewGuid().ToString();
            if(randomString.Length>32)
                randomString=randomString.Substring(0,32);
            SetParam("nonce_str", randomString);
            //签名
            SetParam("sign", CommonHelper.CreateSign(ParamDict, IntParamDict));

            StringBuilder sb = new StringBuilder();
            sb.Append("<xml>");
            foreach(var item in ParamDict)
            {
                string strValue = string.Empty;
                if (!string.IsNullOrWhiteSpace(item.Value))
                    strValue = string.Format("<![CDATA[{0}]]>", item.Value);
                sb.Append(string.Format("<{0}>{1}</{0}>", item.Key, strValue));
            }
            foreach (var item in IntParamDict)
            {
                sb.Append(string.Format("<{0}>{1}</{0}>", item.Key, item.Value));
            }
            sb.Append("</xml>");
            return sb.ToString();
        }

        public void InitByXml(string xml)
        {
            ParamDict = new Dictionary<string, string>();
            
            XmlDocument doc = new XmlDocument(); 
            doc.LoadXml(xml);
            foreach( XmlNode aNode in doc.ChildNodes)
            {
                SetParam(aNode.Name, aNode.InnerText);
            }
        }
    }
}
