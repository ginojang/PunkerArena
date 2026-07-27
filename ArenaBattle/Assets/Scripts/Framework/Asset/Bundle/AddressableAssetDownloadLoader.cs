using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// [Addressables 제거] 기존 Addressables 다운로드/로드 대신 정적 AssetManifest에서
/// 직접참조를 조회한다. AssetManager가 new AddressableAssetDownloadLoader&lt;T&gt;()로 생성하므로
/// 클래스명/제네릭 시그니처는 그대로 유지(호출부 무수정). T는 더 이상 사용하지 않는다.
///
/// AssetManager.Update()가 매 프레임 !IsIdleState 로더만 Update() 펌핑한다.
/// 에셋은 즉시(1펌프) 해석되고, 씬(isSceneAsset)은 Build Settings + SceneManager로 로드한다.
/// </summary>
public class AddressableAssetDownloadLoader<T> : AssetBundleLoader
{
    public override bool IsStarted => processState != PROCESS_STATE.NONE;

    public override bool IsIdleState =>
        processState == PROCESS_STATE.LOADFAILED || processState == PROCESS_STATE.LOADSUCCESSED;

    public float DownloadProgress => processState == PROCESS_STATE.LOADSUCCESSED ? 100f : 0f;

    protected AssetLoader loader;
    protected bool isSceneAsset = false;
    AsyncOperation sceneOp;

    public override void SetDownloadFilePath(string _fullPath, AssetLoader _loader, bool _isSceneAsset = false)
    {
        isSceneAsset = _isSceneAsset;
        processState = PROCESS_STATE.NONE;
        path = _fullPath;
        loader = _loader;
        sceneOp = null;
#if UNITY_EDITOR && ASSET_LOAD_LOG
        AssetManager.Instance.AddAssetByScene(UnityEngine.SceneManagement.SceneManager.GetActiveScene().name, this);
#endif
    }

    public override void Update()
    {
        switch (processState)
        {
            case PROCESS_STATE.NONE:
                if (isSceneAsset) BeginSceneLoad();
                else ResolveFromManifest();
                break;

            case PROCESS_STATE.LOADING: // 씬 로드 진행 중
                if (sceneOp != null && sceneOp.isDone)
                    CompleteLoad();
                break;
        }
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

    void BeginSceneLoad()
    {
        if (loader == null || loader.IsFailed) { Fail("loader null/failed"); return; }

        string sceneName = System.IO.Path.GetFileNameWithoutExtension(path);
        if (string.IsNullOrEmpty(sceneName)) { Fail($"invalid scene path: {path}"); return; }

        sceneOp = SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Single);
        if (sceneOp == null) { Fail($"scene not in Build Settings: {sceneName}"); return; }

        processState = PROCESS_STATE.LOADING;
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
