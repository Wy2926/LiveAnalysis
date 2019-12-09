using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogR
{
    public static class AppLog
    {
        /// <summary>
        /// 日志实体
        /// </summary>
        public static readonly log4net.ILog Log = log4net.LogManager.GetLogger("Info");

        /// <summary>
        ///  添加info信息
        /// </summary>
        /// <param name="info">自定义日志内容说明</param>
        public static void Info(string info)
        {
            try
            {
                Console.WriteLine(info);
                Log.Info(info);
            }
            catch { }
        }

        /// <summary>
        ///  添加Debug信息
        /// </summary>
        /// <param name="info">自定义日志内容说明</param>
        public static void Debug(string msg)
        {
            try
            {
                Console.WriteLine(msg);
                Log.Debug(msg);
            }
            catch { }
        }


        /// <summary>
        /// 添加异常信息
        /// </summary>
        /// <param name="info">自定义日志内容说明</param>
        /// <param name="ex">异常信息</param>
        public static void Error(string info, Exception ex)
        {
            try
            {
                Console.WriteLine($"{info}:{ex.Message}");
                Console.WriteLine(ex.StackTrace);
                Log.Error(info, ex);
            }
            catch { }
        }
    }
}
