using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using Devil.Gui;
using UnityEngine.UI;
using UnityEditor.UI;

[CustomEditor(typeof(LiteButton), true)]
public class LiteButtonEditor : ButtonEditor
{
    LiteButton Instance = null;

    protected override void OnEnable()
    {
        base.OnEnable();

        Instance = (LiteButton)target;
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        EditorGUILayout.ObjectField("버튼 라벨", Instance.Label, typeof(Text), false);
        //EditorGUILayout.ColorField(element.labelColorNormal);
        //EditorGUILayout.ColorField(element.labelColorDisable);
        //EditorGUILayout.ObjectField(element.labelOutline, typeof(Outline), false);

        Instance.SoundPath = EditorGUILayout.TextField("사운드 이름", Instance.SoundPath);
        Instance.CanPlaySound = EditorGUILayout.Toggle("CanPlaySound", Instance.CanPlaySound);

        base.OnInspectorGUI();

        serializedObject.ApplyModifiedProperties();

        if (GUI.changed)
            EditorUtility.SetDirty(Instance);
    }
}
