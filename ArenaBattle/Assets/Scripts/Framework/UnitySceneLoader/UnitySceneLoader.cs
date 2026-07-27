using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using UnityEngine;
using UnityEngine.SceneManagement;
using DG.Tweening;

public abstract class UnitySceneLoader : MonoBehaviour
{
	//
	// Variable
	//
	// 신 로딩을 단계적으로 처리 하기 위한 enum
	public enum PHASE
	{
		NONE,
		// unity scene 로드
		SCENELOAD_BEGIN,                // unity scene 로드 시작
		SCENELOAD_LOADING,              // 대기
		SCENELOAD_COMPLETING,           // 완료

		// resource download
		DOWNLOAD_BEGIN,
		DOWNLOAD_LOADING,
		DOWNLOAD_COMPLETING,

		// scene resource 로드
		RESOURCE_BEGIN,                 // 로딩 리스트 코루틴에 전달
		RESOURCE_LOADING,               // 대기
		RESOURCE_COMPLETING,            // 리소스 로딩 완료

		// woodyzzm: Pooling phase
		POOLING_BEGIN,
		POOLING_LOADING,
		POOLING_COMPLETING,

		// mission 관련 처리 진행
		MISSION_PLAYER_BEGIN,
		MISSION_PLAYER_LOADING,
		MISSION_INTERACTIVE_BEGIN,
		MISSION_INTERACTIVE_LOADING,
		MISSION_SCENE_BEGIN,
		MISSION_SCENE_LOADING,
		MISSION_SCENE_COMPLETING,       // 미션 리소스 로드 완료

		// 로딩 완료
		LOADING_COMPLETING,                // 리소스들 생성 처리 및 로직 처리
		LOADING_COMPLETED,                 // 로직 처리 완료

	}

	#region Unity Scene Loader Static Memeber
	public static bool IsLoading
	{
		get { return loadingObject != null; }
	}

	public static string AssetName { get; private set; }
	public static bool IsIncludeBuildScene { get; private set; }
	private static UnitySceneLoader loadingObject;
	private static string assetFullPath;
	//TODO(ych):더미로 로딩 화면 띄우기 추후 로딩 로직 확정 되면 삭제
	protected static GameObject loadingGuiObject;
	protected static Dictionary<string, GameObject> cachedLoadingGuiObject;
	protected static XmlElement loadingSceneConf;
	protected static Dictionary<string, ValueTuple<string, string, List<string>>> scenePreloadInfo = null;
	public static Action OnCompleteLoadSceneCallback { get; set; }
	public static Action OnCompletePreloadAssetCallback { get; set; }

	protected static string loadSceneName;
	//
	// Static Public Method
	//

	protected virtual void Awake()
	{
		loadingObject = this;
		phase = PHASE.SCENELOAD_BEGIN;
		assetFullPath = AssetName;//Definition.ScenePathPrefix + AssetName + ".unity";

		DontDestroyOnLoad(gameObject);
	}

	internal static bool NotifyStageAwake(GameObject stage_object)
	{
		if (IsLoading)
			return true;

		UnityScene.CloseCurrent();
		AssetName = SceneManager.GetActiveScene().name;

		return true;
	}

	public static bool LoadAsync(string stageName)
	{
		AssetName = stageName;

		// UiScreenTranslation 사용 안할거면
		//EndOpenFadeOut();// 바로호출

		UiScreenTransition.Instance.StartFadeOut(() => EndOpenFadeOut());// EndOpenFadeOut();
		return true;
	}

	public static void EndOpenFadeOut()
	{
		// UiScreenTransition 사용 안할때
		// EndOpenFadeIn();
		UiScreenTransition.Instance.StartFadeIn(EndOpenFadeIn);

		UnityScene.CloseCurrent();

		UiLoadingBasic.Instance.SetActiveObject(true);
		/*		var slider = loadingGuiObject.GetComponentInChildren<UnityEngine.UI.Slider>();
				if (slider != null)
					slider.value = 0f;*/
	}

