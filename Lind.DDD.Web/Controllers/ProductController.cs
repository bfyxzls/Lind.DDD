using Lind.DDD.IoC;
using Lind.DDD.IRepositories;
using Lind.DDD.Web.Filters;
using Lind.DDD.Web.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
namespace Lind.DDD.Web.Controllers
{
    /// <summary>
    /// 产品控制器
    /// </summary>
    [ManagerFilter]
    public class ProductController : BaseController
    {
        IExtensionRepository<Product> productRepository;
        IExtensionRepository<Category> categoryRepository;
        List<SelectListItem> cateList;

        public ProductController()
        {
            productRepository = ServiceLocator.Instance.GetService<IExtensionRepository<Product>>();
            categoryRepository = ServiceLocator.Instance.GetService<IExtensionRepository<Category>>();
            cateList = new List<SelectListItem>();
            foreach (var i in categoryRepository
                .GetModel()
                .ToDictionary(i => i.Id.ToString(), j => j.Name))
            {
                cateList.Add(new SelectListItem { Text = i.Value, Value = i.Key });
            }
        }

        /// <summary>
        /// 产品列表
        /// </summary>
        /// <param name="keyword"></param>
        /// <param name="page"></param>
        /// <returns></returns>
        public ActionResult Index(string keyword, int page = 1)
        {
            ViewBag.keyword = keyword;
            var model = productRepository.GetModel().Include(i => i.Category);
            if (Request.HttpMethod == "POST")
                if (!string.IsNullOrWhiteSpace(keyword))
                    model = model.Where(i => i.Name.Contains(keyword.Trim()));

            return View(model.OrderBy(i => i.Id).ToPagedList(page, PageSize));
        }
        /// <summary>
        /// 产品详细
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult Details(int id)
        {
            return View(productRepository.Find(id));
        }

        public ActionResult Edit(int id)
        {
            var entity = productRepository.Find(id);
            cateList.Where(i => i.Value == entity.CategoryId.ToString()).First().Selected = true;
            ViewBag.category = cateList;
            return View(entity);
        }
        [HttpPost]
        public ActionResult Edit(int id, Product entity)
        {
            productRepository.Update(entity);
            return RedirectToAction("Index");
        }
        public ActionResult Create()
        {
            ViewBag.category = cateList;
            return View(new Product
            {
                UserInfoId = UserId,
                Discount = 100
            });
        }
        [HttpPost]
        public ActionResult Create(Product entity)
        {
            entity.UserInfoId = UserId;
            entity.UserInfoUserName = Lind.DDD.Authorization.CurrentUser.UserName;
            productRepository.Insert(entity);
            return RedirectToAction("Index");
        }
    }
}
