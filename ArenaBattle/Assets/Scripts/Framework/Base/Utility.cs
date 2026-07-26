using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography;
using System.Xml;
using UnityEngine;
using UnityEngine.AI;
using Devil.Common;
using System.Text.RegularExpressions;
using UnityEditor;
public static class Utility
{
#if !SR_TOOL
	public const int MAX_TEXTURE_SIZE = 4096;
	public const float EYEPOS_OFFSET = 1.27f;

	// public static bool UseAssetStreaming = false;

	public const string HTTP_PATH = "http://70.16.1.49:8000/Android/";//"http://192.168.0.10/LUE/";
	public const string LOCAL_PATH = "file://";
	public const string LOCAL_ANDROID_ASSETSTREAMING_PATH = "jar:file://";

	public const string SOUND_UI_ROOT_PATH = "Assets/LUE/Res/Sound/";

	public enum ASSETBUNDLE_TYPE
	{
		EDITOR_LOAD,            // 유니티 에디터 환경에서 로드
		STREAMING_ASSETS,   // StreamingAssets 폴더에 있는 어셋번들 로드
		DOWNLOAD            // 다운로드 하여 어셋 번들 로드
	}

	public static string GetLastDic(string targetPath)
	{
		var paths = targetPath.Split(new[] { '/' }, StringSplitOptions.RemoveEmptyEntries);
		if (0 < paths.Length)
		{
			return paths[paths.Length - 1];
		}
		return string.Empty;
	}

	public static string StreamingAssetsPath(string filename)
	{
		return System.IO.Path.Combine(Application.streamingAssetsPath, filename);
	}

	private static Texture2D _white_texture;

	public static Texture2D WhiteTexture
	{
		get
		{
			if (_white_texture == null)
			{
				_white_texture = Resources.Load<Texture2D>("Texture/WhiteTexture");
			}
			return _white_texture;
		}
	}

	public static Transform FindBone(Transform obj, string bonename)
	{
		Transform returnChild = null;
		for (int i = 0; i < obj.transform.childCount; i++)
		{
			Transform child = obj.transform.GetChild(i);
			if (child.name == bonename)
			{
				returnChild = child;
				break;
			}
			else
			{
				if (child.childCount > 0)
					returnChild = FindBone(child, bonename);
			}

			if (returnChild != null)
				break;
		}

		return returnChild;
	}

	public static void BoneLog(Transform obj)
	{
		for(int i = 0; i < obj.transform.childCount; i++)
		{
			Transform child = obj.transform.GetChild(i);
			Debug.Log(obj.transform.GetChild(i).name);

			if (child.childCount > 0)
				BoneLog(child);
		}
	}
	
	public static T CreateInstance<T>(params object[] args) where T : class
	{
		return (T)CreateInstance(typeof(T), args);
	}


	public static T CreateInstance<T>(Type type, params object[] args) where T : class
	{
		return (T)CreateInstance(type, args);
	}


	public static object CreateInstance(Type type, params object[] args)
	{
		// constructor를 먼저 모두 얻은 다음, 하나씩 type을 비교해서 가장 맞는 type을 찾음
		// args type list를 넘겨서 한 번에 constructor를 얻지 않는 이유는, type 의 base type을 인자로 받는 경우도 제대로 처리하기 위함
		const BindingFlags Binding_flags = BindingFlags.Instance | BindingFlags.Public;
		var ctors = type.GetConstructors(Binding_flags);

		// args type 비교
		ConstructorInfo best_match_ctor = null;
		int best_match_distance = -1;
		foreach (var ctor in ctors)
		{
			int match_distance = _CalcMethodMatchDistance(ctor, args);
			if (match_distance >= 0 && (best_match_ctor == null || match_distance < best_match_distance))
			{
				best_match_distance = match_distance;
				best_match_ctor = ctor;
			}
		}

