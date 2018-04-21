using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System;
public class UIConfigTable : ITableLoad
{
    public uint Id = 0; //UI界面ID
    public string Name = string.Empty;  //名称
    public string AssetName = string.Empty; //AssetName
    public string Hierarchy = string.Empty;	//UI所属层
    public string Scenes = string.Empty;	//UI所属场景
    public bool IsAllScene = false;         //是否所有场景都可以打开
    public string[] SceneList;      //UI所属的场景
    public bool IsChangeSceneClose = false;     //若切场景不销毁，那是否关闭
    public string CloseUI = string.Empty;   //UI打开时要关闭的界面组
    public byte IsCloseUIAnim = 0;  //是否调用关闭界面的动画
    public float DepthSpeed = 0;    //此UI所占用深度
    public byte IsNeedBackground = 0;   //此UI是否需要遮罩
    public byte IsHaveOpenAnim = 0; //是否有打开动画
    public byte IsHaveCloseAnim = 0;    //是否有关闭动画
    public byte IsHide = 0; //关闭UI后是否隐藏
    public byte DestroyType = 0;    //UI销毁方式 0不销毁 1切场景销毁 2关闭销毁

    public uint Load(ByteBuffer byteBuffer)
    {
        Id = byteBuffer.ReadUInt();
        Name = byteBuffer.ReadString(32);
        AssetName = byteBuffer.ReadString(64);
        Hierarchy = byteBuffer.ReadString(32);
        Scenes = byteBuffer.ReadString(32);
        if (Scenes == "All")
        {
            IsAllScene = true;
        }
        else
        {
            IsAllScene = false;
            SceneList = Scenes.Split(',');
        }
        IsChangeSceneClose = byteBuffer.ReadByte() == 1 ? true : false;
        CloseUI = byteBuffer.ReadString(64);
        IsCloseUIAnim = byteBuffer.ReadByte();
        DepthSpeed = byteBuffer.ReadFloat();
        IsNeedBackground = byteBuffer.ReadByte();
        IsHaveOpenAnim = byteBuffer.ReadByte();
        IsHaveCloseAnim = byteBuffer.ReadByte();
        IsHide = byteBuffer.ReadByte();
        DestroyType = byteBuffer.ReadByte();
        return Id;
    }
}