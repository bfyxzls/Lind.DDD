using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using System.Threading;
using System.Text.RegularExpressions;
using System.Collections.Concurrent;

namespace Lind.DDD.UnitTest
{
    public class Prizes
    {
        public int Id { get; set; }
        public string Name { get; set; }
        /// <summary>
        /// 概率分母，分子都是1
        /// </summary>
        public int Probability { get; set; }
        public int Level { get; set; }
        public int Stock { get; set; }
        public decimal Price { get; set; }
    }



    [TestClass]
    public class PrizeTest
    {

        [TestMethod]
        public void gaobeishu()
        {
            Console.WriteLine(ArrayMultiple(10, 5, 2, 3));
        }

        #region 属性
        List<Prizes> pl = new List<Prizes>();
        Dictionary<int, Tuple<int, int>> basic = new Dictionary<int, Tuple<int, int>>();
        double minTotal = 0;
        int minBeishu = 0;
        public PrizeTest()
        {
            pl.Add(new Prizes { Id = 1, Name = "房", Probability = 10000, Level = 0, Stock = 10, Price = 3000000 });
            pl.Add(new Prizes { Id = 2, Name = "车", Probability = 1000, Level = 0, Stock = 100, Price = 300000 });
            pl.Add(new Prizes { Id = 3, Name = "电脑", Probability = 500, Level = 0, Stock = 500, Price = 5000 });
            pl.Add(new Prizes { Id = 4, Name = "手机", Probability = 300, Level = 0, Stock = 500, Price = 2200 });
            pl.Add(new Prizes { Id = 5, Name = "手表", Probability = 100, Level = 0, Stock = 800, Price = 800 });
            pl.Add(new Prizes { Id = 7, Name = "挂件", Probability = 10, Level = 0, Stock = 1000, Price = 5 });
            pl.Add(new Prizes { Id = 8, Name = "红包", Probability = 4, Level = 0, Stock = 1000, Price = 1 });

            pl.ForEach(i =>
            {
                var arr = Fraction(1 / i.Probability).Split(new char[] { '/' }, StringSplitOptions.RemoveEmptyEntries);
                basic.Add(i.Id, new Tuple<int, int>(1, i.Probability));
                minTotal += (double)1 / i.Probability;
            });
            if (minTotal > 1)
            {
                throw new ArgumentException("请调整奖项的分配比例，所有比例加在一起不能大于１");
            }
            pl.Add(new Prizes { Id = 9, Name = "谢谢参与", Probability = 1, Level = 9, Stock = 10000, Price = 0 });

            minBeishu = ArrayMultiple(basic.Values.Select(i => i.Item2).ToArray());
        }


        #endregion
        #region 方法１
        [TestMethod]
        public void Init()
        {
            pl.ForEach(i =>
            {
                //奖品
                RedisManager.Instance.GetDatabase().HashSet("Prizes", i.Id, Utils.SerializeMemoryHelper.SerializeToJson(i));
                //库存
                RedisManager.Instance.GetDatabase().StringSet("Prizes_" + i.Id, i.Stock);
            });

        }
        static List<Prizes> list = new List<Prizes>();
        [TestMethod]
        public void DrawTest()
        {
            foreach (var item in RedisManager.Instance.GetDatabase().HashGetAll("Prizes"))
            {
                list.Add(Utils.SerializeMemoryHelper.DeserializeFromJson<Prizes>(item.Value));
            }

            int mockPeople = 50;
            ConcurrentDictionary<string, int> resultDis = new ConcurrentDictionary<string, int>();
            resultDis.TryAdd("房", 0);
            resultDis.TryAdd("车", 0);
            resultDis.TryAdd("电脑", 0);
            resultDis.TryAdd("手机", 0);
            resultDis.TryAdd("手表", 0);
            resultDis.TryAdd("挂件", 0);
            resultDis.TryAdd("红包", 0);
            Parallel.ForEach(Enumerable.Range(1, mockPeople), i =>
            {
                var result = Draw(24);
                if (result > 0)
                {
                    switch (result)
                    {
                        case 1:
                            resultDis["房"] += 1;
                            break;
                        case 2:
                            resultDis["车"] += 1;
                            break;
                        case 3:
                            resultDis["电脑"] += 1;
                            break;
                        case 4:
                            resultDis["手机"] += 1;
                            break;
                        case 5:
                            resultDis["手表"] += 1;
                            break;
                        case 6:
                            resultDis["挂件"] += 1;
                            break;
                        case 7:
                            resultDis["红包"] += 1;
                            break;
                    }

                    var entity = RedisManager.Instance.GetDatabase().HashGet("Prizes", result);
                    Console.WriteLine("恭喜您{0}号会员，你中的奖品为：{1}", i, Utils.SerializeMemoryHelper.DeserializeFromJson<Prizes>(entity).Name);
                }
                else
                    Console.WriteLine(i + "谢谢参与！");
            });
            foreach (var item in resultDis)
            {
                Console.WriteLine("key:{0},value:{1}", item.Key, item.Value);
            }
        }
        #endregion

