using Lind.DDD.Events;
using Lind.DDD.IoC;
using Microsoft.Practices.Unity;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Lind.DDD.UnitTest
{
    public interface IoCTest
    {
        void Hello();
    }
    public class IoCTestChina : IoCTest
    {

        public void Hello()
        {
            Console.WriteLine("世界你好！");
        }
    }
    public class IoCTestEnglish : IoCTest
    {
        public void Hello()
        {
            Console.WriteLine("Hello world!");
        }
    }



    [TestClass]
    public class IoC
    {
        [TestMethod]
        public void TestMethodIoC()
        {
            Console.WriteLine(typeof(IoCTest).IsAssignableFrom(typeof(IoCTestEnglish)));
            //全局入口注册
            string implementType = "Lind.DDD.UnitTest.IoCTestEnglish,Lind.DDD.UnitTest";
            IoCFactory.Instance.CurrentContainer.RegisterType(typeof(IoCTest), Type.GetType(implementType));
            //具体使用
            var helloIoC = IoCFactory.Instance.CurrentContainer.Resolve<IoCTest>();
            helloIoC.Hello();

            var ioc = new UnityContainer();
            ioc.RegisterType(typeof(IoCTestChina));
            Assert.IsTrue(ioc.IsRegistered(typeof(IoCTestChina)));
            var t = ioc.Resolve(typeof(IoCTestChina)) as IoCTestChina;
            t.Hello();
        }

        public static bool IsAssignableToGenericType(Type givenType, Type genericType)
        {
            if (genericType.IsAssignableFrom(givenType))
                return true;

            var interfaceTypes = givenType.GetInterfaces();

            foreach (var it in interfaceTypes)
            {
                if (it.IsGenericType && it.GetGenericTypeDefinition() == genericType)
                    return true;
            }

            if (givenType.IsGenericType && givenType.GetGenericTypeDefinition() == genericType)
                return true;

            Type baseType = givenType.BaseType;
            if (baseType == null) return false;

            return IsAssignableToGenericType(baseType, genericType);
        }



        [TestMethod]
        public void DllHelper()
        {
            foreach (Assembly assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                var eventTypes = assembly
                    .GetTypes()
                    .Where
                    (
                        type =>
                            IsAssignableToGenericType(type, typeof(Lind.DDD.Events.IEventHandler<>))
                            && !type.IsAbstract
                    );
                foreach (Type eventType in eventTypes)
                    Console.WriteLine(eventType);
            }
        }
    }
}
