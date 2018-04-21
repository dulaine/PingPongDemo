using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

/// <summary>
/// 自写toggle组件
/// </summary>
public class UIToggleItem : MonoBehaviour
{
    public UIToggleGroup group;
    public UIToggleGroup child;     //item取消选中时，子group下的所有物体都会取消选中
    [HideInInspector]
    public bool isOn;
    [SerializeField]
    GameObject imageOn;
    [SerializeField]
    GameObject imageOff;
    [SerializeField]
    GameObject hint;        //提示物体，仅适用于点击此item后提示取消
    public Action<bool> onChange;   //状态变化回调

    void OnEnable()
    {
        EventTriggerListener.Get(imageOff).onClick = OnClick;
    }

    /// <summary>
    /// 被通知item关闭
    /// </summary>
    public void ChangeOff(bool _isCB = true)
    {
        if (!isOn)
        {
            return;
        }
        isOn = false;
        SetActive(false);
        //执行子物体组的取消选中
        if (child != null)
        {
            child.Reset();
        }
        if (_isCB && onChange != null)
        {
            onChange(false);
        }
    }
    /// <summary>
    /// 点击打开item
    /// </summary>
    public void ChangeOn(bool _isCB = true)
    {
        Hint(false);        //点击item时隐藏提示点
        if (isOn)
        {
            return;
        }
        isOn = true;
        SetActive(true);
        //先通知group，让group取消选中其他
        group.Change(this);
        //调用回调
        if (_isCB && onChange != null)
        {
            onChange(true);
        }
    }
    /// <summary>
    /// 选项提示
    /// </summary>
    public void Hint(bool _show)
    {
        if (hint != null)
        {
            hint.SetActive(_show);
        }
    }
    //--------------内部调用----------------
    #region 内部调用

    void SetActive(bool _isOn)
    {
        if (imageOn != null)
        {
            imageOn.SetActive(_isOn);
        }
        if (imageOff != null)
        {
            imageOff.SetActive(!_isOn);
        }
    }
    #endregion

    //--------------回调方法----------------
    #region 回调方法
    void OnClick(GameObject _obj)
    {
        if (isOn)
        {
            return;
        }
        ChangeOn();
    }
    #endregion
}
