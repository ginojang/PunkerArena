using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Devil.Gui;
using UnityEngine.SceneManagement;

public class PatchScene : UnityScene
{
	protected override void Awake()
	{
		base.Awake();


	}

	protected override void Start()
	{
		base.Start();

	}

	protected override void OnDestroy()
	{
		base.OnDestroy();
	}

	protected override void OnClose()
	{
		base.OnClose();
	}

	protected override void OnPostLoad()
	{
		Debug.Log("Patch scene load completed");
		SceneManager.LoadSceneAsync("SceneDownloading", LoadSceneMode.Additive);
		/*
		#if true //mcshin test
				if (GameDataMgr.Instance.Player.CharacterCount <= 0)
		#else
				if (true)
		#endif
				{
					PlayState.Instance.ChangePlayState(PlayState.STATES.CreateCharacter);
				}
				else
				{
		#if DEVELOPMENT
					System.DateTime regTime = System.Convert.ToDateTime(GameDataMgr.Instance.Player.RegisterTime);
					System.DateTime cTime = System.Convert.ToDateTime("2021-04-14");
					if (regTime.CompareTo(cTime) < 0)
					{
						if (GameDataMgr.Instance.bWelcomeEventTarget)
						{
							PlayState.Instance.ChangePlayState(PlayState.STATES.WelcomeEvent);
						}
						else
						{
							PlayState.Instance.ChangePlayState(PlayState.STATES.MyHome);
						}
					}
					else
					{
						if (TutorialPlayerData.IsComplete(Tutorial.EIndex.SR) == false)
						{
							// [TODO] : 캐릭터 생성씬으로 이동시키고 튜토리얼을 진행시킨다.
							PlayState.Instance.ChangePlayState(PlayState.STATES.CreateCharacter);
						}
						else
						{
							if (GameDataMgr.Instance.bWelcomeEventTarget)
							{
								PlayState.Instance.ChangePlayState(PlayState.STATES.WelcomeEvent);
							}
							else
							{
								PlayState.Instance.ChangePlayState(PlayState.STATES.MyHome);
							}
						}
					}
		#else
					if (TutorialPlayerData.IsComplete(Tutorial.EIndex.SR) == false)
					{
						// [TODO] : 캐릭터 생성씬으로 이동시키고 튜토리얼을 진행시킨다.
						PlayState.Instance.ChangePlayState(PlayState.STATES.CreateCharacter);
					}
					else
					{
						if (GameDataMgr.Instance.bWelcomeEventTarget)
						{
							PlayState.Instance.ChangePlayState(PlayState.STATES.WelcomeEvent);
						}
						else
						{
							PlayState.Instance.ChangePlayState(PlayState.STATES.MyHome);
						}
					}
		#endif
				}*/
	}
}