using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEditor.SceneManagement;
public class dinoSkillEffectEditorWindow : dinoSkillEditorWindow
{
	static dinoSkillEffectEditorWindow instance = null;

	bool isInit = true;
	static bool isExecute = false;

	[MenuItem("Tools/Dino Tool/Skill&Effect Viewer")]
	public static void Execute()
	{
		if (!Utility.IsSymbolAlreadyDefined("DINO_SKILL_EDITOR"))
		{
			var result = EditorUtility.DisplayDialog("알림",
				"Dino Skill Viewer를 보려면 Define Symbol에 DINO_SKILL_EDITOR 존재해야합니다.\n\n자동으로 추가하시겠습니까?", "예", "아니오");
			if (result == true)
			{
				Utility.AddDefineSymbol("DINO_SKILL_EDITOR");
				instance = GetWindow<dinoSkillEffectEditorWindow>("Skill & Effect Viewer", typeof(dinoSkillEffectEditorWindow));
				isExecute = true;
				//var def = PlayerSettings.GetScriptingDefineSymbolsForGroup(EditorUserBuildSettings.selectedBuildTargetGroup);
				//PlayerSettings.SetScriptingDefineSymbolsForGroup(EditorUserBuildSettings.selectedBuildTargetGroup, def + ";DINO_SKILL_EDITOR");
			}
		}
		else
		{
			instance = GetWindow<dinoSkillEffectEditorWindow>("Skill & Effect Viewer", typeof(dinoSkillEffectEditorWindow));
			isExecute = true;
		}
	}

	protected override void Initialize()
	{
		LoadData();
		base.Initialize();
		isInit = true;
		if (!Application.isPlaying)
		{
			if (EditorSceneManager.GetActiveScene().name != "SkillViewer")
				EditorSceneManager.OpenScene($"Assets/Scenes/Tool/SkillViewer.unity");
		}
	}

	protected void LoadData()
	{
#if UNITY_EDITOR
		CSVDataManager.InitTables();
		SetSkillListByTalent();

		InitMonsterList();
#else
		CSVDataManager.InitAWS();
#endif
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

		selectedSkill = 500;
		monsterSelectedSkill = 401;
		base.OnEnable();
	}

	protected override void OnDestroy()
	{
		isExecute = false;
		isInit = true;
		talentIndex = 0;
		classIndex = 0;
		skillIndex = 0;
		monsterIndex = 0;
		monsterSkillIndex = 0;
		selectedSkill = 500;
		monsterSelectedSkill = 401;
		animTimeScaleIndex = 3;
		Time.timeScale = 1;
		Utility.RemoveDefineSymbol("DINO_SKILL_EDITOR");
	}

	protected void Update()
	{
		if(isInit)
		{
			if(dinoTool == null)
			{
				GameObject dino = GameObject.Find("DinoSkillTool");
				dinoTool = dino.GetComponent<DinoSkillTool>();
			}
			if(dinoTool != null && dinoTool.bInit && dinoTool.bInitAsset)
			{
				isInit = false;
				dinoTool.Initialize(monsterNameList[0]);
			}
		}
		if(dinoTool.skillPlaying && !skillPause)
			Repaint();
	}

	protected override void Save()
	{
		base.Save();
	}
}
