using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Linq;

public class partInfo
{
	public partInfo(string name, System.Action action)
	{
		_name = name;
		_action = action;
	}

	public string _name;
	public System.Action _action;
}

public abstract class dinoViewerEditorWindow : EditorWindow
{
	private Vector2 dataListViewScrollPosition;
	private Vector2 windowViewScrollPosition;
	protected uint searchQuestId;
	protected bool isDirty;
	protected int selectIndex;
	protected GUIStyle orgLabelStyle = null;
	protected GUIStyle titleLabelStyle = null;
	protected GUIStyle dataLabelStyle = null;
	protected GUIStyle buttonLabelStyle = null;
	public DinoPartsTool dinoTool = null;

	protected abstract bool LoadData();
	protected abstract void AddData();
	protected abstract dinoPartsBaseData.BaseData GetData();
	protected abstract string[] GetQuestIDs();
	protected abstract int GetDataCount();
	protected abstract void DeleteData();
	protected abstract void DrawMenu();
	protected abstract void DrawKindButton();
	protected abstract void DrawPartsButton();
	protected abstract void DrawClassButton();
	protected abstract void DrawAnimationButton();
	protected abstract void RandomSelect();
	protected abstract void DrawDatas();
	protected abstract void UnityPlay();
	protected abstract void HerbivoreData();
	protected abstract void PredatorData();
	protected abstract void OmnivoreData();

	protected abstract void PartsButton();

	protected abstract void ReloadDataTable();
	protected abstract void OrangePartsData();
	protected abstract void WatermelonPartsData();
	protected abstract void CoconutPartsData();

	protected abstract void PartsChanged();
	protected abstract void BlueberryPartsData();
	protected abstract void PineapplePartsData();
	protected abstract void BananaPartsData();
	protected abstract void DurianPartsData();
	protected abstract void RambutanPartsData();
	protected abstract void MelonPartsData();
	protected abstract void DragonfruitPartsData();
	protected abstract void LimitedPartsData();

	protected abstract void SelectedAnimation();

	public GUISkin CustomGuiSkin;
	private Texture button_select_tex = null;
	private Texture button_enable_tex = null;
	private Texture button_disable_tex = null;
	private GUIContent button_select_tex_Con = null;
	private GUIContent button_enable_tex_Con = null;
	private GUIContent button_disable_tex_Con = null;

	GUIContent content = new GUIContent();

	private CharacterTalent curCharTalent = CharacterTalent.Carnivore;
	private CharacterClass curClass = CharacterClass.None;
	private ItemType curParts = ItemType.body;

	// 현재 파츠에 선택된 값
	private selectedPartInfo curSelectPart = new selectedPartInfo(CharacterTalent.Carnivore, CharacterClass.orange, "", -1);
	// 파츠별 현재 선택된 정보 리스트
	private Dictionary<ItemType, selectedPartInfo> itemTypeSelectedList = new Dictionary<ItemType, selectedPartInfo>();

	private Dictionary<CharacterClass, partInfo> partActionList = new Dictionary<CharacterClass, partInfo>();

	private List<string> animationList = new List<string>();

	private int selectedIndex = 0;
	private int curAnimation = 0;

	private string curSelectedItemName = "";
	private Dictionary<ItemType, string> curSelectedNameList = new Dictionary<ItemType, string>();

	private Dictionary<int, Generated.CsvData.partsData> curClassPartData = new Dictionary<int, Generated.CsvData.partsData>();

	public CharacterTalent CurTalent
	{
		get { return curCharTalent; }
		set { curCharTalent = value; }
	}

	public CharacterClass CurClass
	{
		get { return curClass; }
		set { curClass = value; }
	}

	public ItemType CURPARTS
	{
		get { return curParts; }
		set { curParts = value; }
	}

	public int CURINDEX
	{
		get { return selectedIndex; }
		set { selectedIndex = value; }
	}

	public int CURANIMATION
	{
		get { return curAnimation; }
		set { curAnimation = value; }
	}

	public string ANINAME
	{
		get { return animationList[CURANIMATION]; }
	}
	protected virtual void OnEnable()
	{
		Initialize();
	}

