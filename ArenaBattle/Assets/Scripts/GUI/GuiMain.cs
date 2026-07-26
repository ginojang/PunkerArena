using Devil.Gui;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class GuiMain : GuiEvents<GuiMain>
{
	private static GuiMain instance = null;
	public static GuiMain Instance
	{
		get
		{
			if (instance == null)
			{
				Debug.LogError("First call GuiMain.Init()");
			}
			return instance;
		}
	}

	private static bool isDebugMode = false;

	public static void Init(bool debugMode = false)
	{
		if (instance != null)
			return;

		isDebugMode = debugMode;

        GameObject go = Resources.Load<GameObject>("GUI/__GuiMain__");
        instance = GameObject.Instantiate(go).GetComponent<GuiMain>();
        instance.name = instance.name.Replace("(Clone)", "");
        // 시리얼라이즈된 리스트를 딕에 다시 넣어준다. 파인드용
        instance.guiList.CreateDic();

        DontDestroyOnLoad(instance.gameObject);
	}


	private const string UI_CONTENTS_PATH = "Assets/GUI/Prefabs/Contents/";
	private const string UI_CONTENTS_RESOURCES = "GUI/Contents/";
	public const string UI_SOUND_WORLD_DIC_PATH = "Assets/Asset/english/audio/ui/world_dic/";
	public const string UI_SOUND_EFFECTS_PATH = "Assets/Asset/english/audio/ui/effects/";
	private int LAYER_UI;

	private Canvas canvasOverlay;
	public Canvas CanvasOverlay => canvasOverlay;

	private GUIScreenRatio guiScreenRatio = null;

	string openUI = "";
	/// <summary>
	/// Canvas Mode가 확장될 경우, 그에 맞는 Canvas Resolution 값을 구하는 코드 작성 필요
	/// </summary>
	public Vector2 CanvasResolution
	{
		get
		{
			if (canvasOverlay == null)
				return Vector2.zero;

			var canvasScaler = canvasOverlay.GetComponent<CanvasScaler>();
			if (canvasScaler == null)
				return Vector2.zero;

			return canvasScaler.referenceResolution;
		}
	}

	// UI contents list
    [SerializeField]
	private GuiList guiList = new GuiList();

	void Awake()
	{
		LAYER_UI = LayerMask.NameToLayer("UI");
		gameObject.layer = LAYER_UI;

		// 기본 구성 Child Objects 생성
		// 1. Canvas
		CreateCanvasOverlay();

		// 2. EventSystem
		CreateEventSystem();

		// 3. Debug Mode
		if (isDebugMode) CreateDebugMode();
	}

	void Start()
	{
		SetAspectRatio();

		// Init Call
		Broadcast(COMMON_EVENT_ID.INIT);
	}

	#region about aspect ratio (resolution)
	private void SetAspectRatio()
	{
		guiScreenRatio.SetAspectRatio();
	}

	public float GetAspectRatio()
	{
		return guiScreenRatio.GetAspectRatio();
	}
	#endregion

	private GameObject CreateChildObject(string name)
	{
		Transform childTrns = transform.Find(name);
		GameObject go = null;

		if (childTrns != null)
			go = childTrns.gameObject;
		else
			go = new GameObject(name);

		go.layer = LAYER_UI;
		go.transform.parent = transform;

		return go;
	}

	private void CreateCanvasOverlay()
	{
		GameObject go = CreateChildObject("CanvasOverlay");

		Canvas canvas = go.MakeComponent<Canvas>();
		CanvasScaler canvasScaler = go.MakeComponent<CanvasScaler>();
		GraphicRaycaster graphicRaycaster = go.MakeComponent<GraphicRaycaster>();
		guiScreenRatio = go.MakeComponent<GUIScreenRatio>();
		guiScreenRatio.UICanvasScaler = canvasScaler;

		canvas.renderMode = RenderMode.ScreenSpaceCamera;//ScreenSpaceOverlay;
		canvas.pixelPerfect = true;

		canvasScaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
		canvasScaler.referenceResolution = new Vector2(1920f, 1080f);
		canvasScaler.screenMatchMode = CanvasScaler.ScreenMatchMode.MatchWidthOrHeight;
		canvasScaler.matchWidthOrHeight = 1.0f;

		canvasOverlay = canvas;
	}

	private void CreateEventSystem()
	{
		GameObject go = CreateChildObject("EventSystem");

		EventSystem eventSystem = go.MakeComponent<EventSystem>();
		StandaloneInputModule standaloneInputModule = go.MakeComponent<StandaloneInputModule>();
		BaseInput baseInput = go.MakeComponent<BaseInput>();
	}

	private void CreateDebugMode()
	{
	}

	public void Open<T>() where T : class
	{
		string uiName = typeof(T).Name;
        Open(uiName);
	}

    public void Open(string uiName)
    {
        if (!guiList.HasName(uiName))
        {
            Debug.LogError("Open 하려는 UI가 GuiList에서 관리되고 있지 않습니다.");
            return;
        }

        GameObject uiObject = guiList.GetInstance(uiName);
        if (uiObject != null)
        {
            // 이미 등록되어 있는 상태일 경우에는 Active 상태만 Control 한다.
            uiObject.SetActive(true);
            return;
        }

        // UI Object 새로 생성 후 GuiList에 등록
        var loadType = guiList.GetLoadType(uiName);
        switch (loadType)
        {
	        case GuiList.LOAD_TYPE.ADDRESSABLE:
	        {
		        openUI = uiName;
		        string fullPath = string.Concat(UI_CONTENTS_PATH, uiName, "/", uiName, ".prefab");
		        ResourcePoolManager.Instance.AsyncGetData(uiName, objectType.ui, null, null, CompleteUILoadAsset);
		        //AssetManager.Instance.LoadAssetAsync<GameObject>(uiName, CompleteLoadAsset, uiName);
	        }
		        break;

	        case GuiList.LOAD_TYPE.RESOURCE:
	        {
		        string fullPath = string.Concat(UI_CONTENTS_RESOURCES, uiName, "/", uiName);
		        AttachUi(Resources.Load<Object>(fullPath), uiName);
	        }
		        break;
        }
    }

    private void CompleteUILoadAsset(Object data)
    {
	    //Debug.Log(param);
	    // Stage 배치
	    AttachUi((GameObject)data, openUI);
    }
	
    private void AttachUi(GameObject asset, string uiName)
    {
	    asset.name = uiName;
	    asset.transform.SetParent(canvasOverlay.transform, false);

	    // guiList 등록 (Depth 처리)
	    guiList.AddInstance(uiName, asset);
    }
    
    public void Close<T>() where T : class
	{
		string uiName = typeof(T).Name;
        Close(uiName);
	}

    public void Close(string uiName)
    {
        if (!guiList.HasName(uiName))
        {
            Debug.LogError("Close 하려는 UI가 GuiList에서 관리되고 있지 않습니다.");
            return;
        }

        GameObject uiObject = guiList.GetInstance(uiName);
        if (uiObject != null)
        {
            uiObject.SetActive(false);
        }
    }

    private void CompleteLoadAsset(AssetLoader loader, object param)
	{
		//Debug.Log(param);
		if (!loader.IsLoadSucceed)
		{
			Debug.Log(string.Format("AssetBundle file load fail... {0}", param));
			return;
		}

		// Stage 배치
		AttachUi(loader.MainAsset, param.ToString());
	}

	private void AttachUi(Object asset, string uiName)
	{
		GameObject ui = Instantiate(asset) as GameObject;
		ui.name = uiName;
		ui.transform.SetParent(canvasOverlay.transform, false);

		// guiList 등록 (Depth 처리)
		guiList.AddInstance(uiName, ui);
	}

	public void OnFinalize()
	{
		guiList.OnFinalize();
	}

	/// <summary>
	/// __GuiMain__ 에 배치되어 있는 UiContents를 Destroy (withDontDestroy : Don't Destroy Ojbect를 포함할지 여부)
	/// </summary>
	/// <param name="withDontDestroy"></param>
	public void ClearUi(bool withDontDestroy = false)
	{
		guiList.RemoveInstance(withDontDestroy);
	}

	public GameObject GetUIInstance(string uiName)
	{   // Tutorial
		return guiList.GetInstance(uiName);
	}

	public void ChangeRenderModeTo(RenderMode rm)
	{
		switch (rm)
		{
			case RenderMode.ScreenSpaceOverlay:
				{
					Camera theCurrentCamera = Utility.GetCurrentSceneCamera();
					theCurrentCamera.cullingMask = LayerMask.NameToLayer("UI");

					canvasOverlay.renderMode = RenderMode.ScreenSpaceOverlay;
					canvasOverlay.sortingOrder = 0;
					canvasOverlay.worldCamera = theCurrentCamera;
				}
				break;
			case RenderMode.ScreenSpaceCamera:
				{
					Camera theCurrentCamera = Utility.GetCurrentSceneCamera();
					//theCurrentCamera.cullingMask = LayerMask.NameToLayer("Everything") & ~(1 << LayerMask.NameToLayer("UI")) & ~(1 << LayerMask.NameToLayer("WorldSpaceUI"));
					theCurrentCamera.cullingMask = LayerMask.NameToLayer("UI");

					canvasOverlay.renderMode = RenderMode.ScreenSpaceCamera;
					canvasOverlay.worldCamera = theCurrentCamera;
					canvasOverlay.planeDistance = 100f;
					canvasOverlay.sortingOrder = 1;
					canvasOverlay.GetComponent<GraphicRaycaster>().ignoreReversedGraphics = false;
				}
				break;
			case RenderMode.WorldSpace:
				break;
			default:
				break;
		}
	}
}
