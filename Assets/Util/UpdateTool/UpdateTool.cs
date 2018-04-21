using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;
using System.Xml;
using System.Linq;

using BestHTTP;

/// <summary>
/// 更新工具类
/// </summary>
public class UpdateTool
{
    const bool _isUncompress = false;

    const string cIpPort = "http://192.168.1.228/catchghost/";          //下载文件的地址
    const string cVersionbase = "versionbase.xml";
    const string cPlatform = "platform.xml";
    const string cVersion = "Version.txt";

    static Action mUpdateFinishCb;          //更新完成后的回调
    static List<string> _UpdateFilePaths = new List<string>() { };      //要下载的文件的地址
    static int _NowLoadIndex = 0;
    //获取到的数据
    static string mNowVersion = "";         //当前最新版本
    static string mDownloadPath = "";       //当前下载地址
    static string mVersion = "";            //版本文件数据

    //临时数据
    static int mConfNum = 0;    //要下载的配置文件的数量
    static string mVersionPath = "";

    //--------------外部调用-----------------
    #region 外部调用
    /// <summary>
    /// 下载版本配置文件
    /// </summary>
    public static void DoDownloadConfigFiles(Action callback)
    {
        HTTPRequest _request = new HTTPRequest(new Uri(cIpPort + "versionbase.xml"), OnVersionBaseOver);
        _request.Send();
        if (callback != null)
        {
            mUpdateFinishCb = callback;
        }
    }
    /// <summary>
    /// 下载更新文件
    /// </summary>
    public static void DoUpdateFiles(List<string> paths)
    {
        _UpdateFilePaths = paths;
        _NowLoadIndex = 0;
        DoUpdateFile();
    }
    #endregion

    //--------------内部调用-----------------
    #region 内部调用
    /// <summary>
    /// 配置文件下载完毕
    /// </summary>
    static void OnConfigDownload()
    {
        mConfNum--;
        if (mConfNum <= 0)
        {
            //所有配置下载完毕，开始对比版本
            CheckVersion();
        }
    }
    /// <summary>
    /// 对比版本
    /// </summary>
    static void CheckVersion()
    {
        //根据版本号，判断当前应该热更还是更新整包
        var myV = Application.version.Split('.');
        var nowV = mNowVersion.Split('.');
        if (myV[0] != nowV[0])      //大版本有变化，更新完整包
        {

        }
        else if (myV[1] != nowV[1])     //小版本不同，更新热更包
        {
            //检测本机版本文件和下载版本文件的区别
            mVersionPath = string.Concat(IOPath.Instance.NowPlatformPath, IOPath.Instance.NowPlatformName, "_", cVersion);
            string oldStr = "";
            if (IOTool.CheckHaveFile(mVersionPath))
            {
                oldStr = IOTool.LoadFileString(mVersionPath);
            }
            List<string> add;
            List<string> del;
            List<string> diff;
            VersionComparisonTool.Comparison(oldStr, mVersion, out add, out del, out diff);
            //删除所有旧文件
            for (int i = 0; i < del.Count; i++)
            {
                var dPath = IOPath.Instance.NowPlatformPath + del[i];
                if (IOTool.CheckHaveFile(dPath))    //可能被删除过
                {
                    IOTool.DeleteFile(dPath);
                }
            }
            //下载文件
            var list = add.Concat(diff).ToList();
            if (list.Count > 0)
            {

                DoUpdateFiles(list);
            }
            else
            {
                if (mUpdateFinishCb != null)
                {
                    mUpdateFinishCb();
                }
            }
        }
    }
    /// <summary>
    /// 下载更新文件
    /// </summary>
    static void DoUpdateFile()
    {
        if (_NowLoadIndex < _UpdateFilePaths.Count)
        {
            Debug.Log("文件下载地址 " + _UpdateFilePaths.Count + "       " + mDownloadPath + _UpdateFilePaths[_NowLoadIndex]);
            var _request = new HTTPRequest(new Uri(mDownloadPath + _UpdateFilePaths[_NowLoadIndex]), OnUpdateOver);
            _request.Send();
        }
        else
        {

            //所有文件更新完毕
            Debug.Log("所有文件更新完毕 " + mVersionPath);
            //保存版本文件
            IOTool.CreateFileString(mVersionPath, mVersion);
            if (mUpdateFinishCb != null)
            {
                mUpdateFinishCb();
            }
        }
    }
    /// <summary>
    /// 解压数据，放在永久性存储文件夹下
    /// </summary>
    static void CreateFile(byte[] data)
    {
        Debug.Log("下载完毕，准备写文件到： " + IOPath.Instance.NowPlatformPath + _UpdateFilePaths[_NowLoadIndex]);
        IOTool.CreateFileBytes(IOPath.Instance.NowPlatformPath + _UpdateFilePaths[_NowLoadIndex], data);

        ++_NowLoadIndex;
        Debug.Log("下载进度-> " + _NowLoadIndex + "/" + _UpdateFilePaths.Count);
        DoUpdateFile();     //循环下载
    }
    #endregion