	protected virtual void Initialize()
	{
		isDirty = false;
//		selectIndex = 0;
//		searchQuestId = 0;
//		dataListViewScrollPosition = Vector2.zero;

		if(this.button_select_tex_Con == null)
		{
			if(this.button_select_tex == null)
			{
				this.button_select_tex = (Texture)AssetDatabase.LoadAssetAtPath("Assets/EditorResources/Btnbg_Selected.png", typeof(Texture));
			}
			this.button_select_tex_Con = new GUIContent(this.button_select_tex);
		}

		if (this.button_enable_tex_Con == null)
		{
			if (this.button_enable_tex == null)
			{
				this.button_enable_tex = (Texture)AssetDatabase.LoadAssetAtPath("Assets/EditorResources/Btnbg_Enabled.png", typeof(Texture));
			}
			this.button_enable_tex_Con = new GUIContent(this.button_enable_tex);
		}

		if (this.button_disable_tex_Con == null)
		{
			if (this.button_disable_tex == null)
			{
				this.button_disable_tex = (Texture)AssetDatabase.LoadAssetAtPath("Assets/EditorResources/Btnbg_Disabled.png", typeof(Texture));
			}
			this.button_disable_tex_Con = new GUIContent(this.button_disable_tex);
		}

		if(itemTypeSelectedList.Count == 0)
		{
			itemTypeSelectedList.Add(ItemType.body, new selectedPartInfo(CharacterTalent.Carnivore, CharacterClass.orange, "ca_orange_01_body", -1));
			itemTypeSelectedList.Add(ItemType.headparts, new selectedPartInfo(CharacterTalent.Carnivore, CharacterClass.orange, "ca_orange_01_headparts", -1));
			itemTypeSelectedList.Add(ItemType.eyes, new selectedPartInfo(CharacterTalent.Carnivore, CharacterClass.orange, "ca_orange_01_eyes", -1));
			itemTypeSelectedList.Add(ItemType.mouth, new selectedPartInfo(CharacterTalent.Carnivore, CharacterClass.orange, "ca_orange_01_mouth", -1));
			itemTypeSelectedList.Add(ItemType.back, new selectedPartInfo(CharacterTalent.Carnivore, CharacterClass.orange, "ca_orange_01_back", -1));
			itemTypeSelectedList.Add(ItemType.tail, new selectedPartInfo(CharacterTalent.Carnivore, CharacterClass.orange, "ca_orange_01_tail", -1));
			itemTypeSelectedList.Add(ItemType.wing, new selectedPartInfo(CharacterTalent.Carnivore, CharacterClass.orange, "", -1));
			itemTypeSelectedList.Add(ItemType.pattern, new selectedPartInfo(CharacterTalent.Carnivore, CharacterClass.orange, "", -1));
		}

		if (partActionList.Count == 0)
		{
			partActionList.Add(CharacterClass.orange, new partInfo("오렌지", OrangePartsData));
			partActionList.Add(CharacterClass.watermelon, new partInfo("수박", WatermelonPartsData));
			partActionList.Add(CharacterClass.durian, new partInfo("두리안", DurianPartsData));
			partActionList.Add(CharacterClass.coconut, new partInfo("코코넛", CoconutPartsData));
			partActionList.Add(CharacterClass.blueberry, new partInfo("블루베리", BlueberryPartsData));
			partActionList.Add(CharacterClass.melon, new partInfo("멜론", MelonPartsData));
			partActionList.Add(CharacterClass.pineapple, new partInfo("파인애플", PineapplePartsData));
			partActionList.Add(CharacterClass.banana, new partInfo("바나나", BananaPartsData));
			partActionList.Add(CharacterClass.rambutan, new partInfo("람부탄", RambutanPartsData));
			partActionList.Add(CharacterClass.dragonfruit, new partInfo("용과", DragonfruitPartsData));
			partActionList.Add(CharacterClass.limited, new partInfo("한정판", LimitedPartsData));
		}

		if (animationList.Count == 0)
		{
			animationList.Clear();
			animationList.Add("디폴트");
			animationList.Add("아이들");
			animationList.Add("이동");
			animationList.Add("공격");
			animationList.Add("피격");
			animationList.Add("사망");
			animationList.Add("스킬1");
			animationList.Add("스킬2");
			animationList.Add("스킬3");
			animationList.Add("스킬4");
			animationList.Add("스킬5");
		}

		if(curSelectedNameList.Count == 0)
		{
			curSelectedNameList.Add(ItemType.body, "ca_orange_01_body");
			curSelectedNameList.Add(ItemType.headparts, "ca_orange_01_headparts");
			curSelectedNameList.Add(ItemType.eyes, "ca_orange_01_eyes");
			curSelectedNameList.Add(ItemType.mouth, "ca_orange_01_mouth");
			curSelectedNameList.Add(ItemType.back, "ca_orange_01_back");
			curSelectedNameList.Add(ItemType.tail, "ca_orange_01_tail");
			curSelectedNameList.Add(ItemType.wing, "");
			curSelectedNameList.Add(ItemType.pattern, "");
		}

//		curClassPartData = CSVDataManager.GetClassPartInfo(CharacterClass.Orange);
	}

