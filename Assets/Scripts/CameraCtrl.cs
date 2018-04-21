using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class CameraCtrl : MonoBehaviour
{

    public List<Vector3> Pos = new List<Vector3>();
    public List<Vector3> Rotations = new List<Vector3>();

    void Start()
    {
        //InitCamera9Conf();
        GameCSData.Instance.ReadCamera9Conf();
    }

    bool isRightBtnDown = false;
    Vector3 mousePos = Vector3.zero;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.W))
        {
            Front();
        }
        if (Input.GetKeyDown(KeyCode.S))
        {
            Back();
        }
        if (Input.GetKeyDown(KeyCode.A))
        {
            Left();
        }
        if (Input.GetKeyDown(KeyCode.D))
        {
            Right();
        }
        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            Up();
        }
        if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            Down();
        }
        //右键按下调整摄像机旋转
        if (Input.GetMouseButtonDown(1))
        {
            //记录按下时的位置
            if (!isRightBtnDown)
            {
                isRightBtnDown = true;
                mousePos = Input.mousePosition;
            }
        }
        else if (Input.GetMouseButtonUp(1))
        {
            isRightBtnDown = false;
        }
    }
    #region 配置数据
    void InitCamera9Conf()
    {
        GameCSData.AllData temp = new GameCSData.AllData();
        for (int i = 0; i < Pos.Count; i++)
        {
            GameCSData.CameraData data = new GameCSData.CameraData();
            data.Position = Pos[i];
            data.Rotations = Rotations[i];
            temp.datas.Add(data);
        }
        GameCSData.Instance.WriteCamera9Conf(temp);
    }
    #endregion

    #region 操作摄像机
    void Front()
    {

    }
    void Back()
    {

    }
    void Left()
    {

    }
    void Right()
    {

    }
    void Up()
    {

    }
    void Down()
    {

    }
    void RotateHorizontal(float _dis)
    {

    }
    void RotateVertical(float _dis)
    {

    }
    #endregion
}
