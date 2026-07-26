using Generated.CsvData;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
//using System;
//using System.Reflection;
//using System.Text.RegularExpressions;
// 코스튬을 요청한 곳에서 메시지를 받기 위해 구분자 정도로 사용됨
public enum costumeType
{
	costumeType_Tool = 0,
	costumeType_Hud,
	costumeType_Lobby,
}

public class loadDataInfo
{
	public loadDataInfo(string full, string part, ItemType itemType)
	{
		fullname = full;
		partname = part;
		type = itemType;
	}

	public string fullname;
	public string partname;
	public ItemType type;
}

public class costumePartsInfo
{
    public costumePartsInfo(Transform partobj, string part, List<Texture2D> texlist)
    {
        obj = partobj;
        textureList = texlist;
		partName = part;
    }

    public Transform obj;
    public List<Texture2D> textureList;
	public string partName;
}

public class costumCharInfo
{
	public int index;
	private bool bEquiped = false;
	private string charId;
	private bool isAttack = false;
	private GameObject model_object = null;

	private GameObject basicMouth = null;
	private GameObject ahMouth = null;

	private CharacterTalent curTalent = CharacterTalent.None;
	private CharacterClass curClass = CharacterClass.None;

	private Dictionary<ItemType, costumePartsInfo> nowPartsTransform = new Dictionary<ItemType, costumePartsInfo>();
	public string charID
	{
		get { return charId; }
		set { charId = value; }
	}

	public bool ATTACK
	{
		get { return isAttack; }
		set { isAttack = value; }
	}

	public CharacterTalent CURTALENT
	{
		get { return curTalent; }
		set { curTalent = value; }
	}

	public CharacterClass CURCLASS
	{
		get { return curClass; }
		set { curClass = value; }
	}

	public bool Equip
	{
		get { return bEquiped; }
		set { bEquiped = value; }
	}
	public GameObject model
	{
		get { return model_object; }
		set { model_object = value; }
	}
	public Dictionary<ItemType, costumePartsInfo> partList
	{
		get { return nowPartsTransform; }
		set { nowPartsTransform = value; }
	}

	public GameObject Mouth
	{
		get { return basicMouth; }
		set { basicMouth = value; }
	}

	public GameObject AHMouth
	{
		get { return ahMouth; }
		set { ahMouth = value; }
	}

	public costumCharInfo(CharacterTalent charTalent, CharacterClass charClass, Transform baseBody)
	{
		Initialize(charTalent, charClass, baseBody);
	}

	public void SetPartsObject(ItemType part, string partname, Transform obj)
	{
		if (partList.ContainsKey(part))
		{
			partList[part].obj = obj;
			partList[part].partName = partname;
		}
		else
			partList.Add(part, new costumePartsInfo(obj, partname, null));
	}

	public void Initialize(CharacterTalent charTalent, CharacterClass charClass, Transform baseBody)
	{
		// CostumableMgr Initialize 자체에서 베이스 캐릭터들을 로드하기 때문에 따로 로드 안해도 될듯
		//		if (baseChar == null)
		//			baseChar = CostumeManager.Instance.GetBaseChar(charTalent, charClass);

		curTalent = charTalent;
		curClass = charClass;

		if (baseBody)
		{
			nowPartsTransform.Add(ItemType.body, new costumePartsInfo(baseBody, "ca_orange_01_body", null));
			nowPartsTransform.Add(ItemType.headparts, new costumePartsInfo(baseBody, "ca_orange_01_headparts", null));
			nowPartsTransform.Add(ItemType.eyes, new costumePartsInfo(baseBody, "ca_orange_01_eyes", null));
			nowPartsTransform.Add(ItemType.mouth, new costumePartsInfo(baseBody, "ca_orange_01_mouth", null));
			nowPartsTransform.Add(ItemType.back, new costumePartsInfo(baseBody, "ca_orange_01_back", null));
			nowPartsTransform.Add(ItemType.tail, new costumePartsInfo(baseBody, "ca_orange_01_tail", null));
//			nowPartsTransform.Add(ItemType.wing, new costumePartsInfo(baseBody, "ca_banana_01_wing", null));
//			nowPartsTransform.Add(ItemType.pattern, new costumePartsInfo(baseBody, "ca_banana_01_pattern", null));
		}
	}


	private void FinishLoadPlayer(AssetLoader ld, object p)
	{
		if (ld.IsLoadSucceed == false)
			return;

		// 로드된 케릭터를 게임오브젝트로 생성 및 플레이어에 저장
		var model_prefab = ld.MainAsset as GameObject;
	}
}

[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]
public class CostumeManager : MonoBehaviour
{
	public static CostumeManager instance_value;

	public static CostumeManager Instance
	{
		get
		{
			if (instance_value == null)
			{
				GameObject go = new GameObject("CostumeManager");
				instance_value = go.AddComponent<CostumeManager>();
				ImmortalGameObject.AttachObject(go);
			}

			return instance_value;
		}
	}

	bool doCostume = false; // 현재 코스튬 진행중 여부

	private bool isInitialized = false;
	costumCharInfo changeChar = null;
	//Dictionary<int, costumCharInfo> changeCharList = new Dictionary<int, costumCharInfo>();
	//Dictionary<int, costumCharInfo> changedCharList = new Dictionary<int, costumCharInfo>();

