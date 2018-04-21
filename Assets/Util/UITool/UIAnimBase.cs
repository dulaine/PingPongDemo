using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

/// <summary>
/// UI动画接口
/// </summary>
public interface IUIAnim
{
    /// <summary>
    /// 显示动画
    /// </summary>
    void EnterAnimation(Action _callback);

    /// <summary>
    /// 隐藏动画
    /// </summary>
    void QuitAnimation(Action _callback);

    /// <summary>
    /// 重置动画
    /// </summary>
    void ResetAnimation();
}

public class UIAnimBase : MonoBehaviour
{

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }
}
