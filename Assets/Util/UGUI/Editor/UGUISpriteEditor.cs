using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.UI;


[CustomEditor(typeof(UGUISprite))]
public class UGUISpriteEditor : ImageEditor
{
    public override void OnInspectorGUI()
    {
        UGUISprite _sprite = target as UGUISprite;
        //选择图集
        if (GUILayout.Button("选择图集", "DropDown", GUILayout.Width(76f)))
        {
            ComponentSelector.Show<UGUIAtlas>((_obj) =>
            {
                _sprite.Atlas = _obj as UGUIAtlas;
            });
        }
        _sprite.Atlas = EditorGUILayout.ObjectField("图集", _sprite.Atlas, typeof(UGUIAtlas), true) as UGUIAtlas;
        if (_sprite.Atlas != null)      //有图集的情况下，根据name获取图片
        {
            ////选择图集中的图片
            //if (GUILayout.Button("选择图集", "DropDown", GUILayout.Width(76f)))
            //{
            //    SpriteSelector.Show(_sprite.Atlas, (_obj) =>
            //    {
            //        _sprite.sprite = _obj;
            //    });
            //    Debug.Log("选中了一张图片");
            //}

            if (_sprite.sprite != null)
            {
                if (_sprite.Name != _sprite.sprite.name)
                {
                    _sprite.Name = _sprite.sprite.name;
                }
            }
            else
            {
                if (_sprite.Name != "")
                {
                    _sprite.InitSprite();
                }
            }
        }
        EditorGUILayout.LabelField("图片名称：" + _sprite.Name);

        base.OnInspectorGUI();
    }
}
