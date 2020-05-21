using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;

namespace Lind.DDD.FastDFS
{
    /// <summary>
    /// 管理连接池
    /// </summary>
    public class Pool
    {
        public Pool(IPEndPoint endPoint, int maxConnection)
        {
            autoEvent = new AutoResetEvent(false);
            inUse = new List<Connection>(maxConnection);
            idle = new Stack<Connection>(maxConnection);
            this.maxConnection = maxConnection;
            this.endPoint = endPoint;
        }

        /// <summary>
        /// 使用中的服务器
        /// </summary>　
        private List<Connection> inUse;
        /// <summary>
        /// 断网的服务器
        /// </summary>
        private Stack<Connection> idle;
        private AutoResetEvent autoEvent = null;
        private IPEndPoint endPoint = null;
        private int maxConnection = 0;

        private Connection GetPoolConncetion()
        {
            Connection result = null;
            lock ((idle as ICollection).SyncRoot)
            {
                if (idle.Count > 0)
                    result = idle.Pop();
                if (result != null && (int)(DateTime.Now - result.LastUseTime).TotalSeconds > FDFSConfig.Connection_LifeTime)
                {
                    foreach (Connection conn in idle)
                    {
                        conn.Close();
                    }
                    idle = new Stack<Connection>(maxConnection);
                    result = null;
                }
            }
            lock ((inUse as ICollection).SyncRoot)
            {
                if (inUse.Count == maxConnection)
                    return null;
                if (result == null)
                {
                    result = new Connection();
                    result.Connect(endPoint);
                    result.Pool = this;
                }
                inUse.Add(result);
            }
            return result;
        }

        public Connection GetConnection()
        {
            int timeOut = FDFSConfig.ConnectionTimeout * 1000;
            Connection result = null;
            Stopwatch watch = Stopwatch.StartNew();
            while (timeOut > 0)
            {
                result = GetPoolConncetion();
                if (result != null)
                    return result;
                if (!autoEvent.WaitOne(timeOut, false))
                    break;
                watch.Stop();
                timeOut = timeOut - (int)watch.ElapsedMilliseconds;
            }
            throw new FDFSException("Connection Time Out");
        }
        public void ReleaseConnection(Connection conn)
        {
            if (!conn.InUse)
            {
                try
                {
                    FDFSHeader header = new FDFSHeader(0, Consts.FDFS_PROTO_CMD_QUIT, 0);
                    byte[] buffer = header.ToByte();
                    conn.GetStream().Write(buffer, 0, buffer.Length);
                    conn.GetStream().Close();
                }
                catch
                {
                }
            }
            conn.Close();
            lock ((inUse as ICollection).SyncRoot)
            {
                inUse.Remove(conn);
            }
            autoEvent.Set();
        }
        public void CloseConnection(Connection conn)
        {
            conn.InUse = false;
            lock ((inUse as ICollection).SyncRoot)
            {
                inUse.Remove(conn);
            }
            lock ((idle as ICollection).SyncRoot)
            {
                idle.Push(conn);
            }
            autoEvent.Set();
        }
    }
}
