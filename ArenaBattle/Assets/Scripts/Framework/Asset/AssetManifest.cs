using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Addressables 카탈로그(주소키 → 에셋)를 대체하는 정적 매니페스트.
/// 에디터에서 AssetManifestBuilder로 생성(Assets/Resources/AssetManifest.asset)하고,
/// 런타임에선 Resources.Load&lt;AssetManifest&gt;("AssetManifest")로 로드해 키로 직접참조를 얻는다.
///
/// 키는 두 종류를 모두 담는다: (1) 어드레스블 짧은 주소(ca_orange_01, UiMenu …),
/// (2) 에셋 풀 프로젝트 경로(Assets/Asset/BattleSource/BattleManager.prefab).
/// 코드가 둘 중 무엇으로 로드하든 해석되게 하기 위함.
/// </summary>
public class AssetManifest : ScriptableObject
{
    [System.Serializable]
    public struct Entry
    {
        public string key;
        public Object asset;
    }

    public List<Entry> entries = new List<Entry>();

    Dictionary<string, Object> _map;

    void BuildMap()
    {
        _map = new Dictionary<string, Object>(entries.Count);
        for (int i = 0; i < entries.Count; i++)
        {
            var e = entries[i];
            if (string.IsNullOrEmpty(e.key) || e.asset == null) continue;
            _map[e.key] = e.asset; // 키 고유(생성기에서 dedup) — 마지막 우선
        }
    }

    public bool TryGet(string key, out Object asset)
    {
        if (_map == null) BuildMap();
        return _map.TryGetValue(key, out asset);
    }

    public Object Get(string key)
    {
        return TryGet(key, out var o) ? o : null;
    }

    // ---- 싱글턴 접근 (Resources 로드, 1회 캐시) ----
    public const string ResourcePath = "AssetManifest"; // Assets/Resources/AssetManifest.asset

    static AssetManifest _instance;
    public static AssetManifest Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = Resources.Load<AssetManifest>(ResourcePath);
                if (_instance == null)
                    Debug.LogError($"[AssetManifest] Resources/{ResourcePath} 를 찾지 못함. AssetManifestBuilder로 생성 필요.");
            }
            return _instance;
        }
    }
}
