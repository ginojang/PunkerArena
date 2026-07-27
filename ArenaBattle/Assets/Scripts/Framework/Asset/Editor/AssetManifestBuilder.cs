#if UNITY_EDITOR
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.AddressableAssets;
using UnityEngine;

/// <summary>
/// AddressableAssetSettings의 전 엔트리를 읽어 AssetManifest(Assets/Resources/AssetManifest.asset)를 생성한다.
/// Addressables 카탈로그 → 정적 매니페스트로 대체하기 위한 1단계.
/// 각 에셋을 (1)어드레스블 주소, (2)풀 프로젝트 경로 두 키로 등록한다.
/// 씬(.unity)은 매니페스트에 넣지 않고 목록만 로그한다(Build Settings 처리는 별도 단계).
/// </summary>
public static class AssetManifestBuilder
{
    const string ManifestPath = "Assets/Resources/AssetManifest.asset";

    [MenuItem("Tools/Addressables Removal/Build AssetManifest")]
    public static void Build()
    {
        var settings = AddressableAssetSettingsDefaultObject.Settings;
        if (settings == null)
        {
            Debug.LogError("[AssetManifest] AddressableAssetSettings 를 찾지 못함.");
            return;
        }

        var manifest = ScriptableObject.CreateInstance<AssetManifest>();
        var seen = new HashSet<string>();
        int assetCount = 0, keyCount = 0, skipped = 0;
        var scenes = new List<string>();

        foreach (var group in settings.groups)
        {
            if (group == null) continue;
            foreach (var entry in group.entries)
            {
                if (entry == null) continue;
                string path = entry.AssetPath;
                if (string.IsNullOrEmpty(path)) { skipped++; continue; }

                if (path.EndsWith(".unity"))
                {
                    scenes.Add(path); // 씬은 Build Settings 단계에서 처리
                    continue;
                }

                if (AssetDatabase.IsValidFolder(path))
                {
                    Debug.LogWarning($"[AssetManifest] 폴더 엔트리 스킵(하위 확장 미구현): {path}");
                    skipped++;
                    continue;
                }

                var asset = AssetDatabase.LoadMainAssetAtPath(path);
                if (asset == null) { skipped++; continue; }

                if (AddKey(manifest, seen, entry.address, asset)) keyCount++;
                if (AddKey(manifest, seen, path, asset)) keyCount++;
                assetCount++;
            }
        }

        if (!AssetDatabase.IsValidFolder("Assets/Resources"))
            AssetDatabase.CreateFolder("Assets", "Resources");

        // 기존 매니페스트가 있으면 덮어씀
        var existing = AssetDatabase.LoadAssetAtPath<AssetManifest>(ManifestPath);
        if (existing != null)
        {
            existing.entries = manifest.entries;
            EditorUtility.SetDirty(existing);
        }
        else
        {
            AssetDatabase.CreateAsset(manifest, ManifestPath);
        }
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        Debug.Log($"[AssetManifest] 생성 완료: assets={assetCount}, keys={keyCount}, skipped={skipped}, scenes={scenes.Count}");
        if (scenes.Count > 0)
            Debug.Log("[AssetManifest] 씬 목록(Build Settings 단계 대상):\n  " + string.Join("\n  ", scenes));
    }

    static bool AddKey(AssetManifest m, HashSet<string> seen, string key, Object asset)
    {
        if (string.IsNullOrEmpty(key) || !seen.Add(key)) return false;
        m.entries.Add(new AssetManifest.Entry { key = key, asset = asset });
        return true;
    }
}
#endif
