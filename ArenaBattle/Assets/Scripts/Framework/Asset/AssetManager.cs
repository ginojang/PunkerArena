using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.AddressableAssets.ResourceLocators;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceLocations;

public class AssetManager : SingletonWithMonoBehavior<AssetManager>
{
	public delegate void FCallback(object param);

	private class RequestCallback
	{
		public List<AssetLoader> requestLoadedList;
		public FCallback Callable;
		public object param;
	}

	public static Utility.ASSETBUNDLE_TYPE BundleType { get; private set; }

	public Dictionary<string, List<IResourceLocation>> BundleFirstLocations = new Dictionary<string, List<IResourceLocation>>();
	public Dictionary<string, List<IResourceLocation>> DownloadBundles { get; private set; } = new Dictionary<string, List<IResourceLocation>>();
	public long TotalDownloadBundleSize { get; set; } = 0L;
	public Dictionary<string, long> DownloadBundlesSizeDic = new Dictionary<string, long>();

	public List<string> HasBundleInIconImage { get; private set; } = new List<string>();

	private bool initialized;
	private Dictionary<string, AssetBundleLoader> loadedAssetBundles;
	private Dictionary<string, AssetLoader> loadedAssets;
	private List<RequestCallback> requestCallbackList;

#if UNITY_EDITOR && ASSET_LOAD_LOG
	private Dictionary<string, List<AssetBundleLoader>> loadedAssetByScene;
	public void AddAssetByScene(string sceneName, AssetBundleLoader loader)
	{
		if (loadedAssetByScene.ContainsKey(sceneName) == false)
		{
			var loaderList = new List<AssetBundleLoader>();
			loaderList.Add(loader);
			loadedAssetByScene.Add(sceneName, loaderList);
		}
		else
		{
			if (loadedAssetByScene[sceneName].Find(x => x.Path == loader.Path) == null)
			{
				loadedAssetByScene[sceneName].Add(loader);
			}
		}
	}

	public void OutputSceneByAsset(bool isFileWrite = false)
	{
		string output = "Loaded Asset list by scene\n";

		foreach(var key in loadedAssetByScene.Keys)
		{
			output += $"Scene name : {key}, Loaded asset count : {loadedAssetByScene[key].Count}, Total loaded asset count : {loadedAssetBundles.Count}, used memory : {Profiler.GetTotalAllocatedMemoryLong()}\n";
			foreach(var asset in loadedAssetByScene[key])
			{
				output += $"\tLoaded asset : {asset.Path}\n";
			}
		}

		if (isFileWrite)
		{
			System.IO.File.WriteAllText(Path.Combine(Application.dataPath , "\\log_loaded_asset_by_scene.txt"), output);
		}
	}
#endif
	protected override void Awake()
	{
		base.Awake();
	}

	private void Update()
	{
		if (loadedAssetBundles != null)
		{
			foreach (KeyValuePair<string, AssetBundleLoader> pair in loadedAssetBundles)
			{
				if (!pair.Value.IsIdleState)
				{
					pair.Value.Update();
				}
			}
		}

		if (requestCallbackList != null)
		{
			for (int i = 0; i < requestCallbackList.Count;)
			{
				if (requestCallbackList[i] != null)
				{
					if (requestCallbackList[i].requestLoadedList == null)
					{
						if (requestCallbackList[i].Callable != null)
						{
							requestCallbackList[i].Callable(requestCallbackList[i].param);
							requestCallbackList[i].Callable = null;
							requestCallbackList.RemoveAt(i);
						}
						else
						{
							++i;
						}
					}
					else
					{
						for (int j = 0; j < requestCallbackList[i].requestLoadedList.Count;)
						{
							var requsetLoadedAssetLd = requestCallbackList[i].requestLoadedList[j];
							if (requsetLoadedAssetLd.IsCallbackCalled)
							{
								requestCallbackList[i].requestLoadedList.RemoveAt(j);
							}
							else if (requsetLoadedAssetLd.IsFailed)
							{
								requestCallbackList[i].requestLoadedList.RemoveAt(j);
							}
							else
							{
								++j;
								//break;
							}
						}

						if (requestCallbackList[i].requestLoadedList.Count == 0)
						{
							if (requestCallbackList[i].Callable != null)
							{
								requestCallbackList[i].Callable(requestCallbackList[i].param);
								requestCallbackList[i].Callable = null;
								requestCallbackList.RemoveAt(i);
							}
							else
							{
								++i;
							}
						}
						else
						{
							break;
						}
						//else
						//{
						//    ++i;
						//}
					}
				}
			}
		}
	}

