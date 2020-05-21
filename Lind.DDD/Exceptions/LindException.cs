using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lind.DDD.Exceptions
{
    /// <summary>
    /// Lind框架的异常类
    /// </summary>
    public class LindException : Exception
    {
        public LindException(string message)
            : base("LindException:" + message)
        { }

    }
}
