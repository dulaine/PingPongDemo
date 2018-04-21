using UnityEngine;
using System.Collections;
using System.Collections.Generic;

//变量类型定义
public enum EParamType
{
    eParamType_None = 0,        //类型 无
    eParamType_Byte,
    eParamType_Short,
    eParamType_UShort,
    eParamType_Int,
    eParamType_UInt,
    eParamType_Long,
    eParamType_ULong,
    eParamType_String,
    eParamType_OtherData,

    eParamType_Byte_Array,
    eParamType_Short_Array,
    eParamType_UShort_Array,
    eParamType_Int_Array,
    eParamType_UInt_Array,
    eParamType_Long_Array,
    eParamType_ULong_Array,
    eParamType_String_Array,
    eParamType_OtherData_Array,
}

//消息 数据结构 公用的 参数结构
public class ParamData
{
    public EParamType mParamType = EParamType.eParamType_None;
    public string mType = "";           //类型
    public string mName = "";           //变量名
    public string mValue = "";          //变量默认值
    public string mComment = "";        //注释
    public string mStringLen = "";      //字符长度
    public string mArrayType = "";      //数组类型
    public string mArrayLen = "";       //数组长度
    public string mArrayDynLen = "";     //变长 数组 长度变量

    public ParamData() { }

    //根据名字 解析类型
    public void ParseParamType()
    {
        if (mType.EndsWith("[]")) //数组
        {
            switch (mType)
            {
                case "byte[]":
                    mParamType = EParamType.eParamType_Byte_Array;
                    mArrayType = "byte";
                    break;
                case "short[]":
                    mParamType = EParamType.eParamType_Short_Array;
                    mArrayType = "short";
                    break;
                case "ushort[]":
                    mParamType = EParamType.eParamType_UShort_Array;
                    mArrayType = "ushort";
                    break;
                case "int[]":
                    mParamType = EParamType.eParamType_Int_Array;
                    mArrayType = "int";
                    break;
                case "uint[]":
                    mParamType = EParamType.eParamType_UInt_Array;
                    mArrayType = "uint";
                    break;
                case "long[]":
                    mParamType = EParamType.eParamType_Long_Array;
                    mArrayType = "long";
                    break;
                case "ulong[]":
                    mParamType = EParamType.eParamType_ULong_Array;
                    mArrayType = "ulong";
                    break;
                case "string[]":
                    mParamType = EParamType.eParamType_String_Array;
                    mArrayType = "string";
                    break;
                default:
                    mParamType = EParamType.eParamType_OtherData_Array;
                    mArrayType = mType.Replace("[]", "");
                    break;
            }
        }
        else    //普通数据
        {
            switch (mType)
            {
                case "byte": mParamType = EParamType.eParamType_Byte; break;
                case "short": mParamType = EParamType.eParamType_Short; break;
                case "ushort": mParamType = EParamType.eParamType_UShort; break;
                case "int": mParamType = EParamType.eParamType_Int; break;
                case "uint": mParamType = EParamType.eParamType_UInt; break;
                case "long": mParamType = EParamType.eParamType_Long; break;
                case "ulong": mParamType = EParamType.eParamType_ULong; break;
                case "string": mParamType = EParamType.eParamType_String; break;
                default: mParamType = EParamType.eParamType_OtherData; break;
            }
        }
    }
}

//输出用字符串
public class ParamDataOutString
{
    public List<string> mOutDeclareString = new List<string>();     //声明字符串
    public List<string> mOutCtorString = new List<string>();        //初始化字符串
    public List<string> mOutPackString = new List<string>();        //打包字符串
    public List<string> mOutUnpackString = new List<string>();      //解包字符串
}

