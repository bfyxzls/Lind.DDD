using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lind.DDD.Thrift.Demo.Server
{
    public class HelloThriftHandler :
        HelloThriftspace.HelloThrift.Iface
    {
        #region Iface 成员

        public void HelloWorld()
        {
            Console.WriteLine("hello world!");
        }

        public string GetData(int uid)
        {
            return "从客户端传来的数据是:" + uid;
        }

        #endregion


        public int adding(int a, int b)
        {
            Console.WriteLine("a+b={0}", (a + b).ToString());
            return a + b;
        }
    }

}
