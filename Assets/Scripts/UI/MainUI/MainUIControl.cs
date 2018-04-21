
/*
   file desc: Auto Generation by [UGUIScriptGenerator] 
*/


using UnityEngine;
using UnityEngine.UI;
using System.IO;
using System;
using System.Collections.Generic;
using PureMVC.Interfaces;
using UnityEngine.EventSystems;

namespace PingpongUI
{
    public class MainUIControl : UIControllerBase
    {
        public MainUIControl(UIViewBase viewBase) : base(viewBase)
        {
            m_mediatorName = "MainUIControl";
        }

        protected MainUIView View
        {
            get { return (MainUIView) baseView; }
        }


        #region UI初始化,打开,关闭,销毁

        //第一次创建时候触发,只执行一次, 在Open前执行
        public override void OnInit()
        {
            base.OnInit();

            UIConfigData.forceClearNavigation = false; //以后需要导航的时候使用 清除导航信息
            UIConfigData.navigationMode = UIWindowNavigationMode.IgnoreNavigation; //以后需要导航的时候使用 是否记录导航信息
            UIConfigData.PreWindowId = WindowID.WindowID_Invaild; //以后需要导航的时候使用 默认的前置UI

            UIConfigData.showMode = UIWindowShowMode.HideOtherWindow; //默认打开ui前,会关闭其他显示的UI,
            UIConfigData.colliderMode = UIWindowColliderMode.Normal; //默认添加一个背景遮罩
            UIConfigData.windowType = UIWindowType.Normal; //默认,切换场景销毁;如果是DontDestory类型, 选择DontDestory,切换场景不销毁UI
            UIConfigData.depth = 0; //默认深度为0

            AddUIEvent();
        }

        protected override void OnUIOpened()
        {
            base.OnUIOpened();

            //todo 具体的UI打开处理 在播放UI进场动画之前
            EventManager.RefreshTrackUI();
        }

        protected override void OnUIHided()
        {
            base.OnUIHided();

            //todo 具体的UI关闭处理/退场动画结束回调
        }

        protected override void BeforeDestroyWindow()
        {
            base.BeforeDestroyWindow();
            RemoveUIEvent();
        }

        public override void DestroyWindow()
        {
            base.DestroyWindow();
        }

        #endregion

        #region 如果有入场和退场动画在这里添加

        public override void EnteringAnimation(Action onComplete)
        {
            //todo 具体的Dotween动画, 必须在结束回调onComplete, 否则OnEnteringAnimationEnd函数不会被调用
            if (onComplete != null)
            {
                onComplete();
            }
        }

        public override void ExitingAnimation(Action onComplete)
        {
            //todo 具体的Dotween动画, 必须在结束回调onComplete, 否则OnUIHided()函数不会被调用
            if (onComplete != null)
            {
                onComplete();
            }
        }

        #endregion


        public override void UpdateUI()
        {
            base.UpdateUI();
        }

        #region 外部事件处理

        public override IEnumerable<string> ListNotificationInterests
        {
            get { return mCmdList; }
        }

        List<string> mCmdList
        {
            get
            {
                return new List<string>()
                {
                };
            }
        }

        public override void HandleNotification(INotification notification)
        {
            switch (notification.Name)
            {
            }
        }

        #endregion


        #region UGUI事件监听处理

