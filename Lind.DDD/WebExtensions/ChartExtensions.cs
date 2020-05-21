using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Helpers;

namespace Lind.DDD.WebExtensions
{
    /// <summary>
    /// 图表相关的操作
    /// </summary>
    public class ChartExtensions
    {

        /// <summary>
        /// 生成图表
        /// 使用：<img src="/controller/chart"></img>
        /// </summary>
        /// <param name="type"></param>
        public static void Chart(string title, string type)
        {
            new Chart(width: 600, height: 400, theme: ChartTheme.Green)
               .AddTitle(title)
               .AddSeries(name: "Employee"
               , chartType: type//Column,Pie,Range,Stock,Point,Area
               , xValue: new[] { "一月份", "二月份", "三月份", "四月份", "五月份", "六月份", "七月份", "八月份", "九月份" }
               , yValues: new[] { "2", "6", "4", "5", "3", "4", "9", "2", "5" })
               .Write();
        }

        /// <summary>
        /// 通过数据生成图表
        /// </summary>
        /// <param name="type"></param>
        public static void DataChart<T>(string title, string type, IEnumerable<T> totalList, string xFiled)
        {

            new Chart(width: 600, height: 400, theme: ChartTheme.Green)
               .AddTitle(title)
               .AddSeries(name: "Employee"
               , chartType: type)//Column,Pie,Range,Stock,Point,Area
               .DataBindTable(totalList, xFiled)
               .Write();
        }
    }
}