	public void Initialize(System.Action completeEvent = null)
	{
		Debug.Log($"[BUNDLE] AssetManager.Initialize({completeEvent})");
		if (!initialized)
		{
#if UNITY_IOS
            // Debug.Log("ClearCache !!!!!") ;
            // Caching.ClearCache(); //cache CRC error 처리 테스트용 
#endif
			initialized = true;
			if (completeEvent == null)
			{
				Addressables.InitializeAsync();
			}
			else
			{
				StartCoroutine(InitializeAddressableAndCheckBundle(completeEvent));
			}

			loadedAssets = new Dictionary<string, AssetLoader>();
			loadedAssetBundles = new Dictionary<string, AssetBundleLoader>();
			requestCallbackList = new List<RequestCallback>();

#if UNITY_EDITOR && ASSET_LOAD_LOG
			loadedAssetByScene = new Dictionary<string, List<AssetBundleLoader>>();
#endif
		}
	}

	private IEnumerator InitializeAddressableAndCheckBundle(System.Action completeEvent)
	{
		Debug.Log($"[BUNDLE] {Addressables.BuildPath} {Addressables.RuntimePath} {Addressables.PlayerBuildDataPath}");
		AsyncOperationHandle<IResourceLocator> initHandle = Addressables.InitializeAsync();
		yield return new WaitUntil(() => initHandle.IsDone);

		IResourceLocator rootLocator = Addressables.ResourceLocators.FirstOrDefault();
		if (rootLocator.Equals(default(IResourceLocator)))
		{
			Debug.LogError("[BUNDLE] NOT found root locator");
			yield break;
		}
		Dictionary<string, List<IResourceLocation>> bundleLocations = new Dictionary<string, List<IResourceLocation>>();

		BundleFirstLocations.Clear();
		DownloadBundles.Clear();
		HasBundleInIconImage.Clear();

		IList<IResourceLocation> locations;
		foreach (var res in rootLocator.Keys)
		{
			rootLocator.Locate(res, typeof(object), out locations);
			if (locations != null)
			{
				foreach (var loc in locations)
				{
					if (loc.Dependencies != null && loc.Dependencies.Count > 0)
					{
						List<IResourceLocation> firstLoc = null;
						string locationsKey = null;
						int depBundleCount = 0;
						foreach (var dep in loc.Dependencies)
						{
							if (dep.PrimaryKey.Contains(".bundle") == true)
							{
								if (depBundleCount == 0)
									locationsKey = dep.PrimaryKey;
								depBundleCount++;

								firstLoc = null;
								if (bundleLocations.TryGetValue(dep.PrimaryKey, out firstLoc) == false)
								{
									firstLoc = new List<IResourceLocation>();
									firstLoc.Add(loc);
									bundleLocations.Add(dep.PrimaryKey, firstLoc);
								}
							}
						}

						if (string.IsNullOrEmpty(locationsKey) == false && depBundleCount == 1)
						{
							firstLoc = null;
							if (loc.Dependencies.Count == 1 && BundleFirstLocations.TryGetValue(locationsKey, out firstLoc) == false)
							{
								firstLoc = new List<IResourceLocation>();
								firstLoc.Add(loc);
								BundleFirstLocations.Add(locationsKey, firstLoc);
							}
						}
					}

					if (loc.PrimaryKey.Contains("Assets/Asset/ui/world_map/") || loc.PrimaryKey.Contains("Assets/Asset/ui/episode_cut/"))
					{
						if (Path.GetExtension(loc.PrimaryKey) == ".png")
						{
							if (HasBundleInIconImage.Contains(loc.PrimaryKey) == false)
								HasBundleInIconImage.Add(loc.PrimaryKey);
						}
					}
				}
			}
		}

		Debug.Log($"[BUNDLE] total bundle count #1 : {BundleFirstLocations.Count} == {bundleLocations.Count} ???");
		if (BundleFirstLocations.Count < bundleLocations.Count)
		{
			foreach (var loc in bundleLocations)
			{
				if (BundleFirstLocations.ContainsKey(loc.Key) == false)
					BundleFirstLocations.Add(loc.Key, loc.Value);
			}
		}

		//TotalDownloadBundleSize = 0;

		// 한번에 Download Size 가져오기
		// Addressables Asset version upgrade 해야 기능이 작동 합니다.
		//
		// AsyncOperationHandle<long> totalDownloadSizeHandle = Addressables.GetDownloadSizeAsync(rootLocator.Keys);
		// yield return new WaitUntil(() => totalDownloadSizeHandle.IsDone);
		// long totalDownloadBundleSize = totalDownloadSizeHandle.Result;
		// TotalDownloadBundleSize = totalDownloadBundleSize;


		TotalDownloadBundleSize = 0;
		DownloadBundlesSizeDic.Clear();
		foreach (var bundle in BundleFirstLocations)
		{
			AsyncOperationHandle<long> downloadSizeHandle = Addressables.GetDownloadSizeAsync(bundle.Value);
			yield return new WaitUntil(() => downloadSizeHandle.IsDone);

			if (downloadSizeHandle.Result > 0)
			{
				DownloadBundles.Add(bundle.Key, bundle.Value);
				TotalDownloadBundleSize += downloadSizeHandle.Result;
				DownloadBundlesSizeDic.Add(bundle.Key, downloadSizeHandle.Result);
			}
			else
			{
				Debug.Log($"[BUNDLE] Warning : downloadSizeHandle.Result is <= 0 {downloadSizeHandle.Result} : {bundle.Key} ");
			}
		}
		//Debug.Log($"[BUNDLE] total download size : {totalDownloadBundleSize} == {TotalDownloadBundleSize} ???");


		Debug.Log($"[BUNDLE] total locator count : {rootLocator.Keys.Count()}");
		Debug.Log($"[BUNDLE] total bundle count #2 : {BundleFirstLocations.Count} == {bundleLocations.Count} ???");
		Debug.Log($"[BUNDLE] download bundle count : {DownloadBundles.Count}");
		Debug.Log($"[BUNDLE] icon images count : {HasBundleInIconImage.Count}");
		var count = 1;
		foreach (var b in BundleFirstLocations.Keys) Debug.Log($"[BUNDLE] {count++} : {b} in BundleFirstLocations.Keys");
		count = 1;
		foreach (var b in DownloadBundles.Keys) Debug.Log($"[BUNDLE] {count++} : download {b} in DownloadBundles.Keys");
		completeEvent();
	}

