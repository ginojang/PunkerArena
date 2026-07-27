using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MonsterLove.StateMachine;
using UnityEngine.SceneManagement;

public class PlayState : MonoBehaviour
{
	#region Singleton
	private static PlayState instance = null;

	public static PlayState Instance
	{
		get
		{
			if (instance == null)
			{
				GameObject go = new GameObject("PlayState");
				instance = go.AddComponent<PlayState>();
				ImmortalGameObject.AttachObject(go);
			}

			return instance;
		}
	}

	#endregion

	public enum STATES
	{
		None,
		Patch,
		Title,
		Menu,
		StageMenu,
		Jungle,
		Desert,
		Ice,
		Avatar,
		Beach,
	}

	public STATES CurrentState { get { return fsm.State; } }

	private StateMachine<STATES> fsm;

	private void Awake()
	{
		// Initialize State Machine Engine
		fsm = StateMachine<STATES>.Initialize(this, STATES.None);
		fsm.Changed += OnStateChanged;
	}

	public void ChangePlayState(STATES state)
	{
		if (fsm == null)
			return;
		fsm.ChangeState(state);
	}

	// [DATA-DRIVEN] 모든 상태는 "enum 이름 == 씬 이름"으로 로드된다.
	// 예전엔 상태마다 X_Enter/Update/Exit 리전(전부 LoadAsync("X")만)이 있었으나
	// MonsterLove의 Changed 이벤트 하나로 대체(약 180줄 → 이 메서드 하나).
	// 새 스테이지 추가 시 enum에 값만 추가하면 됨(씬 이름을 enum과 동일하게).
	private void OnStateChanged(STATES state)
	{
		if (state == STATES.None)
			return;

		UnitySceneLoader.LoadAsync(state.ToString());
	}
}
