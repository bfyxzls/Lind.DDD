using Lind.DDD.FastDFS;
using Lind.DDD.Logger;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web;

namespace Lind.DDD.Upload
{
    /// <summary>
    /// 使用fastDFS完成文件上传
    /// </summary>
    internal class FastDFSUploader : IFileUploader
    {
        /// <summary>
        /// 文件上传的缓冲区大小
        /// </summary>
        const int SIZE = 1024 * 1024;
        /// <summary>
        /// 目录名,需要提前在fastDFS上建立
        /// </summary>
        public string DFSGroupName { get { return "tsingda"; } }
        /// <summary>
        /// FastDFS结点
        /// </summary>
        public StorageNode Node { get; private set; }
        /// <summary>
        /// 服务器地址
        /// </summary>
        public string Host { get; private set; }
        /// <summary>
        /// 失败次数
        /// </summary>
        protected int FaildCount { get; set; }
        /// <summary>
        /// 失败阀值
        /// </summary>
        public int MaxFaildCount { get; set; }

        public FastDFSUploader()
        {
            InitStorageNode();
            MaxFaildCount = 3;
        }

        #region Private Methods
        private void InitStorageNode()
        {
            Node = FastDFSClient.GetStorageNode(DFSGroupName);
            Host = Node.EndPoint.Address.ToString();
        }

        private List<string> CreateImagePath(string fileName)
        {
            List<string> pathList = new List<string>();
            string snapshotPath = "";
            //视频截图
            List<string> localList = new VideoSnapshoter().GetVideoSnapshots(fileName, out snapshotPath);
            foreach (var item in localList)
            {
                string aImage = SmallFileUpload(item);
                pathList.Add(aImage);
            }
            //清除本地多余的图片，有的视频截取的图片多，有的视频截取的图片少
            string[] strArr = Directory.GetFiles(snapshotPath);
            try
            {
                foreach (var strpath in strArr)
                {
                    File.Delete(strpath);
                }
                Directory.Delete(snapshotPath);
            }
            catch (Exception ex)
            {
                LoggerFactory.Instance.Logger_Info("删除图片截图异常" + ex.Message);
            }
            return pathList;
        }
        private string SmallFileUpload(string filePath)
        {
            if (string.IsNullOrEmpty(filePath))
                throw new ArgumentNullException("filePath 参数不能为空");
            if (!File.Exists(filePath))
                throw new Exception("上传的文件不存在");
            byte[] content;
            using (FileStream streamUpload = new FileStream(filePath, FileMode.Open, FileAccess.Read))
            {
                using (BinaryReader reader = new BinaryReader(streamUpload))
                {
                    content = reader.ReadBytes((int)streamUpload.Length);
                }
            }
            string shortName = FastDFSClient.UploadFile(Node, content, "png");
            return GetFormatUrl(shortName);
        }
        /// <summary>
        /// 文件分块上传，适合大文件
        /// </summary>
        /// <param name="file"></param>
        /// <returns></returns>
        private string MultipartUpload(UploadParameterBase param)
        {
            Stream stream = param.Stream;
            if (stream == null)
                throw new ArgumentNullException("stream参数不能为空");
            byte[] content = new byte[SIZE];
            Stream streamUpload = stream;
            //  第一个数据包上传或获取已上传的位置
            string ext = param.FileName.Substring(param.FileName.LastIndexOf('.') + 1);
            streamUpload.Read(content, 0, SIZE);
            string shortName = FastDFSClient.UploadAppenderFile(Node, content, ext);

            BeginUploadPart(stream, shortName);

            return CompleteUpload(stream, shortName);
        }
        /// <summary>
        /// 断点续传
        /// </summary>
        /// <param name="stream"></param>
        /// <param name="serverShortName"></param>
        private void ContinueUploadPart(Stream stream, string serverShortName)
        {
            var serviceFile = FastDFSClient.GetFileInfo(Node, serverShortName);
            stream.Seek(serviceFile.FileSize, SeekOrigin.Begin);
            BeginUploadPart(stream, serverShortName);
        }
        /// <summary>
        /// 从指定位置开始上传文件
        /// </summary>
        /// <param name="stream"></param>
        /// <param name="beginOffset"></param>
        /// <param name="serverShortName"></param>
        private void BeginUploadPart(Stream stream, string serverShortName)
        {
            try
            {

                byte[] content = new byte[SIZE];

                while (stream.Position < stream.Length)
                {
                    stream.Read(content, 0, SIZE);
                    FastDFSClient.AppendFile(DFSGroupName, serverShortName, content);
                }
            }
            catch (Exception ex)
            {
                LoggerFactory.Instance.Logger_Info("上传文件中断！" + ex.Message);
                if (NetCheck())
                {
                    //重试
                    if (FaildCount < MaxFaildCount)
                    {
                        FaildCount++;
                        InitStorageNode();
                        ContinueUploadPart(stream, serverShortName);
                        Thread.Sleep(1000);
                    }
                    else
                    {
                        LoggerFactory.Instance.Logger_Info("已达到失败重试次数仍没有上传成功"); ;
                        throw ex;
                    }
                }
                else
                {
                    LoggerFactory.Instance.Logger_Info("当前网络不可用");
                    throw ex;
                }
            }
        }
        /// <summary>
        /// 网络可用为True,否则为False
        /// </summary>
        /// <returns></returns>
        private bool NetCheck()
        {
            return NetworkInterface.GetIsNetworkAvailable();
        }
        /// <summary>
        /// 拼接Url
        /// </summary>
        /// <param name="shortName"></param>
        /// <returns></returns>
        private string GetFormatUrl(string shortName)
        {
            return string.Format("http://{0}/{1}/{2}", Host, DFSGroupName, shortName);
        }

