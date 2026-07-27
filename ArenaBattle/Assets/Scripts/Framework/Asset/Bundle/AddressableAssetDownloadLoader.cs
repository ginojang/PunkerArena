using UnityEngine;

/// <summary>
/// [Addressables 제거] 기존 Addressables 다운로드/로드 대신 정적 AssetManifest에서
/// 직접참조를 조회한다. AssetManager가 new AddressableAssetDownloadLoader&lt;T&gt;()로 생성하므로
/// 클래스명/제네릭 시그니처는 그대로 유지(호출부 무수정). T는 더 이상 사용하지 않는다.
///
/// AssetManager.Update()가 매 프레임 !IsIdleState 로더만 Update() 펌핑한다.
/// 에셋은 즉시(1펌프) 매니페스트에서 해석된다.
/// 씬(isSceneAsset)은 여기서 로드하지 않는다 — 준비 단계로 즉시 성공만 알리고,
/// 실제 씬 로드는 UnitySceneLoader가 SceneManager(Build Settings)로 수행한다.
/// </summary>
public class AddressableAssetDownloadLoader<T> : AssetBundleLoader
{
    public override bool IsStarted => processState != PROCESS_STATE.NONE;

    public override bool IsIdleState =>
        processState == PROCESS_STATE.LOADFAILED || processState == PROCESS_STATE.LOADSUCCESSED;

    public float DownloadProgress => processState == PROCESS_STATE.LOADSUCCESSED ? 100f : 0f;

    protected AssetLoader loader;
    protected bool isSceneAsset = false;

    public override void SetDownloadFilePath(string _fullPath, AssetLoader _loader, bool _isSceneAsset = false)
    {
        isSceneAsset = _isSceneAsset;
        processState = PROCESS_STATE.NONE;
        path = _fullPath;
        loader = _loader;
#if UNITY_EDITOR && ASSET_LOAD_LOG
        AssetManager.Instance.AddAssetByScene(UnityEngine.SceneManagement.SceneManager.GetActiveScene().name, this);
#endif
    }

    public override void Update()
    {
        if (processState != PROCESS_STATE.NONE)
            return;

        if (isSceneAsset)
        {
            // 씬 준비 단계: 실제 로드는 UnitySceneLoader(SceneManager)가 담당. 여기선 성공만 통보.
            CompleteLoad();
            return;
        }

        ResolveFromManifest();
    }

    void ResolveFromManifest()
    {
        if (loader == null || loader.IsFailed) { Fail("loader null/failed"); return; }

        // 1) 기존(레거시) 에셋: 정적 매니페스트(짧은키/풀패스). 앞으로 여긴 안 건드린다.
        UnityEngine.Object asset = null;
        var manifest = AssetManifest.Instance;
        if (manifest != null)
            manifest.TryGet(path, out asset);

        // 2) 정석 폴백: Resources 폴더. 새 에셋은 Assets/**/Resources/ 아래에 넣고
        //    그 상대경로(예: "Character/foo")로 로드하면 매니페스트 수정 없이 자동으로 잡힌다.
        if (asset == null)
            asset = Resources.Load(ToResourcesPath(path));

        if (asset != null)
        {
            if (loader.MainAsset == null)
                loader.MainAsset = asset;
            CompleteLoad();
        }
        else
        {
            Fail($"매니페스트/Resources 어디에도 없음: {path}");
        }
    }

    // 로드 키를 Resources.Load용 상대경로로 정규화.
    //  "Assets/.../Resources/Foo/Bar.prefab" -> "Foo/Bar",  "Foo/Bar" -> "Foo/Bar"
    static string ToResourcesPath(string key)
    {
        const string marker = "Resources/";
        int idx = key.LastIndexOf(marker, System.StringComparison.Ordinal);
        string k = idx >= 0 ? key.Substring(idx + marker.Length) : key;
        int slash = k.LastIndexOf('/');
        int dot = k.LastIndexOf('.');
        if (dot > slash) // 파일 확장자 제거(폴더/무확장 키는 그대로)
            k = k.Substring(0, dot);
        return k;
    }

    void CompleteLoad()
    {
        if (loader == null) { processState = PROCESS_STATE.LOADSUCCESSED; return; }
        loader.IsLoadSucceed = true;
        loader.CallEventFuncs();
        loader.IsCallbackCalled = true;
        processState = PROCESS_STATE.LOADSUCCESSED;
        loader = null;
    }

    void Fail(string reason)
    {
        Debug.LogError($"[AssetLoad] fail ({path}): {reason}");
        if (loader != null) loader.IsFailed = true;
        processState = PROCESS_STATE.LOADFAILED;
        loader = null;
    }

    // 매니페스트는 직접참조(공유 에셋)라 개별 언로드/해제 없음. 인스턴스 파괴는 호출측 책임.
    public override void UnloadSafe(bool clearMemory) { }
    public override void Release() { processState = PROCESS_STATE.NONE; }
}