		// ctor 실행
		if (best_match_ctor == null)
		{
			Debug.LogError("CreateInstance ERROR - No matching ctor : type = " + type + ",  args = " + args);
			return null;
		}
		return best_match_ctor.Invoke(args);
	}


	private static int _CalcMethodMatchDistance(MethodBase method, object[] args)
	{
		Debug.Assert(method != null);
		var method_params = method.GetParameters();
		Debug.Assert(method_params != null);

		// params이 비었을 때 처리
		if (args == null || args.Length == 0)
		{
			return method_params.Length == 0 ? 0 : -1;
		}
		if (args.Length != method_params.Length)
		{
			return -1;
		}

		// 각 param 검사
		int match_distance = 0;
		int args_index = 0;
		foreach (var param in method_params)
		{
			// arg type 얻기
			Type param_type = param.ParameterType;
			Type arg_type;
			{
				var arg_object = args[args_index++];
				if (arg_object == null)
				{
					if (!param_type.IsClass)
						return -1;
					continue;
				}
				arg_type = arg_object.GetType();
			}

			// type이 같은지 검사, 같으면 distance를 증가시키지 않고 통과하기
			if (param_type == arg_type) continue;
			if (!arg_type.IsSubclassOf(param_type))
			{
				return -1;
			}

			// type이 같지 않지만 subclass이면, distance 증가
			do
			{
				match_distance++;
				Debug.Assert(arg_type != null);
				arg_type = arg_type.BaseType;
			}
			while (arg_type != param_type);
		}

		// 리턴
		return match_distance;
	}

	public static void Swap<T>(ref T value1, ref T value2)
	{
		T temp_value = value2;
		value2 = value1;
		value1 = temp_value;
	}

	public static void TryParse(string sz, out Vector3 v)
	{
		v = Vector3.zero;

		if (false == string.IsNullOrEmpty(sz))
		{
			string[] tab = sz.Split(","[0]);

			v.x = float.Parse(tab[0], System.Globalization.NumberStyles.Float);
			v.y = float.Parse(tab[1], System.Globalization.NumberStyles.Float);
			v.z = float.Parse(tab[2], System.Globalization.NumberStyles.Float);
		}
	}

	public static void TryParse(string sz, out Vector2 v)
	{
		v = Vector2.zero;

		if (false == string.IsNullOrEmpty(sz))
		{
			string[] tab = sz.Split(","[0]);

			v.x = float.Parse(tab[0], System.Globalization.NumberStyles.Float);
			v.y = float.Parse(tab[1], System.Globalization.NumberStyles.Float);
		}
	}

	public static void TryParse(string sz, out float f, float default_value = 0.0f)
	{
		f = default_value;

		if (false == string.IsNullOrEmpty(sz))
		{
			f = float.Parse(sz);
		}
	}

	public static void TryParse(string sz, out double f, double default_value = 0.0f)
	{
		f = default_value;

		if (false == string.IsNullOrEmpty(sz))
		{
			f = double.Parse(sz);
		}
	}

	public static void TryParse(string sz, out long n, long default_value = 0L)
	{
		n = default_value;

		if (false == string.IsNullOrEmpty(sz))
		{
			if (false == long.TryParse(sz, out n))
			{
				System.Text.StringBuilder sb = new System.Text.StringBuilder();
				foreach (char c in sz)
				{
					if (c >= '0' && c <= '9')
					{
						sb.Append(c);
					}
				}
				if (false == long.TryParse(sb.ToString(), out n))
				{
					Debug.LogError("TryParse LongType converting error!!! string = " + sz);
				}
			}
		}
	}

	public static T Convert<T>(string sz, out T n, T default_value = default)
	{
		n = default_value;

		if (false == string.IsNullOrEmpty(sz))
		{
			try
			{
				var converter = System.ComponentModel.TypeDescriptor.GetConverter(typeof(T));
				if (converter != null)
				{
					n = (T)converter.ConvertFromString(sz);
				}
			}
			catch (NotSupportedException)
			{
				Debug.LogError("LongType converting error!!! string = " + sz);
			}
		}
		return n;
	}