	public AssetLoader PreLoadAsset(string addressPath, AssetLoader.cbFinishLoad cb, object param = null)
	{
		if (!initialized)
			return null;

		return PreloadAssetInternal(addressPath, cb, param);
	}

	public AssetLoader PreLoadAsset(int assetId, AssetLoader.cbFinishLoad cb, object param = null)
	{
		if (!initialized)
			return null;

		if (AssetMapper.Instance.GetAssetPaths(assetId, out string assetFullPath))
		{
			return PreloadAssetInternal(assetFullPath, cb, param);
		}

		return null;
	}

	public IEnumerator LoadAssetAsyncForPreloadAsset<type>(List<AssetLoader> preloadAssetList)
	{
		var isFinished = false;
		LoadAssetAsyncForPreloadAsset(preloadAssetList, (x) => isFinished = true);

		while (isFinished == false)
		{
			yield return null;
		}
	}

	public bool LoadAssetAsyncForPreloadAsset(List<AssetLoader> preloadAssetList, FCallback cb, object param = null)
	{
		if (!initialized)
			return false;

		Debug.Log("[BUNDLE] LoadAssetAsyncForPreloadAsset");
		Debug.Log("[BUNDLE] ---- preloadAssetList");
		var count = 1;
		foreach (var preloadAsset in preloadAssetList)
		{
			Debug.Log($"[BUNDLE] {count++} {preloadAsset.AssetFullPath} ");
		}

		var uncompressedPreloadAsset = new List<AssetLoader>();
		var compressedPreloadAsset = new List<AssetLoader>();

		// compress,uncompress preload asset 분리
		foreach (var preloadAsset in preloadAssetList)
		{
			if (preloadAsset.IsCompressed == true)
			{
				compressedPreloadAsset.Add(preloadAsset);
			}
			else
			{
				uncompressedPreloadAsset.Add(preloadAsset);
			}
		}

		if (uncompressedPreloadAsset.Count > 0)
		{
			RequestCallback requestUncompressAssetCallback = new RequestCallback
			{
				requestLoadedList = new List<AssetLoader>()
			};

			// uncompress preload asset 을 먼저 다운로드
			Caching.compressionEnabled = false;
			foreach (var preloadAsset in uncompressedPreloadAsset)
			{
				if (LoadAssetAsyncForPreloadAssetInternal(preloadAsset.AssetFullPath, preloadAsset))
				{
					requestUncompressAssetCallback.requestLoadedList.Add(preloadAsset);
				}
			}

			requestCallbackList.Add(requestUncompressAssetCallback);

			// 람다 변수 캡쳐를 위함
			var completeCallback = cb;
			var completeParam = param;
			// 완료후 compress preload asset 다운로드
			requestUncompressAssetCallback.Callable = (nullParam) =>
			{
				RequestCallback requestCompressAssetCallback = new RequestCallback
				{
					requestLoadedList = new List<AssetLoader>()
				};
				Caching.compressionEnabled = true;
				foreach (var preloadAsset in compressedPreloadAsset)
				{
					if (LoadAssetAsyncForPreloadAssetInternal(preloadAsset.AssetFullPath, preloadAsset))
					{
						requestCompressAssetCallback.requestLoadedList.Add(preloadAsset);
					}
				}
				requestCallbackList.Add(requestCompressAssetCallback);

				requestCompressAssetCallback.param = completeParam;
				requestCompressAssetCallback.Callable = completeCallback;
			};
		}
		else
		{
			RequestCallback requestCompressAssetCallback = new RequestCallback
			{
				requestLoadedList = new List<AssetLoader>()
			};

			Caching.compressionEnabled = true;
			foreach (var preloadAsset in compressedPreloadAsset)
			{
				if (LoadAssetAsyncForPreloadAssetInternal(preloadAsset.AssetFullPath, preloadAsset))
				{
					requestCompressAssetCallback.requestLoadedList.Add(preloadAsset);
				}
			}
			requestCallbackList.Add(requestCompressAssetCallback);

			requestCompressAssetCallback.param = param;
			requestCompressAssetCallback.Callable = cb;
		}

		return true;
	}

