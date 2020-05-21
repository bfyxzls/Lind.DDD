using Lind.DDD.ConfigConstants;
using Lind.DDD.ConfigConstants.Models;
using System;
using System.Collections.Generic;

namespace PureCat.Configuration
{
    /// <summary>
    ///   Cat客户端配置
    /// </summary>
    public class ClientConfig
    {
        private CatDomain _mDomain;
        private Random _mRandom = new Random();
        private readonly object _lock = new object();

        private List<CatServer> _server = new List<CatServer>();

        public ClientConfig(CatDomain domain = null, params CatServer[] serverList)
        {
            _mDomain = domain ?? new CatDomain();
            if (serverList != null && serverList.Length != 0)
                Servers.AddRange(serverList);
            RandomServer();
        }

        public CatDomain Domain
        {
            get { return _mDomain ?? (_mDomain = new CatDomain()); }

            set { _mDomain = value; }
        }

        /// <summary>
        ///   Cat日志服务器，可以有多个
        /// </summary>
        public List<CatServer> Servers { get { return _server; } set { lock (_lock) _server = value; } }
        public void RandomServer()
        {
            if (_server == null || _server.Count < 2)
                return;
            lock (_lock)
            {
                int k = 0;
                int index = 0;
                CatServer tmpServer = null;
                for (int i = 0; i < _server.Count * 3; i++)
                {
                    index = i % _server.Count;
                    k = _mRandom.Next(_server.Count);
                    if (k != index)
                    {
                        tmpServer = _server[index];
                        _server[index] = _server[k];
                        _server[k] = tmpServer;
                    }
                }
            }

        }
    }
}