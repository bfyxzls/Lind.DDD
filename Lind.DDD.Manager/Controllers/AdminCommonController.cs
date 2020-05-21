using Lind.DDD.Authorization;
using Lind.DDD.Domain;
using Lind.DDD.Manager.Models;
using Lind.DDD.Manager.ViewModels;
using Lind.DDD.TreeHelper;
using Lind.DDD.Utils;
using PureCat;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Web.Mvc;
namespace Lind.DDD.Manager.Controllers
{
   
    /// <summary>
    /// 公用模块
    /// </summary>
    public class AdminCommonController : BaseController
    {
        private IEnumerable<WebManageMenus> menuList;

        private List<int> MenuIdArr = new List<int>();

        List<WebDepartments> DeptDB;
        public AdminCommonController()
        {
            if (CurrentUser.IsLogin)
                MenuIdArr = Lind.DDD.Utils.SerializeMemoryHelper.DeserializeFromJson<List<Tuple<int, string, int>>>(CurrentUser.ExtInfo).Select(i => i.Item1).ToList();
            menuList = menuRepository.GetModel(i => i.Level == 1 && MenuIdArr.Contains(i.Id)).ToList();

            DeptDB = departmentsRepository.GetModel().ToList();

        }
        public string Test()
        {
            return base.CommonAreaProvince();
        }
        public ActionResult ModifyPassword()
        {
            return View();
        }
        [HttpPost]
        public ActionResult ModifyPassword(ModifyPasswordModel model)
        {
            if (string.IsNullOrWhiteSpace(model.OldPassword)
            || string.IsNullOrWhiteSpace(model.NewPassword)
            || string.IsNullOrWhiteSpace(model.ConfirmPassword))
            {
                ModelState.AddModelError("", "请认真填写密码！");
                return View();
            }


            string oldPassword = Lind.DDD.Utils.Encryptor.Utility.EncryptString(model.OldPassword, Lind.DDD.Utils.Encryptor.Utility.EncryptorType.MD5);
            string newPassword = Lind.DDD.Utils.Encryptor.Utility.EncryptString(model.NewPassword, Lind.DDD.Utils.Encryptor.Utility.EncryptorType.MD5);

            var old = userRepository.Find(i => i.LoginName == CurrentUser.UserName && i.Password == oldPassword);
            if (old == null)
            {
                ModelState.AddModelError("", "原密码不正确！");
                return View();
            }

            if (model.NewPassword != model.ConfirmPassword)
            {
                ModelState.AddModelError("", "新密码和确认密码必须相同！");
                return View();
            }

            old.Password = newPassword;
            userRepository.Update(old);
            ViewBag.Msg = "<span style='color: green;font-size: 1.1em;font-weight:bold;'>密码成功修改，请重新<a href='/AdminCommon/Logon'>登陆</a></span>";
            return View();
        }


        #region 菜单相关
        public string TopMenu()
        {

            var menu = new DataTree<WebManageMenus>(menuRepository.GetModel(i => i.IsDisplayMenuTree && MenuIdArr.Contains(i.Id))
                                                                  .OrderBy(i => i.SortNumber)
                                                                  .ToList());

            return menu.CreateTreeUL(menuRepository.Find(i => i.Level == 0));
        }

        /// <summary>
        /// 找儿子菜单
        /// </summary>
        /// <param name="menuList"></param>
        private void FindMenuSonsTree(IEnumerable<WebManageMenus> menuList)
        {

            foreach (var item in menuList)
            {
                item.Sons = menuRepository.GetModel()
                    .Where(i => i.IsDisplayMenuTree && i.ParentID == item.Id && MenuIdArr.Contains(i.Id))
                    .ToList();
                if (item.Sons != null && item.Sons.Count() > 0)//如果子树，加加载这块
                {
                    this.FindMenuSonsTree(item.Sons);
                }

            }

        }
        #endregion

        #region 部门&角色相关
        /// <summary>
        /// 找儿子部门
        /// </summary>
        /// <param name="menuList"></param>
        private void FindDeptSonsTree(IEnumerable<WebDepartments> deptList, List<int> deptArr)
        {

            foreach (var item in deptList)
            {
                deptArr.Add(item.Id);
                item.Sons = DeptDB
                    .Where(i => i.ParentID == item.Id)
                    .ToList();
                if (item.Sons != null && item.Sons.Count() > 0)//如果子树，加加载这块
                {
                    this.FindDeptSonsTree(item.Sons, deptArr);
                }

            }

        }