/// <summary>
/// 对参数进行解析
/// </summary>
public class CmdParserComUitls
{
    private static string mCurExportFileName = "Error";
    #region CS 文件部分
    //解析  变量声明 部分  C#文件
    public static ParamDataOutString CmdParamParserDeclare_CS(ParamData tarPa, string fileName, bool isCmdType = false, bool isLua = false)
    {
        mCurExportFileName = fileName;
        ParamDataOutString outDataString = new ParamDataOutString();
        string packageParamName = "byteBuffer";
        if (isCmdType && !isLua)
            packageParamName = "mByteBuffer";
        switch (tarPa.mParamType)
        {
            case EParamType.eParamType_Byte:
                CmdParamParserByBaseType_CS("Byte", packageParamName, outDataString, tarPa);
                break;
            case EParamType.eParamType_Short:
                CmdParamParserByBaseType_CS("Short", packageParamName, outDataString, tarPa);
                break;
            case EParamType.eParamType_UShort:
                CmdParamParserByBaseType_CS("UShort", packageParamName, outDataString, tarPa);
                break;
            case EParamType.eParamType_Int:
                CmdParamParserByBaseType_CS("Int", packageParamName, outDataString, tarPa);
                break;
            case EParamType.eParamType_UInt:
                CmdParamParserByBaseType_CS("UInt", packageParamName, outDataString, tarPa);
                break;
            case EParamType.eParamType_Long:
                CmdParamParserByBaseType_CS("Long", packageParamName, outDataString, tarPa);
                break;
            case EParamType.eParamType_ULong:
                CmdParamParserByBaseType_CS("ULong", packageParamName, outDataString, tarPa);
                break;
            case EParamType.eParamType_String:
                CmdParamParserByString_CS(packageParamName, outDataString, tarPa);
                break;
            case EParamType.eParamType_OtherData:
                CmdParamParserByClass_CS(packageParamName, outDataString, tarPa);
                break;
            case EParamType.eParamType_Byte_Array:
                CmdParamParserArrayByByte_CS(packageParamName, outDataString, tarPa);
                break;
            case EParamType.eParamType_Short_Array:
                CmdParamParserArrayByBaseType_CS("Short", packageParamName, outDataString, tarPa);
                break;
            case EParamType.eParamType_UShort_Array:
                CmdParamParserArrayByBaseType_CS("UShort", packageParamName, outDataString, tarPa);
                break;
            case EParamType.eParamType_Int_Array:
                CmdParamParserArrayByBaseType_CS("Int", packageParamName, outDataString, tarPa);
                break;
            case EParamType.eParamType_UInt_Array:
                CmdParamParserArrayByBaseType_CS("UInt", packageParamName, outDataString, tarPa);
                break;
            case EParamType.eParamType_Long_Array:
                CmdParamParserArrayByBaseType_CS("Long", packageParamName, outDataString, tarPa);
                break;
            case EParamType.eParamType_ULong_Array:
                CmdParamParserArrayByBaseType_CS("ULong", packageParamName, outDataString, tarPa);
                break;
            case EParamType.eParamType_String_Array:
                CmdParamParserArrayByString_CS(packageParamName, outDataString, tarPa);
                break;
            case EParamType.eParamType_OtherData_Array:
                CmdParamParserArrayByClass_CS(packageParamName, outDataString, tarPa);
                break;
            default:
                break;
        }
        return outDataString;
    }
    //解析基础类型 根据类型
    public static void CmdParamParserByBaseType_CS(string baseTypeStr, string bufferStr, ParamDataOutString outStrData, ParamData tarPa)
    {
        string bType = baseTypeStr.ToLower();
        string strDel = "public {0} {1} = {2}; \t\t\t\t//{3}";
        outStrData.mOutDeclareString.Add(string.Format(strDel, bType, tarPa.mName, tarPa.mValue, tarPa.mComment));
        string strPack = "{0}.Write{1}({2});";
        outStrData.mOutPackString.Add(string.Format(strPack, bufferStr, baseTypeStr, tarPa.mName));
        string strUnpack = "{0} = {1}.Read{2}();";
        outStrData.mOutUnpackString.Add(string.Format(strUnpack, tarPa.mName, bufferStr, baseTypeStr));
    }
    //解析基础类型 根据类型  string
    public static void CmdParamParserByString_CS(string bufferStr, ParamDataOutString outStrData, ParamData tarPa)
    {
        string strDel = "";
        if (tarPa.mValue == "string.Empty")
        {
            strDel = "public string {0} = {1}; \t\t\t\t//{2}";
        }
        else
        {
            strDel = "public string {0} = \"{1}\"; \t\t\t\t//{2}";
        }
        outStrData.mOutDeclareString.Add(string.Format(strDel, tarPa.mName, tarPa.mValue, tarPa.mComment));
        string tarStrLen = tarPa.mStringLen;
        int strLenOut = 0;
        if (!int.TryParse(tarStrLen, out strLenOut))    //不是数字
        {
            string strPack = "{0}.WriteString({1}, (uint){2});";
            outStrData.mOutPackString.Add(string.Format(strPack, bufferStr, tarPa.mName, tarPa.mStringLen));

            string strUnpack = "{0} = {1}.ReadString((int){2});";
            outStrData.mOutUnpackString.Add(string.Format(strUnpack, tarPa.mName, bufferStr, tarPa.mStringLen));
        }
        else
        {
            string strPack = "{0}.WriteString({1}, {2});";
            outStrData.mOutPackString.Add(string.Format(strPack, bufferStr, tarPa.mName, tarPa.mStringLen));

            string strUnpack = "{0} = {1}.ReadString({2});";
            outStrData.mOutUnpackString.Add(string.Format(strUnpack, tarPa.mName, bufferStr, tarPa.mStringLen));
        }
    }
    //解析基础类型 其他数据结构 class
    public static void CmdParamParserByClass_CS(string bufferStr, ParamDataOutString outStrData, ParamData tarPa)
    {
        string strDel = "public {0} {1} = new {2}(); \t\t\t\t//{3}";
        outStrData.mOutDeclareString.Add(string.Format(strDel, tarPa.mType, tarPa.mName, tarPa.mType, tarPa.mComment));
        string strPack = "{0}.PackData({1});";
        outStrData.mOutPackString.Add(string.Format(strPack, tarPa.mName, bufferStr));
        if (string.IsNullOrEmpty(tarPa.mArrayDynLen))
        {
            string strUnpack = "{0}.UnPackData({1});";
            outStrData.mOutUnpackString.Add(string.Format(strUnpack, tarPa.mName, bufferStr));
        }
        else
        {
            string strUnpack = "";
            string format1 = "if ({0} != 0)";
            strUnpack += string.Format(format1, tarPa.mArrayDynLen);
            strUnpack += "{";
            string format2 = "{0}.UnPackData({1});";
            strUnpack += string.Format(format2, tarPa.mName, bufferStr);
            strUnpack += "}";
            outStrData.mOutUnpackString.Add(strUnpack);
        }
    }
    //解析基础类型 根据类型 数组
    public static void CmdParamParserArrayByBaseType_CS(string baseTypeStr, string bufferStr, ParamDataOutString outStrData, ParamData tarPa)
    {
        string bType = baseTypeStr.ToLower();
        if (tarPa.mArrayLen != "") //定长数组
        {
            string tarStrLen = tarPa.mArrayLen;
            int strLenOut = 0;
            if (!int.TryParse(tarStrLen, out strLenOut)) //不是数字
                tarStrLen = "(int)" + tarPa.mArrayLen;
            string strDel = "public {0}[] {1} = new {2}[{3}]; \t\t\t\t// {4}";
            outStrData.mOutDeclareString.Add(string.Format(strDel, bType, tarPa.mName, bType, tarStrLen, tarPa.mComment));
            string strPack1 = "for(int i = 0; i < {0}.Length; ++i)";
            outStrData.mOutPackString.Add(string.Format(strPack1, tarPa.mName));
            string strPack2 = "\t{0}.Write{1}({2}[i]);";
            outStrData.mOutPackString.Add(string.Format(strPack2, bufferStr, baseTypeStr, tarPa.mName));
            string strUnpack1 = "for(int i = 0; i < {0}.Length; ++i)";
            outStrData.mOutUnpackString.Add(string.Format(strUnpack1, tarPa.mName));
            string strUnpack2 = "\t{0}[i] = {1}.Read{2}();";
            outStrData.mOutUnpackString.Add(string.Format(strUnpack2, tarPa.mName, bufferStr, baseTypeStr));
        }
        else if (tarPa.mArrayDynLen != "") //变长数组
        {
            string strDel = "public {0}[] {1} = null; \t\t\t\t// {2}";
            outStrData.mOutDeclareString.Add(string.Format(strDel, bType, tarPa.mName, tarPa.mComment));
            string strPack1 = "for(int i = 0; i < {0}; ++i)";
            outStrData.mOutPackString.Add(string.Format(strPack1, tarPa.mArrayDynLen));
            string strPack2 = "\t{0}.Write{1}({2}[i]);";
            outStrData.mOutPackString.Add(string.Format(strPack2, bufferStr, baseTypeStr, tarPa.mName));
            string strUnpack1 = "{0} = new {1}[{2}];";
            outStrData.mOutUnpackString.Add(string.Format(strUnpack1, tarPa.mName, bType, tarPa.mArrayDynLen));
            string strUnpack2 = "for(int i = 0; i < {0}; ++i)";
            outStrData.mOutUnpackString.Add(string.Format(strUnpack2, tarPa.mArrayDynLen));
            string strUnpack3 = "\t{0}[i] = {1}.Read{2}();";
            outStrData.mOutUnpackString.Add(string.Format(strUnpack3, tarPa.mName, bufferStr, baseTypeStr));
        }
        else
        {
            Debug.LogError("导出消息数组类型长度错误：" + mCurExportFileName);
        }
    }
    //解析基础类型 根据类型 数组 string
    public static void CmdParamParserArrayByString_CS(string bufferStr, ParamDataOutString outStrData, ParamData tarPa)
    {
        if (tarPa.mArrayLen != "") //定长数组
        {
            string tarArrLen = tarPa.mArrayLen;
            int arrLenOut = 0;
            if (!int.TryParse(tarArrLen, out arrLenOut)) //不是数字
                tarArrLen = "(int)" + tarPa.mArrayLen;
            string strDel = "public string[] {0} = new string[{1}]; \t\t\t\t// {2}";
            outStrData.mOutDeclareString.Add(string.Format(strDel, tarPa.mName, tarArrLen, tarPa.mComment));
            //字符串 长度
            string tarStrLen = tarPa.mStringLen;
            int strLenOut = 0;
            if (!int.TryParse(tarStrLen, out strLenOut)) //不是数字
            {
                string strPack1 = "for(int i = 0; i < {0}.Length; ++i)";
                outStrData.mOutPackString.Add(string.Format(strPack1, tarPa.mName));
                string strPack2 = "\t{0}.WriteString({1}[i], (uint){2});";
                outStrData.mOutPackString.Add(string.Format(strPack2, bufferStr, tarPa.mName, tarPa.mStringLen));
                string strUnpack1 = "for(int i = 0; i < {0}.Length; ++i)";
                outStrData.mOutUnpackString.Add(string.Format(strUnpack1, tarPa.mName));
                string strUnpack2 = "\t{0}[i] = {1}.ReadString((int){2});";
                outStrData.mOutUnpackString.Add(string.Format(strUnpack2, tarPa.mName, bufferStr, tarPa.mStringLen));
            }
            else
            {
                string strPack1 = "for(int i = 0; i < {0}.Length; ++i)";
                outStrData.mOutPackString.Add(string.Format(strPack1, tarPa.mName));
                string strPack2 = "\t{0}.WriteString({1}[i] , {2});";
                outStrData.mOutPackString.Add(string.Format(strPack2, bufferStr, tarPa.mName, tarPa.mStringLen));
                string strUnpack1 = "for(int i = 0; i < {0}.Length; ++i)";
                outStrData.mOutUnpackString.Add(string.Format(strUnpack1, tarPa.mName));
                string strUnpack2 = "\t{0}[i] = {1}.ReadString({2});";
                outStrData.mOutUnpackString.Add(string.Format(strUnpack2, tarPa.mName, bufferStr, tarPa.mStringLen));
            }
        }
        else if (tarPa.mArrayDynLen != "") //变长数组
        {
            string strDel = "public string[] {0} = null; \t\t\t\t// {1}";
            outStrData.mOutDeclareString.Add(string.Format(strDel, tarPa.mName, tarPa.mComment));
            //字符串 长度
            string tarStrLen = tarPa.mStringLen;
            int strLenOut = 0;
            if (!int.TryParse(tarStrLen, out strLenOut)) //不是数字
            {
                string strPack1 = "for(int i = 0; i < {0}.Length; ++i)";
                outStrData.mOutPackString.Add(string.Format(strPack1, tarPa.mName));
                string strPack2 = "\t{0}.WriteString({1}[i], (uint){2});";
                outStrData.mOutPackString.Add(string.Format(strPack2, bufferStr, tarPa.mName, tarPa.mStringLen));
                string strUnpack1 = "{0} = new string[{1}];";
                outStrData.mOutUnpackString.Add(string.Format(strUnpack1, tarPa.mName, tarPa.mArrayDynLen));
                string strUnpack2 = "for(int i = 0; i < {0}.Length; ++i)";
                outStrData.mOutUnpackString.Add(string.Format(strUnpack2, tarPa.mName));
                string strUnpack3 = "\t{0}[i] = {1}.ReadString( (int){2} );";
                outStrData.mOutUnpackString.Add(string.Format(strUnpack3, tarPa.mName, bufferStr, tarPa.mStringLen));
            }
            else
            {
                string strPack1 = "for(int i = 0; i < {0}.Length; ++i)";
                outStrData.mOutPackString.Add(string.Format(strPack1, tarPa.mName));
                string strPack2 = "\t{0}.WriteString({1}[i] , {2});";
                outStrData.mOutPackString.Add(string.Format(strPack2, bufferStr, tarPa.mName, tarPa.mStringLen));
                string strUnpack1 = "{0} = new string[{1}];";
                outStrData.mOutUnpackString.Add(string.Format(strUnpack1, tarPa.mName, tarPa.mArrayDynLen));
                string strUnpack2 = "for(int i = 0; i < {0}.Length; ++i)";
                outStrData.mOutUnpackString.Add(string.Format(strUnpack2, tarPa.mName));
                string strUnpack3 = "\t{0}[i] = {1}.ReadString({2});";
                outStrData.mOutUnpackString.Add(string.Format(strUnpack3, tarPa.mName, bufferStr, tarPa.mStringLen));
            }
        }
        else
        {
            Debug.LogError("导出消息数组类型长度错误：" + mCurExportFileName);
        }
    }

