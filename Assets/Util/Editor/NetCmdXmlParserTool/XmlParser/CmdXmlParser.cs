using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Xml;
using System.Text;
using System.IO;
using System;

/// <summary>
/// 普通CMD 数据解析
/// </summary>
public class CmdXmlParser : ICmdXmlParser
{
    //节点路径
    private const string cmdNodeStr = "AllDefine/CmdData";                      //消息 节点
    private const string cmdDesNodeStr = "AllDefine/Comment";                   //消息描述 节点
    private const string cmdFatherDesNodeStr = "AllDefine/Father";              //消息 父类 节点
    private const string cmdGsParamNodeStr = "AllDefine/Gsparam";               //消息 消息值命名 节点
    private const string cmdPathNameStr = "AllDefine/PathName";                 //消息放置路径文件夹名字 节点
    private const string cmdIsUnPackNodeStr = "AllDefine/IsUnPack";             //是否解包 节点
    private const string cmdIsPackNodeStr = "AllDefine/IsPack";                 //是否发包 节点
    private const string cmdIsHandlerNodeStr = "AllDefine/IsHandler";           //是否Handler 节点
    private const string cmdIsNeedCNodeStr = "AllDefine/IsNeedC";               //是否需要生成C#脚本 节点
    private const string cmdIsNeedLuaNodeStr = "AllDefine/IsNeedLua";           //是否需要生成Lua脚本 节点
    private const string cmdNumNodeStr = "AllDefine/Num";                       //子消息ID 节点
    private const string cmdFatherNumNodeStr = "AllDefine/FatherNum";           //父消息ID 节点
    //其他
    private const string cmdNodeNameStr = "Cmd";                                //消息    节点名字
    private const string cmdOutFilePathName_C = "NetCmd_C";
    private const string cmdOutFilePathName_S = "NetCmd_S";
    private const string cmdOutFilePathName_Handler = "NetCmd_Handler";
    private const string cmdOutFilePath_C = "/" + cmdOutFilePathName_C + "/";                   //消息    pack输出路径
    private const string cmdOutFilePath_S = "/" + cmdOutFilePathName_S + "/";                   //消息    unpack输出路径
    private const string cmdOutFilePath_Handler = "/" + cmdOutFilePathName_Handler + "/";       //消息    handler输出路径

    public const string cmdHandlerFuncName = "DoCmdMsg";

    /// <summary>
    /// 文件结构
    /// </summary>
    public class CmdStructFileData
    {
        public string mClassName = "";          //类名
        public string mFatherClassName = "";    //父类名
        public string mDescribe = "";       //描述
        public string mGsParam = "";        //自己的消息名称
        public string mNum = "";            //自己的消息ID
        public string mFatherNum = "";      //父消息ID
        public string mIsUnPack = "";       //是否解包
        public string mIsPack = "";         //是否打包
        public string mIsHandler = "";      //是否需要handle（通常解包的肯定要单独的handle）
        public string mIsNeedLua = "";      //是否打Lua脚本
        public string mIsNeedC = "";        //是否打C#脚本
        public string mPathName = "";       //放在哪个文件夹下
        public List<ParamData> mCmdList = new List<ParamData>();        //所有的变量
    }