	public static void TryParse(string sz, out int n, int default_value = 0)
	{
		n = default_value;

		if (false == string.IsNullOrEmpty(sz))
		{
			n = int.Parse(sz);
		}
	}

	public static void TryParse(string sz, out bool b, bool default_value = false)
	{
		b = default_value;

		if (string.IsNullOrEmpty(sz)) return;
		string tmp = sz.ToLower();

		switch (tmp)
		{
			case "true":
				b = true;
				break;
			case "false":
				b = false;
				break;
			default:
				b = "0" != tmp;
				break;
		}
	}


	public static string GetXmlChildText(XmlNode node, params string[] attr_names)
	{
		Debug.Assert(node != null);

		foreach (var attr_name in attr_names)
		{
			node = node[attr_name];
			if (node == null) return null;
		}
		return node.InnerText;
	}

	public static string GetXmlChildText(string default_value, XmlNode node, params string[] attr_names)
	{
		Debug.Assert(node != null);

		foreach (var attr_name in attr_names)
		{
			node = node[attr_name];
			if (node == null) return default_value;
		}
		return node.InnerText;
	}


	public static string GetXmlAttributeText(XmlNode node, string attr_name)
	{
		if (node.Attributes == null)
			return null;
		var attr = node.Attributes[attr_name];
		return attr == null ? null : attr.InnerText;
	}


	public static string GetXmlAttributeText(string default_value, XmlNode node, string attr_name)
	{
		if (node.Attributes == null)
			return default_value;
		var attr = node.Attributes[attr_name];
		return attr == null ? default_value : attr.InnerText;
	}


	public static T MakeComponent<T>(this GameObject game_object) where T : Component
	{
		if (null == game_object)
			return null;

		// 아래처럼 ?? 연산자를 사용하면, AddComponent가 제대로 실행되지 않는다. 반드시 풀어 써줘야 한다.
		//var component = go.GetComponent<T>() ?? go.AddComponent<T>();
		// ReSharper disable ConvertIfStatementToNullCoalescingExpression
		var component = game_object.GetComponent<T>();
		if (component == null)
		{
			component = game_object.AddComponent<T>();
			Debug.Assert(component != null);
		}
		// ReSharper restore ConvertIfStatementToNullCoalescingExpression
		return component;
	}


	// public static void RequestDownloadCoroutine(string fullPath, AssetLoader.cbFinishLoad on_finish, object param=null)
	// {
	//     AssetBundleLoader bundle_loader = AssetBundleHolder.CreateLoader(fullPath);
	//     if (null == bundle_loader) return;

	//     //string filepath = NpUtil.RESOURCE_FOLDER_FULL_PATH + "/" + bundleName + ".unity3d";
	//     // string filepath = bundle_name + ".unity3d";

	//     bundle_loader.SetDownloadFilePath(fullPath);
	//     bundle_loader.SetEventFinishLoad(on_finish, param);

	//     NpImmortal.BundleService.RequestDownloadCoroutine(bundle_loader, null, null);
	// }


	public static void UseCustomShader(GameObject game_object)
	{
		// if (null == game_object)
		// {
		//     return;
		// }

		// // self
		// _UseCustomShader(game_object.GetComponent<Renderer>());

		// // childs
		// Renderer[] renderers = game_object.GetComponentsInChildren<Renderer>();

		// foreach (Renderer renderer in renderers)
		// {
		//     _UseCustomShader(renderer);
		// }
	}

	private static void _UseCustomShader(Renderer renderer)
	{
		if (null == renderer) return;

		Material[] materials = renderer.sharedMaterials;
		if (null == materials) return;

		foreach (Material material in materials.Where(t => null != t).Where(t => null != t.shader))
		{
			material.shader = Shader.Find(material.shader.name);
		}
	}