	// 베이스 캐릭터들의 오브젝트 풀
	private Dictionary<string, GameObject> dinoBasePool = new Dictionary<string, GameObject>();
	private string baseBodyname = "";
	private GameObject baseChar = null;
	private string baseBodyPartname = "orange_01_body";

	private Dictionary<int, Generated.CsvData.partsData> curClassPartData = new Dictionary<int, Generated.CsvData.partsData>();

	public bool DOCOSTUME
	{
		get { return doCostume; }
		set { doCostume = value; }
	}

	public void Clear()
	{
		//changeCharList.Clear();
	}

	public GameObject GetBaseChar()
	{
		GameObject returnObj = null;

		if (dinoBasePool.ContainsKey(baseBodyname))
			returnObj = dinoBasePool[baseBodyname];
		//switch()
		return returnObj;
	}
	/// <summary>
	/// 
	/// </summary>
	/// 
	public Dictionary<string, GameObject> objectList = new Dictionary<string, GameObject>();

	public void Initialize()
	{
		if (isInitialized == false)
		{
			/*			List<object> param = new List<object>();
						AssetLoader girlLoader = AssetManager.Instance.PreLoadAsset(Definition.PcElliePrefabPath, null);
						AssetLoader boyLoader = AssetManager.Instance.PreLoadAsset(Definition.PcJimmyPrefabPath, null);
						AssetLoader girlMyHomeLoader = AssetManager.Instance.PreLoadAsset(Definition.PcElliePrefabPathMyHome, null);
						AssetLoader boyMyHomeLoader = AssetManager.Instance.PreLoadAsset(Definition.PcJimmyPrefabPathMyHome, null);

						List<AssetLoader> baseCharList = new List<AssetLoader>();
						baseCharList.Add(girlLoader);
						baseCharList.Add(boyLoader);
						baseCharList.Add(girlMyHomeLoader);
						baseCharList.Add(boyMyHomeLoader);
						param.Add(girlLoader);
						param.Add(boyLoader);
						param.Add(girlMyHomeLoader);
						param.Add(boyMyHomeLoader);
						AssetManager.Instance.LoadAssetAsyncForPreloadAsset(baseCharList, FinishLoadBasePlayer, param);

						// load default home deco object
						LoadDefaultHomeDecoObject();

						Messenger.AddListener<int>(Definition.INVENTORY_EVENT_COSTUME_EQUIP, OnEquipItem);
						Messenger.AddListener<int>(Definition.INVENTORY_EVENT_COSTUME_UNEQUIP, OnUnEquipItem);

						isInitialized = true;*/
		}
	}

	public void AddCostumeMonster(GameObject character, int idx, CharacterClass charclass, CharacterTalent talent, 
		int monsterIndex, Dictionary<ItemType, string> itemlist)
	{
		costumCharInfo info = new costumCharInfo(CharacterTalent.Carnivore, CharacterClass.orange, character.transform);
		
		info.CURTALENT = talent;
		info.CURCLASS = charclass;
		info.model = character;
		info.index = idx;

		ChangeDinoPartsList(info, talent, charclass, monsterIndex, itemlist);
	}
	
	public void AddLobbyCostumeCharacter(GameObject character, int index)
	{
		costumCharInfo info = new costumCharInfo(CharacterTalent.Carnivore, CharacterClass.orange, character.transform);
		info.CURTALENT = CharacterTalent.Carnivore;
		info.CURCLASS = CharacterClass.orange;
		info.model = character;
		info.index = index;

		RandomChangeDinoPartsList(info);
	}

	public void AddCostumeCharacter(GameObject character, int idx, CharacterClass charclass, CharacterTalent talent, Dictionary<ItemType, string> itemlist)
	{
		costumCharInfo info = new costumCharInfo(CharacterTalent.Carnivore, CharacterClass.orange, character.transform);
		
		info.CURTALENT = talent;
		info.CURCLASS = charclass;
		info.model = character;
		info.index = idx;

		ChangeDinoPartsList(info, talent, charclass, -1, itemlist);
	}
	
	public void AddCostoumCharacter(GameObject character, int index)
	{
		Character charbase = character.GetComponent<Character>();

		costumCharInfo info = new costumCharInfo(charbase.CharacterInfo.costumeInfo.CURTALENT, charbase.CharacterInfo.costumeInfo.CURCLASS, character.transform);
		info.partList = character.GetComponent<Character>().CharacterInfo.costumeInfo.partList;
		info.model = character;
		info.index = index;

		ChangeDinoPartsList(character.GetComponent<Character>().CharacterInfo.costumeInfo, info.CURTALENT, info.CURCLASS, -1, charbase.CharacterInfo.partslist);

		// 현재 서버 정보가 없으니까 강제 랜덤으로 실행
		//RandomChangeDinoPartsList(info);
	}
	public void AddCostoumCharacter(costumCharInfo info, Dictionary<ItemType, string> partsList)
	{ 
		ChangeDinoPartsList(info, info.CURTALENT, info.CURCLASS, -1, partsList);
    }

	public void ChangeDinoAttackMouth(GameObject characterObj, bool bAttack = false) // true 면 공격용 입으로, false 면 원래 입으로..
	{
		Character character = characterObj.GetComponent<Character>();

		costumCharInfo info = character.CharacterInfo.costumeInfo;

		CharacterTalent talent = info.CURTALENT;

		CharacterClass charClass = info.CURCLASS;

		string mouthname = info.partList[ItemType.mouth].partName;

		if(bAttack)
		{
			mouthname += "_AH";
		}
		else
		{
			mouthname = mouthname.Replace("_AH","");
		}

		info.partList[ItemType.mouth].partName = mouthname;

		ChangeDinoPart(characterObj, talent, charClass, mouthname, ItemType.mouth);
	}

