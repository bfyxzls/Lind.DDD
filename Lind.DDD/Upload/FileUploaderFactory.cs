using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lind.DDD.Upload
{
    /// <summary>
    /// 文件上传生产者
    /// </summary>
    public sealed class FileUploaderFactory
    {
        /// <summary>
        /// 上传实例
        /// </summary>
        public readonly static IFileUploader Instance;
        private static object lockObj = new object();

        static FileUploaderFactory()
        {
            if (Instance == null)
            {
                lock (lockObj)
                {
                    Instance = new FastDFSUploader();
                }
            }
        }
    }
}
