using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Devil.Gui;
using System;
using System.Globalization;
using UnityEngine.EventSystems;
using UnityEngine.Events;
using System.IO;
using SRF;

public class UiMenu : UiBase<UiMenu>, IDragHandler
{
	private PlayState.STATES tempState = PlayState.STATES.None;
	[SerializeField]
	public Image bg = null;
	[SerializeField]
    private Text test_Text = null;
	[SerializeField]
	private Button snapShot = null;
	[SerializeField]
	private Button test_Btn = null;
	[SerializeField]
	private Button start_Btn = null;
	[SerializeField]
	private Button play_Btn = null;
	[SerializeField]
	private Button close_stage_Btn = null;
	[SerializeField]
	private Button setting_Btn = null;
	[SerializeField]
	private GameObject modePopUp = null;
	[SerializeField]
	private GameObject costumePopUp = null;
	[SerializeField]
	private RawImage[] renderTexturePlayer;
	[SerializeField]
	private Button[] savePlayer_Btn;

	[SerializeField]
	private Button[] classChangeBtn;
	[SerializeField]
	private Button[] randomChangeBtn;
	[SerializeField]
	private Button	allrandomChangeBtn;
	[SerializeField]
	private Button[] stageArray = null;

	[SerializeField]
	private GameObject testObj = null;
	[SerializeField]
	private Button createDinoBtn = null;
	
	private List<GameObject> charList = new List<GameObject>();

	protected override void Awake()
	{
		base.Awake();
	}

	protected override void Start()
	{
		base.Start();

		costumePopUp.SetActive(true);
		modePopUp.SetActive(false);

		test_Btn.onClick.AddListener(OnClickChangeTextBtn);
		start_Btn.onClick.AddListener(OnClickStageMode);
		setting_Btn.onClick.AddListener(OnClickSettingPostion);

		snapShot.onClick.AddListener(CreateCharacterTexture);

		// [SIMPLIFY] 5개씩 3그룹 래퍼(15개) 제거 → 인덱스 루프 (idx 캡처)
		for (int i = 0; i < classChangeBtn.Length; i++)  { int idx = i; classChangeBtn[i].onClick.AddListener(() => Messenger.Broadcast(Definition.MENU_CHANGERANDOMCLASS, idx)); }
		for (int i = 0; i < randomChangeBtn.Length; i++) { int idx = i; randomChangeBtn[i].onClick.AddListener(() => Messenger.Broadcast(Definition.MENU_CHANGERANDOMPARTS, idx)); }
		for (int i = 0; i < savePlayer_Btn.Length; i++)  { int idx = i; savePlayer_Btn[i].onClick.AddListener(() => Messenger.Broadcast(Definition.MENU_CHANGESAVE, idx)); }

		allrandomChangeBtn.onClick.AddListener(OnClickChangeAllRandom);

		stageArray[0].onClick.AddListener(OnClickSelectStage1);
		stageArray[1].onClick.AddListener(OnClickSelectStage2);
		stageArray[2].onClick.AddListener(OnClickSelectStage3);
		stageArray[3].onClick.AddListener(OnClickSelectStage4);

		close_stage_Btn.onClick.AddListener(OnClickCloseStage);
		play_Btn.onClick.AddListener(CompleteSelect);
		
		createDinoBtn.onClick.AddListener(OnClickCreateDino);
		
		Messenger.AddListener<int, GameObject>(Definition.MENU_CHARACTER_LOADED, SetMenuCharacter);
		Messenger.AddListener<int, GameObject>(Definition.MENU_CHARACTER_CHANGE, ChangeMenuCharacter);
		Messenger.AddListener<int, GameObject>(Definition.MENU_INVENTORY_SNAPSHOT, CreateInventoryCharacterTexture);
		Messenger.AddListener<string>(Definition.CreateDinoSetUI, CreateCharacterTestUI);
	}

	private void OnClickChangeAllRandom()
	{
		Messenger.Broadcast(Definition.MENU_CHANGERANDOMALL);
		//MenuUiController.Instance.ChangeAllRandom();
	}

	private void OnClickSelectStage1()
	{
		tempState = PlayState.STATES.Beach;
		GameDataManager.Instance.SelectedStage = 1;
	}

	private void OnClickSelectStage2()
	{
		tempState = PlayState.STATES.Desert;
		GameDataManager.Instance.SelectedStage = 2;
	}

	private void OnClickSelectStage3()
	{
		tempState = PlayState.STATES.Ice;
		GameDataManager.Instance.SelectedStage = 3;
	}
	private void OnClickSelectStage4()
    {
		tempState = PlayState.STATES.Avatar;
		GameDataManager.Instance.SelectedStage = 4;
	}

	private void ChangeMenuCharacter(int index, GameObject obj)
	{
		if (charList.Count <= index)
			charList.Add(obj);
		else
			charList[index] = obj;
	}

	private void SetMenuCharacter(int index, GameObject obj)
	{
		GameObject renderroot = GameObject.Find($"OwnerRenderTextureRoot");
		Transform positionRoot = renderroot.transform.Find($"Character{index + 1}");

		Transform positionObj = positionRoot.Find($"CharacterPos{index + 1}");
		Camera renderCam = positionRoot.Find("Camera").GetComponent<Camera>();
		renderTexturePlayer[index].texture = renderCam.targetTexture;

		obj.transform.SetParent(positionRoot);
		obj.transform.localPosition = Vector3.zero;

		charList.Add(obj);
		//Capture(renderCam);
		//GameDataManager.Instance.CharacterSnapShot(renderCam, index);
	}


