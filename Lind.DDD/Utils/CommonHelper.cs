using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.UI;
using Microsoft.VisualBasic;
using System.Collections;
using System.Runtime.Serialization.Formatters.Binary;

namespace Lind.DDD.Utils
{
    /// <summary>
    /// 工具类
    /// </summary>
    public class CommonHelper
    {

        private static FileVersionInfo AssemblyFileVersion = FileVersionInfo.GetVersionInfo(Assembly.GetExecutingAssembly().Location);

        #region 获取 通过替换为转义码来转义的元字符集
        /// <summary>
        /// 获取 通过替换为转义码来转义的元字符集
        /// </summary>
        /// <param name="str">包含要转换的文本的输入字符串</param>
        /// <param name="isWeight">当需要在HTML页面中输出时候选择 true</param>
        /// <returns>包含要转换的文本的输入字符串</returns>
        public static string GetEscape(string str, bool isWeight)
        {
            StringBuilder sb = new StringBuilder();
            foreach (char item in str)
            {
                if (item.Equals('$')
                    || item.Equals('{')
                    || item.Equals('}'))
                    if (isWeight)
                        sb.Append('\\').Append('\\').Append(item);
                    else
                        sb.Append('\\').Append(item);
                else
                    sb.Append(item);
            }
            return sb.ToString();
        }
        #endregion

        #region 字符串截取
        /// <summary>
        /// 字符串截取
        /// </summary>
        /// <param name="oString">字符窜对象</param>
        /// <param name="showLong">显示长度</param>
        /// <param name="appeStr">追加字符串</param>
        /// <returns>返回处理后的字符串</returns>
        public static string CutString(string str, int showLong, string appeStr)
        {
            if (str.Length < showLong)
                return str;
            StringBuilder sbRtn = new StringBuilder();
            int curr = 0;
            showLong = showLong * 2;
            char[] cArr = new char[1];
            foreach (char c in str)
            {
                cArr[0] = c;
                if (curr >= showLong)
                    break;
                curr += GetStrRealLength(cArr);
                sbRtn.Append(c);
            }
            if (sbRtn.Length < str.Length)
                sbRtn.Append(appeStr);
            return sbRtn.ToString();
        }

        #region 返回字符串真实长度, 1个汉字长度为2
        /// <summary>
        /// 返回字符串真实长度, 1个汉字长度为2
        /// </summary>
        /// <returns></returns>
        static int GetStrRealLength(string str)
        {
            return Encoding.Default.GetBytes(str).Length;
        }
        /// <summary>
        /// 返回字符串真实长度, 1个汉字长度为2
        /// </summary>
        /// <returns></returns>
        static int GetStrRealLength(char[] cArr)
        {
            return Encoding.Default.GetBytes(cArr).Length;
        }
        #endregion

        #endregion

        #region 删除字符串尾部的回车/换行/空格
        /// <summary>
        /// 删除字符串尾部的回车/换行/空格
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static string RTrim(string str)
        {
            for (int i = str.Length; i >= 0; i--)
            {
                if (str[i].Equals(" ") || str[i].Equals("\r") || str[i].Equals("\n"))
                {
                    str.Remove(i, 1);
                }
            }
            return str;
        }
        #endregion

        #region 清除给定字符串中的回车及换行符
        /// <summary>
        /// 清除给定字符串中的回车及换行符
        /// </summary>
        /// <param name="str">要清除的字符串</param>
        /// <returns>清除后返回的字符串</returns>
        public static string ClearBR(string str)
        {
            Regex RegexBr = new Regex(@"(\r\n)", RegexOptions.IgnoreCase);
            Match m = null;
            for (m = RegexBr.Match(str); m.Success; m = m.NextMatch())
            {
                str = str.Replace(m.Groups[0].ToString(), string.Empty);
            }
            return str;
        }
        #endregion

        #region 自定义的替换字符串函数
        /// <summary>
        /// 自定义的替换字符串函数
        /// </summary>
        /// <param name="SourceString">源字符</param>
        /// <param name="SearchString">老字符</param>
        /// <param name="ReplaceString">新字符</param>
        /// <param name="IsCaseInsensetive">是否区分大小写</param>
        /// <returns>已经替换的字符串</returns>
        public static string ReplaceString(string sourceString, string oldStr, string newStr, bool isCaseInsensetive)
        {
            return Regex.Replace(
                    sourceString,
                    Regex.Escape(oldStr),
                    newStr,
                    isCaseInsensetive ? RegexOptions.IgnoreCase : RegexOptions.None
                );
        }
        #endregion

