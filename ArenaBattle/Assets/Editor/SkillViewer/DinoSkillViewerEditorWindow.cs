using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Linq;
using System.IO;
using UnityEngine.UI;
using Generated.CsvData;

public class skillInfo
{
	public string skillName; // UI 용 이름
	public string aniName; // Talent 에 맞는 애니메이션 이름 찾아서 세팅
	public int skillAniIndex;
}

public abstract class dinoSkillEditorWindow : EditorWindow
{
	private string savePath = string.Empty;

	private Vector2 dataListViewScrollPosition;
	private Vector2 windowViewScrollPosition;
	protected bool isDirty;
	protected GUIStyle orgLabelStyle = null;
	protected GUIStyle titleLabelStyle = null;
	protected GUIStyle dataLabelStyle = null;
	protected GUIStyle buttonLabelStyle = null;
	private GUISkin CustomGuiSkin;
	private GUIContent content = new GUIContent();

	private string[] talentList = new string[] {"육식", "잡식", "초식"};
	private Dictionary<int, skillInfo> skillList = new Dictionary<int, skillInfo>();
	private Dictionary<int, CharacterClass> classList = new Dictionary<int, CharacterClass>();
	private Dictionary<int, eventData>  eventList = new Dictionary<int, eventData>();
	public int talentIndex = 0;
	public int classIndex = 0;
	public int skillIndex = 0;
	public int selectedSkill = 0;
	private skillInfo curSkillInfo;
	public DinoSkillTool dinoTool = null;
	public eventData infolist;

	public bool isPlay = false;
	private Action playBtn;
	private Action stopBtn;
	public bool skillPlaying = false;
	public bool skillPause = false;
	private bool selectDinoFold = false;
	private bool selectMonsterFold = false;
	private int selectEventIndex = 0;

	
	public int monsterIndex = 0;
	public int monsterSkillIndex = 0;
	public int monsterSelectedSkill = 0;
	private skillInfo curMonsterSkillInfo;
	public List<string> monsterNameList = new List<string>();
	List<int> monsterIDList = new List<int>();
	List<int> monsterSkillIDList = new List<int>();
	private Dictionary<int, skillInfo> monsterSkillList = new Dictionary<int, skillInfo>();
	private Dictionary<int, eventData>  monsterEventList = new Dictionary<int, eventData>();
	public eventData monsterInfolist;
	private int monsterSelectEventIndex = 0;
	public int animTimeScaleIndex = 4;
	private string[] animTimeScaleList = new string[] { "0.15", "0.25", "0.5", "0.75", "1", "1.5", "2" };

	private bool bPlayDino = true; //true : dino   false : monster
	protected virtual void OnEnable()
	{
		Initialize();
	}

	protected virtual void OnDisable()
	{
		talentIndex = 0;
		classIndex = 0;
		skillIndex = 0;
		monsterIndex = 0;
		monsterSkillIndex = 0;
	}
	
	protected virtual void Initialize()
	{
		isDirty = false;
		playBtn = null;
		stopBtn = null;
		skillPlaying = false;
		classList.Clear();
		SetSkillListByTalent();
		PartsTable dinoTbl = CSVDataManager.GetTable<PartsTable>();

		int count = 0;
		for (int i = 1; i < 11; i++)
		{
			Dictionary<int, Generated.CsvData.partsData> data = dinoTbl.GetDataList((CharacterClass) i);

			if (data != null)
			{
				classList.Add(count, (CharacterClass)i);
				count++;
			}
		}

		playBtn += OnPlay;
		stopBtn += OnStop;
	}