        void AddUIEvent()
        {
            View.UI_Button_Track.onClick.AddListener(OnUI_Button_TrackClick);
            View.UI_Button_Follow.onClick.AddListener(OnUI_Button_FollowClick);
            View.UI_Button_Main.onClick.AddListener(OnUI_Button_MainClick);
            View.UI_Button_ShowPoint.onClick.AddListener(OnUI_Button_ShowPointClick);
            View.UI_Button_Statistics.onClick.AddListener(OnUI_Button_StatisticsClick);
            View.UI_Button_MutiDisplay.onClick.AddListener(OnUI_Button_MutiDisplayClick);
            View.UI_Button_Pre.onClick.AddListener(OnUI_Button_PreClick);
            View.UI_Button_Next.onClick.AddListener(OnUI_Button_NextClick);
            View.UI_Button_PlayTrack.onClick.AddListener(OnUI_Button_PlayTrackClick);
            View.UI_Button_PlayeTrackSeq.onClick.AddListener(OnUI_Button_PlayeTrackSeqClick);
            View.UI_Button_ReconnectDB.onClick.AddListener(OnUI_Button_ReconnectDBClick);
            View.UI_Button1.onClick.AddListener(OnUI_Button1Click);
            View.UI_Button2.onClick.AddListener(OnUI_Button2Click);
            View.UI_Button3.onClick.AddListener(OnUI_Button3Click);
            View.UI_Button4.onClick.AddListener(OnUI_Button4Click);
            View.UI_Button5.onClick.AddListener(OnUI_Button5Click);
            View.UI_Button6.onClick.AddListener(OnUI_Button6Click);
            View.UI_Button7.onClick.AddListener(OnUI_Button7Click);
            View.UI_Button8.onClick.AddListener(OnUI_Button8Click);
            View.UI_Button9.onClick.AddListener(OnUI_Button9Click);
            View.UI_Button_About.onClick.AddListener(OnUI_Button_AboutClick);
            View.UI_Button_Exit.onClick.AddListener(OnUI_Button_ExitClick);
            View.UI_Button_Set.onClick.AddListener(OnUI_Button_SetClick);
            View.UI_TableColor.onClick.AddListener(OnUI_TableColorClick);
            View.UI_BallColor.onClick.AddListener(OnUI_BallColorClick);
            View.UI_FloorColor.onClick.AddListener(OnUI_FloorColorClick);
            View.UI_Billboard.onClick.AddListener(OnUI_BillboardClick);
            View.UI_StadiumAd.onClick.AddListener(OnUI_StadiumAdClick);
            View.UI_PlaySpeed.onClick.AddListener(OnUI_PlaySpeedClick);
            View.UI_AdjustTable.onClick.AddListener(OnUI_AdjustTableClick);
            View.UI_SetCameraConfig.onClick.AddListener(OnUI_SetCameraConfigClick);
            View.UI_SaveCameraCofig.onClick.AddListener(OnUI_SaveCameraCofigClick);
            View.UI_CancelCameraCofig.onClick.AddListener(OnUI_CancelCameraCofigClick);
            View.UI_Button_FreeCamera.onClick.AddListener(OnUI_Button_FreeCameraClick);
            View.UI_PlaySpeedSlider.onValueChanged.AddListener(OnSliderValueChanged); 
            Facade.RegisterMediator(this);

            EventManager.DBStateChange += OnDataBaseStateChanged;
            EventManager.ShowTrackInfo += OnShowTrackInfo;
            View.UI_IsShowPoint.onValueChanged.AddListener(OnShowPointValueChanged);
            EventManager.ShowTouchPoint += OnShowTouchPoint;
            EventManager.RefreshTrackUI += RefreshCurTrack;
        }