	public void ChangeDinoPart(GameObject character, CharacterTalent talent, CharacterClass charClass, string item, ItemType type)
	{

		DOCOSTUME = true;

		changeChar.model = character;
		string itemname = GetFilenameByPartsData(talent, item);
		List<aniTableData> aniList = new List<aniTableData>();
		Dictionary<ItemType, string> itemlist = new Dictionary<ItemType, string>();

		//string itemname = $"{baseFilePath}Character3.prefab";


		// 리소스 로드
		List<AssetLoader> addressableKeyList = new List<AssetLoader>();
/*
		// 리소스풀에 있는 데이터들
		List<loadDataInfo> resourceExist = new List<loadDataInfo>();
		List<loadDataInfo> resourceNotExist = new List<loadDataInfo>();

		if (dinoBasePool.ContainsKey(itemname))
		{
			resourceExist.Add(new loadDataInfo(itemname, item, type));
		}
		else
		{
			AssetLoader objLoader = AssetManager.Instance.PreLoadAsset(itemname, null);
			addressableKeyList.Add(objLoader);
			resourceNotExist.Add(new loadDataInfo(itemname, item, type));
		}

		if(item.Contains("body"))
			baseBodyname = itemname;

		List<object> param = new List<object>();
		param.Add(talent);
		param.Add(charClass);
		param.Add(resourceExist);
		param.Add(resourceNotExist);
		param.Add(addressableKeyList);
		AssetManager.Instance.LoadAssetAsyncForPreloadAsset(addressableKeyList, FinishLoadPartsObject, param);*/

		List<loadDataInfo> resourceExist = new List<loadDataInfo>();
		List<loadDataInfo> resourceNotExist = new List<loadDataInfo>();
		List<object> param = new List<object>();
		List<objectType> typelist = new List<objectType>();
		
		if (ResourcePoolManager.Instance.IsContainsData(itemname))
		{
			resourceExist.Add(new loadDataInfo(itemname, item, type));
		}
		else
		{
			AssetLoader objLoader = AssetManager.Instance.PreLoadAsset(itemname, null);
			addressableKeyList.Add(objLoader);
			resourceNotExist.Add(new loadDataInfo(itemname, item, type));
			typelist.Add(objectType.character);
		}

		// 필요한 애니 리스트 생성
		itemlist.Add(type, item);
		CreateAniList(aniList, talent, charClass, -1, itemlist);

		param.Add(typelist);
		param.Add(talent);
		param.Add(charClass);
		param.Add(resourceExist);
		param.Add(resourceNotExist);
		param.Add(aniList);
		//		param.Add(addressableKeyList);

		ResourcePoolManager.Instance.AsyncLoadDataList(addressableKeyList, param, false, FinishLoadPartObjectList);
	}

	private void FinishLoadPartsObject(object p = null)
	{
		List<object> param = (List<object>)p;
		CharacterTalent talent = (CharacterTalent)(param[0]);
		CharacterClass charClass = (CharacterClass)(param[1]);
		List<loadDataInfo> resourceExist = (List<loadDataInfo>)(param[2]);
		List<loadDataInfo> resourceNotExist = (List<loadDataInfo>)(param[3]);
		List<AssetLoader> loaderList = (List<AssetLoader>)param[4];

		int count = 0;
		foreach (var status in resourceExist)
		{
			GameObject obj = null;

			obj = dinoBasePool[status.fullname]; // 파츠오브젝트
			changeChar.SetPartsObject(status.type, status.partname, obj.transform);
		}

		foreach(var status in resourceNotExist)
		{
			GameObject obj = null;
			// 로드된 케릭터를 게임오브젝트로 생성 및 플레이어에 저장
			obj = loaderList[count].MainAsset as GameObject; // 파츠오브젝트
			if (obj == null)
			{
				return;
			}

			if (status.type == ItemType.body)
				baseChar = obj;

			changeChar.SetPartsObject(status.type, status.partname, obj.transform);

			if(!dinoBasePool.ContainsKey(status.fullname))
				dinoBasePool.Add(status.fullname, obj);

			count++;
		}

		if (resourceExist.Count > 0 || resourceNotExist.Count > 0)
		{
			costumCharInfo info = changeChar;
			info.CURTALENT = talent;
			info.CURCLASS = charClass;
			DoCostume(ref info);
		}
	}

