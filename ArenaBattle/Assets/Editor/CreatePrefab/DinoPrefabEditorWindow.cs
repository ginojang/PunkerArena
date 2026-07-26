using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Linq;
using System.IO;

public abstract class dinoPrefabEditorWindow : EditorWindow
{
	private string savePath = string.Empty;
	//private string infoPath = "Assets/Asset/Cha/Dino/info.prefab";
	private string g2pXML = "Assets/Asset/Cha/Dino";
	private Vector2 dataListViewScrollPosition;
	private Vector2 windowViewScrollPosition;
	protected bool isDirty;
	protected GUIStyle orgLabelStyle = null;
	protected GUIStyle titleLabelStyle = null;
	protected GUIStyle dataLabelStyle = null;
	protected GUIStyle buttonLabelStyle = null;

	//좌표리스트
	Dictionary<string, Dictionary<CharacterTalent, Vector3>> offsetList =
		new Dictionary<string, Dictionary<CharacterTalent, Vector3>>();
	
	protected abstract void UnityPlay();

	public GUISkin CustomGuiSkin;

	GUIContent content = new GUIContent();

	protected virtual void OnEnable()
	{
		Initialize();
	}

	protected virtual void Initialize()
	{
		isDirty = false;
		SetOffsetData();
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

		GUILayout.Space(10);

		GUILayout.BeginVertical("box");
		{
			windowViewScrollPosition = GUILayout.BeginScrollView(windowViewScrollPosition, GUILayout.ExpandWidth(true), GUILayout.ExpandHeight(true));
			{
				GUILayout.BeginHorizontal();
				{
					GUIStyle textFieldStyle = new GUIStyle(EditorStyles.textField);
					g2pXML = EditorGUILayout.TextField(" - FBX 폴더를 선택", g2pXML, textFieldStyle);
					if (GUILayout.Button("...", GUILayout.Width(50)))
					{
						var path = EditorUtility.OpenFolderPanel("Select Folder", g2pXML, "");
						if (string.IsNullOrEmpty(path) == false)
						{
							g2pXML = path;
						}
					}
				}
				GUILayout.EndHorizontal();
				GUILayout.Space(20);

				GUILayout.BeginHorizontal();
				{
					DrawNormalButton("FBX -> Prefab 변환", true, () =>
					{
						GUI.FocusControl("");
						CreatePrefab();
					});
					GUILayout.EndHorizontal();
				}
			}
			GUILayout.EndScrollView();
			GUILayout.Space(10);
		}
		GUILayout.EndVertical();
	}

	private bool CreatePrefab()
	{
		string[] dirList = Directory.GetDirectories(g2pXML);

		for(int i = 0; i < dirList.Length; i++)
		{
			// prefab 생성
			string pathname = Path.GetDirectoryName(dirList[i]);

			string[] dinoPaths = Directory.GetDirectories(pathname);

			string[] files = Directory.GetFiles(dinoPaths[i]);

			for(int k = 0; k < files.Length; k++)
			{
				string file = Path.GetFileName(files[k]);
				file = file.ToLower();
				if(file.Contains(".fbx") && !file.Contains(".meta"))
				{
					string filefullname = files[k];
					bool bRes = File.Exists(filefullname);
					int r = filefullname.IndexOf("Assets");
					filefullname = filefullname.Substring(r);
					GameObject modelRootGO = (GameObject)AssetDatabase.LoadAssetAtPath(filefullname, typeof(GameObject));
					var instanceRoot = (GameObject)PrefabUtility.InstantiatePrefab(modelRootGO);
					savePath = $"{pathname}\\prefab";//Path.GetDirectoryName(files[k]);
					if (!Directory.Exists(savePath))
						Directory.CreateDirectory(savePath);

					string saveFile = string.Empty;
					file = Path.GetFileNameWithoutExtension(file);
					saveFile = $"{savePath}\\{file}.prefab";

					CharacterTalent talent = CharacterTalent.None;
					if (file.Contains("ca_"))
					{
						talent = CharacterTalent.Carnivore;
					}
					else if (file.Contains("om_"))
					{
						talent = CharacterTalent.Omnivore;
					}
					else if (file.Contains("he_"))
					{
						talent = CharacterTalent.Herbivore;
					}
					
					//if(File.Exists(saveFile))
					//	File.Delete(saveFile);
					//GameObject variantRoot = PrefabUtility.SaveAsPrefabAsset(instanceRoot, saveFile);

					string animatorname = "";
					animatorname = "Assets/Asset/Cha/Dino/Ani/Animator/" + "animator_base.controller";
					
					var controller = AssetDatabase.LoadAssetAtPath<RuntimeAnimatorController>(animatorname);
					// script 추가
					Animator animator = instanceRoot.AddComponent<Animator>();
					Character character = instanceRoot.AddComponent<Character>();

					animator.runtimeAnimatorController = controller;

					//info 추가
					//GameObject infoObj = (GameObject)AssetDatabase.LoadAssetAtPath(infoPath, typeof(GameObject));
					//GameObject instanceInfo = (GameObject)PrefabUtility.InstantiatePrefab(infoObj);

					GameObject instanceInfo = new GameObject("info");
					
					instanceInfo.transform.SetParent(instanceRoot.transform);
					instanceInfo.transform.position = new Vector3(0, 1.3f, 0);
					character.InfoTransObject = instanceInfo;
					
					// material 적용
					filefullname = Path.GetDirectoryName(filefullname);
					filefullname = $"{filefullname}";
					
					string[] matfiles = Directory.GetFiles(filefullname);

					SkinnedMeshRenderer[] theRenderers = instanceRoot.GetComponentsInChildren<SkinnedMeshRenderer>();

					for (int t = 0; t < theRenderers.Length; t++)
					{
						string matname = string.Empty;
						string transname = theRenderers[t].name;
						
						if (transname.Contains("AH") || transname.Contains("body"))
						{
							// 공격용 입은 body 메트리얼로 강제 세팅
							string[] transnamelist = transname.Split('_');
							transnamelist[2] = "01";
							transnamelist[3] = "body";
							transname = $"{transnamelist[0]}_{transnamelist[1]}_{transnamelist[2]}_{transnamelist[3]}";
							matname = $"{filefullname}\\{transname}.mat";
						}
						else
						{
							matname = $"{filefullname}\\{transname}.mat";
						}
						
						Material mat = (Material)AssetDatabase.LoadAssetAtPath(matname, typeof(Material));
						theRenderers[t].material = mat;
					}

					// 이펙트 관련 본 더미 생성
					// head
					GameObject boneInfo = new GameObject("fx_head");
					Transform boneTrans = Utility.FindBone(instanceRoot.transform, "Bip001 Head");
					boneInfo.transform.localPosition =  offsetList["fx_head"][talent];
					boneInfo.transform.SetParent(boneTrans);
					
					//mouth
					boneInfo = new GameObject("fx_mouth");
					boneTrans = Utility.FindBone(instanceRoot.transform, "Bip001 Head");
					boneInfo.transform.localPosition =  offsetList["fx_mouth"][talent];
					boneInfo.transform.SetParent(boneTrans);

					//back
					boneInfo = new GameObject("fx_back");
					boneTrans = Utility.FindBone(instanceRoot.transform, "Bip001 Back");
					boneInfo.transform.localPosition =  offsetList["fx_back"][talent];
					boneInfo.transform.SetParent(boneTrans);
					
					//hit
					boneInfo = new GameObject("fx_hit");
					boneTrans = Utility.FindBone(instanceRoot.transform, "Bip001");
					boneInfo.transform.localPosition =  offsetList["fx_hit"][talent];
					boneInfo.transform.SetParent(boneTrans);
					
					//body
					boneInfo = new GameObject("fx_body");
					boneTrans = Utility.FindBone(instanceRoot.transform, "Bip001 Spine");
					boneInfo.transform.localPosition =  offsetList["fx_body"][talent];
					boneInfo.transform.SetParent(boneTrans);
					
					//tail
					boneInfo = new GameObject("fx_tail");
					boneTrans = Utility.FindBone(instanceRoot.transform, "Bip001 Tail2");
					boneInfo.transform.localPosition =  offsetList["fx_tail"][talent];
					boneInfo.transform.SetParent(boneTrans);
					
					ChangeLayersRecursively(instanceRoot.transform, "Character");
					PrefabUtility.SaveAsPrefabAsset(instanceRoot, saveFile);
					
					DestroyImmediate(instanceRoot);
					Save();
				}
			}
		}

		return true;
	}

