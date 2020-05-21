using Lind.DDD.Utils.ImageHelper;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lind.DDD.UnitTest
{
    [TestClass]
    public class ImageHelperTest
    {
        [TestMethod]
        public void ImageWorld()
        {
            ImageHelper.GeneratorWorldOfImage(new ImageWorldRequest
            {
                FilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "招商会 (61).jpg"),
                Worlds = new string[] { "中国人", "美国人", "小日本", "吴毅挺，目前负责携程私有云、虚拟桌面云、网站应用持续交付等研发团队，专注于 Cloud/Continuous Delivery，用技术创新提升研发、运营效率；2012 年加入携程，从零组建携程云平台团队，基于OpenStack研发携程私有云，用于管理携程所有的开发、测试及生产环境多数据中心基础设施；曾在 OpenStack 香港峰会、中国云计算大会、QCon等大会做分享。 " },
                FontSize = 24,

            }).Save("1.jpg");

        }
    }
}
