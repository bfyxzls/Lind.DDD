using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;

namespace Lind.DDD.UnitTest
{
    [TestClass]
    public class TextWriterRef
    {
        [TestMethod]
        public void Run()
        {
            using (var sw = new StringWriter())
            {
                logger1(sw);
                logger2(sw);
                sw.ToString();
            }

        }

        void logger1(TextWriter log)
        {
            if (log == null)
                log = TextWriter.Null;
            log.WriteLine("hello world");

        }
        void logger2(TextWriter log)
        {
            if (log == null)
                log = TextWriter.Null;
            log.WriteLine("hello beijing");

        }
    }
}
