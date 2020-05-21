using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Lind.DDD.Manager.Models;
using System.Web.Mvc.Html;
using System.Text;
using Lind.DDD.CatClientPur;
using Lind.DDD.Authorization;
using Lind.DDD.Manager.Filters;
using Lind.DDD.Filters;
using System.Linq.Expressions;

namespace Lind.DDD.Manager.Controllers
{


    public class WebUserController : BaseController
    {
        public WebUserController()
        {
            deptList = departmentsRepository.GetModel(i => i.Level == 1).ToList();
        }

        /// <summary>
        /// 获取用户信息通过ID
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [AllowAnonymous]
        public ActionResult GetUserInfo(int id = 0)
        {
            var entity = userRepository.Find(id);
            if (entity != null)
                return Json(entity, JsonRequestBehavior.AllowGet);
            else
                return Json(new WebManageUsers(), JsonRequestBehavior.AllowGet);
        }

        [ActionAuthority(Authority.Detail), ManagerActionLoggerAttribute("用户列表")]
        public ActionResult Index(DateTime? startDate, DateTime? endDate, int deptId = 0, int RoleId = 0, int page = 1, int pageSize = 10)
        {

            var linq = userRepository.GetModel()
                                     .Include(i => i.WebManageRoles)
                                     .Include(i => i.WebDepartments); ;


            if (deptId > 0)
                linq = linq.Where(i => i.WebDepartments.Where(j => j.Id == deptId).Count() > 0);

            if (RoleId > 0)
            {

                linq = linq.Where(i => i.WebManageRoles.Where(j => RoleId == j.Id).Count() > 0);
            }
            if (startDate.HasValue)
            {
                linq = linq.Where(i => i.DataCreateDateTime >= startDate.Value);
            }
            if (endDate.HasValue)
            {
                var end = endDate.Value.AddDays(1);
                linq = linq.Where(i => i.DataCreateDateTime < end);
            }
            ViewBag.TotalRecord = linq.Count();

            return View(linq.OrderBy(i => i.Id)
                            .ToPagedList(new Paging.PageParameters(page, pageSize))
                            .ToList());
        }

        public PartialViewResult Details(int id)
        {
            var model = userRepository.GetModel()
                                      .Include(i => i.WebDepartments)
                                      .Include(i => i.WebManageRoles)
                                      .FirstOrDefault(i => i.Id == id);
            //区域权限
            FillRoleAreaData(model.WebManageRoles.Select(i => i.Id).ToArray());
            //var a = 0;
            //var b = 1 / a;
            return PartialView(model);
        }


        public ActionResult Create()
        {
            return View();
        }

        [ManagerActionLoggerAttribute("添加用户")]
        [ActionAuthority(Authority.Create)]
        [HttpPost]
        public ActionResult Create(WebManageUsers entity, FormCollection form)
        {
            if (GetRole == null || GetDept == null)
            {
                ModelState.AddModelError("", "请把表单填写完整...");
                return View();
            }

            if (userRepository.Find(i => i.LoginName == entity.LoginName) != null)
            {
                ModelState.AddModelError("", "名称不能重复...");
                return View();
            }

            entity.Operator = "";
            entity.WebManageRoles = roleRepository.GetModel(i => GetRole.Contains(i.Id)).ToList();
            entity.WebDepartments = departmentsRepository.GetModel(i => GetDept.Contains(i.Id)).ToList();
            if (ModelState.IsValid)
            {
                entity.Password = Lind.DDD.Utils.Encryptor.Utility.EncryptString(entity.Password, Utils.Encryptor.Utility.EncryptorType.MD5);
                userRepository.Insert(entity);
                return RedirectToAction("Index");
            }
            else
            {
                ModelState.AddModelError("", "请把表单填写完整...");
                return View(entity);
            }

        }

