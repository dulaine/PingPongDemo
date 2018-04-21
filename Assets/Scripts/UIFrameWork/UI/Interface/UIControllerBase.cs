using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public delegate bool BoolDelegate();

public class UIControllerBase: EventMediator, IWindowAnimation
{
    protected UIViewBase baseView;

    private bool isLoading = false;                             //正在加载或者Entering Animation中

    private bool isExiting = false;                             //正在Exiting Animation中

    protected bool isActive = false;

    private event BoolDelegate returnPreLogic = null;           //导航离开UI前的逻辑处理, 比如弹出确认UI

    private object m_UIAsset = null; //AB包中资源 version79.4
    public bool IsLoading
    {
        get { return isLoading; }
        set { isLoading = value; }
    }

    public bool IsExiting
    {
        get { return isExiting; }
        set { isExiting = value; }
    }

    public bool IsActive
    {
        get { return isActive; }
        set { isActive = value; }
    }

    public WindowConfigData UIConfigData
    {
        get { return baseView.windowData; }
    }

    public WindowID ID
    {
        get { return baseView.ID; }
    }

    public UIControllerBase(UIViewBase viewBase)
    {
        baseView = viewBase;
    }

    //version79.4
    public object Asset
    {
        set { m_UIAsset = value; }
    }

    public bool IsNavigationTypeUI
    {
        get { return UIConfigData.navigationMode == UIWindowNavigationMode.NormalNavigation; }
    }

    #region 打开
    //第一次打开
    public virtual void OnInit()
    {
    }

    public void ShowWindow(WindowContextDataBase contextData = null)
    {
        if (baseView == null) return;
        baseView.SetActive(true);
        IsActive = true;
        IsLoading = true;
        IsExiting = false;

        OnUIOpened();

        //处理动画
        DoEnteringAnimation(OnEnteringAnimationEnd);

        // this.RegisterReturnLogic(this.RetrunPreLogic);
    }

    // UI打开完毕, 进场动画结束回调
    protected virtual void OnUIOpened()
    {
        IsLoading = false;
        isActive = true;

        //todo 具体的UI打开处理 在开始动画之前
    }

    // UI打开完毕, 进场动画结束回调
    protected virtual void OnEnteringAnimationEnd()
    {
        IsLoading = false;
        isActive = true;

        //todo 具体的UI打开处理/进场动画结束回调
    }
    #endregion

    #region 关闭
    //通常是有exiting animation的关闭
    public  void HideWindow(Action action = null)
    {
        if (baseView == null) return;
        IsLoading = false;
        IsExiting = true;

        //处理动画
        Action exitingCallBack = OnUIHided;
        if (action != null)
        {
            exitingCallBack += action;
        }
        DoExitingAnimation(exitingCallBack);
    }

    //UI关闭, 退场动画结束回调
    protected virtual void OnUIHided()
    {
        IsExiting = false;
        isActive = false;
        baseView.SetActive(false);

        //todo 具体的UI关闭处理/退场动画结束回调
    }


    //不经过Exiting Animation就关闭, 多个UI统一强关
    public void HideWindowDirectly()
    {
        if (baseView == null) return;
        IsActive = false;
        IsLoading = false;
        IsExiting = false;
        baseView.SetActive(false);
        OnUIHided();
    }
    #endregion

    #region 销毁
    protected virtual void BeforeDestroyWindow()
    {
    }

    public virtual void DestroyWindow()
    {
        //if (baseView == null) return; //version49
        IsActive = false;
        IsLoading = false;
        IsExiting = false;
        BeforeDestroyWindow();

        //if (baseView == null) return; //version49
        if (baseView != null)//version49
        {
            //销毁
            GameObject obj = baseView.gameObject;
            baseView.Controller = null;
            baseView = null;
            GameObject.Destroy(obj);
            obj = null;
        }

        //version79.4
        //ResourceComponent.Instance.UnloadAsset(m_UIAsset);
    }
    #endregion

    #region UI动画

    //todo 具体的Dotween动画
    public virtual void EnteringAnimation(Action onComplete)
    {
        if (onComplete != null)
        {
            onComplete();
        }
    }

    //todo 具体的Dotween动画
    public virtual void ExitingAnimation(Action onComplete)
    {
        if (onComplete != null)
        {
            onComplete();
        }
    }

    //重置动画
    public void ResetAnimation()
    {

    }


    private void DoEnteringAnimation(Action onComplete)
    {
        ResetAnimation();
        EnteringAnimation(onComplete);
    }

    private void DoExitingAnimation(Action onComplete)
    {
        ResetAnimation();
        ExitingAnimation(onComplete);
    }
    #endregion

    #region 其他
    // 添加背景Collider后事件处理,
    // 比如给背景Collider或者Texture添加点击onClick事件, 点击背景关闭
    public virtual void OnAddColliderBg(GameObject obj)
    {

    }

    /// <summary>
    /// 点击Return导航离开UI前的逻辑处理, 
    /// 比如弹出确认UI confirm MessageBox
    /// </summary>
    protected void RegisterReturnLogic(BoolDelegate newLogic)
    {
        returnPreLogic = newLogic;
    }

    public bool ExecuteReturnLogic()
    {
        if (returnPreLogic == null)
            return false;
        else
            return returnPreLogic();
    }
    #endregion


    protected void LoadSpriteFromAtlas(string atlas, string sprite, Action<object> callback)//version68
    {
        // WolfGame.AtlasManager.GetInstance().GetSpriteByAtlasNameAndImageName(atlas, sprite, callback);
    }

    virtual public void UpdateUI()
    {

    }
}