	public static void UseCustomShader_UILabel(GameObject game_object)
	{
		// if (null == game_object)
		// {
		//     return;
		// }

		// // self
		// _UseCustomShader(game_object.GetComponent<UILabel>());

		// // childs
		// UILabel[] labels = game_object.GetComponentsInChildren<UILabel>();

		// foreach (UILabel label in labels)
		// {
		//     _UseCustomShader(label);
		// }
	}

	// private static void _UseCustomShader(UILabel label)
	// {
	//     if (null != label && null != label.bitmapFont)
	//     {
	//         if (null != label.bitmapFont.material)
	//             label.bitmapFont.material.shader = Shader.Find(label.bitmapFont.material.shader.name);
	//     }

	//     if (null != label && null != label.trueTypeFont)
	//     {
	//         if (null != label.trueTypeFont.material)
	//             label.trueTypeFont.material.shader = Shader.Find(label.trueTypeFont.material.shader.name);
	//     }
	// }

	public static void SetLayer(GameObject game_object, int layer, bool is_recursive)
	{
		if (null == game_object) return;
		game_object.layer = layer;
		if (!is_recursive) return;

		_SetLayer_Recursive(game_object, layer);
	}

	private static void _SetLayer_Recursive(GameObject game_object, int layer)
	{
		for (int i = 0; i < game_object.transform.childCount; ++i)
		{
			var child_game_object = game_object.transform.GetChild(i).gameObject;
			if (child_game_object == null) continue;
			child_game_object.layer = layer;
			_SetLayer_Recursive(child_game_object.gameObject, layer);
		}
	}

	public static Vector3 LueToUnityPosistion(float x, float y, float z)
	{
		return new Vector3(-x, y, -z);
	}

	public static Vector3 LueToUnityPosistion(Vector3 lue)
	{
		return new Vector3(-lue.x, lue.y, -lue.z);
	}

	public static Vector3 LueToUnityRotation(float x, float y, float z)
	{
		// LUE : yaw, pitch, roll
		// Unity : pitch, yaw, roll
		return new Vector3(y, x, z);
	}

	public static Vector3 LueToUnityRotation(Vector3 lue)
	{
		return new Vector3(lue.y, lue.x, lue.z);
	}

	public static float LueToUnityFOV(float lue, float aspect)
	{
		float fov = Mathf.Tan(lue * Mathf.Deg2Rad * 0.5f) / aspect;
		fov = Mathf.Atan(fov) * 2.0f * Mathf.Rad2Deg;
		return fov;
	}


	// public static bool FileExists(string path)
	// {
	//     if (UseAssetStreaming)
	//     {
	//         return Application.platform == RuntimePlatform.Android
	//             ? System.IO.File.Exists(Application.dataPath + "!/assets/" + path)
	//             : System.IO.File.Exists(Application.dataPath + "/StreamingAssets/" + path);
	//     }

	//     string tmp = Application.persistentDataPath + "/StreamingAssets/" + path;
	//     tmp = tmp.Replace('/', '\\');

	//     return System.IO.File.Exists(tmp);
	// }


	public static void CopyTransformTo(this GameObject from_object, GameObject to_object)
	{
		to_object.transform.position = from_object.transform.position;
		to_object.transform.rotation = from_object.transform.rotation;
		to_object.transform.localScale = from_object.transform.localScale;
	}

	public static void CopyTransformTo(this Component from_object, Component to_object)
	{
		to_object.transform.position = from_object.transform.position;
		to_object.transform.rotation = from_object.transform.rotation;
		to_object.transform.localScale = from_object.transform.localScale;
	}

	public static void CopyTransformTo(this Component from_object, GameObject to_object)
	{
		to_object.transform.position = from_object.transform.position;
		to_object.transform.rotation = from_object.transform.rotation;
		to_object.transform.localScale = from_object.transform.localScale;
	}

	public static void CopyTransformTo(this GameObject from_object, Component to_object)
	{
		to_object.transform.position = from_object.transform.position;
		to_object.transform.rotation = from_object.transform.rotation;
		to_object.transform.localScale = from_object.transform.localScale;
	}

