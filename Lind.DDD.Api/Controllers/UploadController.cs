using Lind.DDD.Upload;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;

namespace Lind.DDD.Api.Controllers
{
    /// <summary>
    /// api统一文件上传
    /// </summary>
    public class UploadController : ApiController
    {
        public HttpResponseMessage Post()
        {
            try
            {
                HttpPostedFile file = HttpContext.Current.Request.Files[0];
                var fileName = Lind.DDD.Upload.FileUploaderFactory.Instance.UploadImage(new ImageUploadParameter(file.InputStream, file.FileName, 1048576 * 10));
                return new HttpResponseMessage { Content = new StringContent(fileName.FilePath) };
            }
            catch (Exception)
            {

                throw;
            }
        }
    }
}
