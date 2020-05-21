using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using StackExchange.Redis;
using Lind.DDD.RedisUtils;
using System.Linq;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using Lind.DDD.Utils;
namespace Lind.DDD.UnitTest
{
    class VoteModel
    {
        public int UserID { get; set; }
        public string UserName { get; set; }
        public int ProjectID { get; set; }
        public string ProjectName { get; set; }
        public DateTime CreateTime { get; set; }
        public override bool Equals(object obj)
        {
            return this.UserID == ((VoteModel)obj).UserID;
        }
    }
    [TestClass]
    public class Redis
    {
        static object lockObj = new object();
        static ConnectionMultiplexer conn = ConnectionMultiplexer.Connect("127.0.0.1:6379");
        RedisValue[] getCity()
        {
            return conn.GetDatabase().SetMembers("testlong");
        }
        RedisValue[] getCityAsync()
        {
            return conn.GetDatabase().SetMembersAsync("testlong").Result;
        }

        ITransaction trans = Lind.DDD.RedisManager.Instance.GetDatabase().CreateTransaction();

        /// <summary>
        /// 多redis语句的事务
        /// </summary>
        [TestMethod]
        public void TransTest()
        {
            //测试结果－事实没有保证一致性
            string custKey = "lockRedis";
            trans.AddCondition(Condition.HashNotExists(custKey, "UniqueID"));
            trans.HashSetAsync(custKey, "UniqueID", Guid.NewGuid().ToString());
            var task = trans.StringSetAsync("testhashset108", DateTime.Now.ToString()).ContinueWith(t =>
            {
                trans.StringSetAsync("testhashset109", DateTime.Now.ToString());

                //  var a1 = trans.HashGetAllAsync("testhashset16");
            });
            trans.Execute();
        }

        /// <summary>
        /// 同步代码测试异步
        /// </summary>
        [TestMethod]
        public void TransTest2()
        {
            //测试结果－事实没有保证一致性
            AsyncTaskManager.RunSync(() => trans.StringSetAsync("testhashset21", DateTime.Now.ToString()));
            AsyncTaskManager.RunSync(() => trans.StringSetAsync("testhashset22", DateTime.Now.ToString()));
            var result = AsyncTaskManager.RunSync<RedisValue>(() =>
              {
                  return trans.StringGetAsync("testhashset16");
              });
            trans.Execute();

        }
        [TestMethod]
        public void TW()
        {
            //连接TW服务器
            ConfigurationOptions sentinelConfig = new ConfigurationOptions();
            sentinelConfig.EndPoints.Add("192.168.1.190:22121");
            sentinelConfig.Proxy = Proxy.Twemproxy;
            ConnectionMultiplexer conn = ConnectionMultiplexer.Connect(sentinelConfig);
            conn.GetDatabase().StringSet("zzltest", "test");
        }

        [TestMethod]
        public void testRedis()
        {
            Console.WriteLine("Redis sync & async testing!");
            Stopwatch sw = new Stopwatch();
            sw.Restart();
            for (int i = 0; i < 100000; i++)
            {
                conn.GetDatabase().SetAddAsync("async_testlong", i.ToString());
            }
            sw.Stop();
            long timer2 = sw.ElapsedMilliseconds;

            sw.Restart();
            for (int i = 0; i < 100000; i++)
            {
                conn.GetDatabase().SetAdd("testlong", i.ToString());
            }
            sw.Stop();
            long timer1 = sw.ElapsedMilliseconds;

            sw.Restart();
            var a2 = getCityAsync();
            sw.Stop();
            long timer4 = sw.ElapsedMilliseconds;

            sw.Restart();
            var a1 = getCity();
            sw.Stop();
            long timer3 = sw.ElapsedMilliseconds;


            Console.WriteLine("SetAdd Timer:" + timer1 + "\r\nSetAddAsync async timer:" + timer2 + "\r\nSetMembers Timer:" + timer3 + "\r\nSetMembersAsync Timer:" + timer4);

        }

