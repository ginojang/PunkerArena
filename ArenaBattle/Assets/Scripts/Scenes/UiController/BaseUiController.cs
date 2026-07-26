using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public abstract class BaseUiController<T> : MonoBehaviour where T : class
{
	public static T Instance { get; private set; }

	/// <summary>
	/// 현재 씬 로딩 중에 AddGuiInitListener 호출된 수
	/// </summary>
//	private static int initGuiCount = 0;

	private void Awake()
	{
		if (Instance == null)
		{
			Debug.Assert(GetType() == typeof(T));
			Instance = this as T;
		}

		UnitySceneLoader.OnCompletePreloadAssetCallback += OnCompletePreloadAsset;
		OnAwake();
	}

	private void OnDestroy()
	{
		OnDestroyComponent();
		Instance = null;
	}

	protected void AddGuiInitListener<U>(GuiCallback<GuiObject> initCallback) where U : GuiEvents<U>
	{
		//TODO(ych):로딩 프로세스 구현후 삭제 예정
		//initGuiCount++;
		GuiEvents<U>.AddListener(GuiObject.COMMON_EVENT_ID.INIT, (x) =>
		{
			initCallback(x);
			//TODO(ych):로딩 프로세스 구현후 삭제 예정
			/*
			initGuiCount--;
			if (initGuiCount <= 0)
			{
				initGuiCount = 0;
				UnitySceneLoader.UnitySceneAdvancePhase(UnitySceneLoader.PHASE.LOADING_COMPLETING);
			}
            */
		});
	}

	protected virtual void OnDestroyComponent()
	{

	}

	protected virtual void OnAwake() { }

	protected virtual void OnCompleteSceneLoaded() { }

	protected abstract void OnCompletePreloadAsset();


}
