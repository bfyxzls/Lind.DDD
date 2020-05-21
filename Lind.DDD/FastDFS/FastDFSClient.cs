
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Configuration;
namespace Lind.DDD.FastDFS
{
    /// <summary>
    /// FastDFS客户端驱动
    /// </summary>
    public class FastDFSClient
    {
        #region Constructors
        /// <summary>
        /// fastDFS服务器地址列表
        /// </summary>
        static List<IPEndPoint> trackerIPs = new List<IPEndPoint>();
        /// <summary>
        /// FastDFS文件组
        /// </summary>
        public static StorageNode DefaultGroup;
        /// <summary>
        /// 当前默认的组，节，卷名称
        /// 开发人员可以通过FastDFSClient.GetStorageNode("groupname")去指定自己的组
        /// </summary>
        static FastDFSClient()
        {
            List<IPEndPoint> trackerIPs = new List<IPEndPoint>();
            TrackerSection trackersSection = ConfigurationManager.GetSection("TrackerSection") as TrackerSection;
            foreach (tracker item in trackersSection.Trackers)
            {
                trackerIPs.Add(new IPEndPoint(IPAddress.Parse(item.Host), item.Port));
            }
            ConnectionManager.Initialize(trackerIPs);
            try
            {
                DefaultGroup = GetStorageNode("tsingda");

            }
            catch (Exception ex)
            {
                Logger.LoggerFactory.Instance.Logger_Error(ex);
            }
        }
        #endregion

        /// <summary>
        /// 获取存储节点,组名为空返回默认组
        /// </summary>
        /// <param name="groupName">组名</param>
        /// <returns>存储节点实体类</returns>
        public static StorageNode GetStorageNode(string groupName = null)
        {
            FDFSRequest trackerRequest = QUERY_STORE_WITH_GROUP_ONE.Instance.GetRequest(groupName);
            QUERY_STORE_WITH_GROUP_ONE.Response trackerResponse = new QUERY_STORE_WITH_GROUP_ONE.Response(trackerRequest.GetResponse());
            IPEndPoint storeEndPoint = new IPEndPoint(IPAddress.Parse(trackerResponse.IPStr), trackerResponse.Port);

            if (groupName == null)
                return DefaultGroup;

            StorageNode result = new StorageNode();
            result.GroupName = trackerResponse.GroupName;
            result.EndPoint = storeEndPoint;
            result.StorePathIndex = trackerResponse.StorePathIndex;
            return result;
        }

        /// <summary>
        /// 拼接Url
        /// </summary>
        /// <param name="shortName"></param>
        /// <returns></returns>
        public static string GetFormatUrl(string host, string group, string shortName)
        {
            return string.Format("http://{0}/{1}/{2}", host, group, shortName);
        }
        /// <summary>
        /// 返回fastDFS路径
        /// </summary>
        /// <param name="url"></param>
        /// <param name="group"></param>
        /// <returns></returns>
        public static string GetShortNameFromUrl(string host, string url, string group)
        {
            if (string.IsNullOrEmpty(url))
                return string.Empty;
            Uri uri = new Uri(url);
            string urlFirstPart = string.Format("http://{0}/{1}/", host, group);
            if (!url.StartsWith(urlFirstPart))
                return string.Empty;
            return url.Substring(urlFirstPart.Length);
        }


        /// <summary>
        /// 上传文件到FastDFS
        /// </summary>
        /// <param name="storageNode"></param>
        /// <param name="contentByte"></param>
        /// <param name="fileExt"></param>
        /// <param name="beginDelegate">上传前回调</param>
        /// <param name="afterDelegate">上传后回调，参数为URL文件名</param>
        /// <returns>返回短文件名</returns>
        public static string UploadFile(StorageNode storageNode, byte[] contentByte, string fileExt, System.Action<string> beginDelegate, System.Action<string> afterDelegate)
        {
            if (null != beginDelegate)
                beginDelegate(fileExt);

            FDFSRequest storageReqeust = UPLOAD_FILE.Instance.GetRequest(storageNode.EndPoint, storageNode.StorePathIndex, contentByte.Length, fileExt, contentByte);
            UPLOAD_FILE.Response storageResponse = new UPLOAD_FILE.Response(storageReqeust.GetResponse());

            if (null != afterDelegate)
                afterDelegate(GetFormatUrl(storageNode.EndPoint.Address.ToString(), storageNode.GroupName, storageResponse.FileName));

            return storageResponse.FileName;
        }

