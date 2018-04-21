using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

/// <summary>
/// toggle组
/// 原则一定是，先触发取消，再触发选中
/// </summary>
public class UIToggleGroup : MonoBehaviour
{
    [SerializeField]
    UIToggleItem father;
    [SerializeField]
    List<UIToggleItem> toggles = new List<UIToggleItem>();
    public bool isInit = true;      //是否默认初始化一个
    public UIToggleItem normalShow;

    UIToggleItem mNowShow;

    void OnEnable()
    {
        Init();
    }
    bool mIsInit = false;
    /// <summary>
    /// 初始化toggle
    /// </summary>
    public void Init()
    {
        if (mIsInit)
        {
            return;
        }
        mIsInit = true;
        if (!isInit)
        {
            return;
        }
        if (mNowShow == null)
        {
            if (normalShow != null)
            {
                mNowShow = normalShow;
            }
            else
            {
                mNowShow = toggles[0];
            }
            mNowShow.ChangeOn();
        }
    }
    /// <summary>
    /// 清理toggle
    /// </summary>
    public void Clear()
    {
        toggles.Clear();
    }
    /// <summary>
    /// 添加一个选项
    /// </summary>
    public void Add(UIToggleItem _item, Action _SameCB = null)
    {
        toggles.Add(_item);
        if (_item == mNowShow && _SameCB != null)
        {
            _SameCB();
        }
    }
    /// <summary>
    /// 默认选中一个
    /// </summary>
    public void Choice(string _name)
    {

    }
    /// <summary>
    /// 将选中的item取消选中
    /// </summary>
    public void Reset()
    {
        Init();
        if (mNowShow != null)
        {
            mNowShow.ChangeOff();
            mNowShow = null;
        }
    }
    /// <summary>
    /// 设置某个item被选中
    /// </summary>
    public void Change(UIToggleItem _item)
    {
        if (mNowShow == _item)
        {
            return;
        }
        Reset();
        mNowShow = _item;
        if (father != null)
        {
            father.ChangeOn();
        }
    }
}