	protected virtual  void OnDestroy()
	{
		talentIndex = 0;
		classIndex = 0;
		skillIndex = 0;
		monsterIndex = 0;
		monsterSkillIndex = 0;
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

	protected virtual void Save()
	{
		AssetDatabase.SaveAssets();
		AssetDatabase.Refresh();

		isDirty = false;
		GUI.FocusControl("");

		Debug.Log("Success Save.");
	}

	void OnGUI()
	{
		InitGuiStyle();

		windowViewScrollPosition = GUILayout.BeginScrollView(windowViewScrollPosition, GUILayout.ExpandWidth(true), GUILayout.ExpandHeight(true));
		{
			DrawTitle(ReloadDataTable, SaveDataTable, UnityPlay);
			
			GUILayout.Space(20);
		
			//Select & Play
			selectDinoFold = EditorGUILayout.Foldout(selectDinoFold, "Dino Skill Area", true);

			if (selectDinoFold)
			{
				GUILayout.BeginVertical("box");
				{
					GUILayout.Space(10);
					GUILayout.BeginVertical();
					DrawTalent_Class();
					GUILayout.Space(20);
					DrawSkill();
					GUILayout.Space(20);
					DrawEventTriggerInfo();
					GUILayout.Space(50);
					DrawSkillPlayer();
					GUILayout.EndVertical();
				}

				GUILayout.Space(20);
				GUILayout.EndVertical();
			}
			
			GUILayout.Space(20);
			
			selectMonsterFold = EditorGUILayout.Foldout(selectMonsterFold, "Monster Skill Area", true);

			if (selectMonsterFold)
			{
				GUILayout.BeginVertical("box");
				{
					GUILayout.Space(10);
					GUILayout.BeginVertical();
					DrawMonsterList();
					GUILayout.Space(20);
					DrawMonsterSkill();
					GUILayout.Space(20);
					DrawMonsterEventTriggerInfo();
					GUILayout.Space(50);
					DrawMonsterSkillPlayer();
					GUILayout.EndVertical();
				}

				GUILayout.Space(20);
				GUILayout.EndVertical();
			}
			
			GUILayout.Space(20);
		}
		GUILayout.EndScrollView();
		GUILayout.Space(10);
	}

	// Talent Skill 가져오기
	public void SetSkillListByTalent()
	{
		skillList.Clear();
		eventList.Clear();
		SkillTable skillTbl = CSVDataManager.GetTable<SkillTable>();
		StringTable stringTbl = CSVDataManager.GetTable<StringTable>();
		AniskillTable aniskillTbl = CSVDataManager.GetTable<AniskillTable>();
		Anim_EventTable eventTbl = CSVDataManager.GetTable<Anim_EventTable>();

		//string aniname = "";
		
		//foreach (var skill in skillTbl.DicData)
		//{
  //          if (skill.Value.skillani != 0)
  //          {
  //              // aniskill 테이블에서 애니메이션 이름리스트를 얻어옴
  //              List<aniskillData> aniskillList = aniskillTbl.GetData(skill.Value.skillani);

  //              aniname = string.Empty;
  //              for (int i = 0; i < aniskillList.Count; i++)
  //              {
  //                  if (!aniskillList[i].aniName.Contains("ca_") && !aniskillList[i].aniName.Contains("he_") &&
  //                      !aniskillList[i].aniName.Contains("om_"))
  //                  {
  //                      //몬스터용 스킬이라서 패스
  //                      continue;
  //                  }
  //                  else
  //                  {
  //                      // 현재 선택되어 있는 Talent에 맞는 애니메이션만 세팅
  //                      if (aniskillList[i].aniName.Contains("ca_") && talentIndex == 0)
  //                      {
  //                          aniname = aniskillList[i].aniName;
  //                      }
  //                      else if (aniskillList[i].aniName.Contains("om_") && talentIndex == 1)
  //                      {
  //                          aniname = aniskillList[i].aniName;
  //                      }
  //                      else if (aniskillList[i].aniName.Contains("he_") && talentIndex == 2)
  //                      {
  //                          aniname = aniskillList[i].aniName;
  //                      }

  //                      if (aniname != string.Empty)
  //                      {
  //                          skillInfo info = new skillInfo();
  //                          var skillname = CSVDataManager.GetTable<StringTable>().GetData(skill.Value.skill_name, "");
  //                          info.skillName = $"skill index : {skill.Value.idx}  {skillname}";
  //                          info.aniName = aniname;
  //                          info.skillAniIndex = skill.Value.skillani;

  //                          skillList.Add(skill.Value.idx, info);

  //                          // 해당 애니메이션의 이벤트 정보 세팅
  //                          eventData data = eventTbl.GetData(info.aniName);

  //                          eventData eventinfolist = new eventData();
  //                          eventinfolist.index = 0;
  //                          eventinfolist.aniName = aniname;
  //                          eventinfolist.eventCount = data.eventCount;
  //                          eventinfolist.aniLenght = data.aniLenght;

  //                          foreach (var val in data.eventList)
  //                          {
  //                              AnimEventData eventinfo = new AnimEventData();
  //                              eventinfo.fTime = val.fTime;
  //                              eventinfo.eventName = val.eventName;
  //                              eventinfo.effectarrayidx = val.effectarrayidx;
  //                              eventinfo.effecttarget = val.effecttarget;
  //                              eventinfolist.eventList.Add(eventinfo);
  //                          }
  //                          eventList.Add(skill.Value.idx, eventinfolist);

  //                          break;
  //                      }
  //                  }
  //              }
  //          }
  //      }
		
		int count = 0;
		foreach (var skill in skillList)
		{
			if (skillIndex == count)
			{
				curSkillInfo = skill.Value;
				selectedSkill = skill.Key;
				break;
			}
			else
				count++;
		}
	}

	private void ChangeDinoByClass()
	{
		dinoTool.LoadStartCharacter((CharacterTalent)(talentIndex+1), classList[classIndex]);
	}
	
	private void ChangeMonster(string prefabname)
	{
		dinoTool.LoadMonster(prefabname);
	}

	private void DrawTitle(Action _reload_action, Action _save_action, Action _play_action)
	{
		GUILayout.BeginHorizontal();
		GUILayout.Label("[[[[[Skill & Effect Viewer ]]]]]", titleLabelStyle, GUILayout.Height(50));
		GUILayout.Space(15);
		DrawNormalButton("이벤트 테이블 다시 로드", true, () =>
		{
			GUI.FocusControl("");
			_reload_action?.Invoke();
		});
		
		DrawNormalButton("이벤트 테이블 저장", true, () =>
		{
			GUI.FocusControl("");
			_save_action?.Invoke();
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
		
		GUILayout.EndHorizontal();
	}

	private void DrawTalent_Class()
	{
		GUILayout.BeginHorizontal();
		GUILayout.Label("Select Dino Talent", dataLabelStyle, GUILayout.Height(50));
		GUILayout.Space(15);
		GUILayout.BeginVertical();
		GUILayout.Space(15);
		
		int talindex = EditorGUILayout.Popup("", talentIndex, talentList);
		
		GUILayout.EndVertical();
		
		GUILayout.Space(15);
		GUILayout.Label("Select Dino Class", dataLabelStyle, GUILayout.Height(50));
		GUILayout.Space(15);
		GUILayout.BeginVertical();
		GUILayout.Space(15);

		string[] classArray = new string[classList.Count];

		int count = 0;
		foreach (var data in classList)
		{
			classArray[count] = data.Value.ToString();
			count++;
		}
		int classindex = EditorGUILayout.Popup("", classIndex, classArray);
		
		GUILayout.EndVertical();
		GUILayout.EndHorizontal();
		
		bool bTalent = false;
		if (talindex != talentIndex || classindex != classIndex)
		{
			if (talindex != talentIndex)
			{
				bTalent = true;
				talentIndex = talindex;
			}

			if (bTalent)
				SetSkillListByTalent();

			classIndex = classindex;
			
			if (Application.isPlaying)
				ChangeDinoByClass();
		}

		classArray = null;
	}

	private void DrawSkill()
	{
		GUILayout.BeginHorizontal();
		GUILayout.Label("Select Dino Skill", dataLabelStyle, GUILayout.Height(50));
		GUILayout.Space(15);
		GUILayout.BeginVertical();
		GUILayout.Space(15);
		string[] skills = new string[skillList.Count];

		int i = 0;
		foreach (var info in skillList)
		{
			skills[i] = info.Value.skillName;
			i++;
		}
		
		int skillindex = EditorGUILayout.Popup("", skillIndex, skills);

		if (skillindex != skillIndex)
		{
			skillIndex = skillindex;
			int count = 0;
			foreach (var skill in skillList)
			{
				if (skillIndex == count)
				{
					curSkillInfo = skill.Value;
					selectedSkill = skill.Key;

					if (dinoTool != null)
						dinoTool.curSkillIndex = selectedSkill;
					break;
				}
				else
					count++;
			}
		}
		
		if(curSkillInfo != null)
			GUILayout.Label($"Current Skill Animation Name   :  {curSkillInfo.aniName}", dataLabelStyle, GUILayout.Height(50));

		GUILayout.EndVertical();
		GUILayout.EndHorizontal();
	}

	private void DrawEventTriggerInfo()
	{
		int count = 1;
		if (selectedSkill == 0 || eventList.Count == 0)
			return;
		infolist = eventList[selectedSkill];
		string[] ids = new string[infolist.eventCount];
		for (int k = 0; k < ids.Length; k++)
		{
			ids[k] = (k + 1).ToString();
		}
		GUIStyle textFieldStyle = new GUIStyle(EditorStyles.textField);
		GUILayout.BeginHorizontal();
			GUILayout.Label("- Animation Event Info", dataLabelStyle, GUILayout.Height(50));
			GUILayout.Space(30);
			EditorGUILayout.TextField("Event Count  : ", infolist.eventCount.ToString(), textFieldStyle);
		GUILayout.EndHorizontal();

		bool bChange = false;
		foreach (var info in infolist.eventList)
		{
			GUILayout.BeginVertical();
				GUILayout.BeginHorizontal();
				
				string eventtime = $"{count}     eventTime{count}";
				string invoketime = EditorGUILayout.TextField(eventtime, info.fTime.ToString(), textFieldStyle);
				if (!invoketime.Equals(info.fTime.ToString()))
				{
					info.fTime = float.Parse(invoketime);
					bChange = true;
				}

				GUILayout.Space(15);
				
				string eventname = $"eventFunc{count}";
				string invokename = EditorGUILayout.TextField(eventname, info.eventName, textFieldStyle);
				if (!invokename.Equals(info.eventName))
				{
					info.eventName = invokename;
					bChange = true;
				}

				GUILayout.Space(15);
				
				string eventarray = $"eventArrayIndex{count}";
				string effectindex = EditorGUILayout.TextField(eventarray, info.effectarrayidx.ToString(), textFieldStyle);
				if (!effectindex.Equals(info.effectarrayidx.ToString()))
				{
					info.effectarrayidx = int.Parse(effectindex);
					bChange = true;
				}

				GUILayout.Space(15);
				
				string eventtarget = $"eventTargetIndex{count}";
				string targetindex = EditorGUILayout.TextField(eventtarget, info.effecttarget.ToString(), textFieldStyle);
				if (!targetindex.Equals(info.effecttarget.ToString()))
				{
					info.effecttarget = int.Parse(targetindex);
					bChange = true;
				}

				GUILayout.Space(15);

				if (bChange)
					eventList[selectedSkill] = infolist;

				count++;
				GUILayout.EndHorizontal();
			GUILayout.EndVertical();
		}
		GUILayout.Space(30);
		
		GUILayout.Label("Select Info", dataLabelStyle, GUILayout.Height(50));
		var index = GUILayout.SelectionGrid(selectEventIndex, ids, infolist.eventCount);
		if (selectEventIndex != index)
		{
			selectEventIndex = index;
		}
		
		GUILayout.Space(30);
		
		GUILayout.BeginHorizontal();
		DrawNormalButton("Add Event", true, () =>
		{
			eventData infolist = eventList[selectedSkill];
			AnimEventData info = new AnimEventData();
			info.effectarrayidx = 0;
			info.effecttarget = 0;
			info.fTime = 0;
			info.eventName = "";
			
			infolist.eventList.Add(info);

			infolist.eventCount = infolist.eventList.Count;
		});
		
		DrawNormalButton("Insert Event", true, () =>
		{
			eventData infolist = eventList[selectedSkill];
			AnimEventData info = new AnimEventData();
			info.effectarrayidx = 0;
			info.effecttarget = 0;
			info.fTime = 0;
			info.eventName = "";
			
			infolist.eventList.Insert(selectEventIndex, info);
			infolist.eventCount = infolist.eventList.Count;
		});
		
		DrawNormalButton("Delete Event", true, () =>
		{
			eventData infolist = eventList[selectedSkill];

			infolist.eventList.RemoveAt(selectEventIndex);
			infolist.eventCount = infolist.eventList.Count;
		});

		DrawNormalButton("Save Event", true, () =>
		{
			foreach (var VARIABLE in eventList)
			{
				CSVDataManager.GetTable<Anim_EventTable>().ChangeValue(VARIABLE.Value);
			}
		});
		
		GUILayout.EndHorizontal();
	}
	
	private void DrawSkillPlayer()
	{
		GUILayout.BeginHorizontal();
		GUILayout.Label("Play Dino Skill", dataLabelStyle, GUILayout.Height(50));
		string play = "";
		if (dinoTool.skillPlaying && !skillPause)
			play = "Pause Skill";
		else
			play = "Play Skill";

		if (dinoTool.skillPlaying != skillPlaying)
		{
			skillPlaying = dinoTool.skillPlaying;
		}
		
		DrawNormalButton(play, true, () =>
		{
			if (Application.isPlaying)
			{
				bPlayDino = true;
				skillPlaying = !skillPlaying;
				dinoTool.skillPlaying = skillPlaying;

				if (skillPlaying)
				{
					if (skillPause)
					{
						Time.timeScale = float.Parse(animTimeScaleList[animTimeScaleIndex]);
						skillPause = false;
					}
					else
					{
						dinoTool.PlayElapseTime = 0f;
						//GUI.FocusControl("");
						playBtn?.Invoke();
					}
				}
				else
				{
					// pause가 눌렸음
					Time.timeScale = 0;
					skillPause = true;
				}
			}
		});
		
		GUILayout.Space(15);
		
		DrawNormalButton("Stop Skill", true, () =>
		{
			skillPlaying = false;
			dinoTool.skillPlaying = skillPlaying;
			
			GUI.FocusControl("");
			stopBtn?.Invoke();
		});
		
		GUILayout.Space(15);
		
		GUILayout.Label("Timeline");
		
		if(infolist!= null)
			EditorGUILayout.Slider(dinoTool.PlayElapseTime, 0f, infolist.aniLenght);
		GUILayout.Space(15);
		
		animTimeScaleIndex = EditorGUILayout.Popup("재생 속도", animTimeScaleIndex, animTimeScaleList);
		if (Time.timeScale != 0f)
		{
			Time.timeScale = float.Parse(animTimeScaleList[animTimeScaleIndex]);
		}

//		GUILayout.BeginVertical();
//		GUILayout.Space(15);
		
//		GUILayout.EndVertical();
		GUILayout.EndHorizontal();
	}
	
	public void ReloadDataTable()
	{
#if UNITY_EDITOR
		CSVDataManager.InitTables(true);
		SetSkillListByTalent();
#else
		CSVDataManager.InitAWS(true);
#endif
	}

	public void SaveDataTable()
	{
		CSVDataManager.GetTable<Anim_EventTable>().SaveTable("Assets/Resources/CSV/ani_event.csv");
	}
	
	private void UnityPlay()
	{
		if (EditorApplication.isPlaying)
			EditorApplication.isPlaying = false;
		else
		{
			EditorApplication.isPlaying = true;
			isPlay = true;
		}
	}
	
	private void OnPlay()
	{
		if(bPlayDino)
			dinoTool.StartAnimation(curSkillInfo.skillAniIndex, infolist, bPlayDino);
		else
			dinoTool.StartAnimation(curMonsterSkillInfo.skillAniIndex, monsterInfolist, bPlayDino);
	}

	private void OnStop()
	{
		dinoTool.StopAnimation();
		Repaint();
	}


	#region Monster Area

	private void DrawMonsterList()
	{
		GUILayout.BeginHorizontal();
		GUILayout.Label("Select Monster", dataLabelStyle, GUILayout.Height(50));
		GUILayout.Space(15);
		GUILayout.BeginVertical();
		GUILayout.Space(15);
		int monsterindex = EditorGUILayout.Popup("", monsterIndex, monsterNameList.ToArray());
		if (monsterindex != monsterIndex)
		{
			monsterIndex = monsterindex;
			SetMonster(monsterIndex);
			
			if (Application.isPlaying)
				ChangeMonster(monsterNameList[monsterIndex]);
		}

		GUILayout.EndVertical();
		GUILayout.EndHorizontal();
	}

	public void InitMonsterList()
	{
		StageMonsterTable monTbl = CSVDataManager.GetTable<StageMonsterTable>();
		
		monsterNameList.Clear();
		
		foreach (var data in monTbl.DicData)
		{
			for (int i = 0; i < data.Value.Count; i++)
			{
				if (!monsterNameList.Contains(data.Value[i].monsterprefab))
				{
					monsterNameList.Add(data.Value[i].monsterprefab);
					if (!monsterIDList.Contains(data.Value[i].monster))
					{
						monsterIDList.Add(data.Value[i].monster);
					}
				}
			}
		}

		SetMonster(0);
	}
	
	public void SetMonster(int selectIndex)
	{
		monsterIDList.Clear();
		monsterSkillIDList.Clear();

		string monster = monsterNameList[selectIndex];
		
		StageMonsterTable monTbl = CSVDataManager.GetTable<StageMonsterTable>();
		//DinoMonsterTable monsterTbl = CSVDataManager.GetTable<DinoMonsterTable>();

		foreach (var data in monTbl.DicData)
		{
			for (int i = 0; i < data.Value.Count; i++)
			{
				if(data.Value[i].monsterprefab != monster)
					continue;

				if (!monsterIDList.Contains(data.Value[i].monster))
					monsterIDList.Add(data.Value[i].monster);
			}
		}
		
		for (int i = 0; i < monsterIDList.Count; i++)
		{
			//MonsterData data = monsterTbl.GetData(monsterIDList[i]);

			//for (int j = 0; j < data.arraySkillData.Length; j++)
			//{
			//	if (!monsterSkillIDList.Contains(data.arraySkillData[j]) && data.arraySkillData[j] != 0)
			//	{
			//		monsterSkillIDList.Add(data.arraySkillData[j]);
			//	}
			//}
		}

		SetMonsterSkillList();
	}
	
	public void SetMonsterSkillList()
	{
		monsterSkillList.Clear();
		monsterEventList.Clear();
		SkillTable skillTbl = CSVDataManager.GetTable<SkillTable>();
		StringTable stringTbl = CSVDataManager.GetTable<StringTable>();
		AniskillTable aniskillTbl = CSVDataManager.GetTable<AniskillTable>();
		Anim_EventTable eventTbl = CSVDataManager.GetTable<Anim_EventTable>();

		//string aniname = "";

		//foreach (var id in monsterSkillIDList)
		//{
  //          skillData skill = skillTbl.GetData(id);
  //          List<aniskillData> aniskillList = aniskillTbl.GetData(skill.skillani);
  //          aniname = aniskillList[0].aniName;

  //          if (aniname != string.Empty)
  //          {
  //              skillInfo info = new skillInfo();
  //              var skillname = CSVDataManager.GetTable<StringTable>().GetData(skill.skill_name, "");
  //              info.skillName = $"skill index : {skill.idx}  {skillname}";
  //              info.aniName = aniname;
  //              info.skillAniIndex = skill.skillani;

  //              monsterSkillList.Add(skill.idx, info);

  //              // 해당 애니메이션의 이벤트 정보 세팅
  //              eventData data = eventTbl.GetData(info.aniName);

  //              eventData eventinfolist = new eventData();
  //              eventinfolist.index = 0;
  //              eventinfolist.aniName = aniname;
  //              eventinfolist.eventCount = data.eventCount;
  //              eventinfolist.aniLenght = data.aniLenght;

  //              foreach (var val in data.eventList)
  //              {
  //                  AnimEventData eventinfo = new AnimEventData();
  //                  eventinfo.fTime = val.fTime;
  //                  eventinfo.eventName = val.eventName;
  //                  eventinfo.effectarrayidx = val.effectarrayidx;
  //                  eventinfo.effecttarget = val.effecttarget;
  //                  eventinfolist.eventList.Add(eventinfo);
  //              }
  //              monsterEventList.Add(skill.idx, eventinfolist);
  //          }
  //      }

		monsterSelectedSkill = 0;
		
		int count = 0;
		foreach (var skill in monsterSkillList)
		{
			if (skillIndex == count)
			{
				curMonsterSkillInfo = skill.Value;
				monsterSelectedSkill = skill.Key;

				if (dinoTool != null)
					dinoTool.curMonsterSkillIndex = monsterSelectedSkill;
				break;
			}
			else
				count++;
		}
	}
	
	private void DrawMonsterSkill()
	{
		GUILayout.BeginHorizontal();
		GUILayout.Label("Select Monster Skill", dataLabelStyle, GUILayout.Height(50));
		GUILayout.Space(15);
		GUILayout.BeginVertical();
		GUILayout.Space(15);
		string[] skills = new string[monsterSkillList.Count];

		int i = 0;
		foreach (var info in monsterSkillList)
		{
			skills[i] = info.Value.skillName;
			i++;
		}
		
		int skillindex = EditorGUILayout.Popup("", monsterSkillIndex, skills);

		if (skillindex != monsterSkillIndex)
		{
			monsterSkillIndex = skillindex;
			int count = 0;
			foreach (var skill in monsterSkillList)
			{
				if (monsterSkillIndex == count)
				{
					curMonsterSkillInfo = skill.Value;
					monsterSelectedSkill = skill.Key;

					if (dinoTool != null)
						dinoTool.curMonsterSkillIndex = monsterSelectedSkill;
					break;
				}
				else
					count++;
			}
		}
		
		if(curMonsterSkillInfo != null)
			GUILayout.Label($"Current Skill Animation Name   :  {curMonsterSkillInfo.aniName}", dataLabelStyle, GUILayout.Height(50));

		GUILayout.EndVertical();
		GUILayout.EndHorizontal();
	}

	private void DrawMonsterEventTriggerInfo()
	{
		int count = 1;
		if (monsterSelectedSkill == 0 || eventList.Count == 0)
			return;

		if (monsterEventList.ContainsKey(monsterSelectedSkill))
			monsterInfolist = monsterEventList[monsterSelectedSkill];
		
		string[] ids = new string[monsterInfolist.eventCount];
		for (int k = 0; k < ids.Length; k++)
		{
			ids[k] = (k + 1).ToString();
		}
		GUIStyle textFieldStyle = new GUIStyle(EditorStyles.textField);
		GUILayout.BeginHorizontal();
			GUILayout.Label("- Animation Event Info", dataLabelStyle, GUILayout.Height(50));
			GUILayout.Space(30);
			EditorGUILayout.TextField("Event Count  : ", monsterInfolist.eventCount.ToString(), textFieldStyle);
		GUILayout.EndHorizontal();

		bool bChange = false;
		foreach (var info in monsterInfolist.eventList)
		{
			GUILayout.BeginVertical();
				GUILayout.BeginHorizontal();
				
				string eventtime = $"{count}     eventTime{count}";
				string invoketime = EditorGUILayout.TextField(eventtime, info.fTime.ToString(), textFieldStyle);
				if (!invoketime.Equals(info.fTime.ToString()))
				{
					info.fTime = float.Parse(invoketime);
					bChange = true;
				}

				GUILayout.Space(15);
				
				string eventname = $"eventFunc{count}";
				string invokename = EditorGUILayout.TextField(eventname, info.eventName, textFieldStyle);
				if (!invokename.Equals(info.eventName))
				{
					info.eventName = invokename;
					bChange = true;
				}

				GUILayout.Space(15);
				
				string eventarray = $"eventArrayIndex{count}";
				string effectindex = EditorGUILayout.TextField(eventarray, info.effectarrayidx.ToString(), textFieldStyle);
				if (!effectindex.Equals(info.effectarrayidx.ToString()))
				{
					info.effectarrayidx = int.Parse(effectindex);
					bChange = true;
				}

				GUILayout.Space(15);
				
				string eventtarget = $"eventTargetIndex{count}";
				string targetindex = EditorGUILayout.TextField(eventtarget, info.effecttarget.ToString(), textFieldStyle);
				if (!targetindex.Equals(info.effecttarget.ToString()))
				{
					info.effecttarget = int.Parse(targetindex);
					bChange = true;
				}

				GUILayout.Space(15);

				if (bChange)
					monsterEventList[monsterSelectedSkill] = monsterInfolist;

				count++;
				GUILayout.EndHorizontal();
			GUILayout.EndVertical();
		}
		GUILayout.Space(30);
		
		GUILayout.Label("Select Info", dataLabelStyle, GUILayout.Height(50));
		var index = GUILayout.SelectionGrid(monsterSelectEventIndex, ids, monsterInfolist.eventCount);
		if (monsterSelectEventIndex != index)
		{
			monsterSelectEventIndex = index;
		}
		
		GUILayout.Space(30);
		
		GUILayout.BeginHorizontal();
		DrawNormalButton("Add Event", true, () =>
		{
			eventData infolist = monsterEventList[monsterSelectedSkill];
			AnimEventData info = new AnimEventData();
			info.effectarrayidx = 0;
			info.effecttarget = 0;
			info.fTime = 0;
			info.eventName = "";
			
			infolist.eventList.Add(info);

			infolist.eventCount = infolist.eventList.Count;
		});
		
		DrawNormalButton("Insert Event", true, () =>
		{
			eventData infolist = monsterEventList[monsterSelectedSkill];
			AnimEventData info = new AnimEventData();
			info.effectarrayidx = 0;
			info.effecttarget = 0;
			info.fTime = 0;
			info.eventName = "";
			
			infolist.eventList.Insert(monsterSelectEventIndex, info);
			infolist.eventCount = infolist.eventList.Count;
		});
		
		DrawNormalButton("Delete Event", true, () =>
		{
			eventData infolist = monsterEventList[monsterSelectedSkill];

			infolist.eventList.RemoveAt(monsterSelectEventIndex);
			infolist.eventCount = infolist.eventList.Count;
		});

		DrawNormalButton("Save Event", true, () =>
		{
			foreach (var VARIABLE in monsterEventList)
			{
				CSVDataManager.GetTable<Anim_EventTable>().ChangeValue(VARIABLE.Value);
			}
		});
		
		GUILayout.EndHorizontal();
	}
	
	private void DrawMonsterSkillPlayer()
	{
		GUILayout.BeginHorizontal();
		GUILayout.Label("Play Dino Skill", dataLabelStyle, GUILayout.Height(50));
		string play = "";
		if (dinoTool.skillPlaying && !skillPause)
			play = "Pause Skill";
		else
			play = "Play Skill";

		if (dinoTool.skillPlaying != skillPlaying)
		{
			skillPlaying = dinoTool.skillPlaying;
		}
		
		DrawNormalButton(play, true, () =>
		{
			if (Application.isPlaying)
			{
				bPlayDino = false;
				skillPlaying = !skillPlaying;
				dinoTool.skillPlaying = skillPlaying;

				if (skillPlaying)
				{
					if (skillPause)
					{
						Time.timeScale = float.Parse(animTimeScaleList[animTimeScaleIndex]);
						skillPause = false;
					}
					else
					{
						dinoTool.PlayElapseTime = 0f;
						//GUI.FocusControl("");
						playBtn?.Invoke();
					}
				}
				else
				{
					// pause가 눌렸음
					Time.timeScale = 0;
					skillPause = true;
				}
			}
		});
		
		GUILayout.Space(15);
		
		DrawNormalButton("Stop Skill", true, () =>
		{
			skillPlaying = false;
			dinoTool.skillPlaying = skillPlaying;
			
			GUI.FocusControl("");
			stopBtn?.Invoke();
		});
		
		GUILayout.Space(15);
		
		GUILayout.Label("Timeline");
		
		if(infolist!= null)
			EditorGUILayout.Slider(dinoTool.PlayElapseTime, 0f, infolist.aniLenght);
		GUILayout.Space(15);
		
		animTimeScaleIndex = EditorGUILayout.Popup("재생 속도", animTimeScaleIndex, animTimeScaleList);
		if (Time.timeScale != 0f)
		{
			Time.timeScale = float.Parse(animTimeScaleList[animTimeScaleIndex]);
		}

//		GUILayout.BeginVertical();
//		GUILayout.Space(15);
		
//		GUILayout.EndVertical();
		GUILayout.EndHorizontal();
	}
	
	#endregion
	private void ChangeLayersRecursively(Transform trans, string layer)
	{
		trans.gameObject.layer = LayerMask.NameToLayer(layer);
		foreach(Transform child in trans)
		{
			ChangeLayersRecursively(child, layer);
		}
	}

	private void DrawNormalButton(string _name, bool _is_enable, System.Action _click_action, int _width = 0)
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

	private void DrawLabel(string _label, System.Action<string> _action, int _width = 150)
	{
		GUILayout.BeginHorizontal();
		{
			// - Label
			GUILayout.Label($"{_label} : ", dataLabelStyle, GUILayout.Width(_width), GUILayout.Height(30));

			_action?.Invoke(_label);
		}
		GUILayout.EndHorizontal();
	}

	private string DrawTextField(string _label, string _text)
	{
		// - TextField
		GUI.SetNextControlName(_label);
		var text_style = GUI.skin.GetStyle("TextField");
		text_style.alignment = TextAnchor.MiddleLeft;
		text_style.fontSize = 12;
		return GUILayout.TextField(_text, text_style, GUILayout.Height(30));
	}

	private int DrawIntField(string _label, int _value)
	{
		// - TextField
		GUI.SetNextControlName(_label);
		var text_style = GUI.skin.GetStyle("TextField");
		text_style.alignment = TextAnchor.MiddleLeft;
		text_style.fontSize = 12;
		return EditorGUILayout.IntField(_value, text_style, GUILayout.Height(30));
	}

	private long DrawLongField(string _label, long _value)
	{
		// - TextField
		GUI.SetNextControlName(_label);
		var text_style = GUI.skin.GetStyle("TextField");
		text_style.alignment = TextAnchor.MiddleLeft;
		text_style.fontSize = 12;
		return EditorGUILayout.LongField(_value, text_style, GUILayout.Height(30));
	}
}
