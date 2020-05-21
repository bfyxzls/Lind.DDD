using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lind.DDD.CQRS
{
    /// <summary>
    /// 命令处理程序
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface ICommandHandler<T>
        : Lind.DDD.LindPlugins.IPlugins where T : Command
    {
        /// <summary>
        /// 执行具体的命令
        /// </summary>
        /// <param name="command"></param>
        void Execute(T command);
    }
}
