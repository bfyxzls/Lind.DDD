using AutoMapper;
using Lind.DDD.Paging;
using MongoDB.Bson;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace System.Linq
{
    /// <summary>
    /// AutoMapper映射扩展方法
    /// </summary>
    public static class AutoMapExtensions
    {
        static AutoMapExtensions()
        {
            //string->int
            Mapper.CreateMap<string, int>().ConvertUsing((x) =>
            {
                int o = 0;
                int.TryParse(x, out o);
                return o;
            });
            //int->bool
            Mapper.CreateMap<int, bool>().ConvertUsing((x) =>
            {
                return x > 0;
            });
            //string->objectId
            Mapper.CreateMap<string, ObjectId>().ConvertUsing((x) =>
            {
                return (x != null) ? ObjectId.Parse(x) : ObjectId.Empty;
            });
            //objectId->string
            Mapper.CreateMap<ObjectId, string>().ConvertUsing((x) =>
            {
                return (x != null) ? x.ToString() : string.Empty;
            });

        }
        /// <summary>
        /// 为集合进行automapper
        /// </summary>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="self"></param>
        /// <returns></returns>
        public static IEnumerable<TResult> MapTo<TResult>(this IEnumerable self)
        {
            if (self == null)
                throw new ArgumentNullException();
            return (IEnumerable<TResult>)Mapper.DynamicMap(self, self.GetType(), typeof(IEnumerable<TResult>));
        }
        /// <summary>
        /// 为分页集合进行automapper
        /// </summary>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="self"></param>
        /// <returns></returns>
        public static PagedList<TResult> MapToPaged<TResult>(this IPagedList self)
        {
            if (self == null)
                throw new ArgumentNullException();
            var result = (PagedList<TResult>)Mapper.DynamicMap(self, self.GetType(), typeof(PagedList<TResult>));
            //显示的为分页参数赋值
            result.PageIndex = self.PageIndex;
            result.PageSize = self.PageSize;
            result.TotalCount = self.TotalCount;
            result.TotalPages = self.TotalPages;
            return result;

        }
        /// <summary>
        /// 为新对象进行automapper
        /// </summary>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="self"></param>
        /// <returns></returns>
        public static TResult MapTo<TResult>(this object self)
        {
            if (self == null)
                throw new ArgumentNullException();
            Mapper.CreateMap(self.GetType().UnderlyingSystemType, typeof(TResult));
            return (TResult)Mapper.Map(self, self.GetType(), typeof(TResult));
        }

        /// <summary>
        /// 为已经存在的对象进行automapper
        /// </summary>
        /// <typeparam name="TSource"></typeparam>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="self"></param>
        /// <param name="result"></param>
        /// <returns></returns>
        public static TResult MapTo<TResult>(this object self, TResult result)
        {
            if (self == null)
                throw new ArgumentNullException();

            //解决相同类型，原值被新值（字段的默认值）覆盖的问题。新的问题：字段默认值为0时，不能实现赋值了
            //foreach (var item in typeof(TResult).GetProperties(Reflection.BindingFlags.Public | Reflection.BindingFlags.Instance))
            //{
            //    if (self.GetType().GetProperty(item.Name) != null)
            //    {
            //        var newVal = self.GetType().GetProperty(item.Name).GetValue(self);
            //        Type pType = self.GetType().GetProperty(item.Name).PropertyType;
            //        if (Convert.ChangeType(DefaultForType(pType), pType) != null && Convert.ChangeType(newVal, pType).GetHashCode() != Convert.ChangeType(DefaultForType(pType), pType).GetHashCode()
            //            )
            //        {
            //            item.SetValue(result, newVal);
            //        }
            //    }
            //}
            Mapper.CreateMap(self.GetType().UnderlyingSystemType, typeof(TResult));
            return (TResult)Mapper.Map(self, result, self.GetType(), typeof(TResult));
        }

        public static object DefaultForType(Type targetType)
        {
            return targetType.IsValueType ? Activator.CreateInstance(targetType) : null;
        }


    }
}
