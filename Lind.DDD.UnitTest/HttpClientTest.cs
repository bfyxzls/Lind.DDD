using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Diagnostics;
using System.Net.Http;

namespace Lind.DDD.UnitTest
{
    [TestClass]
    public class HttpClientTest
    {
        readonly static HttpClient http = new HttpClient(new HttpClientHandler()
        {
            AutomaticDecompression = System.Net.DecompressionMethods.GZip
        });
        [TestMethod]
        public void Get()
        {
            Stopwatch sw = new Stopwatch();
            sw.Restart();
            for (int i = 0; i < 1000; i++)
            {
                var response = http.GetAsync("http://www.sina.com").Result;
            }
            sw.Stop();
            Console.WriteLine("1000个请求的时间" + sw.ElapsedMilliseconds);
        }
    }
}
