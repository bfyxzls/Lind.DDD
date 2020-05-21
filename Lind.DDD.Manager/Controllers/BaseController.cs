using Lind.DDD.Authorization;
using Lind.DDD.IRepositories;
using Lind.DDD.Manager.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace Lind.DDD.Manager.Controllers
{
    public abstract class BaseController : Controller
    {
        protected readonly IExtensionRepository<WebManageMenus> menuRepository;
        protected readonly IExtensionRepository<WebManageRoles> roleRepository;
        protected readonly IExtensionRepository<WebDepartments> departmentsRepository;
        protected readonly IExtensionRepository<WebManageUsers> userRepository;
        protected readonly IExtensionRepository<WebDataCtrl> webDataCtrlRepository;
        protected readonly IExtensionRepository<WebDataSetting> webDataSettingRepository;
        protected readonly IExtensionRepository<WebManageRoles_WebManageMenus_Authority_R> webManageRoles_WebManageMenus_Authority_RRepository;
        protected readonly IExtensionRepository<WebLogger> webLoggerRepository;
        protected readonly IExtensionRepository<WebCommonAreas> webCommonAreasRepository;
        protected readonly IExtensionRepository<Lind.DDD.Domain.WebAuthorityCommands> webAuthorityCommandsRepository;

        protected readonly ManagerContext db;
        public BaseController()
        {
            db = new ManagerContext();
            menuRepository = new ManagerEfRepository<WebManageMenus>();
            roleRepository = new ManagerEfRepository<WebManageRoles>();
            webDataCtrlRepository = new ManagerEfRepository<WebDataCtrl>();
            webDataSettingRepository = new ManagerEfRepository<WebDataSetting>();
            departmentsRepository = new ManagerEfRepository<WebDepartments>();
            userRepository = new ManagerEfRepository<WebManageUsers>();
            webManageRoles_WebManageMenus_Authority_RRepository = new ManagerEfRepository<WebManageRoles_WebManageMenus_Authority_R>();
            webLoggerRepository = new ManagerEfRepository<WebLogger>();
            webCommonAreasRepository = new ManagerEfRepository<WebCommonAreas>();
            webAuthorityCommandsRepository = new ManagerEfRepository<Lind.DDD.Domain.WebAuthorityCommands>();
            menuRepository.SetDataContext(db);
            roleRepository.SetDataContext(db);
            webDataCtrlRepository.SetDataContext(db);
            webDataSettingRepository.SetDataContext(db);
            departmentsRepository.SetDataContext(db);
            userRepository.SetDataContext(db);
            webManageRoles_WebManageMenus_Authority_RRepository.SetDataContext(db);
            webLoggerRepository.SetDataContext(db);
            webCommonAreasRepository.SetDataContext(db);
            webAuthorityCommandsRepository.SetDataContext(db);
        }

        public int Current_UserId
        {
            get
            {
                int userid;
                int.TryParse(CurrentUser.UserID, out userid);
                return userid;
            }
        }

        /// <summary>
        /// 填充区域数据
        /// </summary>
        /// <param name="id">角色ID</param>
        protected void FillRoleAreaData(int id)
        {
            FillRoleAreaData(new int[] { id });
        }
        /// <summary>
        /// 填充区域数据
        /// </summary>
        /// <param name="id">角色ID</param>
        protected void FillRoleAreaData(int[] id)
        {
            var typeArr = webDataCtrlRepository.GetModel(i => i.DataCtrlType == areaType).Select(i => i.Id);
            if (typeArr != null && typeArr.Count() > 0)
            {
                var valueArr = webDataSettingRepository.GetModel(i => id.Contains(i.WebManageRolesId) && typeArr.Contains(i.WebDataCtrlId))
                                                       .Select(i => i.ObjectIdArr);
                if (valueArr != null && valueArr.Count() > 0)
                {
                    var areaSel = new List<int>();
                    foreach (var item in valueArr)
                    {
                        var arr = item.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                        areaSel.AddRange(Array.ConvertAll(arr, i => Convert.ToInt32(i)));
                    }
                    ViewBag.areaSel = areaSel;
                }
            }
        }

        public string DeptDropdownList2(int? parentId)
        {

            StringBuilder sbr = new StringBuilder();
            var dept = departmentsRepository.Find(i => i.Level == 0);
            if (parentId.HasValue)
            {
                dept = departmentsRepository.Find(i => i.Id == parentId);
                GetSelectList(dept, sbr);
            }
            return sbr.ToString();
        }

        /// <summary>
        /// 递归得到它的所有祖宗以selectlist的形式进行拼接
        /// </summary>
        /// <param name="son"></param>
        /// <param name="sbr"></param>
        protected void GetSelectList(WebDepartments son, StringBuilder sbr)
        {
            StringBuilder inSbr = new StringBuilder();
            if (son != null)
            {
                if (son.Level == 0)
                    inSbr.Append("<select name='Parent' onchange = 'areaOnSelect(this)'><option value=''>全部</option>");
                else
                    inSbr.Append("<select name='Sub' onchange = 'areaOnSelect(this)'><option value=''>全部</option>");

                departmentsRepository.GetModel(i => i.ParentID == son.Id).ToList().ForEach(i =>
                {
                    if (i.Id == son.Id)
                        inSbr.Append("<option value='" + i.Id + "' selected='true'>" + i.Name + "</option>");
                    else
                        inSbr.Append("<option value='" + i.Id + "'>" + i.Name + "</option>");
                });

                inSbr.Append("</select>");
                sbr.Insert(0, inSbr);
                //   GetSelectList(son.Father, sbr);
            }
        }

        protected void GetSelectListAll(WebDepartments son, StringBuilder sbr, List<int> fatherList)
        {
            StringBuilder inSbr = new StringBuilder();
            if (son != null)
            {

                if (son.Sons != null && son.Sons.Any())
                {
                    inSbr.Append("<select name='DeptId' class='form-control'><option value='0'>请选择</option>");
                    son.Sons.ToList().ForEach(i =>
                    {
                        if (i.Id == son.Id || i.Id == son.ParentID || fatherList.Contains(i.Id))
                            inSbr.Append("<option value='" + i.Id + "' selected='true'>" + i.Name + "</option>");
                        else
                            inSbr.Append("<option value='" + i.Id + "'>" + i.Name + "</option>");
                    });

                    inSbr.Append("</select>");
                    sbr.Insert(0, inSbr);
                }
                GetSelectListAll(son.Father, sbr, fatherList);
            }
        }


        protected string areaType = "CommonArea";
        protected string areaprovince = "AreaProvince";
        protected string areacity = "AreaCity";
        protected string areadistrict = "AreaDistrict";

        /// <summary>
        /// 返回值处理
        /// </summary>
        Func<string, string> ProcessResult = (arr) =>
        {
            if (!string.IsNullOrWhiteSpace(arr))
            {
                var result = arr.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries).ToList();
                if (result != null && result.Count > 0)
                {
                    result.RemoveAll(i => i == "0");
                }
                return string.Join(",", result.ToArray());
            }
            return string.Empty;
        };
        /// <summary>
        /// 得到当前用户的区域限制（省）
        /// </summary>
        /// <returns></returns>
        protected string CommonAreaProvince()
        {
            return ProcessResult(GetDataSet(areaType, areaprovince));

        }
        /// <summary>
        /// 得到当前用户的区域限制（市）
        /// </summary>
        /// <returns></returns>
        protected string CommonAreaCity()
        {
            return ProcessResult(GetDataSet(areaType, areacity));
        }
        /// <summary>
        /// 得到当前用户的区域限制（县）
        /// </summary>
        /// <returns></returns>
        protected string CommonAreaDistrict()
        {
            return ProcessResult(GetDataSet(areaType, areadistrict));
        }

        /// <summary>
        /// 得到数据集
        /// </summary>
        /// <param name="type"></param>
        /// <param name="field"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        string GetDataSet(string type = "", string field = "", int userId = 0)
        {
            userId = userId == 0 ? Current_UserId : userId;
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

    }
}