        #region 根据阿拉伯数字返回月份的名称(可更改为某种语言)
        /// <summary>
        /// 根据阿拉伯数字返回月份的名称(可更改为某种语言)
        /// </summary>	
        public static string[] Monthes
        {
            get
            {
                return new string[] { "January", "February", "March", "April", "May", "June", "July", "August", "September", "October", "November", "December" };
            }
        }
        #endregion

        #region 分割字符串
        /// <summary>
        /// 分割字符串
        /// </summary>
        /// <param name="strContent"></param>
        /// <param name="strSplit"></param>
        /// <returns></returns>
        public static string[] SplitString(string strContent, string strSplit)
        {
            if (strContent.IndexOf(strSplit) < 0)
            {
                string[] tmp = { strContent };
                return tmp;
            }
            return Regex.Split(strContent, Regex.Escape(strSplit), RegexOptions.IgnoreCase);
        }
        #endregion

        //Web 开发

        #region 得到Email 的主机名  HostName
        /// <summary>
        /// 得到Email 的主机名  HostName
        /// </summary>
        /// <param name="strEmail">要判断的email字符串</param>
        /// <returns>Email 的主机名  HostName</returns>
        public static string GetEmailHostName(string strEmail)
        {
            if (strEmail.IndexOf("@") < 0)
            {
                return "";
            }
            return strEmail.Substring(strEmail.LastIndexOf("@")).ToLower();
        }
        #endregion

        #region 生成指定数量的html空格符号
        /// <summary>
        /// 生成指定数量的html空格符号
        /// </summary>
        public static string HtmlSpaces(int nSpaces)
        {
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < nSpaces; i++)
            {
                sb.Append(" &nbsp;&nbsp;");
            }
            return sb.ToString();
        }
        #endregion

        #region 判断文件名是否为浏览器可以直接显示的图片文件名
        /// <summary>
        /// 判断文件名是否为浏览器可以直接显示的图片文件名
        /// </summary>
        /// <param name="filename">文件名</param>
        /// <returns>是否可以直接显示</returns>
        public static bool IsImgFilename(string filename)
        {
            filename = filename.Trim();
            if (filename.EndsWith(".") || filename.IndexOf(".") == -1)
            {
                return false;
            }
            string extname = filename.Substring(filename.LastIndexOf(".") + 1).ToLower();
            return (extname == "jpg" || extname == "jpeg" || extname == "png" || extname == "bmp" || extname == "gif");
        }
        #endregion

        #region 返回URL中结尾的文件名
        /// <summary>
        /// 返回URL中结尾的文件名
        /// </summary>		
        public static string GetUrlFileName(string url)
        {
            if (url == null)
            {
                return "";
            }
            string[] strs1 = url.Split(new char[] { '/' });
            return strs1[strs1.Length - 1].Split(new char[] { '?' })[0];
        }
        #endregion

        #region 替换回车换行符为html换行符'<br />'
        /// <summary>
        /// 替换回车换行符为html换行符
        /// </summary>
        public static string StrFormat(string str)
        {
            string str2;

            if (str == null)
            {
                str2 = "";
            }
            else
            {
                str = str.Replace("\r\n", "<br />");
                str = str.Replace("\n", "<br />");
                str2 = str;
            }
            return str2;
        }
        #endregion

        #region  HTML 编码 操作
        /// <summary>
        /// 返回 HTML 字符串的编码结果
        /// </summary>
        /// <param name="str">字符串</param>
        /// <returns>编码结果</returns>
        public static string HtmlEncode(string str)
        {
            return HttpUtility.HtmlEncode(str);
        }

        /// <summary>
        /// 返回 HTML 字符串的解码结果
        /// </summary>
        /// <param name="str">字符串</param>
        /// <returns>解码结果</returns>
        public static string HtmlDecode(string str)
        {
            return HttpUtility.HtmlDecode(str);
        }

