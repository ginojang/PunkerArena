using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
public class MenuUiController : BaseUiController<MenuUiController>
{
	private bool bCostume = false;
	private List<GameObject> charList = new List<GameObject>();
	private List<CharacterBase> playerList = new List<CharacterBase>();

	private bool bChangerDinoClass = false;
	private bool bChangerDinoParts = false;
	private string charUID = "";
	public GameObject GetMenuCharacter(int index)
	{
		return charList[index];
	}

	public int GetMenuCharacterCount()
	{
		return charList.Count;
	}
	
	protected override void OnCompletePreloadAsset()
    {
        GuiMain.Instance.Open<UiMenu>();

		AddGuiInitListener<UiMenu>(Initialize);

		//StartCoroutine(InitLobbyCharacter());

		//		AddGuiInitListener<UiMenu>(Initialize);
	}

	private void Initialize(GuiObject _target)
	{
		/*
		ClientNetworkContents.SendUsereData("test", "devil410", (respone) =>
		{
			if (respone == null)
			{
				// Popup으로 실패 했음을 알려주고 확인시 타이틀로 다시....
				//GuiUtility.ShowNoticeOkPopUp("음성인식을 3회 실패해서 강제로 넘어갑니다.", OnClickGoBetia);
				
				PlayState.Instance.ChangePlayState(PlayState.STATES.Title);
			}
			else
			{
				StartCoroutine(InitLobbyCharacter());
			}
		});
		*/
		
		StartCoroutine(InitLobbyCharacter());
		//UiInventory.Instance.Initialize(_target);
	}

	protected override void OnAwake()
	{
		base.OnAwake();
	}

	// Start is called before the first frame update
	void Start()
    {
        Messenger.AddListener(Definition.Event_ChangeText, OnClickedChangeText);

		Messenger.AddListener<int>(Definition.MENU_CHANGERANDOMCLASS, ChangeRandomClass);
		Messenger.AddListener<int>(Definition.MENU_CHANGERANDOMPARTS, ChangeRandomParts);
		Messenger.AddListener<int>(Definition.MENU_CHANGESAVE, SaveCharacter);
		Messenger.AddListener(Definition.MENU_CHANGERANDOMALL, ChangeAllRandom);
		Messenger.AddListener(Definition.CreateDino, CreateDino);

		GameDataManager.Instance.LoadCharacter();
	}

	IEnumerator InitInventoryCharacter()
	{
		for (int i = 0; i < 100; i++)
		{
			bCostume = true;
			Messenger.AddListener<int, GameObject>(Definition.Dino_Costume_End, FinishInventoryCostumeChar);

			var character = GameObject.Instantiate(GameDataManager.Instance.baseCharacter);
			CostumeManager.Instance.AddLobbyCostumeCharacter(character, i);

			yield return new WaitUntil(() => !bCostume);

			//Messenger.Broadcast(Definition.MENU_INVENTORY_SNAPSHOT, i, character);
		}
		
		//StartCoroutine(InitLobbyCharacter());
	}
	
    IEnumerator InitLobbyCharacter()
    {
        yield return new WaitUntil(() => GameDataManager.Instance.baseCharacter != null);

        var characterObj = GameDataManager.Instance.baseCharacter;
        var monsterObj = GameDataManager.Instance.baseMonster;

        StartCoroutine(SetPlayerCharacter());

		int rand = UnityEngine.Random.Range(0, 4);
		AssetManager.Instance.LoadAssetAsync<Sprite>($"Assets/GUI/Textures/background/{rand + 1}.png", (AssetLoader ld, object p) =>
		{
			UiMenu.Instance.bg.sprite = ld.MainAsset as Sprite;
		});
	}

