using Lind.DDD.Domain;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lind.DDD.Web.Models
{
    public class UserExtension : Entity
    {
        [DisplayName("昵称"), Required, MaxLength(50)]
        public string NickName { get; set; }
        [DisplayName("学校"), MaxLength(255)]
        public string School { get; set; }
        public UserInfo UserInfo { get; set; }
    }
}
