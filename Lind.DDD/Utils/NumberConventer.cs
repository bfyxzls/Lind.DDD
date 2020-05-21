using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Lind.DDD.Utils
{
    /// <summary>
    /// 阿拉伯数字转中文数字，中文数字转阿拉伯数字。
    /// by hcling97.Net 2007.03
    /// </summary>
    public class NumberConventer
    {
        private static string[] ArabinNum = { "0", "1", "2", "3", "4", "5", "6", "7", "8", "9" };
        private static string[] ChnNum = { "零", "一", "二", "三", "四", "五", "六", "七", "八", "九", "十", "百", "千", "万", "亿" };
        private static string[] Union = { "", "十", "百", "千" };
        public NumberConventer()
        {
            //
            // TODO: 在此处添加构造函数逻辑
            //
        }
        public static string ArabToChn(Decimal ArabNum)
        {
            string neg = string.Empty;
            neg = (ArabNum < 0) ? "负" : "";
            string result = string.Empty;
            string[] part = (ArabNum.ToString().Replace("-", string.Empty)).Split('.');
            Int64 temp = Convert.ToInt64(part[0]);
            Int64 epart = temp;
            string dotpart =
            (part.Length > 1) ? part[1] : string.Empty;
            if (part.Length > 1)
            {
                dotpart = GetDotPart(dotpart);
            }
            string tmp = string.Empty;
            string lasttemp = string.Empty;
            for (int i = 0; i <= ((epart.ToString().Length - 1) / 4); i++)
            {
                int thousand = 0;
                thousand = Convert.ToInt32(temp %
                10000);
                temp = temp / 10000;
                lasttemp = tmp;
                tmp = GetThousandPart(thousand);

                if (i == 0)
                {
                    result = tmp;
                    lasttemp = tmp;
                }
                if (i == 1)//返回的是万
                {
                    if (result == "零")
                    {
                        result = string.Empty;
                    }
                    result = tmp + "万" +
                    ((lasttemp.IndexOf("千") == -1 && lasttemp != "零") ? "零" : "") + result;
                }
                if (i == 2)//亿
                {
                    if (result.IndexOf("零万") != -1)
                    {

                        result = result.Replace("零万", string
                        .Empty);
                    }
                    result = tmp + "亿" +
                    ((lasttemp.IndexOf("千") == -1 && lasttemp != "零") ? "零" : "") + result;
                }
                if (i == 3)//万亿
                {
                    if (result.IndexOf("零亿") != -1)
                    {
                        result = result.Replace("零亿", "亿");
                    }
                    result = tmp + "万" +
                    ((lasttemp.IndexOf("千") == -1 && lasttemp != "零") ? "零" : "") + result;
                }
                if (i == 4)//亿亿
                {
                    if (result.IndexOf("零万") != -1)
                    {

                        result = result.Replace("零万", string
                        .Empty);
                    }
                    result = tmp + "亿" +
                    ((lasttemp.IndexOf("千") == -1 && lasttemp != "零") ? "零" : "") + result;
                }
            }
            result = neg + result + dotpart;
            return result;

        }
        /// <summary>
        /// 处理小数部分
        /// </summary>
        /// <param name="dotPart"></param>
        /// <returns></returns>
        private static string GetDotPart(string dotPart)
        {
            string result = "点";
            for (int i = 0; i < dotPart.Length; i++)
            {

                result += ChnNum[Convert.ToInt32(dotPart[
                i].ToString())];
            }
            for (int j = 0; j < result.Length; j++)
            //去除无效零或点
            {

                if (result[result.Length - j - 1].ToString() != "点" && result[result.Length - j - 1].ToString() != "零")
                {
                    break;
                }
                else
                {
                    result =
                    result.Substring(0, (result.Length - j - 1
                    ));
                }
            }
            return result;
        }
        /// <summary>
        /// 万位以下的分析
        /// </summary>
        /// <returns></returns>
        private static string GetThousandPart(int number)
        {
            if (number == 0)
            {
                return "零";
            }
            string result = string.Empty;
            bool lowZero = false;
            //记录低位有没有找到非零值，没找到置true
            bool befZero = false;
            //记录前一位是不是非零值，是0则置true
            int temp = number;
            int index = number.ToString().Length;
            for (int i = 0; i < index; i++)
            {
                int n = temp % 10;
                temp = temp / 10;
                if (i == 0) //起始位
                {
                    if (n == 0)
                    {
                        lowZero = true; //低位有0
                        befZero = true; //前位为0
                    }
                    else
                    {
                        result = ChnNum[n];
                    }
                }
                else
                {
                    if (n != 0)
                    {
                        if (ChnNum[n] == "一" && Union[i] == "十")
                            result = Union[i] + result;
                        else
                            result = ChnNum[n] + Union[i] + result;
                        lowZero = false;
                        befZero = false;
                    }
                    else
                    {
                        if (!lowZero)
                        {
                            if (!befZero)
                            //低位有数，且前位不为0，本位为0填零
                            //eg.5906
                            {
                                result = ChnNum[n] + result;
                                befZero = true;
                            }
                            else
                            //低位有数，且前位为0省略零eg. 5008
                            {
                            }
                        }
                        else //低位为0
                        {
                            if (!befZero)//理论上不存在eg 5080
                            {
                                result = ChnNum[n] + result;
                                befZero = true;
                            }
                            else //eg. 5000
                            {
                            }
                        }
                    }
                }
            }
            return result;
        }
        public Decimal ChnToArab(string ChnNum)
        {
            Decimal result = 0;
            string temp = ChnNum;
            bool neg = false;
            if (ChnNum.IndexOf("负") != -1)
            {
                neg = true;
                temp = temp.Replace("负", string.Empty);
            }
            string pre = string.Empty;
            string abo = string.Empty;
            temp = temp.Replace("点", ".");
            string[] part = temp.Split('.');
            pre = part[0];
            Decimal dotPart = 0;
            if (part.Length > 1)
            {
                abo = part[1];
                dotPart = this.GetArabDotPart(abo);
            }

            int yCount = 0;
            //"亿"的个数，有可能出现亿亿。
            //int yPos = 0;

            int index = 0;
            while (index < pre.Length)
            {
                if (pre.IndexOf("亿", index) != -1)
                {
                    yCount++;
                    //yPos = pre.IndexOf("亿",index);
                    index = pre.IndexOf("亿", index) + 1;
                }
                else
                {
                    break;
                }
            }
            if (yCount == 2)//亿亿
            {
                pre = pre.Replace("亿", ",");
                string[] sp = pre.Split(',');
                result =
                (neg ? -1 : 1) * ((this.HandlePart(sp[0]) * 10000000000000000) + (this.HandlePart(sp[1]) * 100000000) + this.HandlePart(sp[2])) + dotPart;
            }
            else
            {
                if (yCount == 1)
                {
                    pre = pre.Replace("亿", ",");
                    string[] sp = pre.Split(',');
                    result =
                    (neg ? -1 : 1) * ((this.HandlePart(sp[0]) * 100000000) + this.HandlePart(sp[1])) + dotPart;
                }
                else
                {
                    if (yCount == 0)
                    {
                        result =
                        (neg ? -1 : 1) * this.HandlePart(pre) + dotPart;
                    }
                }
            }
            return result;

        }
        /// <summary>
        /// 处理亿以下内容。
        /// </summary>
        /// <param name="num"></param>
        /// <returns></returns>
        private Decimal HandlePart(string num)
        {
            Decimal result = 0;
            string temp = num;
            temp = temp.Replace("万", ",");
            string[] part = temp.Split(',');
            for (int i = 0; i < part.Length; i++)
            {
                result +=
                Convert.ToDecimal(this.GetArabDotPart(part[part.Length - i - 1])) * Convert.ToDecimal((System.Math.Pow(10000, Convert.ToDouble(i))));
            }
            return result;
        }

        /// <summary>
        /// 取得阿拉伯数字小数部分。
        /// </summary>
        /// <returns></returns>
        private Decimal GetArabDotPart(string
        dotpart)
        {
            Decimal result = 0.00M;
            string spe = "0.";
            for (int i = 0; i < dotpart.Length; i++)
            {
                spe +=
                this.switchNum(dotpart[i].ToString()).ToString();
            }
            result = Convert.ToDecimal(spe);
            return result;
        }

        public int GetArabThousandPart(string
        number)
        {

            string ChnNumString = number;
            if (ChnNumString == "零")
            {
                return 0;
            }


            ChnNumString = ChnNumString.Replace("零", string.Empty);
            //去除所有的零
            int result = 0;
            int index = ChnNumString.IndexOf("千");
            if (index != -1)
            {
                result +=
                this.switchNum(ChnNumString.Substring(0
                , index)) * 1000;
                ChnNumString =
                ChnNumString.Remove(0, index + 1);
            }
            index = ChnNumString.IndexOf("百");
            if (index != -1)
            {
                result +=
                this.switchNum(ChnNumString.Substring(0
                , index)) * 100;
                ChnNumString =
                ChnNumString.Remove(0, index + 1);
            }
            index = ChnNumString.IndexOf("十");
            if (index != -1)
            {
                result +=
                this.switchNum(ChnNumString.Substring(0, index)) * 10;
                ChnNumString =
                ChnNumString.Remove(0, index + 1);
            }
            if (ChnNumString != string.Empty)
            {
                result += this.switchNum(ChnNumString);
            }
            return result;
        }
        /// <summary>
        /// 取得汉字对应的阿拉伯数字
        /// </summary>
        /// <param name="n"></param>
        /// <returns></returns>
        private int switchNum(string n)
        {
            switch (n)
            {
                case "零":
                    {
                        return 0;
                    }
                case "一":
                    {
                        return 1;
                    }
                case "二":
                    {
                        return 2;
                    }
                case "三":
                    {
                        return 3;
                    }
                case "四":
                    {
                        return 4;
                    }
                case "五":
                    {
                        return 5;
                    }
                case "六":
                    {
                        return 6;
                    }
                case "七":
                    {
                        return 7;
                    }
                case "八":
                    {
                        return 8;
                    }
                case "九":
                    {
                        return 9;
                    }
                default:
                    {
                        return -1;
                    }
            }
         }
    }

}