	public void ChangeDinoPartsList(costumCharInfo character, CharacterTalent talent, CharacterClass charClass, int monsterIndex, Dictionary<ItemType, string> itemlist)
	{
		DOCOSTUME = true;

		changeChar = character;// new costumCharInfo(talent, charClass, baseChar.transform);
		//changeChar.model = character;

		List<AssetLoader> addressableKeyList = new List<AssetLoader>();
		List<loadDataInfo> resourceExist = new List<loadDataInfo>();
		List<loadDataInfo> resourceNotExist = new List<loadDataInfo>();
		List<object> param = new List<object>();
		List<objectType> typelist = new List<objectType>();
		List<aniTableData> aniList = new List<aniTableData>();

		foreach (var item in itemlist)
		{
			if (item.Value == "" || item.Value == "none")
				continue;

			string itemname = GetFilenameByPartsData(talent, item.Value);

			if (ResourcePoolManager.Instance.IsContainsData(itemname))
			{
				resourceExist.Add(new loadDataInfo(itemname, item.Value,item.Key));
			}
			else
			{
				AssetLoader objLoader = AssetManager.Instance.PreLoadAsset(itemname, null);
				addressableKeyList.Add(objLoader);
				resourceNotExist.Add(new loadDataInfo(itemname, item.Value, item.Key));
				typelist.Add(objectType.character);
			}
		}

		// 필요한 애니 리스트 생성
		CreateAniList(aniList, talent, charClass, monsterIndex, itemlist);

		// 생성한 리스트로 로드 요청
		foreach (var element in aniList)
		{
			if (ResourcePoolManager.Instance.IsContainsData(element.res_ani) == true)
				continue;

			AssetLoader objLoader = AssetManager.Instance.PreLoadAsset(element.res_ani, null);
			addressableKeyList.Add(objLoader);
			typelist.Add(objectType.aniClip);
		}

		param.Add(typelist);
		param.Add(talent);
		param.Add(charClass);
		param.Add(resourceExist);
		param.Add(resourceNotExist);
		param.Add(aniList);
		//		param.Add(addressableKeyList);

		ResourcePoolManager.Instance.AsyncLoadDataList(addressableKeyList, param, false, FinishLoadPartObjectList);
	}

	private void FinishLoadPartObjectList(object p)
	{
		List<object> param = (List<object>)p;
		CharacterTalent talent = (CharacterTalent)(param[1]);
		CharacterClass charClass = (CharacterClass)(param[2]);
		List<loadDataInfo> resourceExist = (List<loadDataInfo>)(param[3]);
		List<loadDataInfo> resourceNotExist = (List<loadDataInfo>)(param[4]);
		List<aniTableData> aniList = (List<aniTableData>)(param[5]);

		int count = 0;
		foreach (var status in resourceExist)
		{
			GameObject obj = null;

			obj = (GameObject)ResourcePoolManager.Instance.GetResourceData(status.fullname); // 파츠오브젝트
			changeChar.SetPartsObject(status.type, status.partname, obj.transform);
		}

		foreach (var status in resourceNotExist)
		{
			GameObject obj = null;
			// 로드된 케릭터를 게임오브젝트로 생성 및 플레이어에 저장
			obj = (GameObject)ResourcePoolManager.Instance.GetResourceData(status.fullname); // 파츠오브젝트
			
			if (obj == null)
			{
				return;
			}

			changeChar.SetPartsObject(status.type, status.partname, obj.transform);

			count++;
		}

		changeChar.CURTALENT = talent;
		changeChar.CURCLASS = charClass;

		if (resourceExist.Count > 0 || resourceNotExist.Count > 0)
			DoCostume(ref changeChar);

		SetAnimatorOverride(changeChar, aniList);

		Messenger.Broadcast<int, GameObject>(Definition.Dino_Costume_End, changeChar.index, changeChar.model);
	}

	private void CreateAniList(List<aniTableData> result, CharacterTalent talent, CharacterClass charClass, int monsterIndex, Dictionary<ItemType, string> itemlist)
	{
		bool isWing = false;
		string wingValue = string.Empty;
		List<SkillData> skilDataList = new List<SkillData>();
		aniTableData aniData = null;
		aniTableData runData = null;

		// 날개 체크
		itemlist.TryGetValue(ItemType.wing, out wingValue);
		if (wingValue == null || wingValue == "" || wingValue == "none")
			isWing = false;
		else
			isWing = true;

		// 기본 필수 애니데이터 로드
		foreach (EDefaultAniType element in System.Enum.GetValues(typeof(EDefaultAniType)))
		{
			aniData = CSVDataManager.GetTable<AniTable>().GetData((int)element, talent, isWing);


			if (aniData == null)
				Debug.LogError($"AniTable 없음 : Index={(int)element}, talent={talent}, isWing={isWing}");

			result.Add(aniData);
		}

		// 로드할 스킬데이터를 가져온다.
		CSVDataManager.GetTable<SkillTable>().GetDataList(ref skilDataList, monsterIndex, talent, itemlist);

		// 스킬데이터 기반으로 애니를 찾차아서 로드할 리스트에 추가한다.
		foreach (var element in skilDataList)
		{
			aniData = CSVDataManager.GetTable<AniTable>().GetData(element.res_ani_attack, talent, isWing);
			//runData = CSVDataManager.GetTable<AniTable>().GetData(element.res_ani_attack, talent, isWing);
			if(element.res_ani_move_action > 0)
				runData = CSVDataManager.GetTable<AniTable>().GetData(element.res_ani_move_action, talent, isWing);
	
			aniTableData data = new aniTableData();
			if (aniData == null)
				Debug.LogError($"AniTable 없음 : Index={element.res_ani_attack}, talent={talent}, isWing={isWing}");

			result.Add(aniData);
	
		}

		foreach(var element in skilDataList)
        {
			if (element.res_ani_move_action > 0)
            {
				runData = CSVDataManager.GetTable<AniTable>().GetData(element.res_ani_move_action, talent, isWing);
				for(int i = result.Count- 1; i >= 0; i--)
                {
                    if(result[i].state == runData.state)
                    {
						result.RemoveAt(i);
						result.Insert(i, runData);
                    }
                }
            }
		}
	}

