using Lind.DDD.Manager.Models;
using Lind.DDD.TreeHelper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Lind.DDD.Manager
{
    public class AreaTree : DataTree<WebCommonAreas>
    {
        public AreaTree(Expression<Func<WebCommonAreas, bool>> predicate)
            : base(new ManagerEfRepository<WebCommonAreas>().GetModel(predicate)
                                                            .OrderBy(i => i.ID)
                                                            .ToList())
        {

        }
        public AreaTree()
            : this(i => true)
        {

        }
    }
}
