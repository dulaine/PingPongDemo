using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(UIAnimationComponent), true)]
[CanEditMultipleObjects]
public class UIAnimationComponentEditor : Editor {

    public override void OnInspectorGUI()
    {
        GUILayout.Space(6f);
        base.DrawDefaultInspector();

        UIAnimationComponent animComponent = (UIAnimationComponent)target;

        GUILayout.Space(6f);

        if (GUILayout.Button("Play Selected Group"))
        {
            animComponent.PlayAtIndex(animComponent.SelectPlayGroupIndex);
        }
    }
}