	public static void ShuffleArray<T>(T[] array)
	{
		int random1;
		int random2;

		T tmp;

		for (int index = 0; index < array.Length; ++index)
		{
			random1 = UnityEngine.Random.Range(0, array.Length);
			random2 = UnityEngine.Random.Range(0, array.Length);

			tmp = array[random1];
			array[random1] = array[random2];
			array[random2] = tmp;
		}
	}

	/// <summary>
	/// CreateRandomIndexList
	/// </summary>
	/// <param name="list"></param>
	/// <param name="maxCount"></param>
	public static void CreateRandomIndexList(this List<int> list, int maxCount)
	{
		list.FillIndexList(maxCount);
		list.ShuffleList();
	}

	public static void FillIndexList(this List<int> list, int maxCount)
	{
		if (list.Count > 0)
		{
			list.Clear();
		}

		for (int i = 0; i < maxCount; i++)
		{
			list.Add(i);
		}
	}

	public static void ShuffleList<T>(this List<T> list)
	{
		int random1;
		int random2;

		T tmp;

		for (int index = 0; index < list.Count; ++index)
		{
			random1 = UnityEngine.Random.Range(0, list.Count);
			random2 = UnityEngine.Random.Range(0, list.Count);

			tmp = list[random1];
			list[random1] = list[random2];
			list[random2] = tmp;
		}
	}

	public static void RendererEnable(GameObject obj, bool enable)
	{
		if (obj == null)
			return;

		var rendererComp = obj.GetComponent<Renderer>();
		if (rendererComp != null)
		{
			rendererComp.enabled = enable;
		}

		Renderer[] renderers = obj.GetComponentsInChildren<Renderer>();
		foreach (Renderer renderer in renderers)
		{
			renderer.enabled = enable;
		}
	}

	public static bool GetRendererEnable(GameObject obj)
	{
		if (obj == null)
			return false;

		var rendererComp = obj.GetComponent<Renderer>();
		if (rendererComp != null)
			return rendererComp.enabled;

		var enable = false;
		Renderer[] renderers = obj.GetComponentsInChildren<Renderer>();
		foreach (Renderer renderer in renderers)
		{
			enable |= renderer.enabled;
		}

		return enable;
	}
	
	public static string GetFileNameByTalent(CharacterTalent talent, string filename)
	{
		string prefix = "";

		if (filename != "none")
		{
			switch (talent)
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
		}

		return filename = prefix + filename;
	}
	
#region string
	public static string[] GetSplitTextArray(this string context, char separator = ' ')
	{
		return context.Split(separator);
	}
#endregion string

#region MonoExtension
	public static void Invoke(this MonoBehaviour m, Action method, float time)
	{
		m.Invoke(method.Method.Name, time);
	}
#endregion MonoExtension

#region logs
	public static void H2Log(string text)
	{
		Debug.Log($"<color=green>[LOG]</color>: <color=black>{text}</color>");
	}

	public static void H2LogWarning(string text)
	{
		Debug.Log($"<color=blue>[WARNING]</color>: <color=black>{text}</color>");
	}

	public static void H2LogError(string text)
	{
		Debug.Log($"<color=red>[ERROR]</color>: <color=black>{text}</color>");
	}

	public static void H2LogTest(string text)
	{
		Debug.Log($"<color=red>{text}</color>");
	}
#endregion

#region Camera
	private static Camera GetCurrentSceneCameraInternal()
	{
		return UnityScene.CurrentUnityScene?.FrameObject?.FindChildByNameInChildren("Scene Camera")?.GetComponent<Camera>();
	}

	public static Camera GetCurrentSceneCamera()
	{
		Camera the = GetCurrentSceneCameraInternal();
		if (the == null)
		{
			Debug.LogError($"won't find a scene camera while loading scenes.");
		}

		return the;
	}

