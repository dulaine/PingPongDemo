using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

/// <summary>
/// 自写简易池管理组件，基于mono
/// </summary>
public class EasyPool : MonoBehaviour
{
    class EasyPoolData
    {
        public List<Transform> shows = new List<Transform>();
        public List<Transform> pools = new List<Transform>();
    }

    public List<Transform> items = new List<Transform>();   //要走池的物体

    Dictionary<int, EasyPoolData> mDatas = new Dictionary<int, EasyPoolData>();
    Transform mTrans;

    void Awake()
    {
        mTrans = transform;
    }

    /// <summary>
    /// 获取一个物体
    /// </summary>
    public Transform Get(int _id)
    {
        EasyPoolData data = null;
        mDatas.TryGetValue(_id, out data);
        if (data == null)
        {
            data = new EasyPoolData();
            mDatas[_id] = data;
        }
        Transform obj;
        if (data.pools.Count == 0)
        {
            obj = Instantiate(items[_id].gameObject).transform;
        }
        else
        {
            obj = data.pools[0];
        }
        data.pools.Remove(obj);
        data.shows.Add(obj);
        return obj;
    }

    /// <summary>
    /// 将一个物体放入池
    /// </summary>
    public void Push(int _id, Transform _obj)
    {
        var data = mDatas[_id];
        data.shows.Remove(_obj);
        data.pools.Add(_obj);
        _obj.gameObject.SetActive(false);
        _obj.SetParent(mTrans);
    }

    /// <summary>
    /// 将所有物体都放入池
    /// </summary>
    public void PushAll()
    {
        foreach (var child in mDatas.Values)
        {
            for (int i = 0; i < child.shows.Count; i++)
            {
                var obj = child.shows[i];
                child.pools.Add(obj);
                obj.gameObject.SetActive(false);
                obj.SetParent(mTrans);
            }
            child.shows.Clear();
        }
    }
}
