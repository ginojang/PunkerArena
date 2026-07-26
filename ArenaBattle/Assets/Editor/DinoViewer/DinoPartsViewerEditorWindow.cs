using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEditor.SceneManagement;
public class dinoPartsViewerEditorWindow : dinoViewerEditorWindow
{
	static dinoPartsViewerEditorWindow instance = null;

	private dinoPartsData dinoData = null;

	bool isPlay = false;
	bool isInit = true;
	static bool isExecute = false;

	[MenuItem("Tools/Dino Tool/Parts Viewer")]
	public static void Execute()
	{
		instance = GetWindow<dinoPartsViewerEditorWindow>("dino Parts Viewer", typeof(dinoPartsViewerEditorWindow));

		isExecute = true;
	}

	protected override void Initialize()
	{
		base.Initialize();

		if (!Application.isPlaying)
		{
			if (EditorSceneManager.GetActiveScene().name != "DinoPartsViewer")
				EditorSceneManager.OpenScene($"Assets/Scenes/Tool/DinoPartsViewer.unity", OpenSceneMode.Single);
		}

//		GameObject dino = GameObject.Find("DinoPartsTool");
//		dinoTool = dino.GetComponent<DinoPartsTool>();

		LoadData();

		CurTalent = CharacterTalent.Carnivore;
		CurClass = CharacterClass.orange;
		CURPARTS = ItemType.body;

		isInit = true;

		if (isPlay)
		{
			LoadStartCharacter();
			isPlay = false;
		}
	}

	protected void LoadStartCharacter()
	{
		GetSelectedList()[ItemType.body].charTalent = CurTalent;
		GetSelectedList()[ItemType.body].charClass = CurClass;
		GetSelectedList()[ItemType.body].partItemName = Definition.baseCharBodyName + "_body";
	}

	protected override void OnEnable()
	{
//		if (Application.isPlaying)
//			return;

		if(isExecute)
		{
			isExecute = false;
			return;
		}

		base.OnEnable();
	}

	protected void Update()
	{
		if(isInit)
		{
			if(dinoTool == null)
			{
				GameObject dino = GameObject.Find("DinoPartsTool");
				dinoTool = dino.GetComponent<DinoPartsTool>();
			}
			if(dinoTool != null && dinoTool.bInit && dinoTool.bInitAsset)
			{
				isInit = false;
				dinoTool.Initialize(GetSelectedList(), GetSelectedNames());
			}
		}
	}

	protected override bool LoadData()
	{
#if UNITY_EDITOR
		CSVDataManager.InitTables();
#else
		CSVDataManager.InitAWS();
#endif
		return (dinoData != null);
	}

	protected override void ReloadDataTable()
	{
#if UNITY_EDITOR
		CSVDataManager.InitTables(true);
#else
		CSVDataManager.InitAWS(true);
#endif
	}

	protected override void Save()
	{
		if (dinoData == null)
			return;

		EditorUtility.SetDirty(dinoData);

		base.Save();
	}

	// 초식, 육식, 잡식
	protected override void HerbivoreData()
	{
		// 선택된 바디가 있으면 해당 탈렌트의 바디로 변경
		// 선택된 바디가 없으면 교체가 애매하기 때문에 탈렌트 값만 일단 변경해둠
/*		if (SelectedPartName(ItemType.body) == string.Empty)
		{
			EditorUtility.DisplayDialog("알림", "Body Type을 먼저 선택해 주세요.\n", "OK");
			return;
		}
*/		
		CurTalent = CharacterTalent.Herbivore;

		ChangeTalentData();
	}

	protected override void PredatorData()
	{
		// 선택된 바디가 있으면 해당 탈렌트의 바디로 변경
		// 선택된 바디가 없으면 교체가 애매하기 때문에 탈렌트 값만 일단 변경해둠
/*		if (SelectedPartName(ItemType.body) == string.Empty)
		{
			EditorUtility.DisplayDialog("알림", "Body Type을 먼저 선택해 주세요.\n", "OK");
			return;
		}
*/
		CurTalent = CharacterTalent.Carnivore;

		ChangeTalentData();
	}