        [TestMethod]
        public void AppendValue()
        {

            //连接sentinel服务器
            ConfigurationOptions sentinelConfig = new ConfigurationOptions();
            sentinelConfig.ServiceName = "master1";
            sentinelConfig.EndPoints.Add("192.168.2.3", 26379);
            sentinelConfig.EndPoints.Add("192.168.2.3", 26380);
            sentinelConfig.TieBreaker = "";//这行在sentinel模式必须加上
            sentinelConfig.CommandMap = CommandMap.Sentinel;

            // Need Version 3.0 for the INFO command?
            sentinelConfig.DefaultVersion = new Version(3, 0);


            ConnectionMultiplexer conn = ConnectionMultiplexer.Connect(sentinelConfig);




            ISubscriber sub = conn.GetSubscriber();
            sub.Subscribe("+switch-master", (o, i) =>
            {
                Console.WriteLine(o + "-hello pub/sub-" + i);
                var arr = i.ToString().Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                var oldServer = arr[0];
                var newServer = arr[1];
                var conf = "/usr/local/twemporxy/conf/nutcracker.yml";
                lock (lockObj)
                {
                    var result = ReadTxt(conf);
                    result = result.Replace(oldServer, newServer);
                    WriteTxt(conf, result);
                }
            });

            Console.ReadKey();

        }

        [TestMethod]
        public void Redis_Async()
        {
            List<Action> actionList = new List<Action>();
            actionList.Add(() =>
            {
                for (int i = 0; i < 100; i++)
                {
                    RedisManager.Instance.GetDatabase().SetAdd("test01", DateTime.Now.ToString());
                    Thread.Sleep(100);
                    Console.WriteLine("test011" + i);
                }
            });
            actionList.Add(() =>
            {
                for (int i = 0; i < 100; i++)
                {
                    RedisManager.Instance.GetDatabase().SetAdd("test02", DateTime.Now.ToString());
                    Thread.Sleep(10);
                    Console.WriteLine("test012" + i);
                }
            });
            Parallel.Invoke(actionList.ToArray());
        }

        static string ReadTxt(string fileName)
        {
            string msg = string.Empty;
            using (FileStream fs = new FileStream(fileName, FileMode.OpenOrCreate))
            {

                using (StreamReader sw = new StreamReader(fs, Encoding.UTF8))
                {
                    msg = sw.ReadToEnd();
                }

            }
            return msg;
        }

        static void WriteTxt(string fileName, string obj)
        {

            using (FileStream fs = new FileStream(fileName, FileMode.Create))
            {

                using (StreamWriter sw = new StreamWriter(fs, Encoding.UTF8))
                {
                    sw.Write(obj);
                }
            }

        }


        [TestMethod]
        public void Redis_ModelList()
        {

            //   RedisManager.Instance.GetDatabase().SetAdd("ModelList", Utils.SerializeMemoryHelper.SerializeToJson(new People(1, "zzl", 1)));
            //   RedisManager.Instance.GetDatabase().SetAdd("ModelList", Utils.SerializeMemoryHelper.SerializeToJson(new People(2, "zzl2", 2)));
            var list = new List<People>();
            list.Add(new People(1, "zzl", 1));
            list.Add(new People(2, "zzl2", 2));


            string old = Lind.DDD.Utils.SerializeMemoryHelper.SerializeToJson(list);
            var news = Lind.DDD.Utils.SerializeMemoryHelper.DeserializeFromJson<List<People>>(old);


            RedisManager.Instance.GetDatabase().Set("ModelEntity", list);
            var result = RedisManager.Instance.GetDatabase().Get<List<People>>("ModelEntity");
        }

        [TestMethod]
        public void VoteBigData()
        {
            for (var i = 0; i < 1000000; i++)
            {
                var entity = new VoteModel
                {
                    UserID = i,
                    ProjectID = 1,
                    ProjectName = "tel",
                    UserName = "zzl" + i,
                    CreateTime = DateTime.Now
                };
                RedisManager.Instance.GetDatabase().HashSet("VoteList", entity.UserID, Utils.SerializeMemoryHelper.SerializeToJson(entity));
                //空间换时间的索引UserName
                RedisManager.Instance.GetDatabase().HashSet("VoteList_UserName", entity.UserName, entity.UserID);

            }
        }


        [TestMethod]
        public void FindBigData()
        {
            Stopwatch sw = new Stopwatch();
            sw.Start();
            var name = RedisManager.Instance.GetDatabase().HashGet("VoteList_UserName", "zzl15");//找到用户ID
            if (name.HasValue)
            {
                var val = RedisManager.Instance.GetDatabase().HashGet("VoteList", name);//找到用户实体
                Console.WriteLine("name={0},value={1}", name, val);
            }
            else
            {
                Console.WriteLine("没有发现这个Key");
            }
            sw.Stop();
            Console.WriteLine("查询需要的时间：" + sw.ElapsedMilliseconds + "ms");
        }


