using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Lind.DDD.Manager;
using Lind.DDD.Manager.Models;
using Lind.DDD.TreeHelper;
using System.Linq.Expressions;


namespace Lind.DDD.Manager
{
    public class MenuTree : DataTree<WebManageMenus>
    {
        public MenuTree(Expression<Func<WebManageMenus, bool>> predicate)
            : base(new ManagerEfRepository<WebManageMenus>().GetModel(predicate)
                                                            .OrderBy(i => i.SortNumber)
                                                            .ToList())
        {

        }
        public MenuTree()
            : this(i => true)
        {

        }
    }
}