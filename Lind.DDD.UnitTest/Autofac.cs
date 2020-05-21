using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Autofac;
using Autofac.Configuration;
using System.Reflection;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using Lind.DDD.LindPlugins;
namespace Lind.DDD.UnitTest
{
    public class model { }
    public interface ITestRepository<T> where T : class
    {
        void Hello();
    }
    public class ADORepository<T> : ITestRepository<T> where T : class
    {
        #region ITestRepository<T> 成员

        public void Hello()
        {
            Console.WriteLine("ADO泛型你好!");
        }

        #endregion
    }
    public class SQLRepository<T> : ITestRepository<T> where T : class
    {
        #region ITestRepository<T> 成员

        public void Hello()
        {
            Console.WriteLine("SQL泛型你好!");
        }

        #endregion
    }
    public interface IMul : IPlugins
    {
        void Hello();
    }
    public class Mul1 : IMul
    {
        #region IMul 成员

        public void Hello()
        {
            Console.WriteLine("ThreadID:{0},IMul.Mul1...", Thread.CurrentThread.ManagedThreadId);
        }

        #endregion
    }
    public class Mul2 : IMul
    {
        #region IMul 成员

        public void Hello()
        {
            Console.WriteLine("ThreadID:{0},IMul.Mul2...", Thread.CurrentThread.ManagedThreadId);
        }

        #endregion
    }

    /// <summary>
    /// 目前不支持泛型类型的config注册
    /// </summary>
    [TestClass]
    public class Autofac
    {
        [TestMethod]
        public void TestMethod1()
        {
            ContainerBuilder builder = new ContainerBuilder();
            builder.RegisterModule(new ConfigurationSettingsReader("autofac"));

            using (var container = builder.Build())
            {
                container.Resolve<IoCTest>().Hello();
            }

        }
        [TestMethod]
        public void MultiResolve()
        {
            Stopwatch sw = new Stopwatch();
            sw.Restart();

            List<Action> actionList = new List<Action>();

            for (int i = 0; i < 1000; i++)
            {
                string name = "Lind.DDD.UnitTest.Mul2";
                if (i % 2 == 0)
                    name = "Lind.DDD.UnitTest.Mul1";
                actionList.Add(() =>
                {

                    IMul mul = PluginManager.Resolve<IMul>(name);
                    mul.Hello();
                });
            }
            Parallel.Invoke(actionList.ToArray());
            sw.Stop();
            Console.WriteLine("并发运行时间：{0}ms", sw.ElapsedMilliseconds);
        }
    }
}