	public selectedPartInfo GetSelectedPart()
	{
		return curSelectPart;
	}

	public Dictionary<ItemType, selectedPartInfo> GetSelectedList()
	{
		return itemTypeSelectedList;
	}

	public Dictionary<ItemType, string> GetSelectedNames()
	{
		return curSelectedNameList;
	}

	public string SelectedPartName(ItemType type)
	{
		string parts = "";

		if(curSelectedNameList.ContainsKey(type))
		{
			parts = curSelectedNameList[type];
		}
		return parts;
	}

	public void SetSelectedPart(CharacterClass charclass, int idx, bool random = false)
	{
/*		if (SelectedPartName(ItemType.body) == string.Empty && CURPARTS != ItemType.body)
		{
			EditorUtility.DisplayDialog("알림", "Body Type을 먼저 선택해 주세요.\n", "OK");
			return;
		}*/

		bool bDragon = false;
		
		if (CURPARTS == ItemType.body)
		{
			if (curClass == CharacterClass.dragonfruit)
				bDragon = true;
			
			curClass = charclass;
		}
		else
		{
			if (curClass == CharacterClass.dragonfruit)
			{
				EditorUtility.DisplayDialog("알림",
					"용과는 다른 과일과 파츠 교체가 불가합니다.\n\n다른 과일의 Body를 선택해 주세요", "예");

				return;
			}
		}

		curClassPartData = CSVDataManager.GetClassPartInfo(charclass);

		int count = 0;
//		Generated.CsvData.partsData data = null;

		if (charclass == CharacterClass.dragonfruit)
		{
			curClass = CharacterClass.dragonfruit;

			string prefix = "";
			
			switch (CurTalent)
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
			
			itemTypeSelectedList[ItemType.body] = new selectedPartInfo(CurTalent, curClass, prefix + "dragonfruit_01_body", 0);
			itemTypeSelectedList[ItemType.headparts] = new selectedPartInfo(CurTalent, curClass, prefix + "dragonfruit_01_headparts", 0);
			itemTypeSelectedList[ItemType.eyes] = new selectedPartInfo(CurTalent, curClass, prefix + "dragonfruit_01_eyes", 0);
			itemTypeSelectedList[ItemType.mouth] = new selectedPartInfo(CurTalent, curClass, prefix + "dragonfruit_01_mouth", 0);
			itemTypeSelectedList[ItemType.back] = new selectedPartInfo(CurTalent, curClass, prefix + "dragonfruit_01_back", 0);
			itemTypeSelectedList[ItemType.tail] = new selectedPartInfo(CurTalent, curClass, prefix + "dragonfruit_01_tail", 0);
			itemTypeSelectedList[ItemType.wing] = new selectedPartInfo(CurTalent, curClass, prefix + "dragonfruit_01_wing", 0);
			
			ChangeTalent();
		}
		else
		{
			if (bDragon)
			{
				string prefix = "";
			
				switch (CurTalent)
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
			
				itemTypeSelectedList[ItemType.body] = new selectedPartInfo(CurTalent, curClass, prefix + curClass.ToString()+"_01_body", 0);
				itemTypeSelectedList[ItemType.headparts] = new selectedPartInfo(CurTalent, curClass, prefix + curClass.ToString()+"_01_headparts", 0);
				itemTypeSelectedList[ItemType.eyes] = new selectedPartInfo(CurTalent, curClass, prefix + curClass.ToString()+"_01_eyes", 0);
				itemTypeSelectedList[ItemType.mouth] = new selectedPartInfo(CurTalent, curClass, prefix + curClass.ToString()+"_01_mouth", 0);
				itemTypeSelectedList[ItemType.back] = new selectedPartInfo(CurTalent, curClass, prefix + curClass.ToString()+"_01_back", 0);
				itemTypeSelectedList[ItemType.tail] = new selectedPartInfo(CurTalent, curClass, prefix + curClass.ToString()+"_01_tail", 0);
				itemTypeSelectedList[ItemType.wing] = new selectedPartInfo(CurTalent, curClass, prefix + curClass.ToString()+"_01_wing", 0);

				ChangeTalent();

			}
			else
			{
				foreach (var data in curClassPartData)
				{
					string prefix = "";

					if (count == idx)
					{
						switch (CurTalent)
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

						curSelectedItemName = data.Value.partList[CURPARTS].fileName;
						curSelectedNameList[CURPARTS] = prefix + curSelectedItemName;
						itemTypeSelectedList[CURPARTS].charTalent = CurTalent;
						itemTypeSelectedList[CURPARTS].charClass = charclass;
						itemTypeSelectedList[CURPARTS].partItemName = prefix + curSelectedItemName;
						itemTypeSelectedList[CURPARTS].index = idx;
						itemTypeSelectedList[CURPARTS].part = CURPARTS;
						curSelectPart.charTalent = CurTalent;
						curSelectPart.partItemName = prefix + curSelectedItemName;
						curSelectPart.charClass = charclass;
						curSelectPart.index = idx;
						curSelectPart.part = CURPARTS;

						if (!random)
							dinoTool.ChangePart(curSelectPart);
						break;
					}

					count++;
				}
			}
		}
	}