        /// <summary>
        /// 返回 URL 字符串的编码结果
        /// </summary>
        /// <param name="str">字符串</param>
        /// <returns>编码结果</returns>
        public static string UrlEncode(string str)
        {
            return HttpUtility.UrlEncode(str);
        }

        /// <summary>
        /// 返回 URL 字符串的编码结果
        /// </summary>
        /// <param name="str">字符串</param>
        /// <returns>解码结果</returns>
        public static string UrlDecode(string str)
        {
            return HttpUtility.UrlDecode(str);
        }
        #endregion

        #region 移除Html标记
        /// <summary>
        /// 移除Html标记
        /// </summary>
        /// <param name="content">html</param>
        /// <returns>string</returns>
        public static string RemoveHtml(string html)
        {
            string regexstr = @"<[^>]*>";
            return Regex.Replace(html, regexstr, string.Empty, RegexOptions.IgnoreCase);
        }
        #endregion

        #region 过滤HTML中的不安全标签
        /// <summary>
        /// 过滤HTML中的不安全标签
        /// </summary>
        /// <param name="content">html</param>
        /// <returns>string</returns>
        public static string RemoveUnsafeHtml(string html)
        {
            html = Regex.Replace(html, @"(\<|\s+)o([a-z]+\s?=)", "$1$2", RegexOptions.IgnoreCase);
            html = Regex.Replace(html, @"(script|frame|form|meta|behavior|style)([\s|:|>])+", "$1.$2", RegexOptions.IgnoreCase);
            return html;
        }
        /// <summary>
        /// 过滤HTML标签
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        public static string FilterStr(string source)
        {
            source = source.Replace("& ", "& ");
            source = source.Replace(" < ", "< ");
            source = source.Replace("> ", "> ");
            source = source.Replace(" ' ", " ' ' "); source = source.Replace("\n ", " <br/> ");
            source = source.Replace("\r\n ", " <br/> ");
            return source;
        }
        #endregion

        #region 从HTML中获取文本,保留br,p,img
        /// <summary>
        /// 从HTML中获取文本,保留br,p,img
        /// </summary>
        /// <param name="HTML"></param>
        /// <returns></returns>
        public static string GetTextFromHTML(string HTML)
        {
            return new Regex(@"</?(?!br|/?p|img)[^>]*>", RegexOptions.IgnoreCase).Replace(HTML, string.Empty);
        }
        #endregion

        #region FileUpLoad
        /// <summary>
        /// 数据上传
        /// </summary>
        /// <param name="uri">远程地址</param>
        /// <param name="fileFullName">本地文件全部名称</param>
        public static void FileUpLoad(string uri, string fileFullName)
        {
            System.Net.WebClient webClient = new System.Net.WebClient();
            if (File.Exists(fileFullName))
            {
                webClient.UploadFile(uri, fileFullName);
            }
            else
            {
                throw new Exception("请输入存在的文件");
            }

            // 使用流的方式上传
            //FileStream fs = new FileStream(fileFullName, FileMode.Open, FileAccess.Read);
            //BinaryReader br = new BinaryReader(fs);
            //Byte[] postArray = br.ReadBytes(Convert.ToInt32(fs.Length));
            //Stream postStream = webClient.OpenWrite(uri, "PUT");
            //if (postStream.CanWrite)
            //{
            //    postStream.Write(postArray, 0, postArray.Length);
            //}
            //postStream.Close();
            //fs.Close();

        }
        #endregion

        #region 获得Assembly 版本信息
        /// <summary>
        /// 获得Assembly版本号
        /// </summary>
        /// <returns></returns>
        public static string GetAssemblyVersion()
        {
            return string.Format("{0}.{1}.{2}", AssemblyFileVersion.FileMajorPart, AssemblyFileVersion.FileMinorPart, AssemblyFileVersion.FileBuildPart);
        }

        /// <summary>
        /// 获得Assembly产品名称
        /// </summary>
        /// <returns></returns>
        public static string GetAssemblyProductName()
        {
            return AssemblyFileVersion.ProductName;
        }

        /// <summary>
        /// 获得Assembly产品版权
        /// </summary>
        /// <returns></returns>
        public static string GetAssemblyCopyright()
        {
            return AssemblyFileVersion.LegalCopyright;
        }
        #endregion

