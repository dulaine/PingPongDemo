#region 模块信息
/*----------------------------------------------------------------
// Copyright (C) 2015 天津，颐博
//
// 模块名：LoggerHelper
// 创建者：Leon Xu
// 修改者列表：
// 创建日期：2015.12.7
// 模块描述：日志输出类。
//----------------------------------------------------------------*/
#endregion

using System;
using System.Diagnostics;
using System.IO;
using System.Text;
using UnityEngine;


/// <summary>
/// 日志等级声明。
/// </summary>
[Flags]
public enum eLogLevel
{
    eLOG_NONE = 0,           //默认
    eLOG_DEBUG = 1,          //调试
    eLOG_INFO = 2,           //信息
    eLOG_WARNING = 4,        //警告
    eLOG_ERROR = 8,          //错误
    eLOG_EXCEPT = 16,        //异常
    eLOG_CRITICAL = 32,      //关键错误
}

/// <summary>
/// 日志控制类。
/// </summary>
/// 
static public class LoggerHelper
{
    //日志等级，默认
    static public eLogLevel m_sCurrentLogLevels = eLogLevel.eLOG_DEBUG |
                                               eLogLevel.eLOG_INFO |
                                               eLogLevel.eLOG_WARNING |
                                               eLogLevel.eLOG_ERROR |
                                               eLogLevel.eLOG_EXCEPT |
                                               eLogLevel.eLOG_CRITICAL;

    //static public eLogLevel m_sCurrentLogLevels = eLogLevel.eLOG_ERROR |
    //                                           eLogLevel.eLOG_EXCEPT |
    //                                           eLogLevel.eLOG_CRITICAL;

    static private LogWriter m_logWriter;
    static private ulong index = 0;// debug记数

    static LoggerHelper()
    {
        if (GameRoot.Instacne.IsDebug)
        {
            m_sCurrentLogLevels = eLogLevel.eLOG_DEBUG |
                                               eLogLevel.eLOG_INFO |
                                               eLogLevel.eLOG_WARNING |
                                               eLogLevel.eLOG_ERROR |
                                               eLogLevel.eLOG_EXCEPT |
                                               eLogLevel.eLOG_CRITICAL;
        }
        else
        {
            m_sCurrentLogLevels = eLogLevel.eLOG_ERROR |
                                           eLogLevel.eLOG_EXCEPT |
                                           eLogLevel.eLOG_CRITICAL;
        }
        m_logWriter = new LogWriter();
        Application.logMessageReceived += new Application.LogCallback(ProcessExceptionReport);
    }

    static public void Release()
    {
        m_logWriter.Release();
    }

    /// <summary>
    /// 调试日志。
    /// </summary>
    /// <param name="message">日志内容</param>
    /// <param name="isShowStack">是否显示调用栈信息</param>
    static public void Debug(object message, bool isNeedStack = false)
    {
        if (eLogLevel.eLOG_DEBUG == (m_sCurrentLogLevels & eLogLevel.eLOG_DEBUG))
        {
            if (isNeedStack)
                Log(string.Concat(" [DEBUG]: ", message, " Index = ", index++, '\n', GetStacksInfo()), eLogLevel.eLOG_DEBUG);
            else
                Log(string.Concat(" [DEBUG]: ", message, " Index = ", index++), eLogLevel.eLOG_DEBUG);
        }
    }

    /// <summary>
    /// 信息日志。
    /// </summary>
    /// <param name="message">日志内容</param>
    static public void Info(object message)
    {
        if (eLogLevel.eLOG_INFO == (m_sCurrentLogLevels & eLogLevel.eLOG_INFO))
            Log(string.Concat(" [INFO]: ", message), eLogLevel.eLOG_INFO);
    }

