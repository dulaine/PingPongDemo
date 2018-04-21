using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//表格数据load接口
public interface ITableLoad
{
    uint Load(ByteBuffer byteBuffer);
}

/// <summary>
/// 数据表格的基类
/// </summary>
public class ConfigTableBase<T> where T : ITableLoad, new()
{
    public ConfigTableBase() { }
    private Hashtable _mItemMap = new Hashtable();
    private uint _mMaxID = 0;
    private List<T> tableList;
    //表格数量
    public int RecordCount
    {
        get
        {
            return _mItemMap.Count;
        }
    }
    /// <summary>
    /// 加载表格数据
    /// </summary>
    public void Load(byte[] bytes)
    {
        _mItemMap.Clear();
        ByteBuffer byteBuffer = new ByteBuffer(bytes);
        uint count = byteBuffer.ReadUInt();
        for (uint i = 0; i < count; i++)
        {
            T record = new T();
            uint id = record.Load(byteBuffer);
            if (_mItemMap.ContainsKey(id))
            {
                Debug.LogError("表格有重复ID，表格： " + typeof(T) + "ID: " + id);
                continue;
            }
            _mItemMap.Add(id, record);
            if (_mMaxID < id)
                _mMaxID = id;
        }
        byteBuffer.Close();
    }
    public Hashtable GetAllData()
    {
        return _mItemMap;
    }
    public uint GetMaxID()
    {
        return _mMaxID;
    }
    public T GetData(int id)
    {
        return GetData((uint)id);
    }
    public T GetData(uint id)
    {
        if (_mItemMap.ContainsKey(id))
        {
            return (T)_mItemMap[id];
        }
        else
        {
            return default(T);
        }
    }
    public List<T> ToList()
    {
        if (tableList == null)
        {
            tableList = new List<T>();
            foreach (var item in _mItemMap.Keys)
            {
                tableList.Add(GetData((uint)item));
            }
        }
        else if (tableList.Count != _mItemMap.Count)
        {
            tableList.Clear();
            foreach (var item in _mItemMap.Keys)
            {
                tableList.Add(GetData((uint)item));
            }
        }
        return tableList;
    }
}
