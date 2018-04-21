using UnityEngine;
using System.Collections;
using System;

/// <summary>
/// 窗口动画
/// </summary>
interface IWindowAnimation
{
    /// <summary>
    /// 显示动画
    /// </summary>
    void EnteringAnimation(Action onComplete);
        
    /// <summary>
    /// 隐藏动画
    /// </summary>
    void ExitingAnimation(Action onComplete);
        
    /// <summary>
    /// 重置动画
    /// </summary>
    void ResetAnimation();
}


