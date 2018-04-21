using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using System.Xml;
using System.Text;
using System.IO;
using System;

/// <summary>
/// 外部调用总方法
/// </summary>
public class NetCmdXmlParser
{
    public const string fileNameNodeStr = "AllDefine/FileName";         //XML AllDefine节点下的要创建文件的名称
    public const string CmdXmlPath = "Assets/UtilMiddle/NetCmdXml/";    //XML源文件所在路径
    //lua脚本导出路径
    public static string targetLuaFileBasePath
    {
        get
        {
            return Application.dataPath + "/XLua/Lua/NetCmd";
        }
    }
    //C#脚本导出路径
    public static string targetFileBasePath
    {
        get
        {
            return Application.dataPath + "/Script/NetCmd";
        }
    }

    #region Tools调用的方法
    [MenuItem("Tools/导出选中xml的NetCmd脚本", false, 301)]
    static private void ExportCmdXmlChoice()
    {
        var _objs = Selection.activeObject;     //当前选中文件
        ParseAllCmdXml(_objs.name);             //解析当前选中的xml
        AssetDatabase.Refresh();
    }
    [MenuItem("Tools/导出所有xml的NetCmd脚本", false, 302)]
    static private void ExportCmdXmlTest()
    {
        if (EditorUtility.DisplayDialog("提示", "导出会覆盖除单个_Handler外的所有文件，确定吗？", "确定", "取消"))
        {
            DeleteFiles(targetLuaFileBasePath);     //lua目录下的都有删除再重新建文件
            LuaScriptParserTool.BeginLuaParser();   //清空lua handler的所有引用数据
            ParseAllCmdXml();           //解析所有的 消息cmd的xml文件
            AssetDatabase.Refresh();
        }
    }
    #endregion

    //解析所有的 消息cmd的xml文件
    public static void ParseAllCmdXml(string name = "")
    {
        CmdHandlerFileTool.CreateTypeDic();
        string[] _fils = Directory.GetFiles(CmdXmlPath, "*.xml", SearchOption.AllDirectories);      //获取当前工程下的所有xml
        foreach (string filePath in _fils)
        {
            if (filePath.EndsWith("xml"))
            {
                if (!string.IsNullOrEmpty(name))
                {
                    if (!filePath.EndsWith("\\" + name + ".xml"))
                    {
                        continue;
                    }
                    else
                    {
                        Debug.Log("找到文件了");
                    }
                }
                string targetFilePath = filePath.Replace("\\", "/");
                string fileName = targetFilePath.Substring(targetFilePath.LastIndexOf("/") + 1);
                string filePreName = fileName.Substring(0, fileName.IndexOf("_"));      //获取当前数据类型
                ICmdXmlParser targetParser = null;
                if (filePreName == "Const")
                {
                    targetParser = new ConstXmlParser();
                }
                else if (filePreName == "Data")
                {
                    targetParser = new DataXmlParser();
                }
                else if (filePreName == "Enum")
                {
                    targetParser = new EnumXmlParser();
                }
                else if (filePreName == "Base")
                {
                    targetParser = new BaseXmlParser();
                }
                else if (filePreName == "Cmd")
                {
                    targetParser = new CmdXmlParser();
                }
                if (targetParser != null)
                {
                    if (fileName.Contains("stMapDataMapScreenMobileClientCmd"))     //这个协议复杂度过高，无法自动生成，忽略掉
                    {
                        continue;
                    }
                    //解析当前文件
                    targetParser.ParserXml(filePath, !string.IsNullOrEmpty(name));
                }
            }
        }
        //处理最后所有的 消息分发 写文件
        if (string.IsNullOrEmpty(name))
        {
            //创建总的handler文件
            CmdHandlerFileTool.sWriteNetHandlerFile();
            //创建lua的stNullClientCmd文件，其实不创建也可
            StringBuilder file = new StringBuilder();
            file.Append(LuaScriptParserTool.CreateNullClientCmd());     //创建消息根节点
            string outFilePath_lua = NetCmdXmlParser.targetLuaFileBasePath + "/" + LuaScriptParserTool.CmdBaseDirectory + "/stNullClientCmd.lua";
            NetCmdXmlParser.sWriteStringToFile(outFilePath_lua, file);
        }
    }

    //写文件头
    static public void sWriteFileHeadComment(StringBuilder sb, string fileName, string fileDescribe)
    {
        sb.AppendLine("#region 模块信息");
        sb.AppendLine("/*----------------------------------------------------------------");
        sb.AppendLine("// Copyright (C) 2017 天津，颐博");
        sb.AppendLine("//");
        sb.AppendLine("// 模块名：" + fileName);
        sb.AppendLine("// 创建者：机器生成");
        sb.AppendLine("// 修改者列表：");
        sb.AppendLine("// 创建日期： 无");
        sb.AppendLine("// 模块描述：" + fileDescribe);
        sb.AppendLine("//----------------------------------------------------------------*/");
        sb.AppendLine("#endregion");
        sb.AppendLine();
    }

    //Lua写文件头
    static public void sWriteLuaFileHeadComment(StringBuilder sb, string fileName, string fileDescribe)
    {
        sb.AppendLine("-- 模块信息");
        sb.AppendLine("------------------------------------------------------------------");
        sb.AppendLine("-- Copyright (C) 2017 天津，颐博");
        sb.AppendLine("--");
        sb.AppendLine("-- 模块名：" + fileName);
        sb.AppendLine("-- 创建者：机器生成");
        sb.AppendLine("-- 修改者列表：");
        sb.AppendLine("-- 创建日期： 无");
        sb.AppendLine("-- 模块描述：" + fileDescribe);
        sb.AppendLine("------------------------------------------------------------------*/");
        sb.AppendLine();
    }

    //写命名空间引用
    static public void sWriteFileHeadNameSpace(StringBuilder sb)
    {
        sb.AppendLine("using System;");
        sb.AppendLine("using System.Collections;");
        sb.AppendLine("using System.Collections.Generic;");
        sb.AppendLine("using UnityEngine;");
        sb.AppendLine();
    }

    /// <summary>
    /// 写文件
    /// </summary>
    static public void sWriteStringToFile(string filePath, StringBuilder sb, bool append = false)
    {
        string dirPath = filePath.Substring(0, filePath.LastIndexOf("/"));
        if (!Directory.Exists(dirPath))
            Directory.CreateDirectory(dirPath);
        using (StreamWriter textWriter = new StreamWriter(filePath, append, Encoding.UTF8))
        {
            textWriter.Write(sb.ToString());
            textWriter.Flush();
            textWriter.Close();
        }
        Debug.Log("写入文件成功： " + filePath);
    }

    /// <summary>
    /// 删文件
    /// </summary>
    static private bool DeleteFiles(string path)
    {
        if (Directory.Exists(path) == false)
        {
            return false;
        }
        try
        {
            DirectoryInfo dir = new DirectoryInfo(path);
            FileSystemInfo[] fileinfo = dir.GetFileSystemInfos();  //返回目录中所有文件和子目录
            foreach (FileSystemInfo i in fileinfo)
            {
                if (i is DirectoryInfo)            //判断是否文件夹
                {
                    DirectoryInfo subdir = new DirectoryInfo(i.FullName);
                    subdir.Delete(true);          //删除子目录和文件
                }
                else
                {
                    File.Delete(i.FullName);      //删除指定文件
                }
            }
            return true;
        }
        catch (Exception)
        {
            Debug.LogError("Delete Failed!");
            return false;
        }
    }
}
