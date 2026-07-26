using UnityEngine;

public static class ImmortalGameObject
{
	private static GameObject immortalObject;

	public static GameObject RootObject
	{
		get
		{
			if (immortalObject == null)
			{
				immortalObject = GameObject.Find("__immortal__");
				if (immortalObject == null)
					immortalObject = new GameObject("__immortal__") { isStatic = true };

				if (Application.isPlaying)
					Object.DontDestroyOnLoad(immortalObject);
			}

			return immortalObject;
		}
	}

	public static void Destroy()
	{
		if (immortalObject)
		{
			Object.Destroy(immortalObject);
			immortalObject = null;
		}
	}

	public static void AttachObject(GameObject gameObject)
	{
		gameObject.transform.parent = RootObject.transform;
	}

	public static void DetachObject(GameObject gameObject)
	{
		gameObject.transform.parent = null;
	}

	public static void AddObject<T>() where T : Component
	{
		RootObject.MakeComponent<T>();
	}

	public static T GetObject<T>() where T : Component
	{
		T obj = RootObject.GetComponent<T>();
		if (obj == null)
		{
			obj = RootObject.GetComponentInChildren<T>();
		}
		return obj;
	}

	public static void DeleteObject<T>() where T : Component
	{
		T obj = GetObject<T>();
		if (obj != null)
		{
			Object.Destroy(obj);
		}
	}
}
