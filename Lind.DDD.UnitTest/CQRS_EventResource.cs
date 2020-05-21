using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Lind.DDD.CQRS;
using System.Data.Entity;
using Lind.DDD.IRepositories;
using Lind.DDD.Events;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Lind.DDD.UnitTest
{
    #region 领域事件
    /// <summary>
    /// 用户建立事件
    /// </summary>
    public class CreateUserEvent : EventData
    {
        public string UserName { get; set; }
        public string Email { get; set; }
    }
    /// <summary>
    /// 用户修改名字事件
    /// </summary>
    public class RenameUserEvent : EventData
    {
        public int UserID { get; set; }
        public string UserName { get; set; }

    }
    public class EmailUserEvent : EventData
    {
        public int UserID { get; set; }
        public string Email { get; set; }

    }

    #endregion

    #region 领域处理程序
    /// <summary>
    /// 领域处理程序
    /// 领域层
    /// </summary>
    /// <typeparam name="T"></typeparam>
    [Serializable]
    public class UserEventHandler :
       IEventHandler<CreateUserEvent>,
       IEventHandler<RenameUserEvent>,
        IEventHandler<EmailUserEvent>
    {

        #region IEventHandler<CreateOrderCommand> 成员

        public void Handle(CreateUserEvent evt)
        {
            TestCFContext db = new TestCFContext();
            var entity = new TestUser
            {
                Email = evt.Email,
                UserName = evt.UserName,
            };
            db.Entry(entity);
            db.Set<TestUser>().Add(entity);
            db.SaveChanges();
        }

        #endregion

        #region IEventHandler<FinishedOrderCommand> 成员

        public void Handle(RenameUserEvent evt)
        {
            TestCFContext db = new TestCFContext();
            var entity = db.Set<TestUser>().Find(evt.UserID);
            entity.UserName = evt.UserName;
            db.SaveChanges();
        }

        #endregion

        #region IEventHandler<EmailUserEvent> 成员

        public void Handle(EmailUserEvent evt)
        {
            TestCFContext db = new TestCFContext();
            var entity = db.Set<TestUser>().Find(evt.UserID);
            entity.Email = evt.Email;
            db.SaveChanges();
        }

        #endregion
    }

    #endregion

    /// <summary>
    /// 数据模型
    /// </summary>
    public partial class TestUser
    {
        public void CreateUser(CreateUserEvent @event)
        {
            EventBus.Instance.Publish<CreateUserEvent>(@event);
        }

        public void ChangeUserName(string userName)
        {
            EventBus.Instance.Publish(new RenameUserEvent { UserID = this.Id, UserName = userName });
        }

        public void ChangeEmail(string email)
        {
            EventBus.Instance.Publish(new EmailUserEvent { UserID = this.Id, Email = email });
        }
    }

    #region 命令
    public class AddUserCommand : Command
    {
        public string UserName { get; set; }
        public string Email { get; set; }
    }

    public class EditUserCommand : Command
    {
        public int UserID { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
    }

    public class DelUserCommand : Command
    {
        public int UserID { get; set; }
    }
    #endregion

    #region 命令处理程序
    /// <summary>
    /// 命令处理程序
    /// 应用层
    /// </summary>
    [Serializable]
    public class UserCommandHandler :
        ICommandHandler<AddUserCommand>,
        ICommandHandler<EditUserCommand>,
        ICommandHandler<DelUserCommand>
    {
        #region ICommandHandler<DelUserCommand> 成员

        public void Execute(DelUserCommand command)
        {
            Console.WriteLine(DateTime.Now + "删除" + command.UserID);
        }

        #endregion

        #region ICommandHandler<EditUserCommand> 成员

        public void Execute(EditUserCommand command)
        {
            TestCFContext db = new TestCFContext();
            var aggregate = db.Set<TestUser>().Find(command.UserID);
            if (!string.IsNullOrWhiteSpace(command.UserName))
                aggregate.ChangeUserName(command.UserName);
            if (!string.IsNullOrWhiteSpace(command.Email))
                aggregate.ChangeEmail(command.Email);
            Console.WriteLine(DateTime.Now + "编辑" + command.UserName);
        }

        #endregion

        #region ICommandHandler<AddUserCommand> 成员

        public void Execute(AddUserCommand command)
        {
            Console.WriteLine(DateTime.Now + "添加" + command.UserName);
        }

        #endregion
    }
    #endregion

    [TestClass]
    public class CQRS_EventResource
    {
        [TestMethod]
        public void AddUser()
        {
            EventBus.Instance.SubscribeAll();


            CommandBus.Send(new EditUserCommand { UserName = "zzlOK", Email = "Test", UserID = 1 });

        }

        [TestMethod]
        public void UserEvt()
        {
            CommandBus.RegisterAll();
            // 发布添加用户命令
            CommandBus.Send(new AddUserCommand { UserName = "zzl", Email = "zzl@sina.com" });
            // 发布编辑用户命令
            CommandBus.Send(new EditUserCommand { UserName = "zzl", UserID = 1 });
            // 发布刪除用户命令
            CommandBus.Send(new DelUserCommand { UserID = 1 });


        }

    }


}