    //解析基础类型 根据类型 数组 byte
    public static void CmdParamParserArrayByByte_CS(string bufferStr, ParamDataOutString outStrData, ParamData tarPa)
    {
        if (tarPa.mArrayLen != "") //定长数组
        {
            string tarStrLen = tarPa.mArrayLen;
            int strLenOut = 0;
            if (!int.TryParse(tarStrLen, out strLenOut)) //不是数字
                tarStrLen = "(int)" + tarPa.mArrayLen;
            string strDel = "public byte[] {0} = new byte[{1}]; \t\t\t\t// {2}";
            outStrData.mOutDeclareString.Add(string.Format(strDel, tarPa.mName, tarStrLen, tarPa.mComment));
            string strPack = "{0}.WriteBytes({1});";
            outStrData.mOutPackString.Add(string.Format(strPack, bufferStr, tarPa.mName));
            string strUnpack1 = "byte[] temp{0} = {1}.ReadBytes({2}.Length);";
            outStrData.mOutUnpackString.Add(string.Format(strUnpack1, tarPa.mName, bufferStr, tarPa.mName));
            string strUnpack2 = "Array.Copy(temp{0}, {1}, {2}.Length);";
            outStrData.mOutUnpackString.Add(string.Format(strUnpack2, tarPa.mName, tarPa.mName, tarPa.mName));
        }
        else if (tarPa.mArrayDynLen != "") //变长数组
        {
            string strDel = "public byte[] {0} = null; \t\t\t\t// {1}";
            outStrData.mOutDeclareString.Add(string.Format(strDel, tarPa.mName, tarPa.mComment));
            string strPack = "{0}.WriteBytes({1});";
            outStrData.mOutPackString.Add(string.Format(strPack, bufferStr, tarPa.mName));
            string strUnpack1 = "{0} = new byte[{1}];";
            outStrData.mOutUnpackString.Add(string.Format(strUnpack1, tarPa.mName, tarPa.mArrayDynLen));
            string strUnpack2 = "byte[] temp{0} = {1}.ReadBytes({2}.Length);";
            outStrData.mOutUnpackString.Add(string.Format(strUnpack2, tarPa.mName, bufferStr, tarPa.mName));
            string strUnpack3 = "Array.Copy(temp{0}, {1}, {2}.Length);";
            outStrData.mOutUnpackString.Add(string.Format(strUnpack3, tarPa.mName, tarPa.mName, tarPa.mName));
        }
        else
        {
            Debug.LogError("导出消息数组类型长度错误：" + mCurExportFileName);
        }
    }

