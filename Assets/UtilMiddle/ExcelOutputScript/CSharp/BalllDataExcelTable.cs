using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System;
public class BalllDataExcelTable : ITableLoad
{
	public uint Id = 0;	//角色ID
	public string Des = string.Empty;	//描述
	public string Name = string.Empty;	//名称
	public string Icon = string.Empty;	//图标名称
	public string AssetName = string.Empty;	//资源名称
	public string SkillsID = string.Empty;	//自带技能
	public uint FlashlightID = 0;	//自带手电ID
	public float NormalSpeed = 0;	//正常移动速度
	public float FlashlightSpeed = 0;	//打手电移动速度比率
	public float ScareTime = 0;	//惊吓时间
	public string ShowTriggerName = string.Empty;	//展示动画Trigger
	public string SpecialTriggerName = string.Empty;	//特殊动画Trigger

	public uint Load(ByteBuffer byteBuffer)
	{
		Id = byteBuffer.ReadUInt();
		Des = byteBuffer.ReadString(256);
		Name = byteBuffer.ReadString(32);
		Icon = byteBuffer.ReadString(32);
		AssetName = byteBuffer.ReadString(64);
		SkillsID = byteBuffer.ReadString(64);
		FlashlightID = byteBuffer.ReadUInt();
		NormalSpeed = byteBuffer.ReadFloat();
		FlashlightSpeed = byteBuffer.ReadFloat();
		ScareTime = byteBuffer.ReadFloat();
		ShowTriggerName = byteBuffer.ReadString(128);
		SpecialTriggerName = byteBuffer.ReadString(128);

		return Id;
	}
}