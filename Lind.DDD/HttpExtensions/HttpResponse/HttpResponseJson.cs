using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web.Script.Serialization;

namespace Lind.DDD.HttpExtensions.HttpResponse
{
    public class HttpResponseJson
    {
        /// <summary>
        /// API返回JSON对象
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static HttpResponseMessage ResponseJsonMessage(object obj)
        {
            string str;
            if (obj is string || obj is char)
            {
                str = obj.ToString();
            }
            else
            {
                JavaScriptSerializer serialize = new JavaScriptSerializer();
                str = serialize.Serialize(obj);
            }
            HttpResponseMessage result = new HttpResponseMessage()
            {
                Content = new StringContent(str, Encoding.GetEncoding("UTF-8"), "application/json")
            };
            return result;
        }
    }
}