	public void LoadBaseCharacter(string fullname, CharacterTalent talent, CharacterClass charClass, Dictionary<ItemType, string> partlist = null, Transform parent = null, bool bTool = false)
	{
		ResourcePoolManager.Instance.AsyncLoadData(fullname, objectType.character, (data) =>
		{
			GameObject obj = (GameObject)data;
			GameDataManager.Instance.baseCharacter = obj;
			GameDataManager.Instance.baseMonster = obj;

			if (bTool)
			{
				GameObject dino = Instantiate(obj);
				
				if(parent != null)
					dino.transform.SetParent(parent);
				
				dino.transform.localPosition = Vector3.zero;
				dino.transform.localRotation = Quaternion.identity;
				baseChar = obj;
				Messenger.Broadcast<int, GameObject>(Definition.Dino_Costume_End, 0, dino);

				if(!dinoBasePool.ContainsKey(fullname))
					dinoBasePool.Add(fullname, obj);
				baseBodyname = fullname;

				if (partlist != null)
				{
					costumCharInfo info = new costumCharInfo(talent, charClass, dino.transform);
					info.CURTALENT = talent;
					info.CURCLASS = charClass;
					info.model = dino;
					ChangeDinoPartsList(info, talent, charClass, -1, partlist);
				}
			}
		});
/*		AssetManager.Instance.LoadAssetAsync<GameObject>(fullname, (AssetLoader ld, object p) =>
		{
			GameObject obj = ld.MainAsset as GameObject;
			GameDataManager.Instance.baseCharacter = obj;
			GameDataManager.Instance.baseMonster = obj;

			if (bTool)
			{
				GameObject dino = Instantiate(obj);
				
				if(parent != null)
					dino.transform.SetParent(parent);
				
				dino.transform.localPosition = Vector3.zero;
				dino.transform.localRotation = Quaternion.identity;
				baseChar = obj;
				Messenger.Broadcast<int, GameObject>(Definition.Dino_Costume_End, 0, dino);

				if(!dinoBasePool.ContainsKey(fullname))
					dinoBasePool.Add(fullname, obj);
				baseBodyname = fullname;

				if (partlist != null)
				{
					costumCharInfo info = new costumCharInfo(talent, charClass, dino.transform);
					info.CURTALENT = talent;
					info.CURCLASS = charClass;
					info.model = dino;
					ChangeDinoPartsList(info, talent, charClass, partlist);
				}
			}
		});*/
	}

	public string GetFilenameByPartsData(CharacterTalent talent, string item)
	{
		string filename = "";

		string[] itemname = item.Split('_');
		//filename += Definition.baseFilePath;
		//filename += $"{itemname[1]}/prefab/";
		filename += $"{itemname[0]}_";
		filename += $"{itemname[1]}_";
		filename += itemname[2];
		//filename += ".prefab";

		return filename;
	}

	// 순수하게 테스트용으로 사용될 함수
	// 실제는 서버에서 받아온 파츠 정보를 이용해서 교체 작업을 하면 되는데
	// 현재는 서버 데이터가 없는 관계로 완전 무작위로 랜덤값을 생성하여
	// 교체를 해준다.
	public void RandomChangeDinoPartsList(costumCharInfo info)
	{
		RandomSelectCharacter(info);
	}

	protected void RandomSelectCharacter(costumCharInfo info)
	{
		CharacterBase charbase = info.model.GetComponent<CharacterBase>();

		CharacterTalent talent = (CharacterTalent)UnityEngine.Random.Range(1, (int)CharacterTalent.Max);
		CharacterClass curClass = CharacterClass.orange;

		bool bDragon = false;
		
		if (Random.Range(0, 100) > 96) // test로 확률 4%로 용과 등장
			bDragon = true;

		bDragon = false;
		
		if (bDragon)
		{
			curClass = CharacterClass.dragonfruit;
			
			for (int i = 0; i < (int) ItemType.Max; i++)
			{
				if ((ItemType) i == ItemType.pattern)
					continue;
				
				SetSelectedPart(charbase, talent, curClass, (ItemType)i, 0, true);
			}
		}
		else
		{
			for (int i = 0; i < (int) ItemType.Max; i++)
			{
				if ((ItemType) i == ItemType.wing || (ItemType) i == ItemType.pattern)
					continue;

				CharacterClass charclass = CharacterClass.None;
				int index = -1;

				Dictionary<int, Generated.CsvData.partsData> list =
					CSVDataManager.GetRandomPartItem((ItemType) i, ref charclass, ref index);

				ItemType curpart = (ItemType) i;

				if (curpart == ItemType.body)
					curClass = charclass;

				if (list == null)
				{
					Debug.LogError($"선택된 {(ItemType) i} 타입 데이터가 없음");
					continue;
				}

				if (!list[index].partList.ContainsKey((ItemType) i))
					continue;

				//charbase.CharacterInfo.partslist[ItemType.body] = list[index].partList[curpart].fileName;

				SetSelectedPart(charbase, talent, charclass, curpart, index, true);
			}
		}

		ChangeDinoPartsList(info, talent, curClass, -1, charbase.CharacterInfo.partslist);
	}

