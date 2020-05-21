using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Reflection;
using System.Reflection.Emit;

namespace Lind.DDD.UnitTest
{
    [TestClass]
    public class Emit
    {
        [TestMethod]
        public void BulidMethod()
        {
            //得到当前的应用程序域
            AppDomain appDm = AppDomain.CurrentDomain;
            //初始化AssemblyName的一个实例
            AssemblyName an = new AssemblyName();
            //设置程序集的名称
            an.Name = "EmitLind";
            //动态的在当前应用程序域创建一个应用程序集
            AssemblyBuilder ab = appDm.DefineDynamicAssembly(an, AssemblyBuilderAccess.Run);
            //动态在程序集内创建一个模块
            ModuleBuilder mb = ab.DefineDynamicModule("EmitLind");
            //动态的在模块内创建一个类
            TypeBuilder tb = mb.DefineType("HelloEmit", TypeAttributes.Public | TypeAttributes.Class);
            //动态的为类里创建一个方法
            MethodBuilder mdb = tb.DefineMethod("HelloWord", MethodAttributes.Public, null, new Type[] { typeof(string) });

            //得到该方法的ILGenerator
            ILGenerator ilG = mdb.GetILGenerator();
            ilG.Emit(OpCodes.Ldstr, "Hello：{0}");
            //加载传入方法的参数到堆栈
            ilG.Emit(OpCodes.Ldarg_1);
            //调用Console.WriteLine方法，输出传入的字符
            ilG.Emit(OpCodes.Call, typeof(Console).GetMethod("WriteLine", new Type[] { typeof(string), typeof(string) }));

            ilG.Emit(OpCodes.Ret);
            //创建类的Type对象
            Type tp = tb.CreateType();
            //实例化一个类
            object ob = Activator.CreateInstance(tp);
            //得到类中的方法，通过Invoke来触发方法的调用..
            MethodInfo mdi = tp.GetMethod("HelloWord");
            mdi.Invoke(ob, new object[] { "Hello Lind" });
        }

        [TestMethod]
        public void BulidMethodRet()
        {

            //得到当前的应用程序域
            AppDomain appDm = AppDomain.CurrentDomain;
            //初始化AssemblyName的一个实例
            AssemblyName an = new AssemblyName();
            //设置程序集的名称
            an.Name = "EmitLind";
            //动态的在当前应用程序域创建一个应用程序集
            AssemblyBuilder ab = appDm.DefineDynamicAssembly(an, AssemblyBuilderAccess.Run);
            //动态在程序集内创建一个模块
            ModuleBuilder mb = ab.DefineDynamicModule("EmitLind");
            //动态的在模块内创建一个类
            TypeBuilder tb = mb.DefineType("HelloEmit", TypeAttributes.Public | TypeAttributes.Class);
          
            //动态的为类里创建一个方法
            MethodBuilder mdb = tb.DefineMethod(
                "HelloWorldReturn",
                MethodAttributes.Public,
                typeof(string),
                new Type[] { typeof(string), typeof(string) });

            //得到该方法的ILGenerator
            ILGenerator ilG = mdb.GetILGenerator();
            ilG.Emit(OpCodes.Ldstr, "你好：{0}-{1}");
            //加载传入方法的参数到堆栈
            ilG.Emit(OpCodes.Ldarg_1); 
            ilG.Emit(OpCodes.Ldarg_2);
            //调用Console.WriteLine方法，输出传入的字符
            ilG.Emit(OpCodes.Call, typeof(Console).GetMethod("WriteLine", new Type[] { typeof(string), typeof(string) ,typeof(string)}));

            // ilG.Emit(OpCodes.Pop);//加这个就有问题了
            //返回值部分
            LocalBuilder local = ilG.DeclareLocal(typeof(string));
            ilG.Emit(OpCodes.Ldstr, "Return Value：{0}");
            ilG.Emit(OpCodes.Ldarg_1);
            ilG.Emit(OpCodes.Call, typeof(string).GetMethod("Format", new Type[] { typeof(string), typeof(string) }));
            ilG.Emit(OpCodes.Stloc_0, local);
            ilG.Emit(OpCodes.Ldloc_0, local);
            ilG.Emit(OpCodes.Ret);
            //创建类的Type对象
            Type tp = tb.CreateType();
            //实例化一个类
            object ob = Activator.CreateInstance(tp);
            //得到类中的方法，通过Invoke来触发方法的调用..
            MethodInfo mdi = tp.GetMethod("HelloWorldReturn");
            mdi.Invoke(ob, new object[] { "Hello Lind","OK" });
        }
    }
}