    /// <summary>
    /// 接口实现
    /// 解析消息XML
    /// </summary>
    public void ParserXml(string filePath, bool isOneFile = false)
    {
        XmlDocument doc = new XmlDocument();
        doc.Load(filePath);
        CmdStructFileData targetFileData = new CmdStructFileData();
        //文件名
        XmlNode fileNameNode = doc.SelectSingleNode(NetCmdXmlParser.fileNameNodeStr);
        targetFileData.mClassName = fileNameNode.InnerText;
        //父类名
        XmlNode fileFatherNameNode = doc.SelectSingleNode(cmdFatherDesNodeStr);
        targetFileData.mFatherClassName = fileFatherNameNode.InnerText;
        //文件名注释
        XmlNode commentNode = doc.SelectSingleNode(cmdDesNodeStr);
        targetFileData.mDescribe = commentNode.InnerText;
        //协议消息名
        XmlNode gscmd = doc.SelectSingleNode(cmdGsParamNodeStr);
        targetFileData.mGsParam = gscmd.InnerText;
        //自己消息ID
        XmlNode num = doc.SelectSingleNode(cmdNumNodeStr);
        targetFileData.mNum = num.InnerText;
        //父类消息ID
        XmlNode fathernum = doc.SelectSingleNode(cmdFatherNumNodeStr);
        if (fathernum == null)
        {
            Debug.LogError("父类消息值字段解析为空" + targetFileData.mClassName);
        }
        else
        {
            targetFileData.mFatherNum = fathernum.InnerText;
        }
        //路径文件夹名字
        XmlNode pathname = doc.SelectSingleNode(cmdPathNameStr);
        if (pathname == null)
        {
            targetFileData.mPathName = "";
        }
        else
        {
            targetFileData.mPathName = pathname.InnerText;
        }
        //是否需要解包
        XmlNode isunpack = doc.SelectSingleNode(cmdIsUnPackNodeStr);
        targetFileData.mIsUnPack = isunpack.InnerText;
        //是否需要发包
        XmlNode ispack = doc.SelectSingleNode(cmdIsPackNodeStr);
        targetFileData.mIsPack = ispack.InnerText;
        //是否需要Handler包
        XmlNode ishandler = doc.SelectSingleNode(cmdIsHandlerNodeStr);
        if (ishandler == null)
        {
            targetFileData.mIsHandler = isunpack.InnerText;     //不填默认看是否解包
        }
        else
        {
            targetFileData.mIsHandler = ishandler.InnerText;
        }
        //是否需要lua
        XmlNode isneedlua = doc.SelectSingleNode(cmdIsNeedLuaNodeStr);
        targetFileData.mIsNeedLua = isneedlua.InnerText;
        //是否需要C
        XmlNode isneedc = doc.SelectSingleNode(cmdIsNeedCNodeStr);
        targetFileData.mIsNeedC = isneedc.InnerText;
        //常量
        XmlNode cmdNode = doc.SelectSingleNode(cmdNodeStr);
        XmlNodeList cmdNodeList = cmdNode.ChildNodes;
        for (int i = 0; i < cmdNode.ChildNodes.Count; i++)
        {
            if (cmdNodeList[i].Name == cmdNodeNameStr)
            {
                XmlElement element = (XmlElement)cmdNodeList[i];
                ParamData targetCmd = new ParamData();
                targetCmd.mType = element.GetAttribute("type");
                targetCmd.mName = element.GetAttribute("name");
                targetCmd.mValue = element.GetAttribute("value");
                targetCmd.mComment = element.GetAttribute("comment");
                targetCmd.mStringLen = element.GetAttribute("strLen");
                targetCmd.mArrayLen = element.GetAttribute("arrayLen");
                targetCmd.mArrayDynLen = element.GetAttribute("arrayDymanicLen");
                targetCmd.ParseParamType();
                targetFileData.mCmdList.Add(targetCmd);
            }
        }
        bool isunpack_bool = false;
        bool ispack_bool = false;
        bool ishandler_bool = false;
        bool islua_bool = false;
        bool isc_bool = false;
        if (targetFileData.mIsUnPack == "true")
        {
            isunpack_bool = true;
        }
        if (targetFileData.mIsPack == "true")
        {
            ispack_bool = true;
        }
        if (targetFileData.mIsHandler == "true")
        {
            ishandler_bool = true;
        }
        if (targetFileData.mIsNeedC == "true")
        {
            isc_bool = true;
        }
        if (targetFileData.mIsNeedLua == "true")
        {
            islua_bool = true;
        }
        sWriteFile(isunpack_bool, ispack_bool, ishandler_bool, islua_bool, isc_bool, targetFileData);
        //存入数据，方便生成总的handle文件
        if (ishandler_bool && !isOneFile)
        {
            if (isunpack_bool)
            {
                CmdHandlerFileTool.CmdSmallTypeData smallType = new CmdHandlerFileTool.CmdSmallTypeData();
                smallType.mGsClassName = targetFileData.mClassName;
                smallType.mGsParamName = targetFileData.mGsParam;
                smallType.mIsCS = isc_bool;
                smallType.mIsLua = islua_bool;
                smallType.isNeedLua = islua_bool;
                CmdHandlerFileTool.CmdBigTypeData ownerBigType = CmdHandlerFileTool.GetBigType(targetFileData.mFatherClassName);
                ownerBigType.AddSubSmallType(smallType);
            }
        }
    }

