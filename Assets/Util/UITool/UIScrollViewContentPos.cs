using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// UI ScrollView 容器位置自动变化
/// </summary>
public class UIScrollViewContentPos : MonoBehaviour
{
    public RectTransform scrollView;

    bool mIsInit = false;
    const float cBuff = 10;
    RectTransform mRect;

    void Init()
    {
        if (!mIsInit)
        {
            mIsInit = true;
            mRect = transform as RectTransform;
        }
    }

    /// <summary>
    /// 是否在最下方
    /// </summary>
    public bool IsBelow()
    {
        Init();
        if (mRect.rect.height < scrollView.rect.height)
        {
            return true;
        }
        else if (mRect.rect.height - scrollView.rect.height - mRect.localPosition.y < cBuff)
        {
            return true;
        }
        return false;
    }
    public void Refresh()
    {
        Init();
        TimeTool.Instacne.DoWaitForEndOfFrame(() =>
        {
            //是否超过了范围
            if (mRect.rect.height < scrollView.rect.height)
            {
                return;
            }
            mRect.localPosition = new Vector3(mRect.localPosition.x, mRect.rect.height - scrollView.rect.height, 0);
        });
    }
}
