using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEditor.SceneManagement;
public class dinoCreatePrefabEditorWindow : dinoPrefabEditorWindow
{
	static dinoCreatePrefabEditorWindow instance = null;

	//bool isPlay = false;
	static bool isExecute = false;

	[MenuItem("Tools/Dino Tool/Create Dino Prefab")]
	public static void Execute()
	{
		instance = GetWindow<dinoCreatePrefabEditorWindow>("Create Dino Prefab", typeof(dinoCreatePrefabEditorWindow));

		isExecute = true;
	}

	protected override void Initialize()
	{
		base.Initialize();
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
	}

	protected override void Save()
	{
		base.Save();
	}

	protected override void UnityPlay()
	{
		if (EditorApplication.isPlaying)
			EditorApplication.isPlaying = false;
		else
		{
			EditorApplication.isPlaying = true;
			//isPlay = true;
		}
	}

}