	private void CreateCharacterTestUI(string genID)
	{
		testObj.transform.DestroyChildren();

		UserData.CharacterServerData data = GameDataManager.Instance.userData.dinoServerList[genID];
		GameDataManager.Instance.CharacterSnapShotByImage(data.partslist, testObj.transform, data.wingSlot);
	}
	
	private void CreateCharacterTexture()
	{
/*		int rand = UnityEngine.Random.Range(0, charList.Count);
		CharacterBase charbase = charList[rand].GetComponent<CharacterBase>();
		
		testObj.transform.DestroyChildren();

		GameDataManager.Instance.CharacterSnapShotByImage(
			charbase.CharacterInfo.costumeInfo.partList, testObj.transform, charbase.CharacterInfo.IsWing);
*/		
		GameObject renderroot = GameObject.Find($"OwnerRenderTextureRoot");
		Transform snapRoot = renderroot.transform.Find($"SnapShot");
		Transform snapPos = snapRoot.Find("CharacterPos");
		Camera snapCam = snapRoot.Find("Camera").GetComponent<Camera>();
		
		for (int i = 0; i < Definition.MAX_CHARACTER; i++)
		{
			// [OFFLINE GUARD] 캐릭터 리스트가 부족하면(오프라인 빈 편성) 중단
			if (charList == null || i >= charList.Count) break;
			Transform positionRoot = renderroot.transform.Find($"Character{i + 1}");
			Transform positionObj = positionRoot.Find($"CharacterPos{i + 1}");

			Transform backParent = charList[i].transform.parent;
			
			charList[i].transform.SetParent(snapPos);
			charList[i].transform.localPosition = Vector3.zero;
			charList[i].transform.localRotation = Quaternion.identity;
			
			GameDataManager.Instance.characterImage[i] = GameDataManager.Instance.CharacterSnapShot(snapCam, i);
			
			charList[i].transform.SetParent(backParent);
			charList[i].transform.localPosition = Vector3.zero;
			charList[i].transform.localRotation = Quaternion.identity;
		}
	}
	
	private void CreateInventoryCharacterTexture(int i, GameObject charbase)
	{
		GameObject renderroot = GameObject.Find($"OwnerRenderTextureRoot");
		Transform snapRoot = renderroot.transform.Find($"SnapShot");
		Transform snapPos = snapRoot.Find("CharacterPos");
		Camera snapCam = snapRoot.Find("Camera").GetComponent<Camera>();
		
		charbase.transform.SetParent(snapPos);
		charbase.transform.localPosition = Vector3.zero;
		charbase.transform.localRotation = Quaternion.identity;
		Texture2D tex = GameDataManager.Instance.CharacterSnapShot(snapCam, i);

		Destroy(charbase.gameObject);
	}
	
	private void OnClickChangeTextBtn()
	{
		Messenger.Broadcast(Definition.Event_ChangeText);
	}

	private void OnClickStageMode()
	{
		modePopUp.SetActive(true);
		costumePopUp.SetActive(false);
	}

	private void OnClickCloseStage()
	{
		modePopUp.SetActive(false);
		costumePopUp.SetActive(true);
	}

	private void OnClickSettingPostion()
	{

	}

	public void ChangeText(string text)
	{
		test_Text.text = text;
	}

	private void CompleteSelect()
	{
		CreateCharacterTexture();
		PlayState.Instance.ChangePlayState(tempState);
		tempState = PlayState.STATES.None;
	}

	private void OnClickCreateDino()
	{
		Messenger.Broadcast(Definition.CreateDino);
	}
	
	protected override void OnDestroy()
	{
		base.OnDestroy();

		Messenger.RemoveListener<int, GameObject>(Definition.MENU_CHARACTER_LOADED, SetMenuCharacter);
		Messenger.RemoveListener<int, GameObject>(Definition.MENU_CHARACTER_CHANGE, ChangeMenuCharacter);
		Messenger.RemoveListener<int, GameObject>(Definition.MENU_INVENTORY_SNAPSHOT, CreateInventoryCharacterTexture);
	}
	public void OnDrag(PointerEventData eventData)
	{
		if (eventData.pointerCurrentRaycast.gameObject == null)
			return;

		if (eventData.pointerCurrentRaycast.gameObject.name.Contains("Handle"))
		{
			GameObject obj = null;
			if (eventData.pointerCurrentRaycast.gameObject.name == "Handle1")
				obj = charList[0];
			else if (eventData.pointerCurrentRaycast.gameObject.name == "Handle2")
				obj = charList[1];
			else if (eventData.pointerCurrentRaycast.gameObject.name == "Handle3")
				obj = charList[2];
			else if (eventData.pointerCurrentRaycast.gameObject.name == "Handle4")
				obj = charList[3];
			else if (eventData.pointerCurrentRaycast.gameObject.name == "Handle5")
				obj = charList[4];

			if (obj != null)
				obj.transform.Rotate(Vector3.up, -eventData.delta.x);
		}
	}
}