    /// <summary>
    /// 警告日志。
    /// </summary>
    /// <param name="message">日志内容</param>
    static public void Warning(object message, bool isNeedStatck = false)
    {
        if (eLogLevel.eLOG_WARNING == (m_sCurrentLogLevels & eLogLevel.eLOG_WARNING))
        {
            if (isNeedStatck)
                Log(string.Concat(" [WARNING]: ", message, '\n', GetStacksInfo()), eLogLevel.eLOG_WARNING);
            else
                Log(string.Concat(" [WARNING]: ", message), eLogLevel.eLOG_WARNING);
        }
    }

    /// <summary>
    /// 异常日志。
    /// </summary>
    /// <param name="message">日志内容</param>
    static public void Error(object message)
    {
        if (eLogLevel.eLOG_ERROR == (m_sCurrentLogLevels & eLogLevel.eLOG_ERROR))
            Log(string.Concat(" [ERROR]: ", message, '\n', GetStacksInfo()), eLogLevel.eLOG_ERROR);

        m_logWriter.UploadToServer((ushort)LogType.Error, message.ToString());
    }

    /// <summary>
    /// 关键日志。
    /// </summary>
    /// <param name="message">日志内容</param>
    static public void Critical(object message)
    {
        if (eLogLevel.eLOG_CRITICAL == (m_sCurrentLogLevels & eLogLevel.eLOG_CRITICAL))
            Log(string.Concat(" [CRITICAL]: ", message, '\n', GetStacksInfo()), eLogLevel.eLOG_CRITICAL);
    }

    /// <summary>
    /// 异常日志。
    /// </summary>
    /// <param name="ex">异常实例。</param>
    static public void Except(Exception ex, object message = null)
    {
        if (eLogLevel.eLOG_EXCEPT == (m_sCurrentLogLevels & eLogLevel.eLOG_EXCEPT))
        {
            Exception innerException = ex;
            while (innerException.InnerException != null)
            {
                innerException = innerException.InnerException;
            }
            Log(string.Concat(" [EXCEPT]: ", message == null ? "" : message + "\n", ex.Message, innerException.StackTrace), eLogLevel.eLOG_EXCEPT);
        }
    }

    /// <summary>
    /// 获取堆栈信息。
    /// </summary>
    /// <returns></returns>
    private static String GetStacksInfo()
    {
        StringBuilder sb = new StringBuilder();
        StackTrace st = new StackTrace();
        var sf = st.GetFrames();
        for (int i = 2; i < sf.Length; i++)
        {
            sb.AppendLine(sf[i].ToString());
        }

        return sb.ToString();
    }

    /// <summary>
    /// 写日志。
    /// </summary>
    /// <param name="message">日志内容</param>
    private static void Log(string message, eLogLevel level, bool writeEditorLog = true)
    {
        var msg = string.Concat(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss,fff"), message);
        m_logWriter.WriteLog(msg, level, writeEditorLog);
    }

    /// <summary>
    /// 获取调用栈信息。
    /// </summary>
    /// <returns>调用栈信息</returns>
    private static String GetStackInfo()
    {
        StackTrace st = new StackTrace();
        StackFrame sf = st.GetFrame(2);//[0]为本身的方法 [1]为调用方法
        var method = sf.GetMethod();
        return String.Format("{0}.{1}(): ", method.ReflectedType.Name, method.Name);
    }

    static private void ProcessExceptionReport(string message, string stackTrace, LogType type)
    {
        var logLevel = eLogLevel.eLOG_DEBUG;
        switch (type)
        {
            case LogType.Assert:
                logLevel = eLogLevel.eLOG_DEBUG;
                break;
            case LogType.Error:
                logLevel = eLogLevel.eLOG_ERROR;
                break;
            case LogType.Exception:
                logLevel = eLogLevel.eLOG_EXCEPT;
                break;
            case LogType.Log:
                logLevel = eLogLevel.eLOG_DEBUG;
                break;
            case LogType.Warning:
                logLevel = eLogLevel.eLOG_WARNING;
                break;
            default:
                break;
        }

        if (logLevel == (m_sCurrentLogLevels & logLevel))
        {
            //警告 和 debug，不打印堆栈
            if (logLevel == eLogLevel.eLOG_WARNING || logLevel == eLogLevel.eLOG_DEBUG)
                Log(string.Concat(" [SYS_", logLevel, "]: ", message), logLevel, false);
            else
                Log(string.Concat(" [SYS_", logLevel, "]: ", message, '\n', stackTrace), logLevel, false);
        }

        //上传严重错误
        if (logLevel == eLogLevel.eLOG_ERROR ||
            logLevel == eLogLevel.eLOG_EXCEPT ||
            logLevel == eLogLevel.eLOG_CRITICAL)
        {
            m_logWriter.UploadToServer((ushort)type, message);
        }
    }
}

/// <summary>
/// 日志写入文件管理类。
/// </summary>
public class LogWriter
{
    private string m_logPath = Application.persistentDataPath + "/Log/";
    private string m_logFileName = "log_{0}.txt";
    private string m_logFilePath;
    private FileStream m_fs;
    private StreamWriter m_sw;
    private Action<String, eLogLevel, bool> m_logWriter;
    private readonly static object m_locker = new object();