    //解析基础类型 根据类型 数组 class
    public static void CmdParamParserArrayByClass_CS(string bufferStr, ParamDataOutString outStrData, ParamData tarPa)
    {
        if (tarPa.mArrayLen != "") //定长数组
        {
            string tarStrLen = tarPa.mArrayLen;
            int strLenOut = 0;
            if (!int.TryParse(tarStrLen, out strLenOut)) //不是数字
                tarStrLen = "(int)" + tarPa.mArrayLen;
            string strDel = "public {0}[] {1} = new {2}[{3}]; \t\t\t\t// {4}";
            outStrData.mOutDeclareString.Add(string.Format(strDel, tarPa.mArrayType, tarPa.mName, tarPa.mArrayType, tarStrLen, tarPa.mComment));
            string strPack1 = "for(int i = 0; i < {0}.Length; ++i)";
            outStrData.mOutPackString.Add(string.Format(strPack1, tarPa.mName));
            string strPack2 = "\t{0}[i].PackData({1});";
            outStrData.mOutPackString.Add(string.Format(strPack2, tarPa.mName, bufferStr));
            string strUnpack1 = "for(int i = 0; i < {0}.Length; ++i)";
            outStrData.mOutUnpackString.Add(string.Format(strUnpack1, tarPa.mName));
            string strUnpack2 = "\t{0}[i].UnPackData({1});";
            outStrData.mOutUnpackString.Add(string.Format(strUnpack2, tarPa.mName, bufferStr));
            string strCtor1 = "for (int i = 0; i < {0}.Length; ++i)";
            outStrData.mOutCtorString.Add(string.Format(strCtor1, tarPa.mName));
            string strCtor2 = "\t{0}[i] = new {1}();";
            outStrData.mOutCtorString.Add(string.Format(strCtor2, tarPa.mName, tarPa.mArrayType));
        }
        else if (tarPa.mArrayDynLen != "") //变长数组 
        {
            string strDel = "public {0}[] {1} = null; \t\t\t\t// {2}";
            outStrData.mOutDeclareString.Add(string.Format(strDel, tarPa.mArrayType, tarPa.mName, tarPa.mComment));
            string strPack1 = "for(int i = 0; i < {0}; ++i)";
            outStrData.mOutPackString.Add(string.Format(strPack1, tarPa.mArrayDynLen));
            string strPack2 = "\t{0}[i].PackData({1});";
            outStrData.mOutPackString.Add(string.Format(strPack2, tarPa.mName, bufferStr));
            string strUnpack1 = "{0} = new {1}[{2}];";
            outStrData.mOutUnpackString.Add(string.Format(strUnpack1, tarPa.mName, tarPa.mArrayType, tarPa.mArrayDynLen));
            string strUnpack2 = "for(int i = 0; i < {0}.Length; ++i)";
            outStrData.mOutUnpackString.Add(string.Format(strUnpack2, tarPa.mName));
            outStrData.mOutUnpackString.Add("{");
            string strUnpack4 = "\t{0}[i] = new {1}();";
            outStrData.mOutUnpackString.Add(string.Format(strUnpack4, tarPa.mName, tarPa.mArrayType));
            string strUnpack3 = "\t{0}[i].UnPackData({1});";
            outStrData.mOutUnpackString.Add(string.Format(strUnpack3, tarPa.mName, bufferStr));
            outStrData.mOutUnpackString.Add("}");
        }
        else
        {
            Debug.LogError("导出消息数组类型长度错误：" + mCurExportFileName);
        }
    }
    #endregion

