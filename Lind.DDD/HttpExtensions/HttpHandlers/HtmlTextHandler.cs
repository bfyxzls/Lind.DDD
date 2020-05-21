using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace Lind.DDD.HttpExtensions.HttpHandlers
{
    /// <summary>
    /// HTML编码的处理程序
    /// </summary>
    public class HtmlTextHandler : DelegatingHandler
    {
        protected async override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            var response = await base.SendAsync(request, cancellationToken);
            var contentType = response.Content.Headers.ContentType;
            contentType.CharSet = await getCharSetAsync(response.Content);
            return response;
        }

        private async Task<string> getCharSetAsync(HttpContent httpContent)
        {
            var charset = httpContent.Headers.ContentType.CharSet;
            if (!string.IsNullOrEmpty(charset))
                return charset;

            var content = await httpContent.ReadAsStringAsync();
            var match = Regex.Match(content, @"charset=(?<charset>.+?)""", RegexOptions.IgnoreCase);
            if (!match.Success)
                return charset;

            return match.Groups["charset"].Value;
        }

        /*
          public static class WebApiConfig 
          { 
              public static void Register(HttpConfiguration config) 
              { 
                  config.MessageHandlers.Add(new MessageHandler1()); 
                  config.MessageHandlers.Add(new MessageHandler2()); 
          
                  // Other code not shown（未列出其它代码）... 
              } 
          }
         
        */
    }
}
