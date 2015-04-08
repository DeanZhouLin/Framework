﻿using System;
using System.Configuration;
using System.IO;

namespace DeanZhou.Framework
{

    /// <summary>
    /// 以本地文件的形式记录日志
    /// CreatLogFile——包含存放日志的位置和日志文件的大小
    /// </summary>
    public static class LogHelper
    {
        /// <summary>
        /// Debug异常是否有效
        /// </summary>
        public static bool DebugEnabled = true;

        /// <summary>
        /// 程序运行结果是否有效
        /// </summary>
        public static bool InfoEnabled = true;

        /// <summary>
        /// 警告是否有效
        /// </summary>
        public static bool WarnEnabled = true;

        /// <summary>
        /// 错误是否有效
        /// </summary>
        public static bool ErrorEnabled = true;

        /// <summary>
        /// 高级错误是否有效
        /// </summary>
        public static bool FatalEnabled = true;

        /// <summary>
        /// 自定义信息是否有效
        /// </summary>
        public static bool CustomInfoEnabled = true;

        #region 常量定义
        /// <summary>
        /// 异常级别
        /// </summary>
        public const string LogLevel = "Log level:";

        /// <summary>
        /// 发生时间
        /// </summary>
        public const string OccuredTime = "Occured Time:";

        /// <summary>
        /// 异常名称
        /// </summary>
        public const string ExceptionName = "Exception Name:";

        /// <summary>
        /// 异常详细信息
        /// </summary>
        public const string ExceptionDiscription = "Exception:";

        /// <summary>
        /// 异常源
        /// </summary>
        public const string Source = "Sourse:";

        /// <summary>
        /// 堆栈数据
        /// </summary>
        public const string Stacktrace = "StackTrace:";

        /// <summary>
        /// 消息名称
        /// </summary>
        public const string Messagename = "MessageName:";

        /// <summary>
        /// 消息内容
        /// </summary>
        public const string Messagetext = "MessageText:";
        #endregion

        #region 记录Debug信息
        /// <summary>
        /// 记录Debug信息
        /// </summary>
        /// <param name="message">message</param>
        public static void Debug(object message)
        {
            if (DebugEnabled)
            {
                try
                {
                    const string type = "Debug";

                    FileInfo logFile = CreatLogFile(type);

                    StreamWriter writer = logFile.AppendText();

                    writer.WriteLine(LogLevel + type);
                    writer.WriteLine(OccuredTime + DateTime.Now.ToString());
                    writer.WriteLine(Messagename + message.GetType().FullName);
                    writer.WriteLine(Messagetext + message);
                    writer.WriteLine("---------------------------------------------");

                    writer.Flush();
                    writer.Close();
                }
                catch { }
            }
        }
        #endregion

        #region 记录Debug异常信息
        /// <summary>
        /// 记录Debug异常信息
        /// </summary>
        /// <param name="message">message</param>
        /// <param name="e">e</param>
        public static void Debug(object message, Exception e)
        {
            if (DebugEnabled)
            {
                try
                {
                    const string type = "Debug";

                    FileInfo logFile = CreatLogFile(type);

                    StreamWriter writer = logFile.AppendText();

                    writer.WriteLine(LogLevel + type);
                    writer.WriteLine(OccuredTime + DateTime.Now.ToString());
                    writer.WriteLine(ExceptionName + e.GetType());
                    writer.WriteLine(ExceptionDiscription + e.Message);
                    writer.WriteLine(Stacktrace + e.StackTrace);
                    writer.WriteLine(Source + e.Source);
                    writer.WriteLine("---------------------------------------------");

                    writer.Flush();
                    writer.Close();
                }
                catch { }
            }
        }
        #endregion

        #region 记录程序运行结果信息CustomInfo

        /// <summary>
        /// 记录程序运行结果自定义信息
        /// </summary>
        /// <param name="message">message</param>
        /// <param name="type"></param>
        public static void LogLocal(this object message, string type = "CustomInfo")
        {
            if (CustomInfoEnabled)
            {
                try
                {
                    FileInfo logFile = CreatLogFile(type, string.Empty);
                    StreamWriter writer = logFile.AppendText();

                    writer.WriteLine(LogLevel + type);
                    writer.WriteLine(OccuredTime + DateTime.Now);
                    writer.WriteLine(Messagename + message.GetType().FullName);
                    writer.WriteLine(Messagetext + message);
                    writer.WriteLine("---------------------------------------------");

                    writer.Flush();
                    writer.Close();
                }
                catch { }
            }
        }
        #endregion

