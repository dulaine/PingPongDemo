using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 环装列表
/// </summary>
public class UILoopGrid : MonoBehaviour
{
    public RectTransform scrollView;    //scrollView，用来确定当前容器的位置
    public RectTransform content;       //放置item的容器
    public Vector2 size;        //容器元素的大小
    public Vector2 space;       //容器元素的间隔
    //容器元素的来源
    public EasyPool pool;       //元素来源于池

    List<Vector2> mPositions = new List<Vector2>();
    float mStartPosY = 0;

    //-------------------公共方法----------------------
    #region 公共方法
    public void Init()
    {
        mStartPosY = -size.y * 0.5f;
    }
    /// <summary>
    /// 竖直排版
    /// </summary>
    public void SetLayout(int _rowCount)
    {
        content.sizeDelta = new Vector2(content.sizeDelta.x, (size.y + space.y) * _rowCount - space.y);
        //初始化所有索引的位置
        mPositions.Clear();
        for (int i = 0; i < _rowCount; i++)
        {
            mPositions.Add(new Vector2(0, mStartPosY - size.y * i - space.y));
        }
    }

    /// <summary>
    /// 通过索引获取此索引对应的位置
    /// </summary>
    public Vector2 GetPosFromIndex(int _index)
    {
        return mPositions[_index];
    }
    /// <summary>
    /// 刷新界面
    /// </summary>
    public void Refresh()
    {

    }
    #endregion

    /// <summary>
    /// 检测是否刷新位置
    /// </summary>
    bool CheckUpdatePos()
    {
        if (content.sizeDelta.y < scrollView.sizeDelta.y)
        {
            return false;
        }
        if (content.localPosition.y < (content.sizeDelta.y * 0.5f - 2))
        {
            return true;
        }
        return true;
    }
}
