using Lind.DDD.ConfigConstants;
using Lind.DDD.ConfigConstants.Models;
using PureCat.Util;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;

namespace PureCat.Configuration
{
    public class ClientConfigManager
    {
        public ClientConfig ClientConfig { get; private set; }

        public ClientConfigManager()
        {
            Initialize();
        }



        private void Initialize(ClientConfig clientConfig)
        {
            if (clientConfig != null)
            {
                ClientConfig = clientConfig;
                LoadServerConfig();//加载远程服务器的路由配置"http://192.168.2.10:2281/cat/s/router?domain=zzl"

                ClientConfig.RandomServer();
            }

        }

        private void Initialize()
        {
            ClientConfig config = new Configuration.ClientConfig();
            var domain = ConfigManager.Config.Cat.CatDomain;
            var servers = ConfigManager.Config.Cat.CatServers.Where(i => i.Enabled).ToList();


            config.Domain = domain;
            servers.ForEach(server =>
            {
                config.Servers.Add(server);
            });


            Initialize(config);
        }





        /// <summary>
        /// 返回标准的服务端路由（服务器列表）
        /// </summary>
        /// <param name="webPort"></param>
        /// <returns></returns>
        private string GetServerConfigUrl(int webPort = -1)
        {
            if (ClientConfig == null)
                return null;
            var serverList = ClientConfig.Servers.Where(server => server.Enabled);
            foreach (var server in serverList)
            {
                return string.Format("http://{0}:{1}/cat/s/router?domain={2}", server.Ip, (webPort > 0 ? webPort : server.WebPort), ClientConfig.Domain.Id);
            }
            return null;
        }

        private void LoadServerConfig()
        {
            var serverListContent = CatHttpRequest.GetRequest(GetServerConfigUrl());
            if (string.IsNullOrWhiteSpace(serverListContent))
            {
                serverListContent = CatHttpRequest.GetRequest(GetServerConfigUrl(8080));
            }
            if (string.IsNullOrWhiteSpace(serverListContent))
            {
                return;
            }

            PureCat.Util.Logger.Info("Get servers : {0}", serverListContent);


            var serverListSplit = serverListContent.TrimEnd(';').Split(';');

            List<CatServer> serverList = new List<CatServer>();

            foreach (var serverContent in serverListSplit)
            {
                try
                {
                    var content = serverContent.Split(':');
                    var ip = content[0];
                    var port = content[1];
                    serverList.Add(new CatServer(ip, int.Parse(port)));
                }
                catch
                {
                }
            }

            if (serverList.Count > 0)
            {
                ClientConfig.Servers = serverList;
            }
        }
    }
}
