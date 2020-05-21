using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Drawing.Imaging;

namespace Lind.DDD.HttpExtensions.HttpResponse
{
    /// <summary>
    /// 生成验证码对象
    /// </summary>
    public class ValidateCode
    {
        ///<summary>
        ///返回一个登录随机数
        ///<remarks></remarks> 
        ///<param name="minValue">随机数下限。</param>
        ///<param name="maxValue">随机数上限。</param>
        ///</summary>
        public static int LoginRandNum(int minValue, int maxValue)
        {
            try
            {
                Random ro = new Random();
                return ro.Next(minValue, maxValue);
            }
            catch
            {
                return 8888;
            }
        }

        /// 生成验证码图像
        /// </summary>
        /// <param name="VNum"></param>
        /// <returns></returns>
        public static byte[] CreatePicCode(string VNum)
        {
            int gWidth = (int)(VNum.Length * 17);// Gwidth为图片宽度,根据字符长度自动更改图片宽度
            int gHeight = 20;
            System.Drawing.Bitmap Img = new System.Drawing.Bitmap(gWidth, gHeight);
            Graphics g;
            MemoryStream ms = new MemoryStream();
            string tmpstr = "";
            for (int mm = 0; mm < VNum.Length; mm++)
            {
                tmpstr += VNum[mm] + " ";
            }
            VNum = tmpstr;
            g = Graphics.FromImage(Img);
            SolidBrush drawBrushNew = new SolidBrush(Color.White);
            g.FillRectangle(drawBrushNew, 0, 0, Img.Width, Img.Height);
            // Create font and brush.	
            FontStyle style = FontStyle.Bold | FontStyle.Italic;
            Font drawFont = new Font("Arial", 12, style);
            SolidBrush drawBrush = new SolidBrush(Color.Red);
            Color clr = Color.Red;
            Pen p = new Pen(clr, LoginRandNum(1, 4));
            int x1 = LoginRandNum(1, gWidth);
            int y1 = LoginRandNum(1, gHeight);
            int x2 = x1 + LoginRandNum(15, gWidth - 15);
            int y2 = y1 + LoginRandNum(0, gHeight);
            PointF drawPoint1 = new PointF(x1, y1);
            PointF drawPoint2 = new PointF(x2, y2);

            for (int i = 0; i < Convert.ToInt32(LoginRandNum(300, 415)); i++)
            {
                // 生成一个随机宽度
                clr = Color.FromArgb(LoginRandNum(100, 255), LoginRandNum(51, 255), LoginRandNum(11, 255));
                p.Color = clr;
                p.Width = LoginRandNum(1, 4);

                x1 = LoginRandNum(1, gWidth);
                y1 = LoginRandNum(1, gHeight);
                x2 = x1 + LoginRandNum(15, gWidth - 15);
                y2 = y1 + LoginRandNum(0, gHeight);
                drawPoint1.X = x1;
                drawPoint1.Y = y1;
                drawPoint2.X = x2;
                drawPoint2.Y = y2;
                g.DrawLine(p, drawPoint1, drawPoint2);

            }
            p.Dispose();
            PointF drawPoint = new PointF(3, 3);
            g.DrawString(VNum, drawFont, drawBrush, drawPoint); //在矩形内绘制字串（字串，字体，画笔颜色，左上x.左上y）
            Img.Save(ms, ImageFormat.Jpeg);
            return ms.ToArray();
        }
        /// <summary>
        /// 随机生成验证码
        /// </summary>
        /// <param name="Numberlength"></param>
        /// <returns></returns>
        public static string MakeVerifyCode(int Numberlength)
        {
            string strTemp = "";
            string randomchars = "abcdefghijklmnpqrstuvwxyz123456789";
            randomchars = randomchars.ToUpper();
            int iRandNum;
            Random rnd = new Random();
            for (int i = 0; i < Numberlength; i++)
            {
                iRandNum = rnd.Next(randomchars.Length);
                strTemp += randomchars[iRandNum];
            }
            return strTemp;
        }
    }
}
