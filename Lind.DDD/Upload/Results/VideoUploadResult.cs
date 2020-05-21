using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lind.DDD.Upload
{
    /// <summary>
    /// 视频上传返回对象
    /// </summary>
    public class VideoUploadResult : UploadResultBase
    {
        /// <summary>
        /// 上传的视频截图地址
        /// </summary>
        public List<string> ScreenshotPaths { get; set; }
        /// <summary> 
        /// 上传状态
        /// </summary>
        public UploadStatus UploadStatus { get; set; }

        public VideoUploadResult()
        {
            ScreenshotPaths = new List<string>();
        }
        /// <summary>
        /// 把VideoPath和ScreenshotPaths拼起来  以竖线（|）隔开
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(FilePath);
            foreach (var item in ScreenshotPaths)
            {
                sb.Append("|" + item);
            }
            return sb.ToString();
        }
    }

}
