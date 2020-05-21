using Lind.DDD.Domain;
using Lind.DDD.IRepositories;
using Lind.DDD.Utils;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.Mvc;

namespace Lind.DDD.Manager.Controllers
{
    /// <summary>
    /// 拥有者控制器
    /// </summary>
    public class OwnerController : Controller
    {
        /// <summary>
        /// 具有拥有者字段的数据表
        /// </summary>
        /// <returns></returns>
        public ActionResult Index()
        {
            return View(Lind.DDD.Utils.AssemblyHelper.GetTypeNamesByInterfaces(typeof(IOwnerBehavor)));
        }

        /// <summary>
        /// 更新指定表的拥有者字段
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult Edit(string name)
        {
            ViewBag.Name = string.IsNullOrWhiteSpace(name) ? "全部表" : name;
            return View();
        }

        /// <summary>
        /// 更新表字段
        /// </summary>
        /// <param name="name"></param>
        /// <param name="val"></param>
        /// <param name="newVal"></param>
        void UpdateTable(string name, int oldVal, int newVal)
        {
            var type = AssemblyHelper.GetEntityTypeByName(name);
            Type reposType = typeof(ManagerEfRepository<>);
            var objType = reposType.MakeGenericType(type);
            object o = Activator.CreateInstance(objType);
            var entity = objType.InvokeMember("GetModel", BindingFlags.Default | BindingFlags.InvokeMethod, null, o, null);
            var atest = (IEnumerable)entity;

            var newList = (IList)Activator.CreateInstance(typeof(List<>).MakeGenericType(type));

            foreach (var item in atest)
            {
                if ((int)type.GetProperty("OwnerId").GetValue(item) == oldVal)
                {
                    var a = Convert.ChangeType(item, type);
                    type.GetProperty("OwnerId").SetValue(item, newVal);
                    newList.Add(item);
                }
            }

            objType.InvokeMember("BulkUpdate", BindingFlags.Default | BindingFlags.InvokeMethod, null, o, new object[] { newList, "OwnerId" });
        }
        [HttpPost]
        public ActionResult Edit(string name, FormCollection collection)
        {
            try
            {
                int oldVal;
                int.TryParse(collection["oldValue"], out oldVal);
                int val;
                int.TryParse(collection["newValue"], out val);

                if (string.IsNullOrWhiteSpace(name))//全部表
                {
                    foreach (var type in AssemblyHelper.GetTypeNamesByInterfaces(typeof(IOwnerBehavor)))
                    {
                        UpdateTable(type, oldVal, val);
                    }
                }
                else
                {
                    UpdateTable(name, oldVal, val);
                }

                return RedirectToAction("Index");
            }
            catch
            {
                throw;
            }
        }


    }
}