    //----------------------内部调用-------------------------
    #region 内部调用
    /// <summary>
    /// 生成消息脚本
    /// </summary>
    void sWriteFile(bool isCreatUnPack, bool isCreatPack, bool isCreatHandler, bool isCreatLua, bool isCreatC, CmdStructFileData fileData)
    {
        if (isCreatPack)
        {
            if (isCreatC) sWriteCSFileContent("C#_C", fileData);
            if (isCreatLua) sWriteLuaFileContent("Lua_C", fileData);
        }
        if (isCreatUnPack)
        {
            if (isCreatC) sWriteCSFileContent("C#_S", fileData);
            if (isCreatLua) sWriteLuaFileContent("Lua_S", fileData);
        }
        if (isCreatHandler)
        {
            if (isCreatC) sWriteCSFileContent("C#_Handler", fileData);
            if (isCreatLua) sWriteLuaFileContent("Lua_Handler", fileData);
        }
    }

    #region C#
    void sWriteCSFileContent(string fileType, CmdStructFileData fileData)
    {
        StringBuilder sb = new StringBuilder();
        NetCmdXmlParser.sWriteFileHeadComment(sb, fileData.mClassName, fileData.mDescribe);     //生成注释的文件头
        NetCmdXmlParser.sWriteFileHeadNameSpace(sb);            //生成命名空间
        //所需XML数据
        List<string> declareStringList = new List<string>();
        List<string> packStringList = new List<string>();
        List<string> unpackStringList = new List<string>();
        List<string> ctorStringList = new List<string>();   //初始化字符串
        for (int i = 0; i < fileData.mCmdList.Count; ++i)
        {
            ParamDataOutString outString = CmdParserComUitls.CmdParamParserDeclare_CS(fileData.mCmdList[i], fileData.mClassName, true);
            declareStringList.AddRange(outString.mOutDeclareString.ToArray());
            packStringList.AddRange(outString.mOutPackString.ToArray());
            unpackStringList.AddRange(outString.mOutUnpackString.ToArray());
            ctorStringList.AddRange(outString.mOutCtorString.ToArray());
        }

        string filePathName = "";
        if (fileData.mPathName != null)
        {
            filePathName = fileData.mPathName + "/";
        }
        string outFilePath = "";  //生成文件位置
        bool _isWrite = false;
        switch (fileType)
        {
            case "C#_C":
                outFilePath = NetCmdXmlParser.targetFileBasePath + cmdOutFilePath_C + filePathName + fileData.mClassName + "_C.cs";
                sWriteCSFileContent_C(sb, fileData, declareStringList, packStringList);
                _isWrite = true;
                break;
            case "C#_S":
                outFilePath = NetCmdXmlParser.targetFileBasePath + cmdOutFilePath_S + filePathName + fileData.mClassName + "_S.cs";
                sWriteCSFileContent_S(sb, fileData, declareStringList, unpackStringList, ctorStringList);
                _isWrite = true;
                break;
            case "C#_Handler":
                outFilePath = NetCmdXmlParser.targetFileBasePath + cmdOutFilePath_Handler + filePathName + fileData.mClassName + "_Handler.cs";
                if (!File.Exists(outFilePath))
                {
                    sWriteCSFileContent_Handler(sb, fileData);
                    _isWrite = true;
                }
                else
                {
                    return;
                }
                break;
        }
        if (_isWrite)
        {
            NetCmdXmlParser.sWriteStringToFile(outFilePath, sb);
        }
    }
    /// <summary>
    /// 发包
    /// </summary>
    void sWriteCSFileContent_C(StringBuilder sb, CmdStructFileData fileData, List<string> declareStringList, List<string> packStringList)
    {
        sb.AppendLine("public class " + fileData.mClassName + "_C : " + fileData.mFatherClassName);
        sb.AppendLine("{");
        sb.AppendLine("\tpublic const byte " + fileData.mGsParam + " = " + fileData.mNum + ";");
        sb.AppendLine();
        //声明
        for (int j = 0; j < declareStringList.Count; ++j)
            sb.AppendLine("\t" + declareStringList[j]);
        sb.AppendLine();
        //初始化
        sb.AppendLine("\tpublic " + fileData.mClassName + "_C()");
        sb.AppendLine("\t{");
        sb.AppendLine("\t\tgsParam = " + fileData.mGsParam + ";");
        sb.AppendLine("\t}");
        sb.AppendLine("");
        //PackData 打包
        sb.AppendLine("\tpublic override void Pack()");
        sb.AppendLine("\t{");
        sb.AppendLine("\t\tbase.Pack();");
        for (int k = 0; k < packStringList.Count; ++k)
            sb.AppendLine("\t\t" + packStringList[k]);
        sb.AppendLine("\t}");
        sb.AppendLine();
        sb.AppendLine("}");
        sb.AppendLine();
    }
    /// <summary>
    /// 解包
    /// </summary>
    public void sWriteCSFileContent_S(StringBuilder sb, CmdStructFileData fileData, List<string> declareStringList, List<string> unpackStringList, List<string> ctorStringList)
    {
        sb.AppendLine("public class " + fileData.mClassName + "_S : " + fileData.mFatherClassName);
        sb.AppendLine("{");
        sb.AppendLine("\tpublic const byte " + fileData.mGsParam + " = " + fileData.mNum + ";");
        sb.AppendLine();
        //声明
        for (int j = 0; j < declareStringList.Count; ++j)
            sb.AppendLine("\t" + declareStringList[j]);
        sb.AppendLine();
        //初始化
        sb.AppendLine("\tpublic " + fileData.mClassName + "_S()");
        sb.AppendLine("\t{");
        sb.AppendLine("\t\tgsParam = " + fileData.mGsParam + ";");
        for (int n = 0; n < ctorStringList.Count; ++n)
            sb.AppendLine("\t\t" + ctorStringList[n]);
        sb.AppendLine("\t}");
        sb.AppendLine("");
        sb.AppendLine("\tpublic override void UnPack(byte[] msg)");
        sb.AppendLine("\t{");
        sb.AppendLine("\t\tbase.UnPack(msg);");
        for (int m = 0; m < unpackStringList.Count; ++m)
            sb.AppendLine("\t\t" + unpackStringList[m]);
        sb.AppendLine("\t\tReleaseByteBuffer();");
        sb.AppendLine("\t}");
        sb.AppendLine("}");
        sb.AppendLine();
    }
    /// <summary>
    /// Handler
    /// </summary>
    public void sWriteCSFileContent_Handler(StringBuilder sb, CmdStructFileData fileData)
    {
        sb.AppendLine("public class " + fileData.mClassName + "_Handler");
        sb.AppendLine("{");
        sb.AppendLine("\tstatic public void " + cmdHandlerFuncName + "(byte[] msg)");
        sb.AppendLine("\t{");
        sb.AppendLine("\t\t" + fileData.mClassName + "_S cmd = new " + fileData.mClassName + "_S();");
        sb.AppendLine("\t\tcmd.UnPack(msg);");
        sb.AppendLine();
        sb.AppendLine("\t}");
        sb.AppendLine("}");
    }
    #endregion

