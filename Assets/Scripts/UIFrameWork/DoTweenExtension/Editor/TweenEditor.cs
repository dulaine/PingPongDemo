using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;


[CustomEditor(typeof(BaseDoTween), true)]
public class TweenEditor : Editor {

    public override  void OnInspectorGUI()
    {
        GUILayout.Space(6f);
        base.OnInspectorGUI();
        DrawCommonProperties();
    }

    protected void DrawCommonProperties()
    {
        BaseDoTween doTween = target as BaseDoTween;
        GUI.changed = false;

        BaseDoTween.Style style = (BaseDoTween.Style)EditorGUILayout.EnumPopup("Play Style", doTween.style);

        AnimationCurve curve = EditorGUILayout.CurveField("Animation Curve", doTween.animationCurve, GUILayout.Width(170f), GUILayout.Height(62f));

        GUILayout.BeginHorizontal();
        float dur = EditorGUILayout.FloatField("Duration", doTween.duration, GUILayout.Width(170f));
        GUILayout.Label("seconds");
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();
        float del = EditorGUILayout.FloatField("Start Delay", doTween.delay, GUILayout.Width(170f));
        GUILayout.Label("seconds");
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();
        SerializedProperty sp = serializedObject.FindProperty("OnComplete");
        EditorGUILayout.PropertyField(sp, new GUIContent("OnComplete"));
        GUILayout.EndHorizontal();

        if (GUI.changed)
        {
            doTween.animationCurve = curve;
            doTween.style = style;
            doTween.duration = dur;
            doTween.delay = del;
            doTween.OnComplete = sp.objectReferenceValue as BaseDoTween;
#if UNITY_EDITOR
            if (doTween)
            {
                UnityEditor.EditorUtility.SetDirty(doTween);
            }
#endif
        }

    }

}
