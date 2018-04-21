using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationTemplateManager  {

    Dictionary<int, UIAnimationTemplateInfo> templateInfoDic = new Dictionary<int, UIAnimationTemplateInfo>();

    private static AnimationTemplateManager _instance;

    public static AnimationTemplateManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = new AnimationTemplateManager();
            }
            return _instance;
        }
    }

    //初始化动画模板信息[从prefab上读取动画模板信息]
    public void InitTemplated()
    {

    }

    public void ClearTemplate()
    {
        templateInfoDic.Clear();
    }


    public UIAnimationTemplateInfo GetTemplateInfo(int id)
    {
        UIAnimationTemplateInfo info = default(UIAnimationTemplateInfo) ;

        return info;
    }

    #region 通用UI动画获取
    //UI通用入场动画
    public UIAnimationTemplateInfo GetUIOpenTemplate()
    {
        return GetTemplateInfo(1);
    }

    //UI通用退场动画
    public UIAnimationTemplateInfo GetUICloseTemplate()
    {
        return GetTemplateInfo(2);
    }
    #endregion
}
