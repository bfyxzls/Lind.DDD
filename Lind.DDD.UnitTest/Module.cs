using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Lind.DDD.Modules;
using Lind.DDD.IoC;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Reflection;
using System.IO;
namespace Lind.DDD.UnitTest
{

    public interface ITools
    {
        void Hello();
    }
    public class MongoTools : ITools
    {
        public void Hello()
        {
            Console.WriteLine("你好ITools.MongoTools！");
        }
    }
    public class MongoModule : LindModule
    {
        public override void Initialize()
        {
            //初始化和它有关的类型，一般是一个程序集
            IocManager.RegisterType(typeof(ITools), typeof(MongoTools));
            base.Initialize();
        }
    }

    public class RedisTools : ITools
    {
        public void Hello()
        {
            Console.WriteLine("你好ITools.RedisTools！");
        }
    }
    public class RedisModule : LindModule
    {
        public override void Initialize()
        {
            //初始化
            IocManager.RegisterType(typeof(ITools), typeof(RedisTools));
            base.Initialize();
        }
    }
    public class EFModule : LindModule
    {
        public override void Initialize()
        {
            base.Initialize();
        }
    }

    /// <summary>
    /// Module的单元测试
    /// </summary>
    [TestClass]
    public class Module
    {
        [TestMethod]
        public void ModuleTestMethod1()
        {
            IContainer IocManager = IoCFactory.Instance.CurrentContainer;
            new LindModuleManager(IocManager).InitializeModules();
            var arr = IocManager.Resolve<MongoTools>();
            arr.Hello();

        }

    }

    #region 大叔自己写的,已经不用了
    /// <summary>
    /// 大叔设计
    /// </summary>
    public class ModuleFactory
    {
        /// <summary>
        /// 初始化所有模块
        /// </summary>
        public void InitModules()
        {

            IContainer IocManager = IoCFactory.Instance.CurrentContainer;
            foreach (var dll in GetAllAssemblies())
            {
                foreach (var t in GetAllModules(dll))
                {
                    if (!IocManager.IsRegistered(t))
                    {
                        IocManager.RegisterType(t, t);
                        Console.WriteLine("注册类型：{0}", t.Name);
                    }
                    var module = IocManager.Resolve(t) as LindModule;
                    module.PreInitialize();
                    module.Initialize();
                }
            }
        }

        /// <summary>
        /// 当前应用程序下所有dll文件下所有Module
        /// </summary>
        /// <returns></returns>
        public List<Assembly> GetAllAssemblies()
        {
            return AppDomain.CurrentDomain.GetAssemblies().ToList();
        }

        /// <summary>
        /// 加载所有Lind的模版
        /// </summary>
        /// <param name="assembly"></param>
        /// <returns></returns>
        public List<Type> GetAllModules(Assembly assembly)
        {
            var types = new List<Type>();

            foreach (var t in assembly.GetTypes()
                .Where(i => typeof(LindModule).IsAssignableFrom(i)
                    && !i.IsAbstract))
            {
                types.Add(t);
            }

            return types;
        }
    }
    #endregion

}
