using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PureMVC.Interfaces;
using PureMVC.Patterns;

/// <summary>
/// 事件中介基类
/// </summary>
public class EventMediator : Notifier, IMediator
{
    protected string m_mediatorName;
    protected object m_viewComponent;

    public virtual string MediatorName
    {
        get { return m_mediatorName; }
    }

    public object ViewComponent
    {
        get { return m_viewComponent; }
        set { m_viewComponent = value; }
    }

    public virtual IEnumerable<string> ListNotificationInterests
    {
        get { return new List<string>(); }
    }

    public EventMediator()
    {

    }

    public EventMediator(string _mediatorName)
    {
        this.m_mediatorName = _mediatorName;
    }


    public virtual void HandleNotification(INotification notification)
    {

    }

    public virtual void OnRegister()
    {

    }

    public virtual void OnRemove()
    {

    }

    public virtual void ResetMediator()
    {

    }
	
}
