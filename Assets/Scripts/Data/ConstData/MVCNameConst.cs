using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MVCNameConst
{
    //Command名称
    #region Command名称
    //初始化
    public const string Init_Main = "Init_Main";                        //总的初始化方法
    public const string Init_ConfigData = "Init_ConfigData";            //初始化配置数据
    //更新
    public const string Update_Check = "Update_Check";                  //更新检测方法
    //场景
    public const string Scene_LoadLoginScene = "Scene_LoadLoginScene";  //加载登录场景命令
    public const string Scene_LoadMainScene = "Scene_LoadMainScene";    //加载主场景命令
    public const string Scene_LoadWarScene = "Scene_LoadWarScene";      //加载战斗场景命令
    public const string Scene_LoadGuideScene = "Scene_LoadGuideScene";  //加载引导场景命令
    //主场景
    public const string Home_ChangeRole = "Home_ChangeRole";    //Home场景切换角色模型，若没有角色则创建
    //战斗
    public const string Battle_CreateRoleMy = "Battle_CreateRoleMy";    //战斗场景加载自控角色
    public const string Battle_CreateRoleOther = "Battle_CreateRoleOther";      //战斗场景加载其他角色
    public const string Battle_CreateGoods = "Battle_CreateGoods";      //战斗场景创建道具
    public const string Battle_Finished = "Battle_Finished";    //战斗结束命令

    //UI
    public const string UI_ShowDownloadUI = "UI_ShowDownloadUI";        //下载
    public const string UI_RefreshMoneyInfo = "UI_RefreshMoneyInfo";        //任何界面需要刷新金币和钻石时调用
    public const string UI_ShowTalkUI = "UI_ShowTalkUI";        //聊天UI

    //SDK
    public const string SDK_Share = "SDK_Share";        //SDK的分享命令
    public const string SDK_Login = "SDK_Login";        //SDK的登录命令

    #endregion

    //Mediator名称
    #region Mediator名称
    public const string Role_ChangeMoveState = "Role_ChangeMoveState";  //角色修改移动状态
    //自控角色
    public const string Role_TryChangeToMove = "Role_TryChangeToMove";  //切换到移动状态
    public const string Role_TryChangeToIdle = "Role_TryChangeToIdle";  //切换到站立状态
    public const string Role_TryChangeFlashlight = "Role_TryChangeFlashlight";      //尝试修改手电状态
    public const string Role_SetFlashlight = "Role_SetFlashlight";      //设置手电筒（类似换武器）
    public const string Role_Remove = "Role_Remove";          //角色彻底删除
    public const string Role_TryUseItemUp = "Role_TryUseItemUp";    //角色抬起道具按钮
    public const string Role_TryUseItemDown = "Role_TryUseItemDown";    //角色按下道具按钮
    public const string Role_UseItemSuccess = "Role_UseItemSuccess";      //角色道具使用成功
    public const string Role_TryPickUpItem = "Role_TryPickUpItem";      //角色尝试拾取道具
    public const string Role_OnGhostShow = "Role_OnGhostShow";  //鬼隐形现形时发送的方法
    public const string Role_GhostRun = "Role_GhostRun";    //鬼的自控角色尝试使用加速技能
    public const string Role_Goto = "Role_Goto";            //瞬移(传送)方法
    public const string Role_Kick = "Role_Kick";            //人被踢飞到目标点方法
    public const string Role_SetTarget = "Role_SetTarget";              //角色设置自己的当前目标
    public const string Role_RobotWait = "Role_RobotWait";              //机器人等待状态
    public const string Role_RobotStartPatrol = "Role_RobotStartPatrol";        //AI机器人开始巡逻消息
    public const string Role_GoodsEquip = "Role_GoodsEquip";            //道具装备消息
    public const string Role_ShowNetGhost = "Role_ShowNetGhost";        //显示网络鬼
    public const string Role_ForceBack = "RoleNet_ForceBack";           //接到角色强制拽回消息
    public const string Role_TryUseSkillDown = "Role_SkillDown";      //技能按钮按下
    public const string Role_TryUseSkillUp = "Role_SkillUp";          //技能按钮抬起
    public const string Role_RoleFog = "Role_RoleFog";                  //进出人雾的消息
    public const string Role_GhostFog = "Role_GhostFog";                //进出鬼雾的消息

    //角色网络消息
    public const string RoleNet_Move = "RoleNet_Move";          //接到网络角色移动消息
    public const string RoleNet_SetFlashlight = "RoleNet_SetFlashlight";    //接到网络角色手电消息
    public const string RoleNet_SetState = "RoleNet_SetState";              //设置网络状态
    public const string RoleNet_SetScare = "RoleNet_SetScare";              //设置角色惊吓状态
    public const string RoleNet_ClearState = "RoleNet_ClearState";          //取消网络状态
    public const string RoleNet_FlashlightPower = "RoleNet_FlashlightPower";    //角色手电消息
    public const string RoleNet_ChangeDir = "RoleNet_ChangeDir";            //接到网络角色朝向修改消息，通常是机器人
    public const string RoleNet_ChangeLv = "RoleNet_ChangeLv";  //角色等级修改
    public const string RoleNet_Skill = "RoleNet_Skill";        //角色接到技能释放消息
    public const string RoleNet_BackOff = "RoleNet_BackOff";                //角色被击退到目标位置

    //UI
    public const string UI_UIShow = "UI_UIShow";    //UI显示
    public const string UI_UIHide = "UI_UIHide";    //UI隐藏

    public const string UIDisplayMessage_Show = "UIDisplayMessage_Show";    //飘字UI

    public const string UIDownload_DLAssetInit = "UIDownload_DLAssetInit";      //初始化下载场景
    public const string UIDownload_DLComplete = "UIDownload_DLComplete";    //单个资源下载完毕回调

    public const string UIConfirmPanel_Init = "UIConfirmPanel_Init";    //确认面板初始化
    public const string UINetMessatePanel_Init = "UINetMessatePanel_Init";    //网络信息面板初始化
    public const string UIResultPanel_Init = "UIResultPanel_Init";  //初始化战斗结果面板的内容

    //public const string UIUserInfo_Init = "UIUserInfo_Init";    //用户信息——初始化界面
    public const string UIUserInfo_UserName = "UIUserInfo_UserName";  //用户信息——名字
    public const string UIUserInfo_MatchingScore = "UIUserInfo_MatchingScore";    //用户信息——段位分数
    public const string UIUserInfo_Money = "UIUserInfo_Money";    //用户信息——钱
    public const string UIUserInfo_UserIcon = "UIUserInfo_UserIcon";  //用户信息——用户头像
    public const string UIUserInfo_TeamInvite = "UIUserInfo_TeamInvite";    //用户收到组队邀请相关
    public const string UIUserInfoPanel_ShowUserInfo = "UIUserInfoPanel_ShowInfo";  //用户信息面板——显示用户信息
    public const string UIUserInfoPanel_RefreshInfo = "UIUserInfoPanel_RefreshInfo";    //用户信息面板——刷新

    public const string UIGuide_Talk = "UIGuide_Talk";          //引导聊天
    public const string UIGuide_Arrows = "UIGuide_Arrows";      //引导箭头
    public const string UIGuide_Hide = "UIGuide_Hide";          //引导隐藏
    public const string UIGuide_Reset = "UIGuide_Reset";        //引导重置
    public const string UIGuideHUD_Circle = "UIGuideHUD_Circle";    //引导HUD界面的圆圈
    public const string UIGuideHUD_Hide = "UIGuideHUD_Hide";    //引导HUD隐藏

    public const string UIBubbleHUD_Init = "UIBubbleHUD_Init";      //气泡HUD初始化
    public const string UIBubbleHUD_Refresh = "UIBubbleHUD_Refresh";        //气泡HUD刷新
    public const string UIBubbleHUD_HidePanel = "UIBubbleHUD_HidePanel";        //关闭气泡HUD面板

    public const string UIShopInterface_RefreshItemInfo = "UIShopInterface_RefreshItemInfo";        //刷新单个物品信息
    public const string UIShopInterface_RefreshSkinInfo = "UIShopInterface_RefreshSkinInfo";        //切换角色专用，刷新皮肤的装备信息

    public const string UIShopPanel_InitPanel = "UIShopPanel_InitPanel";        //商店弹板初始化
    public const string UIShopPanel_RefreshPanel = "UIShopPanel_RefreshPanel";      //商店面板内容刷新

    public const string UITalk_Add = "UITalk_Add";      //接到一条聊天消息
    public const string UITalk_Del = "UITalk_Del";      //删除一条聊天消息
    public const string UITalk_Channel = "UITalk_Channel";      //刷新一个频道的数据
    public const string UITalk_AddFriendChannel = "UITalk_AddFriendChannel";    //添加一个好友频道
    public const string UITalkSmall_Hint = "UITalkSmall_Hint";      //打开或关闭小对话条的提示

    public const string UIRelationPanel_Add = "UIRelationPanel_Add";    //添加一个关系
    public const string UIRelationPanel_Remove = "UIRelationPanel_Remove";    //移除一个关系
    public const string UIRelationPanel_Update = "UIRelationPanel_Update";    //更新一个关系

    public const string UITeam_Init = "UITeam_Init";    //组队初始化
    public const string UITeam_Refresh = "UITeam_Refresh";  //组队成员信息变化
    public const string UITeam_ChangeMode = "UITeam_ChangeMode";    //组队模式变换
    public const string UITeam_RelationRefresh = "UITeam_RelationRefresh";  //组队界面房主好友刷新

    public const string UIMatchMode_Init = "UIMatchMode_Init";      //根据传入的人数匹配可以选择的模式

    //摇杆
    public const string Joystick_InitButton = "Joystick_InitButton";    //初始化摇杆的技能按钮
    public const string Joystick_ItemUsed_Temp = "Joystick_ItemUsed_Temp";    //临时道具使用完毕
    public const string Joystick_ItemUsed_CD = "Joystick_ItemUsed_CD";    //cd道具使用完毕
    public const string Joystick_SkillUsed = "Joystick_SkillUsed";      //技能使用完毕
    public const string Joystick_HideButton = "Joystick_HideButton";    //隐藏按钮，仅在漫游模式下触发
    public const string Joystick_PickSkillItem = "Joystick_PickSkillItem";      //初始化技能道具

    //准备
    public const string BackpackInterface_Init = "BackpackInterface_Init";      //刷新模型
    public const string BackpackInterface_StopActive = "BackpackInterface_StopActive";      //停止一切活动模型
    
    //战斗
    public const string Thunder_Show = "Thunder_Show";      //显示或切换闪电
    public const string Thunder_OnHide = "Thunder_OnHide";  //闪电隐藏后发送，类似回调
    public const string Role_HPInfo = "Role_HPInfo";      //接收角色血量信息

    public const string BattleInterface_Init = "BattleInterface_Init";        //战斗面板初始化
    public const string BattleInterface_Show = "BattleInterface_Show";      //战斗面板开始显示并计时
    public const string BattleInterface_RefreshGhostNum = "BattleInterface_RefreshGhostNum";    //刷新鬼的命数UI
    public const string BattleInterface_RefreshHumanNum = "BattleInterface_RefreshHumanNum";    //刷新人的血量UI
    public const string BattleInterface_HumanWarning = "BattleInterface_HumanWarning";      //战斗警报等级——人
    public const string BattleInterface_GhostTip = "BattleInterface_GhostTip";      //提示面板——鬼
    //public const string BattleInterface_ClockSecond = "BattleInterface_ClockSecond";      //战斗面板倒计时的秒数
    public const string BattleInterface_StopActive = "BattleInterface_StopActive";  //战斗面板停止活动
    public const string BattleInterface_RefreshMedal = "BattleInterface_RefreshMedal";        //刷新奖章数——鬼

    //HomeScene
    public const string HomeScene_Init = "HomeScene_Init";  //初始化场景
    public const string HomeScene_CharacterChange = "HomeScene_CharacterChange";    //角色切换
    public const string HomeScene_CharacterRotate = "HomeScene_CharacterRotate";      //角色旋转
    public const string HomeScene_CharacterShow = "HomeScene_CharacterShow";      //角色快速展示
    public const string HomeScene_ChangeModelPosition = "HomeScene_ChangeModelPosition";        //模型位置

    //声音
    public const string Audio_BGM = "Audio_BGM";        //背景音乐
    public const string Audio_Effect = "Audio_Effect";      //音效
    public const string Audio_SetBGM = "Audio_SetBGM";      //背景音乐开关
    public const string Audio_SetEffect = "Audio_SetEffect";        //音效开关
    public const string Audio_StopAllSE = "Audio_StopAllAudio";      //停止所有声音

    //聊天
    public const string Talk_SetMic = "Talk_SetMic";        //设置麦克风
    public const string Talk_SetSound = "Talk_SetAudio";        //设置接听他人语音

    //模型演出
    public const string Show_ChangeController = "Show_ChangeController";    //切换动画控制器
    public const string Show_ChangeClip = "Show_ChangeClip";    //切换动画片段
    public const string Show_Idle = "Show_Idle";
    public const string Show_Win = "Show_Win";
    public const string Show_Lose = "Show_Lose";
    public const string Show_Random = "Show_Random";

    //镜子
    public const string Mirror_Goto = "Mirror_Goto";    //镜子传送
    #endregion
}
