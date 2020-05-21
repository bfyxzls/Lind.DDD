using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;

namespace Lind.DDD.CachingQueue.Implements
{
    /// <summary>
    /// 基于文件的队列
    /// </summary>
    public class FileQueue : IQueue
    {

        private static string _folder = Environment.CurrentDirectory
            + "\\"
            + ConfigConstants.ConfigManager.Config.Queue.FilePath
            ?? "FileQueue"
            + "\\";

        #region IQueue 成员

        public void Push(string key, byte[] obj)
        {

            string fileName = Path.Combine(_folder, key, string.Format("{0:yyyyMMddHHmmssffff}.{1}", DateTime.Now, "log"));
            FileStream fs = null;
            if (!System.IO.Directory.Exists(_folder))
            {
                System.IO.Directory.CreateDirectory(_folder);
            }

            try
            {
                fs = new FileStream(fileName, FileMode.Create, FileAccess.Write, FileShare.ReadWrite);
                fs.Write(obj, 0, obj.Length);
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                fs.Close();
                fs.Dispose();
                fs = null;
            }
        }

        public byte[] Pop(string key)
        {
            if (this.Count > 0)
            {
                var nameArr = System.IO.Directory.GetFiles(Path.Combine(_folder, key));
                if (nameArr != null && nameArr.Length > 0)
                {
                    string name = nameArr.OrderByDescending(i => i).First();
                    FileStream fs = null;
                    try
                    {
                        fs = new FileStream(name, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
                        byte[] pReadByte = new byte[0];
                        BinaryReader r = new BinaryReader(fs);
                        r.BaseStream.Seek(0, SeekOrigin.Begin);
                        pReadByte = r.ReadBytes((int)r.BaseStream.Length);
                        return pReadByte;
                    }
                    catch (Exception)
                    {
                        throw;
                    }
                    finally
                    {
                        fs.Close();
                        fs.Dispose();
                        fs = null;
                        //清除这个被pop出来的文件
                        File.Delete(name);
                    }
                }
            }
            return null;
        }

        public int Count
        {
            get
            {
                if (System.IO.Directory.Exists(_folder))
                {
                    return System.IO.Directory.GetFiles(_folder).Length;
                }
                return 0;
            }
        }

        #endregion
    }
}
