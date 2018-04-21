using UnityEngine;
using System.Collections;
using System.IO;
using System.Text;
using System;
using System.Collections.Generic;

public class ByteBuffer
{
    private byte[] mTarUnpackArray = null;
    private int mCurUnpackPos = 0;
    private List<byte> mTarPackList = null;

    /// <summary>
    /// 构造，用于创建二进制流
    /// </summary>
    public ByteBuffer()
    {
        mTarPackList = new List<byte>();
    }

    /// <summary>
    /// 构造，用于读取二进制流
    /// </summary>
    /// <param name="data">二进制流</param>
    public ByteBuffer(byte[] data)
    {
        if (data != null)
        {
            mTarUnpackArray = data;
        }
        else
        {
            mTarPackList = new List<byte>();
        }
    }

    /// <summary>
    /// 释放资源
    /// </summary>
    public void Close()
    {
        if (mTarPackList != null)
        {
            mTarPackList.Clear();
            mTarPackList = null;
        }
        mTarUnpackArray = null;
    }

    /// <summary>
    /// 转换成byte数组
    /// </summary>
    public byte[] ToBytes()
    {
        if (mTarPackList == null)
            return null;
        return mTarPackList.ToArray();
    }
    public byte ReadByte()
    {
        byte tempRes = mTarUnpackArray[mCurUnpackPos];
        mCurUnpackPos += 1;
        return tempRes;
    }

    public void WriteByte(byte v)
    {
        mTarPackList.Add(v);
    }

    public short ReadShort()
    {
        short tempRes = BitConverter.ToInt16(mTarUnpackArray, mCurUnpackPos);
        mCurUnpackPos += 2;
        return tempRes;
    }

    public void WriteShort(short v)
    {
        mTarPackList.Add((byte)(v & 0xFF));
        mTarPackList.Add((byte)((v >> 8) & 0xFF));
    }

    public ushort ReadUShort()
    {
        ushort temp = 0;
        temp += (ushort)(mTarUnpackArray[mCurUnpackPos]);
        temp += (ushort)((mTarUnpackArray[mCurUnpackPos + 1]) << 8);
        mCurUnpackPos += 2;
        return temp;
    }

    public void WriteUShort(ushort v)
    {
        mTarPackList.Add((byte)(v & 0xFF));
        mTarPackList.Add((byte)((v >> 8) & 0xFF));
    }

    public int ReadInt()
    {
        int tempRes = BitConverter.ToInt32(mTarUnpackArray, mCurUnpackPos);
        mCurUnpackPos += 4;
        return tempRes;
    }

    public void WriteInt(int v)
    {
        mTarPackList.Add((byte)(v & 0xFF));
        mTarPackList.Add((byte)((v >> 8) & 0xFF));
        mTarPackList.Add((byte)((v >> 16) & 0xFF));
        mTarPackList.Add((byte)((v >> 24) & 0xFF));
    }

    public uint ReadUInt()
    {
        uint temp = 0;
        temp += (uint)(mTarUnpackArray[mCurUnpackPos]);
        temp += (uint)(mTarUnpackArray[mCurUnpackPos + 1]) << 8;
        temp += (uint)(mTarUnpackArray[mCurUnpackPos + 2]) << 16;
        temp += (uint)(mTarUnpackArray[mCurUnpackPos + 3]) << 24;
        mCurUnpackPos += 4;
        return temp;
    }

    public void WriteUInt(uint v)
    {
        mTarPackList.Add((byte)(v & 0xFF));
        mTarPackList.Add((byte)((v >> 8) & 0xFF));
        mTarPackList.Add((byte)((v >> 16) & 0xFF));
        mTarPackList.Add((byte)((v >> 24) & 0xFF));
    }

    public long ReadLong()
    {
        long tempRes = BitConverter.ToInt64(mTarUnpackArray, mCurUnpackPos);
        mCurUnpackPos += 8;
        return tempRes;
    }

    public void WriteLong(long v)
    {
        mTarPackList.Add((byte)(v & 0xFF));
        mTarPackList.Add((byte)((v >> 8) & 0xFF));
        mTarPackList.Add((byte)((v >> 16) & 0xFF));
        mTarPackList.Add((byte)((v >> 24) & 0xFF));
        mTarPackList.Add((byte)((v >> 32) & 0xFF));
        mTarPackList.Add((byte)((v >> 40) & 0xFF));
        mTarPackList.Add((byte)((v >> 48) & 0xFF));
        mTarPackList.Add((byte)((v >> 56) & 0xFF));
    }

