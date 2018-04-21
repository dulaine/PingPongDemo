using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum UIAnimationType
{
    Alpha,
    Position,
    Scale,
}

public struct UIAnimationTemplateInfo
{
    public int TemplateID;

    public int PlayTime;                    //所有Animation Duration的最大值

    public AnimationInfo[] AnimationInfo;
}

public struct AnimationInfo
{
    public UIAnimationType Type;

    public float From;

    public float To;

    public float Delay;

    public float Duration;

    public AnimationCurve AnimationCurve;
}
