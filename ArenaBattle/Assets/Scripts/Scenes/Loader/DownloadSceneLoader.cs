using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceLocations;
using UnityEngine.UI;

public class DownloadSceneLoader : UnitySceneLoader
{
	System.Action completeDownload = null;
	bool patchComplete = false;

	protected override void OnDownloadResources(System.Action onCompleteDownloadResource)
	{
		Messenger.AddListener(Definition.LOADING_UI_STARTBUTTON_CLICK, OnStartBtnClick);
		Messenger.AddListener(Definition.LOADING_UI_STARTDOWNLOADBUTTON_CLICK, OnStartDownLoadBtnClick);
		Messenger.AddListener(Definition.LOADING_UI_CANCELDOWNLOADBUTTON_CLICK, OnCancelDownLoadBtnClick);

		if (AssetManager.Instance.TotalDownloadBundleSize > 0)
		{
			completeDownload = onCompleteDownloadResource;
		}
		else
		{
			StartCoroutine(DownloadResourcesAsync(onCompleteDownloadResource));
		}
	}

	private void OnStartDownLoadBtnClick()
	{
		Messenger.RemoveListener(Definition.LOADING_UI_STARTDOWNLOADBUTTON_CLICK, OnStartDownLoadBtnClick);
		StartCoroutine(DownloadResourcesAsync(completeDownload));
	}

	private void OnCancelDownLoadBtnClick()
	{
		Messenger.RemoveListener(Definition.LOADING_UI_STARTBUTTON_CLICK, OnStartBtnClick);
		Messenger.RemoveListener(Definition.LOADING_UI_STARTDOWNLOADBUTTON_CLICK, OnStartDownLoadBtnClick);
		Messenger.RemoveListener(Definition.LOADING_UI_CANCELDOWNLOADBUTTON_CLICK, OnCancelDownLoadBtnClick);

		Application.Quit();
	}

	private IEnumerator DownloadResourcesAsync(System.Action onCompleteDownloadResource)
	{
		patchComplete = false;

		StartCoroutine(StartDownloadPatch());

		yield return new WaitUntil(() => patchComplete == true);

		//GuiUtility.Initialize();

		onCompleteDownloadResource();

		/*		var downloadBundleList = AssetManager.Instance.DownloadBundles;

				foreach (var elm in downloadBundleList)
				{
					if (elm.Key.Contains("movies"))
						Caching.compressionEnabled = false;
					else
						Caching.compressionEnabled = true;

					Debug.Log($"bundle downloading.. {elm.Key}");
					yield return Addressables.DownloadDependenciesAsync(elm.Value);
				}

				AssetManager.Instance.TotalDownloadBundleSize = 0;

		//		yield return AssetMapper.Instance.Initialize();
		onCompleteDownloadResource();*/
	}

	protected override void OnUpdateLoadingProgress(AssetLoader loader, object p)
	{
	}

	private void OnStartBtnClick()
	{
		// 비정상 종료 이어하기 부분
		base.DoComplete();

		Main main = Main.MainObject.GetComponent<Main>();

		PlayState.Instance.ChangePlayState(PlayState.STATES.Menu);
	}

	protected override void DoComplete()
	{
#if UNITY_EDITOR
		var persistentPath = System.IO.Path.Combine(Application.persistentDataPath, $"..\\..\\Unity\\{Application.companyName}_{Application.productName}");
#else
		var persistentPath = Application.persistentDataPath;
#endif

		if (System.IO.Directory.Exists(persistentPath))
		{
			// 지워진 번들 삭제
			var downloadedBundlePaths = System.IO.Directory.GetDirectories(persistentPath);
			foreach (var path in downloadedBundlePaths)
			{
				if (path.Contains("assets_all"))
				{
					var bundleName = System.IO.Path.GetFileName(path) + ".bundle";
					if (AssetManager.Instance.BundleFirstLocations.ContainsKey(bundleName) == false)
					{
#if UNITY_EDITOR
						if (UnityEditor.AddressableAssets.AddressableAssetSettingsDefaultObject.Settings.ActivePlayModeDataBuilderIndex == 2)
						{
							System.IO.Directory.Delete(path, true);
						}
#else
					System.IO.Directory.Delete(path, true);
#endif
					}
				}
			}
		}
	}

