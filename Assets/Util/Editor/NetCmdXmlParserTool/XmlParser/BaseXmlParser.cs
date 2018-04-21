using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Xml;
using System.Text;
using System.IO;
using System;

/// <summary>
/// 基类XML数据解析
/// 也支持单导出Lua/C#脚本或都导出
/// </summary>
public class BaseXmlParser : ICmdXmlParser
{
    //节点路径
    private const string baseCmdNodeStr = "AllDefine/CmdData";              //消息基类节点 节点
    private const string baseCmdDesNodeStr = "AllDefine/Comment";           //枚举描述 节点
    private const string baseCmdFatherNodeStr = "AllDefine/Father";         //父类 节点
    private const string baseCmdGscmdNodeStr = "AllDefine/Gscmd";           //消息基类消息值命名 节点
    private const string baseCmdNumNodeStr = "AllDefine/Num";               //消息基类消息值 节点
    private const string baseCmdIsNeedCNodeStr = "AllDefine/IsNeedC";       //是否需要生成C#脚本 节点
    private const string baseCmdIsNeedLuaNodeStr = "AllDefine/IsNeedLua";   //是否需要生成Lua脚本 节点
    //其他
    private const string baseCmdNodeNameStr = "CmdParam";                   //消息基类节点名字
    private const string baseCmdOutFilePath = "/"+LuaScriptParserTool.CmdBaseDirectory+"/";     //消息基类输出路径

    /// <summary>
    /// 文件结构
    /// </summary>
    public class AllBaseCmdFileData
    {
        public string mClassName = "";      //类名
        public string mDescribe = "";       //描述
        public string mFatherName = "";     //父类名称
        public string mGsCmd = "";          //此基类的协议消息名称
        public string mNum = "";            //此基类的协议消息ID
        public bool mIsCS = true;           //是否导出C#
        public bool mIsLua = true;          //是否导出Lua
        public List<ParamData> mAllBaseCmdList = new List<ParamData>();
    }

    /// <summary>
    /// 接口实现
    /// 解析消息基类XML
    /// </summary>
    public void ParserXml(string filePath, bool isOneFile = false)
    {
        XmlDocument doc = new XmlDocument();
        doc.Load(filePath);
        AllBaseCmdFileData targetFileData = new AllBaseCmdFileData();
        //文件名
        XmlNode fileNameNode = doc.SelectSingleNode(NetCmdXmlParser.fileNameNodeStr);
        targetFileData.mClassName = fileNameNode.InnerText;
        //文件描述
        XmlNode commentNode = doc.SelectSingleNode(baseCmdDesNodeStr);
        targetFileData.mDescribe = commentNode.InnerText;
        //父类
        XmlNode fatherNode = doc.SelectSingleNode(baseCmdFatherNodeStr);
        targetFileData.mFatherName = fatherNode.InnerText;
        //协议消息名
        XmlNode gscmd = doc.SelectSingleNode(baseCmdGscmdNodeStr);
        targetFileData.mGsCmd = gscmd.InnerText;
        //消息值
        XmlNode num = doc.SelectSingleNode(baseCmdNumNodeStr);
        targetFileData.mNum = num.InnerText;
        //是否导出C#
        XmlNode isCS = doc.SelectSingleNode(baseCmdIsNeedCNodeStr);
        if (isCS == null)
            targetFileData.mIsCS = true;
        else
            targetFileData.mIsCS = isCS.InnerText == "true" ? true : false;
        //是否导出C#
        XmlNode isLua = doc.SelectSingleNode(baseCmdIsNeedLuaNodeStr);
        if (isLua == null)
            targetFileData.mIsLua = true;
        else
            targetFileData.mIsLua = isLua.InnerText == "true" ? true : false;
        //所有变量
        XmlNode alldataNode = doc.SelectSingleNode(baseCmdNodeStr);
        if (alldataNode!= null)
        {
            XmlNodeList alldataNodeList = alldataNode.ChildNodes;
            for (int i = 0; i < alldataNode.ChildNodes.Count; i++)
            {
                if (alldataNodeList[i].Name == baseCmdNodeNameStr)
                {
                    XmlElement element = (XmlElement)alldataNodeList[i]; ;
                    ParamData targetCmd = new ParamData();
                    targetCmd.mType = element.GetAttribute("type");
                    targetCmd.mName = element.GetAttribute("name");
                    targetCmd.mValue = element.GetAttribute("value");
                    targetCmd.mComment = element.GetAttribute("comment");
                    targetCmd.mStringLen = element.GetAttribute("strLen");
                    targetCmd.mArrayLen = element.GetAttribute("arrayLen");
                    targetCmd.ParseParamType();
                    targetFileData.mAllBaseCmdList.Add(targetCmd);
                }
            }
        }
        //解析并生成脚本
        sWriteFile(targetFileData);

        //存入根消息数据，以便生成总的handle
        CmdHandlerFileTool.CmdBigTypeData bigType = new CmdHandlerFileTool.CmdBigTypeData();
        bigType.mCmdClassName = targetFileData.mClassName;
        bigType.mCmdTypeName = targetFileData.mGsCmd;
        bigType.mIsCS = targetFileData.mIsCS;
        bigType.mIsLua = targetFileData.mIsLua;
        CmdHandlerFileTool.AddBigType(targetFileData.mClassName, bigType);
    }

