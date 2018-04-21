using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(DoTweenAlpha))]
public class DoTweenAlphaEditor: TweenEditor  {

    public override void OnInspectorGUI()
    {
        GUILayout.Space(6f);
        EditorGUIUtility.labelWidth = 120f;

        DoTweenAlpha tw = target as DoTweenAlpha;
        GUI.changed = false;

        float from = EditorGUILayout.Slider("From", tw.From, 0f, 1f);
        float to = EditorGUILayout.Slider("To", tw.To, 0f, 1f);

        if (GUI.changed)
        {
            tw.From = from;
            tw.To = to;
            UnityEditor.EditorUtility.SetDirty(tw);
        }

        DrawCommonProperties();
    }
}