        private void FindDeptFatherTree(WebDepartments dept, List<int> fatherList)
        {
            if (dept != null)
            {
                dept.Father = DeptDB.FirstOrDefault(i => i.Id == dept.ParentID);
                fatherList.Add(dept.ParentID ?? 0);
                FindDeptFatherTree(dept.Father, fatherList);
            }

        }
        /// <summary>
        /// 部门－下拉列表
        /// </summary>
        /// <param name="deptId"></param>
        /// <returns></returns>
        public PartialViewResult Dept_SelectList(int deptId = 0)
        {
            return Dept_Role_SelectList(deptId, 0);
        }
        /// <summary>
        /// 部门－角色－下拉列表
        /// </summary>
        /// <param name="dataSetTypeId"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        public PartialViewResult Dept_Role_SelectList(int deptId = 0, int roleId = 0)
        {
            var deptList = new List<SelectListItem>();
            deptList = new List<SelectListItem>(departmentsRepository.GetModel().Select(i => new SelectListItem
           {
               Text = i.Name,
               Value = i.Id.ToString(),
               Selected = i.Id == deptId ? true : false
           }));
            deptList.Insert(0, new SelectListItem { Text = "请选择", Value = "0", Selected = true });

            var roleList = new List<SelectListItem>();

            if (deptId > 0)
            {
                roleList = new List<SelectListItem>(roleRepository.GetModel(i => i.DepartmentID == deptId).Select(i => new SelectListItem
                {
                    Text = i.RoleName,
                    Value = i.Id.ToString(),
                    Selected = i.Id == roleId ? true : false
                }));
            }
            roleList.Insert(0, new SelectListItem { Text = "请选择", Value = "0", Selected = true });
            ViewBag.roleList = roleList;
            ViewBag.deptList = deptList;

            StringBuilder str = new StringBuilder();
            List<int> fatherList = new List<int>();
            var father = new WebDepartments();
            if (deptId > 0)
            {
                father = departmentsRepository.GetModel().FirstOrDefault(i => i.Id == deptId);
                fatherList.Add(deptId);

            }
            else
            {
                father = departmentsRepository.GetModel().FirstOrDefault(i => i.Level == 1);
                father.Sons = null;//默认“请选择”状态它的子列表为空
            }
            FindDeptFatherTree(father, fatherList);
            GetSelectListAll(father, str, fatherList);
            ViewBag.DeptStr = str.ToString();
            return PartialView();
        }



        #endregion

        [AllowAnonymous, HttpPost, ValidateInput(false)]
        public ActionResult Error(string msgErr = "请求发生错误")
        {
            ViewBag.Msg = msgErr;
            return View();
        }

        #region 登录—登出
        [AllowAnonymous]
        public ActionResult LogOn()
        {
            return View();
        }
        [AllowAnonymous]
        [HttpPost]
        public ActionResult LogOn(FormCollection form)
        {
            string userName = form["UserName"];
            string password = Lind.DDD.Utils.Encryptor.Utility.EncryptString(form["Password"], Lind.DDD.Utils.Encryptor.Utility.EncryptorType.MD5);

            try
            {
                var entity = userRepository.GetModel()
                                           .Include(i => i.WebManageRoles)
                                           .Include(i => i.WebDepartments)
                                           .FirstOrDefault(i => i.DataStatus == Status.Normal && i.LoginName == userName && i.Password == password);
                if (entity != null)
                {


                    #region 应用角色和菜单
                    var roleArr = entity.WebManageRoles.Select(j => j.Id).ToList();
                    var authority = 0;
                    var menuList = new List<Tuple<int, string, long>>();
                    var result = new List<Tuple<int, string, long>>();
                    if (entity.WebManageRoles != null && entity.WebManageRoles.Any())
                    {
                        //当前用户所有角色对应的权限列表
                        var auditList = webManageRoles_WebManageMenus_Authority_RRepository.GetModel()
                            .Include(i => i.WebManageRoles)
                            .Include(i => i.WebManageMenus)
                            .Where(i => roleArr.Contains(i.RoleId))
                            .ToList();

                        auditList.ForEach(i =>
                        {
                            menuList.Add(new Tuple<int, string, long>(i.MenuId, i.WebManageMenus.LinkUrl, i.Authority));
                        });

                        var linq = from m in menuList
                                   group m by new { m.Item1, m.Item2 } into p
                                   select new { ID = p.Key, score = p.BinaryOr(m => m.Item3) };

                        result = linq.Select(i => new Tuple<int, string, long>(i.ID.Item1, i.ID.Item2, i.score)).ToList();
                    }
                    #endregion

                    #region 部门（组织结构）
                    var deptArr = new List<int>();
                    if (entity.WebDepartments != null && entity.WebDepartments.Count > 0)
                    {
                        FindDeptSonsTree(entity.WebDepartments, deptArr);
                    }
                    #endregion

                    CurrentUser.Serialize(
                        entity.Id.ToString(),
                        entity.LoginName,
                        authority: authority,
                        department: string.Join(",", deptArr),
                        extInfo: Lind.DDD.Utils.SerializeMemoryHelper.SerializeToJson(result));

                    if (!string.IsNullOrWhiteSpace(Request["returnUrl"]))
                    {
                        return Redirect(Request["returnUrl"]);
                    }
                    else
                    {
                        return Redirect("/");//回到站点的主页
                    }
                }
                else
                {
                    ModelState.AddModelError("", "用户名密码错误");
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "系统发生未知错误" + ex.Message);
            }
            return View();
        }
        public ActionResult LogOut()
        {
            CurrentUser.Exit();
            return RedirectToAction("LogOn", "AdminCommon");
        }
        #endregion
    }


}
