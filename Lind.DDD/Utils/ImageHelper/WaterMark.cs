using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;

namespace Lind.DDD.Utils.ImageHelper
{
    /// <summary>
    /// 水印
    /// </summary>
    public class WaterMark
    {
        private int _width;
        private int _height;
        private string _fontFamily;
        private int _fontSize;
        private bool _adaptable;
        private FontStyle _fontStyle;
        private bool _shadow;
        private string _backgroundImage;
        private Color _bgColor;
        private int _left;
        private string _resultImage;
        private string _text;
        private int _top;
        private int _alpha;
        private int _red;
        private int _green;
        private int _blue;
        private long _quality;

        public WaterMark()
        {
            _width = 460;
            _height = 30;
            _fontFamily = "华文行楷";
            _fontSize = 20;
            _fontStyle = FontStyle.Regular;
            _adaptable = true;
            _shadow = false;
            _left = 0;
            _top = 0;
            _alpha = 255;
            _red = 0;
            _green = 0;
            _blue = 0;
            _backgroundImage = "";
            _quality = 100;
            _bgColor = Color.FromArgb(255, 229, 229, 229);

        }

        /// <summary>
        /// 字体
        /// </summary>
        public string FontFamily
        {
            set { this._fontFamily = value; }
        }


        /// <summary>
        /// 文字大小
        /// </summary>
        public int FontSize
        {
            set { this._fontSize = value; }
        }


        /// <summary>
        /// 文字风格
        /// </summary>
        public FontStyle FontStyle
        {
            get { return _fontStyle; }
            set { _fontStyle = value; }
        }


        /// <summary>
        /// 透明度0-255,255表示不透明
        /// </summary>
        public int Alpha
        {
            get { return _alpha; }
            set { _alpha = value; }
        }


        /// <summary>
        /// 水印文字是否使用阴影
        /// </summary>
        public bool Shadow
        {
            get { return _shadow; }
            set { _shadow = value; }
        }

        public int Red
        {
            get { return _red; }
            set { _red = value; }
        }

        public int Green
        {
            get { return _green; }
            set { _green = value; }
        }

        public int Blue
        {
            get { return _blue; }
            set { _blue = value; }
        }

        /// <summary>
        /// 底图
        /// </summary>
        public string BackgroundImage
        {
            set { this._backgroundImage = value; }
        }

        /// <summary>
        /// 水印文字的左边距
        /// </summary>
        public int Left
        {
            set { this._left = value; }
        }



        /// <summary>
        /// 水印文字的顶边距
        /// </summary>
        public int Top
        {
            set { this._top = value; }
        }


        /// <summary>
        /// 生成后的图片
        /// </summary>
        public string ResultImage
        {
            set { this._resultImage = value; }
        }


        /// <summary>
        /// 水印文本
        /// </summary>
        public string Text
        {
            set { this._text = value; }
        }



        /// <summary>
        /// 生成图片的宽度
        /// </summary>
        public int Width
        {
            get { return _width; }
            set { _width = value; }
        }


        /// <summary>
        /// 生成图片的高度
        /// </summary>
        public int Height
        {
            get { return _height; }
            set { _height = value; }
        }


        /// <summary>
        /// 若文字太大，是否根据背景图来调整文字大小，默认为适应
        /// </summary>
        public bool Adaptable
        {
            get { return _adaptable; }
            set { _adaptable = value; }
        }

        public Color BgColor
        {
            get { return _bgColor; }
            set { _bgColor = value; }
        }


        /// <summary>
        /// 输出图片质量，质量范围0-100,类型为long
        /// </summary>
        public long Quality
        {
            get { return _quality; }
            set { _quality = value; }
        }


        /// <summary>
        /// 立即生成水印效果图
        /// </summary>
        /// <returns>生成成功返回true,否则返回false</returns>
        public bool Create()
        {
            try
            {
                Bitmap bitmap;
                Graphics g;

                //使用纯背景色
                if (this._backgroundImage.Trim() == "")
                {
                    bitmap = new Bitmap(this._width, this._height, PixelFormat.Format64bppArgb);
                    g = Graphics.FromImage(bitmap);
                    g.Clear(this._bgColor);
                }
                else
                {
                    bitmap = new Bitmap(Image.FromFile(this._backgroundImage));
                    g = Graphics.FromImage(bitmap);
                }
                g.SmoothingMode = SmoothingMode.HighQuality;
                g.InterpolationMode = InterpolationMode.HighQualityBicubic;
                g.CompositingQuality = CompositingQuality.HighQuality;

                Font f = new Font(_fontFamily, _fontSize, _fontStyle);
                SizeF size = g.MeasureString(_text, f);

                // 调整文字大小直到能适应图片尺寸
                while (_adaptable == true && size.Width > bitmap.Width)
                {
                    _fontSize--;
                    f = new Font(_fontFamily, _fontSize, _fontStyle);
                    size = g.MeasureString(_text, f);
                }

                Brush b = new SolidBrush(Color.FromArgb(_alpha, _red, _green, _blue));
                StringFormat StrFormat = new StringFormat();
                StrFormat.Alignment = StringAlignment.Near;

                if (this._shadow)
                {
                    Brush b2 = new SolidBrush(Color.FromArgb(90, 0, 0, 0));
                    g.DrawString(_text, f, b2, _left + 2, _top + 1);
                }
                g.DrawString(_text, f, b, new PointF(_left, _top), StrFormat);

                bitmap.Save(this._resultImage, ImageFormat.Jpeg);
                bitmap.Dispose();
                g.Dispose();
                return true;
            }
            catch
            {
                return false;
            }
        }
    }


}