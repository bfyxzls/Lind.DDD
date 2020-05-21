using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.IO;
using System.Net;
using System.Drawing;
using System.Threading;
 using System.Drawing.Imaging;
using System.Threading.Tasks;
using Lind.DDD.Utils.ImageHelper;

namespace Lind.DDD.HttpExtensions.HttpResponse
{
    /// <summary>
    /// 提供一组文件下载的接口
    /// </summary>
    public class FileRequestController : Controller
    {
        //
        // GET: /DownLoad/

        /// <summary>
        ///  下载Http请求的文件,走http协议
        /// </summary>
        /// <param name="url">图片的ＵＲＩ</param>
        /// <param name="contentType">图片的内容类型</param>
        /// <returns></returns>
        public ActionResult DownHttpRequest(string url)
        {
            Uri uri = new Uri(url);
            if (uri == null)
                throw new ArgumentException("不合法的ＵＲＬ地址");

            string fileName = uri.Segments[uri.Segments.Length - 1];
            string fileExt = fileName.Substring(fileName.LastIndexOf('.') + 1);
            var imageRequest = (HttpWebRequest)WebRequest.Create(uri);
            imageRequest.Timeout = 1000 * 5;  //5s 超时
            var httpResponse = (HttpWebResponse)imageRequest.GetResponse();
            var stream = httpResponse.GetResponseStream();
            return File(
                stream,
                httpResponse.ContentType,
                fileName);
        }

        /// <summary>
        /// 为图像加文字
        /// </summary>
        /// <param name="url">图像文件URI</param>
        /// <param name="text">加添加的字符</param>
        /// <returns></returns>
        public ActionResult ImageAddText(string url, string text)
        {
            var imageRequest = (HttpWebRequest)WebRequest.Create(url);
            var httpResponse = (HttpWebResponse)imageRequest.GetResponse();
            imageRequest.Timeout = 1000 * 5;  //5s 超时
            return File(
                ImageHelper.WordToImageStream(httpResponse.GetResponseStream(), text),
                httpResponse.ContentType);
        }


        /// <summary>
        /// 下载File请求的文件,走File协议, 如file://www.cnblogs.com/lori/1.jpg
        /// </summary>
        /// <param name="url">文件ＵＲＩ或者本地路径</param>
        /// <param name="fileType">文件类型</param>
        /// <returns></returns>
        public ActionResult DownFileRequest(string url)
        {
            Uri uri = new Uri(url);
            if (uri == null)
                throw new ArgumentException("不合法的ＵＲＬ地址");

            string fileName = uri.Segments[uri.Segments.Length - 1];
            var request = (FileWebRequest)WebRequest.Create(uri);
            var response = (FileWebResponse)request.GetResponse();
            var stream = response.GetResponseStream();
            return File(
                stream,
                response.ContentType,
                fileName);//以附件的形式返回

        }
        /// <summary>
        /// 将HTTP请求的文件下载到WWW服务器，分段下载
        /// </summary>
        /// <param name="url">源地址</param>
        /// <param name="desFilePath">目标地址</param>
        public void Down(string url, string desFilePath)
        {
            Uri uri = new Uri(url);
            if (uri == null)
                throw new ArgumentException("不合法的ＵＲＬ地址");
            string fileName = uri.Segments[uri.Segments.Length - 1];

            var request = (HttpWebRequest)WebRequest.Create(uri);
            var response = (HttpWebResponse)request.GetResponse();

            AsyncWriteFileFromStream(desFilePath, response);
            Response.Write("文件下载完成,保存于：" + desFilePath);
        }

        /// <summary>
        /// 异步方式，将HTTP响应流写入文件
        /// Verson:.net frameworks 4.5
        /// </summary>
        /// <param name="desFilePath"></param>
        /// <param name="response"></param>
        /// <returns></returns>
        async void AsyncWriteFileFromStream(string desFilePath, HttpWebResponse response)
        {
            using (FileStream fs = new FileStream(desFilePath, FileMode.Create, FileAccess.Write))
            {
                using (var stream = response.GetResponseStream())
                {
                    //缓冲区太小的话，速度慢而且伤硬盘
                    //声明一个4兆字节缓冲区大小，比如迅雷也有一个缓冲区，如果没有缓冲区的话，
                    //每下载一个字节都要往磁盘进行写，非常伤磁盘，所以，先往内存的缓冲区写字节，当
                    //写够了一定容量之后，再往磁盘进行写操作，减低了磁盘操作。
                    byte[] buffer = new byte[0x1000];//开避4K的缓冲区0x1000为4K
                    int readBytes;
                    //第二个参数Offset表示当前位置的偏移量，一般都传0
                    while ((readBytes = stream.Read(buffer, 0, buffer.Length)) > 0) //读取的位置自动往后挪动。
                    {
                        //readBytes为实际读到的byte数，因为最后一次可能不会读满。
                        await fs.WriteAsync(buffer, 0, readBytes);
                    }

                }
            }
        }


    }
}
