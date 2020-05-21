using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.SessionState;

namespace Lind.DDD.HttpExtensions.HttpHandlers
{
    /// <summary>
    /// 图片动态缩放处理程序
    /// 访Nigux的功能，访问图像资源可以访问http://xxx.png?w=100&h=100，我们会根据w和h为您缩放资源
    /// </summary>
    public class ImageScalingHandler : IHttpHandler
    {
        /// <summary>
        /// 图像等比例缩放，图像默认为白色
        /// </summary>
        /// <param name="image"></param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <returns></returns>
        private Bitmap CreateThumbnail(Image image, int width, int height)
        {
            Point point = new Point(0, 0); //图像从那个坐标点进行截取
            double wRate = 1, hRate = 1, setRate = 1;
            int newWidth = 0, newHeight = 0;
            try
            {
                if (width == 0) width = image.Width;
                if (height == 0) height = image.Height;

                if (image.Height > height)
                {
                    hRate = (double)height / image.Height;
                }

                if (image.Width > width)
                {
                    wRate = (double)width / image.Width;
                }

                if (wRate != 1 || hRate != 1)
                {
                    if (wRate > hRate)
                    {
                        setRate = hRate;
                    }
                    else
                    {
                        setRate = wRate;
                    }
                }

                newWidth = (int)(image.Width * setRate);
                newHeight = (int)(image.Height * setRate);
                if (height > newHeight)
                {
                    point.Y = Convert.ToInt32(height / 2 - newHeight / 2);
                }
                if (width > newWidth)
                {
                    point.X = Convert.ToInt32(width / 2 - newWidth / 2);
                }


                Bitmap bit = new Bitmap(width, height);
                Rectangle r = new Rectangle(point.X, point.Y, (int)(image.Width * setRate), (int)(image.Height * setRate));
                Graphics g = Graphics.FromImage(bit);
                g.Clear(Color.White);
                g.DrawImage(image, r);
                g.Dispose();
                return bit;
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// 处理请求
        /// </summary>
        /// <param name="context"></param>
        public void ProcessRequest(HttpContext context)
        {
            context.Response.Cache.SetCacheability(HttpCacheability.Public);
            context.Response.Cache.SetMaxAge(new TimeSpan(168, 0, 0));
            DateTime lastModifyTime = File.GetLastWriteTime(context.Request.PhysicalPath);

            #region 304和404
            string rawIfModifiedSince = context.Request.Headers.Get("If-Modified-Since");//ie,http1.0标准
            string rawETag = context.Request.Headers.Get("If-None-Match");//火狐,http1.1标准,更精确
            if (!File.Exists(context.Request.PhysicalPath))
            {
                context.Response.StatusCode = 404;
                context.Response.StatusDescription = "Resource is not Exist";
                return;
            }
            if (!string.IsNullOrWhiteSpace(rawIfModifiedSince))
            {
                CultureInfo provider = CultureInfo.InvariantCulture;
                var lastMod = DateTime.ParseExact(rawIfModifiedSince, "r", provider).ToLocalTime();
                if (lastModifyTime.ToString() == lastMod.ToString())
                {
                    context.Response.StatusCode = 304;
                    context.Response.StatusDescription = "Not Modified";
                    return;
                }
            }

            if (!string.IsNullOrWhiteSpace(rawETag))
            {
                if (rawETag == lastModifyTime.Ticks.ToString())
                {
                    context.Response.StatusCode = 304;
                    context.Response.StatusDescription = "Not Modified";
                    return;
                }
            }
            #endregion

            #region 正常加载

            context.Response.Cache.SetLastModified(lastModifyTime);
            context.Response.Cache.SetETag(lastModifyTime.Ticks.ToString());
            int w = 0, h = 0;
            int.TryParse(context.Request.QueryString["w"], out w);
            int.TryParse(context.Request.QueryString["h"], out h);
            Image image = Image.FromFile(context.Request.PhysicalPath);
            context.Response.ContentType = "image/jpeg";
            Bitmap bitMap = CreateThumbnail(image, w, h);
            bitMap.Save(context.Response.OutputStream, ImageFormat.Jpeg);
            image.Dispose();
            bitMap.Dispose();
            #endregion

        }

        public bool IsReusable
        {
            get { return false; }
        }
    }


}
