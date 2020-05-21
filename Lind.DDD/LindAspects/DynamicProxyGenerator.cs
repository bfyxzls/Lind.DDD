using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.Reflection.Emit;
using System.Reflection;

namespace Lind.DDD.LindAspects
{
    /// <summary>
    /// 拦截所需要的动态代理
    /// </summary>
    public class DynamicProxyGenerator
    {
        const string AssemblyName = "Lind.DDD";
        const string AssemblyFileName = "Lind.DDD.dll";
        const string ModuleName = "Lind.DDD.Aspects.DynamicModule";
        const string TypeNameFormat = "Lind.DDD.Aspects.Dynamic{0}Type";

        private Type _realProxyType;
        private Type _interfaceType;

        private AssemblyBuilder _assemblyBuilder;
        private ModuleBuilder _moduleBuilder;
        private TypeBuilder _typeBuilder;

        private FieldBuilder _realProxyField;

        public DynamicProxyGenerator(Type realProxyType, Type interfaceType)
        {
            _realProxyType = realProxyType;
            _interfaceType = interfaceType;
        }

        public Type GenerateType()
        {
            // 构造程序集
            BuildAssembly();
            // 构造模块
            BuildModule();
            // 构造类型
            BuildType();
            // 构造字段
            BuildField();
            // 构造函数
            BuildConstructor();
            // 构造方法
            BuildMethods();

            Type type = _typeBuilder.CreateType();
            // 将新建的类型保存在硬盘上（如果每次都动态生成，此步骤可省略）
            //   _assemblyBuilder.Save(AssemblyFileName);
            return type;
        }

        #region Assembly & Module & Type

        void BuildAssembly()
        {
            // 程序集名字
            AssemblyName assemblyName = new AssemblyName(AssemblyName);

            // 在当前的AppDomain中构造程序集
            _assemblyBuilder = AppDomain.CurrentDomain.DefineDynamicAssembly(assemblyName,
                AssemblyBuilderAccess.RunAndSave, System.AppDomain.CurrentDomain.BaseDirectory);
        }

        void BuildModule()
        {
            // 
            _moduleBuilder = _assemblyBuilder.DefineDynamicModule(ModuleName, AssemblyFileName);
        }

        void BuildType()
        {
            _typeBuilder = _moduleBuilder.DefineType(string.Format(TypeNameFormat, _realProxyType.Name),
                TypeAttributes.Public | TypeAttributes.Sealed);
            _typeBuilder.AddInterfaceImplementation(_interfaceType);
        }

        #endregion

        #region Coustructor

        void BuildConstructor()
        {
            ConstructorBuilder constructorBuilder = _typeBuilder.DefineConstructor(MethodAttributes.Public,
                CallingConventions.HasThis, null);
            ILGenerator generator = constructorBuilder.GetILGenerator();

            // _realProxy = new RealProxy();
            generator.Emit(OpCodes.Ldarg_0);
            ConstructorInfo defaultConstructorInfo = _realProxyType.GetConstructor(Type.EmptyTypes);
            generator.Emit(OpCodes.Newobj, defaultConstructorInfo);
            generator.Emit(OpCodes.Stfld, _realProxyField);

            generator.Emit(OpCodes.Ret);
        }

        #endregion

        #region Field

        void BuildField()
        {
            _realProxyField = _typeBuilder.DefineField("_realProxy", _realProxyType, FieldAttributes.Private);
            _realProxyField.SetConstant(null);
        }

        #endregion

        #region Method

        void BuildMethods()
        {
            MethodInfo[] methodInfos = _realProxyType.GetMethods(BindingFlags.Public | BindingFlags.Instance);
            foreach (MethodInfo methodInfo in methodInfos)
            {
                BuildMethod(methodInfo);
            }
        }

