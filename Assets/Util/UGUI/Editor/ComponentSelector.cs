using UnityEditor;
using UnityEngine;
using System.Collections.Generic;

public class ComponentSelector : ScriptableWizard
{
    public delegate void OnSelectionCallback(Object obj);

    System.Type mType;
    string mTitle;
    OnSelectionCallback mCallback;
    Object[] mObjects;
    bool mSearched = false;
    Vector2 mScroll = Vector2.zero;
    string[] mExtensions = null;

    static string GetName(System.Type t)
    {
        string s = t.ToString();
        s = s.Replace("UnityEngine.", "");
        if (s.StartsWith("UI")) s = s.Substring(2);
        return s;
    }

    static public void Show<T>(OnSelectionCallback cb) where T : Object { Show<T>(cb, new string[] { ".prefab" }); }

    static public void Show<T>(OnSelectionCallback cb, string[] extensions) where T : Object
    {
        System.Type type = typeof(T);
        string title = (type == typeof(UGUIAtlas) ? "Select an " : "Select a ") + GetName(type);
        ComponentSelector comp = ScriptableWizard.DisplayWizard<ComponentSelector>(title);
        comp.mTitle = title;
        comp.mType = type;
        comp.mCallback = cb;
        comp.mExtensions = extensions;
        comp.mObjects = Resources.FindObjectsOfTypeAll(typeof(T));

        if (comp.mObjects == null || comp.mObjects.Length == 0)
        {
            comp.Search();
        }
        else
        {
            if (typeof(T) == typeof(Font))
            {
                for (int i = 0; i < comp.mObjects.Length; ++i)
                {
                    Object obj = comp.mObjects[i];
                    if (obj.name == "Arial") continue;
                    string path = AssetDatabase.GetAssetPath(obj);
                    if (string.IsNullOrEmpty(path)) comp.mObjects[i] = null;
                }
            }

            System.Array.Sort(comp.mObjects,
                delegate (Object a, Object b)
                {
                    if (a == null) return (b == null) ? 0 : 1;
                    if (b == null) return -1;
                    return a.name.CompareTo(b.name);
                });
        }
    }

    void Search()
    {
        mSearched = true;

        if (mExtensions != null)
        {
            string[] paths = AssetDatabase.GetAllAssetPaths();
            bool isComponent = mType.IsSubclassOf(typeof(Component));
            List<Object> list = new List<Object>();
            for (int i = 0; i < mObjects.Length; ++i)
                if (mObjects[i] != null)
                    list.Add(mObjects[i]);
            for (int i = 0; i < paths.Length; ++i)
            {
                string path = paths[i];
                bool valid = false;
                for (int b = 0; b < mExtensions.Length; ++b)
                {
                    if (path.EndsWith(mExtensions[b], System.StringComparison.OrdinalIgnoreCase))
                    {
                        valid = true;
                        break;
                    }
                }
                if (!valid) continue;
                EditorUtility.DisplayProgressBar("搜索中", "搜索所有资源中，请等待...", (float)i / paths.Length);
                Object obj = AssetDatabase.LoadMainAssetAtPath(path);
                if (obj == null || list.Contains(obj)) continue;
                if (!isComponent)
                {
                    System.Type t = obj.GetType();
                    if (t == mType || t.IsSubclassOf(mType) && !list.Contains(obj))
                        list.Add(obj);
                }
                else if (PrefabUtility.GetPrefabType(obj) == PrefabType.Prefab)
                {
                    Object t = (obj as GameObject).GetComponent(mType);
                    if (t != null && !list.Contains(t)) list.Add(t);
                }
            }
            list.Sort(delegate (Object a, Object b) { return a.name.CompareTo(b.name); });
            mObjects = list.ToArray();
        }
        EditorUtility.ClearProgressBar();
    }

    void OnGUI()
    {
        EditorGUIUtility.labelWidth = 80f;
        GUILayout.Label(mTitle, "LODLevelNotifyText");
        GUILayout.Space(6f);

        if (mObjects == null || mObjects.Length == 0)
        {
            Debug.LogError("没有此类型的物体");
        }
        else
        {
            Object sel = null;
            mScroll = GUILayout.BeginScrollView(mScroll);

            foreach (Object o in mObjects)
                if (DrawObject(o))
                    sel = o;

            GUILayout.EndScrollView();

            if (sel != null)
            {
                mCallback(sel);
                Close();
            }
        }
        if (!mSearched)
        {
            GUILayout.Space(6f);
            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            bool search = GUILayout.Button("再检测一遍", GUILayout.Width(120f));
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();
            if (search) Search();
        }
    }

    bool DrawObject(Object obj)
    {
        if (obj == null) return false;
        bool retVal = false;
        Component comp = obj as Component;

        GUILayout.BeginHorizontal();
        {
            string path = AssetDatabase.GetAssetPath(obj);

            if (string.IsNullOrEmpty(path))
            {
                path = "[Embedded]";
                GUI.contentColor = new Color(0.7f, 0.7f, 0.7f);
            }
            else if (comp != null && EditorUtility.IsPersistent(comp.gameObject))
                GUI.contentColor = new Color(0.6f, 0.8f, 1f);

            retVal |= GUILayout.Button(obj.name, GUILayout.Width(160f), GUILayout.Height(20f));
            retVal |= GUILayout.Button(path.Replace("Assets/", ""), GUILayout.Height(20f));
            GUI.contentColor = Color.white;

            retVal |= GUILayout.Button("Select", "ButtonLeft", GUILayout.Width(60f), GUILayout.Height(16f));
        }
        GUILayout.EndHorizontal();
        return retVal;
    }
}