	public void RandomChangeClass(GameObject character, CharacterTalent talent, ItemType type)
	{
		CharacterBase characterBase = character.GetComponent<CharacterBase>();

		CharacterClass charclass = CharacterClass.None;

		int index = -1;

		Dictionary<int, Generated.CsvData.partsData> list = CSVDataManager.GetRandomPartItem(type, ref charclass, ref index);

		if (list == null)
		{
			Debug.LogError($"선택된 {type} 타입 데이터가 없음");
			return;
		}

		if (!list[index].partList.ContainsKey(type))
			return;

		SetSelectedPart(characterBase, talent, charclass, type, index, true);

		ChangeDinoPartsList(characterBase.CharacterInfo.costumeInfo, talent, charclass, -1, characterBase.CharacterInfo.partslist);
	}

	public void RandomChangeNoBody(GameObject character)
	{
		CharacterBase charbase = character.GetComponent<CharacterBase>();
		costumCharInfo charcostumeInfo = charbase.CharacterInfo.costumeInfo;

		CharacterTalent talent = charcostumeInfo.CURTALENT;
		CharacterClass curClass = charcostumeInfo.CURCLASS;

		bool bDragon = false;
		
		if (Random.Range(0, 100) > 50) // test로 확률 4%로 용과 등장
			bDragon = true;

		//일단 용과는 인게임에 나오기 위해선 UI 등 수정해야하는 부분이 많은 관계로
		//새로 UI가 정립 되기전엔 나오지 않도록 막아둠
		bDragon = false;
		
		if (bDragon)
		{
			curClass = CharacterClass.dragonfruit;
			
			for (int i = 0; i < (int) ItemType.Max; i++)
			{
				if ((ItemType) i == ItemType.pattern || (ItemType) i == ItemType.body)
					continue;
				
				SetSelectedPart(charbase, talent, curClass, (ItemType)i, 0, true);
			}
		}
		else
		{
			for (int i = 0; i < (int) ItemType.Max; i++)
			{
				if ((ItemType) i == ItemType.wing || (ItemType) i == ItemType.pattern || (ItemType) i == ItemType.body)
					continue;

				CharacterClass charclass = CharacterClass.None;
				int index = -1;

				Dictionary<int, Generated.CsvData.partsData> list =
					CSVDataManager.GetRandomPartItem((ItemType) i, ref charclass, ref index);

				if (list == null)
				{
					Debug.LogError($"선택된 {(ItemType) i} 타입 데이터가 없음");
					continue;
				}

				if (!list[index].partList.ContainsKey((ItemType) i))
					continue;

				ItemType curpart = (ItemType) i;

				if (curpart == ItemType.body)
					curClass = charclass;
				//charbase.CharacterInfo.partslist[ItemType.body] = list[index].partList[curpart].fileName;

				SetSelectedPart(charbase, talent, charclass, curpart, index, true);
			}
		}

		ChangeDinoPartsList(charbase.CharacterInfo.costumeInfo, talent, curClass, -1, charbase.CharacterInfo.partslist);
	}

	public void SetSelectedPart(CharacterBase charbase, CharacterTalent talent, CharacterClass charclass, ItemType type, int idx, bool random = false)
	{
		curClassPartData = CSVDataManager.GetClassPartInfo(charclass);

		int count = 0;

		foreach (var data in curClassPartData)
		{
			string prefix = "";

			if (count == idx)
			{
				switch (talent)
				{
					case CharacterTalent.Carnivore:
						{
							prefix += "ca_";
						}
						break;
					case CharacterTalent.Omnivore:
						{
							prefix += "om_";
						}
						break;
					case CharacterTalent.Herbivore:
						{
							prefix += "he_";
						}
						break;
				}

				if (!charbase.CharacterInfo.partslist.ContainsKey(type))
					charbase.CharacterInfo.partslist.Add(type, "");

				charbase.CharacterInfo.Character_Talent = talent;

				string partname = "";
				if (type == ItemType.body)
				{
					if (data.Value.partList[type].fileName == "none")
						partname = prefix + baseBodyPartname;
					else
						partname = prefix + data.Value.partList[type].fileName;
				}
				else
					partname = prefix + data.Value.partList[type].fileName;

				charbase.CharacterInfo.partslist[type] = partname;

				break;
			}
			count++;
		}
	}

	private void SetAnimatorOverride(costumCharInfo character, List<aniTableData> aniList)
	{
		if (aniList.Count == 0)
			return;

		CharacterBase charBase = character.model.GetComponent<CharacterBase>();
		AnimatorOverrideController overrideAnimator = charBase.OverrideAnimator;

		if (overrideAnimator == null)
			overrideAnimator = new AnimatorOverrideController(charBase.characterAnimator.runtimeAnimatorController);

		AnimationClipOverrides clipOverrides = new AnimationClipOverrides(overrideAnimator.overridesCount);
		overrideAnimator.GetOverrides(clipOverrides);
		overrideAnimator.name = "animator_override";

		CharacterTalent talent = character.CURTALENT;
		bool isWing = false;
		costumePartsInfo wingValue = null;
		character.partList.TryGetValue(ItemType.wing, out wingValue);
		if (wingValue == null || wingValue.partName == "" || wingValue.partName == "none")
			isWing = false;
		else
			isWing = true;

		// 기본 필수 오버라이드
		foreach (EDefaultAniType element in System.Enum.GetValues(typeof(EDefaultAniType)))
			Utility.SetAnimatorOverrideData(clipOverrides, (int)element, talent, isWing);

		//// 스킬에 따른 애니 오버라이드
		foreach( var element in aniList )
        {
			Utility.SetAnimatorOverrideData(clipOverrides, element.idx, talent, isWing);

        }

		charBase.SetAnimatorOverride(overrideAnimator, clipOverrides);
		charBase.characterAnimator.SetTrigger("test");
	}

