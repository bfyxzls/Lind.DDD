using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Lind.DDD.OnlinePay.WapAlipay;
using System.Text;
using Lind.DDD.Utils;
using System.IO;
namespace Lind.DDD.UnitTest
{
    [TestClass]
    public class Rsa
    {

        string publickey = "MIGfMA0GCSqGSIb3DQEBAQUAA4GNADCBiQKBgQCE7dblrWkBoyi9Mwg5x1doTHK3r/riaf8Qo2+vF4/HcSnxW4VQnjZmRLfA139BP7avxg5my3XCmcAs/WOdVj2EeRFi2hY6zKTWCSjSfuvrizYF7Arnl2Qr4hlaK8+Io+d+azyMQH01ldgQ0koFJIjTSugrHVncCNt8a3xJ6W3b/QIDAQAB";
        string privatekey1 = "MIICdQIBADANBgkqhkiG9w0BAQEFAASCAl8wggJbAgEAAoGBAITt1uWtaQGjKL0zCDnHV2hMcrev+uJp/xCjb68Xj8dxKfFbhVCeNmZEt8DXf0E/tq/GDmbLdcKZwCz9Y51WPYR5EWLaFjrMpNYJKNJ+6+uLNgXsCueXZCviGVorz4ij535rPIxAfTWV2BDSSgUkiNNK6CsdWdwI23xrfEnpbdv9AgMBAAECgYBmw5wTuWjpbCJtigs857/KEPfKy9BvwzP9v+vyd4ueyvx66573wSgbDdrkyXRGlCP+ZWXy0C38wHISFd4x3170ZFdF/Z73lAPokjeigHyYdbXf7UGMP5cK9IvCnHmrdj/5rbSuaPVdIvOxX3/dKxkrbJajt19baXB1OPRhcLXKPQJBAPyhCDz71NiGhhkbELhq6h9YEi6xq3RynkQWoQu8hCF5wfaNHk+2oK4UKix0+7GGBKfti0XnP8q1ZlmdRb1H9hcCQQCGs+sVdLdMdYVE9HGfTHyuzLqcr1UAKJxbur7IggSOLJiJfanzCl6YpPNwxDkOkdqy6krNjc+FA9HhVgNDMp8LAkBSISAGg3CM/B74Zn4nFksK6ZzvpT8yWljtldXBdQYXw/e06efizmKqdU/IqDdzXTiCR42xqh5pDlQ56hFUGeL9AkAvXjl+1ApZIsJ4mxURlY4K0geVbbqQUbeaMjNAwyfi7x+8yannw2+8cdUBhJ8j1np3jBg97G5bMhquCNcg3D49AkBN1U7rOnlOIlGmfRlaP+USlsY0wmHzp3ySLbTUwCf9cmkZlnMhis2W49z9fyZno2adFY2koUNqjw30mP2ebjjR";
        string privatekey
        {
            get
            {

                using (FileStream fsRead = new FileStream(AppDomain.CurrentDomain.BaseDirectory + "\\privatekey.key", FileMode.Open))
                {
                    int fsLen = (int)fsRead.Length;
                    byte[] heByte = new byte[fsLen];
                    int r = fsRead.Read(heByte, 0, heByte.Length);
                    string myStr = System.Text.Encoding.UTF8.GetString(heByte);
                    return myStr.Trim();
                }

            }
        }
        string content = "a=123&b=456&charset=utf-8&sign_type=rsa";

        [TestMethod]
        public void printKey()
        {
            var rsa = RSATools.GetRSAKey();
            Console.WriteLine(rsa.Item1);
            Console.WriteLine(rsa.Item2);
        }
        /// <summary>
        /// 加密
        /// </summary>
        [TestMethod]
        public void encrypt()
        {
            //加密字符串  
            string data = "zzl";
            Console.WriteLine("加密前字符串内容：" + data);
            //加密  
            string encrypteddata = RSATools.EncryptData(ASCIIEncoding.UTF8.GetBytes(data), publickey);
            Console.WriteLine("加密后的字符串为：" + encrypteddata);
            Console.WriteLine(privatekey1 == privatekey);//左面第一个是空格
            Console.WriteLine("解密后的字符串内容：" + RSATools.DecryptData(encrypteddata, privatekey));


        }


        /// <summary>
        /// 生成签名
        /// </summary>
        [TestMethod]
        public void sign()
        {
            Console.WriteLine("sign=" + RSATools.Sign(content, privatekey));
        }
        /// <summary>
        /// 校验签名
        /// </summary>
        [TestMethod]
        public void verify()
        {
            var result = RSATools.Verify(content, RSATools.Sign(content, privatekey, "utf-8"), publickey, "utf-8");
            Console.WriteLine("verify=" + result);
        }

    }
}
