using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Xml;
using System.Text;
using System.IO;
using System;

/// <summary>
/// 枚举XML数据解析
/// </summary>
public class EnumXmlParser : ICmdXmlParser
{
    //节点路径
    private const string enumNodeStr = "AllDefine/EnumData";            //枚举数据 节点
    private const string enumDesNodeStr = "AllDefine/Comment";          //枚举描述 节点
    private const string enumIsNeedCNodeStr = "AllDefine/IsNeedC";          //是否需要生成C#脚本 节点
    private const string enumIsNeedLuaNodeStr = "AllDefine/IsNeedLua";      //是否需要生成Lua脚本 节点
    //其他
    private const string enumNodeNameStr = "Enum";                      //枚举节点名字
    private const string enumLuaFileName = "NetEnum";                   //lua存enum的的文件名，enum不用return故可以将enum放入一个文件，以便引用
    private const string enumOutFilePath = "/" + enumLuaFileName + "/";     //枚举输出路径

    /// <summary>
    /// 单个Enum内容
    /// </summary>
    public class EnumData
    {
        public string mName = "";
        public string mValue = "";
        public string mComment = "";

        public EnumData(string name, string value, string comment)
        {
            mName = name;
            mValue = value;
            mComment = comment;
        }
    }

    /// <summary>
    /// 文件结构
    /// </summary>
    public class EnumFileData
    {
        public string mClassName = "";
        public string mDescribe = "";
        public bool mIsCS = true;           //是否导出C#
        public bool mIsLua = true;          //是否导出Lua
        public List<EnumData> mEnumList = new List<EnumData>();
    }

    /// <summary>
    /// 接口实现
    /// 解析Enum XML
    /// </summary>
    public void ParserXml(string filePath, bool isOneFile = false)
    {
        XmlDocument doc = new XmlDocument();
        doc.Load(filePath);
        EnumFileData targetFileData = new EnumFileData();
        //文件名
        XmlNode fileNameNode = doc.SelectSingleNode(NetCmdXmlParser.fileNameNodeStr);
        targetFileData.mClassName = fileNameNode.InnerText;
        //注释
        XmlNode commentNode = doc.SelectSingleNode(enumDesNodeStr);
        targetFileData.mDescribe = commentNode.InnerText;
        //是否导出C#
        XmlNode isCS = doc.SelectSingleNode(enumIsNeedCNodeStr);
        if (isCS == null)
            targetFileData.mIsCS = true;
        else
            targetFileData.mIsCS = isCS.InnerText == "true" ? true : false;
        //是否导出C#
        XmlNode isLua = doc.SelectSingleNode(enumIsNeedLuaNodeStr);
        if (isLua == null)
            targetFileData.mIsLua = true;
        else
            targetFileData.mIsLua = isLua.InnerText == "true" ? true : false;
        //所有变量
        XmlNode enumNode = doc.SelectSingleNode(enumNodeStr);
        XmlNodeList enumNodeList = enumNode.ChildNodes;
        for (int i = 0; i < enumNode.ChildNodes.Count; i++)
        {
            if (enumNodeList[i].Name == enumNodeNameStr)
            {
                XmlElement element = (XmlElement)enumNodeList[i];
                string name = element.GetAttribute("name");
                string value = element.GetAttribute("value");
                string comment = element.GetAttribute("comment");
                targetFileData.mEnumList.Add(new EnumData(name, value, comment));
            }
        }
        //解析并生成脚本
        sWriteFile(targetFileData);
    }

    //----------------------内部调用-------------------------
    #region 内部调用
    /// <summary>
    /// 生成Enum脚本
    /// </summary>
    void sWriteFile(EnumFileData fileData)
    {
        if (fileData.mIsCS)
        {
            //生成C#脚本
            StringBuilder sb = new StringBuilder();
            NetCmdXmlParser.sWriteFileHeadComment(sb, fileData.mClassName, fileData.mDescribe);     //生成注释的文件头
            sWriteCSFileContent(sb, fileData);      //将解析到的xml生成string，并赋值到sb
            //写文件
            string outFilePath = NetCmdXmlParser.targetFileBasePath + enumOutFilePath + fileData.mClassName + ".cs";
            NetCmdXmlParser.sWriteStringToFile(outFilePath, sb);
        }

        if (fileData.mIsLua)
        {
            //生成Lua脚本
            StringBuilder lua_sb = new StringBuilder();
            NetCmdXmlParser.sWriteLuaFileHeadComment(lua_sb, fileData.mClassName, fileData.mDescribe);      //生成注释的文件头
            sWriteLuaFileContent(lua_sb, fileData); //将解析到的xml生成string，并赋值到sb
            lua_sb.AppendLine();
            //写文件
            string outLuaFilePath = NetCmdXmlParser.targetLuaFileBasePath + enumOutFilePath + enumLuaFileName + ".lua";
            NetCmdXmlParser.sWriteStringToFile(outLuaFilePath, lua_sb, true);
            
            //所有Enum都在一个文件里，统一引用即可，不用再次引用
        }
    }

    /// <summary>
    /// C# 将解析到的xml生成string，并赋值到sb
    /// </summary>
    void sWriteCSFileContent(StringBuilder sb, EnumFileData fileData)
    {
        sb.AppendLine("public enum " + fileData.mClassName);
        sb.AppendLine("{");
        for (int i = 0; i < fileData.mEnumList.Count; ++i)
        {
            EnumData tempData = fileData.mEnumList[i];
            string targetStr = "";
            //如果是NULL 为自动增加 不设定值
            if (tempData.mValue == "NULL")
            {
                targetStr = "\t{0}, \t\t\t\t// {1}";
                sb.AppendFormat(targetStr, tempData.mName, tempData.mComment);
            }
            else
            {
                targetStr = "\t{0} = {1}, \t\t\t\t// {2}";
                sb.AppendFormat(targetStr, tempData.mName, tempData.mValue, tempData.mComment);
            }
            sb.Append("\n");
        }
        sb.AppendLine("}");
    }

    /// <summary>
    /// Lua 将解析到的xml生成string，并赋值到sb
    /// </summary>
    void sWriteLuaFileContent(StringBuilder sb, EnumFileData fileData)
    {
        sb.AppendLine(fileData.mClassName + " = ");
        sb.AppendLine("{");
        int curValue = 0;
        for (int i = 0; i < fileData.mEnumList.Count; ++i)
        {
            EnumData tempData = fileData.mEnumList[i];
            string targetStr = "";
            //如果是NULL 为自动增加 不设定值
            targetStr = "\t{0} = {1}, \t\t\t\t-- {2}";
            if (tempData.mValue == "NULL")
            {
                if (i == 0)
                {
                    sb.AppendFormat(targetStr, tempData.mName, curValue, tempData.mComment);
                }
                else
                {
                    sb.AppendFormat(targetStr, tempData.mName, curValue, tempData.mComment);
                }
            }
            else
            {
                if (int.TryParse(tempData.mValue, out curValue))
                {

                }
                sb.AppendFormat(targetStr, tempData.mName, tempData.mValue, tempData.mComment);
            }
            curValue++;
            sb.Append("\n");
        }
        sb.AppendLine("}");
    }
    #endregion
}