        #region 记录程序运行结果信息
        /// <summary>
        /// 记录程序运行结果信息
        /// </summary>
        /// <param name="message">message</param>
        public static void Info(object message)
        {
            if (InfoEnabled)
            {
                try
                {
                    const string type = "Info";

                    FileInfo logFile = CreatLogFile(type);

                    StreamWriter writer = logFile.AppendText();

                    writer.WriteLine(LogLevel + type);
                    writer.WriteLine(OccuredTime + DateTime.Now);
                    writer.WriteLine(Messagename + message.GetType().FullName);
                    writer.WriteLine(Messagetext + message);
                    writer.WriteLine("---------------------------------------------");

                    writer.Flush();
                    writer.Close();
                }
                catch { }
            }
        }
        #endregion

        #region 记录程序运行结果信息
        /// <summary>
        /// 记录程序运行结果信息
        /// </summary>
        /// <param name="message">message</param>
        /// <param name="e">e</param>
        public static void Info(object message, Exception e)
        {
            if (InfoEnabled)
            {
                try
                {
                    const string type = "Info";

                    FileInfo logFile = CreatLogFile(type);

                    StreamWriter writer = logFile.AppendText();

                    writer.WriteLine(LogLevel + type);
                    writer.WriteLine(OccuredTime + DateTime.Now.ToString());
                    writer.WriteLine(Messagename + message.GetType().FullName);
                    writer.WriteLine("---------------------------------------------");

                    writer.Flush();
                    writer.Close();
                }
                catch { }
            }
        }
        #endregion

        #region 记录警告信息
        /// <summary>
        /// 记录警告信息
        /// </summary>
        /// <param name="message">message</param>
        public static void Warn(object message)
        {
            if (WarnEnabled)
            {
                try
                {
                    const string type = "Warn";

                    FileInfo logFile = CreatLogFile(type);

                    StreamWriter writer = logFile.AppendText();

                    writer.WriteLine(LogLevel + type);
                    writer.WriteLine(OccuredTime + DateTime.Now.ToString());
                    writer.WriteLine(Messagename + message.GetType().FullName);
                    writer.WriteLine(Messagetext + message);
                    writer.WriteLine("---------------------------------------------");

                    writer.Flush();
                    writer.Close();
                }
                catch { }
            }
        }
        #endregion

        #region 记录警告异常信息
        /// <summary>
        /// 记录警告异常信息
        /// </summary>
        /// <param name="message">message</param>
        /// <param name="e">e</param>
        public static void Warn(object message, Exception e)
        {
            if (WarnEnabled)
            {
                try
                {
                    const string type = "Warn";

                    FileInfo logFile = CreatLogFile(type);

                    StreamWriter writer = logFile.AppendText();

                    writer.WriteLine(LogLevel + type);
                    writer.WriteLine(OccuredTime + DateTime.Now.ToString());
                    writer.WriteLine(ExceptionName + e.GetType());
                    writer.WriteLine(ExceptionDiscription + e.Message);
                    writer.WriteLine(Stacktrace + e.StackTrace);
                    writer.WriteLine(Source + e.Source);
                    writer.WriteLine("---------------------------------------------");

                    writer.Flush();
                    writer.Close();
                }
                catch { }
            }
        }
        #endregion

        #region 记录错误信息
        /// <summary>
        /// 记录错误信息
        /// </summary>
        /// <param name="message">message</param>
        public static void Error(object message)
        {
            if (!ErrorEnabled) return;
            try
            {
                const string type = "Error";

                FileInfo logFile = CreatLogFile(type);

                StreamWriter writer = logFile.AppendText();

                writer.WriteLine(LogLevel + type);
                writer.WriteLine(OccuredTime + DateTime.Now);
                writer.WriteLine(Messagename + message.GetType().FullName);
                writer.WriteLine(Messagetext + message);
                writer.WriteLine("---------------------------------------------");

                writer.Flush();
                writer.Close();
            }
            catch { }
        }

        #endregion