	public static void EndOpenFadeIn()
	{
		//		UiLoadingBasic.Instance.SetActiveObject(false);//, 3.0f, CompletedEndLoading);
		loadingObject?.ClearLoadingObject();
		SceneManager.LoadScene(GetLoadingScene(AssetName));
	}

	static private void CompletedEndLoading()
	{
		SceneManager.LoadScene(GetLoadingScene(AssetName));
	}

	public static string GetLoadingScene(string sceneName)
	{
		if (scenePreloadInfo.ContainsKey(sceneName))
		{
			(string loadingScene, string loadingGui, List<string> preloadAssets) preloadInfo = scenePreloadInfo[sceneName];
			return preloadInfo.loadingScene;
		}
		return "SceneLoading";
	}

	public static void AddPreLoad(AssetLoader loader)
	{
		Debug.Assert(loader != null);
		if (loadingObject == null)
		{
			Debug.LogError("No Stage is Loading");
			return;
		}

		loadingObject.AddPreLoadInner(loader);

		// lhnew test 
		Debug.Log("AddPreLoad " + loader.AssetFullPath);
	}

	public static void AddPreLoad(string assetPath)
	{
		if (loadingObject == null)
		{
			Debug.LogError("No Stage is Loading");
			return;
		}

		var loader = new AssetLoader()
		{
			AssetFullPath = assetPath
		};

		loadingObject.AddPreLoadInner(loader);

		// lhnew test 
		Debug.Log("AddPreLoad " + loader.AssetFullPath);
	}

	public static void UnitySceneEnd()
	{
		if (loadingObject == null)
		{
			Debug.LogError("No Stage is Loading");
			return;
		}

		loadingObject.End();
	}

	public static void UnitySceneAdvancePhase(PHASE newPhase)
	{
		if (loadingObject == null)
		{
			Debug.LogError("No Stage is Loading");
			return;
		}

		loadingObject.AdvancePhase(newPhase);
	}

	public static PHASE GetUnitySceneAdvancePhase()
	{
		if (loadingObject == null)
		{
			Debug.LogError("No Stage is Loading");
			return PHASE.NONE;
		}

		return loadingObject.phase;
	}

	public static void SceneConfLoad()
	{
		if (scenePreloadInfo != null)
		{
			return;
		}

		scenePreloadInfo = new Dictionary<string, (string, string, List<string>)>();

		TextAsset xmlFile = Resources.Load(Definition.SceneConfPath) as TextAsset;
		Debug.Assert(xmlFile != null);
		MemoryStream assetStream = new MemoryStream(xmlFile.bytes);
		XmlReader xmlReader = XmlReader.Create(assetStream);
		XmlDocument xmlDoc = new XmlDocument();
		xmlDoc.Load(xmlReader);
		loadingSceneConf = xmlDoc.DocumentElement;
		Debug.Assert(loadingSceneConf != null);
		var root = loadingSceneConf["SceneList"];
		Debug.Assert(root != null);

		for (int i = 0; i < root.ChildNodes.Count; ++i)
		{
			(string loadingScene, string loadingGui, List<string> preloadAssets) sceneloadInfo;

			var sceneElm = root.ChildNodes[i] as XmlElement;
			Debug.Assert(sceneElm != null);

			var sceneName = sceneElm.GetAttribute("Title");
			Debug.Assert(sceneName != null);

			sceneloadInfo.loadingScene = sceneElm.GetAttribute("LoadingScene");
			sceneloadInfo.loadingGui = sceneElm.GetAttribute("LoadingGui");
			sceneloadInfo.preloadAssets = new List<string>();

			var preloadAssetsNode = sceneElm["PreloadAssets"];
			for (int j = 0; j < preloadAssetsNode.ChildNodes.Count; j++)
			{
				var assetPathElm = preloadAssetsNode.ChildNodes[j] as XmlElement;
				Debug.Assert(assetPathElm != null);

				sceneloadInfo.preloadAssets.Add(assetPathElm.GetAttribute("Path"));
			}

			scenePreloadInfo.Add(sceneName, sceneloadInfo);
		}
		GuiMain.Instance.Open<UiScreenTransition>();
		GuiMain.Instance.Open<UiLoadingBasic>();
//		GuiMain.Instance.Open<UiPatch>();
		UiLoadingBasic.Instance.SetActiveObject(false);
	}

