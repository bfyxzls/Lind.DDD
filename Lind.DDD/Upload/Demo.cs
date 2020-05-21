using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
namespace Lind.DDD.Upload
{
    /// <summary>
    /// fastDFS分存布文件上传使用
    /// 在图片地址后添加尺寸的后缀自动生成小像，例如 http://www.fastdfs.com/demo/pictruename_100x100.jpg
    /// </summary>
    public class Demo
    {

        public List<string> Upload()
        {
            HttpRequestBase Request = null;
            List<string> result = new List<string>();
            for (int i = 0; i < Request.Files.Count; i++)
            {
                HttpPostedFileBase file = Request.Files[i];
                // result.Add(FileUploaderFactory.Instance.UploadFile(new uploadfile());
            }
            return result;
        }

    }
}
