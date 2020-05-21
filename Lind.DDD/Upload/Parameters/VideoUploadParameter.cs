using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.IO;
namespace Lind.DDD.Upload
{
    /// <summary>
    /// 视频上传参数对象
    /// </summary>
    public class VideoUploadParameter : UploadParameterBase
    {
        public bool IsScreenshot { get; set; }

        public VideoUploadParameter(Stream stream, bool isScreenshot, string fileName)
        {
            Stream = stream;
            IsScreenshot = isScreenshot;
            FileName = fileName;
        }
        public VideoUploadParameter(Stream stream, string fileName)
        {
            Stream = stream;
            IsScreenshot = false;
            FileName = fileName;
        }
    }
}
