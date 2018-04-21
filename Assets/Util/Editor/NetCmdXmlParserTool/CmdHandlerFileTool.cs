using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Text;

/// <summary>
/// 总的Handler文件生成
/// </summary>
public class CmdHandlerFileTool
{

    private const string className = "NetCmdCsHandler";            //文件名字 类名
    private const string LuaHandler = "NetCmdLuaHandler";
    private const string functionName = "HandleCommond";
    private static Dictionary<string, CmdBigTypeData> mCmdBigDic = null;
    public static void CreateTypeDic()
    {
        if (mCmdBigDic == null)
            mCmdBigDic = new Dictionary<string, CmdBigTypeData>();
        mCmdBigDic.Clear();
    }
    public static CmdBigTypeData GetBigType(string cmdName)
    {
        if (mCmdBigDic == null)
            return null;
        if (!mCmdBigDic.ContainsKey(cmdName))
            return null;
        return mCmdBigDic[cmdName];
    }
    public static void AddBigType(string cmdName, CmdBigTypeData bigType)
    {
        if (mCmdBigDic != null && !mCmdBigDic.ContainsKey(cmdName))
        {
            mCmdBigDic[cmdName] = bigType;
        }
        else
        {
            Debug.LogError("错误");
        }
    }
    public  static Dictionary<string, CmdBigTypeData> GetBigDic()
    {
        return mCmdBigDic;
    }
    /// <summary>
    /// 基类结构
    /// </summary>
    public class CmdBigTypeData
    {
        public string mCmdClassName = "";       //消息 基类 名字
        public string mCmdTypeName = "";        //消息 基类 类型名字
        public string mFunctionName = "";       //方法名字
        public bool mIsCS = false;
        public bool mIsLua = false;
        public List<CmdSmallTypeData> mAllSubData = new List<CmdSmallTypeData>();       //此父消息下存在的所有子消息，方便生成总的handle文件
        public void AddSubSmallType(CmdSmallTypeData smallType)
        {
            mAllSubData.Add(smallType);
        }
    }
    /// <summary>
    /// 子类结构
    /// </summary>
    public class CmdSmallTypeData
    {
        public string mGsClassName = "";    //类名
        public string mGsParamName = "";    //子类名字
        public string mHandlerName = "";   //handler类名字
        public bool mIsCS = false;
        public bool mIsLua = false;
        public bool isNeedLua = true;
    }
    //写消息处理方法
    public static void sWriteNetHandlerFile()
    {
        //C#
        StringBuilder sb = new StringBuilder();
        NetCmdXmlParser.sWriteFileHeadComment(sb, className, "handler总文件");     //总的handle注释
        NetCmdXmlParser.sWriteFileHeadNameSpace(sb);        //加入命名空间
        sWriteCSFileContent(sb, mCmdBigDic);        //解析字典生成string
        //写文件
        string outFilePath = NetCmdXmlParser.targetFileBasePath + "/" + className + ".cs";
        NetCmdXmlParser.sWriteStringToFile(outFilePath, sb);
        //Lua
        StringBuilder sb_lua = new StringBuilder();
        NetCmdXmlParser.sWriteLuaFileHeadComment(sb_lua, LuaHandler, "handler总文件");     //总的handle注释
        sWriteLuaFileContent(sb_lua, mCmdBigDic);   //解析字典生成string
        //写文件
        string outFilePath_lua = NetCmdXmlParser.targetLuaFileBasePath + "/" + LuaHandler + ".lua";
        NetCmdXmlParser.sWriteStringToFile(outFilePath_lua, sb_lua);
    }
    #region C#部分
    /// <summary>
    /// 解析c#的总handle，解析字典生成string
    /// </summary>
    static void sWriteCSFileContent(StringBuilder sb, Dictionary<string, CmdBigTypeData> cmdbigTypeList)
    {
        sb.AppendLine("public class " + className);
        sb.AppendLine("{");
        sWriteFileBigType(sb, cmdbigTypeList);      //解析基类
        sWriteFileSmallType(sb, cmdbigTypeList);    //解析所有子类
        sb.AppendLine("}");
    }

