using Lind.DDD.Authorization;
using Lind.DDD.Manager.Filters;
using Lind.DDD.Manager.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Web;
using System.Web.Mvc;
using Lind.DDD.Filters;

namespace Lind.DDD.Manager.Controllers
{
    public class WebDataSettingController : BaseController
    {
        [ActionAuthority(Authority.Detail), ManagerActionLoggerAttribute("数据集列表")]
        public ActionResult Index(int page = 1)
        {
            var model = webDataSettingRepository.GetModel()
                                                .Include(i => i.WebManageRoles)
                                                .Include(i => i.WebDataCtrl)
                                                .OrderByDescending(i => i.Id)
                                                .ToPagedList(new Paging.PageParameters(page, 10));
            ViewBag.totalRecord = model.Count();
            return View(model);
        }

        public PartialViewResult Details(int id)
        {
            var model = webDataSettingRepository.GetModel()
                                               .Include(i => i.WebManageRoles)
                                               .Include(i => i.WebDataCtrl)
                                               .FirstOrDefault(i => i.Id == id);
            return PartialView(model);
        }

        public ActionResult Create()
        {
            DoDeptDropdownList(null);
            return View();
        }
        public void DoDeptDropdownList(int? parentId)
        {
            if (Current_UserId > 0)
            {
                var user = userRepository.GetModel()
                    .Include(i => i.WebDepartments)
                    .Include(i => i.WebManageRoles)
                    .FirstOrDefault(i => i.Id == Current_UserId);
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
        }

        /// <summary>
        /// 数据集类型－数值
        /// </summary>
        /// <param name="dataSetTypeId"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        public PartialViewResult Dept_DataSet(int? dataSetTypeId, int? id)
        {
            var sl = new List<SelectListItem>();
            var selList = new List<string>();
            sl = new List<SelectListItem>(webDataCtrlRepository.GetModel().Select(i => new SelectListItem
            {
                Text = i.DataCtrlName + "->" + i.DataCtrlField,
                Value = i.Id.ToString(),
                Selected = i.Id == dataSetTypeId ? true : false
            }));
            sl.Insert(0, new SelectListItem { Text = "请选择", Value = "" });
            ViewBag.sl = sl;

            var chkList = new List<SelectListItem>();
            if (dataSetTypeId.HasValue)
            {
                if (id.HasValue)
                {
                    var entity = webDataSettingRepository.Find(i => i.Id == id);
                    selList = entity.ObjectIdArr.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries).ToList();
                }
                var type = webDataCtrlRepository.Find(dataSetTypeId.Value);
                if (Uri.IsWellFormedUriString(type.DataCtrlApi, UriKind.Absolute))
                {
                    try
                    {
                        var response = Lind.DDD.Utils.HttpHelper.Get(type.DataCtrlApi);
                        chkList = Lind.DDD.Utils.SerializeMemoryHelper.DeserializeFromJson<List<SelectListItem>>(response.Content.ReadAsStringAsync().Result);

                    }
                    catch (Exception)
                    {
                        //数据获取时出现问题
                    }
                }
            }
            ViewBag.chkList = chkList;
            ViewBag.selList = selList;
            return PartialView();
        }

        [ActionAuthority(Authority.Create), ManagerActionLoggerAttribute("添加数据集")]
        [HttpPost]
        public ActionResult Create(WebDataSetting entity)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var roleArr = Request.Form.GetValues("roleId");
                    foreach (var item in roleArr)
                    {

                        var str = Request.Form.GetValues("ObjectIdArr");
                        var nameStr = Request.Form["ObjectNameArr"];
                        var webDataCtrl = Request.Form["WebDataCtrlId"];
                        int roleId;
                        int webDataCtrlId;
                        int.TryParse(webDataCtrl, out webDataCtrlId);
                        int.TryParse(item, out roleId);

                        var role = base.roleRepository.Find(i => i.Id == roleId);
                        if (webDataSettingRepository.Find(i => i.WebManageRolesId == roleId && i.WebDataCtrlId == webDataCtrlId) == null)//不能插入重复的类型数据集
                        {
                            entity.WebDepartmentsId = role.DepartmentID;
                            entity.ObjectIdArr = string.Join(",", str);
                            entity.ObjectNameArr = nameStr.Substring(0, nameStr.Length - 1);
                            entity.WebManageRolesId = roleId;
                            entity.WebDataCtrlId = webDataCtrlId;
                            webDataSettingRepository.Insert(entity);
                        }
                    }
                    return RedirectToAction("Index");
                }

