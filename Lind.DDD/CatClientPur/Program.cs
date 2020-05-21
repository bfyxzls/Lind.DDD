using PureCat.Context;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
namespace PureCat
{
    class Program
    {
        /// <summary>
        /// 这个相当于Ａ网站
        /// </summary>
        /// <param name="args"></param>
        static void Main(string[] args)
        {
            #region Cat实时监控

            CatClient.Initialize();
            var context = CatClient.DoTransaction("Do", "Test", () =>
            {
                CatClient.NewEvent("Do1", "Test1");
                CatClient.NewEvent("Do2", "Test2");
            });

            CatClient.NewEvent("outer", "It is outter with Do");　//它不在内部，与Do事务是独立的

            var url = "http://localhost:4532/home/index";
            var handler = new HttpClientHandler() { };
            using (var http = new HttpClient(handler))
            {
                CatClient.SetCatContextToRequestHeader(http, context);
                var response = http.GetAsync(url).Result;
                var staus = response.IsSuccessStatusCode;
            }

            Console.ReadLine();
            #endregion

        }

        /// <summary>
        /// 这是相当于Ｂ网站
        /// </summary>
        static void DistributeApi()
        {
            CatClient.DoTransactionAction("Do", "Add", () =>
            {
                CatClient.LogRemoteCallServer(CatClient.GetCatContextFromServer());
                CatClient.LogEvent("Do", "Add", "0", "hello distribute api４５６");
                CatClient.LogError(new Exception());
            });
        }
    }
}