	public SkinnedMeshRenderer mouthMesh = null;
	public SkinnedMeshRenderer mouthAHMesh = null;
	void DoCostume(ref costumCharInfo character)
	{
		bool bExistAHMouth = true;
		List<CombineInstance> combine_instances = new List<CombineInstance>();
		List<Material> materials = new List<Material>();
		List<Transform> bones = new List<Transform>();

		List<Transform> meshList = null;
		MultiDictionary<string, Transform> basepartsdictMesh = new MultiDictionary<string, Transform>();
		Material bodyMat = null;

		GameObject root = null;
		GameObject baseBody = character.partList[ItemType.body].obj.gameObject;
		root = Instantiate(baseBody.gameObject) as GameObject;
		root.name = character.model.name;
		root.transform.position = character.model.transform.position;
		root.transform.rotation = character.model.transform.rotation;
		Transform[] transforms = root.GetComponentsInChildren<Transform>();
		MultiDictionary<string, Transform> transform_dictionary = new MultiDictionary<string, Transform>();
		for (int o = 0; o < transforms.Length; o++)
		{
			if (transform_dictionary.ContainsKey(transforms[o].name))
				continue;

			transform_dictionary.Add(transforms[o].name, transforms[o]);
		}

		foreach (var part in character.partList) 
		{
			basepartsdictMesh.Clear();

			if (part.Value.partName == null)
				continue;

			string partname = "";
			partname = part.Value.partName;

			Transform parts = part.Value.obj.Find(partname);
			if (parts == null)
			{
				Debug.LogError($"{part.Key} is not Exist in {part.Value.obj}");
				continue;
			}

			bool bres = true;
			if (part.Value.partName.Contains("mouth"))
			{
				string dinoname = part.Value.partName.Replace("_mouth", "");

				ResourcePoolManager.Instance.AsyncLoadData(dinoname, objectType.character, (dino) =>
					//foreach (var dinodata in dinoBasePool)
				{
					if (dino != null)
					{
						GameObject dinodata = (GameObject)dino;
						
						Transform obj = dinodata.transform.Find(part.Value.partName);
						mouthMesh = obj.GetComponent<SkinnedMeshRenderer>();
						//mouthMesh.gameObject.SetActive(true);
						//character.Mouth = ren.sharedMesh;
						//obj.name = part.Value.partName;
						//mouthMesh.receiveShadows = false;
						Transform obj2 = dinodata.transform.Find(part.Value.partName + "_AH");

						if (obj2 == null)
						{
							bExistAHMouth = false;
							bres = false;
							Debug.LogError("mouth Object is not Found");
							return;
						}

						mouthAHMesh = obj2.GetComponent<SkinnedMeshRenderer>();
						//mouthAHMesh.gameObject.SetActive(false);
						//character.AHMouth = ren2.sharedMesh;
						//obj2.name = part.Value.partName + "_AH";
						//mouthAHMesh.receiveShadows = false;

						//break;
					}
				});

				if(bres)
					continue;
			}

			basepartsdictMesh.Add(part.Value.partName, parts);
			meshList = basepartsdictMesh[part.Value.partName];

			CheckPartsInfo(root, meshList, transform_dictionary, ref combine_instances, ref materials, ref bones);
		}

		// Obtain and configure the SkinnedMeshRenderer attached to
		// the character base.
		SkinnedMeshRenderer r = root.MakeComponent<SkinnedMeshRenderer>();

		r.sharedMesh = new Mesh();
		r.sharedMesh.CombineMeshes(combine_instances.ToArray(), false, false);
		r.bones = bones.ToArray();
		r.materials = materials.ToArray();
		r.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.On;
		r.receiveShadows = false;
		r.updateWhenOffscreen = true;
		if (null != character)
		{
			root.transform.parent = character.model.transform.parent;
			root.transform.localScale = character.model.transform.localScale;
			root.transform.eulerAngles = character.model.transform.eulerAngles;
			root.transform.position = character.model.transform.position;
		}

		root.GetComponent<CharacterBase>().CharacterInfo = character.model.GetComponent<CharacterBase>().CharacterInfo;

//		Character ccc = ComponentExtensions.GetCopyOf(root.GetComponent<Character>(), character.model.GetComponent<Character>());

		DestroyImmediate(character.model);
		character.model = null;

		character.model = root;

		SkinnedMeshRenderer[] meshs = character.model.GetComponentsInChildren<SkinnedMeshRenderer>();
		foreach (var mesh in meshs)
		{
			if (mesh.name.Contains("mouth"))
			{
				bool existbody = false;
				costumePartsInfo info = character.partList[ItemType.body];
				string basebody = info.partName.Replace("_body", "");

				ResourcePoolManager.Instance.AsyncLoadData(basebody, objectType.character, (dino) =>
				{
					if (dino != null)
					{
						GameObject dinobase = (GameObject) dino;
						
						Transform bodytrans = dinobase.transform.Find(info.partName);
						if(bodytrans != null)
						{
							bodyMat = bodytrans.GetComponent<SkinnedMeshRenderer>().sharedMaterials[0];
							existbody = true;
						}

//						break;
					}
				});

				if (existbody == false)
					continue;

				if (mesh.name.Contains("AH"))
				{
					if (bExistAHMouth)
					{
						for (int i = 0; i < mouthAHMesh.bones.Length; i++)
						{
							string bonename = mouthAHMesh.bones[i].name;
							Transform findbone = Utility.FindBone(character.model.transform, bonename);
							mesh.bones[i] = findbone;
						}

						mesh.sharedMesh = mouthAHMesh.sharedMesh;
						mesh.sharedMaterials = mouthAHMesh.sharedMaterials;
						mesh.material = bodyMat;
						mesh.receiveShadows = false;
						character.AHMouth = mesh.gameObject;
					}
					mesh.gameObject.SetActive(false);
				}
				else
				{
					mesh.sharedMesh = mouthMesh.sharedMesh;
					mesh.sharedMaterials = mouthMesh.sharedMaterials;
					mesh.material = bodyMat;
					mesh.receiveShadows = false;
					character.Mouth = mesh.gameObject;

					mesh.gameObject.SetActive(true);
				}

				continue;
			}

			if (character.model == mesh.gameObject)
				continue;
			UnityEngine.Object.Destroy(mesh.gameObject);
		}

		// Animation 초기화
		//var anm_name = (GameDataMgr.Instance.Player.Gender == Definition.GENDER.GIRL ? "pc_f01_ellie_pose_idle" : "pc_m01_jimmy_pose_idle");
		//character.model.GetComponent<AnmActor>().LoopAnim(anm_name, 1, 0);
		//character.model.GetComponent<AnmActor>().FixedUpdate();
		//character.model.GetComponent<AnmActor>().Update();

		character.model.GetComponent<CharacterBase>().CharacterInfo.costumeInfo = character;

		DOCOSTUME = false;
	}