    public static GameObject GetDeepChild(GameObject dstObject, string childName)
    {
        if (dstObject == null)
            return null;

        if (dstObject.name == childName)
            return dstObject;

        for (int i = 0; i < dstObject.transform.childCount; i++)
        {
            Transform childTransform = dstObject.transform.GetChild(i);
            GameObject childObject = GetDeepChild(childTransform.gameObject, childName);
            if (null != childObject)
                return childObject;
        }
        return null;
    }

    #endregion

    #region Editor DefineSymbol Controll
#if UNITY_EDITOR
    public struct DefineSymbolData
	{
		public BuildTargetGroup buildTargetGroup; // 현재 빌드 타겟 그룹
		public string fullSymbolString;           // 현재 빌드 타겟 그룹에서 정의된 심볼 문자열 전체
		public Regex symbolRegex;

		public DefineSymbolData(string symbol)
		{
			buildTargetGroup = EditorUserBuildSettings.selectedBuildTargetGroup;
			fullSymbolString = PlayerSettings.GetScriptingDefineSymbolsForGroup(buildTargetGroup);
			symbolRegex = new Regex(@"\b" + symbol + @"\b(;|$)");
		}
	}

	/// <summary> 심볼이 이미 정의되어 있는지 검사 </summary>
	public static bool IsSymbolAlreadyDefined(string symbol)
	{
		DefineSymbolData dsd = new DefineSymbolData(symbol);

		return dsd.symbolRegex.IsMatch(dsd.fullSymbolString);
	}

	/// <summary> 심볼이 이미 정의되어 있는지 검사 </summary>
	public static bool IsSymbolAlreadyDefined(string symbol, out DefineSymbolData dsd)
	{
		dsd = new DefineSymbolData(symbol);

		return dsd.symbolRegex.IsMatch(dsd.fullSymbolString);
	}

	/// <summary> 특정 디파인 심볼 추가 </summary>
	public static void AddDefineSymbol(string symbol)
	{
		// 기존에 존재하지 않으면 끝에 추가
		if (!IsSymbolAlreadyDefined(symbol, out var dsd))
		{
			PlayerSettings.SetScriptingDefineSymbolsForGroup(dsd.buildTargetGroup, $"{dsd.fullSymbolString};{symbol}");
		}
	}

	/// <summary> 특정 디파인 심볼 제거 </summary>
	public static void RemoveDefineSymbol(string symbol)
	{
		// 기존에 존재하면 제거
		if (IsSymbolAlreadyDefined(symbol, out var dsd))
		{
			string strResult = dsd.symbolRegex.Replace(dsd.fullSymbolString, "");

			PlayerSettings.SetScriptingDefineSymbolsForGroup(dsd.buildTargetGroup, strResult);
		}
	}
#endif
#endregion

#region Animator
	public static float GetCurrentAnimationLength(this Animator animator)
	{
		// Don't get confused, this one has already considered length / the current animation state's speed.
		return animator.GetCurrentAnimatorStateInfo(0).length;
	}

	public static bool IsPlaying(this Animator animator)
	{
		Debug.Assert(animator);
		return animator.GetCurrentAnimatorStateInfo(0).normalizedTime < 1;
	}

	public static void ResetAnimationOf(this Animator animator, string animationName)
	{
		animator.Play(animationName, -1, 0f);
	}

	/// <summary>
	/// Force to play animation with time normalized. (For base layer only)
	/// </summary>
	/// <param name="normalizedTime">start=0, end=0</param>
	public static void ForceToPlayAnimation(this Animator animator, string animationName, float normalizedTime)
	{
		animator.Play(animationName, -1, normalizedTime);
		animator.Update(0f);
	}

