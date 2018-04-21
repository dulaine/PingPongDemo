using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UGUISprite : Image
{
    [SerializeField]
    public UGUIAtlas Atlas;
    [SerializeField]
    string _Name = "";
    public string Name
    {
        get
        {
            return _Name;
        }
        set
        {
            if (_Name != value && Atlas != null)
            {
                var _sprite = Atlas.GetSprite(value);
                if (_sprite != null)
                {
                    _Name = value;
                    sprite = _sprite;
                }
            }
        }
    }

    protected override void OnEnable()
    {
        base.OnEnable();
        if (sprite != null)
        {
            _Name = sprite.name;
        }
    }

    /// <summary>
    /// 若有图集，有名字，没图片，就初始化一下图片
    /// </summary>
    public void InitSprite()
    {
        if (_Name != "" && sprite == null && Atlas != null)
        {
            sprite = Atlas.GetSprite(_Name);
        }
    }
}
