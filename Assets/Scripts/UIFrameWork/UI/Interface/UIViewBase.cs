using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[Serializable]
public abstract class UIViewBase : MonoBehaviour {

    public UIControllerBase Controller { get; set; }

    private WindowID windowID = WindowID.WindowID_Invaild;

    public WindowConfigData windowData = new WindowConfigData(); //在UI打开前初始化, 以后使用配置表

    protected Transform mTransCache;

    #region 属性
    public WindowID ID
    {
        get
        {
            if (this.windowID == WindowID.WindowID_Invaild) Debug.LogError("window id is " + WindowID.WindowID_Invaild);
            return windowID;
        }
        protected set { windowID = value; }
    }
    #endregion

    #region 初始化
    public virtual void Awake()
    {
        this.gameObject.SetActive(true);
        mTransCache = this.gameObject.transform;
        SetWindowId();
        InitWindowConfigData();//以后需要导航的时候使用
        InitWindowOnAwake();
        InitController();
    }

    protected abstract void SetWindowId();

    protected abstract void InitWindowConfigData();
    protected abstract void InitWindowOnAwake();
    protected abstract void InitController();


    #endregion

    public void SetActive(bool active)
    {
        this.gameObject.SetActive(active);
    }
}
