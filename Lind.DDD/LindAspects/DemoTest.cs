using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lind.DDD.LindAspects
{
    public interface ITest
    {
        void Do();
    }
    /// <summary>
    /// AOP调用方式
    /// </summary>
    public class LoggerAspectTest : ITest
    {
        [LoggerAspectAttribute]
        public void Do()
        {
            //我做事情
            Console.WriteLine("我做事情");
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            ProxyFactory.CreateProxy<ITest>(typeof(LoggerAspectTest)).Do();
            Console.Read();
        }

    }
}
