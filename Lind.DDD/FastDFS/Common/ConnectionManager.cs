using System;
using System.Collections.Generic;
using System.Collections;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Diagnostics;
namespace Lind.DDD.FastDFS
{

    /// <summary>
    /// 服务器连接管理器
    /// </summary>
    public class ConnectionManager
    {
        private static List<IPEndPoint> listTrackers = new List<IPEndPoint>();

        public static Dictionary<IPEndPoint, Pool> trackerPools = new Dictionary<IPEndPoint, Pool>();
        public static Dictionary<IPEndPoint, Pool> storePools = new Dictionary<IPEndPoint, Pool>();
       
        public static bool Initialize(List<IPEndPoint> trackers)
        {
            foreach (IPEndPoint point in trackers)
            {
                if (!trackerPools.ContainsKey(point))
                    trackerPools.Add(point, new Pool(point, FDFSConfig.Tracker_MaxConnection));
            }
            listTrackers = trackers;
            return true;
        }
        public static Connection GetTrackerConnection()
        {
            Random random = new Random();
            int index = random.Next(trackerPools.Count);
            Pool pool = trackerPools[listTrackers[index]];
            return pool.GetConnection();
        }
        public static Connection GetStorageConnection(IPEndPoint endPoint)
        {
            lock ((storePools as ICollection).SyncRoot)
            {
                if (!storePools.ContainsKey(endPoint))
                {
                    Pool pool = new Pool(endPoint, FDFSConfig.Storage_MaxConnection);
                    storePools.Add(endPoint, pool);
                }
            }
            return storePools[endPoint].GetConnection();
        }
    }
}
