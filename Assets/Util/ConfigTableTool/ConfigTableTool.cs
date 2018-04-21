using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;


/// <summary>
/// 配置表工具
/// </summary>
public class ConfigTableTool
{
    static ConfigTableTool _Instance;
    public static ConfigTableTool Instance
    {
        get
        {
            if (_Instance == null)
            {
                _Instance = new ConfigTableTool();
                _Instance.Init();
            }
            return _Instance;
        }
    }
    Hashtable _Tables = new Hashtable();
    public string TablePath;    //表格数据所在路径

    public void Init()
    {
        _Tables.Clear();
        TablePath = string.Concat(IOPath.Instance.NowWWWPath, "Table/");
        //TablePath = string.Concat(IOPath.Instance.NowPlatformPath, "Table/");
    }
    /// <summary>
    /// 获取整个表格
    /// </summary>
    public ConfigTableBase<T> GetTableData<T>() where T : ITableLoad, new()
    {
        Type tableType = typeof(T);
        if (_Tables.Contains(tableType))
            return _Tables[tableType] as ConfigTableBase<T>;
        else
            Debug.LogError("查找表数据，未找到表： " + typeof(T));

        return null;
    }
    /// <summary>
    /// 获取表格中数据条数
    /// </summary>
    public int GetTableCount<T>() where T : ITableLoad, new()
    {
        int res = 0;

        Type tableType = typeof(T);
        if (_Tables.Contains(tableType))
        {
            ConfigTableBase<T> table = _Tables[tableType] as ConfigTableBase<T>;
            return table.RecordCount;
        }
        else
            Debug.LogError("查找表数据，未找到表： " + typeof(T));

        return res;
    }
    public T GetRecord<T>(ushort id) where T : ITableLoad, new()
    {
        return GetRecord<T>((uint)id);
    }

    public T GetRecord<T>(short id) where T : ITableLoad, new()
    {
        return GetRecord<T>((uint)id);
    }

    public T GetRecord<T>(int id) where T : ITableLoad, new()
    {
        return GetRecord<T>((uint)id);
    }
    /// <summary>
    /// 获取此表中某条数据
    /// </summary>
    public T GetRecord<T>(uint id) where T : ITableLoad, new()
    {
        ConfigTableBase<T> table = GetTableData<T>();
        if (table == null)
            return default(T);
        return table.GetData(id);
    }

    //---------------Lua使用-----------------
    #region Lua使用
    public byte[] GetTableData(string name)
    {
        return IOTool.LoadFileBytes(string.Concat(TablePath, name));
    }
    #endregion
}
