using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using System.Linq;

/// <summary>
/// 时间工具
/// </summary>
public class TimeTool : MonoBehaviour
{
    class TimeData      //时间
    {
        public string Key;
        public float Seconds;
        public object data;
        public Action<object> Callback;
        public bool isDependScene;
        public float OverTime;
        public TimeData(string key, float seconds, Action<object> callback, bool _isDependScene, object _data)
        {
            Key = key;
            Seconds = seconds;
            Callback = callback;
            isDependScene = _isDependScene;
            data = _data;
            OverTime = Time.time + seconds;
        }
    }

    public static TimeTool Instacne;

    List<TimeData> _TimeList = new List<TimeData>();
    List<TimeData> _OverList = new List<TimeData>();
    bool _IsCompute = false;
    bool _IsSomeOver = false;

    void Awake()
    {
        Instacne = this;
    }
    void FixedUpdate()
    {
        if (_IsCompute)
        {
            for (int i = 0; i < _TimeList.Count; i++)
            {
                if (Time.time >= _TimeList[i].OverTime)
                {
                    _OverList.Add(_TimeList[i]);
                    _IsSomeOver = true;
                }
            }
            if (_IsSomeOver)
            {
                _IsSomeOver = false;
                for (int i = 0; i < _OverList.Count; i++)
                {
                    _TimeList.Remove(_OverList[i]);
                }
                if (_TimeList.Count == 0)
                {
                    _IsCompute = false;
                }
                //执行回调方法
                for (int i = 0; i < _OverList.Count; i++)
                {
                    _OverList[i].Callback(_OverList[i].data);
                }
                _OverList.Clear();
            }
        }
    }

    //---------------外部调用----------------
    #region 外部调用
    /// <summary>
    /// 场景切换时调用，切换依赖场景的TimeTool
    /// </summary>
    public void DoChangeScene()
    {
        var tRemove = _TimeList.Where(p => p.isDependScene).ToList();
        for (int i = 0; i < tRemove.Count; i++)
        {
            _TimeList.Remove(tRemove[i]);
        }
    }
    /// <summary>
    /// 等待一帧后，执行后续方法
    /// </summary>
    public void DoWaitForEndOfFrame(Action callback, bool _isDependScene = true)
    {
        StartCoroutine(WaitFrame(callback));
    }
    /// <summary>
    /// 等待几秒后执行方法
    /// </summary>
    public void DoWaitForSeconds(float seconds, Action<object> callback, string key = "", bool _isDependScene = true, object _data = null)
    {
        bool tIsNeed = true;
        if (!string.IsNullOrEmpty(key))
        {
            //看是否有当前key，若有，就重置时间
            for (int i = 0; i < _TimeList.Count; i++)
            {
                if (_TimeList[i].Key == key)
                {
                    _TimeList[i].OverTime = Time.time + seconds;
                    _TimeList[i].Callback = callback;
                    tIsNeed = false;
                    break;
                }
            }
        }
        if (tIsNeed)
        {
            TimeData data = new TimeData(key, seconds, callback, _isDependScene, _data);
            _TimeList.Add(data);
            _IsCompute = true;
        }
    }
    /// <summary>
    /// 删除等待
    /// </summary>
    public void RemoveWait(string key)
    {
        var _data = _TimeList.Where(p => p.Key == key).FirstOrDefault();
        _TimeList.Remove(_data);
        if (_TimeList.Count == 0)
        {
            _IsCompute = false;
        }
    }
    #endregion

    //---------------内部调用----------------
    #region 内部调用
    /// <summary>
    /// 等一帧的协程
    /// </summary>
    IEnumerator WaitFrame(Action callback)
    {
        yield return new WaitForEndOfFrame();
        if (callback != null)
        {
            callback.Invoke();
        }
    }
    #endregion
}
