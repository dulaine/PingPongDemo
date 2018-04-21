using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;

[Serializable]
public class DoTweenAlpha : BaseDoTween {

    [Range(0f, 1f)]
    public float From = 1f;
    [Range(0f, 1f)]
    public float To = 1f;

    private CanvasGroup m_CanvasGroup;

    void Start()
    {
        m_CanvasGroup = GetComponent<CanvasGroup>();
    }

    public override Sequence GetAnimationSequence()
    {
        Tweener tweener = null;

        //1. 检测是否有CanvaGroup
        m_CanvasGroup = GetComponent<CanvasGroup>();
        if (m_CanvasGroup != null)
        {
            uiAnimation = DOTween.Sequence();
            if (delay > 0) uiAnimation.AppendInterval(delay);

            tweener = GameUIUtility.DoAlpha(m_CanvasGroup, From, To, duration);

        }
        else
        {
            //2. 是否是Button
            Button button = GetComponent<Button>();
            if (button != null)
            {
                uiAnimation = DOTween.Sequence();
                if (delay > 0) uiAnimation.AppendInterval(delay);
                tweener = GameUIUtility.DoAlpha(button, From, To, duration);
            }
            else
            {
                //3. 是否是Image RawImage或者Text
                MaskableGraphic graphic = GetComponent<MaskableGraphic>();
                if (graphic != null)
                {
                    uiAnimation = DOTween.Sequence();
                    if (delay > 0) uiAnimation.AppendInterval(delay);
                    tweener = GameUIUtility.DoAlpha<MaskableGraphic>(graphic, From, To, duration);
                }
                else
                {
                    //4. 都不是, 那么应该添加CanvasGroup来实现
                    m_CanvasGroup = this.gameObject.AddComponent<CanvasGroup>();
                    //Debug.LogError("需要CanvasGroup组件来执行Alpha Animation");
                    //return null;
                    uiAnimation = DOTween.Sequence();
                    if (delay > 0) uiAnimation.AppendInterval(delay);

                    tweener = GameUIUtility.DoAlpha(m_CanvasGroup, From, To, duration);
                }
            }
        }

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

        ////结束回调
        //if (OnComplete != null)
        //{
        //    uiAnimation.onComplete = () => { OnComplete.Play(); };
        //}

        return uiAnimation;
    }
}
