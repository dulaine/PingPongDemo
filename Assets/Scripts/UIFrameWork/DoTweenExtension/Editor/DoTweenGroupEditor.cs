using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(DoTweenGroup),false)]
public class DoTweenGroupEditor : Editor {

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        GUILayout.Space(6f);
        EditorGUIUtility.labelWidth = 120f;

        GUI.changed = false;

        DoTweenGroup tw = target as DoTweenGroup;
        //在添加新的数组时候, 会清除原来的内容, out
        //SerializedProperty sp = serializedObject.FindProperty("AnimationArray");
        //if (sp.isArray)
        //{
        //    DrawArray(serializedObject, "AnimationArray", "AnimationArray");
        //}
        base.DrawDefaultInspector();

        GUILayout.BeginHorizontal();
        float del = EditorGUILayout.FloatField("Start Delay", tw.delay, GUILayout.Width(170f));
        GUILayout.Label("seconds");
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();
        SerializedProperty OncompleteProperty = serializedObject.FindProperty("OnComplete");
        EditorGUILayout.PropertyField(OncompleteProperty, new GUIContent("OnComplete"));
        GUILayout.EndHorizontal();

        if (GUI.changed)
        {
            tw.delay = del;
            tw.OnComplete = OncompleteProperty.objectReferenceValue as BaseDoTween;
            UnityEditor.EditorUtility.SetDirty(tw);
        }

        if (GUILayout.Button("Play"))
        {
            tw.Play();
        }
        if (GUILayout.Button("Stop"))
        {
            tw.Stop();
        }
    }

    public void DrawArray(SerializedObject obj, string property, string title)
    {
        SerializedProperty sp = obj.FindProperty(property + ".Array.size");

        int size = sp.intValue;
        int newSize = EditorGUILayout.IntField("Size", size);
        if (newSize != size)
        {
            //添加新的数组内容
            obj.FindProperty(property + ".Array.size").intValue = newSize;
            DoTweenGroup player = target as DoTweenGroup;
            player.AnimationArray = new DoTweenAlpha[newSize];
        }

        EditorGUI.indentLevel = 1;

        for (int i = 0; i < newSize; i++)
        {
            //随时保存变更的数组内容
            SerializedProperty p = obj.FindProperty(string.Format("{0}.Array.data[{1}]", property, i));
            EditorGUILayout.PropertyField(p);
            BaseDoTween anim = p.objectReferenceValue as BaseDoTween;
            DoTweenGroup player = target as DoTweenGroup;
            player.AnimationArray[i] = anim;
        }

        EditorGUI.indentLevel = 0;
    }
}