	#endregion

	private PHASE phase;
	private List<AssetLoader> preLoadList;
	protected int loadingCount;
	protected int currentLoadingCount;

	private bool isCompleted = false;

	protected virtual void OnPreloadResources()
	{
	}

	protected virtual void OnDownloadResources(System.Action onCompleteDownloadResource)
	{
		onCompleteDownloadResource();
	}

	// woodyzzm: Pooling complete callback
	private void OnPoolingComplete()
	{
		// Advance current phase.
		AdvancePhase(PHASE.POOLING_COMPLETING);

		//OnCompletePreloadAssetCallback?.Invoke();
		AdvancePhase(PHASE.LOADING_COMPLETING);
	}

	protected virtual void Update()
	{
		switch (phase) //코루틴 중복 실수 방지
		{
			case PHASE.SCENELOAD_BEGIN:
				{
					// woodyzzm
					UiAddressablePoolManager.Instance.Destroy();
					AdvancePhase(PHASE.SCENELOAD_LOADING);
					if (IsIncludeBuildScene)
					{
						StartCoroutine(LoadSceneInBuild(AssetName));
					}
					else
					{
						AssetManager.Instance.LoadSceneAsync(assetFullPath, SceneAssetBundleLoadCompleted);
					}
				}
				break;

			case PHASE.SCENELOAD_LOADING:
				{
				}
				break;

			case PHASE.SCENELOAD_COMPLETING:
				{
				}
				break;

			case PHASE.DOWNLOAD_BEGIN:
				{
					AdvancePhase(PHASE.DOWNLOAD_LOADING);
					OnDownloadResources(CompletedResourceDownload);
				}
				break;

			case PHASE.DOWNLOAD_LOADING:
				{

				}
				break;

			case PHASE.DOWNLOAD_COMPLETING:
				{

				}
				break;

			case PHASE.RESOURCE_BEGIN:
				{
					AdvancePhase(PHASE.RESOURCE_LOADING);

					OnPreloadResources();

					PreLoadCompleted(null);
				}
				break;

			case PHASE.RESOURCE_LOADING:
				break;

			case PHASE.RESOURCE_COMPLETING:
				break;

			#region woodyzzm
			case PHASE.POOLING_BEGIN:
				{
					Debug.Log("[UiAddressablePoolManager] Pooling started.");
					AdvancePhase(PHASE.POOLING_LOADING);
					UiAddressablePoolManager.Instance.Create(OnPoolingComplete);
				}
				break;
			case PHASE.POOLING_LOADING:
				{
				}
				break;
			case PHASE.POOLING_COMPLETING:
				{
					Debug.Log("[UiAddressablePoolManager] Pooling finished.");
				}
				break;
			#endregion

			case PHASE.MISSION_PLAYER_BEGIN:
				{
				}
				break;

			case PHASE.MISSION_PLAYER_LOADING:
				{
				}
				break;

			case PHASE.MISSION_INTERACTIVE_BEGIN:
				{
					AdvancePhase(PHASE.MISSION_INTERACTIVE_LOADING);
				}
				break;

			case PHASE.MISSION_INTERACTIVE_LOADING:
				{
					AdvancePhase(PHASE.MISSION_SCENE_BEGIN);
				}
				break;

			case PHASE.MISSION_SCENE_BEGIN:
				{
				}
				break;

			case PHASE.MISSION_SCENE_LOADING:
				{
				}
				break;

			case PHASE.MISSION_SCENE_COMPLETING:
				{
					AdvancePhase(PHASE.LOADING_COMPLETING);
				}
				break;

			// BaseUiController.cs 의 AddGuiInitListener 에서 gui 가 초기화 된후에 phase 변경
			case PHASE.LOADING_COMPLETING:
				{
					if (!isCompleted)
					{
						isCompleted = true;

						//						DoComplete();
						//						AdvancePhase(PHASE.LOADING_COMPLETED);

						Invoke("DelayEnd", 1.0f);
					}
				}
				break;

			case PHASE.LOADING_COMPLETED:
				isCompleted = false;
				break;
		}
	}


