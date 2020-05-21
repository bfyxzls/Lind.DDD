using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;

namespace Lind.DDD.IRepositories.DistributedReadWriteForEF
{
    /// <summary>
    /// redis配置信息加载
    /// </summary>
    internal class DistributedReadWriteManager
    {
        /// <summary>
        /// 配置信息实体
        /// </summary>
        public static IList<DistributedReadWriteSection> Instance
        {
            get
            {
                return GetSection();
            }
        }

        private static IList<DistributedReadWriteSection> GetSection()
        {
            var dic = ConfigurationManager.GetSection("DistributedReadWriteSection") as Dictionary<string, DistributedReadWriteSection>;
            if (dic != null)
                return dic.Values.ToList();
            return null;
        }
    }
}
