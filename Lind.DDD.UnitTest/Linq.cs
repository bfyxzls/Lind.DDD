using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using Lind.DDD.Specification;
using System.Linq.Expressions;
using System.Data.Linq;
namespace Lind.DDD.UnitTest
{

    [TestClass]
    public class Linq
    {

        static List<User> arr = new List<User>();
        static Linq()
        {
            arr.Add(new User { Id = "1", UserName = "zzl", Age = 11 });
            arr.Add(new User { Id = "1", UserName = "zzl456", Age = 100 });
            arr.Add(new User { Id = "2", UserName = "zzl1234", Age = 7 });
            arr.Add(new User { Id = "3", UserName = "lr", Age = 12 });
        }

        /// <summary>
        /// 条件动态构建(Specification模式)
        /// </summary>
        [TestMethod]
        public void Spec()
        {
            Specification.Specification<User> spec = new TrueSpecification<User>();
            spec = spec & new DirectSpecification<User>(i => i.Age > 10);
            spec = spec & new DirectSpecification<User>(i => i.UserName.Contains("zzl"));
            var linq = arr.AsQueryable().Where(spec.SatisfiedBy());
            foreach (var item in linq)
                Console.WriteLine(item.UserName);
        }
        /// <summary>
        /// 条件动态构建(表达式树)
        /// </summary>
        [TestMethod]
        public void Expression()
        {
            Expression<Func<User, bool>> param = i => true;
            param = ExpressionBuilder.And(param, i => i.UserName.Contains("zzl"));
            param = ExpressionBuilder.And(param, i => i.Age < 10);
            var linq = arr.AsQueryable().Where(param);
            foreach (var item in linq)
                Console.WriteLine(item.UserName);
        }

    }
}
