using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System.Text;
using System.Reflection;
using Lind.DDD.Paging;
using System.Web.Script.Serialization;
using Newtonsoft.Json.Linq;

/// <summary>
///JsonHelper 的摘要说明
/// </summary>
namespace Lind.DDD.Utils
{
    /// <summary>
    /// JSON序列化帮助类
    /// </summary>
    public static class JsonHelper
    {
        private static JsonSerializerSettings _jsonSettings;

        static JsonHelper()
        {
            IsoDateTimeConverter datetimeConverter = new IsoDateTimeConverter();
            datetimeConverter.DateTimeFormat = "yyyy-MM-dd HH:mm:ss";

            _jsonSettings = new JsonSerializerSettings();
            _jsonSettings.MissingMemberHandling = Newtonsoft.Json.MissingMemberHandling.Ignore;
            _jsonSettings.NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore;
            _jsonSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore;
            _jsonSettings.Converters.Add(datetimeConverter);

        }

        #region 反序列化Json
        /// <summary>
        /// 将JSON串反序列化成对象
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="json"></param>
        /// <returns></returns>
        public static T FromJson<T>(this string json)
        {
            try
            {
                return JsonConvert.DeserializeObject<T>(json, _jsonSettings);
            }
            catch (Exception)
            {

                return default(T);
            }
        }

        /// <summary>
        /// 递归构建JSON对象
        /// </summary>
        /// <param name="old"></param>
        /// <param name="t"></param>
        /// <param name="item"></param>
        static void GeneratorJson(object old, Type t, dynamic item)
        {
            foreach (JProperty p in (item is JProperty ? item.First : item))
            {
                //属性 
                var property = t.GetProperty(p.Name);
                //属性类型
                var propertyType = property.PropertyType;
                //是不为复杂类型
                if (propertyType.IsClass && propertyType != typeof(string))
                {
                    //建立类的实例
                    var subEntity = Activator.CreateInstance(t.GetProperty(p.Name).PropertyType);
                    subEntity = JsonConvert.DeserializeObject(p.Value.ToString(), propertyType);
                    property.SetValue(old, subEntity);

                }
                //简单类型直接赋值
                else
                {
                    t.GetProperty(p.Name).SetValue(old, Convert.ChangeType(p.Value, propertyType));
                }
            }


        }

        /// <summary>
        /// 通过递归查找JSON对象
        /// </summary>
        /// <param name="old"></param>
        /// <param name="t"></param>
        /// <param name="item"></param>
        static void RecursionGeneratorJson(object old, Type t, dynamic item)
        {
            foreach (JProperty p in (item is JProperty ? item.First : item))
            {
                //属性 
                var property = t.GetProperty(p.Name);
                //属性类型
                var propertyType = property.PropertyType;
                //是不为复杂类型
                if (propertyType.IsClass && propertyType != typeof(string))
                {
                    //建立类的实例
                    var subEntity = Activator.CreateInstance(propertyType);
                    //为上级对象的本属性赋值
                    property.SetValue(old, subEntity);
                    RecursionGeneratorJson(subEntity, propertyType, p);
                }
                //简单类型直接赋值
                else
                {
                    t.GetProperty(p.Name).SetValue(old, Convert.ChangeType(p.Value, propertyType));
                }
            }
        }

        /// <summary>
        /// 将具有PagedList`1格式的JSON 数据反序列化成指定对象。
        /// </summary>
        /// <typeparam name="T">对象类型。</typeparam>
        /// <param name="json">JSON 数据。</param>
        /// <returns></returns>
        public static PagedList<T> FromPagedListJson<T>(this string json) where T : class,new()
        {
            if (string.IsNullOrWhiteSpace(json))
                return default(PagedList<T>);

            var list = JsonConvert.DeserializeObject<dynamic>(json, _jsonSettings);
            PagedList<T> pl = new PagedList<T>();
            foreach (var item in list["Model"])
            {
                T old = new T();
                GeneratorJson(old, typeof(T), item);
                pl.Add(old);
            }
            pl.PageIndex = list["PageIndex"];
            pl.PageSize = list["PageSize"];
            pl.TotalCount = list["TotalCount"];
            pl.PageSize = list["PageSize"];
            return pl;
        }
        #endregion

