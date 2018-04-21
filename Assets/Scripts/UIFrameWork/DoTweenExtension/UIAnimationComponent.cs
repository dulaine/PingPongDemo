using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIAnimationComponent : MonoBehaviour
{
    public BaseDoTween[] MyTweenGroups;       //拥有的UI动画[美术配置]

    public int SelectPlayGroupIndex;

    #region 按索引播放,停止动画
    //播放自己拥有的UI动画[美术挂在这个UI上的动画]
    public void PlayAtIndex(int index, Action callBack = null)
    {
        if (MyTweenGroups != null && MyTweenGroups.Length > index)
        {
            BaseDoTween anim = MyTweenGroups[index];
            if (anim != null)
            {
                SelectPlayGroupIndex = index;

                if (callBack != null)
                    anim.OnCompleteAction = callBack;

                anim.Play();
            }
        }
    }

    //停止当前播放的动画
    public void StopCurrentAnimation()
    {
        if (SelectPlayGroupIndex > 0)
        {
            StopAtIndex(SelectPlayGroupIndex);
        }
    }

    //停止动画
    public void StopAtIndex(int index)
    {
        if (MyTweenGroups != null && MyTweenGroups.Length > index)
        {
            BaseDoTween anim = MyTweenGroups[index];
            if (anim != null)
            {
                anim.Stop();
            }
        }
    }
    #endregion


    #region 播放模板动画
    //播放指定的动画模板[比如:通用的开场和退场UI动画]
    public void PlayByTemplateInfo(int ID)
    {
        UIAnimationTemplateInfo info = AnimationTemplateManager.Instance.GetTemplateInfo(ID);
        PlayTemplate(info);
    }

    private void PlayTemplate(UIAnimationTemplateInfo info)
    {
        //根据Info的type,添加对应的脚本, play动画
    }
    #endregion

    //public void PlayAlpahTween()
    //{

    //}

    //public void PlayPositionTween()
    //{

    //}

    //public void PlayScaleTween()
    //{

    //}


}
