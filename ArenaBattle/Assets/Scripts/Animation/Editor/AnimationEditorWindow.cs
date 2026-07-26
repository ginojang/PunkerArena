using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class AnimationEditorWindow : EditorWindow
{
    private string m_SelectAniClipList = "";
    private Object m_OverwriteAniClipTargetFolder = null;

    [MenuItem("Tools/Animation/AniClipOverWrite")]
    static void Open()
    {
        EditorWindow.GetWindow(typeof(AnimationEditorWindow));
    }

    private void OnGUI()
    {
        ProcessOverwriteAnimationClipType();
    }

    private void ProcessOverwriteAnimationClipType()
    {
        EditorGUILayout.BeginVertical("box");
        EditorGUILayout.LabelField("선택한 애니 클립을 타겟 폴더에서 같은게 있는지 검사후 복사해준다.");
        EditorGUILayout.EndVertical();

        EditorGUILayout.Space();

        EditorGUILayout.BeginVertical("box");

        m_SelectAniClipList = "";
        for (int i = 0; i < Selection.objects.Length; i++)
        {
            m_SelectAniClipList += Selection.objects[i].name;
            m_SelectAniClipList += ", ";
        }

        m_SelectAniClipList = EditorGUILayout.TextField("선택한 애니 리스트", m_SelectAniClipList);

        m_OverwriteAniClipTargetFolder = EditorGUILayout.ObjectField("덮어씌기할 타겟 폴더", m_OverwriteAniClipTargetFolder, typeof(Object), true);

        EditorGUILayout.EndVertical();

        EditorGUILayout.Space();

        EditorGUILayout.BeginHorizontal();

        if (GUILayout.Button("Set"))
        {
            string[] tempPath = new string[1];
            tempPath[0] = AssetDatabase.GetAssetPath(m_OverwriteAniClipTargetFolder);
            string[] assetsGuidList = AssetDatabase.FindAssets("t:AnimationClip", tempPath);

            List<AnimationClip> targetList = new List<AnimationClip>();

            for (int i = 0; i < assetsGuidList.Length; i++)
            {
                string findAssetPath = AssetDatabase.GUIDToAssetPath(assetsGuidList[i]);
                AnimationClip addData = AssetDatabase.LoadAssetAtPath(findAssetPath, typeof(AnimationClip)) as AnimationClip;
                targetList.Add(addData);
            }

            //if (targetList.Count == 0)
            //{
            //    EditorUtility.DisplayDialog("End", "실패 : 타겟폴더에 AnimationClip이 없습니다.", "닫기");
            //    return;
            //}

            foreach (Object element in Selection.objects)
            {
                string path = AssetDatabase.GetAssetPath(element);
                ModelImporter importer = ModelImporter.GetAtPath(path) as ModelImporter;
                AnimationClip selectClip = null;

                if (importer != null)
                {
                    Object[] objects = AssetDatabase.LoadAllAssetsAtPath(path);

                    bool check = false;

                    for (int i = 0; i < objects.Length; i++)
                    {
                        AnimationClip tempClip = objects[i] as AnimationClip;

                        if (tempClip == null)
                            continue;

                        if (tempClip.name.Contains("_preview") == true)
                            continue;

                        if (check == true)
                        {
                            EditorUtility.DisplayDialog("End", "실패 : Split 애니는 지원하지 않습니다.", "닫기");
                            return;
                        }
                        check = true;

                        selectClip = tempClip;
                    }
                }
                else if (element.GetType() == typeof(AnimationClip))
                {
                    selectClip = element as AnimationClip;
                }
                else
                    continue;

                if (selectClip == null)
                    continue;

                bool IsOverwrite = false;
                for (int i = 0; i < targetList.Count; i++)
                {
                    AnimationClip targetClip = targetList[i];

                    if (targetClip.name != selectClip.name)
                        continue;

                    if (targetClip == selectClip)
                        continue;

                    // 기존 애니 이벤트를 복사해둔다.
                    List<AnimationEvent> targetEventList = new List<AnimationEvent>();
                    if (targetClip.events.Length > 0)
                        targetEventList.AddRange(targetClip.events);

                    // 선택한 애니를 타겟에 복사해준다.
                    EditorUtility.CopySerialized(selectClip, targetClip);

                    // 기존 애니 이벤트를 복사해한다.
                    if( targetEventList.Count > 0 )
                        AnimationUtility.SetAnimationEvents(targetClip, targetEventList.ToArray());

                    EditorUtility.SetDirty(targetClip);

                    IsOverwrite = true;
                    break;
                }

                if( IsOverwrite == false)
                {
                    //string newPath = System.IO.Path.Combine(System.IO.Path.GetDirectoryName(tempPath[0]), selectClip.name) + ".anim";
                    string newPath = tempPath[0] +"/"+ selectClip.name + ".anim";

                    AnimationClip resultClip = new AnimationClip();
                    EditorUtility.CopySerialized(selectClip, resultClip);
                    AssetDatabase.CreateAsset(resultClip, newPath);
                }
            }

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh(ImportAssetOptions.ForceUpdate);
        }

        EditorGUILayout.EndHorizontal();
    }

    private AnimationClip CreateAnimationClip(AnimationClip _clip)
    {
        string path = AssetDatabase.GetAssetPath(_clip);
        ModelImporter importer = ModelImporter.GetAtPath(path) as ModelImporter;

        // 임포터가 아니라면 애니 클립이니 무시한다.
        if (importer == null)
            return _clip;

        string newPath = System.IO.Path.Combine(System.IO.Path.GetDirectoryName(path), _clip.name) + ".anim";

        AnimationClip resultClip = AssetDatabase.LoadAssetAtPath(newPath, typeof(AnimationClip)) as AnimationClip;
        if (resultClip == null)
        {
            resultClip = new AnimationClip();
            EditorUtility.CopySerialized(_clip, resultClip);
            AssetDatabase.CreateAsset(resultClip, newPath);
        }

        return resultClip;
    }
}
