using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(DoTweenPosition))]
public class DoTweenPositionEditor : TweenEditor
{
    public override void OnInspectorGUI()
    {
        GUILayout.Space(6f);
        EditorGUIUtility.labelWidth = 120f;

        DoTweenPosition tw = target as DoTweenPosition;
        GUI.changed = false;

        Vector3 from = EditorGUILayout.Vector3Field("From", tw.From);
        Vector3 to = EditorGUILayout.Vector3Field("To", tw.To);

        if (GUI.changed)
        {
            tw.From = from;
            tw.To = to;
            UnityEditor.EditorUtility.SetDirty(tw);
        }

        DrawCommonProperties();
    }
}