	private void SetOffsetData()
	{
		offsetList.Clear();

		Dictionary<CharacterTalent, Vector3> offset = new Dictionary<CharacterTalent, Vector3>();
		
		offset.Add(CharacterTalent.Carnivore, new Vector3(0, 0.94f, 0.27f));
		offset.Add(CharacterTalent.Omnivore, new Vector3(0, 0.92f, 0.36f));
		offset.Add(CharacterTalent.Herbivore, new Vector3(0, 0.62f, 0.57f));
		offsetList.Add("fx_head", offset);
		
		offset = new Dictionary<CharacterTalent, Vector3>();
		offset.Add(CharacterTalent.Carnivore, new Vector3(0, 0.55f, 0.44f));
		offset.Add(CharacterTalent.Omnivore, new Vector3(0, 0.54f, 0.47f));
		offset.Add(CharacterTalent.Herbivore, new Vector3(0, 0.2f, 0.55f));
		offsetList.Add("fx_mouth", offset);
		
		offset = new Dictionary<CharacterTalent, Vector3>();
		offset.Add(CharacterTalent.Carnivore, new Vector3(0, 0.79f, -0.41f));
		offset.Add(CharacterTalent.Omnivore, new Vector3(0, 0.84f, -0.31f));
		offset.Add(CharacterTalent.Herbivore, new Vector3(0, 0.78f, -0.35f));
		offsetList.Add("fx_back", offset);
		
		offset = new Dictionary<CharacterTalent, Vector3>();
		offset.Add(CharacterTalent.Carnivore, new Vector3(0, 0.3f, 0.4f));
		offset.Add(CharacterTalent.Omnivore, new Vector3(0, 0.35f, 0.42f));
		offset.Add(CharacterTalent.Herbivore, new Vector3(0, 0.33f, 0.6f));
		offsetList.Add("fx_hit", offset);
		
		offset = new Dictionary<CharacterTalent, Vector3>();
		offset.Add(CharacterTalent.Carnivore, new Vector3(0, 0.3f, -0.11f));
		offset.Add(CharacterTalent.Omnivore, new Vector3(0, 0.32f, 0));
		offset.Add(CharacterTalent.Herbivore, new Vector3(0, 0.29f, 0.02f));
		offsetList.Add("fx_body", offset);
		
		offset = new Dictionary<CharacterTalent, Vector3>();
		offset.Add(CharacterTalent.Carnivore, new Vector3(0, 0.45f, -0.7f));
		offset.Add(CharacterTalent.Omnivore, new Vector3(0, 0.23f, -0.5f));
		offset.Add(CharacterTalent.Herbivore, new Vector3(0, 0.27f, -0.63f));
		offsetList.Add("fx_tail", offset);
	}
	
	private void ChangeLayersRecursively(Transform trans, string layer)
	{
		trans.gameObject.layer = LayerMask.NameToLayer(layer);
		foreach(Transform child in trans)
		{
			ChangeLayersRecursively(child, layer);
		}
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
}
