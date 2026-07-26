using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UiBase<T> : GuiEvents<T> where T : class
{
	public static T Instance { get; private set; }

	protected virtual void Awake()
	{
		if (Instance == null)
		{
			Debug.Assert(GetType() == typeof(T));
			Instance = this as T;
		}
		else
		{
			Destroy(gameObject);
		}
	}

	protected virtual void Start()
	{
		Broadcast(COMMON_EVENT_ID.INIT);
	}

	protected override void OnDestroy()
	{
		base.OnDestroy();

		Instance = null;
	}
}