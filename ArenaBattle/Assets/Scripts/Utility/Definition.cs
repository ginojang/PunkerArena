public class Definition
{
	public enum SERVICE_CHECK_ERROR
	{
		SUCCESS = 0,
		UNKNOWN = 11,
		FILE_NOT_EXIST,
		SERVERTYPE_NOT_MATCH,
		RESOURCE_NUMBER_NOT_EXIST,
		SERVER_CONNECT_INFO_NOT_EXIST,
	}
	public enum BUILD_PHASE
    {
		TEST,
		INVALID,
		DEVELOP,
		STAGING,
		PRODUCTION
    }

	public static int MAX_CHARACTER = 5;
	public static int MAX_MONSTER = 9;

	public const string STR_LOCALIZATION_KEY = "locale";
	public const string STR_LOCALIZATION_PREFIX = "localization/";

	public const string ScenePathPrefix = "Assets/Scenes/";
	public const string SceneConfPath = "SceneConf/SceneConf";
	public const string MusicPath = "Assets/Asset/Music/";
	public const string NotePath = "Assets/Asset/Note/";
	public const string TexturePath = "Assets/Textures/";
	public const string SoundPath = "Assets/Asset/Sound/";
	public const float DefaultFOV = 30f;

	public const string Event_ChangeText = "EventChangeText";

	public const string UISettingComplete = "UISettingComplete";
	public const string EndTargetAction = "EndTargetAction";
	public const string ChangeSkillStatus = "ChangeSkillStatus";

	// 버프 관련
	public const string AlertNewTurn = "AlertNewTurn";

	

	// Character Setting
	public const string LoadData = "LoadData";

	//배틀
	public const string DamageUIOn = "DamageUIOn";
	public const string EvadeDamageOn = "EvadeDamageOn";
	public const string HealDamageOn = "HealDamageOn";

	// patch
	public const string LOADING_UI_STARTBUTTON_CLICK = "LOADING_UI_STARTBUTTON_CLICK";
	public const string LOADING_UI_STARTDOWNLOADBUTTON_CLICK = "LOADING_UI_STARTDOWNLOADBUTTON_CLICK";
	public const string LOADING_UI_CANCELDOWNLOADBUTTON_CLICK = "LOADING_UI_CANCELDOWNLOADBUTTON_CLICK";
	public const string LOADING_UI_PATCH_COUNT = "LOADING_UI_PATCH_COUNT";
	public const string LOADING_UI_PATCH_COMPLETE = "LOADING_UI_PATCH_COMPLETE";
	public const int BundleDownloadRecheckCount = 5;

	public const string DeviceIdKey = "nakama.deviceId";
	public static string AuthTokenKey = "nakama.authToken";
	public static string RefreshTokenKey = "nakama.refreshToken";

	//Character
	public static string AttachTargetEffect = "AttachTargetEffect";
	public static string CharacterAI = "CharacterAI";
	

	//Turn
	public static string StartTurn = "StartTurn";

	//InGame
	public static string InGameLoad = "InGameLoad";
	public static string InGameReady = "InGameReady";
	public static string InGameGame = "InGameGame";
	public static string InGameResult = "InGameResult";
	public const string CompleteState = "CompleteState";
	public static string ChaaracterIntroEnd = "ChaaracterIntroEnd";
	public static string InitializeCharacterATB = "InitializeCharacterATB";
	public static string AddBurstActionCharacter = "AddBurstActionCharacter";
	public static string RemoveBurstACtionCharacter = "RemoveBurstACtionCharacter";
	public static string BurstModeEnd = "BurstModeEnd";
	public static string BurstOnDamage = "BurstOnDamage";
	public static string CharacterDeath = "CharacterDeath";
	//UIGame
	public static string UpdateHPSlider = "UpdateHPSlider";
	public static string UpdateSPSlider = "UpdateSPSlider";
	public static string CharacterInfoSliderOFF = "CharacterInfoSliderOFF";
	public static string InSkillUI = "InSkillUI";
	public static string OutSkillUI = "OutSkillUI";
	public static string InSkillAIUI = "InSkillAIUI";
	public static string OutSkillAIUI = "OutSkillAIUi";
	public static string ReserveSkillComplete = "ReserveSkillOn";
	//public static string SetSkillDescription = "SetSkillDescription";
	public static string AttachBuffIcon = "AttachBuffIcon";
	public static string RemoveBuffIcon = "RemoveBuffIcon";
	public static string BattleInfoSet = "BattleInfoSet";
	public static string CharacterInfoSliderAllOn = "CharacterInfoSliderAllOn";
	
	//Burst
	public static string AttachBurstStartEffect = "AttachBurstStartEffect";
	public static string BurstCoolUp = "BurstCoolUp";

	public static string ChangeTimeColor = "ChangeTimeColor";
	//Grid
	public static string AllGridOff = "AllGridOff";
	public static string GridSystemEnter = "GridSystemEnter";
	public static string SetTargetGrid = "SetTargetGrid";

	// Trigger



	//Cam
	public static string SetBasicTarget = "SetBasicTarget";
	public static string SetLookAtTarget = "SetLookAtTarget";
	public static string IntroOwnerCam = "IntroOwnerCam";
	public static string IntroEnemyCam = "IntroEnemyCam";

	public static string GameResult = "GameResult";

	public static string StopTime = "StopTime";

	public const string ServiceUrl = "https://futtidino-data.s3.ap-southeast-1.amazonaws.com/game-data";//"https://devil410.synology.me/FruitDinoCSV";//"https://futtidino-data.s3.ap-southeast-1.amazonaws.com/game-data";

	#region Trigger
	public static string StartCheckTrigger = "StartCheckTrigger";
	public static string InsertComplete = "InsertComplete";
	public static string StartCheckAttackLock = "StartCheckAttackLock";
	
	#endregion

	#region Buff
	public static string InsertBuff = "InsertBuff";
	public static string CharacterCheckTriggerBuff = "CharacterCheckTriggerBuff";
	public static string CharacterCheckHitBuff = "CharacterCheckHitBuff";
	public static string CheckRemoveBuff = "CheckRemoveBuff";
	public static string CheckTriggerBuffDone = "CheckTriggerBuffDone";
	#endregion

	#region CC
	public static string InsertCC = "InsertCC";
	public static string CheckRemoveCC = "RemoveCC";
	public static string GetCharacterState = "GetCharacterState";
	public static string SetCharacterState = "SetCharacterState";
	public static string CheckRemoveCC_Clear = "CheckRemoveCC_Clear";
    #endregion
    #region UI
    public static string SetCurrentTurnUI = "SetCurrentTurnUI";
	public static string SetNextTurnUI = "SetNextTurnUI";
	public static string SetNextTurnUIOff = "SetNextTurnUIOff";
	public static string SetNextTurnUIAllOff = "SetNextTurnUIAllOff";
	public static string SetSkillButtonInteractiveFalse = "SetSkillButtonInteractiveFalse";
	public static string SetPosition = "SetPosition";
	public static string SetSkillDescription = "SetSkillDescription";
	public static string SkillDescriptionOff = "SkillDescriptionOff";
	public static string SetRoundUI = "SetRoundUI";
	public static string InsertInfoIcon = "InsertInfoIcon";
	public static string RemoveInfoIcon = "RemoveInfoIcon";
	public static string ResetPassClick = "ResetPassClick";
	public static string PassClick = "PassClick";
	public static string AddBurst = "AddBurst";
    #endregion

    #region New
    public static int Dino_BasicAttack = 1;


    #region Grid
    public static string SkillSelect = "SkillSelect";
	public static string GridOff = "GridOff";
	public static string GridTargetOff = "GridTargetOff";
	public static string GridTouchOn = "GridTouchOn";
	public static string StartAIGrid = "StartAIGrid";
	public static string SkillSelectTime = "SkillSelectTime";
    #endregion

    #region Turn Manager
    public static string SetCasterNextTurn = "SetCasterNextTurn";
	public static string SetTargetNextTurn = "SetTargetNextTurn";
	public static string ExitTurn = "ExitTurn";
	public static string StartMakeCurrentTurn = "StartMakeCurrentTurn";
    public static string StartEmptyTurn = "StartEmptyTurn";
	public static string StartSetRoundProfile = "StartSetRoundProfile";
	public static string StartInsertNextTurn = "StartInsertNextTurn";
	public static string SetPassNextTurn = "SetPassNextTurn";
    #endregion

    #region BattleManager
    public static string BattleManagerInvokeAction = "BattleManagerInvokeAction";
	public static string AddCharacterBehavior = "AddCharacterBehavior";
	public static string RemoveCharacterBehavior = "RemoveCharacterBehavior";
	public static string CompleteInitTurn = "CompleteInitTurn";
	public static string RemoveActionFlow = "RemoveActionFlow";
	public static string CompleteInsertTrigger = "CompleteInsertTrigger";
	public static string GoToState = "GoToState";
	#endregion

	#region Costume
	public static string Dino_Costume_End = "Dino_Costume_End";
    public static string Dino_ServerInfoCostume_End = "Dino_ServerInfoCostume_End";
	public static string Dino_IngameCostume_End = "Dino_IngameCostume_End";
	public static string baseFilePath = "Assets/Asset/Cha/Dino/";
	public static string baseMonsterFilePath = "Assets/Asset/Cha/monster/";
	public static string baseCharBodyName = "ca_orange_01";
	public static string MENU_CHARACTER_LOADED = "MENU_CHARACTER_LOADED";
	public static string MENU_CHARACTER_CHANGE = "MENU_CHARACTER_CHANGE";
	public static string MENU_CHANGERANDOMCLASS = "MENU_CHANGERANDOMCLASS";
	public static string MENU_CHANGERANDOMPARTS = "MENU_CHANGERANDOMPARTS";
	public static string MENU_CHANGESAVE = "MENU_CHANGESAVE";
	public static string MENU_CHANGERANDOMALL = "MENU_CHANGERANDOMALL";
	public static string MENU_INVENTORY_SNAPSHOT = "MENU_INVENTORY_SNAPSHOT";
	public static string CreateDino = "CreateDino";
	public static string CreateDinoSetUI = "CreateDinoSetUI";
	#endregion

	#region AI
	public static string SetMakeAIData = "SetMakeAIData";
    #endregion
    #endregion
}