    //--------------回调方法-----------------
    #region 回调方法
    /// <summary>
    /// VersionBase下载完毕
    /// </summary>
    static void OnVersionBaseOver(HTTPRequest req, HTTPResponse resp)
    {
        Debug.Log(req.State);
        switch (req.State)
        {
            case HTTPRequestStates.Initial:
                break;
            case HTTPRequestStates.Queued:
                break;
            case HTTPRequestStates.Processing:
                break;
            case HTTPRequestStates.Finished:
                //下载完毕
                Debug.Log("versionbase.xml download complete! " + resp.DataAsText);
                //转换为xml
                StringReader reader = new StringReader(resp.DataAsText);
                XmlDocument xmlDoc = new XmlDocument();
                xmlDoc.Load(reader);
                //根节点
                XmlNode root = xmlDoc.SelectSingleNode("clientversion");
                if (root.HasChildNodes)
                {
                    foreach (XmlNode item in root.ChildNodes)
                    {
                        switch (item.LocalName)
                        {
                            case "versionflag":
                                mNowVersion = item.Attributes.GetNamedItem("ver").Value;
                                break;
                            case "versionres":
                                mDownloadPath = item.Attributes.Item(0).Value;      //所有文件下载地址
                                mConfNum = 2;
                                //开始下载
                                var request = new HTTPRequest(new Uri(mDownloadPath + cPlatform), OnPlatformOver);
                                request.Send();
                                request = new HTTPRequest(new Uri(string.Concat(mDownloadPath, IOPath.Instance.NowPlatformName, "_", cVersion)), OnVersionesOver);
                                request.Send();
                                break;
                            case "loginurl":
                                //ShareSDKTool.instance.Login_Url = item.Attributes.Item(0).Value;
                                break;
                            case "payurl":
                                //ShareSDKTool.instance.Pay_Url = item.Attributes.Item(0).Value;
                                break;
                            case "authurl":
                                //ShareSDKTool.instance.Auth_Url = item.Attributes.Item(0).Value;
                                break;
                        }
                    }
                }
                reader.Close();
                break;
            case HTTPRequestStates.Error:
                break;
            case HTTPRequestStates.Aborted:
                break;
            case HTTPRequestStates.ConnectionTimedOut:
                break;
            case HTTPRequestStates.TimedOut:
                break;
            default:
                break;
        }
    }
    /// <summary>
    /// 渠道信息下载完毕
    /// </summary>
    static void OnPlatformOver(HTTPRequest req, HTTPResponse resp)
    {
        switch (req.State)
        {
            case HTTPRequestStates.Initial:
                break;
            case HTTPRequestStates.Queued:
                break;
            case HTTPRequestStates.Processing:
                break;
            case HTTPRequestStates.Finished:
                Debug.Log("platform.xml download complete! " + resp.DataAsText);
                //下载完毕
                StringReader reader = new StringReader(resp.DataAsText);
                XmlDocument xmlDoc = new XmlDocument();
                xmlDoc.Load(reader);
                XmlNode root = xmlDoc.SelectSingleNode("iplist");
                if (root.HasChildNodes)
                {
                    foreach (XmlNode item in root.ChildNodes)
                    {
                        //ShareSDKTool.instance.mServerPortList.Add(uint.Parse(item.Attributes.Item(0).Value), item.Attributes.Item(1).Value);
                    }
                }
                reader.Close();
                OnConfigDownload();     //配置文件下载完毕
                break;
            case HTTPRequestStates.Error:
                break;
            case HTTPRequestStates.Aborted:
                break;
            case HTTPRequestStates.ConnectionTimedOut:
                break;
            case HTTPRequestStates.TimedOut:
                break;
            default:
                break;
        }
    }
    /// <summary>
    /// 版本信息下载完毕
    /// </summary>
    static void OnVersionesOver(HTTPRequest req, HTTPResponse resp)
    {
        switch (req.State)
        {
            case HTTPRequestStates.Initial:
                break;
            case HTTPRequestStates.Queued:
                break;
            case HTTPRequestStates.Processing:
                break;
            case HTTPRequestStates.Finished:
                //下载完毕
                Debug.Log("Version.txt download complete! " + resp.DataAsText);
                //版本文件下载完毕
                mVersion = resp.DataAsText;
                OnConfigDownload();     //配置文件下载完毕
                break;
            case HTTPRequestStates.Error:
                break;
            case HTTPRequestStates.Aborted:
                break;
            case HTTPRequestStates.ConnectionTimedOut:
                break;
            case HTTPRequestStates.TimedOut:
                break;
            default:
                break;
        }
    }

    /// <summary>
    /// 下载完毕
    /// </summary>
    static void OnUpdateOver(HTTPRequest req, HTTPResponse resp)
    {
        switch (req.State)
        {
            case HTTPRequestStates.Finished:
                if (resp.IsSuccess)
                {
                    if (_isUncompress)
                    {
                        ZipTool.Zip(ZipTool.ZipType.UncompressBySevenZipByte, ZipOver, resp.Data);
                    }
                    else
                    {
                        CreateFile(resp.Data);
                    }
                }
                else
                {
                    Debug.LogWarning(string.Format("下载完毕，但是服务器发送了个错误信息： Status Code: {0}-{1} Message: {2}",
                                                    resp.StatusCode,
                                                    resp.Message,
                                                    resp.DataAsText));
                }
                break;
            case HTTPRequestStates.Error:
                Debug.LogError("下载遇到错误： " + (req.Exception != null ? (req.Exception.Message + "\n" + req.Exception.StackTrace) : "无异常"));
                break;
            case HTTPRequestStates.Aborted:
                Debug.LogWarning("请求失效！");
                break;
            case HTTPRequestStates.ConnectionTimedOut:
                Debug.LogError("连接目标地址超时！");
                break;
            case HTTPRequestStates.TimedOut:
                Debug.LogError("下载处理超时！");
                break;
        }
    }
    /// <summary>
    /// 解压完毕回调
    /// </summary>
    static void ZipOver(object data)
    {
        CreateFile(data as byte[]);
    }
    #endregion
}
