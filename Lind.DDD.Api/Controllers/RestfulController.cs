using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Lind.DDD.Api.Models;
using Lind.DDD.DTO;
using Lind.DDD.Paging;
using System.Web;
using System.Diagnostics;
using System.Threading.Tasks;
using Lind.DDD.Authorization.Api;
using System.Web.Http.Cors;

namespace Lind.DDD.Api.Controllers
{
    /// <summary>
    /// 基于Restful标准的接口
    /// </summary>
    [EnableCors(origins: "http://localhost:34630", headers: "*", methods: "*")]
    public class RestfulController : ApiController
    {
        static List<TestApiModel> peopleList = new List<TestApiModel>();
        static RestfulController()
        {
            for (int i = 0; i < 10; i++)
            {
                peopleList.Add(new TestApiModel
                {
                    Id = i + 1,
                    Name = "张大叔" + i,
                    IsMale = true,
                    Info = "是一个人",
                    Birthday = DateTime.Now,
                    Deposit = 1000000,
                    Father = new TestApiModel
                    {
                        Name = "张老爸",
                        IsMale = true
                    },
                    Category = new Category
                    {
                        Title = "中国人",
                        Address = new Address
                        {
                            Province = "北京",
                            City = "房山",
                            District = "良乡",
                            OldAddress = new List<Address> { new Address { Province = "北平" }, new Address { Province = "大都" } }
                        }
                    },
                    OrderList = new List<OrderList> {
                        new OrderList {Price = 100, ProductName = "电视", Address = new Address { Province = "北京" } }, new OrderList { Price = 20, ProductName = "充值卡", Address = new Address { Province = "河南" } }, new OrderList { Price = 5, ProductName = "红牛" }, new OrderList { Price = 10, ProductName = "椰子汁", Address = new Address { Province = "湖南" } } }
                });
                peopleList.Add(new TestApiModel
                {
                    Id = i + 2,
                    Name = "李二叔" + i,
                    IsMale = true,
                    Info = "是一个小人",
                    Birthday = DateTime.Now,
                    Deposit = 500000,
                    Father = new TestApiModel
                    {
                        Name = "张二叔",
                        IsMale = true
                    },
                    Category = new Category
                    {
                        Title = "美国人",
                        Address = new Address
                        {
                            Province = "河南",
                            City = "开封",
                            District = "西红门",
                            OldAddress = new List<Address> { new Address { Province = "黄城根" }, new Address { Province = "汴梁" } }
                        }
                    },
                    OrderList = new List<OrderList> { new OrderList { Price = 1499, ProductName = "冰箱" }, new OrderList { Price = 10, ProductName = "雪糕" } }
                });
                peopleList.Add(new TestApiModel
                {
                    Id = i + 3,
                    Name = "王大妈" + i,
                    IsMale = false,
                    Info = "是一个女人",
                    Birthday = DateTime.Now,
                    Deposit = 30000,
                    Father = new TestApiModel
                    {
                        Name = "张三叔",
                        IsMale = true
                    },
                    Category = new Category
                    {
                        Title = "中国人",
                        Address = new Address
                        {
                            Province = "河北 ",
                            City = "唐山",
                            District = "滦县",
                            OldAddress = new List<Address> { new Address { Province = "天津－唐山" } }
                        }
                    },
                    OrderList = new List<OrderList> { new OrderList { Price = 9999, ProductName = "电脑" }, new OrderList { Price = 100, ProductName = "快餐" } }
                });
            }
        }

        /// <summary>
        /// 所有集合
        /// </summary>
        /// <returns></returns>
        public IHttpActionResult Get()

        {

            Contact[] contacts = new Contact[]

  {

                           new Contact { Name = "张三", PhoneNo = "123", EmailAddress = "zhangsan@gmail.com" },

               new Contact { Name = "李四", PhoneNo = "456", EmailAddress = "lisi@gmail.com" },

               new Contact { Name = "王五", PhoneNo = "789", EmailAddress = "wangwu@gmail.com" },

 };

            return Json<IEnumerable<Contact>>(contacts);

        }




        /// <summary>
        /// 通过ID拿对象
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ResponseMessage Get([FromUri]int id)
        {
            return new ResponseMessage
            {
                GuidKey = Guid.NewGuid().ToString(),
                Body = peopleList.FirstOrDefault(i => i.Id == id)
            };
        }
        /// <summary>
        /// 通过名字拿对象
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public TestApiModel Get([FromUri]string name)
        {
            return peopleList.FirstOrDefault(i => i.Name == name);
        }
        /// <summary>
        /// 通过性别返回一批数据
        /// </summary>
        /// <param name="isMale"></param>
        /// <returns></returns>
        public IEnumerable<TestApiModel> Get([FromUri]bool isMale)
        {
            return peopleList.Where(i => i.IsMale == isMale);
        }
        /// <summary>
        /// 添加对象
        /// </summary>
        /// <param name="value"></param>
        public HttpResponseMessage Post([FromBody]TestApiModel value)
        {
            try
            {
                //string result = Request.Content.ReadAsStringAsync().Result;
                //value.Id = peopleList.Max(i => i.Id) + 1;
                //peopleList.Add(value);
                return new HttpResponseMessage { Content = new StringContent("操作成功") };
            }
            catch (Exception)
            {

                throw;
            }
        }

        /// <summary>
        /// 更新对象
        /// </summary>
        /// <param name="id"></param>
        /// <param name="value"></param>
        public HttpResponseMessage Put(int id, [FromBody]TestApiModel value)
        {
            var old = peopleList.FirstOrDefault(i => i.Id == id);
            value.MapTo(old);
            return new HttpResponseMessage { Content = new StringContent("操作成功") };
        }
        /// <summary>
        /// 删除对象
        /// </summary>
        /// <param name="id"></param>
        public HttpResponseMessage Delete(int id)
        {
            peopleList.Remove(peopleList.FirstOrDefault(i => i.Id == id));
            return new HttpResponseMessage { Content = new StringContent("操作成功") };
        }
    }

    public class Contact

    {

        public string Name { get; set; }

        public string PhoneNo { get; set; }

        public string EmailAddress { get; set; }

    }
}