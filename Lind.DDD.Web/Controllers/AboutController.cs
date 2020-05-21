using Lind.DDD.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Lind.DDD.Web.Controllers
{
    public class AboutInfo
    {
        public int Id { get; set; }
        public string Function { get; set; }
        public string Project { get; set; }
        public string Detail { get; set; }
        public AboutInfo(int id, string fun, string project, string detail)
        {
            this.Id = id;
            this.Function = fun;
            this.Project = project;
            this.Detail = detail;
        }
    }
    [AllowAnonymous]
    public class AboutController : Controller
    {

        public ActionResult Index()
        {
            IList<AboutInfo> list = new List<AboutInfo>();
            list.Add(new AboutInfo(1, "CodeFirst初始化", "Lind.DDD.Web项目", "LindDbInitializer类"));
            list.Add(new AboutInfo(2, "领域数据上下文定义", "Lind.DDD.Web项目", "LindDbContext"));
            list.Add(new AboutInfo(3, "领域仓储定义", "Lind.DDD.Web项目", "LindDbEfRepository"));
            list.Add(new AboutInfo(4, "实体模型设计", "Lind.DDD项目", "Lind.DDD.Domain"));
            list.Add(new AboutInfo(5, "标准EF的Curd", "Lind.DDD.Web项目", "Home/Index,Order/Index,Product/Index"));
            list.Add(new AboutInfo(6, "Mongodb的Curd", "Lind.DDD.Web项目", "Home/Create"));
            list.Add(new AboutInfo(7, "<b>Lind.DDD全局配置信息</b>", "Lind.DDD.Web项目", "Global.asax"));
            list.Add(new AboutInfo(8, "授权过滤器", "Lind.DDD.Web项目", "App_Start/FilterConfig"));
            list.Add(new AboutInfo(9, "当前登陆的用户", "Lind.DDD项目", "Lind.DDD.Authorization.CurrentUser"));
            list.Add(new AboutInfo(10, "分布式Session共享           ", "Lind.DDD项目     ", "StackExchange.Redis"));
            list.Add(new AboutInfo(11, "UoW工作单元                 ", "Lind.DDD.Web项目 ", "Home/Edit"));
            list.Add(new AboutInfo(12, "Logger日志                  ", "Lind.DDD.Web项目 ", "Order/Paid"));
            list.Add(new AboutInfo(13, "IoC贯穿整个项目             ", "Lind.DDD.Web项目 ", "Web.Config初始化,Home/Index"));
            list.Add(new AboutInfo(14, "Aop方法拦截                 ", "Lind.DDD.Test项目", "拦截对象AopUserBLL和拦截行为Logging,app.config进行ioc和aop的配置"));
            list.Add(new AboutInfo(15, "Redis队列的使用             ", "Lind.DDD.Web项目 ", "Order/Paid"));
            list.Add(new AboutInfo(16, "<b>Redis缓存队列</b>        ", "Lind.DDD项目     ", "Lind.DDD.RedisUtils.RedisQueueManager"));
            list.Add(new AboutInfo(17, "<b>Redis事务</b>                   ", "Lind.DDD项目     ", "Lind.DDD.RedisUtils.RedisTransactionManager"));
            list.Add(new AboutInfo(18, "<b>分布式数据集缓存使用</b>        ", "SOA项目          ", "SOA.API/api/values/1,SOA.Service进行特性注入,web.config进行cache配置"));
            list.Add(new AboutInfo(19, "实体自动验证机制            ", "Lind.DDD.Web项目 ", "Lind.DDD.Web.Models.Product                                                      "));
            list.Add(new AboutInfo(20, "事件总线机制                ", "Lind.DDD.Web项目 ", "Order/Paid,Order/Picked,Order/Dispatched,Order/Signed"));
            list.Add(new AboutInfo(21, "查询规约机制                ", "SOA项目          ", "SOA.Service.IUserService"));
            list.Add(new AboutInfo(22, "<b>消息机制(Email,SMS,RTX,XMPP)</b>", "Lind.DDD.Web项目 ", ""));
            list.Add(new AboutInfo(23, "大叔分页                    ", "Lind.DDD.Web项目 ", ""));
            list.Add(new AboutInfo(24, "任务调试Quartz              ", "Lind.DDD项目     ", "Lind.DDD.Quartz.Demo"));
            list.Add(new AboutInfo(25, "Lind.DDD文件上传            ", "Lind.DDD.Web项目 ", ""));
            list.Add(new AboutInfo(26, "<b>分布式文件存储FastDFS</b>       ", "Lind.DDD.Web项目 ", ""));
            list.Add(new AboutInfo(27, "WebApi&Dto,SOA              ", "SOA项目          ", ""));
            list.Add(new AboutInfo(28, "FastDFS", "Demo文件夹", "DemoFastSocket.WebClient,Lind.DDD.Redis.TestServer"));
            list.Add(new AboutInfo(29, "分布式Sub/Pub", "Lind.DDD.Test项目", "Lind.DDD.Test.Program.SubPub"));
            list.Add(new AboutInfo(30, "第三方在线支付", "Lind.DDD.Web", ""));
            list.Add(new AboutInfo(31, "数据缓存", "Lind.DDD项目", "Lind.DDD.Caching"));
            list.Add(new AboutInfo(32, "SOA请求与响应", "Lind.DDD项目", "Lind.DDD.SOA.RequestBase,Lind.DDD.SOA.ResponseBase,"));
            return View(list);
        }

        /// <summary>
        /// 技术
        /// </summary>
        /// <returns></returns>
        public ActionResult Function()
        {
            return View();
        }

        public ActionResult Error()
        {
            throw new LindException("框架问题");
        }

    }
}
