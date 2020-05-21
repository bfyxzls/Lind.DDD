using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;
using Lind.DDD.UoW;
using System.Collections.Generic;
namespace Lind.DDD.UnitTest
{
    [TestClass]
    public class UoW
    {
        [TestMethod]
        public void UoWTestMethod1()
        {
            IUnitOfWork uow = new UnitOfWork();
            IRepositories.IRepository<WebDataCtrls> repository = new Lind.DDD.Repositories.EF.EFRepository<WebDataCtrls>(new Test_Code_FirstEntities());
            var entity = new WebDataCtrls
            {
                Description = "test",
                DataCtrlType = "OK",
                DataCtrlName = "test",
                DataCtrlField = "Name",
                DataCtrlApi = "#",
                DataCreateDateTime = DateTime.Now,
            };
            var entity2 = new WebDataCtrls
            {
                Description = "test2",
                DataCtrlType = "OK",
                DataCtrlName = "test",
                DataCtrlField = "Name",
                DataCtrlApi = "",
                DataCreateDateTime = DateTime.Now,
            };

            //方法1
            uow.RegisterChangeded(entity2, SqlType.Insert, repository);
            //方法2带回调
            uow.RegisterChangeded(entity, SqlType.Insert, repository, (newEntity) =>
            {
                var old = repository.GetModel().FirstOrDefault(o => o.ID == entity.ID);
                old.DataCtrlName = "modify" + old.DataCtrlName;//刻意让它出错，整个将回滚
                repository.Update(old);
            });

            uow.Commit();

        }
    }
}
