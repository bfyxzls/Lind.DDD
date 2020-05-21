using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Text;
using Lind.DDD.Manager.Models;
using Lind.DDD.Manager;
using Lind.DDD.TreeHelper;

namespace Lind.DDD.Manager
{
    public class DeptTree : DataTree<WebDepartments>
    {
        public DeptTree()
            : base(new ManagerEfRepository<WebDepartments>().GetModel()
                                                            .OrderBy(i => i.SortNumber)
                                                            .ToList())
        {
        }
    }
}