using Lind.DDD.OnlinePay.Weixin.Results;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
 
namespace Lind.DDD.OnlinePay.Weixin
{
    internal class NotifyClient : ClientBase
    {
        public void Notified(Func<NotifyResult, NotifyReturnMessage> onNotified)
        {
            var request = HttpContext.Current.Request;
            var respose = HttpContext.Current.Response;
            respose.ContentType = "text/plain";
            Dictionary<string, string> dictToReturn = new Dictionary<string, string>();
            dictToReturn.Add("return_code", "SUCCESS");
            dictToReturn.Add("return_msg", "");
            NotifyResult result = null;
            string xml;

            xml = GetResultXml(request);
            ResultBuilder resultBuilder = new ResultBuilder(xml);
            resultBuilder.BasicValidate();
            result = new NotifyResult(resultBuilder);

            NotifyReturnMessage returnMessage = onNotified(result);

            if (!returnMessage.Success)
            {
                dictToReturn["return_code"] = "FAIL";
                dictToReturn["return_msg"] = returnMessage.Message;
            }
            respose.Write(CommonHelper.CreateXmlForReturn(dictToReturn));
        }
    }
    public class NotifyReturnMessage
    {
        public bool Success { get; set; }
        public string Message { get; set; }
    }
}