	IEnumerator SetPlayerCharacter()
	{
		UserData.CharacterSquadData data = GameDataManager.Instance.userData.serverSquadDic[modeType.Action][0];

		int count = 0;
		
		foreach (var chardata in data.serverSquadList)
		{
			bCostume = true;
			Messenger.AddListener<int, GameObject>(Definition.Dino_Costume_End, FinishCostumeChar);
			var character = GameObject.Instantiate(GameDataManager.Instance.baseCharacter);
			
			charUID = chardata.Value.dinoID;
			
			CostumeManager.Instance.AddCostumeCharacter(character, count, chardata.Value.charClass, chardata.Value.talent, chardata.Value.partslist);
			
			yield return new WaitUntil(() => !bCostume);
			
			var characterbase = charList[count].GetComponent<Character>();
			playerList.Add(characterbase);
			characterbase.InitializeFSM();
			var talent = characterbase.CharacterInfo.costumeInfo.CURTALENT;

			Messenger.Broadcast(Definition.MENU_CHARACTER_LOADED, count, charList[count]);

			count++;
		}

		/*
		for (int i = 0; i < Definition.MAX_CHARACTER; i++)
		{
			bCostume = true;
			Messenger.AddListener<int, GameObject>(Definition.Dino_Costume_End, FinishCostumeChar);

			var character = GameObject.Instantiate(GameDataManager.Instance.baseCharacter);
			CostumeManager.Instance.AddLobbyCostumeCharacter(character, i);

			yield return new WaitUntil(() => !bCostume);

			var characterbase = charList[i].GetComponent<Character>();
			playerList.Add(characterbase);
			characterbase.InitializeFSM();
			var talent = characterbase.CharacterInfo.costumeInfo.CURTALENT;

			Messenger.Broadcast(Definition.MENU_CHARACTER_LOADED, i, charList[i]);
		}
		*/
	}

	private void FinishInventoryCostumeChar(int index, GameObject basechar)
	{
		Messenger.RemoveListener<int, GameObject>(Definition.Dino_Costume_End, FinishInventoryCostumeChar);

		Messenger.Broadcast(Definition.MENU_INVENTORY_SNAPSHOT, index, basechar);

		bCostume = false;
	}
	
	private void FinishCostumeChar(int index, GameObject basechar)
	{
		Messenger.RemoveListener<int, GameObject>(Definition.Dino_Costume_End, FinishCostumeChar);

		if (charList.Count < index + 1)
			charList.Add(basechar);
		else
			charList[index] = basechar;

		basechar.GetComponent<CharacterBase>().CharacterInfo.dinoID = charUID;
		
		Messenger.Broadcast(Definition.MENU_CHARACTER_CHANGE, index, charList[index]);

		bCostume = false;

		if (bChangerDinoParts)
		{
			SetNewDinoPartsInfo(index, basechar);
		}

		if (bChangerDinoClass)
		{
			SetNewDinoClassInfo(index, basechar);
		}
	}

	private void SetNewDinoPartsInfo(int index, GameObject newChar)
	{
		bChangerDinoParts = false;
		
		GameObject obj = GetMenuCharacter(index);
		Character charbase = obj.GetComponent<Character>();
		
		UserData.CharacterServerData data = GameDataManager.Instance.userData.serverSquadDic[modeType.Action][0].serverSquadList[charbase.CharacterInfo.dinoID];
		
		int count = charbase.CharacterInfo.costumeInfo.partList.Count;
		for(int i = 0; i < count; i++)
			data.partslist[(ItemType)i] = charbase.CharacterInfo.costumeInfo.partList[(ItemType)i].partName;
	}
	
	private void SetNewDinoClassInfo(int index, GameObject newChar)
	{
		bChangerDinoClass = false;
		
		GameObject obj = GetMenuCharacter(index);
		Character charbase = obj.GetComponent<Character>();
		
		UserData.CharacterServerData data = GameDataManager.Instance.userData.serverSquadDic[modeType.Action][0].serverSquadList[charbase.CharacterInfo.dinoID];

		int count = charbase.CharacterInfo.costumeInfo.partList.Count;
		for(int i = 0; i < count; i++)
			data.partslist[(ItemType)i] = charbase.CharacterInfo.costumeInfo.partList[(ItemType)i].partName;
	}
	
	private void Update()
	{
	}

	private void OnClickedChangeText()
    {
        UiMenu.Instance.ChangeText($"Change Text {UnityEngine.Random.Range(0, 1000)}");
    }

	public void ChangeRandomClass(int index)
	{
		Messenger.AddListener<int, GameObject>(Definition.Dino_Costume_End, FinishCostumeChar);

		GameObject obj = GetMenuCharacter(index);
		Character charbase = obj.GetComponent<Character>();
		
		UserData.CharacterServerData data = GameDataManager.Instance.userData.serverSquadDic[modeType.Action][0].serverSquadList[charbase.CharacterInfo.dinoID];

		charUID = data.dinoID;
		
		int count = charbase.CharacterInfo.costumeInfo.partList.Count;
		for(int i = 0; i < count; i++)
			data.partslist[(ItemType)i] = charbase.CharacterInfo.costumeInfo.partList[(ItemType)i].partName;

		CostumeManager.Instance.RandomChangeClass(obj, charbase.CharacterInfo.costumeInfo.CURTALENT, ItemType.body);
		
		bChangerDinoClass = true;
	}