        private string CompleteUpload(Stream stream, string shortName)
        {
            stream.Close();
            return GetFormatUrl(shortName);
        }

        private string GetShortNameFromUrl(string url)
        {
            if (string.IsNullOrEmpty(url))
                return string.Empty;
            Uri uri = new Uri(url);
            string urlFirstPart = string.Format("http://{0}/{1}/", Host, DFSGroupName);
            if (!url.StartsWith(urlFirstPart))
                return string.Empty;
            return url.Substring(urlFirstPart.Length);
        }
        #endregion

        #region IFileUploader 成员

        /// <summary>
        /// 上传视频
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public VideoUploadResult UploadVideo(VideoUploadParameter param)
        {
            VideoUploadResult result = new VideoUploadResult();
            string fileName = MultipartUpload(param);
            if (param.IsScreenshot)
            {
                result.ScreenshotPaths = CreateImagePath(fileName);
            }
            result.FilePath = fileName;
            return result;
        }

        /// <summary>
        /// 上传普通文件
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public FileUploadResult UploadFile(FileUploadParameter param)
        {
            var result = new FileUploadResult();
            try
            {
                string fileName = MultipartUpload(param);
                result.FilePath = fileName;
            }
            catch (Exception ex)
            {

                result.ErrorMessage = ex.Message;
            }
            return result;
        }

        /// <summary>
        /// 上传图片
        /// </summary>
        /// <param name="param"></param>
        /// <param name="message"></param>
        /// <returns></returns>
        public ImageUploadResult UploadImage(ImageUploadParameter param)
        {
            byte[] content;
            string shortName = "";
            string ext = System.IO.Path.GetExtension(param.FileName).ToLower();
            if (param.FilenameExtension != null && param.FilenameExtension.Contains(ext))
            {
                if (param.Stream.Length > param.MaxSize)
                {
                    return new ImageUploadResult
                    {
                        ErrorMessage = "图片大小超过指定大小" + param.MaxSize / (1024 * 1024) + "M，请重新选择",
                        FilePath = shortName
                    };
                }
                else
                {
                    using (BinaryReader reader = new BinaryReader(param.Stream))
                    {
                        content = reader.ReadBytes((int)param.Stream.Length);
                    }

                    shortName = FastDFSClient.UploadFile(Node, content, ext.Contains('.') ? ext.Substring(1) : ext);
                }
            }
            else
            {
                return new ImageUploadResult
                {
                    ErrorMessage = "文件类型不匹配",
                    FilePath = shortName
                };

            }
            return new ImageUploadResult
            {
                FilePath = CompleteUpload(param.Stream, shortName),
            };
        }
        #endregion
    }
}
