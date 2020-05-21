using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;

namespace Lind.DDD.HttpExtensions.HttpModules
{
    /// <summary>
    /// 实现URL的重写，支持扩展名jpg,png,jpeg,gif，主要用于图像的缩放，与ImageScalingHandler一起使用
    /// </summary>
    public class UrlRewriteModule : IHttpModule
    {
        #region IHttpModule 成员

        public void Init(HttpApplication context)
        {
            context.BeginRequest += new EventHandler(Application_BeginRequest);
        }
        /// <summary>
        /// url重写
        /// .png?w=100&h=100
        /// _100x100.png
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void Application_BeginRequest(Object sender, EventArgs e)
        {
            string oldUrl = HttpContext.Current.Request.RawUrl;
            string parttern = "[_]+(\\d)+[x]+(\\d)+[.](jpg|png|jpeg|gif)$";
            var result = Regex.Match(oldUrl, parttern);

            if (result.Success)
            {

                string ext = result.Groups[3].Value;
                string newUrl = Regex.Split(oldUrl, result.Value)[0];
                var param = result.Value
                    .Split(new char[] { '_' }, StringSplitOptions.RemoveEmptyEntries)[0]
                    .Split(new char[] { '.' }, StringSplitOptions.RemoveEmptyEntries)[0]
                    .Split(new char[] { 'x' }, StringSplitOptions.RemoveEmptyEntries);

                newUrl = string.Format("{0}.{1}?w={2}&h={3}", newUrl, ext, param[0], param[1]);
                //将请求中的URL进行重写  
                HttpContext.Current.RewritePath(newUrl);

            }

        }

        #endregion

        #region IHttpModule 成员

        public void Dispose()
        {
        }

        #endregion
    }
}
