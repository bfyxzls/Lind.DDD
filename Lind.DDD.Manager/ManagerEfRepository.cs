using Lind.DDD.Domain;
using Lind.DDD.Repositories.EF;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lind.DDD.Manager
{
    public class ManagerEfRepository<T> : EFRepository<T> where T : class,IEntity
    {
        public ManagerEfRepository()
            : base(new ManagerContext())
        {

        }
    }
}