                ModelState.AddModelError("", "请把表单填写完整...");
                return View();

            }
            catch (Exception ex)
            {
                return View();
            }
        }

        public ActionResult Edit(int id)
        {
            var model = webDataSettingRepository.Find(id);
            return View(model);
        }


        [ActionAuthority(Authority.Edit), ManagerActionLoggerAttribute("编辑数据集")]
        [HttpPost]
        public ActionResult Edit(int id, WebDataSetting entity)
        {
            try
            {
                var roleArr = Request.Form.GetValues("roleId");
                if (ModelState.IsValid)
                {
                    foreach (var item in roleArr)
                    {
                        var str = Request.Form.GetValues("ObjectIdArr");
                        var nameStr = Request.Form["ObjectNameArr"];
                        var webDataCtrl = Request.Form["WebDataCtrlId"];
                        int roleId;
                        int webDataCtrlId;
                        int.TryParse(webDataCtrl, out webDataCtrlId);
                        int.TryParse(item, out roleId);
                        var role = base.roleRepository.Find(i => i.Id == roleId);


                        var old = webDataSettingRepository.Find(id);
                        old.ObjectNameArr = nameStr.Substring(0, nameStr.Length - 1);
                        old.WebDepartmentsId = role.DepartmentID;
                        old.WebManageRolesId = roleId;
                        old.ObjectIdArr = string.Join(",", str);
                        old.WebDataCtrlId = webDataCtrlId;
                        webDataSettingRepository.Update(old);
                        return RedirectToAction("Index");
                    }
                }
                ModelState.AddModelError("", "请把表单填写完整...");
                return View();

            }
            catch
            {
                return View();
            }
        }


        [ActionAuthority(Authority.Delete), ManagerActionLoggerAttribute("删除数据集")]
        public ActionResult Delete(int id)
        {
            var old = webDataSettingRepository.Find(id);
            old.DataStatus = Lind.DDD.Domain.Status.Deleted;
            webDataSettingRepository.Update(old);
            return RedirectToAction("Index");
        }

        /// <summary>
        /// 对外开放的接口，用来根据类型，属性和角色，获取对应的数据
        /// </summary>
        /// <param name="type"></param>
        /// <param name="field"></param>
        /// <param name="roleId"></param>
        /// <returns></returns>
        [AllowAnonymous]
        public string GetDataSet(string type = "", string field = "", int userId = 0)
        {
            var roleIdArr = roleRepository.GetModel(i => i.WebManageUsers.Where(j => j.Id == userId).Count() > 0).Select(i => i.Id);
            string result = string.Empty;
            var model = webDataSettingRepository.GetModel()
                                           .Include(i => i.WebDataCtrl)
                                           .Where(i => i.WebDataCtrl.DataCtrlType.ToLower() == type.ToLower()
                                               && i.WebDataCtrl.DataCtrlField.ToLower() == field.ToLower()
                                               && roleIdArr.Contains(i.WebManageRolesId))
                                           .ToList();
            if (model != null && model.Count > 0)
            {
                result = string.Join(",", model.Select(i => i.ObjectIdArr));
            }
            return result;
        }
        /// <summary>
        /// 测试拿原数据集
        /// </summary>
        /// <returns></returns>
        [AllowAnonymous]
        public JsonResult GetCity()
        {
            List<SelectListItem> sl = new List<SelectListItem>();
            sl.Add(new SelectListItem { Value = "1", Text = "华北" });
            sl.Add(new SelectListItem { Value = "2", Text = "华南" });
            sl.Add(new SelectListItem { Value = "3", Text = "华东" });
            sl.Add(new SelectListItem { Value = "3", Text = "东北" });
            return Json(sl, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 测试拿原数据集
        /// </summary>
        /// <returns></returns>
        [AllowAnonymous]
        public JsonResult GetPrice()
        {
            List<SelectListItem> sl = new List<SelectListItem>();
            sl.Add(new SelectListItem { Value = "0", Text = "免费　" });
            sl.Add(new SelectListItem { Value = "100", Text = "100元" });
            sl.Add(new SelectListItem { Value = "600", Text = "600元" });
            sl.Add(new SelectListItem { Value = "1000", Text = "1000元" });
            return Json(sl, JsonRequestBehavior.AllowGet);
        }
    }


}
