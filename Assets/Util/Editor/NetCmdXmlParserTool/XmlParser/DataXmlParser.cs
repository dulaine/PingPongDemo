using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Xml;
using System.Text;
using System.IO;
using System;

/// <summary>
/// Data XML 数据解析
/// </summary>
public class DataXmlParser : ICmdXmlParser
{
    //节点路径
    private const string paramNodeStr = "AllDefine/ParamData";          //结构体 节点
    private const string paramDesNodeStr = "AllDefine/Comment";         //结构体描述 节点
    private const string paramFatherNodeStr = "AllDefine/Father";       //结构体父级 节点
    private const string paramIsNeedCNodeStr = "AllDefine/IsNeedC";         //是否需要生成C#脚本 节点
    private const string paramIsNeedLuaNodeStr = "AllDefine/IsNeedLua";     //是否需要生成Lua脚本 节点
    //其他
    private const string paramNodeNameStr = "Param";                    //结构体节点名字
    private const string constPackageName = "NetData";
    private const string paramOutFilePath = "/" + constPackageName + "/";           //结构体输出路径

    /// <summary>
    /// 文件结构
    /// </summary>
    class DataStructFileData
    {
        public string mClassName = "";      //文件/类 名
        public string mDescribe = "";       //文件/类 注释
        public string mFather = "";         //父类文件/类 名
        public bool mIsCS = true;           //是否导出C#
        public bool mIsLua = true;          //是否导出Lua
        public List<ParamData> mParamList = new List<ParamData>();
    }

    /// <summary>
    /// 接口实现
    /// 解析Data XML
    /// </summary>
    public void ParserXml(string filePath, bool isOneFile = false)
    {
        XmlDocument doc = new XmlDocument();
        doc.Load(filePath);
        DataStructFileData targetFileData = new DataStructFileData();
        //文件名
        XmlNode fileNameNode = doc.SelectSingleNode(NetCmdXmlParser.fileNameNodeStr);
        targetFileData.mClassName = fileNameNode.InnerText;
        //注释
        XmlNode commentNode = doc.SelectSingleNode(paramDesNodeStr);
        targetFileData.mDescribe = commentNode.InnerText;
        //父级
        XmlNode fatherNode = doc.SelectSingleNode(paramFatherNodeStr);
        targetFileData.mFather = fatherNode.InnerText;
        //是否导出C#
        XmlNode isCS = doc.SelectSingleNode(paramIsNeedCNodeStr);
        if (isCS == null)
            targetFileData.mIsCS = true;
        else
            targetFileData.mIsCS = isCS.InnerText == "true" ? true : false;
        //是否导出C#
        XmlNode isLua = doc.SelectSingleNode(paramIsNeedLuaNodeStr);
        if (isLua == null)
            targetFileData.mIsLua = true;
        else
            targetFileData.mIsLua = isLua.InnerText == "true" ? true : false;
        //所有变量
        XmlNode paramNode = doc.SelectSingleNode(paramNodeStr);
        XmlNodeList paramNodeList = paramNode.ChildNodes;
        for (int i = 0; i < paramNode.ChildNodes.Count; i++)
        {
            if (paramNodeList[i].Name == paramNodeNameStr)
            {
                XmlElement element = (XmlElement)paramNodeList[i];
                ParamData targetParam = new ParamData();
                targetParam.mType = element.GetAttribute("type");
                targetParam.mName = element.GetAttribute("name");
                targetParam.mValue = element.GetAttribute("value");
                targetParam.mComment = element.GetAttribute("comment");
                targetParam.mStringLen = element.GetAttribute("strLen");
                targetParam.mArrayLen = element.GetAttribute("arrayLen");
                targetParam.mArrayDynLen = element.GetAttribute("arrayDymanicLen");
                targetParam.ParseParamType();       //设置此条数据的类型
                targetFileData.mParamList.Add(targetParam);
            }
        }
        //解析并生成脚本
        sWriteFile(targetFileData);
    }

    //----------------------内部调用-------------------------
    #region 内部调用
    /// <summary>
    /// 生成Data脚本
    /// </summary>
    void sWriteFile(DataStructFileData fileData)
    {
        if (fileData.mIsCS)
        {
            //生成C#脚本
            StringBuilder sb = new StringBuilder();
            NetCmdXmlParser.sWriteFileHeadComment(sb, fileData.mClassName, fileData.mDescribe);         //生成注释的文件头
            NetCmdXmlParser.sWriteFileHeadNameSpace(sb);    //写命名空间
            sWriteCSFileContent(sb, fileData);          //将解析到的xml生成string，并赋值到sb
            //写文件
            string outFilePath = NetCmdXmlParser.targetFileBasePath + paramOutFilePath + fileData.mClassName + ".cs";   //生成文件路径
            NetCmdXmlParser.sWriteStringToFile(outFilePath, sb);    //生成文件
        }
        if (fileData.mIsLua)
        {
            //生成Lua脚本
            StringBuilder lua_sb = new StringBuilder();
            NetCmdXmlParser.sWriteLuaFileHeadComment(lua_sb, fileData.mClassName, fileData.mDescribe);  //生成注释的文件头
            sWriteLuaFileContent(lua_sb, fileData);   //将解析到的xml生成string，并赋值到lua_sb
            //写Lua文件
            string outLuaFilePath = NetCmdXmlParser.targetLuaFileBasePath + paramOutFilePath + fileData.mClassName + ".lua";    //生成文件路径
            NetCmdXmlParser.sWriteStringToFile(outLuaFilePath, lua_sb);     //生成文件

            //放入NetCmdLuaHandler中进行引用，以便加载
            LuaScriptParserTool.AddLuaRequireString(constPackageName + "." + fileData.mClassName);
        }
    }

