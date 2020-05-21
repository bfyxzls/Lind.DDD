using Lind.DDD.Domain;
using Lind.DDD.IoC;
using Lind.DDD.IRepositories;
using Lind.DDD.MongoDbClient;
using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lind.DDD.Logger.Implements
{

    /// <summary>
    /// 使用MongoDB进行日志持久化
    /// </summary>
    public class JsonFileLogger : LoggerBase
    {
        private void AddToJson(string message, string level, string detail = null)
        {
            var entity = new GlobalLogger
            {
                Level = level,
                Message = message,
                ProjectName = ConfigConstants.ConfigManager.Config.Logger.ProjectName ?? string.Empty,
            
            };
            if (!Directory.Exists(FileUrl))
                Directory.CreateDirectory(FileUrl);
            string filePath = Path.Combine(FileUrl, "Json" + DateTime.Now.ToLongDateString() + ".log");
            using (System.IO.StreamWriter srFile = new System.IO.StreamWriter(filePath, true))
            {
                srFile.WriteLine(Utils.SerializeMemoryHelper.SerializeToJson(entity) + ",");
            }
        }
        protected override void InputLogger(string message)
        {
            AddToJson(message, "Info");
        }
        public override void Logger_Debug(string message)
        {
            AddToJson(message, "Debug");
        }
        public override void Logger_Error(Exception ex)
        {
            AddToJson(ex.Message, "Error", ex.StackTrace);
        }
        public override void Logger_Fatal(string message)
        {
            AddToJson(message, "Fatal");
        }
        public override void Logger_Info(string message)
        {
            AddToJson(message, "Info");
        }
        public override void Logger_Warn(string message)
        {
            AddToJson(message, "Warn");
        }
    }
}