	protected override void OmnivoreData()
	{
		// 선택된 바디가 있으면 해당 탈렌트의 바디로 변경
		// 선택된 바디가 없으면 교체가 애매하기 때문에 탈렌트 값만 일단 변경해둠
/*		if (SelectedPartName(ItemType.body) == string.Empty)
		{
			EditorUtility.DisplayDialog("알림", "Body Type을 먼저 선택해 주세요.\n", "OK");
			return;
		}
*/
		CurTalent = CharacterTalent.Omnivore;

		ChangeTalentData();
	}

	protected override void PartsButton()
	{
		switch(CURPARTS)
		{
			case ItemType.body:
				{

				}
				break;
			case ItemType.headparts:
				{

				}
				break;
			case ItemType.eyes:
				{

				}
				break;
			case ItemType.mouth:
				{

				}
				break;
			case ItemType.back:
				{

				}
				break;
			case ItemType.tail:
				{

				}
				break;
			case ItemType.wing:
				{

				}
				break;
			case ItemType.pattern:
				{

				}
				break;
		}
	}

	protected override void PartsChanged()
	{

	}

	// 과일별 파츠
	protected override void OrangePartsData()
	{
		SetSelectedPart(CharacterClass.orange, CURINDEX);
	}

	protected override void WatermelonPartsData()
	{
		SetSelectedPart(CharacterClass.watermelon, CURINDEX);
	}

	protected override void CoconutPartsData()
	{
		SetSelectedPart(CharacterClass.coconut, CURINDEX);
	}

	protected override void BlueberryPartsData()
	{
		SetSelectedPart(CharacterClass.blueberry, CURINDEX);
	}

	protected override void PineapplePartsData()
	{
		SetSelectedPart(CharacterClass.pineapple, CURINDEX);
	}

	protected override void BananaPartsData()
	{
		SetSelectedPart(CharacterClass.banana, CURINDEX);
	}

	protected override void DurianPartsData()
	{
		SetSelectedPart(CharacterClass.durian, CURINDEX);
	}

	protected override void RambutanPartsData()
	{
		SetSelectedPart(CharacterClass.rambutan, CURINDEX);
	}

	protected override void MelonPartsData()
	{
		SetSelectedPart(CharacterClass.melon, CURINDEX);
	}

	protected override void DragonfruitPartsData()
	{
		SetSelectedPart(CharacterClass.dragonfruit, CURINDEX);
	}

	protected override void LimitedPartsData()
	{
		SetSelectedPart(CharacterClass.limited, CURINDEX);
	}


	// 애니메이션
	protected override void SelectedAnimation()
	{
		dinoTool?.ChangeAnimation(ANINAME);
	}

	protected override void AddData()
	{
		selectIndex = dinoData.AddData(new dinoPartsData.dinoData());
	}

	protected override dinoPartsBaseData.BaseData GetData()
	{
		return dinoData.GetDataByIndex(selectIndex);
	}

	protected override string[] GetQuestIDs()
	{
		if (searchQuestId > 0)
			return dinoData.datas.Select(d => d.stepUid.ToString()).Where(d => d.Contains(searchQuestId.ToString())).ToArray();
		return dinoData.datas.Select(d => d.stepUid.ToString()).ToArray();
	}

	protected override int GetDataCount()
	{
		return dinoData.Count();
	}

	protected override void DeleteData()
	{
		dinoData.DeleteData(selectIndex);
		if (selectIndex > 0 && selectIndex >= dinoData.Count())
			selectIndex = dinoData.Count() - 1;
	}

	protected override void DrawMenu()
	{
		if (dinoData != null)
		{
			var data = dinoData.GetDataByIndex(selectIndex);
			DrawMenu((data != null), dinoData.Count(), ref selectIndex, ReloadDataTable, RandomSelect, UnityPlay);
		}
		else
			DrawMenu(false, dinoData ? dinoData.Count() : 0, ref selectIndex, ReloadDataTable, RandomSelect, UnityPlay);
	}

