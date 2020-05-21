using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lind.DDD.Upload
{
    /// <summary>
    /// 图片上传参数对象
    /// </summary>
    public class ImageUploadParameter : UploadParameterBase
    {
        /// <summary>
        /// 构造方法
        /// </summary>
        /// <param name="stream"></param>
        /// <param name="fileName"></param>
        /// <param name="filenameExtension">默认支持常用图片格式</param>
        /// <param name="maxSize">3M</param>
        public ImageUploadParameter(Stream stream, string fileName, string[] filenameExtension = null, int maxSize = (3*1024*1024))
        {
            base.Stream = stream;
            base.FileName = fileName;
            base.MaxSize = maxSize;
            base.FilenameExtension = filenameExtension ?? new string[] { ".jpeg", ".jpg", ".gif", ".png" }; ;

        }
        /// <summary>
        /// 构造方法
        /// </summary>
        /// <param name="stream"></param>
        /// <param name="fileName"></param>
        /// <param name="maxSize">单位为M</param>
        public ImageUploadParameter(Stream stream, string fileName, int maxSize)
            : this(stream, fileName, null, maxSize) { }

    }

}