	public void ChangeRandomParts(int index)
	{
		Messenger.AddListener<int, GameObject>(Definition.Dino_Costume_End, FinishCostumeChar);

		GameObject obj = GetMenuCharacter(index);
		Character charbase = obj.GetComponent<Character>();
		
		UserData.CharacterServerData data = GameDataManager.Instance.userData.serverSquadDic[modeType.Action][0].serverSquadList[charbase.CharacterInfo.dinoID];
		
		charUID = data.dinoID;
		
		int count = charbase.CharacterInfo.costumeInfo.partList.Count;
		for(int i = 0; i < count; i++)
			data.partslist[(ItemType)i] = charbase.CharacterInfo.costumeInfo.partList[(ItemType)i].partName;

		CostumeManager.Instance.RandomChangeNoBody(obj);
		
		bChangerDinoParts = true;
	}

	IEnumerator ChangeCharacterAllRandom()
	{
		for (int i = 0; i < Definition.MAX_CHARACTER; i++)
		{
			bCostume = true;
			Messenger.AddListener<int, GameObject>(Definition.Dino_Costume_End, FinishCostumeChar);

			var character = charList[i];
			var characterbase = character.GetComponent<CharacterBase>();
			UserData.CharacterServerData data = GameDataManager.Instance.userData.serverSquadDic[modeType.Action][0].serverSquadList[characterbase.CharacterInfo.dinoID];
		
			charUID = data.dinoID;
		
			int count = characterbase.CharacterInfo.costumeInfo.partList.Count;
			for(int j = 0; j < count; j++)
				data.partslist[(ItemType)j] = characterbase.CharacterInfo.costumeInfo.partList[(ItemType)j].partName;
			
			bChangerDinoParts = true;
			bChangerDinoClass = true;
			CostumeManager.Instance.AddLobbyCostumeCharacter(character, i);

			yield return new WaitUntil(() => !bCostume);

			//charList.Add(charList[i]);
			characterbase = charList[i].GetComponent<CharacterBase>();
			data.talent = characterbase.CharacterInfo.Character_Talent;

			if (playerList.Count < i + 1)
				playerList.Add(characterbase);
			else
				playerList[i] = characterbase;

//			characterbase.InitializeFSM();
		}
	}
	
	public void SaveCharacter(int index)
	{
		GameObject obj = GetMenuCharacter(index);
		Character charbase = obj.GetComponent<Character>();

		UserData.CharacterServerData data = GameDataManager.Instance.userData.serverSquadDic[modeType.Action][0].serverSquadList[charbase.CharacterInfo.dinoID];
		
		ClientNetworkContents.SendSetCharacter(data, (response) => 
		{
			UnityEngine.Debug.Log($"Saved {index+1} Character");
		});
	}
/*
	public void SaveAllCharacter()
	{
		List<UserData.CharacterServerData> datalist = new List<UserData.CharacterServerData>();
		
		for (int i = 0; i < GetMenuCharacterCount(); i++)
		{
			GameObject obj = GetMenuCharacter(i);
			Character charbase = obj.GetComponent<Character>();

			UserData.CharacterServerData data = GameDataManager.Instance.userData.serverSquadDic[modeType.Action][0]
				.serverSquadList[charbase.CharacterInfo.dinoID];
			
			datalist.Add(data);
		}

		ClientNetworkContents.SendSetCharacter(datalist, (response) =>
		{
			UnityEngine.Debug.Log($"Saved All Character");
		});
	}
*/
	public void ChangeAllRandom()
	{
		StartCoroutine(ChangeCharacterAllRandom());
	}

	public void CreateDino()
	{
		ClientNetworkContents.SendDevDinoGacha(1, (genid) =>
		{
			Messenger.Broadcast(Definition.CreateDinoSetUI, genid);
		});
	}
	
    private void OnDestroy()
    {
/*		GameDataManager.Instance.userData.lastDatas.UserCharacterData.Clear();
		for (int i = 0; i < playerList.Count; i++)
		{
			GameDataManager.Instance.userData.lastDatas.SetCharacterData(playerList[i]);
		}
*/
		Messenger.RemoveListener<int>(Definition.MENU_CHANGERANDOMCLASS, ChangeRandomClass);
		Messenger.RemoveListener<int>(Definition.MENU_CHANGERANDOMPARTS, ChangeRandomParts);
		Messenger.RemoveListener(Definition.MENU_CHANGERANDOMALL, ChangeAllRandom);
	}
}