	protected override void DrawKindButton()
	{
		DrawKindButton(PredatorData, OmnivoreData, HerbivoreData);
	}

	protected override void DrawPartsButton()
	{
		DrawPartsButton(PartsButton);
	}

	protected override void DrawClassButton()
	{
		DrawClassListButton();
	}

	protected override void DrawAnimationButton()
	{
		DrawAnimationListButton(SelectedAnimation);
	}

	protected override void RandomSelect()
	{
		RandomSelectCharacter();
	}

	protected override void UnityPlay()
	{
		if (EditorApplication.isPlaying)
			EditorApplication.isPlaying = false;
		else
		{
			EditorApplication.isPlaying = true;
			isPlay = true;
		}
	}

	// 초식, 육식, 잡식에 해당하는 베이스 이미지로 변경
	// 베이스 로딩 후, 현재 선택되어 있는 각 파트의 값들로 전부 변경
	private void ChangeTalentData()
	{
		ChangeTalentCharacter();
		ChangeItemTypeInfo();
		ChangePartsInfo();
	}

	// Talent 별 베이스 케릭터 로드
	private void ChangeTalentCharacter()
	{
		ChangeTalent();
	}

	// 부위별파츠 아이템 교체
	private void ChangeItemTypeInfo()
	{

	}

	private void ChangePartsInfo()
	{

	}

	protected override void DrawDatas()
	{
		var data = (dinoPartsData.dinoData)dinoData.GetDataByIndex(selectIndex);

		GUILayout.Space(10);

		// item resource path
		DrawLabel("Item Resource Path", (_label) =>
		{
			var ret = DrawTextField(_label, data.itemResourcePath);
			if (ret != data.itemResourcePath)
			{
				isDirty = true;
				data.itemResourcePath = ret;
			}
		});

		GUILayout.Space(10);

		// item resource size
		DrawLabel("Item Resource Size", (_label) =>
		{
			DrawLabel("X", (_label_x) =>
			{
				var ret = DrawIntField($"{_label}{_label_x}", data.itemResourceSize.x);
				if (ret != data.itemResourceSize.x)
				{
					isDirty = true;
					data.itemResourceSize.x = ret;
				}
			}, 30);
			DrawLabel("Y", (_label_y) =>
			{
				var ret = DrawIntField($"{_label}{_label_y}", data.itemResourceSize.y);
				if (ret != data.itemResourceSize.y)
				{
					isDirty = true;
					data.itemResourceSize.y = ret;
				}
			}, 30);
		});

		GUILayout.Space(10);

		// item resource center
		DrawLabel("Item Resource Center", (_label) =>
		{
			DrawLabel("X", (_label_x) =>
			{
				var ret = DrawIntField($"{_label}{_label_x}", data.itemResourceCenter.x);
				if (ret != data.itemResourceCenter.x)
				{
					isDirty = true;
					data.itemResourceCenter.x = ret;
				}
			}, 30);
			DrawLabel("Y", (_label_y) =>
			{
				var ret = DrawIntField($"{_label}{_label_y}", data.itemResourceCenter.y);
				if (ret != data.itemResourceCenter.y)
				{
					isDirty = true;
					data.itemResourceCenter.y = ret;
				}
			}, 30);
		});
		GUILayout.Space(10);

		// dust density
		DrawLabel("Dust Density Size", (_label) =>
		{
			DrawLabel("X", (_label_x) =>
			{
				var ret = DrawIntField($"{_label}{_label_x}", data.dustDensity.x);
				if (ret != data.dustDensity.x)
				{
					isDirty = true;
					data.dustDensity.x = ret;
				}
			}, 30);
			DrawLabel("Y", (_label_y) =>
			{
				var ret = DrawIntField($"{_label}{_label_y}", data.dustDensity.y);
				if (ret != data.dustDensity.y)
				{
					isDirty = true;
					data.dustDensity.y = ret;
				}
			}, 30);
		});

		GUILayout.Space(10);
	}
}
