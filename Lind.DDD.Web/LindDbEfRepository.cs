using Lind.DDD.Domain;
using Lind.DDD.Repositories.EF;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lind.DDD.Web
{
    public class LindDbEfRepository<T> : EFRepository<T> where T : class,IEntity
    {
        public LindDbEfRepository()
            : base(new LindDbContext())
        {

        }
    }
}
