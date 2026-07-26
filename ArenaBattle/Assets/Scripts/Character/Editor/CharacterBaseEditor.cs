using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(CharacterBase), true), CanEditMultipleObjects]
public class CharacterBaseEditor : Editor
{
    CharacterBase Instance = null;
    Object[] InstanceList = null;

    void OnEnable()
    {
        Instance = (CharacterBase)target;
        InstanceList = targets;
    }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        if (GUILayout.Button("Set") == true)
        {
            if (InstanceList == null)
                Set(Instance);
            else
            {
                for( int i = 0; i < InstanceList.Length; i++ )
                    Set(InstanceList[i]);
            }
        }
    }

    public void Set(Object _object)
    {
        CharacterBase data = _object as CharacterBase;

        if (data == null)
            return;

        data.characterAnimator = data.GetComponent<Animator>();

        if (data.InfoTransObject == null)
        {
            GameObject findObject = Utility.GetDeepChild(data.gameObject, "info");
            data.InfoTransObject = findObject == null ? Utility.GetDeepChild(data.gameObject, "Info") : findObject;
        }

        if (GUI.changed)
            EditorUtility.SetDirty(data);
    }
}
