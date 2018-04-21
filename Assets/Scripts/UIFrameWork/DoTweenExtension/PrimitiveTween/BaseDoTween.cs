using System;
using DG.Tweening;
using UnityEngine;

[Serializable]
public abstract class BaseDoTween : MonoBehaviour , ITweener
{
    protected ulong playID = 0;  //动画标示ID

    public enum Style
    {
        Once,
        Loop,
        PingPong,
    }


    /// <summary>
    /// Does it play once? Does it loop?
    /// </summary>

    [HideInInspector]
    public Style style = Style.Once;


    /// <summary>
    /// Optional curve to apply to the tween's time factor value.
    /// </summary>

    [HideInInspector]
    public AnimationCurve animationCurve = new AnimationCurve(new Keyframe(0f, 0f, 0f, 1f), new Keyframe(1f, 1f, 1f, 0f));

    /// <summary>
    /// How long is the duration of the tween?
    /// </summary>

    [HideInInspector]
    public float duration = 1f;

    /// <summary>
    /// How long will the tweener wait before starting the tween?
    /// </summary>

    [HideInInspector]
    public float delay = 0f;

    [HideInInspector]
    protected Sequence uiAnimation;

    [HideInInspector]
    public BaseDoTween OnComplete;      //动画结束回调动画, 美术配置用, 注意循环的动画没有回调

    [HideInInspector]
    public Action OnCompleteAction;     //动画结束回调, 程序代码使用,注意循环的动画没有回调

    //播放自己的Tweeners
    public virtual void Play(float extraDelay = 0)
    {
        Stop();
        uiAnimation = GetAnimationSequence();
        if (extraDelay > 0) uiAnimation.SetDelay(extraDelay);

        //结束回调美术配置动画
        if (OnComplete != null)
        {
            uiAnimation.onComplete += () =>
            {
                OnComplete.Play();
            };
        }

        //结束回调Action
        if (OnCompleteAction != null)
        {
            uiAnimation.onComplete += () =>
            {
                OnCompleteAction();
            };
        }

        playID = GameUIUtility.GetUniquePlayID();
        uiAnimation.SetId(playID);

        uiAnimation.Play();
    }

    public virtual void Stop()
    {
        if (playID > 0)
        {
            DOTween.Kill(playID, false);
            playID = 0;
            uiAnimation = null;
        }
    }

    public virtual void Reset()
    {

    }

    public abstract Sequence GetAnimationSequence();

    public virtual float GetAnimationLength()
    {
        return duration;
    }
}