    //写大解析
    static void sWriteFileBigType(StringBuilder sb, Dictionary<string, CmdBigTypeData> cmdbigTypeList)
    {
        sb.AppendLine("\tstatic public void " + functionName + "(byte[] msg)");
        sb.AppendLine("\t{");
        sb.AppendLine("\t\tif (msg.Length < 6)");
        sb.AppendLine("\t\t\tDebug.LogError(\"消息包数据长度过小\");");
        sb.AppendLine();
        sb.AppendLine("\t\tbyte gsCmd = msg[0];");
        sb.AppendLine("\t\tbyte gsParamType = msg[5];");
        sb.AppendLine();
        sb.AppendLine("\t\tswitch (gsCmd)");
        sb.AppendLine("\t\t{");
        foreach (KeyValuePair<string, CmdBigTypeData> iterType in cmdbigTypeList)
        {
            CmdBigTypeData bigType = iterType.Value;
            //判断是否生成C#的handle文件中
            if (!bigType.mIsCS)
            {
                continue;
            }
            string targetDefine = "\t\t\tcase {0}.{1}:";
            sb.AppendLine(string.Format(targetDefine, bigType.mCmdClassName, bigType.mCmdTypeName));
            string targetFun = "\t\t\t\tParse{0}(gsCmd, gsParamType, msg);";
            sb.AppendLine(string.Format(targetFun, bigType.mCmdClassName));
            sb.AppendLine("\t\t\t\tbreak;");
        }
        sb.AppendLine("\t\t\tdefault : ");
        sb.AppendLine("\t\t\t\t//C#没有此消息的处理方法，尝试让Lua处理");
        sb.AppendLine("\t\t\t\tLuaTool.Instance.CallFunction(\"NetCmdLuaHandler.HandleCommond\", gsCmd, gsParamType, msg);");
        sb.AppendLine("\t\t\t\tbreak;");
        sb.AppendLine("\t\t}");
        sb.AppendLine("\t}");
    }
    //写小解析
    public static void sWriteFileSmallType(StringBuilder sb, Dictionary<string, CmdBigTypeData> cmdbigTypeList)
    {
        foreach (KeyValuePair<string, CmdBigTypeData> iterType in cmdbigTypeList)
        {
            CmdBigTypeData bigType = iterType.Value;
            //判断是否生成C#的handle文件中
            if (!bigType.mIsCS)
            {
                continue;
            }
            sb.AppendLine();
            string targetFun = "\tstatic void Parse{0}(byte gsCmd, byte gsParamType, byte[] msg)";
            sb.AppendLine(string.Format(targetFun, bigType.mCmdClassName));
            sb.AppendLine("\t{");
            sb.AppendLine("\t\tswitch (gsParamType)");
            sb.AppendLine("\t\t{");
            for (int j = 0; j < bigType.mAllSubData.Count; ++j)
            {
                CmdSmallTypeData subType = bigType.mAllSubData[j];
                //判断是否生成C#的handle文件中
                if (!subType.mIsCS)
                {
                    continue;
                }
                string targetDefine = "\t\t\tcase {0}_S.{1}:";
                sb.AppendLine(string.Format(targetDefine, subType.mGsClassName, subType.mGsParamName));
                string targetHandler = "\t\t\t\t{0}_Handler.{1}(msg);";
                sb.AppendLine(string.Format(targetHandler, subType.mGsClassName, CmdXmlParser.cmdHandlerFuncName));
                sb.AppendLine("\t\t\t\tbreak;");
            }
            sb.AppendLine("\t\t\tdefault : ");
            sb.AppendLine("\t\t\t\t//C#没有此消息的处理方法，尝试让Lua处理");
            sb.AppendLine("\t\t\t\tLuaTool.Instance.CallFunction(\"NetCmdLuaHandler.HandleCommond\", gsCmd, gsParamType, msg);");
            sb.AppendLine("\t\t\t\tbreak;");
            sb.AppendLine("\t\t}");
            sb.AppendLine("\t}");
        }
    }
    #endregion

    #region Lua
    //写常量类的内容
    public static void sWriteLuaFileContent(StringBuilder sb, Dictionary<string, CmdBigTypeData> cmdbigTypeList)
    {
        sb.AppendLine("require  \"NetCmd.NetBase.stNullClientCmd\"");       //引用基类
        sb.AppendLine("require  \"NetCmd.NetEnum.NetEnum\"");        //引用统一在一起的NetEnum
        sb.Append(LuaScriptParserTool.GetLuaRequireString());       //lua的handle需要的所有引用
        sb.AppendLine();
        sb.AppendLine(LuaHandler + " = {}");
        sb.AppendLine("local this =" + LuaHandler);
        sb.AppendLine();
        sWriteFileBigType_Lua(sb, cmdbigTypeList);
        sWriteFileSmallType_Lua(sb, cmdbigTypeList);
    }
    //写大解析
    public static void sWriteFileBigType_Lua(StringBuilder sb, Dictionary<string, CmdBigTypeData> cmdbigTypeList)
    {
        sb.AppendLine("function " + LuaHandler + "." + functionName + "(gsCmd,gsParam,msgBytes)");
        sb.AppendLine("\t\t\tif gsCmd == nil then");
        foreach (KeyValuePair<string, CmdBigTypeData> iterType in cmdbigTypeList)
        {
            CmdBigTypeData bigType = iterType.Value;
            string targetDefine = "\t\t\telseif gsCmd == {0}.{1} then ";
            sb.AppendLine(string.Format(targetDefine, bigType.mCmdClassName, bigType.mCmdTypeName));
            string targetFun = "\t\t\t\t this.Parse{0}(gsCmd,gsParam,msgBytes);";
            sb.AppendLine(string.Format(targetFun, bigType.mCmdClassName));
        }
        sb.AppendLine("\t\t\tend");
        sb.AppendLine("end");
    }
    //写小解析
    public static void sWriteFileSmallType_Lua(StringBuilder sb, Dictionary<string, CmdBigTypeData> cmdbigTypeList)
    {
        foreach (KeyValuePair<string, CmdBigTypeData> iterType in cmdbigTypeList)
        {
            CmdBigTypeData bigType = iterType.Value;
            sb.AppendLine();
            string targetFun = "function " + LuaHandler + ".Parse{0}(gsCmd,gsParam,msgBytes)";
            sb.AppendLine(string.Format(targetFun, bigType.mCmdClassName));
            sb.AppendLine("\t\t\tif gsCmd == nil then");
            for (int j = 0; j < bigType.mAllSubData.Count; ++j)
            {
                CmdSmallTypeData subType = bigType.mAllSubData[j];
                if (!subType.isNeedLua) continue;
                string targetDefine = "\t\t\telseif gsParam == {0}_S.{1} then ";
                sb.AppendLine(string.Format(targetDefine, subType.mGsClassName, subType.mGsParamName));
                string targetHandler = "\t\t\t\t{0}_Handler.{1}(msgBytes);";
                sb.AppendLine(string.Format(targetHandler, subType.mGsClassName, CmdXmlParser.cmdHandlerFuncName));
            }
            sb.AppendLine("\t\t\tend");
            sb.AppendLine("end");
        }
    }
    #endregion
}