        /// <summary>
        /// 上传文件到FastDFS
        /// </summary>
        /// <param name="storageNode"></param>
        /// <param name="contentByte"></param>
        /// <param name="fileExt"></param>
        /// <returns></returns>
        public static string UploadFile(StorageNode storageNode, byte[] contentByte, string fileExt)
        {
            return UploadFile(storageNode, contentByte, fileExt, null, null);
        }
        /// <summary>
        /// 上传从文件
        /// </summary>
        /// <param name="storageNode">GetStorageNode方法返回的存储节点</param>
        /// <param name="contentByte">文件内容</param>
        /// <param name="master_filename">主文件名</param>
        /// <param name="prefix_name">从文件后缀</param>
        /// <param name="fileExt">文件扩展名(注意:不包含".")</param>
        /// <returns>文件名</returns>
        public static string UploadSlaveFile(string groupName, byte[] contentByte, string master_filename, string prefix_name, string fileExt)
        {
            FDFSRequest trackerRequest = QUERY_UPDATE.Instance.GetRequest(groupName, master_filename);
            QUERY_UPDATE.Response trackerResponse = new QUERY_UPDATE.Response(trackerRequest.GetResponse());
            IPEndPoint storeEndPoint = new IPEndPoint(IPAddress.Parse(trackerResponse.IPStr), trackerResponse.Port);

            FDFSRequest storageReqeust = UPLOAD_SLAVE_FILE.Instance.GetRequest(storeEndPoint, contentByte.Length, master_filename, prefix_name, fileExt, contentByte);
            UPLOAD_FILE.Response storageResponse = new UPLOAD_FILE.Response(storageReqeust.GetResponse());
            return storageResponse.FileName;
        }
        /// <summary>
        /// 上传可以Append的文件
        /// </summary>
        /// <param name="storageNode">GetStorageNode方法返回的存储节点</param>
        /// <param name="contentByte">文件内容</param>
        /// <param name="fileExt">文件扩展名(注意:不包含".")</param>
        /// <returns>文件名</returns>
        public static string UploadAppenderFile(StorageNode storageNode, byte[] contentByte, string fileExt)
        {
            FDFSRequest storageReqeust = UPLOAD_APPEND_FILE.Instance.GetRequest(storageNode.EndPoint, storageNode.StorePathIndex, contentByte.Length, fileExt, contentByte);
            UPLOAD_APPEND_FILE.Response storageResponse = new UPLOAD_APPEND_FILE.Response(storageReqeust.GetResponse());
            return storageResponse.FileName;
        }
        /// <summary>
        /// 附加文件
        /// </summary>
        /// <param name="groupName">组名</param>
        /// <param name="fileName">文件名</param>
        /// <param name="contentByte">文件内容</param>
        public static byte[] AppendFile(string groupName, string fileName, byte[] contentByte)
        {
            FDFSRequest trackerRequest = QUERY_UPDATE.Instance.GetRequest(groupName, fileName);
            QUERY_UPDATE.Response trackerResponse = new QUERY_UPDATE.Response(trackerRequest.GetResponse());
            IPEndPoint storeEndPoint = new IPEndPoint(IPAddress.Parse(trackerResponse.IPStr), trackerResponse.Port);

            FDFSRequest storageReqeust = APPEND_FILE.Instance.GetRequest(storeEndPoint, fileName, contentByte);
            return storageReqeust.GetResponse();
        }
        /// <summary>
        /// 删除文件
        /// </summary>
        /// <param name="groupName">组名</param>
        /// <param name="fileName">文件名</param>
        public static void RemoveFile(string groupName, string fileName)
        {
            FDFSRequest trackerRequest = QUERY_UPDATE.Instance.GetRequest(groupName, fileName);
            QUERY_UPDATE.Response trackerResponse = new QUERY_UPDATE.Response(trackerRequest.GetResponse());
            IPEndPoint storeEndPoint = new IPEndPoint(IPAddress.Parse(trackerResponse.IPStr), trackerResponse.Port);
            FDFSRequest storageReqeust = DELETE_FILE.Instance.GetRequest(storeEndPoint, groupName, fileName);
            storageReqeust.GetResponse();
        }
        /// <summary>
        /// 下载文件
        /// </summary>
        /// <param name="storageNode">GetStorageNode方法返回的存储节点</param>
        /// <param name="fileName">文件名</param>
        /// <returns>文件内容</returns>
        public static byte[] DownloadFile(StorageNode storageNode, string fileName)
        {
            FDFSRequest storageReqeust = DOWNLOAD_FILE.Instance.GetRequest(storageNode.EndPoint, 0L, 0L, storageNode.GroupName, fileName);
            DOWNLOAD_FILE.Response storageResponse = new DOWNLOAD_FILE.Response(storageReqeust.GetResponse());
            return storageResponse.Content;
        }
        /// <summary>
        /// 增量下载文件
        /// </summary>
        /// <param name="storageNode">GetStorageNode方法返回的存储节点</param>
        /// <param name="fileName">文件名</param>
        /// <param name="offset">从文件起始点的偏移量</param>
        /// <param name="length">要读取的字节数</param>
        /// <returns>文件内容</returns>
        public static byte[] DownloadFile(StorageNode storageNode, string fileName, long offset, long length)
        {
            FDFSRequest storageReqeust = DOWNLOAD_FILE.Instance.GetRequest(storageNode.EndPoint, offset, length, storageNode.GroupName, fileName);
            DOWNLOAD_FILE.Response storageResponse = new DOWNLOAD_FILE.Response(storageReqeust.GetResponse());
            return storageResponse.Content;
        }
        /// <summary>
        /// 获取文件信息
        /// </summary>
        /// <param name="storageNode">GetStorageNode方法返回的存储节点</param>
        /// <param name="fileName">文件名</param>
        /// <returns></returns>
        public static FDFSFileInfo GetFileInfo(StorageNode storageNode, string fileName)
        {
            FDFSRequest storageReqeust = QUERY_FILE_INFO.Instance.GetRequest(storageNode.EndPoint, storageNode.GroupName, fileName);
            FDFSFileInfo result = new FDFSFileInfo(storageReqeust.GetResponse());
            return result;
        }
    }
}