	public static void SetAnimatorOverrideData(AnimationClipOverrides overriedList, int tableIndex, CharacterTalent talent, bool isWing)
    {
		Generated.CsvData.aniTableData tableData = null;
		AnimationClip aniClip = null;

		tableData = CSVDataManager.GetTable<AniTable>().GetData(tableIndex, talent, isWing);

		if (tableData == null)
			Debug.LogError($"AniTable 없음 : Index={tableIndex}, talent={talent}, isWing={isWing}");

		aniClip = ResourcePoolManager.Instance.GetResourceData(tableData.res_ani) as AnimationClip;

		if (aniClip == null)
			Debug.LogError($"AniClip 없음 : Index={tableData.res_ani}");

		overriedList[tableData.state] = aniClip;
	}
#endregion

	#region AI
	public static bool IsStop(this NavMeshAgent agent)
	{
		if (!agent.pathPending)
		{
			if (agent.remainingDistance <= agent.stoppingDistance)
			{
				if (!agent.hasPath || agent.velocity.sqrMagnitude == 0f)
				{
					return true;
				}
			}
		}

		return false;
	}
#endregion
#endif

#region Date

	/// <summary>
	/// Get a local time (not UTC one) span compared to date time now.
	/// </summary>
	/// <param name="expirationDate"></param>
	/// <returns></returns>
	public static TimeSpan GetTimeSpanFromNowDate(string expirationDate)
	{
		DateTime StartDate = System.Convert.ToDateTime(DateTime.Now);
		DateTime EndDate = System.Convert.ToDateTime(expirationDate);

		//Exception test code
		//DateTime EndDate = DateTime.Now;

		// A normal item could be here in case that there are period items left in the detail dialog for previwing.
		/*
		try
		{			
			EndDate = System.Convert.ToDateTime(expirationDate);
		}
		catch
		{
			int woody = 0;
		}
		*/

		TimeSpan dateDiff = EndDate - StartDate;
		return dateDiff;
	}

	public static bool IsExpired(this DateTime endDate)
	{
		DateTime StartDate = System.Convert.ToDateTime(DateTime.Now);
		int result = DateTime.Compare(StartDate, endDate);
		if (result > 0)
		{
			// EndDate is bigger, meaning not expired yet.
			return true;
		}
		/*
		else if (result < 0)
		{
			// EndDate is bigger, meaning not expired yet.
		}
		else
		{
			// Being equal
		}
		*/

		return false;
	}

	public static TimeSpan GetTimeSpanFromNowDate(this DateTime dateTime)
	{
		TimeSpan dateDiff = dateTime - DateTime.Now;
		return dateDiff;
	}

	public static TimeSpan GetTimeSpanFromSeconds(int seconds)
	{
		return TimeSpan.FromSeconds(seconds);
	}

	public static TimeSpan GetTimeSpanFromDate(string date)
	{
		DateTime StartDate = System.Convert.ToDateTime(date);
		DateTime NowDate = System.Convert.ToDateTime(DateTime.Now);

		TimeSpan dateDiff = NowDate - StartDate;
		return dateDiff;
	}

	public static DateTime GetDateTimeFromDate(string date)
	{
		DateTime dateTime = System.Convert.ToDateTime(date);

		return dateTime;
	}

	public static string GetDateTimeTextOf24Hours(this DateTime dateTime)
	{
		//"0000/00/ 00:00"
		return dateTime.ToString("yyyy-MM-dd HH:mm");
	}

	public static int GetWeekNumberOfMonth(DateTime date)
	{
		date = date.Date;
		DateTime firstMonthDay = new DateTime(date.Year, date.Month, 1);
		DateTime firstMonthMonday = firstMonthDay.AddDays((DayOfWeek.Monday + 7 - firstMonthDay.DayOfWeek) % 7);
		if (firstMonthMonday > date)
		{
			firstMonthDay = firstMonthDay.AddMonths(-1);
			firstMonthMonday = firstMonthDay.AddDays((DayOfWeek.Monday + 7 - firstMonthDay.DayOfWeek) % 7);
		}
		return (date - firstMonthMonday).Days / 7 + 1;
	}