        void RemoveUIEvent()
        {
            //注意消息的监听的删除, 检测View是否为空
            if (View != null) View.UI_Button_Track.onClick.RemoveListener(OnUI_Button_TrackClick);
            if (View != null) View.UI_Button_Follow.onClick.RemoveListener(OnUI_Button_FollowClick);
            if (View != null) View.UI_Button_Main.onClick.RemoveListener(OnUI_Button_MainClick);
            if (View != null) View.UI_Button_ShowPoint.onClick.RemoveListener(OnUI_Button_ShowPointClick);
            if (View != null) View.UI_Button_Statistics.onClick.RemoveListener(OnUI_Button_StatisticsClick);
            if (View != null) View.UI_Button_MutiDisplay.onClick.RemoveListener(OnUI_Button_MutiDisplayClick);
            if (View != null) View.UI_Button_Pre.onClick.RemoveListener(OnUI_Button_PreClick);
            if (View != null) View.UI_Button_Next.onClick.RemoveListener(OnUI_Button_NextClick);
            if (View != null) View.UI_Button_PlayTrack.onClick.RemoveListener(OnUI_Button_PlayTrackClick);
            if (View != null) View.UI_Button_PlayeTrackSeq.onClick.RemoveListener(OnUI_Button_PlayeTrackSeqClick);
            if (View != null) View.UI_Button_ReconnectDB.onClick.RemoveListener(OnUI_Button_ReconnectDBClick);
            if (View != null) View.UI_Button1.onClick.RemoveListener(OnUI_Button1Click);
            if (View != null) View.UI_Button2.onClick.RemoveListener(OnUI_Button2Click);
            if (View != null) View.UI_Button3.onClick.RemoveListener(OnUI_Button3Click);
            if (View != null) View.UI_Button4.onClick.RemoveListener(OnUI_Button4Click);
            if (View != null) View.UI_Button5.onClick.RemoveListener(OnUI_Button5Click);
            if (View != null) View.UI_Button6.onClick.RemoveListener(OnUI_Button6Click);
            if (View != null) View.UI_Button7.onClick.RemoveListener(OnUI_Button7Click);
            if (View != null) View.UI_Button8.onClick.RemoveListener(OnUI_Button8Click);
            if (View != null) View.UI_Button9.onClick.RemoveListener(OnUI_Button9Click);
            if (View != null) View.UI_Button_About.onClick.RemoveListener(OnUI_Button_AboutClick);
            if (View != null) View.UI_Button_Exit.onClick.RemoveListener(OnUI_Button_ExitClick);
            if (View != null) View.UI_Button_Set.onClick.RemoveListener(OnUI_Button_SetClick);
            if (View != null) View.UI_TableColor.onClick.RemoveListener(OnUI_TableColorClick);
            if (View != null) View.UI_BallColor.onClick.RemoveListener(OnUI_BallColorClick);
            if (View != null) View.UI_FloorColor.onClick.RemoveListener(OnUI_FloorColorClick);
            if (View != null) View.UI_Billboard.onClick.RemoveListener(OnUI_BillboardClick);
            if (View != null) View.UI_StadiumAd.onClick.RemoveListener(OnUI_StadiumAdClick);
            if (View != null) View.UI_PlaySpeed.onClick.RemoveListener(OnUI_PlaySpeedClick);
            if (View != null) View.UI_AdjustTable.onClick.RemoveListener(OnUI_AdjustTableClick);
            if (View != null) View.UI_SetCameraConfig.onClick.RemoveListener(OnUI_SetCameraConfigClick);
            if (View != null) View.UI_SaveCameraCofig.onClick.RemoveListener(OnUI_SaveCameraCofigClick);
            if (View != null) View.UI_CancelCameraCofig.onClick.RemoveListener(OnUI_CancelCameraCofigClick);
            if (View != null) View.UI_Button_FreeCamera.onClick.RemoveListener(OnUI_Button_FreeCameraClick);

            Facade.RemoveMediator(MediatorName);

            EventManager.DBStateChange -= OnDataBaseStateChanged;
            EventManager.ShowTrackInfo -= OnShowTrackInfo;
            if (View != null) View.UI_IsShowPoint.onValueChanged.RemoveListener(OnShowPointValueChanged);
            EventManager.ShowTouchPoint -= OnShowTouchPoint;
            EventManager.RefreshTrackUI -= RefreshCurTrack;
            View.UI_PlaySpeedSlider.onValueChanged.RemoveListener(OnSliderValueChanged);
        }



        private void OnShowPointValueChanged(bool arg0)
        {
            OnUIClick();
            if (arg0)
            {
                OnUI_Button_ShowPointClick();
            }
            else
            {
                FunctionManager.Instance.HideTouchTablePoints();
            }
        }


        void OnUI_Button_TrackClick()
        {
            OnUIClick();
            OnUI_Button_PlayTrackClick();
        }

        void OnUI_Button_FollowClick()
        {
            OnUIClick();

            string trackID = View.UI_TrackID.text;
            if (string.IsNullOrEmpty(trackID))
            {
                PlayerManager.Instance.PlayHawkEyeChallenge(DataManager.Instance.CurTrackInfo.TrackIndex);
            }
            else
            {
                int trackIndex = 0;
                if (int.TryParse(trackID.Trim(), out trackIndex))
                {
                    PlayerManager.Instance.PlayHawkEyeChallenge(trackIndex);
                }
            }
        }

        void OnUI_Button_MainClick()
        {
            OnUIClick();
            CameraManager.Instance.SelectedCamera(CameraID.Default);
        }

        void OnUI_Button_ShowPointClick()
        {
            OnUIClick();
            FunctionManager.Instance.ShowTouchTablePoints();
        }

        void OnUI_Button_StatisticsClick()
        {
            OnUIClick();
            FunctionManager.Instance.ShowTouchTablePoints();
        }

        void OnUI_Button_MutiDisplayClick()
        {
            OnUIClick();
            FunctionManager.Instance.MultiDisplaye();
        }