        #region 序列化Json字符串
        /// <summary>
        /// 将指定的对象序列化成 JSON 数据。
        /// </summary>
        /// <param name="obj">要序列化的对象。</param>
        /// <returns></returns>  
        public static string ToJson(this object obj)
        {
            try
            {
                if (null == obj)
                    return null;

                if (obj is System.Data.DataTable)
                {
                    System.Data.DataTable _sourceTable = (System.Data.DataTable)obj;
                    foreach (System.Data.DataRow row in _sourceTable.Rows)
                    {

                        foreach (System.Data.DataColumn column in _sourceTable.Columns)
                        {

                            if (column.DataType == typeof(System.Int32))
                            {
                                if (row[column] == DBNull.Value)
                                {

                                    row[column.ColumnName] = 0;
                                }
                            }
                            if (column.DataType == typeof(System.Decimal))
                            {

                                if (row[column] == DBNull.Value)
                                {
                                    row[column.ColumnName] = 0M;
                                }

                            }

                        }
                    }
                    _sourceTable.AcceptChanges();
                    return JsonConvert.SerializeObject(_sourceTable, Formatting.None, _jsonSettings);

                }
                else
                {
                    if (obj is IPagedList)
                    {
                        var page = obj as IPagedList;
                        //处理分页结果的对象
                        return JsonConvert.SerializeObject(new
                        {
                            Model = obj,
                            PageIndex = page.PageIndex,
                            PageSize = page.PageSize,
                            TotalCount = page.TotalCount,
                            TotalPages = page.TotalPages
                        }, Formatting.None, _jsonSettings);
                    }


                    return JsonConvert.SerializeObject(obj, Formatting.None, _jsonSettings);
                }


            }
            catch (Exception)
            {
                return null;

            }


        }

        /// <summary>
        /// 功能:集合按需要序列化
        /// author:仓储大叔
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="param"></param>
        /// <returns></returns>
        public static string ToJson(this object obj, List<string> param)
        {

            if (obj is System.Collections.IEnumerable || obj is IPagedList)
            {
                //分页集合序列化
                if (obj is IPagedList)
                {
                    var page = obj as IPagedList;
                    foreach (var t in (IEnumerable<object>)obj)
                    {
                        GeneratorJson(t, param);
                    }
                    //处理分页结果的对象，它在反序列化时出现了问题
                    return JsonConvert.SerializeObject(new
                    {
                        Model = obj,
                        PageIndex = page.PageIndex,
                        PageSize = page.PageSize,
                        TotalCount = page.TotalCount,
                        TotalPages = page.TotalPages
                    }, Formatting.None, _jsonSettings);
                }
                else
                {
                    foreach (var t in (IEnumerable<object>)obj)
                    {
                        GeneratorJson(t, param);
                    }
                }
            }

            else
            {
                GeneratorJson(obj, param);
            }


            return JsonConvert.SerializeObject(obj, Formatting.None, _jsonSettings);
        }
        #endregion

        /// <summary>
        /// 为对象生成Json字符串
        /// </summary>
        /// <param name="buffer"></param>
        /// <param name="t"></param>
        /// <param name="param"></param>
        static void GeneratorJson(object t, List<string> param)
        {
            var pList = t.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase)
                .Where(i => i.Name != "Capacity" && i.Name != "Count" && i.Name != "Item");
            foreach (var item in pList)
            {
                if (param != null)
                {
                    if (item != null && !param.Contains(item.Name, new IgnorEqualityComparer()) && item.CanWrite)
                    {

                        item.SetValue(t, null);
                    }
                }
            }

        }

    }

    public class IgnorEqualityComparer : IEqualityComparer<string>
    {

        #region IEqualityComparer<string> 成员

        public bool Equals(string x, string y)
        {
            x = x ?? string.Empty;
            y = y ?? string.Empty;
            return x.ToLower() == y.ToLower();
        }

        public int GetHashCode(string obj)
        {
            return obj.GetHashCode();
        }

        #endregion
    }
}