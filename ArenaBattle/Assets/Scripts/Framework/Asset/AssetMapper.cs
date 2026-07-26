using System.Collections;
using UnityEngine;

public class AssetMapper : MonoBehaviour
{
	#region Singleton
	private static AssetMapper instance = null;

	public static AssetMapper Instance
	{
		get
		{
			if (instance == null)
			{
				GameObject go = new GameObject("AssetMapper");
				instance = go.AddComponent<AssetMapper>();
				ImmortalGameObject.AttachObject(go);
			}

			return instance;
		}
	}
	#endregion

	private AddressableAssetPathDataBase assetPathDatabase;
	public AddressableAssetPathDataBase AssetPathDatabase { get { return assetPathDatabase; } }

	public IEnumerator Initialize()
	{
		var asyncOp = AssetManager.Instance.LoadAssetAsync<TextAsset>("Assets/data/AddressableAssetPaths.json");
		yield return asyncOp; 

/*		var addressablePathsJson = asyncOp.Current as TextAsset;
		if (addressablePathsJson == null)
		{
			if (Application.isEditor && AssetManager.BundleType == AssetManager.ASSETBUNDLE_TYPE.EDITOR_LOAD)
			{
				Debug.Log("에디터 모드 세팅으로 어셋 번들 코드가 실행 중입니다.");
			}
			else
			{
				Debug.LogError("AssetBundlePaths.json 파일이 생성되지 않았습니다.");
			}
		}

		if (addressablePathsJson != null)
		{
			assetPathDatabase = JsonUtility.FromJson<AddressableAssetPathDataBase>(addressablePathsJson.text);
		}*/

	}

	public void InitializeWithSync(System.Action callback = null)
	{
		AssetManager.Instance.LoadAssetAsync<TextAsset>("Assets/data/AddressableAssetPaths.json", (ld, param) =>
		{
			var addressablePathsJson = ld.MainAsset as TextAsset;
			if (addressablePathsJson == null)
			{
/*				if (Application.isEditor && AssetManager.BundleType == AssetManager.ASSETBUNDLE_TYPE.EDITOR_LOAD)
				{
					Debug.Log("에디터 모드 세팅으로 어셋 번들 코드가 실행 중입니다.");
				}
				else
				{
					Debug.LogError("AssetBundlePaths.json 파일이 생성되지 않았습니다.");
				}*/
			}

			if (addressablePathsJson != null)
			{
				assetPathDatabase = JsonUtility.FromJson<AddressableAssetPathDataBase>(addressablePathsJson.text);
			}

			callback?.Invoke();
		}, null);
		
	}
#if UNITY_EDITOR
	public void InitializeWithSyncEditor()
	{
		var json = UnityEditor.AssetDatabase.LoadAssetAtPath<TextAsset>("Assets/data/AddressableAssetPaths.json");
		assetPathDatabase = JsonUtility.FromJson<AddressableAssetPathDataBase>(json.text);
	}
#endif

	public bool GetAssetPaths(int assetId, out string assetFullPath)
	{
		assetFullPath = "";

		if (assetPathDatabase == null)
		{
			return false;
		}

		bool findAsset = false;
		for (int i = 0; i < assetPathDatabase.assetPaths.Length; i++)
		{
			if (assetPathDatabase.assetPaths[i].assetID == assetId)
			{
				assetFullPath = assetPathDatabase.assetPaths[i].addressablePath;
				findAsset = true;
				break;
			}
		}

		if (findAsset == false)
		{
			Debug.LogError($"not found asset in bundle {assetId}");
		}

		return findAsset;
	}

	public bool GetAssetPaths(string assetFullPath)
	{
		bool findAsset = false;
		for (int i = 0; i < assetPathDatabase.assetPaths.Length; i++)
		{
			if (assetPathDatabase.assetPaths[i].addressablePath == assetFullPath)
			{
				findAsset = true;
				break;
			}
			else
			{
				string fileName = System.IO.Path.GetFileNameWithoutExtension(assetPathDatabase.assetPaths[i].addressablePath);
				if (fileName.ToLower() == assetFullPath.ToLower())
				{
					findAsset = true;
					break;
				}
			}
		}

		if (findAsset == false)
		{
			Debug.LogError($"not found asset in bundle {assetFullPath}");
		}

		return findAsset;
	}
}
