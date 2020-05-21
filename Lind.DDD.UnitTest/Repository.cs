using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Lind.DDD.Domain;
using Lind.DDD.IRepositories;

namespace Lind.DDD.UnitTest
{
    public partial class User : NoSqlEntity
    {
        public string UserName { get; set; }
        public int Age { get; set; }
        public string Address { get; set; }
    }
    [TestClass]
    public class Repository
    {
        IRepository<User> _repository = Lind.DDD.IoC.ServiceLocator.Instance.GetService<IRepository<User>>();

        [TestMethod]
        public void TestMethod1()
        {
            Assert.IsNotNull(_repository);
            for (var i = 0; i < 10; i++)
                _repository.Insert(new User
                {
                    UserName = "test" + i,
                    Age = 100,
                    Address = "beijing"
                });
            foreach (var item in _repository.GetModel())
            {
                Console.WriteLine("name:{0},age:{1},address:{2}", item.UserName, item.Age, item.Address);
            }


        }
    }
}