	public T CopyComponent<T>(T original, GameObject destination) where T : Component
	{

		System.Type type = original.GetType();
		Destroy(destination.GetComponent<T>());
		Component copy = destination.AddComponent(type);
		System.Reflection.FieldInfo[] fields = type.GetFields();
		foreach (System.Reflection.FieldInfo field in fields)
		{
			field.SetValue(copy, field.GetValue(original));
		}
		return copy as T;
	}

	bool CheckPartsInfo(GameObject root, List<Transform> meshList, MultiDictionary<string, Transform> transform_dictionary, ref List<CombineInstance> combine_instances, ref List<Material> materials, ref List<Transform> bonelist)
	{
		for (int i = 0; i < meshList.Count; ++i)
		{
			Renderer rd = meshList[i].GetComponent<Renderer>();
			string objName = rd.gameObject.name;

			if (null != (rd as SkinnedMeshRenderer))
			{
				List<string> boneNames = new List<string>();

				GameObject rendererClone = GameObject.Instantiate(rd.gameObject);
				rendererClone.name = rd.gameObject.name;

				SkinnedMeshRenderer smr = rendererClone.GetComponent<SkinnedMeshRenderer>();
				// When assembling a character, we load SkinnedMeshRenderers from assetbundles,
				// and as such they have lost the references to their bones. To be able to
				// remap the SkinnedMeshRenderers to use the bones from the characterbase assetbundles,
				// we save the names of the bones used.                        
				smr.updateWhenOffscreen = true;
				for (int sub = 0; sub < smr.sharedMesh.subMeshCount; ++sub)
				{
					foreach (Transform t in smr.bones)
					{
						boneNames.Add(t.name);
					}
				}

				materials.AddRange(smr.materials);
				for (int sub = 0; sub < smr.sharedMesh.subMeshCount; sub++)
				{
					CombineInstance ci = new CombineInstance { mesh = smr.sharedMesh, subMeshIndex = sub };
					combine_instances.Add(ci);
				}

				foreach (string bone in boneNames)
				{
					List<Transform> tsmlist = transform_dictionary[bone];
					foreach (var tsm in tsmlist)
					{
						bonelist.Add(tsm);
					}
				}

				UnityEngine.Object.Destroy(smr.gameObject);
			}
			else if (null != (rd as MeshRenderer))
			{
				MeshFilter mf = rd.GetComponent<MeshFilter>();

				//GameObject rendererClone = (GameObject) EditorUtility.InstantiatePrefab( mf.gameObject );
				GameObject rendererClone = GameObject.Instantiate(mf.gameObject);
				rendererClone.name = objName;

				List<string> boneNames = new List<string>();

				if (null != mf.transform.parent)
				{
					GameObject rendererParent = rendererClone.transform.root.gameObject;
					if (rendererClone.transform.parent)
						boneNames.Add(GetGameObjectPath(rendererClone.transform.parent.gameObject));

					rendererClone.transform.parent = null;
					UnityEngine.Object.DestroyImmediate(rendererParent);
				}

				if (0 < boneNames.Count)
				{
					string path = boneNames[0];//.Substring(1);
					rendererClone.transform.parent = root.transform.Find(path);
				}

				if (rendererClone)
				{
					MeshRenderer renderer = rendererClone.GetComponent<Renderer>() as MeshRenderer;
					Debug.Assert(renderer != null);

					renderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
					renderer.receiveShadows = false;
				}
			}
		}

		return true;
	}

	string GetGameObjectPath(GameObject obj)
	{
		GameObject go = obj;

		if (null == go)
		{
			return "";
		}

		string path = "/" + go.name;
		while (go.transform.parent != null)
		{
			go = go.transform.parent.gameObject;

			if (go.transform.parent == obj.transform.root || go.transform.parent == null)
			{
				path = go.name + path;
				break;
			}

			path = "/" + go.name + path;
		}

		return path;
	}
}