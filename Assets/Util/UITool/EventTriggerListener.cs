using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;


//public class EventTriggerListener : EventTrigger
/// <summary>
/// 取消onDrag的监听，防止重写了ScrollRect的监听消息
/// </summary>
public class EventTriggerListener : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler, IPointerUpHandler, IPointerClickHandler, IUpdateSelectedHandler, ISelectHandler, IDeselectHandler, IEventSystemHandler
{
    public delegate void VoidDelegate(GameObject go);
    public delegate void EventDelegate(GameObject go, PointerEventData eventData);
    public VoidDelegate onClick;
    public EventDelegate onDown;
    //public VoidDelegate onDrag;   //取消onDrag的监听，防止重写了ScrollRect的监听消息
    public VoidDelegate onEnter;
    public VoidDelegate onExit;
    public VoidDelegate onUp;
    public VoidDelegate onSelect;
    public VoidDelegate onUpdateSelect;
    public VoidDelegate onDeselect;
    public EventDelegate onPress;
    //用于实现Press的字段
    private bool isPointDown = false;
    private float lastInvokeTime;
    private float pressInterval = 0.1f;
    private PointerEventData _eventData;
    //其他
    //ScrollRect mScrollView = null;

    public static EventTriggerListener Get(GameObject go)
    {
        EventTriggerListener listener = go.GetComponent<EventTriggerListener>();
        if (listener == null) listener = go.AddComponent<EventTriggerListener>();
        return listener;
    }

    public virtual void OnPointerClick(PointerEventData eventData)
    {
        if (onClick != null)
            onClick(gameObject);
    }
    public virtual void OnPointerDown(PointerEventData eventData)
    {
        if (onDown != null) onDown(gameObject, eventData);
        //用于实现Press
        if (onPress != null)
        {
            _eventData = eventData;
            isPointDown = true;
            lastInvokeTime = Time.time;
            onPress.Invoke(gameObject, _eventData);
        }
    }
    public virtual void OnPointerEnter(PointerEventData eventData)
    {
        if (onEnter != null) onEnter(gameObject);
    }
    public virtual void OnPointerExit(PointerEventData eventData)
    {
        if (onExit != null) onExit(gameObject);
        //用于实现Press
        isPointDown = false;
        _eventData = null;
    }
    public virtual void OnPointerUp(PointerEventData eventData)
    {
        if (onUp != null) onUp(gameObject);
        //用于实现Press
        isPointDown = false;
        _eventData = null;
    }
    public virtual void OnSelect(BaseEventData eventData)
    {
        if (onSelect != null) onSelect(gameObject);
    }
    public virtual void OnUpdateSelected(BaseEventData eventData)
    {
        if (onUpdateSelect != null) onUpdateSelect(gameObject);
    }
    public virtual void OnDeselect(BaseEventData eventData)
    {
        if (onDeselect != null) onDeselect(gameObject);
    }

    //public virtual void OnDrag(PointerEventData eventData)
    //{
    //    if (mScrollView != null)
    //    {
    //        mScrollView.OnDrag(eventData);
    //    }
    //    else if (onDrag != null)
    //    {
    //        onDrag(gameObject);
    //    }
    //}

    void Update()
    {
        if (isPointDown && onPress != null)
        {
            if (Time.time - lastInvokeTime > pressInterval)
            {
                //触发点击;
                onPress.Invoke(gameObject, _eventData);
                lastInvokeTime = Time.time;
            }
        }

    }

    //------------公共方法---------------
    #region 公共方法
    /// <summary>
    /// 设置UGUI的滑动条
    /// 通过此设置，OnDrag就不会响应回调，而是直接调用ScrollRect的OnDrag
    /// </summary>
    //public void SetScrollView(ScrollRect _rect)
    //{
    //    mScrollView = _rect;
    //}
    #endregion
}