        #region 方法２
        [TestMethod]
        public void Init2()
        {
            RedisManager.Instance.GetDatabase().KeyDelete("PrizesPoolList");
            RedisManager.Instance.GetDatabase().KeyDelete("Prize_Info");
            RedisManager.Instance.GetDatabase().KeyDelete("Prize_Inventory");
            Init();

            List<int> oldList = new List<int>();
            pl.ForEach(j =>
            {

                var newList = GenerateRandom(minBeishu, (int)(minBeishu / j.Probability), 1, oldList);
                oldList.AddRange(newList);
                newList.ForEach(i =>
                {
                    RedisManager.Instance.GetDatabase().HashSet("PrizesPoolList", i, j.Id);
                });
            });

        }
        [TestMethod]
        public void DrawTest2()
        {
            int mockPeople = 100;


            GenerateRandom(minBeishu, mockPeople).ForEach(num =>
            {
                if (RedisManager.Instance.GetDatabase().LockTake("zzlKey", "prize", TimeSpan.FromMilliseconds(1)))
                {
                    try
                    {
                        var prize = RedisManager.Instance.GetDatabase().HashGet("PrizesPoolList", num.ToString());
                        if (prize.HasValue)
                        {
                            var entity = Utils.SerializeMemoryHelper.DeserializeFromJson<Prizes>(RedisManager.Instance.GetDatabase().HashGet("Prizes", prize));
                            if (entity != null && entity.Stock > 0)
                            {
                                RedisManager.Instance.GetDatabase().StringDecrement("Prizes_" + prize, 1);
                                Console.WriteLine("您抽的号是:{0}，中的奖是:{1}", num, entity.Name);
                            }
                            else
                            {
                                Console.WriteLine("谢谢参与");
                            }
                        }
                    }
                    catch (Exception)
                    {

                        throw;
                    }
                    finally
                    {
                        RedisManager.Instance.GetDatabase().LockRelease("zzlKey", "prize");
                    }
                }


            });

        }
        #endregion

        #region Private Methods

        static int Divisor(int a, int b)//最大公约数 
        {
            if (a < b) { a = a + b; b = a - b; a = a - b; }
            return (a % b == 0) ? b : Divisor(a % b, b);
        }

        static int Multiple(int a, int b)//最小公倍数 
        {
            return a * b / Divisor(a, b);
        }
        /// <summary>
        /// 数组最小公倍数
        /// </summary>
        /// <param name="b"></param>
        /// <returns></returns>
        static int ArrayMultiple(params int[] b)
        {
            int am = 1;
            for (int i = 0; i < b.Length; i++)
            {
                am = Multiple(am, b[i]);
            }
            return am;
        }


        /// <summary>
        /// 产生指定数量的随机数
        /// </summary>
        /// <param name="iMax"></param>
        /// <param name="iNum"></param>
        /// <returns></returns>
        private List<int> GenerateRandom(int iMax, int iNum, int iMin = 1, List<int> exceptList = null)
        {
            if (exceptList != null && exceptList.Any())
            {
                return Enumerable.Range(iMin, iMax).Except(exceptList).OrderByNewId().Take(iNum).ToList();
            }
            else
            {
                long lTick = DateTime.Now.Ticks + Thread.CurrentThread.ManagedThreadId;
                List<int> lstRet = new List<int>();
                for (int i = 1; i < iNum + 1; i++)
                {
                    Random ran = new Random((int)lTick * i);
                    if (iMin > iMax) iMin = iMax;
                    int iTmp = ran.Next(iMin, iMax);
                    lstRet.Add(iTmp);
                    lTick += (new Random((int)lTick).Next(978));
                }
                return lstRet;
            }
        }
        /// <summary>
        /// 返回小数对应的分数
        /// </summary>
        /// <param name="d"></param>
        /// <returns></returns>
        private string Fraction(double d)
        {
            string dn = Regex.Match(d.ToString(), @"(?<=\.)\d+").Value;
            int tn = 1;
            for (int i = dn.Length; i > 0; i--)
            {
                tn *= 10;
            }
            return (d * tn).ToString() + "/" + tn.ToString();
        }
        /// <summary>
        /// 抽奖方法：遍历
        /// </summary>
        /// <param name="num"></param>
        /// <returns></returns>
        private int Draw(int num)
        {

            if (RedisManager.Instance.GetDatabase().LockTake("zzlKey", "prize", TimeSpan.FromMilliseconds(100)))
            {
                try
                {
                    foreach (var item in list.OrderBy(i => i.Level))
                    {
                        var frac = Fraction(item.Probability).Split(new char[] { '/' }, StringSplitOptions.RemoveEmptyEntries);
                        var sub = Convert.ToInt32(frac[0]);
                        var max = Convert.ToInt32(frac[1]);
                        var min = 1;
                        if (max < num)//1 24
                        {
                            var old = max / 2.0;
                            max = num + (int)Math.Ceiling(old);
                            min = num - (int)Math.Floor(old);
                        }
                        if (GenerateRandom(max, sub, min).Contains(num))
                        {
                            RedisManager.Instance.GetDatabase().StringDecrement("Prizes_" + item.Id, 1);
                            return item.Id;
                        }
                    }
                }
                finally
                {

                    RedisManager.Instance.GetDatabase().LockRelease("zzlKey", "prize");
                }

            }
            return 0;
        }
        #endregion


    }
}
