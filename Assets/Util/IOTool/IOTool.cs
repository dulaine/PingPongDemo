using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System;

/// <summary>
/// 文件IO
/// </summary>
public class IOTool
{
    /// <summary>
    /// 检测是否包含文件
    /// </summary>
    public static bool CheckHaveFile(string _path)
    {
        return File.Exists(_path);
    }
    /// <summary>
    /// 创建byte[]文件
    /// </summary>
    public static void CreateFileBytes(string path, byte[] data)
    {
        string _dir = Path.GetDirectoryName(path);
        if (!Directory.Exists(_dir))
        {
            Directory.CreateDirectory(_dir);
        }
        FileStream fs = new FileStream(path, FileMode.Create);
        BinaryWriter bw = new BinaryWriter(fs);
        bw.Write(data);
        bw.Flush();
        fs.Flush();
        fs.Close();
    }
    /// <summary>
    /// 读取byte[]数据
    /// </summary>
    public static byte[] LoadFileBytes(string path)
    {
        FileStream fs = new FileStream(path, FileMode.Open);
        //获取文件大小
        long size = fs.Length;
        byte[] array = new byte[size];
        //将文件读到byte数组中
        fs.Read(array, 0, array.Length);
        fs.Close();
        return array;
    }
    /// <summary>
    /// 创建string文件
    /// </summary>
    public static void CreateFileString(string path, string coutent)
    {
        string _dir = Path.GetDirectoryName(path);
        if (!Directory.Exists(_dir))
        {
            Directory.CreateDirectory(_dir);
        }
        FileStream fs = new FileStream(path, FileMode.Create);
        StreamWriter sw = new StreamWriter(fs);
        sw.Write(coutent);
        sw.Flush();
        sw.Close();
        fs.Close();
    }
    /// <summary>
    /// 读取string数据
    /// </summary>
    public static string LoadFileString(string path)
    {
        return File.ReadAllText(path);
    }
    /// <summary>
    /// 移动文件
    /// </summary>
    public static void MoveFile(string sourcePath, string targetPath)
    {
        File.Move(sourcePath, targetPath);
    }
    /// <summary>
    /// 删除文件
    /// </summary>
    public static void DeleteFile(string path)
    {
        if (File.Exists(path))
        {
            File.Delete(path);
        }
    }
    /// <summary>
    /// int转字节 （小端）
    /// </summary>
    public static byte[] IntToBytes(int key)
    {
        byte[] tempbytes = new byte[4];
        tempbytes[0] = (byte)(0xff & key);
        tempbytes[1] = (byte)((0xff00 & key) >> 8);
        tempbytes[2] = (byte)((0xff0000 & key) >> 16);
        tempbytes[3] = (byte)((0xff000000 & key) >> 24);
        return tempbytes;
    }

    /// <summary>
    /// 字节 转 int（小端）
    /// </summary>
    public static int BytesToInt(byte[] key)
    {
        int temp = 0;
        temp = (int)key[0];
        temp += (int)(key[1] << 8);
        temp += (int)(key[2] << 16);
        temp += (int)(key[3] << 24);
        return temp;
    }

    /// <summary>
    /// ushort 转 字节（小端）
    /// </summary>
    public static byte[] UshortToBytes(ushort key)
    {
        byte[] tempbytes = new byte[2];
        tempbytes[0] = (byte)(0xff & key);
        tempbytes[1] = (byte)((0xff00 & key) >> 8);
        return tempbytes;
    }

    /// <summary>
    /// 字节 转 ushort（小端）
    /// </summary>
    public static ushort BytesToUshort(byte[] key)
    {
        ushort temp = 0;
        temp += (ushort)key[0];
        temp += (ushort)(key[1] << 8);
        return temp;
    }

    //----------------------文件夹操作-------------------------
    #region 文件夹操作
    /// <summary>
    /// 复制文件夹
    /// </summary>
    public static void CopyDir(string fromDir, string toDir)
    {
        if (!Directory.Exists(fromDir))
            return;
        if (Directory.Exists(toDir))
        {
            Directory.Delete(toDir, true);
        }
        Directory.CreateDirectory(toDir);
        //复制文件
        string[] files = Directory.GetFiles(fromDir);
        for (int i = 0; i < files.Length; i++)
        {
            string fileName = Path.GetFileName(files[i]);
            string toFileName = Path.Combine(toDir, fileName);
            File.Copy(files[i], toFileName);
        }
        //复制文件夹
        string[] fromDirs = Directory.GetDirectories(fromDir);
        for (int i = 0; i < fromDirs.Length; i++)
        {
            string dirName = Path.GetFileName(fromDirs[i]);
            string toDirName = Path.Combine(toDir, dirName);
            CopyDir(fromDirs[i], toDirName);
        }
    }

    /// <summary>
    /// 重命名后缀
    /// </summary>
    public static void ChangeExtension(string _dir, string _from, string _to)
    {
        if (!Directory.Exists(_dir))
            return;
        //重命名文件
        string[] files = Directory.GetFiles(_dir);
        for (int i = 0; i < files.Length; i++)
        {
            string extension = Path.GetExtension(files[i]);
            if (extension == _from)
            {
                string dfileName = Path.ChangeExtension(files[i], _to);
                File.Move(files[i], dfileName);
            }
        }
        //循环遍历
        string[] fromDirs = Directory.GetDirectories(_dir);
        for (int i = 0; i < fromDirs.Length; i++)
        {
            ChangeExtension(fromDirs[i], _from, _to);
        }
    }
    #endregion
}