	private void DelayEnd()
	{
		DoComplete();
		AdvancePhase(PHASE.LOADING_COMPLETED);
	}

	// Load phase 변경
	private void AdvancePhase(PHASE newPhase)
	{
		phase = newPhase;
	}

	// 유니티 scene 어셋번들 로드 완료
	private void SceneAssetBundleLoadCompleted(AssetLoader loader, object param)
	{
		if (!loader.IsLoadSucceed)
		{
			Debug.Log(string.Format("Stage asset bundle file load fail.... '{0}'", AssetName));
			End();
			return;
		}

		Main main = Main.MainObject.GetComponent<Main>();
		main.StartCoroutine(LoadScene());
		AdvancePhase(PHASE.SCENELOAD_COMPLETING);
	}


	private IEnumerator LoadScene()
	{
		var async = SceneManager.LoadSceneAsync(assetFullPath);
		yield return async;

		AdvancePhase(PHASE.DOWNLOAD_BEGIN);
	}

	private IEnumerator LoadSceneInBuild(string sceneName)
	{
		var async = SceneManager.LoadSceneAsync(sceneName);
		yield return async;
		AdvancePhase(PHASE.DOWNLOAD_BEGIN);
	}

	// 로딩 신 프로그레스 업데이트 
	protected virtual void OnUpdateLoadingProgress(AssetLoader loader, object p)
	{
		++currentLoadingCount;
	}

	private void CompletedResourceDownload()
	{
		AdvancePhase(PHASE.RESOURCE_BEGIN);
	}

	// 로딩 완료
	private void PreLoadCompleted(object p)
	{
		AdvancePhase(PHASE.POOLING_BEGIN);
	}


	private void AddPreLoadInner(AssetLoader loader)
	{
		loader.SetEventFinishLoad(OnUpdateLoadingProgress);

		if (preLoadList == null)
		{
			preLoadList = new List<AssetLoader>();
		}

		preLoadList.Add(loader);
	}

	protected virtual void DoComplete()
	{
		End();
	}

	protected void OnVideoEnd()
	{
		End();
	}

	protected static void OnChangedHomeDeco()
	{
	}

	protected void End()
	{
		// UiScreenTransition 안쓰면 
		//EndCloseFadeOut();

		UiScreenTransition.Instance.StartFadeOut(EndCloseFadeOut);
	}

	protected void OnFadeOutDelay()
	{
		// UiScreenTransition 안쓰면 
		//EndCloseFadeIn();

		UiScreenTransition.Instance.StartFadeIn(EndCloseFadeIn);
	}

	protected void EndCloseFadeOut()
	{
		UiScreenTransition.Instance.StartFadeIn(EndCloseFadeIn);

		UiLoadingBasic.Instance.SetActiveObject(false);

		OnCompletePreloadAssetCallback?.Invoke();

		ClearLoadingObject();

		OnCompletePreloadAssetCallback = null;

		UnityScene.CurrentUnityScene?.PostLoadScene();

		AdvancePhase(PHASE.LOADING_COMPLETED);

#if UNITY_EDITOR && ASSET_LOAD_LOG
		AssetManager.Instance.OutputSceneByAsset(true);
#endif
	}

	protected static void EndCloseFadeIn()
	{

	}

	protected void ClearLoadingObject()
	{
		Destroy(gameObject);
		loadingObject = null;
	}
}
