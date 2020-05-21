using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lind.DDD.Web.Models
{
    public class Category : Lind.DDD.Domain.Entity
    {
        [DisplayName("分类名称"), Required]
        public string Name { get; set; }
        [DisplayName("描述")]
        public string Description { get; set; }
        [DisplayName("父ID")]
        public int ParentId { get; set; }
        [DisplayName("级别")]
        public int Level { get; set; }
        public IList<Product> Product { get; set; }
    }
}