        #region 记录错误异常信息
        /// <summary>
        /// 记录错误异常信息
        /// </summary>
        /// <param name="message">message</param>
        /// <param name="e">e</param>
        public static void Error(object message, Exception e)
        {
            if (ErrorEnabled)
            {
                try
                {
                    const string type = "Error";

                    FileInfo logFile = CreatLogFile(type);

                    StreamWriter writer = logFile.AppendText();

                    writer.WriteLine(LogLevel + type);
                    writer.WriteLine(OccuredTime + DateTime.Now.ToString());
                    writer.WriteLine(ExceptionName + e.GetType());
                    writer.WriteLine(ExceptionDiscription + e.Message);
                    writer.WriteLine(Stacktrace + e.StackTrace);
                    writer.WriteLine(Source + e.Source);
                    writer.WriteLine("----------------------------");

                    writer.Flush();
                    writer.Close();
                }
                catch { }
            }

        }
        #endregion

        #region 记录高级错误异常信息
        /// <summary>
        /// 记录高级错误异常信息
        /// </summary>
        /// <param name="message">message</param>
        public static void Fatal(object message)
        {
            if (FatalEnabled)
            {
                try
                {
                    const string type = "Fatal";

                    FileInfo logFile = CreatLogFile(type);

                    StreamWriter writer = logFile.AppendText();

                    writer.WriteLine(LogLevel + type);
                    writer.WriteLine(OccuredTime + DateTime.Now.ToString());
                    writer.WriteLine(Messagename + message.GetType().FullName);
                    writer.WriteLine(Messagetext + message);
                    writer.WriteLine("---------------------------------------------");

                    writer.Flush();
                    writer.Close();
                }
                catch { }
            }
        }
        #endregion

        #region 记录高级错误异常信息
        /// <summary>
        /// 记录高级错误异常信息
        /// </summary>
        /// <param name="message">message</param>
        /// <param name="e">e</param>
        public static void Fatal(object message, Exception e)
        {
            if (FatalEnabled)
            {
                try
                {
                    const string type = "Fatal";

                    FileInfo logFile = CreatLogFile(type);

                    StreamWriter writer = logFile.AppendText();

                    writer.WriteLine(LogLevel + type);
                    writer.WriteLine(OccuredTime + DateTime.Now.ToString());
                    writer.WriteLine(ExceptionName + e.GetType());
                    writer.WriteLine(ExceptionDiscription + e.Message);
                    writer.WriteLine(Stacktrace + e.StackTrace);
                    writer.WriteLine(Source + e.Source);
                    writer.WriteLine("---------------------------------------------");

                    writer.Flush();
                    writer.Close();
                }
                catch { }
            }
        }
        #endregion

        #region 创建/读取日志文件

        /// <summary>
        /// 创建/读取文件
        /// </summary>
        /// <param name="type">异常类型</param>
        /// <param name="logFilePath"></param>
        /// <returns></returns>
        private static FileInfo CreatLogFile(string type, string logFilePath = "")
        {
            if (string.IsNullOrEmpty(logFilePath))
                logFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Log");
            //string logFilePath = AppDomain.CurrentDomain.BaseDirectory;

            //创建目录
            try
            {
                if (!Directory.Exists(logFilePath))
                {
                    Directory.CreateDirectory(logFilePath);
                }
            }
            catch { }

            string logFileNameShort = DateTime.Now.ToString("yyyyMMdd") + type;
            string logFileName = logFileNameShort + ".log";

            FileInfo logFile = null;
            if (!logFilePath.EndsWith("/"))
            {
                logFilePath = logFilePath + "/";
            }
            //Find file
            try
            {
                logFile = new FileInfo(logFilePath + logFileName);

                if (logFile.Length >= CurrentAppConfig.LogFileMaxSize * 1024)
                {
                    int i = 1;
                    do
                    {
                        // ReSharper disable RedundantAssignment
                        logFileNameShort = logFileNameShort.Split('_')[0];
                        logFileNameShort = logFileNameShort + "_" + i;
                        logFileName = logFileNameShort + ".log";
                        i++;
                        // ReSharper restore RedundantAssignment
                        logFile = new FileInfo(logFilePath + logFileName);

                    } while (logFile.Length >= CurrentAppConfig.LogFileMaxSize * 1024);
                }
            }
            catch { }

            return logFile;
        }
        #endregion
    }

    public static class CurrentAppConfig
    {
        #region PathConfig

        public static int LogFileMaxSize
        {
            get { return ConfigurationManager.AppSettings["LogFileMaxSize"] == null ? 512 : int.Parse(ConfigurationManager.AppSettings["LogFileMaxSize"]); }
        }

        public static string LogSaveDic
        {
            get { return ConfigurationManager.AppSettings["LogSaveDic"]; }
        }

        #endregion
    }

}