	public void SetRandomSelectPart()
	{

	}

	private void InitGuiStyle()
	{
		if (orgLabelStyle == null)
		{
			orgLabelStyle = new GUIStyle(GUI.skin.GetStyle("Label"))
			{
				alignment = TextAnchor.MiddleLeft,
				fontStyle = FontStyle.Normal,
				fontSize = 15
			};
		}

		if (titleLabelStyle == null)
		{
			titleLabelStyle = new GUIStyle(GUI.skin.GetStyle("Label"))
			{
				alignment = TextAnchor.MiddleCenter,
				fontStyle = FontStyle.Normal,
				fontSize = 15
			};
		}

		if (dataLabelStyle == null)
		{
			dataLabelStyle = new GUIStyle(GUI.skin.GetStyle("Label"))
			{
				alignment = TextAnchor.MiddleLeft,
				fontStyle = FontStyle.Bold,
				fontSize = 12
			};
		}

		if (buttonLabelStyle == null)
		{
			buttonLabelStyle = new GUIStyle(GUI.skin.GetStyle("Label"))
			{
				alignment = TextAnchor.MiddleLeft,
				fontStyle = FontStyle.Bold,
				fontSize = 18,

			};
		}
	}

	public void ChangeTalent()
	{
		for (int type = 0; type < (int)ItemType.Max; type++)
		{
			itemTypeSelectedList[(ItemType)type].charTalent = CurTalent;

			itemTypeSelectedList[(ItemType)type].partItemName = ChangeTalentPartName(itemTypeSelectedList[(ItemType)type].partItemName, CurTalent);

			curSelectedNameList[(ItemType)type] = ChangeTalentPartName(curSelectedNameList[(ItemType)type], CurTalent);
		}

		dinoTool.ChangeParts(CurTalent, CurClass, GetSelectedList());
	}

	private string ChangeTalentPartName(string source, CharacterTalent destTalent)
	{
		string prefix = "";
		string part = "";
		switch (CurTalent)
		{
			case CharacterTalent.Carnivore:
				{
					prefix = "ca_";
				}
				break;
			case CharacterTalent.Omnivore:
				{
					prefix = "om_";
				}
				break;
			case CharacterTalent.Herbivore:
				{
					prefix = "he_";
				}
				break;
		}

		if (source != string.Empty)
		{
			string[] partnames = source.Split('_');
			string partname = prefix;
			for (int o = 1; o < partnames.Length; o++)
			{
				partname += partnames[o];
				if (o < partnames.Length - 1)
					partname += '_';
			}

			part = partname;
		}

		return part;
	}

