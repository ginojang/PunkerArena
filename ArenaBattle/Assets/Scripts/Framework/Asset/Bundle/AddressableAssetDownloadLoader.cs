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

        var manifest = AssetManifest.Instance;
        if (manifest != null && manifest.TryGet(path, out var asset) && asset != null)
        {
            if (loader.MainAsset == null)
                loader.MainAsset = asset;
            CompleteLoad();
        }
        else
        {
            Fail($"manifest key not found: {path}");
        }
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
