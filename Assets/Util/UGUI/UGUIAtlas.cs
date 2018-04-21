using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// UGUI图集上挂的脚本
/// 用来通过名字获取sprite
/// </summary>
[System.Serializable]
public class UGUIAtlas : MonoBehaviour
{
    [SerializeField]
    public Texture2D MainTexture;
    [SerializeField]
    public List<Sprite> SpriteList = new List<Sprite>();

    /// <summary>
    /// 通过名称获取图片
    /// </summary>
    public Sprite GetSprite(string spritename)
    {
        return SpriteList.Find((Sprite s) => { return s.name == spritename; });
    }
}