        [TestMethod]
        public void Trans()
        {
            Parallel.ForEach(Enumerable.Range(1, 100000), (o) =>
            {
                #region Redis并发锁机制

                //var val = RedisManager.Instance.GetDatabase().StringGet("testTrans4");
                //var transaction = RedisManager.Instance.GetDatabase().CreateTransaction();
                //transaction.AddCondition(Condition.StringEqual("testTrans4", val));
                //商品　库存
                //人　抽签　商品数减1
                //   transaction.StringIncrementAsync("testTrans4", 1);
                //  transaction.Execute();

                RedisManager.Instance.GetDatabase().StringIncrement("testTrans22", 1);//result:100
                #endregion
            });

        }

        /// <summary>
        /// 分布锁
        /// </summary>
        [TestMethod]
        public void Locks()
        {
            Stopwatch sw = new Stopwatch();
            sw.Restart();
            var db = RedisManager.Instance.GetDatabase();
            RedisValue token = Environment.MachineName;

            db.StringSet("product1", 10);//库存
            while (true)
            {

                //锁超时为１秒，程序执行时候大于１秒时锁失效
                if (db.LockTake("zzlKey", token, new TimeSpan(0, 0, 1)))
                {
                    try
                    {
                        var val = Convert.ToInt32(db.StringGet("product1"));
                        if (val == 0)
                        {
                            Console.WriteLine("没了");
                            break;
                        }
                        Console.WriteLine("抽中:" + val);
                        db.StringDecrement("product1", 1);
                        Thread.Sleep(100);//执行需要100毫秒
                    }
                    finally
                    {
                        db.LockRelease("zzlKey", token);
                    }
                }
            }
            sw.Stop();
            Console.WriteLine("result=" + sw.ElapsedMilliseconds);

        }

        /// <summary>
        /// 多线程随机数
        /// </summary>
        [TestMethod]
        public void TaskRandom()
        {
            Enumerable.Range(1, 10).ToList().ForEach(i =>
            {
                Console.WriteLine(new Random().Next(1, 2));
            });
        }

        [TestMethod]
        public void MulTaskRandom()
        {
            var sw = new Stopwatch();
            sw.Restart();
            Parallel.ForEach(Enumerable.Range(1, 10), i =>
                     {
                         int second = DateTime.Now.Second;
                         int msecond = DateTime.Now.Millisecond;
                         var seed = Thread.CurrentThread.ManagedThreadId ^ second ^ msecond;
                         Console.WriteLine("id:{0},second:{1},msecond:{2},number:{3}"
                             , Thread.CurrentThread.ManagedThreadId
                             , second
                             , msecond
                             , new Random(seed).Next(1, 10000));

                     });

            sw.Stop();
            Console.WriteLine(sw.ElapsedMilliseconds);

        }
        [TestMethod]
        public void MulInvokeRandom()
        {
            var sw = new Stopwatch();
            sw.Restart();
            List<Action> ar = new List<Action>();
            for (var i = 0; i < 10; i++)
            {
                ar.Add(() =>
                {
                    int second = DateTime.Now.Second;
                    int msecond = DateTime.Now.Millisecond;
                    var seed = Thread.CurrentThread.ManagedThreadId ^ second ^ msecond;
                    Console.WriteLine("id:{0},second:{1},msecond:{2},number:{3}"
                        , Thread.CurrentThread.ManagedThreadId
                        , second
                        , msecond
                        , new Random(seed).Next(1, 10000));
                });
            }
            Parallel.Invoke(ar.ToArray());

            sw.Stop();
            Console.WriteLine(sw.ElapsedMilliseconds);

        }
        [TestMethod]
        public void ListRandom()
        {
            Console.WriteLine("0.0001=" + Fraction(0.0001));
            Console.WriteLine("0.09=" + Fraction(0.09));
            Console.WriteLine("0.002=" + Fraction(0.02));
            GenerateRandom(10000, 10).ForEach(i => Console.WriteLine(i));
        }

        static string Fraction(double d)
        {
            string dn = Regex.Match(d.ToString(), @"(?<=\.)\d+").Value;
            int tn = 1;
            for (int i = dn.Length; i > 0; i--)
            {
                tn *= 10;
            }
            return (d * tn).ToString() + "/" + tn.ToString();
        }


        static List<int> GenerateRandom(int iMax, int iNum)
        {
            long lTick = DateTime.Now.Ticks;
            List<int> lstRet = new List<int>();
            for (int i = 0; i < iNum; i++)
            {
                Random ran = new Random((int)lTick * i);
                int iTmp = ran.Next(iMax);
                lstRet.Add(iTmp);
                lTick += (new Random((int)lTick).Next(978));
            }
            return lstRet;
        }

    }


}
