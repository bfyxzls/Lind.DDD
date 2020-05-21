using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Threading;
using Lind.DDD.LindPlugins;
using System.Linq;
using Lind.DDD.LindAspects;
namespace Lind.DDD.UnitTest
{
    public interface IAopHelloTest2 :
        IAspectProxy
    {
        List<DtoUser> GetData(string title, int age);
        void AddData(string title);
    }
    public class DtoUser
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }
    public class AopHello : IAopHelloTest2
    {
        #region IHello 成员
        [CachingAspect(CachingMethod.Get)]
        public List<DtoUser> GetData(string title, int age)
        {
            //我可能从缓存返回数据...
            return new Test_Code_FirstEntities().WebManageUsers.Select(i => new DtoUser
            {
                Id = i.ID,
                Name = i.LoginName
            }).ToList();

        }

        [CachingAspect(CachingMethod.Remove, "GetData")]
        public void AddData(string title)
        {
            Console.WriteLine("添加");
        }

        #endregion
    }
    [TestClass]
    public class Aop
    {
        [TestMethod]
        public void TestMethod1()
        {
            ITest test = ProxyFactory.CreateProxy(typeof(ITest), typeof(LoggerAspectAttribute)) as ITest;
            test.Do();
        }

        [TestMethod]
        public void AspectCachingGet()
        {
            var old = PluginManager.Resolve<IAopHelloTest2>();
            var result = old.GetData("lr", 1);
            Console.WriteLine(result);
        }

        [TestMethod]
        public void AspectCachingAdd()
        {
            var old = PluginManager.Resolve<IAopHelloTest2>("Lind.DDD.UnitTest.AopHello");
            old.AddData("zz");
        }

    }
}