	// 음력 날짜 구하기
	public static DateTime GetLunarDateTime(DateTime date)
	{
		KoreanLunisolarCalendar ksc = new KoreanLunisolarCalendar();
		int year = ksc.GetYear(date);
		int month = ksc.GetMonth(date);
		int day = ksc.GetDayOfMonth(date);

		DateTime date2 = ksc.ToDateTime(year, month, day, 0, 0, 0, 0);

		return date2;
	}

	/*
	public static bool IsTimeUp(string expirationDate)
	{
		TimeSpan the = GetTimeSpanFromNowDate(expirationDate);
		if (the.Milliseconds < 0 || the.Seconds < 0 || the.Minutes < 0 || the.Hours < 0 || the.Days < 0)
		{
			return true;
		}

		return false;
	}
	*/
	/*
		public static float GetCurrentAnimationLength(this Animator animator, string animationClipName)
		{
			AnimationClip[] clips = animator.runtimeAnimatorController.animationClips;
			AnimationClip theClip = Array.Find
			(
				clips,
				the =>
				{
					return the.name.Equals(animationClipName);
				}
			);

			if (theClip == null)
			{
				return 0f;
			}

			AnimatorStateInfo theStateInfo = animator.GetCurrentAnimatorStateInfo(0);
			Debug.Log(theStateInfo.speed);

			return theClip.length / animator.speed;
		}*/
#endregion

#region Events
#endregion

#region MD5
	public static string CalculateMD5(string filename)
	{
		using (var md5 = MD5.Create())
		{
			using (var stream = File.OpenRead(filename))
			{
				var hash = md5.ComputeHash(stream);
				return BitConverter.ToString(hash).Replace("-", "").ToLowerInvariant();
			}
		}
	}
	public static string CalculateMD5(byte[] buffer)
	{
		using (var md5 = MD5.Create())
		{
			var hash = md5.ComputeHash(buffer);
			return BitConverter.ToString(hash).Replace("-", "").ToLowerInvariant();
		}
	}

	public static string GetDirInPersistentPath(string searchDir, string persistentPath = "")
	{
		if (string.IsNullOrEmpty(persistentPath))
			persistentPath = Application.persistentDataPath;

		if (Directory.Exists(persistentPath) == false)
			return string.Empty;

		var dirs = Directory.GetDirectories(persistentPath, searchDir, SearchOption.AllDirectories);

		if (dirs.Length > 0)
		{
			return dirs[0];
		}

		return string.Empty;
	}
#endregion

#region Verion Check

	public static bool IsSameServerVersion(string serverVersion)
	{
		var appVersionInfos = Application.version.Split('.');
		if (appVersionInfos.Length < 3)
		{
			Debug.LogError($"Invalid app version info {Application.version}");
			return false;
		}

		var appPhase = appVersionInfos[0];
		var appMajor = int.Parse(appVersionInfos[1]);
		var appMinor = int.Parse(appVersionInfos[2]);
		var authVersionInfos = serverVersion.Split('.');

		if (authVersionInfos.Length < 3)
		{
			Debug.LogError($"Invalid server version info {serverVersion}");
			return false;
		}

		var authPhase = authVersionInfos[0];
		var authMajor = int.Parse(authVersionInfos[1]);
		var authMinor = int.Parse(authVersionInfos[2]);
		if (authPhase == appPhase && authMajor == appMajor && appMinor == authMinor)
		{
			return true;
		}
		else
		{
			Debug.LogError("different app and server version");
			return false;
		}
	}
    #endregion

    #region >> GUI <<
    //public static string GetString(EStringTableType _type, string _index)
    //{
    //    switch(_type)
    //    {
    //        case EStringTableType.String:
    //            return CSVDataManager.GetTable<StringTable>().GetString(_index);
    //        case EStringTableType.UI:
    //            return CSVDataManager.GetTable<StringUiTable>().GetString(_index);
    //        default:
    //            return string.Empty;
    //    }
    //}
#endregion

}
