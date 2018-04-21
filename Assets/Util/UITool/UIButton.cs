using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.EventSystems;

using DG.Tweening;

/// <summary>
/// UI 按钮组件
/// </summary>
public class UIButton : MonoBehaviour
{
    //按钮状态
    public enum ButtonState
    {
        Up,         //普通状态
        Down,       //按下状态（按下后可不抬起）
        Disable,    //禁用状态
    }

    public UGUISprite up;       //默认显示的图片
    public Color upColor = Color.white;     //默认颜色
    public UGUISprite down;     //按下显示的图片，若无就用up
    public Color downColor = Color.white;   //按下颜色
    public UGUISprite disable;  //禁用显示的图片，若无就用up
    public Color disableColor = Color.white;    //禁用颜色
    public Vector3 downScale = Vector3.one;     //按下时的大小
    public float downScaleTime = 0.2f;          //按下变形事件
    //点击事件
    public Action<GameObject> onClick;      //按钮点击事件
    public Action<GameObject> onUp;         //按钮抬起回调
    public Action<GameObject, PointerEventData> onDown;     //按钮按下回调
    public Action<GameObject> onDisableClick;   //按钮被禁用状态时的点击回调
    //公共变量
    [HideInInspector]
    ButtonState state = ButtonState.Up;


    //私有
    bool mIsInit = false;
    EventTriggerListener mListener;
    Transform mTrans;
    Tweener mTween;

    void Start()
    {
        Init();
    }

    //--------------------公共方法----------------------
    #region 公共方法
    /// <summary>
    /// 初始化方法
    /// </summary>
    public virtual void Init()
    {
        if (!mIsInit)
        {
            mIsInit = true;
            if (up == null)
            {
                up = GetComponent<UGUISprite>();
                if (up == null)
                {
                    Debug.Log("按钮必须有张默认图片！！！");
                    return;
                }
            }
            mTrans = transform;
            mListener = EventTriggerListener.Get(gameObject);
            mListener.onClick = OnBtnClick;
            mListener.onUp = OnBtnUp;
            mListener.onDown = OnBtnDown;

            state = ButtonState.Up;
        }
    }
    /// <summary>
    /// 设置状态
    /// </summary>
    public void SetState(ButtonState _state)
    {
        if (state == _state)
        {
            return;
        }
        state = _state;
        SetSprite(state);
        switch (_state)
        {
            case ButtonState.Up:
                if (mTrans.localScale != Vector3.one && downScale != Vector3.one)
                {
                    KillTween();
                    mTween = mTrans.DOScale(Vector3.one, downScaleTime);
                }
                break;
            case ButtonState.Down:
                if (downScale != Vector3.one)
                {
                    KillTween();
                    mTween = mTrans.DOScale(downScale, downScaleTime);
                }
                break;
            case ButtonState.Disable:
                KillTween();
                mTrans.localScale = Vector3.one;
                break;
            default:
                break;
        }
    }
    #endregion

    //--------------------内部调用----------------------
    #region 内部调用
    void KillTween()
    {
        if (mTween != null)
        {
            mTween.Kill();
            mTween = null;
        }
    }
    /// <summary>
    /// 设置图片显隐及颜色
    /// </summary>
    void SetSprite(ButtonState _state)
    {
        if (down != null)
        {
            down.gameObject.SetActive(_state == ButtonState.Down);
        }
        if (disable != null)
        {
            disable.gameObject.SetActive(_state == ButtonState.Disable);
        }
        switch (_state)
        {
            case ButtonState.Up:
                up.enabled = true;
                up.color = upColor;     //此处不加判断，因为up是白色，其他可能有色，不如直接赋值颜色
                break;
            case ButtonState.Down:
                if (down == null)
                {
                    up.enabled = true;
                }
                else
                {
                    up.enabled = false;
                }
                if (downColor != Color.white)
                {
                    if (down == null)
                    {
                        up.color = downColor;
                    }
                    else
                    {
                        down.color = downColor;
                    }
                }
                break;
            case ButtonState.Disable:
                if (disable == null)
                {
                    up.enabled = true;
                }
                else
                {
                    up.enabled = false;
                }
                if (disableColor != Color.white)
                {
                    if (disable == null)
                    {
                        up.color = disableColor;
                    }
                    else
                    {
                        disable.color = disableColor;
                    }
                }
                break;
            default:
                break;
        }
    }
    #endregion

    //--------------------回调方法----------------------
    #region 回调方法
    void OnBtnClick(GameObject _obj)
    {
        //Debug.Log("点击");
        if (state == ButtonState.Disable)
        {
            if (onDisableClick != null)
            {
                onDisableClick(_obj);
            }
            return;
        }
        if (onClick != null)
        {
            onClick(_obj);
        }
    }
    void OnBtnUp(GameObject _obj)
    {
        //Debug.Log("抬起");
        if (state == ButtonState.Disable)
        {
            return;
        }
        SetState(ButtonState.Up);
        if (onUp != null)
        {
            onUp(_obj);
        }
    }
    void OnBtnDown(GameObject _obj, PointerEventData _event)
    {
        //Debug.Log("按下");
        if (state == ButtonState.Disable)
        {
            return;
        }
        SetState(ButtonState.Down);
        if (onDown != null)
        {
            onDown(_obj, _event);
        }
    }
    #endregion

}
