using UnityEditor;
using UnityEngine;
using System.Collections.Generic;
using System;
using UnityEngine.UI;

/// <summary>
/// 选择图集中的某张图片
/// </summary>
public class SpriteSelector : ScriptableWizard
{
    const int cMaxWidthHeight = 80;
    const int cSpace = 10;

    List<Sprite> mSprites = new List<Sprite>();
    Action<Sprite> mCb = null;

    static public void Show(UGUIAtlas _atlas, Action<Sprite> _cb)
    {
        SpriteSelector comp = ScriptableWizard.DisplayWizard<SpriteSelector>("选择图片");
        comp.mCb = _cb;
        comp.mSprites = _atlas.SpriteList;
    }

    void OnGUI()
    {
        EditorGUIUtility.labelWidth = 80f;
        GUILayout.Label("选择图片", "LODLevelNotifyText");
        GUILayout.Space(6f);


        float size = cMaxWidthHeight;
        float padded = size + cSpace;
        int columns = Mathf.FloorToInt(Screen.width / padded);
        if (columns < 1) columns = 1;
        var nowNum = 0;

        //mScroll = GUILayout.BeginScrollView(mScroll);
        foreach (var child in mSprites)
        {
            //if (nowNum == 0)
            //{
            //    GUILayout.BeginHorizontal();
            //}
            nowNum++;
            DrawSprite(child);
            if (nowNum >1)
            {
                return;
            }
            if (nowNum == columns)
            {
                nowNum = 0;
                //GUILayout.EndHorizontal();
            }
        }
        //GUILayout.EndScrollView();



        Sprite sel = null;


        if (sel != null)
        {
            mCb(sel);
            Close();
        }
    }

    void DrawSprite(Sprite _sprite)
    {
        //DrawTiledTexture(_sprite.rect, _sprite.texture);
        GUI.DrawTexture(new Rect(_sprite.rect.x, _sprite.rect.y, _sprite.texture.width, _sprite.texture.height), _sprite.texture);
        GUILayout.Space(cSpace);
    }
    void DrawTiledTexture(Rect rect, Texture tex)
    {
        Debug.Log(rect);
        GUI.BeginGroup(rect);
        {
            GUI.DrawTexture(new Rect(rect.x, rect.y, tex.width, tex.height), tex);

            //int width = Mathf.RoundToInt(rect.width);
            //int height = Mathf.RoundToInt(rect.height);

            //for (int y = 0; y < height; y += tex.height)
            //{
            //    for (int x = 0; x < width; x += tex.width)
            //    {
            //        GUI.DrawTexture(new Rect(x, y, tex.width, tex.height), tex);
            //    }
            //}
        }
        GUI.EndGroup();
    }
}
