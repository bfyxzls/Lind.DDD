using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lind.DDD.CQRS
{

    /// <summary>
    /// 命令，它应该是在应用层的，它可以由事件处理程序去实现
    /// </summary>
    public abstract class Command
    {
        Guid aggregateId;
        public Command()
            : this(Guid.NewGuid())
        {

        }
        public Command(Guid aggregateId)
        {
            this.aggregateId = aggregateId;
        }
    }
}
