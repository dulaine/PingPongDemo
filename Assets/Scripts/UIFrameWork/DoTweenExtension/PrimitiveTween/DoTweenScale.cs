using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

[Serializable]
public class DoTweenScale : BaseDoTween
{
    public Vector3 From;

    public Vector3 To;

    public override Sequence GetAnimationSequence()
    {
        uiAnimation = DOTween.Sequence();

        if (delay > 0) uiAnimation.AppendInterval(delay);
        
        Tweener tweener = GameUIUtility.DoScale(this.transform, From, To, duration);

        //Animation Curve
        if (animationCurve != null) tweener.SetEase(animationCurve);

        //Loops
        switch (style)
        {
            case Style.Once:
                break;
            case Style.Loop:
                tweener.SetLoops(int.MaxValue, LoopType.Restart);
                break;
            case Style.PingPong:
                tweener.SetLoops(int.MaxValue, LoopType.Yoyo);
                break;
        }

        uiAnimation.Append(tweener);

        //结束回调
        if (OnComplete != null)
        {
            uiAnimation.onComplete = () => { OnComplete.Play(); };
        }
        return uiAnimation;
    }


}
