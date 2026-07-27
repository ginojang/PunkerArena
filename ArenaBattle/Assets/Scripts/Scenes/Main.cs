//using Nakama;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;
using System.Runtime.InteropServices;
public class Main : MonoBehaviour
{
#if UNITY_ANDROID
    private const string ImportName = "grpc_csharp_ext";
#else
	private const string ImportName = "__Internal";
#endif
    [DllImport (ImportName)]
    private static extern float Foopluginmethod();
    
    [SerializeField]
    private AudioClip audioClip = null;

    public static GameObject MainObject { get; private set; }

    //[SerializeField] private GameConnection _connection;
    public AudioClip curAudioClip
	{
		get { return audioClip; }
	}
    //
    // Event
    //
    public static void NotifyStageAwake()
    {
        if (MainObject != null)
            return;

        MainObject = new GameObject("Main", typeof(Main));
    }

    protected virtual void Awake()
	{
        GuiMain.Init();

        Application.targetFrameRate = 60;
    }

    // Start is called before the first frame update
    void Start()
    {
        MainObject = gameObject;

        Debug.Log("UNITY_EDITOR");
        ImmortalGameObject.AttachObject(gameObject);
        AnimationManager.Instance.Initialize();
        NetworkManager.Instance.Initialize();
        AssetManager.Instance.Initialize(OnAssetManagerIntializeComplete);

/*
#if !UNITY_EDITOR// || UNITY_ANDROID
        Debug.Log("UNITY_EDITOR");
        ImmortalGameObject.AttachObject(gameObject);
        AssetManager.Instance.Initialize(OnAssetManagerIntializeComplete);
        CSVDataManager.InitTables();
        GameDataManager.Instance.Initialize();
        AnimationManager.Instance.Initialize();
        NetworkManager.Instance.Initialize();
#else
        Debug.Log("UNITY_ANDROID");
        //StartCoroutine(CSVDataManager.InitServerTableData(CompleteServiceCheck));
        CSVDataManager.InitAWSData(false, CompleteServiceCheck);
#endif
*/
    }

    protected virtual void OnAssetManagerIntializeComplete()
    {
        UiAddressablePoolManager.Instance.Initialize();
        UnitySceneLoader.SceneConfLoad();

        // 아틀라스를 미리 로드해둔다.
        ResourcePoolManager.Instance.AsyncLoadData("Ingame", objectType.atlasSprite);
        ResourcePoolManager.Instance.AsyncLoadData("Outgame", objectType.atlasSprite);
        ResourcePoolManager.Instance.AsyncLoadData("CharacterParts", objectType.atlasSprite);
        ResourcePoolManager.Instance.AsyncLoadData("Skill_Icon1", objectType.atlasSprite);

        // [OFFLINE] 서버(AWS) 테이블 로딩 제거 — 에디터/디바이스 모두 로컬 CSV로 부팅.
        CSVDataManager.InitTables();
        GameDataManager.Instance.Initialize();
        PlayState.Instance.ChangePlayState(PlayState.STATES.Title);

        
//        Login();
    }

    // Update is called once per frame
    void Update()
    {
        deltaTime += (Time.unscaledDeltaTime - deltaTime) * 0.1f;
    }
    float deltaTime = 0;
    private void OnGUI()
	{
		int w = Screen.width, h = Screen.height;
		GUIStyle style = new GUIStyle();
		Rect rect = new Rect(0, 0, w, h * 2 / 100);
		style.alignment = TextAnchor.UpperLeft;
		style.fontSize = h * 2 / 70;
		style.normal.textColor = Color.green;
		float msec = deltaTime * 1000.0f;
		float fps = 1.0f / deltaTime;
		string text = string.Format("{0:0.0} ms ({1:0.} fps)", msec, fps);
		GUI.Label(rect, text, style);
	}

    private async void Login()
	{
        //if (_connection.Session == null)
        //{
        //    string deviceId = GetDeviceId();

        //    if (!string.IsNullOrEmpty(deviceId))
        //    {
        //        PlayerPrefs.SetString(GameConstants.DeviceIdKey, deviceId);
        //    }

        //    await InitializeGame(deviceId);
        //}
    }
/*
    private async Task InitializeGame(string deviceId)
    {
        var client = new Client("http", "localhost", 7350, "defaultkey", UnityWebRequestAdapter.Instance);
        client.Timeout = 5;

        var socket = client.NewSocket(useMainThread: true);

        string authToken = PlayerPrefs.GetString(Definition.AuthTokenKey, null);
        bool isAuthToken = !string.IsNullOrEmpty(authToken);

        string refreshToken = PlayerPrefs.GetString(Definition.RefreshTokenKey, null);

        ISession session = null;

        // refresh token can be null/empty for initial migration of client to using refresh tokens.
        if (isAuthToken)
        {
            session = Session.Restore(authToken, refreshToken);

            // Check whether a session is close to expiry.
            if (session.HasExpired(DateTime.UtcNow.AddDays(1)))
            {
                try
                {
                    // get a new access token
                    session = await client.SessionRefreshAsync(session);
                }
                catch (ApiResponseException)
                {
                    // get a new refresh token
                    session = await client.AuthenticateDeviceAsync(deviceId);
                    PlayerPrefs.SetString(Definition.RefreshTokenKey, session.RefreshToken);
                }

                PlayerPrefs.SetString(Definition.AuthTokenKey, session.AuthToken);
            }
        }
        else
        {
            session = await client.AuthenticateDeviceAsync(deviceId);
            PlayerPrefs.SetString(Definition.AuthTokenKey, session.AuthToken);
            PlayerPrefs.SetString(Definition.RefreshTokenKey, session.RefreshToken);
        }

        try
        {
            await socket.ConnectAsync(session);
        }
        catch (Exception e)
        {
            Debug.LogWarning("Error connecting socket: " + e.Message);
        }

        IApiAccount account = null;

        try
        {
            account = await client.GetAccountAsync(session);
        }
        catch (ApiResponseException e)
        {
            Debug.LogError("Error getting user account: " + e.Message);
        }

        //_connection.Init(client, socket, account, session);

        if (AssetManager.Instance.TotalDownloadBundleSize > 0)
            PlayState.Instance.ChangePlayState(PlayState.STATES.Patch);
        else
            PlayState.Instance.ChangePlayState(PlayState.STATES.Menu);
    }
*/
    private string GetDeviceId()
    {
        string deviceId = "";

        //deviceId = PlayerPrefs.GetString(GameConstants.DeviceIdKey);

        if (string.IsNullOrWhiteSpace(deviceId))
        {
            // Ordinarily, we would use SystemInfo.deviceUniqueIdentifier but for the purposes
            // of this demo we use Guid.NewGuid() so that developers can test against themselves locally.
            // Also note: SystemInfo.deviceUniqueIdentifier is not supported in WebGL.
            deviceId = Guid.NewGuid().ToString();
        }

        return deviceId;
    }
}