	protected void RandomSelectCharacter()
	{
		//		CURPARTS = (ItemType)(Random.Range(0, (int)ItemType.Max));

		CurTalent = (CharacterTalent)Random.Range(1, (int)CharacterTalent.Max);

		for(int i = 0; i < (int)ItemType.Max; i++)
		{
			if ((ItemType)i == ItemType.wing || (ItemType)i == ItemType.pattern)
				continue;

			CharacterClass charclass = CharacterClass.None;
			int index = -1;

			Dictionary<int, Generated.CsvData.partsData> list = CSVDataManager.GetRandomPartItem((ItemType)i, ref charclass, ref index);

			if(list == null)
			{
				Debug.LogError($"선택된 {(ItemType)i} 타입 데이터가 없음");
				continue;
			}

			if (!list[index].partList.ContainsKey((ItemType)i))
				continue;

			CURPARTS = (ItemType)i;

			//string names = list[index].partList[(ItemType)i].fileName;

			SetSelectedPart(charclass, index, true);
		}

		ChangeTalent();
		//dinoTool.ChangeParts(CurTalent, CurClass, GetSelectedList());
	}

	protected virtual void Save()
	{
		AssetDatabase.SaveAssets();
		AssetDatabase.Refresh();

		isDirty = false;
		GUI.FocusControl("");

		Debug.Log("Success Save.");
	}

	protected void DrawMenu(bool _is_enable, int _data_count, ref int _select_index, System.Action _reload_action, System.Action _random_action, System.Action _play_action)
	{
		GUILayout.Space(20);
		GUILayout.Label("[[[[[[[[[[   Dino Parts Viewer   ]]]]]]]]]]", titleLabelStyle, GUILayout.Height(50));
		GUILayout.Space(100);

		DrawNormalButton("테이블 다시 로드", true, () =>
		{
			GUI.FocusControl("");
			_reload_action?.Invoke();
		});

		DrawNormalButton("랜덤 선택", true, () =>
		{
			GUI.FocusControl("");
			_random_action?.Invoke();
		});
		string play = "";
		if (EditorApplication.isPlaying)
			play = "유니티 종료";
		else
			play = "유니티 실행";
		DrawNormalButton(play, true, () =>
		{
			_play_action.Invoke();
		});
	}

	protected void DrawKindButton(System.Action _predator, System.Action _omnivore, System.Action _herbivore)
	{
		GUILayout.BeginHorizontal();
		{
			DrawRoleButton("육식", CharacterTalent.Carnivore, true, () =>
			{
				GUI.FocusControl("");
				_predator?.Invoke();
			});

			DrawRoleButton("잡식", CharacterTalent.Omnivore, true, () =>
			{
				GUI.FocusControl("");
				_omnivore?.Invoke();
			});

			DrawRoleButton("초식", CharacterTalent.Herbivore, true, () =>
			{
				GUI.FocusControl("");
				_herbivore?.Invoke();
			});
		}
		GUILayout.EndHorizontal();

		GUILayout.Space(50);
	}

	protected void DrawPartsButton(System.Action _action)
	{
		GUILayout.BeginVertical();
		{
			GUILayout.Label("Parts", orgLabelStyle, GUILayout.Height(50));
		}
		GUILayout.EndVertical();

		GUILayout.BeginHorizontal();
		{
			DrawPartsButton("몸통", ItemType.body, true, () =>
			{
				GUI.FocusControl("");

				CURPARTS = ItemType.body;

				_action?.Invoke();
			});

			DrawPartsButton("머리부위", ItemType.headparts, true, () =>
			{
				GUI.FocusControl("");

				CURPARTS = ItemType.headparts;

				_action?.Invoke();
			});

			DrawPartsButton("눈", ItemType.eyes, true, () =>
			{
				GUI.FocusControl("");

				CURPARTS = ItemType.eyes;

				_action?.Invoke();
			});

			DrawPartsButton("입", ItemType.mouth, true, () =>
			{
				GUI.FocusControl("");

				CURPARTS = ItemType.mouth;

				_action?.Invoke();
			});

			DrawPartsButton("등", ItemType.back, true, () =>
			{
				GUI.FocusControl("");

				CURPARTS = ItemType.back;

				_action?.Invoke();
			});

			DrawPartsButton("꼬리", ItemType.tail, true, () =>
			{
				GUI.FocusControl("");

				CURPARTS = ItemType.tail;

				_action?.Invoke();
			});

			DrawPartsButton("날개", ItemType.wing, true, () =>
			{
				GUI.FocusControl("");

				CURPARTS = ItemType.wing;

				_action?.Invoke();
			});

			DrawPartsButton("패턴", ItemType.pattern, true, () =>
			{
				GUI.FocusControl("");

				CURPARTS = ItemType.pattern;

				_action?.Invoke();
			});
		}
		GUILayout.EndHorizontal();

		GUILayout.Space(40);
	}