    /// <summary>
    /// C# 将解析到的xml生成string，并赋值到sb
    /// </summary>
    void sWriteCSFileContent(StringBuilder sb, DataStructFileData fileData)
    {
        List<string> declareStringList = new List<string>();
        List<string> packStringList = new List<string>();
        List<string> unpackStringList = new List<string>();
        for (int i = 0; i < fileData.mParamList.Count; ++i)
        {
            ParamDataOutString outString = CmdParserComUitls.CmdParamParserDeclare_CS(fileData.mParamList[i], fileData.mClassName);     //将ParamData转为string
            declareStringList.AddRange(outString.mOutDeclareString.ToArray());  //赋值 声明string
            packStringList.AddRange(outString.mOutPackString.ToArray());        //赋值 写入string
            unpackStringList.AddRange(outString.mOutUnpackString.ToArray());    //赋值 读取string
        }
        if (fileData.mFather == "NULL" || fileData.mFather == null)
        {
            sb.AppendLine("public class " + fileData.mClassName);
        }
        else
        {
            sb.AppendLine("public class " + fileData.mClassName + " : " + fileData.mFather);
        }
        sb.AppendLine("{");
        //声明变量
        for (int j = 0; j < declareStringList.Count; ++j)
            sb.AppendLine("\t" + declareStringList[j]);
        sb.AppendLine();
        //PackData 写数据
        if (fileData.mFather != "NULL" & fileData.mFather != null)
        {
            sb.AppendLine("\tpublic override void PackData(ByteBuffer byteBuffer)");
            sb.AppendLine("\t{");
            sb.AppendLine("\t\tbase.PackData(byteBuffer);");
        }
        else
        {
            sb.AppendLine("\tpublic virtual void PackData(ByteBuffer byteBuffer)");
            sb.AppendLine("\t{");
        }
        for (int k = 0; k < packStringList.Count; ++k)
        {
            sb.AppendLine("\t\t" + packStringList[k]);
        }
        sb.AppendLine("\t}");
        sb.AppendLine();
        //UnPackData 读数据
        if (fileData.mFather != "NULL" & fileData.mFather != null)
        {
            sb.AppendLine("\tpublic override void UnPackData(ByteBuffer byteBuffer)");
            sb.AppendLine("\t{");
            sb.AppendLine("\t\tbase.UnPackData(byteBuffer);");
        }
        else
        {
            sb.AppendLine("\tpublic virtual void UnPackData(ByteBuffer byteBuffer)");
            sb.AppendLine("\t{");
        }
        for (int m = 0; m < unpackStringList.Count; ++m)
            sb.AppendLine("\t\t" + unpackStringList[m]);
        sb.AppendLine("\t}");
        sb.AppendLine("}");
        sb.AppendLine();
    }

    /// <summary>
    /// Lua 将解析到的xml生成string，并赋值到sb
    /// </summary>
    void sWriteLuaFileContent(StringBuilder sb, DataStructFileData fileData)
    {
        List<string> declareStringList = new List<string>();
        List<string> ctorStringList = new List<string>();
        List<string> packStringList = new List<string>();
        List<string> unpackStringList = new List<string>();
        for (int i = 0; i < fileData.mParamList.Count; ++i)
        {
            ParamDataOutString outString = CmdParserComUitls.CmdParamParserDeclare_Lua(fileData.mParamList[i], fileData.mClassName);
            declareStringList.AddRange(outString.mOutDeclareString.ToArray());
            ctorStringList.AddRange(outString.mOutCtorString.ToArray());
            packStringList.AddRange(outString.mOutPackString.ToArray());
            unpackStringList.AddRange(outString.mOutUnpackString.ToArray());
        }
        if (fileData.mFather == "NULL" || fileData.mFather == null)
        {
            sb.AppendLine(fileData.mClassName + " = class(\"" + fileData.mClassName + "\");");
        }
        else
        {
            sb.AppendLine(fileData.mClassName + " = class(\"" + fileData.mClassName + "\"," + fileData.mFather + ");");
        }
        sb.AppendLine();
        //声明
        for (int j = 0; j < declareStringList.Count; ++j)
            sb.AppendLine("\t" + declareStringList[j].Replace("self", fileData.mClassName));
        sb.AppendLine();
        //ctor
        sb.Append("function " + fileData.mClassName + ":ctor()");
        sb.AppendLine();
        for (int i = 0; i < ctorStringList.Count; i++)
        {
            sb.Append("\n" + ctorStringList[i]);
        }
        sb.Append("\nend\n");
        sb.AppendLine();
        //PackData 打包
        sb.AppendLine("function " + fileData.mClassName + ":PackData(byteBuffer)");
        if (fileData.mFather != "NULL" & fileData.mFather != null)
        {
            sb.AppendLine("\t " + fileData.mClassName + ".super:PackData(byteBuffer);");
        }
        for (int k = 0; k < packStringList.Count; ++k)
            sb.AppendLine("\t\t" + packStringList[k].Replace("self.byteBuffer", "byteBuffer"));
        sb.Append("end");
        sb.AppendLine();
        //UnPackData
        sb.AppendLine("function " + fileData.mClassName + ":UnPackData(byteBuffer)");
        if (fileData.mFather != "NULL" & fileData.mFather != null)
        {
            sb.AppendLine("\t " + fileData.mClassName + ".super.UnPackData(self,byteBuffer);");
        }
        for (int m = 0; m < unpackStringList.Count; ++m)
        {
            sb.AppendLine("\t\t" + unpackStringList[m].Replace("self.byteBuffer", "byteBuffer"));
        }
        sb.Append("\nend\n");
        sb.AppendLine();
    }
    #endregion

}