        void OnUI_Button_PreClick()
        {
            OnUIClick();

            View.UI_TrackID.text= "";
            TrackInfo info = DataManager.Instance.GetTrackInfoFromDB(DataManager.Instance.CurTrackInfo.TrackIndex - 1);
            if (info == null)
            {
                return;
            }
            else
            {
                //if (info.RoundID != DataManager.Instance.CurTrackInfo.RoundID)
                {
                    //DataManager.Instance.CurTrackInfo.RoundID = info.RoundID;
                    //DataManager.Instance.CurTrackInfo.TrackID = info.ID; //"29a821c6-e3b5-409b-a5ad-2b9ebfb80334";//info.TrackInfos[info.TrackInfos.Count - 1].ID;
                    //DataManager.Instance.CurTrackInfo.TrackIndex = info.Index;
                    DataManager.Instance.SetCurTrackInfo(info.RoundID, info.Index);
                }
            }
            OnUI_Button_PlayTrackClick();
            //string curRoundID = DataManager.Instance.CurTrackInfo.RoundID;
            //string curTrackID = DataManager.Instance.CurTrackInfo.TrackID;

            //RoundInfo roundInfo = DataManager.Instance.GetRoundInfo(curRoundID);
            //for (int i = 0; i < roundInfo.TrackInfos.Count; i++)
            //{
            //    if (roundInfo.TrackInfos[i].ID.Equals(curTrackID))
            //    {
            //        int pre = i - 1;
            //        if (pre >= 0)
            //        {
            //            //DataManager.Instance.CurTrackInfo.TrackID = roundInfo.TrackInfos[pre].ID;
            //            //DataManager.Instance.CurTrackInfo.TrackIndex = roundInfo.TrackInfos[pre].Index;
            //            DataManager.Instance.SetCurTrackInfo(roundInfo.RoundID, roundInfo.TrackInfos[pre].Index);
            //            OnUI_Button_PlayTrackClick();
            //        }
            //        break;
            //    }
            //}
        }

        void OnUI_Button_NextClick()
        {
            OnUIClick();

            View.UI_TrackID.text = "";

            TrackInfo info = DataManager.Instance.GetTrackInfoFromDB(DataManager.Instance.CurTrackInfo.TrackIndex + 1);
            if (info == null)
            {
                return;
            }
            else
            {
                //if (info.RoundID != DataManager.Instance.CurTrackInfo.RoundID)
                {
                    //DataManager.Instance.CurTrackInfo.RoundID = info.RoundID;
                    //DataManager.Instance.CurTrackInfo.TrackID = info.ID; //"29a821c6-e3b5-409b-a5ad-2b9ebfb80334";//info.TrackInfos[info.TrackInfos.Count - 1].ID;
                    //DataManager.Instance.CurTrackInfo.TrackIndex = info.Index;
                    DataManager.Instance.SetCurTrackInfo(info.RoundID, info.Index);
                    //EventManager.RefreshTrackUI();
                }
            }

            OnUI_Button_PlayTrackClick();
            //string curRoundID = DataManager.Instance.CurTrackInfo.RoundID;
            //string curTrackID = DataManager.Instance.CurTrackInfo.TrackID;

            //RoundInfo roundInfo = DataManager.Instance.GetRoundInfo(curRoundID);
            //for (int i = 0; i < roundInfo.TrackInfos.Count; i++)
            //{
            //    if (roundInfo.TrackInfos[i].ID.Equals(curTrackID))
            //    {
            //        int next = i + 1;
            //        if (next < roundInfo.TrackInfos.Count)
            //        {
            //            DataManager.Instance.CurTrackInfo.TrackID = roundInfo.TrackInfos[next].ID;
            //            DataManager.Instance.CurTrackInfo.TrackIndex = roundInfo.TrackInfos[next].Index;
            //            OnUI_Button_PlayTrackClick();
            //        }
            //        break;
            //    }
            //}
        }

        void OnUI_Button_PlayTrackClick()
        {
            OnUIClick();
            EventManager.RefreshTrackUI();

            string trackID = View.UI_TrackID.text;
            if (string.IsNullOrEmpty(trackID))
            {
                PlayerManager.Instance.PlayTrack(DataManager.Instance.CurTrackInfo.TrackIndex);
            }
            else
            {
                int trackIndex = 0;
                if (int.TryParse(trackID.Trim(), out trackIndex))
                {
                    PlayerManager.Instance.PlayTrack(trackIndex);
                }
            }

         
        }