    public ulong ReadULong()
    {
        ulong temp = 0;
        temp += (ulong)(mTarUnpackArray[mCurUnpackPos]);
        temp += (ulong)(mTarUnpackArray[mCurUnpackPos + 1]) << 8;
        temp += (ulong)(mTarUnpackArray[mCurUnpackPos + 2]) << 16;
        temp += (ulong)(mTarUnpackArray[mCurUnpackPos + 3]) << 24;
        temp += (ulong)(mTarUnpackArray[mCurUnpackPos + 4]) << 32;
        temp += (ulong)(mTarUnpackArray[mCurUnpackPos + 5]) << 40;
        temp += (ulong)(mTarUnpackArray[mCurUnpackPos + 6]) << 48;
        temp += (ulong)(mTarUnpackArray[mCurUnpackPos + 7]) << 56;
        mCurUnpackPos += 8;
        return temp;
    }

    public void WriteULong(ulong v)
    {
        mTarPackList.Add((byte)(v & 0xFF));
        mTarPackList.Add((byte)((v >> 8) & 0xFF));
        mTarPackList.Add((byte)((v >> 16) & 0xFF));
        mTarPackList.Add((byte)((v >> 24) & 0xFF));
        mTarPackList.Add((byte)((v >> 32) & 0xFF));
        mTarPackList.Add((byte)((v >> 40) & 0xFF));
        mTarPackList.Add((byte)((v >> 48) & 0xFF));
        mTarPackList.Add((byte)((v >> 56) & 0xFF));
    }

    public float ReadFloat()
    {
        float tempRes = BitConverter.ToSingle(mTarUnpackArray, mCurUnpackPos);
        mCurUnpackPos += 4;
        return tempRes;
    }

    public void WriteFloat(float v)
    {
        byte[] temp = BitConverter.GetBytes(v);
        WriteBytes(temp);
    }

    public double ReadDouble()
    {
        double tempRes = BitConverter.ToDouble(mTarUnpackArray, mCurUnpackPos);
        mCurUnpackPos += 8;
        return tempRes;
    }

    public void WriteDouble(double v)
    {
        byte[] temp = BitConverter.GetBytes(v);
        WriteBytes(temp);
    }

    public byte[] ReadBytes(int len)
    {
        byte[] temp = new byte[len];
        Array.Copy(mTarUnpackArray, mCurUnpackPos, temp, 0, len);
        mCurUnpackPos += len;
        return temp;
    }

    public void WriteBytes(byte[] v)
    {
        for (int i = 0; i < v.Length; i++)
        {
            mTarPackList.Add(v[i]);
        }
    }

    //读取固定长度字符串
    public string ReadString(int len)
    {
        byte[] buffer = ReadBytes(len);
        char[] array = Encoding.UTF8.GetChars(buffer);
        int arrlen = 0;
        for (; arrlen < array.Length; ++arrlen)
            if (array[arrlen] == 0) break;
        return new string(array, 0, arrlen);
    }

    //写入固定长度字符串
    public void WriteString(string v, uint len)
    {
        byte[] temp = new byte[len];
        if (v == null)
            return;
        Encoding.UTF8.GetBytes(v, 0, v.Length, temp, 0);
        WriteBytes(temp);
    }

    ////////////////////////////////////静态 辅助类函数////////////////////////////////////

    //在byte数组中 解析uint
    public static uint BytesToUint(byte[] src, int startidx = 0)
    {
        uint temp = 0;
        temp = (uint)(src[startidx + 3]) << 24;
        temp += (uint)(src[startidx + 2]) << 16;
        temp += (uint)(src[startidx + 1]) << 8;
        temp += (uint)(src[startidx + 0]);
        return temp;
    }

    //uint 转byte数组
    public static byte[] UintToBytes(uint value, byte[] src, int startidx = 0)
    {
        src[startidx + 3] = (byte)((value >> 24) & 0xFF);
        src[startidx + 2] = (byte)((value >> 16) & 0xFF);
        src[startidx + 1] = (byte)((value >> 8) & 0xFF);
        src[startidx] = (byte)(value & 0xFF);
        return src;
    }

    //bytes 转 string
    public static string BytesToString(byte[] bytes)
    {
        char[] array = Encoding.UTF8.GetChars(bytes);
        int arrlen = 0;
        for (; arrlen < array.Length; ++arrlen)
            if (array[arrlen] == 0) break;
        return new string(array, 0, arrlen);
    }

    public static string BytesToString(byte[] buff, int start, int length)
    {
        byte[] temp = new byte[length];
        Array.Copy(buff, start, temp, 0, length);
        return BytesToString(temp);
    }

    //创建byte数组 传出给lua
    public static byte[] CreateByteArray(int arrLength)
    {
        if (arrLength <= 0)
            return null;
        return new byte[arrLength];
    }

    //拷贝数据
    public static void ArrayCopy(byte[] srcArr, byte[] tarArr, int arrLen)
    {
        Array.Copy(srcArr, tarArr, arrLen);
    }
}
