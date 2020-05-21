using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading;
using System.Threading.Tasks;
using System.Linq;
using Newtonsoft.Json;
namespace Lind.DDD.UnitTest
{
    public class Classes { }
    public class Student : Classes, IComparable<Student>
    {
        public string Name { get; set; }
        public int? Score { get; set; }

        #region IComparable<Student> 成员

        public int CompareTo(Student other)
        {
            int result = 0;

            if (this.Score > other.Score)
                result = 1;
            else if (this.Score == other.Score)
                result = 0;
            else
                result = -1;

            return result;
        }

        #endregion
    }
    public class Student1 : Classes
    {
        public string Name { get; set; }
        public int Score { get; set; }
    }
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void TimeCompare()
        {
            var dt = DateTime.Parse("11:00");
            Console.WriteLine(dt);
        }

        [TestMethod]
        public void Json()
        {
            //整形为null，把为null的字段过滤
            var jSetting = new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore };
            var json = JsonConvert.SerializeObject(new Student { Name = "zzl" }, Formatting.Indented, jSetting);

            var str = JsonConvert.DeserializeObject<Student1>(json);
        }
        [TestMethod]
        public void TimeStamp()
        {
            var a2 = (DateTime.Now - new DateTime(1970, 1, 1)).TotalMinutes;

            var a3 = new DateTime(1970, 1, 1);

            Console.WriteLine(a2);
        }
        [TestMethod]
        public void Json_Dic()
        {
            Dictionary<string, object> dic = new Dictionary<string, object> { { "name", "zzl" }, { "age", "33" } };
            string url = dic.ToUrl();
            IDictionary<string, string> adic = DictionaryExtensions.FromUrl("a=5&b=6&c=7&d=zzl");
            int dd = 1;
        }

        static Classes GetStudent()
        {
            return new Student();
        }

        static object lockObj = new object();
        [TestMethod]
        public void MultiTask()
        {

            var mod = 10 ^ 5;
            var mod2 = 10 % 5;

            var old = DateTime.Now.ToString("yyyyMMdd");
            Console.WriteLine(DateTime.Now);
            int a = 0;
            while (a < 10000)
            {
                Task();
                a++;
            }
            Console.WriteLine(DateTime.Now);
        }

        [TestMethod]
        public void MultiTaskLock()
        {
            Console.WriteLine(DateTime.Now);
            int a = 0;
            while (a < 10000)
            {
                TaskLock();
                a++;
            }
            Console.WriteLine(DateTime.Now);
        }

        public void Task()
        {

            var actions = new List<Action>();

            List<WebManageMenus> list = new List<WebManageMenus>();

            List<WebManageMenus> list1 = new List<WebManageMenus>();
            List<WebManageMenus> list2 = new List<WebManageMenus>();
            List<WebManageMenus> list3 = new List<WebManageMenus>();
            actions.Add(() =>
            {
                var db = new Test_Code_FirstEntities();
                list1.AddRange(db.WebManageMenus.ToList());
            });
            actions.Add(() =>
            {
                var db = new Test_Code_FirstEntities();

                list2.AddRange(db.WebManageMenus.ToList());
            });
            actions.Add(() =>
            {
                var db = new Test_Code_FirstEntities();
                list3.AddRange(db.WebManageMenus.ToList());
            });
            Parallel.Invoke(actions.ToArray());
            list.AddRange(list1);
            list.AddRange(list2);
            list.AddRange(list3);
        }
        public void TaskLock()
        {

            var actions = new List<Action>();

            List<WebManageMenus> list = new List<WebManageMenus>();
            var db = new Test_Code_FirstEntities();
            actions.Add(() =>
            {
                lock (lockObj)
                    list.AddRange(db.WebManageMenus.ToList());
            });
            actions.Add(() =>
            {
                lock (lockObj)
                    list.AddRange(db.WebManageMenus.ToList());
            });
            actions.Add(() =>
            {
                lock (lockObj)
                    list.AddRange(db.WebManageMenus.ToList());
            });
            Parallel.Invoke(actions.ToArray());

        }

        [TestMethod]
        public void Comparable()
        {
            var a1 = new Student { Name = "zzl", Score = 80 };
            var a2 = new Student { Name = "lr", Score = 100 };
            Console.WriteLine("zzl > lr score:{0}", a1.CompareTo(a2));
        }
        [TestMethod]
        public void Test()
        {
            var from = DateTime.MinValue;
            var to = DateTime.MaxValue;
            for (long i = 1; i < 1024; i = i << 1)
            {
                Console.WriteLine(i);
            }

            DateTime dt = DateTime.Now.AddDays(1);
            var dtLast = new DateTime(dt.Year, dt.Month, dt.Day, 0, 0, 0);
            Console.WriteLine(dtLast);

            for (int i = 1; i < 100; i += 10)
            {
                Console.WriteLine(i.ToString().PadLeft(5, '0'));
            }


            Node node = new Node(0, new Node(2, new Node(4, null)));
            Console.WriteLine(
                "node length:" + GetLengthRecursively(node)
            );

            TreeNode tree = new TreeNode(1, 0,
                 left: new TreeNode(11, 1, new TreeNode(111, 2, new TreeNode(1111, 3, null, null), new TreeNode(1112, 3, null, null)), null),
                 right: new TreeNode(12, 1, new TreeNode(121, 2, null, null), new TreeNode(122, 2, null, null)));

            PreOrderTraversal(tree, true);
        }

        /// <summary>
        /// 单向链表
        /// </summary>
        public class Node
        {
            public Node(int value, Node next)
            {
                this.Value = value;
                this.Next = next;
            }

            public int Value { get; private set; }

            public Node Next { get; private set; }
        }
        /// <summary>
        /// 取链表的长度
        /// </summary>
        /// <param name="head"></param>
        /// <returns></returns>
        public static int GetLengthRecursively(Node head)
        {
            if (head == null) return 0;
            return GetLengthRecursively(head.Next) + 1;
        }

        /// <summary>
        /// 二叉树（Ｂ树）
        /// </summary>
        public class TreeNode
        {
            public TreeNode(int value, int level, TreeNode left, TreeNode right)
            {
                this.Value = value;
                this.Level = level;
                this.Left = left;
                this.Right = right;
            }

            public int Value { get; private set; }
            public int Level { get; set; }
            public TreeNode Left { get; private set; }
            public TreeNode Right { get; private set; }
        }

        public static void PreOrderTraversal(TreeNode root, bool isLeft)
        {
            if (root == null) return;
            if (isLeft)
                Console.WriteLine(root.Value.ToString().PadLeft(root.Level * 10, '-'));
            else
                Console.WriteLine(root.Value.ToString().PadLeft(root.Level * 10, '-'));

            PreOrderTraversal(root.Left, true);
            PreOrderTraversal(root.Right, false);
        }

    }
}
