
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using System.Collections;

public class AddressableAssetDownloadLoader<T> : AssetBundleLoader
{
    public override bool IsStarted { get { return downloadOperationHandle.IsValid() && downloadOperationHandle.Status != AsyncOperationStatus.None; } }

    public override bool IsIdleState
    {
        get
        {
            bool bResult = false;
            bResult |= (processState == PROCESS_STATE.LOADFAILED);
            bResult |= (processState == PROCESS_STATE.LOADSUCCESSED);
            return bResult;
        }
    }

    public float DownloadProgress { get { return downloadProgress; } }
    protected AssetLoader loader;
    protected float downloadProgress;
	protected AsyncOperationHandle downloadOperationHandle;
	protected AsyncOperationHandle<T> loadOperationHandle;
	protected bool isSceneAsset = false;

	public AddressableAssetDownloadLoader()
    {
    }

    public override void Update()
    {
        switch (processState)
        {
            case PROCESS_STATE.NONE:
                {
                    if (downloadOperationHandle.IsValid() == false)
                    {
						Debug.Log($"download start bundle {path}");
						downloadOperationHandle = Addressables.DownloadDependenciesAsync(path);
						processState = PROCESS_STATE.DOWNLOADING;
						if (downloadOperationHandle.OperationException != null)
						{
							AssetLoadFailedEvent();
						}
					}
					else if (downloadOperationHandle.Status == AsyncOperationStatus.Succeeded)
                    {
						processState = PROCESS_STATE.LOADSUCCESSED;
						CallEventFuncs();
					}
					else if (downloadOperationHandle.Status == AsyncOperationStatus.Failed)
					{
						processState = PROCESS_STATE.LOADFAILED;
						AssetLoadFailedEvent();
					}
				}
                break;

            case PROCESS_STATE.DOWNLOADING:
                {
                    if (downloadOperationHandle.Status == AsyncOperationStatus.Failed)
                    {
						processState = PROCESS_STATE.LOADFAILED;
						AssetLoadFailedEvent();
					}

					if (downloadOperationHandle.Status == AsyncOperationStatus.Succeeded)
                    {
                        processState = PROCESS_STATE.LOADING;
                    }
                    else
                    {
                        downloadProgress = downloadOperationHandle.PercentComplete * 100.0f;
					}
				}
                break;

            case PROCESS_STATE.LOADING:
                {
					if (downloadOperationHandle.Status == AsyncOperationStatus.Succeeded)
                    {
                        processState = PROCESS_STATE.LOADSUCCESSED;
                        CallEventFuncs();
                    }
                    else
                    {
						processState = PROCESS_STATE.LOADFAILED;
						AssetLoadFailedEvent();
                    }
                }
                break;

            case PROCESS_STATE.LOADSUCCESSED:
                break;
            case PROCESS_STATE.LOADFAILED:
                break;
        }
    }

	
	public override void SetDownloadFilePath(string _fullPath, AssetLoader _loader, bool _isSceneAsset = false)
    {
		isSceneAsset = _isSceneAsset;
        processState = PROCESS_STATE.NONE;
        path = _fullPath;
        this.loader = _loader;
#if UNITY_EDITOR && ASSET_LOAD_LOG
		AssetManager.Instance.AddAssetByScene(UnityEngine.SceneManagement.SceneManager.GetActiveScene().name, this);
#endif
	}

	public override void CallEventFuncs()
    {
		if (downloadOperationHandle.Result == null)
            return;

        if (loader == null)
            return;

		if (loader.IsFailed)
			return;

		if (isSceneAsset == true)
		{
			OnLoadSceneAssetComplete(downloadOperationHandle);
		}
		else
		{
			//TODO(ych):load 실패시 예외 처리 필요할까? (다운로드 후 사실상 동기 수준의 비동기)
			loadOperationHandle = Addressables.LoadAssetAsync<T>(path);
			loadOperationHandle.Completed += OnLoadAssetComplete;
		}
	}

	private void AssetLoadFailedEvent()
	{
		Debug.LogError("############# Asset loading error reason : " + downloadOperationHandle.OperationException);
		if (loader == null)
		{
			//Debug.LogError("############# Asset loader null : " + downloadOperationHandle.DebugName);
			return;
		}
        processState = PROCESS_STATE.LOADFAILED;
        loader.IsFailed = true;
		loader = null;
	}

	private void OnLoadSceneAssetComplete(AsyncOperationHandle handle)
	{
		loader.IsLoadSucceed = true;
		loader.CallEventFuncs();
		loader.IsCallbackCalled = true;
		loader = null;
	}

	private void OnLoadAssetComplete(AsyncOperationHandle<T> handle)
	{
		if (loader == null)
		{
			//Debug.LogError("############# Asset loader null : " + handle.DebugName);
			return;
		}

		if (loader.MainAsset == null)
		{
			loader.MainAsset = handle.Result as UnityEngine.Object;
		}
		
		loader.IsLoadSucceed = true;
		loader.CallEventFuncs();
		loader.IsCallbackCalled = true;
		loader = null;
	}

	// 로드된 object 를 안전하게 unload
	public override void UnloadSafe(bool clearMemory)
    {
        if (loadOperationHandle.Result == null)
        {
            return;
        }

        if (clearMemory)
        {
            Release();
        }
        else
        {
            if (true == IsLoadSucceed)
            {
				if (loadOperationHandle.Result != null)
				{
					Addressables.ReleaseInstance(loadOperationHandle);
				}
            }
        }
	}

	//TODO(ych):downloadOperationHandle의 해제 타이밍은 언제?
	public override void Release()
    {
        if (loadOperationHandle.Result == null)
        {
            return;
        }

        if (true == IsLoadSucceed)
        {
			if (loadOperationHandle.Result != null)
			{
				Addressables.Release(loadOperationHandle);
			}
		}

        processState = PROCESS_STATE.NONE;
    }
}
