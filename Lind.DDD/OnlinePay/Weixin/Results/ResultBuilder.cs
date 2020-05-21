using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace Lind.DDD.OnlinePay.Weixin.Results
{
    public class ResultBuilder
    {
        protected Dictionary<string, string> ParamDict;

        public string OriginResultXml { get; set; }

        public ResultBuilder(string xml)
        {
            OriginResultXml = xml;
            ParamDict = new Dictionary<string, string>();
            XmlDocument doc = new XmlDocument();
            doc.LoadXml(xml);
            XmlNode rootNode = doc.SelectSingleNode("xml");
            if (rootNode != null)
            {
                foreach (XmlNode aNode in rootNode.ChildNodes)
                {
                    SetParam(aNode.Name, aNode.InnerText);
                }
            }
        }

        protected void SetParam(string name, string value)
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

        public string GetParam(string name)
        {
            if (ParamDict.ContainsKey(name))
                return ParamDict[name];
            return null;
        }

        public void BasicValidate()
        {
            ReturnCodeValidate();
            //ResultCodeValidate();
            //SecurityValidate();
        }

        /// <summary>
        /// 返回状态码验证
        /// </summary>
        private void ReturnCodeValidate()
        {
            if (GetParam("return_code") == "FAIL")
                throw new ReturnErrorException("通信出错：" + GetParam("return_msg"));
        }
        /// <summary>
        /// 业务结果验证
        /// </summary>
        private void ResultCodeValidate()
        {
            if (GetParam("result_code") == "FAIL")
                throw new ResultErrorException(GetParam("err_code"), GetParam("err_code_des"));
        }

        public void ValidateNotNullField(string fieldName, string errorMessage)
        {
            string value = GetParam(fieldName);
            if (string.IsNullOrEmpty(fieldName))
                throw new SDKRuntimeException(errorMessage + ":" + fieldName);
        }
        /// <summary>
        /// 验证默认配置的微信账号
        /// </summary>
        public void ValidateWeixinAccount()
        { 
            
        }
        /// <summary>
        /// 验证微信账号
        /// </summary>
        /// <param name="appid"></param>
        /// <param name="mch_id"></param>
        public void ValidateWeixinAccount(string appid,string mch_id)
        {

        }

        /// <summary>
        /// 验证签名
        /// </summary>
        private void SecurityValidate()
        {
            //验证签名
            if (string.IsNullOrEmpty(GetParam("sign")))
                throw new SignErrorException("签名为空!");

            string recreateSign = CommonHelper.CreateSign(ParamDict);
            if (GetParam("sign") != recreateSign)
                throw new SignErrorException("参数中的签名不正确！");

            //验证AppId
            if (PaymentConfig.Instance.AppId != GetParam("appid"))
                throw new ReturnParamException("从微信返回的参数appid不正确！");

            //验证Mch_Id
            if (PaymentConfig.Instance.Mch_Id != GetParam("mch_id"))
                throw new ReturnParamException("从微信返回的参数mch_id不正确！");

        }

    }
}