	private IEnumerator StartDownloadPatch()
	{
		patchComplete = false;

		// 다운로드 받아야할 숫자
		int totalBundleDownloadCount = 0;
		//int totalMovieDownloadCount = 0;
		int totalDownloadCount = 0;
		// 다운로드된 파일 숫자
		int downloadCount = 0;
		// 전체 다운로드 용량(movie + asset)
		long totalDownloadSize = 0;
		// 다운로드된 용량
		ulong downloadedSize = 0;
		// 다운로드중인 파일의 현재 용량.(file.downloadedBytes가 확인시점마다 달라서 오차가 생기므로 저장해놓는다)
		ulong cdownloadSize = 0;
		// 다온로드중인 파일의 바로 직전 용량(진행율 체크용)
		ulong predownloadSize = 0;


		Debug.Log("StartDownloadPatch");
		totalBundleDownloadCount = AssetManager.Instance.DownloadBundles.Count;
		totalDownloadCount = totalBundleDownloadCount;
		downloadCount = 0;
//		if (sliderBar == null)
//			sliderBar = loadingGuiObject.GetComponentInChildren<Slider>();

		// 다운로드받아야 할 사이즈 확인
		totalDownloadSize = AssetManager.Instance.TotalDownloadBundleSize;
		downloadedSize = 0;
		predownloadSize = 0;

		if (totalBundleDownloadCount > 0)
		{
//			FBAnalyticsManager.DownloadBegin();

			Debug.Log($"[BUNDLE] downloading.. begin");
			int downloadRetryCount = 0;

			Dictionary<string, long> downloadBundleSizeDic = new Dictionary<string, long>(AssetManager.Instance.DownloadBundlesSizeDic);
			Dictionary<string, List<IResourceLocation>> needdownloadBundleDic = new Dictionary<string, List<IResourceLocation>>(AssetManager.Instance.DownloadBundles);


			int debugAssetDownlaodCount = 0;

			// 다운로드 시작전에 이미 받아져있는 에셋이 있음.
			// 사이즈 확인하여 다운로드중인 데이타와 사이즈 블렌딩하여 게이지를 자연스럽게 보이도록 수정
			long preDownloadedBundleSize = 0;
			long preTotaldownloadedBundleSize = 0;
			foreach (var next in downloadBundleSizeDic)
			{
				List<IResourceLocation> nextlocations = needdownloadBundleDic[next.Key];
				var nextBundleSizeOp = Addressables.GetDownloadSizeAsync(nextlocations);
				yield return nextBundleSizeOp;
				if (nextBundleSizeOp.Result <= 0)
				{
					if (downloadBundleSizeDic.TryGetValue(next.Key, out preDownloadedBundleSize) == true)
					{
						downloadCount++;
						preTotaldownloadedBundleSize += preDownloadedBundleSize;
						// 받아야할 번들목록
						needdownloadBundleDic.Remove(next.Key);

						++debugAssetDownlaodCount;
					}
				}

				Addressables.Release(nextBundleSizeOp);
			}

			// 남은 다운로드 용량
			long needDownloadBundleSize = AssetManager.Instance.TotalDownloadBundleSize - preTotaldownloadedBundleSize;
			// 진행중인 다운로드 비율 구하기 위해
			float cdownloadrate = 0f;
			// 이번틱에 받은 파일크기
			ulong cdownloadsizeTick = 0;

			foreach (var bundle in needdownloadBundleDic)
			{
				List<IResourceLocation> locations = bundle.Value;
				downloadRetryCount = Definition.BundleDownloadRecheckCount;
				Caching.compressionEnabled = true;
				predownloadSize = 0;
				cdownloadSize = 0;
				cdownloadsizeTick = 0;

				for (; downloadRetryCount > 0; downloadRetryCount--) //downloadRetryCount도 이제 의미 없다. 이제 1번만 수행후 success/fail 확인후 바로 루프 out
				{
					Debug.Log($"[BUNDLE] downloading.. {downloadCount + 1:D3} {bundle.Key} - retryCount : {Definition.BundleDownloadRecheckCount - downloadRetryCount}");

					AsyncOperationHandle downloadHandle = Addressables.DownloadDependenciesAsync(locations);
					yield return new WaitUntil(() =>
					{
						cdownloadSize = (ulong)downloadHandle.GetDownloadStatus().DownloadedBytes;

						//Debug.Log($"downloadHandle.GetDownloadStatus().DownloadedBytes = {cdownloadSize} bytes");

						// 현재 다운로드 받는 파일의 사이즈
						cdownloadsizeTick = cdownloadSize - predownloadSize;
						// 다운받을 데이타 대비 비율
						cdownloadrate = (float)cdownloadsizeTick / (float)needDownloadBundleSize;
						// 다운받은용량에 더해주고
						downloadedSize += cdownloadsizeTick + (ulong)(preTotaldownloadedBundleSize * cdownloadrate);
						// (현재 다운받은 사이즈 + (이미 받아진 사이즈*현재 받은비율)
						var curvalue = (float)downloadedSize / (float)totalDownloadSize;
//						sliderBar.value = (float)downloadedSize / (float)totalDownloadSize;
						Messenger.Broadcast(Definition.LOADING_UI_PATCH_COUNT, curvalue, (curvalue * 100f).ToString("F1"));

						predownloadSize = cdownloadSize;
						cdownloadrate = 0;
						if (downloadHandle.IsDone)
							Debug.Log($"downloadHandle.IsDone = {downloadHandle.IsDone} downloadStatus = {downloadHandle.Status}");
						return downloadHandle.IsDone;
					});

					Addressables.Release(downloadHandle);
					break;
				}

				if (downloadRetryCount <= 0) //이리로 들어올일도 이제 없을거 같은데.
				{
					Debug.Log("다운로드 오류 종료");
					Application.Quit();

					yield break;
				}
			}

//			if (totalMovieDownloadCount == 0) FBAnalyticsManager.DownloadEnd();
		}

//		sliderBar.value = 1;
		Messenger.Broadcast(Definition.LOADING_UI_PATCH_COMPLETE);
		AssetManager.Instance.TotalDownloadBundleSize = 0;
		patchComplete = true;

		Debug.Log("EndDownloadPatch");
	}
}