	/// <summary>
	/// 어셋 로드 동기 함수
	/// 이전에 이미 어셋을 로드한 경우 loadedAssets 에 캐쉬된 어셋을 가져온다
	/// </summary>
	/// <typeparam name="T"></typeparam>
	/// <param name="addressPath"></param>
	/// <returns></returns>
	public T LoadAsset<T>(string addressPath) where T : UnityEngine.Object
	{
		if (loadedAssets.ContainsKey(addressPath))
		{
			if (loadedAssets[addressPath].MainAsset != null)
				return loadedAssets[addressPath].MainAsset as T;
		}

		return null;
	}

	public bool LoadAssetAsync<T>(string addressPath, AssetLoader.cbFinishLoad cbFinish, object param = null)
	{
		if (!initialized)
			return false;

		return LoadAssetAsyncInternal<T>(addressPath, cbFinish, param);
	}

	public bool LoadAssetAsync<T>(int assetId, AssetLoader.cbFinishLoad cbFinish, object param = null)
	{
		if (!initialized)
			return false;

		if (AssetMapper.Instance.GetAssetPaths(assetId, out string assetFullPath))
		{
			return LoadAssetAsyncInternal<T>(assetFullPath, cbFinish, param);
		}

		return false;
	}

	public IEnumerator LoadAssetAsync<T>(int assetId)
	{
		Debug.Log($"[BUNDLE] LoadAssetAsync<T>({assetId})");
		if (AssetMapper.Instance.GetAssetPaths(assetId, out string assetFullPath))
		{
			if (!loadedAssetBundles.TryGetValue(assetFullPath, out AssetBundleLoader loader))
			{
				loader = new AddressableAssetDownloadLoader<T>();
				loadedAssetBundles.Add(assetFullPath, loader);
			}

			var assetLoader = new AssetLoader();
			loader.SetDownloadFilePath(assetFullPath, assetLoader);

			while (assetLoader.IsFailed == false && assetLoader.IsLoadSucceed == false)
			{
				yield return null;
			}

			yield return assetLoader.MainAsset;
		}
	}

	public IEnumerator LoadAssetAsync<T>(string addressPath)
	{
		Debug.Log($"[BUNDLE] LoadAssetAsync<T>({addressPath})");
		if (loadedAssetBundles != null)
		{
			if (!loadedAssetBundles.TryGetValue(addressPath, out AssetBundleLoader loader))
			{
				loader = new AddressableAssetDownloadLoader<T>();
				loadedAssetBundles.Add(addressPath, loader);
			}

			var assetLoader = new AssetLoader();
			loader.SetDownloadFilePath(addressPath, assetLoader);

			while (assetLoader.IsFailed == false && assetLoader.IsLoadSucceed == false)
			{
				yield return null;
			}

			yield return assetLoader.MainAsset;
		}
	}