	protected void DrawClassListButton()
	{
		foreach(var classinfo in partActionList)
		{
			GUILayout.BeginHorizontal();
			{
				int count = 0;
				Dictionary<int, Generated.CsvData.partsData> list = CSVDataManager.GetClassPartInfo(classinfo.Key);

				bool enable = true;

				GUILayout.Label(classinfo.Value._name, buttonLabelStyle, GUILayout.Width(250), GUILayout.Height(30));
				GUILayout.Space(50);
				if (list != null)
				{
					foreach (var item in list)
					{
						if (!item.Value.partList.ContainsKey(CURPARTS))
							continue;

						string partname = item.Value.partList[CURPARTS].fileName;
						string buttonname = $"{count + 1}";
						if (partname != "")
						{
							if (partname == "none")
							{
								enable = false;
								buttonname = "준비중";
							}

							DrawPartButton(buttonname, enable, classinfo.Key, count, () =>
							//DrawPartButton(partname, true, classinfo.Key, count, () =>
							{
								GUI.FocusControl("");
								classinfo.Value._action?.Invoke();
							});
						}
						count++;
					}
				}
				for (int i = count; i < 10; i++)
				{
					DrawPartButton($"준비중", false, classinfo.Key, count, () =>
					{
						GUI.FocusControl("");
						classinfo.Value._action?.Invoke();
					});
				}
			}

			GUILayout.EndHorizontal();

			GUILayout.Space(20);
		}

		GUILayout.Label($"Selected Item Name : {curSelectedNameList[CURPARTS]}", titleLabelStyle, GUILayout.Height(50));
		GUILayout.Space(10);
	}

	protected void DrawAnimationListButton(System.Action _action)
	{
		GUILayout.BeginHorizontal();
		{
			for (int i = 0; i < animationList.Count; i++)
			{
				if (0 == i % 5)
				{
					GUILayout.EndHorizontal();
					GUILayout.BeginHorizontal();
				}
				DrawAnimButton(animationList[i], true, i, () =>
				{
					CURANIMATION = i;

					GUI.FocusControl("");
					_action?.Invoke();
				});
			}
		}

		GUILayout.EndHorizontal();
	}

	private void DrawDatas(dinoPartsBaseData.BaseData _data)
	{
		if (_data == null)
		{
			GUILayout.Label("데이터가 없습니다. Add 버튼을 눌러 데이터를 추가하세요.", titleLabelStyle, GUILayout.Height(50));
		}
		else
		{
			GUILayout.Label("- Data", orgLabelStyle);
			GUILayout.BeginHorizontal();
			{
				GUILayout.Space(20);
				GUILayout.BeginVertical("box");
				{
					GUILayout.Space(10);

					// step uid
					DrawLabel("Step UID", (_label) =>
					{
						var ret = DrawLongField(_label, _data.stepUid);
						if (ret != _data.stepUid)
						{
							isDirty = true;
							_data.stepUid = (uint)ret;
						}
					});

					GUILayout.Space(10);

					// uid
					DrawLabel("UID", (_label) =>
					{
						var ret = DrawLongField(_label, _data.uid);
						if (ret != _data.uid)
						{
							isDirty = true;
							_data.uid = (uint)ret;
						}
					});

					GUILayout.Space(10);

					// minigame id
					DrawLabel("Minigame ID", (_label) =>
					{
						var ret = DrawTextField(_label, _data.minigameId);
						if (ret != _data.minigameId)
						{
							isDirty = true;
							_data.minigameId = ret;
						}
					});

					DrawDatas();
				}
				GUILayout.EndVertical();
			}
			GUILayout.EndHorizontal();

			GUILayout.Label(GetDataCount() == 0 ? "0" : $"{selectIndex + 1} / {GetDataCount()}", titleLabelStyle, GUILayout.Height(30));

			if (isDirty == true)
			{
				GUILayout.Space(20);

				if (GUILayout.Button("Save", GUILayout.Height(30)))
					Save();

				var focus = GUI.GetNameOfFocusedControl();
				if (string.IsNullOrEmpty(focus) == true && focus != "searchText")
					Save();
			}

			GUILayout.Space(20);

			DrawScrollView();

			GUILayout.Label("Tip : 입력란에 추가 또는 수정하고 Enter키를 누르거나 활성화된 Save 버튼을 누르면 저장됩니다.", titleLabelStyle, GUILayout.Height(30));

			if (Event.current.keyCode == KeyCode.Return)
				GUI.FocusControl("");
		}
	}