        void BuildMethod(MethodInfo methodInfo)
        {
            string methodName = methodInfo.Name;
            string fullMethodName = methodInfo.ReflectedType.FullName + "." + methodInfo.Name;
            ParameterInfo[] parameterInfos = methodInfo.GetParameters();
            Type returnType = methodInfo.ReturnType;

            MethodBuilder methodBuilder = null;

            methodBuilder = _typeBuilder.DefineMethod(methodName,
                MethodAttributes.Public | MethodAttributes.Virtual,
                returnType, parameterInfos.Select(pi => pi.ParameterType).ToArray());

            var generator = methodBuilder.GetILGenerator();

            Label castPreSuccess = generator.DefineLabel();
            Label castPostSuccess = generator.DefineLabel();
            Label castExSuccess = generator.DefineLabel();

            #region Decalre InvokeContext

            Type contextType = typeof(InvokeContext);
            var contextLocal = generator.DeclareLocal(contextType);
            generator.Emit(OpCodes.Newobj, contextType.GetConstructor(Type.EmptyTypes));
            generator.Emit(OpCodes.Stloc, contextLocal);

            // set method name
            generator.Emit(OpCodes.Ldloc, contextLocal);
            generator.Emit(OpCodes.Ldstr, fullMethodName);
            generator.Emit(OpCodes.Call, contextType.GetMethod("SetMethod", BindingFlags.Public | BindingFlags.Instance));

            #endregion

            #region Decalre result

            LocalBuilder resultLocal = null;

            if (returnType != typeof(void))
            {
                resultLocal = generator.DeclareLocal(returnType);
                if (returnType.IsValueType)
                {
                    generator.Emit(OpCodes.Ldstr, returnType.FullName);
                    generator.Emit(OpCodes.Call, typeof(Type).GetMethod("GetType", new Type[] { typeof(string) }));
                    generator.Emit(OpCodes.Call, typeof(Activator).GetMethod("CreateInstance", new Type[] { typeof(Type) }));
                }
                else
                {
                    generator.Emit(OpCodes.Ldnull);
                }

                generator.Emit(OpCodes.Stloc, resultLocal);
            }

            #endregion

            #region Declare Exception

            var exceptionLocal = generator.DeclareLocal(typeof(Exception));
            generator.Emit(OpCodes.Ldnull);
            generator.Emit(OpCodes.Stloc, exceptionLocal);
            #endregion

            #region Invoke PreInvoke

            #region Set parameter to InvkeContext

            for (int i = 1; i <= parameterInfos.Length; i++)
            {
                generator.Emit(OpCodes.Ldloc, contextLocal);
                generator.Emit(OpCodes.Ldarg, i);
                if (parameterInfos[i - 1].ParameterType.IsValueType)
                {
                    generator.Emit(OpCodes.Box, parameterInfos[i - 1].ParameterType);
                }
                generator.Emit(OpCodes.Call, contextType.GetMethod("SetParameter", BindingFlags.Public | BindingFlags.Instance));
            }

            #endregion

            /*
             * C# 代码
             * MethodInfo methodInfoLocal = _realProxyField.GetType().GetMethod("methodName");
             * PreAspectAttribute preAspectLocal = 
             *      (PreAspectAttribute)Attribute.GetCustomAttribute(methodInfoLocal, typeof(PreAspectAttribute))
             *      
             * if (preAspectLocal != null)
             * {
             *      preAspectLocal.Action(contextLocal);
             * }
             * 
             */
            // get pre aspect if has
            var methodInfoLocal = generator.DeclareLocal(typeof(System.Reflection.MethodInfo));
            var preAspectLocal = generator.DeclareLocal(typeof(BeforeAspectAttribute));

            generator.Emit(OpCodes.Ldarg_0);
            generator.Emit(OpCodes.Ldfld, _realProxyField);
            generator.Emit(OpCodes.Callvirt, typeof(System.Object).GetMethod("GetType", BindingFlags.Public | BindingFlags.Instance));
            generator.Emit(OpCodes.Ldstr, methodName);

            generator.Emit(OpCodes.Callvirt,
                typeof(System.Type).GetMethod("GetMethod", new Type[] { typeof(string) }));
            generator.Emit(OpCodes.Stloc, methodInfoLocal);

            generator.Emit(OpCodes.Ldloc, methodInfoLocal);
            generator.Emit(OpCodes.Ldtoken, typeof(BeforeAspectAttribute));

            generator.Emit(OpCodes.Call,
                typeof(System.Type).GetMethod("GetTypeFromHandle", new Type[] { typeof(System.RuntimeTypeHandle) }));
            generator.Emit(OpCodes.Call,
                typeof(System.Attribute).GetMethod("GetCustomAttribute",
                new Type[] { typeof(System.Reflection.MethodInfo), typeof(System.Type) }));
            generator.Emit(OpCodes.Castclass, typeof(BeforeAspectAttribute));
            generator.Emit(OpCodes.Stloc, preAspectLocal);
            generator.Emit(OpCodes.Ldloc, preAspectLocal);
            generator.Emit(OpCodes.Ldnull);
            generator.Emit(OpCodes.Ceq);
            generator.Emit(OpCodes.Brtrue_S, castPreSuccess);

            if (typeof(void) == returnType)
            {
                #region ActionInvoke
                generator.Emit(OpCodes.Ldloc, preAspectLocal);
                generator.Emit(OpCodes.Ldloc, contextLocal);
                generator.Emit(OpCodes.Ldloc, methodInfoLocal);
                generator.Emit(OpCodes.Callvirt,
                   typeof(AspectAttribute).GetMethod("ActionInvoke", new Type[] { typeof(InvokeContext), typeof(MethodInfo) }));
                generator.MarkLabel(castPreSuccess);
                #endregion
            }
            else
            {
                #region FuncInvoke
                generator.Emit(OpCodes.Ldloc, preAspectLocal);
                generator.Emit(OpCodes.Ldloc, contextLocal);
                generator.Emit(OpCodes.Ldloc, methodInfoLocal);
                generator.Emit(OpCodes.Callvirt, typeof(AspectAttribute).GetMethod("FuncInvoke", new Type[] { typeof(InvokeContext), typeof(MethodInfo) }));
                LocalBuilder local = generator.DeclareLocal(typeof(object));
                generator.Emit(OpCodes.Stloc_0, local);
                generator.Emit(OpCodes.Ldloc_0, local);
                generator.Emit(OpCodes.Ret);
                generator.MarkLabel(castPreSuccess);
                #endregion


            }


            #endregion

            #region Begin Exception Block

            Label exLbl = generator.BeginExceptionBlock();

            #endregion

            #region Invoke

            generator.Emit(OpCodes.Ldarg_0);
            generator.Emit(OpCodes.Ldfld, _realProxyField);
            for (int i = 1; i <= parameterInfos.Length; i++)
            {
                generator.Emit(OpCodes.Ldarg, i);
            }

            generator.Emit(OpCodes.Call, _realProxyType.GetMethod(methodName, BindingFlags.Public | BindingFlags.Instance));
            if (typeof(void) != returnType)
            {
                generator.Emit(OpCodes.Stloc, resultLocal);
            }

            #endregion

            #region Invoke PostInovke

            #region Set result to InvkeContext

            generator.Emit(OpCodes.Ldloc, contextLocal);
            // load parameter
            if (typeof(void) != returnType)
            {
                generator.Emit(OpCodes.Ldloc, resultLocal);
                if (returnType.IsValueType)
                {
                    generator.Emit(OpCodes.Box, returnType);
                }
            }
            else
            {
                generator.Emit(OpCodes.Ldnull);
            }
            generator.Emit(OpCodes.Call, contextType.GetMethod("SetResult", BindingFlags.Public | BindingFlags.Instance));

            #endregion

            /*
             * C# 代码
             * MethodInfo methodInfoLocal = _realProxyField.GetType().GetMethod("methodName");
             * PostAspectAttribute postAspectLocal = 
             *      (PostAspectAttribute)Attribute.GetCustomAttribute(methodInfoLocal, typeof(PostAspectAttribute))
             *      
             * if (postAspectLocal != null)
             * {
             *      postAspectLocal.Action(contextLocal);
             * }
             * 
             */
            // get post aspect if has
            //var methodInfoLocal = generator.DeclareLocal(typeof(System.Reflection.MethodInfo));
            var postAspectLocal = generator.DeclareLocal(typeof(AfterAspectAttribute));

            /*
            generator.Emit(OpCodes.Ldarg_0);
            generator.Emit(OpCodes.Ldfld, _realProxyField);
            generator.Emit(OpCodes.Callvirt, typeof(System.Object).GetMethod("GetType", BindingFlags.Public | BindingFlags.Instance));
            generator.Emit(OpCodes.Ldstr, methodName);
            generator.Emit(OpCodes.Callvirt,
                typeof(System.Type).GetMethod("GetMethod", new Type[] { typeof(string) }));
            generator.Emit(OpCodes.Stloc, methodInfoLocal);
             */

            generator.Emit(OpCodes.Ldloc, methodInfoLocal);
            generator.Emit(OpCodes.Ldtoken, typeof(AfterAspectAttribute));

            generator.Emit(OpCodes.Call,
                typeof(System.Type).GetMethod("GetTypeFromHandle", new Type[] { typeof(System.RuntimeTypeHandle) }));
            generator.Emit(OpCodes.Call,
                typeof(System.Attribute).GetMethod("GetCustomAttribute",
                new Type[] { typeof(System.Reflection.MethodInfo), typeof(System.Type) }));
            generator.Emit(OpCodes.Castclass, typeof(AfterAspectAttribute));
            generator.Emit(OpCodes.Stloc, postAspectLocal);

            generator.Emit(OpCodes.Ldloc, postAspectLocal);
            generator.Emit(OpCodes.Ldnull);
            generator.Emit(OpCodes.Ceq);
            generator.Emit(OpCodes.Brtrue_S, castPostSuccess);


            if (typeof(void) == returnType)
            {
                #region ActionInvoke
                generator.Emit(OpCodes.Ldloc, preAspectLocal);
                generator.Emit(OpCodes.Ldloc, contextLocal);
                generator.Emit(OpCodes.Ldloc, methodInfoLocal);
                generator.Emit(OpCodes.Callvirt, typeof(AspectAttribute).GetMethod("ActionInvoke", new Type[] { typeof(InvokeContext), typeof(MethodInfo) }));
                generator.MarkLabel(castPostSuccess);
                #endregion
            }
            else
            {

                generator.Emit(OpCodes.Ldloc, postAspectLocal);
                generator.Emit(OpCodes.Ldloc, contextLocal);
                generator.Emit(OpCodes.Ldloc, methodInfoLocal);
                var method = typeof(AspectAttribute).GetMethod("FuncInvoke", new Type[] { typeof(InvokeContext), typeof(MethodInfo) });
                generator.Emit(OpCodes.Callvirt, method);
                var local = generator.DeclareLocal(typeof(object));
                generator.Emit(OpCodes.Stloc_0, local);
                generator.Emit(OpCodes.Ldloc_0, local);
                generator.Emit(OpCodes.Ret);
                generator.MarkLabel(castPostSuccess);

            }




            #endregion

            #region Catch Block

            generator.BeginCatchBlock(typeof(Exception));

            generator.Emit(OpCodes.Stloc, exceptionLocal);
            generator.Emit(OpCodes.Ldloc, contextLocal);
            generator.Emit(OpCodes.Ldloc, exceptionLocal);
            generator.Emit(OpCodes.Call, contextType.GetMethod("SetError", BindingFlags.Public | BindingFlags.Instance));

            /*
              * C# 代码
              * MethodInfo methodInfoLocal = _realProxyField.GetType().GetMethod("methodName");
              * ExceptionAspectAttribute exAspectLocal = 
              *      (ExceptionAspectAttribute)Attribute.GetCustomAttribute(methodInfoLocal, typeof(ExceptionAspectAttribute))
              *      
              * if (exAspectLocal != null)
              * {
              *      exAspectLocal.Action(contextLocal);
              * }
              * 
              */
            // get exception aspect if has
            //var methodInfoLocal = generator.DeclareLocal(typeof(System.Reflection.MethodInfo));
            var exAspectLocal = generator.DeclareLocal(typeof(AfterAspectAttribute));

            /*
            generator.Emit(OpCodes.Ldarg_0);
            generator.Emit(OpCodes.Ldfld, _realProxyField);
            generator.Emit(OpCodes.Callvirt, typeof(System.Object).GetMethod("GetType", BindingFlags.Public | BindingFlags.Instance));
            generator.Emit(OpCodes.Ldstr, methodName);
            generator.Emit(OpCodes.Callvirt,
                typeof(System.Type).GetMethod("GetMethod", new Type[] { typeof(string) }));
            generator.Emit(OpCodes.Stloc, methodInfoLocal);
             */
            generator.Emit(OpCodes.Ldloc, methodInfoLocal);
            generator.Emit(OpCodes.Ldtoken, typeof(ExceptionAspectAttribute));
            generator.Emit(OpCodes.Call,
               typeof(System.Type).GetMethod("GetTypeFromHandle", new Type[] { typeof(System.RuntimeTypeHandle) }));
            generator.Emit(OpCodes.Call,
                typeof(System.Attribute).GetMethod("GetCustomAttribute",
                new Type[] { typeof(System.Reflection.MethodInfo), typeof(System.Type) }));
            generator.Emit(OpCodes.Castclass, typeof(ExceptionAspectAttribute));
            generator.Emit(OpCodes.Stloc, exAspectLocal);

            generator.Emit(OpCodes.Ldloc, exAspectLocal);
            generator.Emit(OpCodes.Ldnull);
            generator.Emit(OpCodes.Ceq);
            generator.Emit(OpCodes.Brtrue_S, castExSuccess);

            if (typeof(void) == returnType)
            {
                #region ActionInvoke
                generator.Emit(OpCodes.Ldloc, preAspectLocal);
                generator.Emit(OpCodes.Ldloc, contextLocal);
                generator.Emit(OpCodes.Ldloc, methodInfoLocal);
                generator.Emit(OpCodes.Callvirt, typeof(AspectAttribute).GetMethod("ActionInvoke", new Type[] { typeof(InvokeContext), typeof(MethodInfo) }));
                generator.MarkLabel(castExSuccess);
                #endregion
            }
            else
            {

                generator.Emit(OpCodes.Ldloc, exAspectLocal);
                generator.Emit(OpCodes.Ldloc, contextLocal);
                generator.Emit(OpCodes.Ldloc, methodInfoLocal);

                generator.Emit(OpCodes.Callvirt, typeof(AspectAttribute).GetMethod("FuncInvoke", new Type[] { typeof(InvokeContext), typeof(MethodInfo) }));

                var local = generator.DeclareLocal(typeof(object));
                generator.Emit(OpCodes.Stloc_0, local);
                generator.Emit(OpCodes.Ldloc_0, local);
                generator.Emit(OpCodes.Ret);
                generator.MarkLabel(castExSuccess);
            }


            #endregion

            #region End Exception Block

            generator.EndExceptionBlock();

            #endregion

            if (typeof(void) != returnType)
            {
                generator.Emit(OpCodes.Ldloc, resultLocal);
            }


            generator.Emit(OpCodes.Ret);
        }
        #endregion
    }

}
