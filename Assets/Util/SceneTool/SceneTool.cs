using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// 场景工具
/// </summary>
public class SceneTool : MonoBehaviour
{
    public static SceneTool Instance;
    [HideInInspector]
    public string NowSceneName = "";    //当前场景名称

    AsyncOperation _Async = null;
    Action _OnLoadOver = null;          //场景加载完毕后的回调
    Action _OnSceneChange;              //场景改变时的回调
    Action<float> _OnProgressChange;
    int _NowProgress = 0;

    void Awake()
    {
        Instance = this;
    }
    int _progress;
    void Update()
    {
        //说明需要监听场景加载进度
        if (_Async != null && _OnProgressChange != null)
        {
            _progress = (int)_Async.progress;
            if (_progress != _NowProgress)
            {
                _OnProgressChange.Invoke(_progress);
            }
        }
    }

    //----------------外部调用------------------
    #region 外部调用
    public void LoadScene(string sceneName, Action onLoadOver, Action onSceneChange = null, Action<float> onProgressChange = null)
    {
        TimeTool.Instacne.DoChangeScene();
        //尝试调用场景切换前的回调
        if (_OnSceneChange != null)
        {
            _OnSceneChange();
            _OnSceneChange = null;
        }
        //场景切换了，通知UITool
        UITool.Instance.OnSceneChange(sceneName);
        //开始真正切换场景
        _NowProgress = 0;
        _OnLoadOver = onLoadOver;
        _OnSceneChange = onSceneChange;
        _OnProgressChange = onProgressChange;
        StartCoroutine(DoLoad(sceneName));
    }
    #endregion

    //----------------内部调用------------------
    #region 内部调用
    /// <summary>
    /// 场景加载方法
    /// </summary>
    IEnumerator DoLoad(string sceneName)
    {
        _Async = SceneManager.LoadSceneAsync(sceneName);
        yield return _Async;
        NowSceneName = sceneName;
        _Async = null;
        TimeTool.Instacne.DoWaitForEndOfFrame(() =>     //等一帧，测试下
        {
            PoolTool.CreateScenePool();
            if (_OnLoadOver != null)
            {
                _OnLoadOver.Invoke();
            }
        });
    }
    #endregion
}