	void OnGUI()
	{
		InitGuiStyle();

		GUILayout.Space(10);

		GUILayout.BeginVertical("box");
		{
			windowViewScrollPosition = GUILayout.BeginScrollView(windowViewScrollPosition, GUILayout.ExpandWidth(true), GUILayout.ExpandHeight(true));
			{
				GUILayout.BeginHorizontal();
				{
					DrawMenu();
				}
				GUILayout.EndHorizontal();
				GUILayout.Space(10);

				DrawKindButton();

				GUILayout.Space(10);

				GUILayout.BeginVertical("box");
				{
					DrawPartsButton();

					DrawClassButton();
				}
				GUILayout.EndVertical();

				GUILayout.BeginVertical("box");
				{
					GUILayout.Label("  애니메이션  ", buttonLabelStyle, GUILayout.Height(50));

					GUILayout.Space(20);

					DrawAnimationButton();
				}
				GUILayout.EndVertical();
			}
			GUILayout.EndScrollView();
			GUILayout.Space(10);
		}
		GUILayout.EndVertical();
	}

	protected void DrawNormalButton(string _name, bool _is_enable, System.Action _click_action, int _width = 0)
	{
		GUILayoutOption[] options = { GUILayout.ExpandWidth(true), GUILayout.Height(30) };
		GUI.skin.button.normal.background = null;

		content = new GUIContent(_name);
		GUI.enabled = _is_enable;
		if (GUILayout.Button(content, (GUILayout.Width(150)), GUILayout.Height(30)))
		{
			GUI.FocusControl("");
			_click_action?.Invoke();
			Save();
		}
		GUI.enabled = true;
	}

	protected void DrawRoleButton(string _name, CharacterTalent role, bool _is_enable, System.Action _click_action, int _width = 0)
	{
		GUILayoutOption[] options = { GUILayout.ExpandWidth(true), GUILayout.Height(30) };

		if (role == CurTalent)
			GUI.skin.button.normal.background = (Texture2D)button_select_tex;
		else
			GUI.skin.button.normal.background = null;

		content = new GUIContent(_name);
		GUI.enabled = _is_enable;
		if (GUILayout.Button(content, (_width == 0 ? GUILayout.ExpandWidth(true) : GUILayout.Width(_width)), GUILayout.Height(30)))
		{
			GUI.FocusControl("");
			_click_action?.Invoke();
			Save();
		}
		GUI.enabled = true;
	}

	protected void DrawPartsButton(string _name, ItemType type, bool _is_enable, System.Action _click_action, int _width = 0)
	{
		GUILayoutOption[] options = { GUILayout.ExpandWidth(true), GUILayout.Height(30) };

		if (type == CURPARTS)
			GUI.skin.button.normal.background = (Texture2D)button_select_tex;
		else
			GUI.skin.button.normal.background = null;

		content = new GUIContent(_name);
		GUI.enabled = _is_enable;

		GUILayout.BeginVertical();

		if (GUILayout.Button(content, (_width == 0 ? GUILayout.ExpandWidth(true) : GUILayout.Width(_width)), GUILayout.Height(30)))
		{
			GUI.FocusControl("");

			curSelectPart.charTalent = itemTypeSelectedList[type].charTalent;
			curSelectPart.charClass = itemTypeSelectedList[type].charClass;
			curSelectPart.partItemName = itemTypeSelectedList[type].partItemName;
			curSelectPart.index = itemTypeSelectedList[type].index;

			_click_action?.Invoke();
			Save();
		}

		{
			content = new GUIContent(curSelectedNameList[type]);
			GUILayout.Label(content, options);
		}

		GUILayout.EndVertical();

		GUI.enabled = true;
	}

