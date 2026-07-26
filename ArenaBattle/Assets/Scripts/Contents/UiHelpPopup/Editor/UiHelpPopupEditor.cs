using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEditor;
using UnityEditor.Events;

[CustomEditor(typeof(UiHelpPopup))]
public class UiHelpPopupEditor : Editor
{
    private UiHelpPopup mHelpPopup;
    private SerializedProperty mBindBtn_Prev;
    private SerializedProperty mBindBtn_Next;
    private SerializedProperty mBindBtn_Close;
    private SerializedProperty mBindText_Page;
    private SerializedProperty mPageGameObjects;

    public void OnEnable()
    {
        mHelpPopup = (UiHelpPopup)target;

        mBindBtn_Prev = serializedObject.FindProperty("mBindBtn_Prev");
        mBindBtn_Next = serializedObject.FindProperty("mBindBtn_Next");
        mBindBtn_Close = serializedObject.FindProperty("mBindBtn_Close");
        mBindText_Page = serializedObject.FindProperty("mBindText_Page");
        mPageGameObjects = serializedObject.FindProperty("mPageGameObjects");
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        EditorGUILayout.PropertyField(mBindBtn_Prev, new GUIContent("Prev Button"));
        EditorGUILayout.PropertyField(mBindBtn_Next, new GUIContent("Next Button"));
        EditorGUILayout.PropertyField(mBindBtn_Close, new GUIContent("Close Button"));
        EditorGUILayout.PropertyField(mBindText_Page, new GUIContent("Page Text"));

        EditorGUILayout.Space(10);
        EditorGUILayout.PropertyField(mPageGameObjects, new GUIContent("Page List"));

        if (GUILayout.Button("Set"))
        {
            SetButton();
            SetPage();
        }

        serializedObject.ApplyModifiedProperties();

        if (GUI.changed)
            EditorUtility.SetDirty(mHelpPopup);
    }

    private void SetButton()
    {
        GameObject findPrev = Utility.GetDeepChild(mHelpPopup.gameObject, "BindBtnPrev");
        GameObject findNext = Utility.GetDeepChild(mHelpPopup.gameObject, "BindBtnNext");
        GameObject findClose = Utility.GetDeepChild(mHelpPopup.gameObject, "BindBtnClose");
        GameObject findTextPage = Utility.GetDeepChild(mHelpPopup.gameObject, "BindTextPage");

        if( findPrev != null )
        {
            Button button = findPrev.GetComponent<Button>();

            if (button != null)
            {
                UnityEventTools.RemovePersistentListener(button.onClick, mHelpPopup.OnClickMovePrev);
                UnityEventTools.AddVoidPersistentListener(button.onClick, mHelpPopup.OnClickMovePrev);
            }
        }

        if (findNext != null)
        {
            Button button = findNext.GetComponent<Button>();

            if (button != null)
            {
                
                UnityEventTools.RemovePersistentListener(button.onClick, mHelpPopup.OnClickMoveNext);
                UnityEventTools.AddVoidPersistentListener(button.onClick, mHelpPopup.OnClickMoveNext);
            }
        }

        if (findClose != null)
        {
            Button button = findClose.GetComponent<Button>();

            if (button != null)
            {
                UnityEventTools.RemovePersistentListener(button.onClick, mHelpPopup.OnClickClose);
                UnityEventTools.AddVoidPersistentListener(button.onClick, mHelpPopup.OnClickClose);
            }
        }

        mBindBtn_Prev.objectReferenceValue = findPrev == null ? null : findPrev;
        mBindBtn_Next.objectReferenceValue = findNext == null ? null : findNext;
        mBindBtn_Close.objectReferenceValue = findClose == null ? null : findClose;
        mBindText_Page.objectReferenceValue = findTextPage == null ? null : findTextPage;
    }

    private void SetPage()
    {
        GameObject pageList = Utility.GetDeepChild(mHelpPopup.gameObject, "BindPageList");

        if (pageList == null || pageList.transform.childCount == 0)
            return;

        List<GameObject> fidObjects = new List<GameObject>();

        for (int i = 0; i < pageList.transform.childCount; i++)
        {
            fidObjects.Add(pageList.transform.GetChild(i).gameObject);
        }

        mPageGameObjects.ClearArray();

        for (int i = 0; i < fidObjects.Count; i++)
        {
            mPageGameObjects.InsertArrayElementAtIndex(i);
            mPageGameObjects.GetArrayElementAtIndex(i).objectReferenceValue = fidObjects[i];
        }
    }
}
