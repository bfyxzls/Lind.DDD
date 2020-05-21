using Lind.DDD.Manager.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Linq.Expressions;
using System.Web;
using System.Web.Mvc;
using Lind.DDD.Manager.Filters;
using Lind.DDD.Filters;

namespace Lind.DDD.Manager.Controllers
{
    /// <summary>
    /// 角色管理
    /// </summary>
    public class WebRoleController : BaseController
    {
        static WebRoleController()
        {
            var repository = new ManagerEfRepository<WebCommonAreas>();
            staticAreaList = repository.GetModel().ToList();
        }
        [ActionAuthority(Authorization.Authority.Detail), ManagerActionLoggerAttribute("角色列表")]
        public ActionResult Index(int deptId = 0, int page = 1)
        {
            var linq = roleRepository.GetModel().Include(i => i.WebDepartments);

            if (deptId > 0)
            {
                linq = linq.Where(i => i.DepartmentID == deptId);
            }
            ViewBag.TotalRecord = linq.Count();

            return View(linq.OrderBy(i => i.Id).ToPagedList(new Paging.PageParameters(page, 10)));

        }


        public PartialViewResult Details(int id)
        {
            var model = roleRepository.GetModel()
                                             .Include(i => i.WebDepartments)
                                             .Include(i => i.WebManageRoles_WebManageMenus_Authority_R)
                                             .FirstOrDefault(i => i.Id == id);
            foreach (var i in model.WebManageRoles_WebManageMenus_Authority_R)
            {
                i.WebManageMenus = menuRepository.Find(i.MenuId);
            }

            FillRoleAreaData(id);

            return PartialView(model);
        }


        public ActionResult Create()
        {
            return View();
        }
        public JsonResult SaveCreate(int id, string roleName, int sortNumber, string about, int departmentId, string menuArr)
        {
            if (roleRepository.Find(i => i.RoleName == roleName) != null)
            {
                ModelState.AddModelError("", "名称不能重复...");
                return Json(new { status = 0, msg = "名称不能重复" });
            }
            return SaveEdit(id, roleName, sortNumber, about, departmentId, menuArr);

        }

        static List<WebCommonAreas> staticAreaList;
        /// <summary>
        /// 更新角色的数据集权限
        /// </summary>
        /// <param name="roleId"></param>
        void UpdateDataSetting(int roleId, int deptId)
        {
            if (!string.IsNullOrWhiteSpace(Request.Form["area"]))
            {
                try
                {

                    var requestAreaArr = Request.Form["area"].Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries).ToList();
                    //删除老的数据权限
                    webDataSettingRepository.Delete(webDataSettingRepository.GetModel(i => i.WebManageRolesId == roleId).ToList());
                    //添加新的数据权限
                    var areaArr = Array.ConvertAll(requestAreaArr.ToArray(), i => Convert.ToInt32(i));
                    if (areaArr.Contains(-1))
                    {
                        areaArr = areaArr.Where(i => i == -1 || i == 0).ToArray();
                    }
                    var provinceArr = staticAreaList.Where(i => i.ParentId == 0 && areaArr.Contains(i.ID)).Select(i => i.ID).ToList();
                    var provinceNameArr = staticAreaList.Where(i => i.ParentId == 0 && areaArr.Contains(i.ID)).Select(i => i.Name).ToList();
                    try
                    {
                        var all = staticAreaList.FirstOrDefault(i => !i.ParentId.HasValue);
                        provinceArr.Add(all.ID);
                        provinceNameArr.Add(all.Name);
                    }
                    catch (Exception)
                    {
                        throw new ArgumentException("请配置ID为0，ParentID为NULL的全国区域记录");
                    }


                    var cityArr = staticAreaList.Where(i => provinceArr.Contains(i.ParentId ?? 0) && areaArr.Contains(i.ID)).Select(i => i.ID).ToList();
                    var cityNameArr = staticAreaList.Where(i => provinceArr.Contains(i.ParentId ?? 0) && areaArr.Contains(i.ID)).Select(i => i.Name).ToList();
                    var districtArr = staticAreaList.Where(i => cityArr.Contains(i.ParentId ?? 0) && areaArr.Contains(i.ID)).Select(i => i.ID).ToList();
                    var districtNameArr = staticAreaList.Where(i => cityArr.Contains(i.ParentId ?? 0) && areaArr.Contains(i.ID)).Select(i => i.Name).ToList();
                    var provinceCtrlId = webDataCtrlRepository.Find(i => i.DataCtrlField == areaprovince).Id;
                    var cityCtrlId = webDataCtrlRepository.Find(i => i.DataCtrlField == areacity).Id;
                    var districtCtrlId = webDataCtrlRepository.Find(i => i.DataCtrlField == areadistrict).Id;
                    if (provinceArr.Count > 0)
                        webDataSettingRepository.Insert(new WebDataSetting
                        {
                            ObjectIdArr = string.Join(",", provinceArr),
                            ObjectNameArr = string.Join(",", provinceNameArr),
                            WebDataCtrlId = provinceCtrlId,
                            WebManageRolesId = roleId,
                            WebDepartmentsId = deptId
                        });
                    if (cityArr.Count > 0)
                        webDataSettingRepository.Insert(new WebDataSetting
                        {
                            ObjectIdArr = string.Join(",", cityArr),
                            ObjectNameArr = string.Join(",", cityNameArr),
                            WebDataCtrlId = cityCtrlId,
                            WebManageRolesId = roleId,
                            WebDepartmentsId = deptId
                        });
                    if (districtArr.Count > 0)
                        webDataSettingRepository.Insert(new WebDataSetting
                        {
                            ObjectIdArr = string.Join(",", districtArr),
                            ObjectNameArr = string.Join(",", districtNameArr),
                            WebDataCtrlId = districtCtrlId,
                            WebManageRolesId = roleId,
                            WebDepartmentsId = deptId
                        });
                }
                catch (Exception)
                {
                    throw new ArgumentException("请先添加数据集类型CommonArea，及字段AreaProvince，AreaCity和AreaDistrict！");
                }

            }
        }