	protected void DrawPartButton(string _name, bool _is_enable, CharacterClass type, int index, System.Action _click_action, int _width = 0)
	{
		GUILayoutOption[] options = { GUILayout.ExpandWidth(true), GUILayout.Height(30) };

		if (type == curSelectPart.charClass && index == curSelectPart.index)
			GUI.skin.button.normal.background = (Texture2D)button_select_tex;
		else
			GUI.skin.button.normal.background = null;

		content = new GUIContent(_name);

		if(CURPARTS == ItemType.body)
		{

		}

		GUI.enabled = _is_enable;
		if (GUILayout.Button(content, (_width == 0 ? GUILayout.ExpandWidth(true) : GUILayout.Width(_width)), GUILayout.Height(30)))
		{
			CURINDEX = index;

			GUI.FocusControl("");
			_click_action?.Invoke();
			Save();
		}
		GUI.enabled = true;
	}

	protected void DrawAnimButton(string _name, bool _is_enable, int index, System.Action _click_action, int _width = 0)
	{
		GUILayoutOption[] options = { GUILayout.ExpandWidth(true), GUILayout.Height(30) };

		if (index == CURANIMATION)
			GUI.skin.button.normal.background = (Texture2D)button_select_tex;
		else
			GUI.skin.button.normal.background = null;

		content = new GUIContent(_name);
		GUI.enabled = _is_enable;
		if (GUILayout.Button(content, (GUILayout.Width(150)), GUILayout.Height(30)))
		{
			GUI.FocusControl("");
			_click_action?.Invoke();
			Save();
		}
		GUI.enabled = true;
	}

	protected void DrawLabel(string _label, System.Action<string> _action, int _width = 150)
	{
		GUILayout.BeginHorizontal();
		{
			// - Label
			GUILayout.Label($"{_label} : ", dataLabelStyle, GUILayout.Width(_width), GUILayout.Height(30));

			_action?.Invoke(_label);
		}
		GUILayout.EndHorizontal();
	}

	protected string DrawTextField(string _label, string _text)
	{
		// - TextField
		GUI.SetNextControlName(_label);
		var text_style = GUI.skin.GetStyle("TextField");
		text_style.alignment = TextAnchor.MiddleLeft;
		text_style.fontSize = 12;
		return GUILayout.TextField(_text, text_style, GUILayout.Height(30));
	}

	protected int DrawIntField(string _label, int _value)
	{
		// - TextField
		GUI.SetNextControlName(_label);
		var text_style = GUI.skin.GetStyle("TextField");
		text_style.alignment = TextAnchor.MiddleLeft;
		text_style.fontSize = 12;
		return EditorGUILayout.IntField(_value, text_style, GUILayout.Height(30));
	}

	protected long DrawLongField(string _label, long _value)
	{
		// - TextField
		GUI.SetNextControlName(_label);
		var text_style = GUI.skin.GetStyle("TextField");
		text_style.alignment = TextAnchor.MiddleLeft;
		text_style.fontSize = 12;
		return EditorGUILayout.LongField(_value, text_style, GUILayout.Height(30));
	}

	private void DrawScrollView()
	{
		GUILayout.BeginHorizontal();
		{
			GUILayout.Label("- Data 바로찾기", orgLabelStyle, GUILayout.Width(150), GUILayout.Height(30));

			GUILayout.Label("# Quest Id 검색 : ", orgLabelStyle, GUILayout.Width(120), GUILayout.Height(30));
			searchQuestId = (uint)DrawLongField("SearchText", searchQuestId);
		}
		GUILayout.EndHorizontal();

		GUILayout.BeginHorizontal();
		{
			GUILayout.Space(20);

			dataListViewScrollPosition = GUILayout.BeginScrollView(dataListViewScrollPosition, GUILayout.ExpandWidth(true), GUILayout.Height(60));
			{
				var count = GetDataCount();
				var index = GUILayout.SelectionGrid(selectIndex, GetQuestIDs(), count, GUILayout.Width(100 * count), GUILayout.Height(30));
				if (selectIndex != index)
				{
					selectIndex = index;
				}
			}
			GUILayout.EndScrollView();
		}
		GUILayout.EndHorizontal();
	}
}
