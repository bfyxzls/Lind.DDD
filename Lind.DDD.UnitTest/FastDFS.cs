using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;
using Lind.DDD.FastDFS;
using System.Collections.Generic;
using System.Net;
using Lind.DDD.Logger;
using System.Net.NetworkInformation;
using System.Threading.Tasks;

namespace Lind.DDD.UnitTest
{
    [TestClass]
    public class FastDFS
    {

        #region Fields & Properties
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
        #endregion
        private void InitStorageNode()
        {
            Node = FastDFSClient.GetStorageNode(DFSGroupName);
            Host = Node.EndPoint.Address.ToString();
        }
        public FastDFS()
        {
            InitStorageNode();
            MaxFaildCount = 3;
        }

        [TestMethod]
        public void UploadFile()
        {

            #region 处理文件上传，并把结果返回fileName
            if (File.Exists(@"e:\lzs.jpg"))
            {
                FileStream streamUpload = new FileStream(@"e:\lzs.jpg", FileMode.Open);
                string fileName = MultipartUpload(streamUpload, "jpg");
                Console.WriteLine("upload file:" + fileName);
            }
            #endregion

        }

        /// <summary>
        /// 文件分块上传，适合大文件
        /// </summary>
        /// <param name="file"></param>
        /// <returns></returns>
        private string MultipartUpload(Stream stream, string ext)
        {
            if (stream == null)
                throw new ArgumentNullException("stream参数不能为空");
            int size = 1024 * 1024;
            byte[] content = new byte[size];
            //  第一个数据包上传或获取已上传的位置
            stream.Read(content, 0, size);
            string shortName = FastDFSClient.UploadAppenderFile(Node, content, ext);
            BeginUploadPart(stream, shortName, size);
            return CompleteUpload(stream, shortName);
        }
        /// <summary>
        /// 断点续传
        /// </summary>
        /// <param name="stream"></param>
        /// <param name="serverShortName"></param>
        private void ContinueUploadPart(Stream stream, string serverShortName, int bufferSize)
        {
            var serviceFile = FastDFSClient.GetFileInfo(Node, serverShortName);
            stream.Seek(serviceFile.FileSize, SeekOrigin.Begin);
            BeginUploadPart(stream, serverShortName, bufferSize);
        }
        /// <summary>
        /// 从指定位置开始上传文件
        /// </summary>
        /// <param name="stream"></param>
        /// <param name="beginOffset"></param>
        /// <param name="serverShortName"></param>
        private void BeginUploadPart(Stream stream, string serverShortName, int bufferSize)
        {
            try
            {

                byte[] content = new byte[bufferSize];

                while (stream.Position < stream.Length)
                {
                    stream.Read(content, 0, bufferSize);

                    var result = FastDFSClient.AppendFile(DFSGroupName, serverShortName, content);
                    Console.WriteLine("分块上传开始,大小:" + bufferSize);
                    if (result.Length == 0)
                    {
                        FaildCount = 0;
                        continue;
                    }
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
                        ContinueUploadPart(stream, serverShortName, bufferSize);
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

    }
}