    //----------------------内部调用-------------------------
    #region 内部调用
    /// <summary>
    /// 生成base脚本
    /// </summary>
    void sWriteFile(AllBaseCmdFileData fileData)
    {
        if (fileData.mIsCS)
        {
            //生成C#脚本
            StringBuilder sb = new StringBuilder();
            NetCmdXmlParser.sWriteFileHeadComment(sb, fileData.mClassName, fileData.mDescribe);     //生成注释的文件头
            NetCmdXmlParser.sWriteFileHeadNameSpace(sb);        //写命名空间引用
            sWriteCSFileContent(sb, fileData);         //将解析到的xml生成string，并赋值到sb
            //写文件
            string outFilePath = NetCmdXmlParser.targetFileBasePath + baseCmdOutFilePath + fileData.mClassName + ".cs";
            NetCmdXmlParser.sWriteStringToFile(outFilePath, sb);
        }

        if (fileData.mIsLua)
        {
            //生成Lua脚本
            StringBuilder lua_db = new StringBuilder();
            NetCmdXmlParser.sWriteLuaFileHeadComment(lua_db, fileData.mClassName, fileData.mDescribe);      //生成注释的文件头
            sWriteLuaFileContent(lua_db, fileData);  //将解析到的xml生成string，并赋值到sb
            //写Lua文件
            string outLuaFilePath = NetCmdXmlParser.targetLuaFileBasePath + baseCmdOutFilePath + fileData.mClassName + ".lua";
            NetCmdXmlParser.sWriteStringToFile(outLuaFilePath, lua_db);

            //放入NetCmdLuaHandler中进行引用，以便加载
            LuaScriptParserTool.AddLuaRequireString(LuaScriptParserTool.CmdBaseDirectory + "." + fileData.mClassName);
        }
    }

    /// <summary>
    /// C# 将解析到的xml生成string，并赋值到sb
    /// 这个类 是 唯一的 特殊性太强 直接写
    /// </summary>
    void sWriteCSFileContent(StringBuilder sb, AllBaseCmdFileData fileData)
    {
        List<string> declareStringList = new List<string>();
        List<string> packStringList = new List<string>();
        List<string> unpackStringList = new List<string>();
        for (int i = 0; i < fileData.mAllBaseCmdList.Count; ++i)
        {
            ParamDataOutString outString = CmdParserComUitls.CmdParamParserDeclare_CS(fileData.mAllBaseCmdList[i], fileData.mClassName, true);   //将ParamData转为string
            declareStringList.AddRange(outString.mOutDeclareString.ToArray());  //赋值 声明string
            packStringList.AddRange(outString.mOutPackString.ToArray());        //赋值 写入string
            unpackStringList.AddRange(outString.mOutUnpackString.ToArray());    //赋值 读取string
        }
        sb.AppendLine("public class " + fileData.mClassName + " : " + fileData.mFatherName);
        sb.AppendLine("{");
        //声明
        for (int j = 0; j < declareStringList.Count; ++j)
            sb.AppendLine("\t" + declareStringList[j]);
        sb.AppendLine();
        //写声明消息值
        sb.AppendLine("\tpublic const byte " + fileData.mGsCmd + " = " + fileData.mNum + ";");
        sb.AppendLine("");
        sb.AppendLine("\tpublic " + fileData.mClassName + "()");
        sb.AppendLine("\t{");
        sb.AppendLine("\t\tgsCmd = " + fileData.mGsCmd + ";");
        sb.AppendLine("\t}");
        sb.AppendLine("");
        //写打包
        sb.AppendLine("\tpublic override void Pack()");
        sb.AppendLine("\t{");
        sb.AppendLine("\t\tbase.Pack();");
        for (int k = 0; k < packStringList.Count; ++k)
            sb.AppendLine("\t\t" + packStringList[k]);
        sb.AppendLine("\t}");
        sb.AppendLine("");
        //写解包
        sb.AppendLine("\tpublic override void UnPack(byte[] msg)");
        sb.AppendLine("\t{");
        sb.AppendLine("\t\tbase.UnPack(msg);");
        for (int m = 0; m < unpackStringList.Count; ++m)
            sb.AppendLine("\t\t" + unpackStringList[m]);
        sb.AppendLine("\t}");
        sb.AppendLine("}");
    }

    /// <summary>
    /// Lua 将解析到的xml生成string，并赋值到sb
    /// 这个类 是 唯一的 特殊性太强 直接写
    /// </summary>
    void sWriteLuaFileContent(StringBuilder lua_db, AllBaseCmdFileData fileData)
    {
        lua_db.AppendLine("local " + fileData.mFatherName + " = require('NetCmd." + LuaScriptParserTool.CmdBaseDirectory + "." + fileData.mFatherName + "')");
        lua_db.AppendLine(fileData.mClassName + " = class(\"" + fileData.mClassName + "\"," + fileData.mFatherName + ");");
        lua_db.AppendLine();
        string target = "{0}.{1} = {2};";
        lua_db.AppendFormat(target, fileData.mClassName, fileData.mGsCmd, fileData.mNum);;
        lua_db.Append("\n");
        lua_db.AppendLine("function " + fileData.mClassName + ":ctor()");
        lua_db.AppendLine("\tself.super:ctor();");
        lua_db.AppendLine("\tself.super.gsCmd = " + fileData.mClassName + "." +fileData.mGsCmd + ";");
        lua_db.AppendLine("end");
        lua_db.AppendLine();
        //写打包
        lua_db.AppendLine("function " + fileData.mClassName + ":Pack()");
        lua_db.AppendLine("\tself.super:Pack()");
        lua_db.AppendLine("end");
        lua_db.AppendLine();
        //写解包
        lua_db.AppendLine("function " + fileData.mClassName + ":UnPack(byteArray)");
        lua_db.AppendLine("\tself.super:UnPack(byteArray)");
        lua_db.AppendLine("end");
        lua_db.AppendLine();
        lua_db.AppendLine("return "+ fileData.mClassName);
    }
    #endregion
}
