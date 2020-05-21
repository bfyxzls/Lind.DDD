using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lind.DDD.Api.Models
{
    /// <summary>
    /// 关于测试API的模型
    /// </summary>
    public class TestApiModel : Lind.DDD.DTO.ResponseBase
    {
        [DisplayName("父亲")]
        public TestApiModel Father { get; set; }
        [DisplayName("编号")]
        public int Id { get; set; }
        [DisplayName("姓名")]
        public string Name { get; set; }
        [DisplayName("说明")]
        public string Info { get; set; }
        [DisplayName("存款")]
        public decimal Deposit { get; set; }
        [DisplayName("性别")]
        public bool IsMale { get; set; }
        [DisplayName("生日")]
        public DateTime Birthday { get; set; }
        [DisplayName("类型")]
        public Category Category { get; set; }
        [DisplayName("订单列表")]
        public List<OrderList> OrderList { get; set; }
    }
    public class Simple
    {

        public string Name { get; set; }
        public bool IsMale { get; set; }
    }
    public class Category
    {
        public string Title { get; set; }
        public Address Address { get; set; }
    }

    public class Address
    {
        public string Province { get; set; }
        public string City { get; set; }
        public string District { get; set; }
        /// <summary>
        /// 以前的地址
        /// </summary>
        public List<Address> OldAddress { get; set; }
        public override string ToString()
        {
            return string.Format("{0}-{1}-{2}", Province, City, District);
        }
    }

    public class OrderList
    {
        public string ProductName { get; set; }
        public decimal Price { get; set; }
        public Address Address { get; set; }

    }
}
