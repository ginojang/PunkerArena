using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Malee;

#region AddressableItem
[System.Serializable]
public class AddressableItem
{
	public string addressableKey;
	public int count;
}

[System.Serializable]
public class AddressableItemList : ReorderableArray<AddressableItem> { }
#endregion

public class UiAddressablePoolManager : MonoBehaviour
{
	#region Singleton
	private bool isInitialized;
	private static UiAddressablePoolManager instance;

	public static UiAddressablePoolManager Instance
	{
		get
		{
			if (instance == null)
			{
				GameObject go = new GameObject("UiAddressablePoolManager");
				instance = go.AddComponent<UiAddressablePoolManager>();
				ImmortalGameObject.AttachObject(go);
			}

			return instance;
		}
	}

	public void Initialize()
	{
		if (isInitialized == true)
		{
			return;
		}

		isInitialized = true;
	}
	#endregion

	#region UiAddressablePool
	public class UiAddressablePool
	{
		private UiAddressablePoolManager parent;

		public int count;
		public GameObject prefab;
		public List<GameObject> list = new List<GameObject>();

		public UiAddressablePool(UiAddressablePoolManager parent, int count)
		{
			this.parent = parent;
			this.count = count;
		}

		public GameObject Expand(int addCount)
		{
			if (addCount < 1)
			{
				Debug.LogWarning($"[UiAddressablePool] You're about to expand by the number which is lower than 1.");
				return null;
			}

			for (int i = 0; i < addCount; i++)
			{
				var theClone = Instantiate<GameObject>(prefab, Vector3.zero, Quaternion.identity);
				theClone.SetActive(false);
				theClone.transform.SetParent(parent.transform);
				list.Add(theClone);
			}

			return list[list.Count - 1];
		}

		public void Destroy()
		{
			for (int i = 0; i < list.Count; i++)
			{
				Object.Destroy(list[i]);
			}

			list.Clear();
			//DestroyUtility.SafeDestroy(ref list, true);
		}

		public GameObject Spawn()
		{
			foreach (var the in list)
			{
				/// GameObject.activeSelf returns the local active state of this GameObject, which is set by using GameObject.SetActive.
				/// Note that a GameObject may be inactive because a parent is not active, even if this returns true.
				/// Using GameObject.activeInHierarchy is more accurate if you want to check whether the GameObject is actually treated as active in the scene.
				if (the.activeInHierarchy == false && the.transform.parent == instance.transform)
				{
					return the;
				}
				/*
				// However what if we need to spawn multiple game objects in one frame?
				if (the.activeSelf == false && the.transform.parent == instance.transform)
				{ 
					return the;
				}
				*/
			}

			return Expand(1);
		}
	}
	#endregion

	#region Fields	
	private UnityAction OnCompleteCallback;
	private List<AssetLoader> assetLoaderList = new List<AssetLoader>();
	private Dictionary<string, UiAddressablePool> pooledObjectList = new Dictionary<string, UiAddressablePool>();
	#endregion

	#region Private methods
	private void OnDestroy()
	{
		Destroy();
	}

	private void AddAssetLoader(string addressableKey, List<AssetLoader> theList)
	{
		var loader = AssetManager.Instance.PreLoadAsset(addressableKey, null);
		if (loader != null)
		{
			if (theList.Contains(loader) == false)
			{
				theList.Add(loader);
			}
			else
			{
				Debug.LogWarning($"[UiAddressablePoolManager] The loader of addressableKey ({addressableKey}) already exists in assetLoaderList.");
			}
		}
	}

	private void OnCompleteAssetLoaderList(object p)
	{
		foreach (var theAssetLoader in assetLoaderList)
		{
			UiAddressablePool thePool = pooledObjectList[theAssetLoader.AssetFullPath];
			thePool.prefab = theAssetLoader.MainAsset as GameObject;
			thePool.Expand(thePool.count);
		}

		// Move on to the next phase.
		if (OnCompleteCallback != null)
		{
			OnCompleteCallback();
		}
		else
		{
			Debug.LogWarning("[UiAddressablePoolManager] No suitable OnCompleteCallback.");
		}
	}	
	#endregion