        void OnUI_Button_PlayeTrackSeqClick()
        {
            OnUIClick();
            string curRoundID = DataManager.Instance.CurTrackInfo.RoundID;
            string curTrackID = DataManager.Instance.CurTrackInfo.TrackID;
            int curTrackIndex = DataManager.Instance.CurTrackInfo.TrackIndex;

            int trackOrderInRound = -1;
            RoundInfo roundInfo = DataManager.Instance.GetRoundInfo(curRoundID);
            for (int i = 0; i < roundInfo.TrackInfos.Count; i++)
            {
                //if (roundInfo.TrackInfos[i].ID.Equals(curTrackID))
                if (roundInfo.TrackInfos[i].Index == curTrackIndex)
                {
                    trackOrderInRound = i;
                    break;
                }
            }
            if (trackOrderInRound >= 0)
            {
                PlayerManager.Instance.PlayeTrack(curRoundID, trackOrderInRound + 1,
                    roundInfo.TrackInfos.Count - trackOrderInRound);
            }
        }

        void OnUI_Button_ReconnectDBClick()
        {
            OnUIClick();
            OnDataBaseStateChanged("Success");
            DBAccessor.Instance.GetLatestRound();
        }

        void OnUI_Button1Click()
        {
            OnUIClick();
            CameraManager.Instance.SelectedCamera(CameraID.Camera1);
        }

        void OnUI_Button2Click()
        {
            OnUIClick();
            CameraManager.Instance.SelectedCamera(CameraID.Camera2);
        }

        void OnUI_Button3Click()
        {
            OnUIClick();
            CameraManager.Instance.SelectedCamera(CameraID.Camera3);
        }

        void OnUI_Button4Click()
        {
            OnUIClick();
            CameraManager.Instance.SelectedCamera(CameraID.Camera4);
        }

        void OnUI_Button5Click()
        {
            OnUIClick();
            CameraManager.Instance.SelectedCamera(CameraID.Camera5);
        }

        void OnUI_Button6Click()
        {
            OnUIClick();
            CameraManager.Instance.SelectedCamera(CameraID.Camera6);
        }

        void OnUI_Button7Click()
        {
            OnUIClick();
            CameraManager.Instance.SelectedCamera(CameraID.Camera7);
        }

        void OnUI_Button8Click()
        {
            OnUIClick();
            CameraManager.Instance.SelectedCamera(CameraID.Camera8);
        }

        void OnUI_Button9Click()
        {
            OnUIClick();
            CameraManager.Instance.SelectedCamera(CameraID.Camera9);
        }

        void OnUI_Button_AboutClick()
        {
            OnUIClick();
        }

        void OnUI_Button_ExitClick()
        {
            Application.Quit();
        }

        void OnUI_Button_SetClick()
        {
            OnUIClick();
        }

        void OnUI_TableColorClick()
        {
            OnUIClick();
            if (FunctionManager.Instance.CurState == FunctionState.ChangeColor)
            {
                FunctionManager.Instance.EndChangeColor();
            }
            else
            {
                GameObject table = ResourceManager.Instance.GetTable();
                Renderer render = table.GetComponentInChildren<MeshRenderer>();
                Material[] mats = render.sharedMaterials;
                FunctionManager.Instance.StartChangeColorFor(mats[7]);
            }
        }

        void OnUI_BallColorClick()
        {
            OnUIClick();
            if (FunctionManager.Instance.CurState == FunctionState.ChangeColor)
            {
                FunctionManager.Instance.EndChangeColor();
            }
            else
            {
                GameObject ball = ResourceManager.Instance.GetBall();
                Renderer render = ball.GetComponent<MeshRenderer>();
                Material[] mats = render.sharedMaterials;
                FunctionManager.Instance.StartChangeColorFor(mats[0]);
            }
        }

        void OnUI_FloorColorClick()
        {
            OnUIClick();
            if (FunctionManager.Instance.CurState == FunctionState.ChangeColor)
            {
                FunctionManager.Instance.EndChangeColor();
            }
            else
            {
                GameObject floor = ResourceManager.Instance.GetFloor();
                Renderer render = floor.GetComponent<MeshRenderer>();
                Material[] mats = render.sharedMaterials;
                FunctionManager.Instance.StartChangeColorFor(mats[0]);
            }
        }

