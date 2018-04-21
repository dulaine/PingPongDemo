using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class InspectorWindowTool : MonoBehaviour {

    [MenuItem("CONTEXT/Component/Fold All")]
    public static void FoldAll(MenuCommand command)
    {
        ActiveEditorTracker editorTracker = ActiveEditorTracker.sharedTracker;
        Editor[] editors = editorTracker.activeEditors;

        bool areAllFolded = true;
        for (int i = 1; i < editors.Length; i++)
        {
            int getVisible = editorTracker.GetVisible(i);

            Editor edit = editors[i];
            string name = edit.name;
            string target = edit.target.name;
            Type type =  edit.target.GetType();

            if (editorTracker.GetVisible(i) != 0)
            {
                areAllFolded = false;
                break;
            }
            
        }

        for (int i = 1; i < editors.Length; i++)
        {
            editorTracker.SetVisible(i, areAllFolded ? 1 : 0);
        }
    }

    [MenuItem("CONTEXT/Component/MoveTop")]
    public static void MoveTop(MenuCommand command)
    {
        Component selected = command.context as Component;
        if (selected == null) return;

        ActiveEditorTracker editorTracker = ActiveEditorTracker.sharedTracker;
        Editor[] editors = editorTracker.activeEditors;

        for (int i = 1; i < editors.Length; i++)
        {
            Editor edit = editors[i];
            Type type = edit.target.GetType();
        }


        for (int i = 0; i < editors.Length - 2; i++)
        {
            UnityEditorInternal.ComponentUtility.MoveComponentUp(selected);
        }
        
    }

}
