using UnityEngine;
using System.Collections;

/// <summary>
/// Lua stNullClientCmd 生成
/// </summary>
public class LuaScriptParserTool
{
    private static string LuaRequireString;
    public const string CmdBaseDirectory = "NetBase";

    #region 记录Require
    /// <summary>
    /// 清空Lua handler的引用
    /// </summary>
    public static void BeginLuaParser()
    {
        LuaRequireString = string.Empty;
    }
    /// <summary>
    /// 将xml创建出来的lua net脚本，放入NetCmdLuaHandler中进行引用，以便加载
    /// </summary>
    public static void AddLuaRequireString(string str)
    {
        LuaRequireString += "require  " + "\"NetCmd." + str + "\" \r\n";
    }
    /// <summary>
    /// 获取lua handler所需的所有引用
    /// </summary>
    public static string GetLuaRequireString()
    {
        return LuaRequireString;
    }

    #endregion

    public static string CreateNullClientCmd()
    {
        return "stNullClientCmd = class(\"stNullClientCmd\");\n" +
@"
-- 成员
stNullClientCmd.gsCmd = 0;
stNullClientCmd.gsKey = 0;
stNullClientCmd.gsParam = 0;
stNullClientCmd.dwTimestamp = 0;
stNullClientCmd.byteBuffer = nil; -- 队列

-- 构造
function stNullClientCmd:ctor()
    self.gsCmd = 0;
    self.gsKey = 0;
    self.gsParam = 0;
    self.dwTimestamp = 0;
    self.byteBuffer = nil;
end

-- 打包
function stNullClientCmd:Pack()
    self.byteBuffer = ByteBuffer.New();
    self.byteBuffer:WriteByte(self.gsCmd);
    self.byteBuffer:WriteUInt(self.gsKey);
    self.byteBuffer:WriteByte(self.gsParam);
    self.dwTimestamp = Time.time * 1000;
    self.byteBuffer:WriteUInt(self.dwTimestamp);
end

-- 解包
function stNullClientCmd:UnPack(byteArray)
    self.byteBuffer = ByteBuffer.New(byteArray);
    self.gsCmd = self.byteBuffer:ReadByte();
    self.gsKey = self.byteBuffer:ReadUInt();
    self.gsParam = self.byteBuffer:ReadByte();
    self.dwTimestamp = self.byteBuffer:ReadUInt();
end

-- 释放bytebuffer
function stNullClientCmd:ReleaseByteBuffer()
    self.byteBuffer:Close();
    self.byteBuffer = nil;
end

-- 获取字节流
function stNullClientCmd:GetDataBytes()
    if self.byteBuffer ~= nil then
        return self.byteBuffer:ToBytes();
    else
        return nil;
    end
end

-- 复制数据
function stNullClientCmd:CopyNullData(data)
    self.gsCmd = data.gsCmd;
    self.gsKey = data.gsKey;
    self.gsParam = data.gsParam;
    self.dwTimestamp = data.dwTimestamp;
end

return stNullClientCmd";
    }
}
