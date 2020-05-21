using Lind.DDD.Authorization;
using Lind.DDD.UoW;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lind.DDD.Web.Controllers
{
    /// <summary>
    /// Lind.DDD项目的controller基类
    /// </summary>
    public abstract class BaseController : System.Web.Mvc.Controller
    {
        /// <summary>
        /// 工作单元
        /// </summary>
        protected IUnitOfWork unitOfWork;
        
        /// <summary>
        /// 数据上下文
        /// </summary>
        protected DbContext db;

        /// <summary>
        /// 初始化
        /// </summary>
        public BaseController()
        {
            db = new LindDbContext();
            unitOfWork = new UnitOfWork();
        }

        /// <summary>
        /// 当前登陆的用户ID
        /// </summary>
        public int UserId
        {
            get
            {
                return Convert.ToInt32(string.IsNullOrWhiteSpace(CurrentUser.UserID) ? "0" : CurrentUser.UserID);
            }
        }

        /// <summary>
        /// 分页参数
        /// </summary>
        public int PageSize = 10;
    }
}
