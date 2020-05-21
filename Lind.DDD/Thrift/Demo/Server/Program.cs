using HelloThriftspace;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Thrift.Protocol;
using Thrift.Server;
using Thrift.Transport;

namespace Lind.DDD.Thrift.Demo.Server
{
    class Program
    {
        static void Main(string[] args)
        {

            //开始Thrift rpc服务
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

        }
    }
}