        void OnUI_BillboardClick()
        {
            OnUIClick();
            // FunctionManager.Instance.StartChooseBillbard();

            OpenFileManager.Instance.ChooseFileFor(ResourceManager.Instance.BillboardObj.transform
                .GetComponent<Renderer>());
        }

        void OnUI_StadiumAdClick()
        {
            OnUIClick();
            //FunctionManager.Instance.StartChooseBillbard();
            OpenFileManager.Instance.ChooseFileFor(ResourceManager.Instance.BillboardObj.transform
                .GetComponent<Renderer>());
        }

        private bool increase = true;

        void OnUI_PlaySpeedClick()
        {
            OnUIClick();
            // if (Display.displays.Length > 1)
            // Display.displays[1].Activate();


            if (PlayerManager.Instance.m_SpeedScale > 3f)
            {
                increase = false;
            }
            else if (PlayerManager.Instance.m_SpeedScale < 0.01f)
            {
                increase = true;
            }

            PlayerManager.Instance.m_SpeedScale += increase ? 0.5f : -0.5f;
        }

        void OnUI_AdjustTableClick()
        {
            OnUIClick();
            if (FunctionManager.Instance.CurState == FunctionState.ConfigTable)
            {
                FunctionManager.Instance.EndAdjustTable();
            }
            else
            {
                FunctionManager.Instance.AdjustTable();
            }
        }

        void OnUI_SetCameraConfigClick()
        {
            OnUIClick();
            FunctionManager.Instance.StartCameraConfig(CameraManager.Instance.CurSelectedCamera);
        }

        void OnUI_SaveCameraCofigClick()
        {
            OnUIClick();
            FunctionManager.Instance.SaveCameraConfig();
        }

        void OnUI_CancelCameraCofigClick()
        {
            OnUIClick();
            FunctionManager.Instance.CancelCameraConfig();
        }


        void OnUI_Button_FreeCameraClick()
        {
            OnUIClick();
            if (FunctionManager.Instance.CurState == FunctionState.FreeCamera)
            {
                FunctionManager.Instance.ExitFreeCameraMode();
            }
            else
            {
                FunctionManager.Instance.EnterFreeCameraMode();
            }
        }

        #endregion

        void OnUIClick()
        {
            FunctionManager.Instance.HideTouchTablePoints();
        }

        private void OnDataBaseStateChanged(string info)
        {
            View.UI_DatabaseState.text = info;
        }

        void RefreshCurTrack()
        {
            View.UI_CurTrackIDText.text = "RoundID: " + DataManager.Instance.CurTrackInfo.RoundID.ToString() + " TrackIndex: " + DataManager.Instance.CurTrackInfo.TrackIndex.ToString();
        }


        private void OnShowTrackInfo(TrackInfo info)
        {
            Text UISpeedText = GameObject.Find("UISpeedText").GetComponent<Text>();
            Text UIRotateSpeedText = GameObject.Find("UIRotateSpeedText").GetComponent<Text>();
            Text UIHeightText = GameObject.Find("UIHeightText").GetComponent<Text>();
            UISpeedText.text = "球速: "  + (View.UI_IsShowMoveSpeed.isOn ? info.Speed.ToString() : "");
            UIRotateSpeedText.text = "转速: " + (View.UI_IsShowRoutateSpeed.isOn ? info.Rotate.ToString() : "");
            UIHeightText.text = "过网:" + (View.UI_IsShowHeight.isOn ? info.OverNetHeight.ToString() : "");
        }

        private void OnShowTouchPoint(Vector3 point)
        {
            OnUIClick();
            FunctionManager.Instance.ShowTouchPoint(point);
        }

        private void OnSliderValueChanged(float value)
        {
            //OnUIClick();

            float scale = (value - 0.5f)/0.5f; //-1 ~ 1

            if (scale > 0f)
            {
                PlayerManager.Instance.m_SpeedScale = 1 +  scale * 3f;  // 1+ ( 0 - 3)
            }
            else
            {
                PlayerManager.Instance.m_SpeedScale = 1 + scale * 0.99f;  //1 - (0 - 0.09)
            }
        }

    }
}

