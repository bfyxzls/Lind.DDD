using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Lind.DDD.Events;
namespace Lind.DDD.Events.Demo
{
    [Serializable]
    public class UserEvent : EventData
    {
        public int UserId { get; set; }
    }
}
