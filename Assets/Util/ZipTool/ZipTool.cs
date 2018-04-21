using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Threading;

using ICSharpCode.SharpZipLib.Zip;
using ICSharpCode.SharpZipLib.Core;
using SevenZip.Compression.LZMA;

public class ZipTool
{
    public enum ZipType
    {
        CompressBySevenZip,
        UncompressBySevenZipByte,
        UncompressBySevenZipString,
        UncompressByFastZip,
    }
    public class ZipData
    {
        public ZipType Type;
        public Action<object> Callback;
        public object[] Params;
        public ZipData(ZipType type, Action<object> callback, object[] param)
        {
            Type = type;
            Callback = callback;
            Params = param;
        }
    }
    static Thread _Thread;
    static bool _IsTheadRun = true;
    static Queue<ZipData> _ZipData = new Queue<ZipTool.ZipData>();

    /// <summary>
    /// 开始压缩或解压数据（新线程中）
    /// </summary>
    public static void Zip(ZipType type, Action<object> callback, params object[] param)
    {
        _ZipData.Enqueue(new ZipData(type, callback, param));
        StartThread();
    }
    /// <summary>
    /// 终止压缩或解压数据线程
    /// </summary>
    public static void ZipOver()
    {
        _IsTheadRun = false;
        _Thread.Abort();
        _Thread = null;
    }

    static void StartThread()
    {
        _IsTheadRun = true;
        if (_Thread == null)
        {
            Debug.Log("新建一个线程");
            _Thread = new Thread(new ThreadStart(ThreadFun));
            _Thread.Start();
        }
        Debug.Log("当前解压缩线程状态：" + _Thread.ThreadState);
        switch (_Thread.ThreadState)
        {
            case ThreadState.Running:
                break;
            case ThreadState.StopRequested:
                break;
            case ThreadState.SuspendRequested:
                break;
            case ThreadState.Background:
                break;
            case ThreadState.Unstarted:
                break;
            case ThreadState.Stopped:
                break;
            case ThreadState.WaitSleepJoin:
                break;
            case ThreadState.Suspended:
                break;
            case ThreadState.AbortRequested:
                break;
            case ThreadState.Aborted:
                break;
            default:
                break;
        }
    }
    
    static void ThreadFun()
    {
        while (_IsTheadRun)
        {
            while (_ZipData.Count > 0)
            {
                //开始解压数据
                var _data = _ZipData.Dequeue();
                switch (_data.Type)
                {
                    case ZipType.CompressBySevenZip:
                        CompressBySevenZip(_data.Params[0] as string, _data.Params[1] as string);
                        break;
                    case ZipType.UncompressBySevenZipByte:
                        if (_data.Callback != null)
                        {
                            _data.Callback(UncompressBySevenZip(_data.Params[0] as byte[]));
                        }
                        break;
                    case ZipType.UncompressBySevenZipString:
                        if (_data.Callback != null)
                        {
                            _data.Callback(UncompressBySevenZip(_data.Params[0] as string));
                        }
                        break;
                    case ZipType.UncompressByFastZip:
                        UncompressByFastZip(_data.Params[0] as string, _data.Params[1] as string, _data.Params.Length > 2 ? _data.Params[2] as CompletedFileHandler : null);
                        break;
                    default:
                        break;
                }
            }
            Thread.Sleep(100);      //没有要解压的数据，就降低执行频率
        }
    }
    static void CompressBySevenZip(string sourcePath, string targetpath)
    {

        SevenZipHelper.Zip(sourcePath, targetpath);
    }
    static byte[] UncompressBySevenZip(byte[] oriData)
    {
        byte[] decomData = SevenZipHelper.Unzip(oriData);
        return decomData;
    }

    static byte[] UncompressBySevenZip(string filename)
    {
        byte[] decomData = SevenZipHelper.Unzip(filename);
        return decomData;
    }

    static bool UncompressByFastZip(string orgPath, string tarPath, CompletedFileHandler progressFileHandle = null)
    {
        bool res = true;
        try
        {
            FastZipEvents events = new FastZipEvents();
            if (progressFileHandle != null)
            {
                events.CompletedFile = progressFileHandle;
            }

            FastZip zipfile2 = new FastZip(events);
            zipfile2.ExtractZip(orgPath, tarPath, "");
        }
        catch (Exception ex)
        {
            Debug.LogError(ex.Message);
            res = false;
        }

        return res;
    }
}
