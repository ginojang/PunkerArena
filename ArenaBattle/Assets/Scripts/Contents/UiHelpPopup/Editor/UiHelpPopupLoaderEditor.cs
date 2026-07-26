using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEditor;
using UnityEditor.Events;

[CustomEditor(typeof(UiHelpPopupLoader))]
public class UiHelpPopupLoaderEditor : Editor
{
    private UiHelpPopupLoader mLoader;
    private SerializedProperty mIndex;
    private SerializedProperty mTriggerObject;

    public void OnEnable()
    {
        mLoader = (UiHelpPopupLoader)target;

        mIndex = serializedObject.FindProperty("mIndex");
        mTriggerObject = serializedObject.FindProperty("mTriggerObject");
}

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        EditorGUILayout.PropertyField(mIndex, new GUIContent("테이블 인덱스(스트링)"));

        EditorGUILayout.Space(10);

        if (GUILayout.Button("Set Event"))
        {
            Button button = mLoader.GetComponent<Button>();

            if (button != null)
            {
                UnityEventTools.RemovePersistentListener(button.onClick, mLoader.Open);
                UnityEventTools.AddVoidPersistentListener(button.onClick, mLoader.Open);
            }
        }

        if (GUILayout.Button("Test Load"))
        {
            if (CSVDataManager.GetTable<HelpPopupBaseTable>() == null)
                CSVDataManager.SetTableData<HelpPopupBaseTable>("helppopup_base");

            mLoader.Open();
        }

        serializedObject.ApplyModifiedProperties();

        if (GUI.changed)
            EditorUtility.SetDirty(mLoader);
    }
}