	/// <summary>
	/// Each UiAddressablePoolController will add it's own list into this,
	/// so that we can collect all of addressable assets which is going to be used in the current scene.
	/// </summary>
	/// <param name="list"></param>
	public void AddList(AddressableItemList list)
	{
		// Need to load addressable resources by using async-loading first, then we can pool.
		for (int i = 0; i < list.Count; i++)
		{
			var the = list[i];
			AddAssetLoader(the.addressableKey, assetLoaderList);

			// Also add it to our pooledObjectList to instantiate clones later.
			if (pooledObjectList.ContainsKey(the.addressableKey) == false)
			{
				pooledObjectList[the.addressableKey] = new UiAddressablePool(this, the.count);
			}
			else
			{
				Debug.LogWarning($"[UiAddressablePoolManager] The addressableKey ({the.addressableKey}) already exists in pooledObjectList.");
			}
		}
	}

	/// <summary>
	/// Start to load assetLoaderList if it contains some data.
	/// </summary>
	/// <returns></returns>
	public UiAddressablePoolManager Create(UnityAction OnComplete)
	{
		OnCompleteCallback += OnComplete;

		if (assetLoaderList.Count == 0)
		{
			OnCompleteCallback();
		}
		else
		{
			AssetManager.Instance.LoadAssetAsyncForPreloadAsset(assetLoaderList, OnCompleteAssetLoaderList);
		}		

		return this;
	}

	/// <summary>
	/// Release all instantiated objects used by current scene.
	/// </summary>
	public void Destroy()
	{
		// Initialize callback.
		OnCompleteCallback = null;

		// Clear pooled (instantiated) clones all, the manager is responsible for clearing, not AssetManager.
		foreach (var theObj in pooledObjectList)
		{
			theObj.Value.Destroy();
		}
		pooledObjectList.Clear();

		// Clear assetLoader list.
		assetLoaderList.Clear();		
	}

	/// <summary>
	/// Spawn
	/// </summary>
	/// <param name="addressableKey"></param>
	/// <returns></returns>
	public GameObject Spawn(string addressableKey)
	{
		if (pooledObjectList.ContainsKey(addressableKey) == false)
		{
			Debug.LogWarning($"[UiAddressablePoolManager] No pooled ones for the addressableKey ({addressableKey}).");
			return null;
		}

		GameObject theSpawned = pooledObjectList[addressableKey].Spawn();

		// messaging won't work if the game object was disabled
		theSpawned.SetActive(true);

		// Like Unity's OnEnable()
		theSpawned.BroadcastMessage("OnPoolSpawned", SendMessageOptions.DontRequireReceiver);
		//theSpawned.SendMessage("OnPoolSpawned", SendMessageOptions.DontRequireReceiver);

		return theSpawned;
	}

	/// <summary>
	/// Spawn with parent
	/// </summary>
	/// <param name="addressableKey"></param>
	/// <returns></returns>
	public GameObject Spawn(string addressableKey, Transform parent, bool worldPositionStays = false)
	{
		GameObject theSpawned = Spawn(addressableKey);
		theSpawned.transform.SetParent(parent, worldPositionStays);
		return theSpawned;
	}

	/// <summary>
	/// Spawn with transform & parent
	/// </summary>
	/// <param name="addressableKey"></param>
	/// <returns></returns>
	public GameObject Spawn(string addressableKey, Vector3 position, Quaternion rotation, Transform parent, bool worldPositionStays = false)
	{
		GameObject theSpawned = Spawn(addressableKey);
		theSpawned.transform.SetParent(parent, worldPositionStays);
		theSpawned.transform.position = position;
		theSpawned.transform.rotation = rotation;		
		return theSpawned;
	}

	/// <summary>
	/// Despawn
	/// </summary>
	/// <param name="theObject"></param>
	public void Despawn(GameObject theObject, bool forceFlag = false)
	{
		if (theObject == null)
		{
			return;
		}

		// The object can be called many times in the same frame, so just return here once the obj has despawnd and disabled.
		// For example, The object can have multiple collisions from other objects that try to despawn the object such as missiles, effects and so on.
		//if (theObject.activeInHierarchy == false)
		if (theObject.activeSelf == false)
		{
			if (forceFlag == false)
			{
				return;
			}			
		}

		// In case you took the object.
		if (transform != theObject.transform.parent)
		{
			theObject.transform.SetParent(transform);
		}

		// Call OnGoPoolDespawned() like Unity's OnDisable()
		theObject.BroadcastMessage("OnPoolDespawned", SendMessageOptions.DontRequireReceiver);
		//theObject.SendMessage("OnPoolDespawned", SendMessageOptions.DontRequireReceiver);

		theObject.SetActive(false);
	}
}