using UnityEngine;
using UnityEditor;
using UnityEngine.UI;

[CustomEditor(typeof(GuiSetText))]

public class GuiSetTextEditor : Editor
{
    GuiSetText Instance = null;

    void OnEnable()
    {
        Instance = (GuiSetText)target;
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        Instance.Label = (Text)EditorGUILayout.ObjectField("라벨", Instance.Label, typeof(Text), true);
        //EditorGUILayout.ColorField(element.labelColorNormal);
        //EditorGUILayout.ColorField(element.labelColorDisable);
        //EditorGUILayout.ObjectField(element.labelOutline, typeof(Outline), false);

        Instance.StringTableType = (EStringTableType)EditorGUILayout.EnumPopup("테이블 타입", Instance.StringTableType);
        Instance.StringTableIndex = EditorGUILayout.IntField("테이블 인덱스", Instance.StringTableIndex);

        if ( GUILayout.Button("Set String") == true )
        {
            switch(Instance.StringTableType)
            {
                case EStringTableType.String:
                    if( CSVDataManager.GetTable<StringTable>() == null )
                        CSVDataManager.SetTableData<StringTable>("string");
                    break;
                case EStringTableType.UI:
                    if (CSVDataManager.GetTable<StringUiTable>() == null)
                        CSVDataManager.SetTableData<StringUiTable>("string_ui");
                    break;
            }

            if (Instance.Label == null)
                Instance.Label = Instance.GetComponent<Text>();

            if (Instance.Label == null)
                EditorUtility.DisplayDialog("!!!!에러!!!!", "라벨이 없습니다.", "확인");
            else
                Instance.Set();
        }

        GUILayout.Space(10);

        if (GUILayout.Button("Table Reload") == true)
            CSVDataManager.ClearTableDatas();

        //base.OnInspectorGUI();

        serializedObject.ApplyModifiedProperties();

        if (GUI.changed)
            EditorUtility.SetDirty(Instance);
    }
}
