#if UNITY_EDITOR
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

/// <summary>
/// 게임 씬을 Build Settings에 등록하는 에디터 유틸(main = index 0).
/// (Addressables 제거 완료 후, 매니페스트 생성 로직은 삭제됨 — AssetManifest는 이제 정적 애셋이다.
///  에셋을 추가/변경하면 Assets/Resources/AssetManifest.asset을 직접 갱신해야 한다.)
/// </summary>
public static class AssetManifestBuilder
{
    [MenuItem("Tools/Addressables Removal/Setup Build Settings Scenes")]
    public static void SetupBuildScenes()
    {
        var guids = AssetDatabase.FindAssets("t:Scene", new[] { "Assets/Scenes" });
        var paths = new List<string>();
        foreach (var g in guids)
        {
            var p = AssetDatabase.GUIDToAssetPath(g);
            if (p.Contains("/Tool/") || p.Contains("_test") || p.EndsWith("SampleScene.unity")) continue;
            paths.Add(p);
        }
        paths.Sort();
        int mainIdx = paths.FindIndex(p => p.EndsWith("/main.unity"));
        if (mainIdx > 0) { var m = paths[mainIdx]; paths.RemoveAt(mainIdx); paths.Insert(0, m); }

        var scenes = new List<EditorBuildSettingsScene>();
        foreach (var p in paths) scenes.Add(new EditorBuildSettingsScene(p, true));
        EditorBuildSettings.scenes = scenes.ToArray();

        Debug.Log($"[BuildScenes] {scenes.Count}개 씬 등록(0=main):\n  " + string.Join("\n  ", paths));
    }
}
#endif