        public JsonResult SaveEdit(int id, string roleName, int sortNumber, string about, int departmentId, string menuArr)
        {
            try
            {
                if (departmentId == 0)
                {
                    ModelState.AddModelError("", "请选择部门...");
                    return Json(new { status = 0, msg = "请选择部门" });

                }

                if (string.IsNullOrWhiteSpace(menuArr))
                {

                    ModelState.AddModelError("", "请选择菜单...");
                    return Json(new { status = 0, msg = "请选择菜单" });

                }

                if (!ModelState.IsValid)
                {
                    ModelState.AddModelError("", "请把表单填写完整...");
                    return Json(new { status = 0, msg = "请把表单填写完整" });

                }
                List<WebManageRoles_WebManageMenus_Authority_R> role_Menu_Authority = new List<WebManageRoles_WebManageMenus_Authority_R>();

                menuArr.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries).ToList().ForEach(item =>
                {
                    var menu_authority = item.Split(new char[] { '|' }, StringSplitOptions.RemoveEmptyEntries);

                    role_Menu_Authority.Add(new WebManageRoles_WebManageMenus_Authority_R
                    {
                        Authority = Convert.ToInt64(menu_authority[1]) | 1,
                        MenuId = Convert.ToInt32(menu_authority[0]),
                    });

                });
                WebManageRoles entity;
                if (id == 0)
                {
                    entity = new WebManageRoles();
                    entity.RoleName = roleName;
                    entity.SortNumber = sortNumber;
                    entity.About = about;
                    entity.DepartmentID = departmentId;
                    entity.WebManageRoles_WebManageMenus_Authority_R = role_Menu_Authority;
                    entity.Operator = "";
                    roleRepository.Insert(entity);
                }
                else
                {
                    entity = roleRepository.GetModel()
                                           .Include(i => i.WebManageRoles_WebManageMenus_Authority_R)
                                           .FirstOrDefault(i => i.Id == id);

                    entity.RoleName = roleName;
                    entity.SortNumber = sortNumber;
                    entity.About = about;
                    entity.DepartmentID = departmentId;
                    role_Menu_Authority.ForEach(i => i.RoleId = id);
                    roleRepository.Update(entity);
                    webManageRoles_WebManageMenus_Authority_RRepository.Delete(webManageRoles_WebManageMenus_Authority_RRepository.GetModel(i => i.RoleId == id).ToList());
                    webManageRoles_WebManageMenus_Authority_RRepository.Insert(role_Menu_Authority);
                }
                UpdateDataSetting(entity.Id, entity.DepartmentID);
                return Json(new { status = 1 });
            }
            catch
            {
                return Json(new { status = 0, msg = "数据处理失败" });
            }
        }

        public ActionResult Edit(int id)
        {
            FillRoleAreaData(id);
            return View(roleRepository.GetModel()
                                      .Include(i => i.WebManageRoles_WebManageMenus_Authority_R)
                                      .FirstOrDefault(i => i.Id == id));
        }


        [ActionAuthority(Authorization.Authority.Delete), ManagerActionLoggerAttribute("删除角色")]
        public ActionResult Delete(int id)
        {
            var old = roleRepository.Find(id);
            old.DataStatus = Lind.DDD.Domain.Status.Deleted;
            roleRepository.Update(old);
            return RedirectToAction("Index");
        }


    }
}
