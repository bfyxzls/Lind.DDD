using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq.Expressions;
using System.Collections.Generic;

namespace Lind.DDD.UnitTest
{

    /// <summary>
    /// 封装成员变量
    /// </summary>
    class Plan
    {
        /// <summary>
        ///  成员变量
        /// </summary>
        public string _Name;
        /// <summary>
        /// 属性，只读属性
        /// </summary>
        public string Name { get; private set; }
        /// <summary>
        /// 属性，只写属性
        ///  </summary>
        public string Name2 { private get; set; }

        public Plan(string name)
        {
            Name = name;
        }
    }

    /// <summary>
    /// 提取到方法
    /// </summary>
    class ExtractMethod
    {
        public void Test(int a, int b)
        {

            //显示人员名称
            //打印信息
            //计算数值（勾股定理）,逻辑比较多，算法复杂，应该提取到方法
            int c = a * a + b * b;
            double d = Math.Sqrt(c);
            //提取到方法后，方法可以复用
            GOUGU(a, b);
        }

        /// <summary>
        /// 方法的复用
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public double GOUGU(int a, int b)
        {
            int c = a * a + b * b;
            double d = Math.Sqrt(c);
            return d;
        }
    }

    /// <summary>
    /// 提取到类
    /// </summary>
    class ExtractClass
    {

        /// <summary>
        /// 动物,抽象类
        /// </summary>
        abstract class Animal
        {
            public string Name { get; set; }
            /// <summary>
            /// 虚方法，子类可以重写自己的逻辑
            /// </summary>
            public virtual void Eat()
            {
            }

            /// <summary>
            /// 抽象方法，子类必须要去实现它
            /// </summary>
            protected abstract void Create();

            /// <summary>
            /// 特殊的行为，应该在特殊的类里，不应该在基类
            /// </summary>
            public void WangWang()
            {
                Console.WriteLine("狗的叫声");
            }

        }

        /// <summary>
        /// 狗 
        /// </summary>
        class Bog : Animal
        {
            public void WangWang()
            {
                Console.WriteLine("狗的叫声");
            }

            protected override void Create()
            {
                Console.WriteLine("建立一只狗");
            }

            /// <summary>
            /// 返回骨头数
            /// </summary>
            /// <param name="count">狗的数量</param>
            /// <returns>骨头的数量</returns>
            public int GetGoutou(int count)
            {
                return 10;
            }

            /// <summary>
            /// 方法名称产生歧义，应该更名
            /// </summary>
            /// <returns></returns>
            public int Gets()
            {
                return 0;
            }

            public int GetCounts()
            {
                return 0;
            }
        }
        /// <summary>
        /// 猫
        /// </summary>
        class Cat : Animal
        {
            public override void Eat()
            {
                base.Eat();
            }
            protected override void Create()
            {
                Console.WriteLine("建立一只猫");

            }
            public int GetFishs()
            {
                return 5;
            }

            #region 参数化


            public Cat GetCat(int id)
            {
                return new Cat();
            }

            public Cat GetCat(string name)
            {
                return new Cat();
            }

            public Cat GetCat(Expression<Func<Cat, bool>> predicate)
            {
                return new Cat();
            }

            /// <summary>
            /// 代码坏味道－方法返回的结果可以被改写
            /// </summary>
            /// <param name="predicate"></param>
            /// <returns></returns>
            public List<Cat> GetCat1(Expression<Func<Cat, bool>> predicate)
            {
                return new List<Cat>();
            }


            /// <summary>
            /// 只读的方法，应该是只能的集合
            /// </summary>
            /// <param name="predicate"></param>
            /// <returns></returns>
            public IEnumerable<Cat> GetCat2(Expression<Func<Cat, bool>> predicate)
            {
                return new List<Cat>();
            }
            #endregion


        }

        /// <summary>
        /// 方法回调，委托参数化
        /// </summary>
        class weituo
        {

            void UIMethod()
            {
                //自己的UI代码

                //调用BLL
                BLLMethod((bll) =>
                {
                    //UI层进行代码处理
                    Console.WriteLine("从BLL层返回数据，在UI层进行处理" + bll);
                });
            }

            void BLLMethod(Action<string> action)
            {
                //处理业务逻辑，生产UI层需要的数据
                var a = "业务层向UI层进行数据传递";

                //回调UI层代码
                action(a);
            }

        }
    }

    [TestClass]
    public class CodeReview
    {
        [TestMethod]
        public void TestMethod1()
        {
            //封装成员变量
            //提取到时类
            //下移到类
            //提取方法

    
        }
    }
}