        public ActionResult Edit(int id)
        {
            return View(userRepository.GetModel().Include(i => i.WebDepartments).Include(i => i.WebManageRoles).FirstOrDefault(i => i.Id == id));
        }
        [ManagerActionLoggerAttribute("编辑用户")]
        [ActionAuthority(Authority.Edit)]
        [HttpPost]
        public ActionResult Edit(int id, WebManageUsers entity, FormCollection form)
        {
            ModelState.Remove("Password");
            if (userRepository.Find(i => i.LoginName == entity.LoginName && i.Id != entity.Id) != null)
            {
                ModelState.AddModelError("", "名称不能重复...");
                return View(new WebManageUsers { Id = id });
            }

            var old = userRepository.GetModel()
                                   .Include(i => i.WebDepartments)
                                   .Include(i => i.WebManageRoles)
                                   .FirstOrDefault(i => i.Id == id);

            old.WebManageRoles = roleRepository.GetModel(i => GetRole.Contains(i.Id)).ToList();
            old.WebDepartments = departmentsRepository.GetModel(i => GetDept.Contains(i.Id)).ToList();
            old.LoginName = entity.LoginName;
            old.ThridUserId = entity.ThridUserId;
            old.RealName = entity.RealName;
            old.Mobile = entity.Mobile;
            old.Email = entity.Email;
            old.Description = entity.Description;
            if (ModelState.IsValid)
            {
                userRepository.Update(old);
                return RedirectToAction("Index");
            }
            else
            {
                ModelState.AddModelError("", "请把表单填写完整...");
                return View(new WebManageUsers { Id = id });
            }


        }


        private int[] GetDept
        {
            get
            {
                var deptList = Request.Form.GetValues("selDept");
                int[] intDept = Array.ConvertAll(deptList, i => Convert.ToInt32(i));
                return intDept;
            }
        }
        private int[] GetRole
        {
            get
            {
                var role = Request.Form.GetValues("selRole");
                List<string> roleList = new List<string>();
                if (role != null && role.Count() > 0)
                {
                    foreach (var r in role)
                    {
                        foreach (var sub in r.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
                        {
                            roleList.Add(sub);
                        }
                    }

                    int[] intRole = Array.ConvertAll(roleList.ToArray(), i => Convert.ToInt32(i));
                    return intRole;
                }
                return null;
            }
        }

        [ActionAuthority(Authority.Delete), ManagerActionLoggerAttribute("删除用户")]
        public ActionResult Delete(int id)
        {
            var old = userRepository.Find(id);
            old.DataStatus = Lind.DDD.Domain.Status.Deleted;
            userRepository.Update(old);
            return RedirectToAction("Index");
        }


        #region 部门与角色
        private IEnumerable<WebDepartments> deptList;
        /// <summary>
        /// 找儿子菜单
        /// </summary>
        /// <param name="menuList"></param>
        private void FindDeptSonsTree(IEnumerable<WebDepartments> deptList)
        {

            foreach (var item in deptList)
            {
                item.Sons = departmentsRepository.GetModel().Where(i => i.ParentID == item.Id).ToList();
                if (item.Sons != null && item.Sons.Count() > 0)//如果子树，加加载这块
                {
                    this.FindDeptSonsTree(item.Sons);
                }

            }

        }

        /// <summary>
        /// 构建部门下拉列表树
        /// </summary>
        /// <param name="html"></param>
        /// <returns></returns>
        public PartialViewResult DeptDropdownList(int? parentId, int userid = 0)
        {
            if (userid > 0)
            {
                var user = userRepository.GetModel()
                                         .Include(i => i.WebDepartments)
                                         .Include(i => i.WebManageRoles)
                                         .FirstOrDefault(i => i.Id == userid);

                ViewBag.DeptList = user.WebDepartments;
                ViewBag.RoleList = user.WebManageRoles;

            }
            StringBuilder sbr = new StringBuilder();
            var dept = departmentsRepository.Find(i => i.Level == 0);
            if (parentId.HasValue)
            {
                dept = departmentsRepository.Find(i => i.ParentID == parentId);
            }

            GetSelectList(dept, sbr);
            ViewBag.Dept = sbr.ToString();
            return PartialView();
        }


        public PartialViewResult RoleCheckbox(int? deptId, string selVal = "")
        {
            ViewBag.SelVal = selVal;
            return PartialView(roleRepository.GetModel(i => i.DepartmentID == deptId).ToList());
        }


        /// <summary>
        /// 冻结
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult Freeze(int id)
        {
            var old = userRepository.Find(id);
            old.DataStatus = Lind.DDD.Domain.Status.Freeze;
            old.LastModifyTime = DateTime.Now;
            userRepository.Update(old);
            return RedirectToAction("Index");
        }
        /// <summary>
        /// 解冻
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult NotFreeze(int id)
        {
            var old = userRepository.Find(id);
            old.DataStatus = Lind.DDD.Domain.Status.Normal;
            old.LastModifyTime = DateTime.Now;
            userRepository.Update(old);
            return RedirectToAction("Index");
        }
        #endregion

    }
}
