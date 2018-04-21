using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 根据人物状态，决定按钮状态
/// 如：角色进入了匹配，则按钮禁用
/// </summary>
public class UIButton_RoleState : UIButton
{
    public enum EListenState
    {
        Team,   //组队
        Room,   //房间
    }
    public List<EListenState> listenState = new List<EListenState>()    //当前按钮监听的状态，先初始化所有
    {
        EListenState.Team,
        EListenState.Room,
    };
    int listen = 0;    //当前监听的数据

    void OnEnable()
    {
        listen = 0;
        for (int i = 0; i < listenState.Count; i++)
        {
            switch (listenState[i])
            {
                case EListenState.Team:
                    //显示开启监听
                    break;
                case EListenState.Room:
                    break;
            }
        }
    }
    void OnDisable()
    {
        //隐藏取消监听
    }

    //--------------------内部调用-----------------------
    #region 内部调用
    /// <summary>
    /// 设置状态选中或取消
    /// </summary>
    void ChangeListen(EListenState _state, bool _isOn)
    {
        if (_isOn)  //加入禁用
        {
            var temp = 1 << (int)_state;
            if ((listen & temp) == 0)
            {
                listen += temp;
                //加入禁用
                SetState(ButtonState.Disable);
            }
        }
        else    //去掉禁用
        {
            var temp = 1 << (int)_state;
            if ((listen & temp) == 1)
            {
                listen -= temp;
                //去掉禁用状态
                if (listen == 0)
                {
                    SetState(ButtonState.Up);
                }
            }
        }
    }
    #endregion

    //--------------------回调方法-----------------------
    #region 回调方法
    void OnTeamStateChange(bool _isTeam)
    {
        ChangeListen(EListenState.Team, _isTeam);
    }
    #endregion

}