    /// <summary>
    /// 默认构造函数。
    /// </summary>
    public LogWriter()
    {
        //UnityEngine.Debug.LogWarning(m_logPath);

        if (!Directory.Exists(m_logPath))
            Directory.CreateDirectory(m_logPath);

        m_logFilePath = String.Concat(m_logPath, String.Format(m_logFileName, DateTime.Today.ToString("yyyyMMdd")));
        try
        {
            m_logWriter = Write;
            m_fs = new FileStream(m_logFilePath, FileMode.Append, FileAccess.Write, FileShare.ReadWrite);
            m_sw = new StreamWriter(m_fs);
        }
        catch (Exception ex)
        {
            UnityEngine.Debug.LogError(ex.Message);
        }
    }

    /// <summary>
    /// 释放资源。
    /// </summary>
    public void Release()
    {
        lock (m_locker)
        {
            if (m_sw != null)
            {
                m_sw.Close();
                m_sw.Dispose();
                m_sw = null;
            }
            if (m_fs != null)
            {
                m_fs.Close();
                m_fs.Dispose();
                m_fs = null;
            }
        }
    }

    //上传日志
    public void UploadToServer(ushort type, string logs)
    {
#if !UNITY_EDITOR
		/*
        CRCClientCmd.stUserBugCommitClientCmd stCmd = new CRCClientCmd.stUserBugCommitClientCmd();
        stCmd.mBug_type = type;
        stCmd.mUser_id = NetAutoConnect.Instance.GetUserId();

        //CUtility.StringToTdrBytesDefault(stCmd.mBug_content, logs);
        stCmd.mBug_size = (ushort)stCmd.mBug_content.Length;

        NetSys.Instance.SendCmd(stCmd);
		*/
#endif
    }

    /// <summary>
    /// 写日志。
    /// </summary>
    /// <param name="msg">日志内容</param>
    public void WriteLog(string msg, eLogLevel level, bool writeEditorLog)
    {
        m_logWriter(msg, level, writeEditorLog);
    }

    private void Write(string msg, eLogLevel level, bool writeEditorLog)
    {
        lock (m_locker)
            try
            {
                //#if UNITY_EDITOR
                if (writeEditorLog)
                {
                    switch (level)
                    {
                        case eLogLevel.eLOG_DEBUG:
                        case eLogLevel.eLOG_INFO:
                            UnityEngine.Debug.Log(msg);
                            break;
                        case eLogLevel.eLOG_WARNING:
                            UnityEngine.Debug.LogWarning(msg);
                            break;
                        case eLogLevel.eLOG_ERROR:
                        case eLogLevel.eLOG_EXCEPT:
                        case eLogLevel.eLOG_CRITICAL:
                            UnityEngine.Debug.LogError(msg);
                            break;
                        default:
                            break;
                    }
                }
                //#endif
                if (m_sw != null)
                {
                    m_sw.WriteLine(msg);
                    m_sw.Flush();
                }
            }
            catch (Exception ex)
            {
                UnityEngine.Debug.LogError(ex.Message);
            }
    }
}
