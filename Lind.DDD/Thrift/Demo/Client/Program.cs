using HelloThriftspace;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Thrift.Protocol;
using Thrift.Transport;

namespace Lind.DDD.Thrift.Demo.Client
{
    class Program
    {
        static void Main(string[] args)
        {
            //调用服务端的HelloThrift的HelloWorld方法
            TTransport transport = new TSocket("localhost", 9090);
            TProtocol protocol = new TBinaryProtocol(transport);
            TMultiplexedProtocol mp1 = new TMultiplexedProtocol(protocol, "HelloThriftHandler");
            HelloThrift.Client client = new HelloThrift.Client(mp1);
            transport.Open();
            client.HelloWorld();
            transport.Close();
        }
    }
}