    #region LUa
    public static ParamDataOutString CmdParamParserDeclare_Lua(ParamData tarPa, string fileName, bool isCmdType = false)
    {
        mCurExportFileName = fileName;
        ParamDataOutString outDataString = new ParamDataOutString();
        string packageParamName = "byteBuffer";
        switch (tarPa.mParamType)
        {
            case EParamType.eParamType_Byte:
                CmdParamParserByBaseType_Lua("Byte", packageParamName, outDataString, tarPa);
                break;
            case EParamType.eParamType_Short:
                CmdParamParserByBaseType_Lua("Short", packageParamName, outDataString, tarPa);
                break;
            case EParamType.eParamType_UShort:
                CmdParamParserByBaseType_Lua("UShort", packageParamName, outDataString, tarPa);
                break;
            case EParamType.eParamType_Int:
                CmdParamParserByBaseType_Lua("Int", packageParamName, outDataString, tarPa);
                break;
            case EParamType.eParamType_UInt:
                CmdParamParserByBaseType_Lua("UInt", packageParamName, outDataString, tarPa);
                break;
            case EParamType.eParamType_Long:
                CmdParamParserByBaseType_Lua("Long", packageParamName, outDataString, tarPa);
                break;
            case EParamType.eParamType_ULong:
                CmdParamParserByBaseType_Lua("ULong", packageParamName, outDataString, tarPa);
                break;
            case EParamType.eParamType_String:
                CmdParamParserByString_Lua(packageParamName, outDataString, tarPa);
                break;
            case EParamType.eParamType_OtherData:
                CmdParamParserByClass_Lua(packageParamName, outDataString, tarPa);
                break;
            case EParamType.eParamType_Byte_Array:
                CmdParamParserArrayByByte_Lua(packageParamName, outDataString, tarPa);
                break;
            case EParamType.eParamType_Short_Array:
                CmdParamParserArrayByBaseType_Lua("Short", packageParamName, outDataString, tarPa);
                break;
            case EParamType.eParamType_UShort_Array:
                CmdParamParserArrayByBaseType_Lua("UShort", packageParamName, outDataString, tarPa);
                break;
            case EParamType.eParamType_Int_Array:
                CmdParamParserArrayByBaseType_Lua("Int", packageParamName, outDataString, tarPa);
                break;
            case EParamType.eParamType_UInt_Array:
                CmdParamParserArrayByBaseType_Lua("UInt", packageParamName, outDataString, tarPa);
                break;
            case EParamType.eParamType_Long_Array:
                CmdParamParserArrayByBaseType_Lua("Long", packageParamName, outDataString, tarPa);
                break;
            case EParamType.eParamType_ULong_Array:
                CmdParamParserArrayByBaseType_Lua("ULong", packageParamName, outDataString, tarPa);
                break;
            case EParamType.eParamType_String_Array:
                CmdParamParserArrayByString_Lua(packageParamName, outDataString, tarPa);
                break;
            case EParamType.eParamType_OtherData_Array:
                CmdParamParserArrayByClass_Lua(packageParamName, outDataString, tarPa);
                break;
            default:
                break;
        }
        return outDataString;
    }
    //解析基础类型 根据类型
    public static void CmdParamParserByBaseType_Lua(string baseTypeStr, string bufferStr, ParamDataOutString outStrData, ParamData tarPa)
    {
        string strDel = "self.{0} = {1}; \t\t\t\t-- {2}";
        string value = tarPa.mValue;
        if (value.Contains(")"))
        {
            value = value.Substring(value.IndexOf(")") + 1);
        }
        outStrData.mOutDeclareString.Add(string.Format(strDel, tarPa.mName, value, tarPa.mComment));
        string strPack = "self.{0}:Write{1}( self.{2} );";
        outStrData.mOutPackString.Add(string.Format(strPack, bufferStr, baseTypeStr, tarPa.mName));
        string strUnpack = "self.{0} = self.{1}:Read{2}();";
        outStrData.mOutUnpackString.Add(string.Format(strUnpack, tarPa.mName, bufferStr, baseTypeStr));
    }
    //解析基础类型 根据类型  string
    public static void CmdParamParserByString_Lua(string bufferStr, ParamDataOutString outStrData, ParamData tarPa)
    {
        string value = string.IsNullOrEmpty(tarPa.mValue) ? "\"\"" : tarPa.mValue;//tarPa.mValue == "string.Empty" ||
        string strDel = "self.{0} = {1}; \t\t\t\t-- {2}";
        outStrData.mOutDeclareString.Add(string.Format(strDel, tarPa.mName, value, tarPa.mComment));
        string tarStrLen = tarPa.mStringLen;
        int strLenOut = 0;
        if (!int.TryParse(tarStrLen, out strLenOut)) //不是数字
        {
            string strPack = "self.{0}:WriteString( self.{1} ,{2});";
            outStrData.mOutPackString.Add(string.Format(strPack, bufferStr, tarPa.mName, tarPa.mStringLen));
            string strUnpack = "self.{0} = self.{1}:ReadString({2});";
            outStrData.mOutUnpackString.Add(string.Format(strUnpack, tarPa.mName, bufferStr, tarPa.mStringLen));
        }
        else
        {
            string strPack = "self.{0}:WriteString( self.{1} , {2});";
            outStrData.mOutPackString.Add(string.Format(strPack, bufferStr, tarPa.mName, tarPa.mStringLen));
            string strUnpack = "self.{0} = self.{1}:ReadString({2});";
            outStrData.mOutUnpackString.Add(string.Format(strUnpack, tarPa.mName, bufferStr, tarPa.mStringLen));
        }
    }
    public static void CmdParamParserArrayByString_Lua(string bufferStr, ParamDataOutString outStrData, ParamData tarPa)
    {
        string strDel = "self.{0} = nil; \t\t\t\t-- {1}";
        outStrData.mOutDeclareString.Add(string.Format(strDel, tarPa.mName, tarPa.mComment));
        string strCtor = "self.{} = {};";
        outStrData.mOutCtorString.Add(string.Format(strCtor, tarPa.mName));
        if (tarPa.mArrayLen != "") //定长数组
        {
            string tarArrLen = tarPa.mArrayLen;
            int arrLenOut = 0;
            if (!int.TryParse(tarArrLen, out arrLenOut)) //不是数字
                tarArrLen = tarPa.mArrayLen;
            //字符串 长度
            string tarStrLen = tarPa.mStringLen;
            int strLenOut = 0;
            if (!int.TryParse(tarStrLen, out strLenOut)) //不是数字
            {
                string strPack1 = "for i = 1, {0} do";
                outStrData.mOutPackString.Add(string.Format(strPack1, tarArrLen));
                string strPack2 = "\tself.{0}:WriteString( {1}[i] ,{2}); \n end";
                outStrData.mOutPackString.Add(string.Format(strPack2, bufferStr, tarPa.mName, tarPa.mStringLen));
                string strUnpack1 = "for i = 1, {0} do";
                outStrData.mOutUnpackString.Add(string.Format(strUnpack1, tarPa.mName));
                string strUnpack2 = "\tself.{0}[i] = self.{1}.ReadString({2} );\n end";
                outStrData.mOutUnpackString.Add(string.Format(strUnpack2, tarPa.mName, bufferStr, tarPa.mStringLen));
            }
            else
            {
                string strPack1 = "for i = 1, {0} do";
                outStrData.mOutPackString.Add(string.Format(strPack1, tarArrLen));
                string strPack2 = "\tself.{0}:WriteString( {1}[i] , {2}); \n end";
                outStrData.mOutPackString.Add(string.Format(strPack2, bufferStr, tarPa.mName, tarPa.mStringLen));
                string strUnpack1 = "for i = 1, {0} do";
                outStrData.mOutUnpackString.Add(string.Format(strUnpack1, tarPa.mName));
                string strUnpack2 = "\tself.{0}[i] = self.{1}:ReadString({2});\n end";
                outStrData.mOutUnpackString.Add(string.Format(strUnpack2, tarPa.mName, bufferStr, tarPa.mStringLen));
            }
        }
        else if (tarPa.mArrayDynLen != "") //变长数组
        {
            //字符串 长度
            string tarStrLen = tarPa.mStringLen;
            int strLenOut = 0;
            if (!int.TryParse(tarStrLen, out strLenOut)) //不是数字
            {
                string strPack1 = "for i = 1, {0} do";
                outStrData.mOutPackString.Add(string.Format(strPack1, tarPa.mArrayDynLen));
                string strPack2 = "\tself.{0}:WriteString( {1}[i] ,{2}); \n end";
                outStrData.mOutPackString.Add(string.Format(strPack2, bufferStr, tarPa.mName, tarPa.mStringLen));
                string strUnpack2 = "for i = 1, {0} do";
                outStrData.mOutUnpackString.Add(string.Format(strUnpack2, tarPa.mName));
                string strUnpack3 = "\tself.{0}[i] = self.{1}:ReadString({2});\n end";
                outStrData.mOutUnpackString.Add(string.Format(strUnpack3, tarPa.mName, bufferStr, tarPa.mStringLen));
            }
            else
            {
                string strPack1 = "for i = 1, {0} do";
                outStrData.mOutPackString.Add(string.Format(strPack1, tarPa.mArrayDynLen));
                string strPack2 = "\tself.{0}:WriteString( {1}[i] , {2}); \nend";
                outStrData.mOutPackString.Add(string.Format(strPack2, bufferStr, tarPa.mName, tarPa.mStringLen));
                string strUnpack2 = "for i = 1, {0} do";
                outStrData.mOutUnpackString.Add(string.Format(strUnpack2, tarPa.mName));
                string strUnpack3 = "\tself.{0}[i] = self.{1}:ReadString({2});\n end";
                outStrData.mOutUnpackString.Add(string.Format(strUnpack3, tarPa.mName, bufferStr, tarPa.mStringLen));
            }
        }
        else
        {
            Debug.LogError("导出消息数组类型长度错误：" + mCurExportFileName);
        }
    }
    //解析基础类型 其他数据结构 class
    public static void CmdParamParserByClass_Lua(string bufferStr, ParamDataOutString outStrData, ParamData tarPa)
    {
        string strDel = "self.{0} = {1}; \t\t\t\t-- {2}";
        outStrData.mOutDeclareString.Add(string.Format(strDel, tarPa.mName, "nil", tarPa.mComment));
        string strCtor = "self.{0} = {1}.new('{1}');";
        outStrData.mOutCtorString.Add(string.Format(strCtor, tarPa.mName, tarPa.mType));
        string strPack = "self.{0}:PackData( self.{1} );";
        outStrData.mOutPackString.Add(string.Format(strPack, tarPa.mName, bufferStr));
        if (string.IsNullOrEmpty(tarPa.mArrayDynLen))
        {
            string strUnpack = "self.{0}:UnPackData( self.{1} );";
            outStrData.mOutUnpackString.Add(string.Format(strUnpack, tarPa.mName, bufferStr));
        }
        else
        {
            string strUnpack = "";
            string format1 = "if self.{0} > 0 then ";
            strUnpack += string.Format(format1, tarPa.mArrayDynLen);
            string format2 = "self.{0}.UnPackData( self.{1} )";
            strUnpack += string.Format(format2, tarPa.mName, bufferStr);
            strUnpack += " end ";
            outStrData.mOutUnpackString.Add(strUnpack);
        }
    }
    public static void CmdParamParserArrayByByte_Lua(string bufferStr, ParamDataOutString outStrData, ParamData tarPa)
    {
        string strDel = "self.{0} = nil; \t\t\t\t-- {1}";
        outStrData.mOutDeclareString.Add(string.Format(strDel, tarPa.mName, tarPa.mComment));
        string strCtor = "self.{0} = {{}};";
        outStrData.mOutCtorString.Add(string.Format(strCtor, tarPa.mName));
        if (tarPa.mArrayLen != "") //定长数组
        {
            string tarStrLen = tarPa.mArrayLen;
            string strPack = "self.{0}:WriteBytes(self.{1});";
            outStrData.mOutPackString.Add(string.Format(strPack, bufferStr, tarPa.mName));
            string strUnpack1 = "for i = 1, {0} do\n\ttable.insert(self.{1}, self.byteBuffer:ReadByte()); \n\tend";
            int strLenOut = 0;
            if (!int.TryParse(tarStrLen, out strLenOut)) //不是数字
            {
                outStrData.mOutUnpackString.Add(string.Format(strUnpack1, tarPa.mArrayLen, tarPa.mName));
            }
            else
            {
                outStrData.mOutUnpackString.Add(string.Format(strUnpack1, strLenOut, tarPa.mName));
            }
        }
        else if (tarPa.mArrayDynLen != "") //变长数组
        {
            string strPack = "self.{0}:WriteBytes(self.{1});";
            outStrData.mOutPackString.Add(string.Format(strPack, bufferStr, tarPa.mName));
            string strUnpack1 = "for i = 1, self.{0} do\n\ttable.insert(self.{1}, self.byteBuffer:ReadByte()); \n\tend";
            outStrData.mOutUnpackString.Add(string.Format(strUnpack1, tarPa.mArrayDynLen, tarPa.mName));
        }
        else
        {
            Debug.LogError("导出消息数组类型长度错误：" + mCurExportFileName);
        }
    }
    //解析基础类型 根据类型 数组 class
    public static void CmdParamParserArrayByClass_Lua(string bufferStr, ParamDataOutString outStrData, ParamData tarPa)
    {
        string strDel = "self.{0} = {1}; \t\t\t\t-- {2}";
        outStrData.mOutDeclareString.Add(string.Format(strDel, tarPa.mName, "{}", tarPa.mComment));
        if (tarPa.mArrayLen != "") //定长数组
        {
            string tarStrLen = tarPa.mArrayLen;
            int strLenOut = 0;
            bool isNum = int.TryParse(tarStrLen, out strLenOut);
            if (!int.TryParse(tarStrLen, out strLenOut)) //不是数字
                tarStrLen = tarPa.mArrayLen;
            string strPack1 = "for  i = 1, {0} do";
            outStrData.mOutPackString.Add(string.Format(strPack1, isNum ? strLenOut.ToString() : tarPa.mArrayLen));
            string strPack2 = "\tself.{0}:PackData( self.{1} ); \n\tend";
            outStrData.mOutPackString.Add(string.Format(strPack2, tarPa.mName, bufferStr));
            string strUnpackFormat = "for  i = 1, {0} do \r\n\t local tempData = {1}.new('{1}');\r\n\t tempData:UnPackData(self.{2});\r\n\t table.insert(self.{3}, tempData)\r\n\t end ";
            outStrData.mOutUnpackString.Add(string.Format(strUnpackFormat, isNum ? strLenOut.ToString() : tarPa.mArrayLen,
                                                          tarPa.mArrayType, bufferStr, tarPa.mName));
        }
        else if (tarPa.mArrayDynLen != "") //变长数组 
        {
            string strPack1 = "for  i = 1, self.{0} do";
            outStrData.mOutPackString.Add(string.Format(strPack1, tarPa.mArrayDynLen));
            string strPack2 = "\tself.{0}:PackData( self.{1} ); \n\tend";
            outStrData.mOutPackString.Add(string.Format(strPack2, tarPa.mName, bufferStr));
            string strUnpackFormat = "for  i = 1, self.{0} do \r\n\t local tempData = {1}.new('{1}');\r\n\t tempData:UnPackData(self.{2});\r\n\t table.insert(self.{3}, tempData)\r\n\t end";
            outStrData.mOutUnpackString.Add(string.Format(strUnpackFormat, tarPa.mArrayDynLen,
                                                          tarPa.mArrayType, bufferStr, tarPa.mName));
        }
        else
        {
            Debug.LogError("导出消息数组类型长度错误：" + mCurExportFileName);
        }
    }
    //解析基础类型 根据类型 数组
    public static void CmdParamParserArrayByBaseType_Lua(string baseTypeStr, string bufferStr, ParamDataOutString outStrData, ParamData tarPa)
    {
        string strDel = "self.{0} = nil; \t\t\t\t-- {1}";
        outStrData.mOutDeclareString.Add(string.Format(strDel, tarPa.mName, tarPa.mComment));
        string strCtor = "self.{0} = {{}};";
        outStrData.mOutCtorString.Add(string.Format(strCtor, tarPa.mName));
        if (tarPa.mArrayLen != "") //定长数组
        {
            string tarStrLen = tarPa.mArrayLen;
            int strLenOut = 0;
            bool isNum = int.TryParse(tarStrLen, out strLenOut);
            if (!int.TryParse(tarStrLen, out strLenOut)) //不是数字
                tarStrLen = tarPa.mArrayLen;
            string strPack1 = "for  i = 1, {0} do";
            outStrData.mOutPackString.Add(string.Format(strPack1, isNum ? strLenOut.ToString() : tarPa.mArrayLen));
            string strPack2 = "\tself.{0}:Write{1}( {2}[i] );\n\tend";
            outStrData.mOutPackString.Add(string.Format(strPack2, bufferStr, baseTypeStr, tarPa.mName));
            string strUnpack1 = "for  i = 1, {0} do";
            outStrData.mOutUnpackString.Add(string.Format(strUnpack1, isNum ? strLenOut.ToString() : tarPa.mArrayLen));
            string strUnpack2 = "\t table.insert(self.{0}[i], self.{1}:Read{2}()); \n\tend";
            outStrData.mOutUnpackString.Add(string.Format(strUnpack2, tarPa.mName, bufferStr, baseTypeStr));
        }
        else if (tarPa.mArrayDynLen != "") //变长数组
        {
            string strPack1 = "for  i = 1, self.{0} do";
            outStrData.mOutPackString.Add(string.Format(strPack1, tarPa.mArrayDynLen));
            string strPack2 = "\tself.{0}:Write{1}( {2}[i] );\n\tend";
            outStrData.mOutPackString.Add(string.Format(strPack2, bufferStr, baseTypeStr, tarPa.mName));
            string strUnpack2 = "for  i = 1, self.{0} do";
            outStrData.mOutUnpackString.Add(string.Format(strUnpack2, tarPa.mArrayDynLen));
            string strUnpack3 = "\t table.insert(self.{0}[i], self.{1}:Read{2}()); \n\tend";
            outStrData.mOutUnpackString.Add(string.Format(strUnpack3, tarPa.mName, bufferStr, baseTypeStr));
        }
        else
        {
            Debug.LogError("导出消息数组类型长度错误：" + mCurExportFileName);
        }
    }
    #endregion
}
