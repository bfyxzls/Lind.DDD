using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;
using System.Text;

namespace Lind.DDD.UnitTest
{
    [TestClass]
    public class NetFileModify
    {
        [TestMethod]
        public void Read_Update()
        {
            string filePath = @"\\192.168.2.21\NugetForTsingda\1.txt";

            string result = ReadTxt(filePath);
            result = "hello world";
            WriteTxt(filePath, result);
        }


        static string ReadTxt(string fileName)
        {
            using (FileStream fs = new FileStream(fileName, FileMode.OpenOrCreate))
            {

                using (StreamReader sw = new StreamReader(fs, Encoding.UTF8))
                {
                    return sw.ReadToEnd();
                }

            }
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

    }
}