        #region 将全角数字转换为数字
        /// <summary>
        /// 将全角数字转换为数字
        /// </summary>
        /// <param name="SBCCase"></param>
        /// <returns></returns>
        public static string SBCCaseToNumberic(string SBCCase)
        {
            char[] c = SBCCase.ToCharArray();
            for (int i = 0; i < c.Length; i++)
            {
                byte[] b = System.Text.Encoding.Unicode.GetBytes(c, i, 1);
                if (b.Length == 2)
                {
                    if (b[1] == 255)
                    {
                        b[0] = (byte)(b[0] + 32);
                        b[1] = 0;
                        c[i] = System.Text.Encoding.Unicode.GetChars(b)[0];
                    }
                }
            }
            return new string(c);
        }
        #endregion

        #region 根据Url获取 请求相关

        /// <summary>
        /// 根据Url获取HTML
        /// </summary>
        /// <param name="url">Url</param>
        /// <param name="encoding"></param>
        /// <returns>HTML</returns>
        public static string GetHTMLByUrl(string url)
        {
            return GetHTMLByUrl(url, null);
        }

        /// <summary>
        /// 根据Url获取HTML
        /// </summary>
        /// <param name="url">Url</param>
        /// <param name="encoding"></param>
        /// <returns>HTML</returns>
        public static string GetHTMLByUrl(string url, Encoding encoding)
        {
            return GetHTMLByUrl(url, encoding, null);
        }

        /// <summary>
        /// 根据Url获取HTML
        /// </summary>
        /// <param name="url">Url</param>
        /// <param name="encoding"></param>
        /// <param name="nvc"></param>
        /// <returns>HTML</returns>
        public static string GetHTMLByUrl(string url, Encoding encoding, System.Collections.Specialized.NameValueCollection nvc)
        {
            Stream s = GetResponseStreamByUrl(url, nvc);

            StreamReader sr = null;
            if (encoding == null)
            {
                sr = new StreamReader(s);
            }
            else
            {
                sr = new StreamReader(s, encoding);
            }
            string content = sr.ReadToEnd();
            sr.Close();
            sr.Dispose();
            sr = null;

            return content;
        }

        /// <summary>
        /// 根绝请求URL 获取响应流
        /// </summary>
        /// <param name="url">URL</param>
        /// <returns>响应流</returns>
        public static Stream GetResponseStreamByUrl(string url)
        {
            return GetResponseStreamByUrl(url, null);
        }

        /// <summary>
        /// 根绝请求URL 获取响应流
        /// </summary>
        /// <param name="url">Url</param>
        /// <param name="nvc">自定义请求头</param>
        /// <returns>响应流</returns>
        public static Stream GetResponseStreamByUrl(string url, System.Collections.Specialized.NameValueCollection nvc)
        {
            System.Net.HttpWebRequest request = (System.Net.HttpWebRequest)System.Net.HttpWebRequest.Create(url);

            if (nvc != null)
            {
                string val = null;
                foreach (string key in nvc.AllKeys)
                {
                    val = nvc[key];
                    if (!string.IsNullOrEmpty(val))
                    {
                        request.Headers.Add(key, System.Web.HttpUtility.UrlEncode(val.Trim()));
                    }
                }
            }
            request.Method = "GET";

            return ((System.Net.HttpWebResponse)request.GetResponse()).GetResponseStream();
        }

        #endregion

        #region GUID 帮助
        /// <summary>
        /// 返回默认的NewGuid
        /// </summary>
        /// <returns>NewGuid</returns>
        public static string GetNewGuid()
        {
            return Guid.NewGuid().ToString();
        }
        #endregion

        #region 类型转换
        byte[] ObjectToByteArray(object o)
        {
            BinaryFormatter bf = new BinaryFormatter();
            MemoryStream ms = new MemoryStream();
            bf.Serialize(ms, o);
            return ms.ToArray();
        }

        object ByteArrayToObject(byte[] ba)
        {
            MemoryStream ms = new MemoryStream(ba);
            BinaryFormatter bf = new BinaryFormatter();
            return bf.Deserialize(ms);
        }
        #endregion

    }
}
