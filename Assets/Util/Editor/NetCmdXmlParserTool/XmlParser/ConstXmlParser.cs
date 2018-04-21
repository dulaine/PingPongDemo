using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Xml;
using System.Text;
using System.IO;
using System;

/// <summary>
/// Const XML 数据解析
/// </summary>
public class ConstXmlParser : ICmdXmlParser
{
    //节点路径
    private const string constNodeStr = "AllDefine/ConstData";          //常量节点 节点
    //其他
    private const string constNodeNameStr = "ConstValue";               //常量节点名字
    private const string constPackageName = "NetConst";
    private const string constOutFilePath = "/" + constPackageName + "/";             //常量输出路径

    /// <summary>
    /// 单个Const内容
    /// </summary>
    public class ConstData
    {
        public string mType = "";
        public string mName = "";
        public string mValue = "";
        public string mComment = "";
        public ConstData(string type, string name, string value, string comment)
        {
            mType = type;
            mName = name;
            mValue = value;
            mComment = comment;
        }
    }

    /// <summary>
    /// 文件结构
    /// </summary>
    public class ConstFileData
    {
        public string mClassName = "";
        public List<ConstData> mConstList = new List<ConstData>();
    }

    /// <summary>
    /// 接口实现
    /// 解析Const XML
    /// </summary>
    public void ParserXml(string filePath, bool isOneFile = false)
    {
        XmlDocument doc = new XmlDocument();
        doc.Load(filePath);
        ConstFileData targetFileData = new ConstFileData();
        //文件名
        XmlNode fileNameNode = doc.SelectSingleNode(NetCmdXmlParser.fileNameNodeStr);
        targetFileData.mClassName = fileNameNode.InnerText;
        //所有常量
        XmlNode constNode = doc.SelectSingleNode(constNodeStr);
        XmlNodeList constNodeList = constNode.ChildNodes;
        for (int i = 0; i < constNode.ChildNodes.Count; i++)
        {
            if (constNodeList[i].Name == constNodeNameStr)
            {
                XmlElement element = (XmlElement)constNodeList[i];
                string type = element.GetAttribute("type");
                string name = element.GetAttribute("name");
                string value = element.GetAttribute("value");
                string comment = element.GetAttribute("comment");
                targetFileData.mConstList.Add(new ConstData(type, name, value, comment));
            }
        }
        sWriteFile(targetFileData);
    }

    //----------------------内部调用-------------------------
    #region 内部调用
    /// <summary>
    /// 生成Const脚本
    /// </summary>
    void sWriteFile(ConstFileData fileData)
    {
        //生成C#脚本
        StringBuilder sb = new StringBuilder();
        NetCmdXmlParser.sWriteFileHeadComment(sb, fileData.mClassName, "网络消息所用常量");     //生成注释的文件头
        NetCmdXmlParser.sWriteFileHeadNameSpace(sb);        //写命名空间
        sWriteCSFileContent(sb, fileData);      //将解析到的xml生成string，并赋值到sb
        //写文件
        string outFilePath = NetCmdXmlParser.targetFileBasePath + constOutFilePath + fileData.mClassName + ".cs";
        NetCmdXmlParser.sWriteStringToFile(outFilePath, sb);

        //生成Lua脚本
        StringBuilder lua_sb = new StringBuilder();
        NetCmdXmlParser.sWriteLuaFileHeadComment(lua_sb, fileData.mClassName, "网络消息所用常量");      //生成注释的文件头
        sWriteLuaFileContent(lua_sb, fileData); //将解析到的xml生成string，并赋值到sb
        //写文件
        string outLuaFilePath = NetCmdXmlParser.targetLuaFileBasePath + constOutFilePath + fileData.mClassName + ".lua";
        NetCmdXmlParser.sWriteStringToFile(outLuaFilePath, lua_sb);
                                                                                 
        //放入NetCmdLuaHandler中进行引用，以便加载
        LuaScriptParserTool.AddLuaRequireString(constPackageName + "." + fileData.mClassName);      //加入const的引用
    }

    /// <summary>
    /// C# 将解析到的xml生成string，并赋值到sb
    /// </summary>
    void sWriteCSFileContent(StringBuilder sb, ConstFileData fileData)
    {
        sb.AppendLine("public class " + fileData.mClassName);
        sb.AppendLine("{");
        for (int i = 0; i < fileData.mConstList.Count; ++i)
        {
            ConstData tempData = fileData.mConstList[i];
            string target = "\tpublic const {0} {1} = {2}; \t\t\t\t//{3}";
            sb.AppendFormat(target, tempData.mType, tempData.mName, tempData.mValue, tempData.mComment);
            sb.Append("\n");
        }
        sb.AppendLine("}");
    }

    /// <summary>
    /// Lua 将解析到的xml生成string，并赋值到sb
    /// </summary>
    void sWriteLuaFileContent(StringBuilder lua_db, ConstFileData fileData)
    {
        lua_db.AppendLine(fileData.mClassName + " = class(\"" + fileData.mClassName + "\");");
        lua_db.AppendLine(fileData.mClassName + ".__index = " + fileData.mClassName + ";");
        lua_db.AppendLine();
        for (int i = 0; i < fileData.mConstList.Count; ++i)
        {
            ConstData tempData = fileData.mConstList[i];
            string target = "{0}.{1} = {2}; \t\t\t\t--{3}";
            lua_db.AppendFormat(target, fileData.mClassName, tempData.mName, tempData.mValue, tempData.mComment);
            lua_db.Append("\n");
        }
    }
    #endregion
}