	private bool LoadAssetAsyncInternal<T>(string assetFullPath, AssetLoader.cbFinishLoad cbFinish, object param)
	{
		if (!string.IsNullOrEmpty(assetFullPath))
		{
			if (!loadedAssets.TryGetValue(assetFullPath, out AssetLoader ld))
			{
				ld = new AssetLoader
				{
					AssetFullPath = assetFullPath
				};
				loadedAssets.Add(assetFullPath, ld);
			}
			ld.SetEventFinishLoad(cbFinish, param);

			if (!loadedAssetBundles.TryGetValue(assetFullPath, out AssetBundleLoader loader))
			{
				loader = new AddressableAssetDownloadLoader<T>();
				loadedAssetBundles.Add(assetFullPath, loader);
			}
			loader.SetDownloadFilePath(assetFullPath, ld);

			return true;
		}

		return false;
	}

	public bool LoadSceneAsync(string addressPath, AssetLoader.cbFinishLoad cbFinish, object param = null)
	{
		if (!initialized)
			return false;

		return LoadSceneAsyncInternal(addressPath, cbFinish, param);
	}

	private bool LoadSceneAsyncInternal(string assetFullPath, AssetLoader.cbFinishLoad cbFinish, object param)
	{
		if (!string.IsNullOrEmpty(assetFullPath))
		{
			if (!loadedAssets.TryGetValue(assetFullPath, out AssetLoader ld))
			{
				ld = new AssetLoader
				{
					AssetFullPath = assetFullPath
				};
				loadedAssets.Add(assetFullPath, ld);
			}
			ld.SetEventFinishLoad(cbFinish, param);

			if (!loadedAssetBundles.TryGetValue(assetFullPath, out AssetBundleLoader loader))
			{
				loader = new AddressableAssetDownloadLoader<UnityEngine.SceneManagement.Scene>();
				loadedAssetBundles.Add(assetFullPath, loader);
			}
			loader.SetDownloadFilePath(fullPath: assetFullPath, loader: ld, isSceneAsset: true);

			return true;
		}

		return false;
	}

	private AssetLoader PreloadAssetInternal(string assetFullPath, AssetLoader.cbFinishLoad cbFinish, object param)
	{
		Debug.Log($"AssetManager.PreloadAssetInternal(assetFullPath={assetFullPath})");
		if (!string.IsNullOrEmpty(assetFullPath))
		{
			if (!loadedAssets.TryGetValue(assetFullPath, out AssetLoader ld))
			{
				ld = new AssetLoader
				{
					AssetFullPath = assetFullPath
				};
				loadedAssets.Add(assetFullPath, ld);
			}
			ld.SetEventFinishLoad(cbFinish, param);

			return loadedAssets[assetFullPath];
		}

		return null;
	}

	private bool LoadAssetAsyncForPreloadAssetInternal(string assetFullPath, AssetLoader assetLoader)
	{
		Debug.Log($"LoadAssetAsyncForPreloadAssetInternal( {assetFullPath })");
		if (!string.IsNullOrEmpty(assetFullPath))
		{
			if (loadedAssets.ContainsKey(assetFullPath) == false)
			{
				loadedAssets.Add(assetFullPath, assetLoader);
			}

			if (!loadedAssetBundles.TryGetValue(assetFullPath, out AssetBundleLoader loader))
			{
				loader = new AddressableAssetDownloadLoader<Object>();
				loadedAssetBundles.Add(assetFullPath, loader);
			}
			loader.SetDownloadFilePath(assetFullPath, assetLoader);

			return true;
		}

		return false;
	}

	/// <summary>
	/// 어셋 번들상에 해당 asset 이 존재하는 체크
	/// </summary>
	/// <param name="assetPath"></param>
	/// <returns></returns>
	//public bool IsExsistAsset(string assetPath)
	//{
	//	return AssetInBundle.ContainsKey(assetPath);
	//}

	public void ReleaseAsset(string assetPath)
	{

	}

	public void ReleaseAllAsset()
	{
		System.GC.Collect();
		Resources.UnloadUnusedAssets();
	}
}