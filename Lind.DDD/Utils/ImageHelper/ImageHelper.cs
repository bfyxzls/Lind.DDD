using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.IO;
using System.Drawing.Imaging;

namespace Lind.DDD.Utils.ImageHelper
{
    /// <summary>
    /// 图片文本参数
    /// </summary>
    public class ImageWorldRequest
    {
        public ImageWorldRequest()
        {
            XPixel = 10;
            YPixel = 10;
            FontFamily = "黑体";
            FontSize = 24;
            FontColor = Brushes.Black;
        }
        /// <summary>
        /// 背景图路径－从这个路径读文件
        /// </summary>
        public string FilePath { get; set; }
        /// <summary>
        /// 每行的文本数组，每个元素是一段
        /// </summary>
        public string[] Worlds { get; set; }
        /// <summary>
        /// 背景左边像素数－相对
        /// </summary>
        public int XPixel { get; set; }
        /// <summary>
        /// 背景上边像素数－相对
        /// </summary>
        public int YPixel { get; set; }
        /// <summary>
        /// 文字大小
        /// </summary>
        public int FontSize { get; set; }
        /// <summary>
        /// 字体
        /// </summary>
        public string FontFamily { get; set; }
        /// <summary>
        /// 字体颜色
        /// </summary>
        public Brush FontColor { get; set; }
    }
    /// <summary>
    /// 图版操作类
    /// </summary>
    public class ImageHelper
    {
        /// <summary>
        /// 在背景图上生产文字
        /// </summary>
        /// <param name="request">参数</param>
        /// <returns>新的图片地址</returns>
        public static Image GeneratorWorldOfImage(ImageWorldRequest request)
        {
            if (!string.IsNullOrWhiteSpace(request.FilePath))
            {
                var buffer = WordToImage(request.FilePath, request.Worlds, request.FontSize, request.FontFamily, request.FontColor, request.XPixel, request.YPixel);
                var image = ByteArrayToImage(buffer);
                // image.Save(Guid.NewGuid().ToString() + ".jpg");
                return image;
            }
            return null;

        }
        /// <summary>
        /// 在图片上输出文字
        /// </summary>
        /// <param name="fileName">背景文件名</param>
        /// <param name="words">文本</param>
        /// <param name="fontSize">文本大小</param>
        /// <param name="family">字体</param>
        /// <param name="x">距左面像素</param>
        /// <param name="y">距右面像素</param>
        /// <returns></returns>
        public static byte[] WordToImage(string fileName, string[] words, int fontSize, string family, Brush fontColor, int x = 0, int y = 0)
        {

            Bitmap image = (Bitmap)Bitmap.FromFile(fileName);
            Graphics g = Graphics.FromImage(image);
            Font font = new Font(family, fontSize, GraphicsUnit.Pixel);

            int wordWidth = (int)g.MeasureString("中", font).Width;
            int wordHeight = (int)g.MeasureString("中", font).Height;
            int lineWidth = image.Width - wordWidth;
            int wordNum = (int)(lineWidth / wordWidth); //能写这么多汉字

            int rowY = y;
            foreach (var item in words)
            {
                for (int i = 0; i < item.Length; i += wordNum)
                {
                    if (i + wordNum > item.Length)
                        g.DrawString(item.Substring(i), font, fontColor, x, rowY);
                    else
                        g.DrawString(item.Substring(i, wordNum), font, fontColor, x, rowY);
                    rowY += wordHeight;
                }
            }
            MemoryStream mem = new MemoryStream();
            image.Save(mem, ImageFormat.Jpeg);
            g.Dispose();
            image.Dispose();
            return mem.ToArray();
        }

        /// <summary>
        /// 字节数组生成图片
        /// </summary>
        /// <param name="Bytes">字节数组</param>
        /// <returns>图片</returns>
        public static Image ByteArrayToImage(byte[] Bytes)
        {
            MemoryStream ms = new MemoryStream(Bytes);
            Image image = System.Drawing.Image.FromStream(ms);
            return image;
        }

        /// <summary>
        /// 把文字加到图像流中
        /// </summary>
        /// <param name="stream"></param>
        /// <param name="words"></param>
        /// <returns></returns>
        public static byte[] WordToImageStream(Stream stream, string words)
        {
            System.Drawing.Image image = System.Drawing.Image.FromStream(stream);
            Bitmap bmap = new Bitmap(image, image.Width, image.Height);
            Graphics g = Graphics.FromImage(bmap);

            //定义笔迹的样式
            SolidBrush drawBrush = new SolidBrush(Color.Red);
            Font drawFont = new Font("Arial", 5, FontStyle.Bold, GraphicsUnit.Millimeter);
            int xPos = bmap.Height - (bmap.Height - 25);
            int yPos = 3;

            //往图片上添加的内容
            g.DrawString(words, drawFont, drawBrush, xPos, yPos);

            //画边框
            //Brush brush = new SolidBrush(Color.Black);
            //Pen pen = new Pen(brush, 1);
            //g.DrawRectangle(pen, new Rectangle(0, 0, Math.Abs(image.Width), Math.Abs(image.Height)));

            //保存修改好的图片
            MemoryStream returnImg = new MemoryStream();
            bmap.Save(returnImg, ImageFormat.Jpeg);
            return returnImg.ToArray();
        }
        /// <summary>
        /// 获取文字图片
        /// </summary>
        /// <param name="fileName"></param>
        /// <param name="words"></param>
        /// <returns></returns>
        public static byte[] GetImageWord(string fileName, Char c)
        {
            int fontSize = 12;
            Bitmap image = new Bitmap(fontSize, fontSize, PixelFormat.Format32bppArgb);
            image.MakeTransparent(Color.White);
            Graphics g = Graphics.FromImage(image);
            Font font = new Font("黑体", fontSize, FontStyle.Regular, GraphicsUnit.Pixel);
            g.DrawString(c.ToString(), font, Brushes.White, 0f, 0f);
            MemoryStream mem = new MemoryStream();
            image.Save(mem, ImageFormat.Jpeg);
            g.Dispose();
            image.Dispose();
            return mem.ToArray();
        }


        /// <summary> 
        /// 将 Stream 转成 byte[] 
        /// </summary> 
        public static byte[] StreamToBytes(Stream stream)
        {
            // 设置当前流的位置为流的开始 
            stream.Seek(0, SeekOrigin.Begin);
            byte[] bytes = new byte[stream.Length];
            stream.Read(bytes, 0, bytes.Length);


            return bytes;
        }

        /// <summary> 
        /// 将 byte[] 转成 Stream 
        /// </summary> 
        public static Stream BytesToStream(byte[] bytes)
        {
            Stream stream = new MemoryStream(bytes);
            return stream;
        }




    }
}
