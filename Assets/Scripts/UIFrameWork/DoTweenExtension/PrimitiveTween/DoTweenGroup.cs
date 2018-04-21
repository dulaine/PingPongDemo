using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

[Serializable]
public class DoTweenGroup : BaseDoTween
{
    public BaseDoTween[] AnimationArray;              //播放动画

    public override Sequence GetAnimationSequence()
    {
        if (AnimationArray != null && AnimationArray.Length > 0)
        {
            uiAnimation = DOTween.Sequence();
            if (delay > 0) uiAnimation.SetDelay(delay);

            for (int i = 0; i < AnimationArray.Length; i++)
            {
                if (AnimationArray[i] == null)
                    continue;

                Sequence anim = AnimationArray[i].GetAnimationSequence();
                if (anim != null)
                {
                    uiAnimation.Join(anim);
                }
            }

            return uiAnimation;
        }

        return null;
    }


    public override float GetAnimationLength()
    {
        float len = 0;
        if (AnimationArray != null && AnimationArray.Length > 0)
        {
            for (int i = 0; i < AnimationArray.Length; i++)
            {
                float thisLen = AnimationArray[i].GetAnimationLength();

                if (thisLen > len)
                {
                    len = thisLen;
                }
            }
        }
        return len;
    }

}
