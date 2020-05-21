using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Lind.DDD.Thrift.Demo.Server;
using HelloThriftspace;
using Thrift.Protocol;
using Thrift.Transport;
using Thrift.Server;
using System.Threading;

namespace Lind.DDD.UnitTest
{
    /// <summary>
    /// Thrift跨语言开发环境测试
    /// </summary>
    [TestClass]
    public class Thrift
    {
        [TestMethod]
        public void ThrifTestMethod1()
        {
            //服务端:开始Thrift rpc服务
            new Thread(() =>
            {

                var processor1 = new HelloThrift.Processor(new HelloThriftHandler());
                TMultiplexedProcessor processor = new TMultiplexedProcessor();
                processor.RegisterProcessor("HelloThriftHandler", processor1);
                var serverTransport = new TServerSocket(9090);
                var server1 = new TThreadedServer(processor, serverTransport);
                Console.WriteLine("向客户端输出服务开启");
                server1.Serve();
            }).Start();

            //客户端:调用服务端的HelloThrift的HelloWorld方法
            TTransport transport = new TSocket("localhost", 9090);
            TProtocol protocol = new TBinaryProtocol(transport);
            TMultiplexedProtocol mp1 = new TMultiplexedProtocol(protocol, "HelloThriftHandler");
            HelloThrift.Client client = new HelloThrift.Client(mp1);
            transport.Open();
            client.HelloWorld();
            client.adding(2, 3);
            Console.WriteLine(client.GetData(1));
            transport.Close();

        }
    }
}
