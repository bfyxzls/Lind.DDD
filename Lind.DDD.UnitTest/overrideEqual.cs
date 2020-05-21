using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Linq;
namespace Lind.DDD.UnitTest
{
    public class deviceList
    {
        public deviceList(string id, string longitude, string latitude)
        {
            this.deviceid = id;
            this.latitude = latitude;
            this.longitude = longitude;
        }
        /// <summary>
        /// 标题
        /// </summary>
        public string title { get; set; }
        /// <summary>
        /// 主键
        /// </summary>
        public string deviceid { get; set; }
        /// <summary>
        /// 精度
        /// </summary>
        public string longitude { get; set; }
        /// <summary>
        /// 纬度
        /// </summary>
        public string latitude { get; set; }
        public override bool Equals(object obj)
        {
            return this.deviceid == (obj as deviceList).deviceid;
        }
        public override int GetHashCode()
        {
            return this.deviceid.GetHashCode();
        }
        public override string ToString()
        {
            return string.Format("id:{0},latitude:{1},longitude:{2}", deviceid, latitude, longitude);
        }
    }

    [TestClass]
    public class overrideEqual
    {
        [TestMethod]
        public void OverrideMethod()
        {
            List<deviceList> arr1 = new List<deviceList>();
            List<deviceList> arr2 = new List<deviceList>();

            arr1.Add(new deviceList("1", "30", "30"));
            arr1.Add(new deviceList("2", "32", "32"));
            arr1.Add(new deviceList("3", "33", "33"));

            arr2.Add(new deviceList("2", "33", "33"));
            Console.WriteLine("Except");
            arr1.Except(arr2).ToList().ForEach(i => Console.WriteLine(i.ToString()));
            Console.WriteLine("Union");
            arr1.Union(arr2).ToList().ForEach(i => Console.WriteLine(i.ToString()));
            Console.WriteLine("Intersect");
            arr1.Intersect(arr2).ToList().ForEach(i => Console.WriteLine(i.ToString()));
        }
    }
}