    #region Lua
    public void sWriteLuaFileContent(string fileType, CmdStructFileData fileData)
    {
        StringBuilder sb = new StringBuilder();
        NetCmdXmlParser.sWriteLuaFileHeadComment(sb, fileData.mClassName, fileData.mDescribe);      //生成注释的文件头
        //所需XML数据
        List<string> declareStringList = new List<string>();
        List<string> packStringList = new List<string>();
        List<string> unpackStringList = new List<string>();
        List<string> ctorStringList = new List<string>();   //初始化字符串
        for (int i = 0; i < fileData.mCmdList.Count; ++i)
        {
            ParamDataOutString outString = CmdParserComUitls.CmdParamParserDeclare_Lua(fileData.mCmdList[i], fileData.mClassName, true);
            declareStringList.AddRange(outString.mOutDeclareString.ToArray());
            packStringList.AddRange(outString.mOutPackString.ToArray());
            unpackStringList.AddRange(outString.mOutUnpackString.ToArray());
            ctorStringList.AddRange(outString.mOutCtorString.ToArray());
        }
        //lua的文件直接生成到对应根目录下，防止lua打包时生成过多零碎文件
        /*
        string filePathName = "";
        if (fileData.mPathName != null)
        {
            filePathName = fileData.mPathName + "/";
        }
        */
        string outFilePath = "";  //生成文件位置
        bool _isWrite = false;
        switch (fileType)
        {
            case "Lua_C":
                outFilePath = NetCmdXmlParser.targetLuaFileBasePath + cmdOutFilePath_C + fileData.mClassName + "_C.lua";
                sWriteLuaFileContent_C(sb, fileData, declareStringList, packStringList);
                _isWrite = true;
                break;
            case "Lua_S":
                outFilePath = NetCmdXmlParser.targetLuaFileBasePath + cmdOutFilePath_S + fileData.mClassName + "_S.lua";
                LuaScriptParserTool.AddLuaRequireString(cmdOutFilePathName_S + "." + fileData.mClassName + "_S");
                sWriteLuaFileContent_S(sb, fileData, declareStringList, unpackStringList, ctorStringList);
                _isWrite = true;
                break;
            case "Lua_Handler":
                outFilePath = NetCmdXmlParser.targetLuaFileBasePath + cmdOutFilePath_Handler + fileData.mClassName + "_Handler.lua";
                LuaScriptParserTool.AddLuaRequireString(cmdOutFilePathName_Handler + "." + fileData.mClassName + "_Handler");
                if (!File.Exists(outFilePath))
                {
                    sWriteLuaFileContent_Handler(sb, fileData);
                    _isWrite = true;
                }
                else
                {
                    return;
                }
                break;
        }
        if (_isWrite)
        {
            NetCmdXmlParser.sWriteStringToFile(outFilePath, sb);
        }
    }
    /// <summary>
    /// 发包
    /// </summary>
    public void sWriteLuaFileContent_C(StringBuilder sb, CmdStructFileData fileData, List<string> declareStringList, List<string> packStringList)
    {
        sb.AppendLine("local " + fileData.mFatherClassName + " = require('NetCmd." + LuaScriptParserTool.CmdBaseDirectory + "." + fileData.mFatherClassName + "')");
        sb.AppendLine();
        string ClassName = fileData.mClassName + "_C";
        sb.AppendLine(ClassName + " = class(\"" + ClassName + "\"," + fileData.mFatherClassName + ");");
        sb.AppendLine();
        string target = "{0}.{1} = {2};";
        sb.AppendFormat(target, ClassName, fileData.mGsParam, fileData.mNum); ;
        sb.Append("\n");
        for (int j = 0; j < declareStringList.Count; ++j)
            sb.AppendLine(declareStringList[j].Replace("self", ClassName));
        sb.AppendLine();
        sb.AppendLine("function " + ClassName + ":ctor()");
        sb.AppendLine("\tself.super:ctor()");
        sb.AppendLine("\tself.super.super.gsParam = " + ClassName + "." + fileData.mGsParam + ";");
        sb.AppendLine("end");
        sb.AppendLine();
        sb.AppendLine("function " + ClassName + ":Pack()");
        sb.AppendLine("\tself.super:Pack();");
        for (int k = 0; k < packStringList.Count; ++k)
            sb.AppendLine("\t" + packStringList[k]);
        sb.AppendLine("end");
        sb.AppendLine();
        sb.AppendLine("function " + ClassName + ":UnPack(byteArray)");
        sb.AppendLine("\tself.super:UnPack(byteArray);");
        sb.AppendLine("end");
        sb.AppendLine();
        sb.AppendLine("return " + ClassName);
    }
    /// <summary>
    /// 解包
    /// </summary>
    public void sWriteLuaFileContent_S(StringBuilder sb, CmdStructFileData fileData, List<string> declareStringList, List<string> unpackStringList, List<string> ctorStringList)
    {
        sb.AppendLine("local " + fileData.mFatherClassName + " = require('NetCmd." + LuaScriptParserTool.CmdBaseDirectory + "." + fileData.mFatherClassName + "')");
        sb.AppendLine();
        string mClassName = fileData.mClassName + "_S";
        sb.AppendLine(mClassName + " = class(\"" + mClassName + "\"," + fileData.mFatherClassName + ");");
        sb.AppendLine();
        string target = "{0}.{1} = {2};";
        sb.AppendFormat(target, mClassName, fileData.mGsParam, fileData.mNum); ;
        sb.Append("\n");
        for (int j = 0; j < declareStringList.Count; ++j)
            sb.AppendLine(declareStringList[j].Replace("self", mClassName));
        sb.AppendLine();
        sb.AppendLine("function  " + mClassName + ":ctor()");
        sb.AppendLine("\tself.super:ctor()");
        sb.AppendLine("\tself.super.super.gsParam = " + mClassName + "." + fileData.mGsParam + ";");
        for (int n = 0; n < ctorStringList.Count; ++n)
            sb.AppendLine("\t" + ctorStringList[n]);
        sb.AppendLine("end");
        sb.AppendLine();
        sb.AppendLine("function " + mClassName + ":UnPack(byteArray)");
        sb.AppendLine("\tself.super:UnPack(byteArray);");
        for (int m = 0; m < unpackStringList.Count; ++m)
            sb.AppendLine("\t" + unpackStringList[m]);
        sb.Append("\tself:ReleaseByteBuffer();\n");
        sb.AppendLine("end");
        sb.AppendLine();
        sb.AppendLine("return " + mClassName);
    }
    /// <summary>
    /// Handler
    /// </summary>
    public void sWriteLuaFileContent_Handler(StringBuilder sb, CmdStructFileData fileData)
    {
        string _tempS = fileData.mClassName + "_S";
        sb.AppendLine("local " + _tempS + " = require('NetCmd." + cmdOutFilePathName_S + "." + _tempS + "')");
        sb.AppendLine();
        string className = fileData.mClassName + "_Handler";
        sb.AppendLine(className + " = class(\"" + className + "\");");
        sb.AppendLine();
        sb.AppendLine("function " + className + "." + cmdHandlerFuncName + "(msg)");
        sb.AppendLine("\tlocal cmd = " + fileData.mClassName + "_S" + ".new()");
        sb.AppendLine("\tcmd:UnPack(msg)");
        sb.AppendLine("end");
        sb.AppendLine();
        sb.AppendLine("return " + className);
    }
    #endregion
    #endregion
}
