using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using LitJson;

public class TexturePackerEditor : MonoBehaviour
{
    public class ERect
    {
        public int x, y;
        public int w, h;
        public ERect(JsonData jsonData)
        {
            x = (int)jsonData["x"];
            y = (int)jsonData["y"];
            w = (int)jsonData["w"];
            h = (int)jsonData["h"];
        }
    }
    public class ESpriteData
    {
        public string m_strName = "";
        public ERect frame;
        public bool rotated = false;
        public bool trimmed = false;
        public ERect spriteSourceSize;
        public int sourceWidth;
        public int sourceHeight;
        public ESpriteData(JsonData jsonData, string _name)
        {
            m_strName = _name.Replace(".png", "");
            frame = new ERect(jsonData["frame"]);
            rotated = (bool)jsonData["rotated"];
            trimmed = (bool)jsonData["trimmed"];
            spriteSourceSize = new ERect(jsonData["spriteSourceSize"]);
            JsonData sourceSize = jsonData["sourceSize"];
            sourceWidth = (int)sourceSize["w"];
            sourceHeight = (int)sourceSize["h"];

        }
    }
    [MenuItem("Tools/切割当前选中图集(需同时选中图片和TPtext)", false, 501)]
    static public void SetAtlas()
    {
        //获取图片及json
        string _text = "";
        Texture2D _texture2d = null;
        TextureImporter _textureImporter = null;
        string _path = "";
        //赋值选中的物体
        var _objs = Selection.objects;
        for (int i = 0; i < _objs.Length; i++)
        {
            if (_objs[i].GetType() == typeof(TextAsset))
            {
                _text = (_objs[i] as TextAsset).text;
            }
            else if (_objs[i].GetType() == typeof(Texture2D))
            {
                _texture2d = (_objs[i] as Texture2D);
                _path = AssetDatabase.GetAssetPath(_texture2d);
                _textureImporter = AssetImporter.GetAtPath(_path) as TextureImporter;
            }
        }
        if (string.IsNullOrEmpty(_text) || _texture2d == null)
        {
            Debug.LogError("选择的图片或TPtext有问题");
            return;
        }
        //解析json
        JsonData frames = JsonMapper.ToObject(_text)["frames"];
        IDictionary dict = frames as IDictionary;
        List<ESpriteData> lstSprite = new List<ESpriteData>();
        foreach (string key in dict.Keys)
        {
            string fileName = key;
            ESpriteData sprite = new ESpriteData(frames[fileName], fileName);
            lstSprite.Add(sprite);
        }
        //拆分图片
        if (_textureImporter != null)
        {
            int width = _texture2d.width;
            int height = _texture2d.height;
            SpriteMetaData[] sprites = new SpriteMetaData[lstSprite.Count];
            for (int i = 0; i < lstSprite.Count; i++)
            {
                ESpriteData spriteData = lstSprite[i];
                ERect frame = spriteData.frame;
                sprites[i] = new SpriteMetaData();
                sprites[i].name = spriteData.m_strName;
                sprites[i].pivot = new Vector2(0.5f, 0.5f);
                sprites[i].rect = new Rect(frame.x, height - frame.y - frame.h, frame.w, frame.h);
            }
            _textureImporter.textureType = TextureImporterType.Sprite;
            _textureImporter.spriteImportMode = SpriteImportMode.Multiple;
            _textureImporter.spritesheet = sprites;
            //_textureImporter.spritePixelsToUnits = 100;
            _textureImporter.spritePixelsPerUnit = 100;
            _textureImporter.filterMode = FilterMode.Point;
            TextureImporterSettings _tipSeting = new TextureImporterSettings();
            _textureImporter.ReadTextureSettings(_tipSeting);
            _textureImporter.SetTextureSettings(_tipSeting);
            AssetDatabase.ImportAsset(_path);
        }
    }
}
