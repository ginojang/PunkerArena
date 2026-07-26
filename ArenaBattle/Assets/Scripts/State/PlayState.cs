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
		//Game,
		Jungle,
		Desert,
		Ice,
		Avatar,
		Beach,
		//CreateCharacter,
		//WaitingRoom,
		//MyHome,
		//WorldMap,
		//Battle,
		//Mission,
		//TimeGuardian,
		//Workbook,
		//ReadersBook,
		//InteractionMovieWorld,
		//SceneSoripen,
	}

	public STATES CurrentState { get { return fsm.State; } }

	private StateMachine<STATES> fsm;
//	private Player player;

	private void Awake()
	{
		//Initialize State Machine Engine
		fsm = StateMachine<STATES>.Initialize(this, STATES.None);
	}

	private void Start()
	{
	}

	public void ChangePlayState(STATES state)
	{
		if (fsm == null)
			return;
		fsm.ChangeState(state);
	}

	#region InteractionMovieWorld
	private void InteractionMovieWorld_Enter()
	{
		UnitySceneLoader.LoadAsync("InteractionMovieWorld");
	}

	private void InteractionMovieWorld_Update()
	{

	}

	private void InteractionMovieWorld_Exit()
	{

	}
	#endregion

	#region Patch
	private void Patch_Enter()
	{
		UnitySceneLoader.LoadAsync("Patch");
	}

	private void Patch_Update()
	{

	}

	private void Patch_Exit()
	{

	}
	#endregion

	#region Waiting Room
	private void WaitingRoom_Enter()
	{
		UnitySceneLoader.LoadAsync("WaitingRoom");
	}

	private void WaitingRoom_Update()
	{
	}

	private void WaitingRoom_Exit()
	{
	}
	#endregion Waiting Room

	#region Login
	private void Login_Enter()
	{
		UnitySceneLoader.LoadAsync("LogIn");
	}

	private void Login_Update()
	{
	}

	private void Login_Exit()
	{
	}
    #endregion Login
    #region Title
	private void Title_Enter()
    {
		UnitySceneLoader.LoadAsync("Title");
	}
	private void Title_Update()
    {

    }
	private void Title_Exit()
    {

    }
    #endregion
    #region Menu
    private void Menu_Enter()
	{
		UnitySceneLoader.LoadAsync("Menu");
	}

	private void Menu_Update()
	{

	}

	private void Menu_Exit()
	{

	}
	#endregion

    #region StageMenu
	private void StageMenu_Enter()
    {
		UnitySceneLoader.LoadAsync("StageMenu");
    }
	private void StageMenu_Update()
    {

    }
	private void StageMenu_Exit()
    {

    }
    #endregion

    #region Jungle
	private void Jungle_Enter()
    {
		UnitySceneLoader.LoadAsync("Jungle");
    }		
	private void Jungle_Update()
    {

    }
	private void Jungle_Exit()
    {

    }
    #endregion
    #region Desert
	private void Desert_Enter()
    {
		UnitySceneLoader.LoadAsync("Desert");
    }
	private void Desert_Update()
    {

    }
	private void Desert_Exit()
    {

    }
    #endregion
    #region Ice
	private void Ice_Enter()
    {
		UnitySceneLoader.LoadAsync("Ice");
	}
	private void Ice_Update()
    {

    }
	private void Ice_Exit()
    {

    }
	#endregion
	#region Avatar
	private void Avatar_Enter()
    {
		UnitySceneLoader.LoadAsync("Avatar");
	}
	private void Avatar_Update()
    {

    }
	private void Avatar_Exit()
    {

    }
    #endregion
    #region Beach
	private void Beach_Enter()
    {
		UnitySceneLoader.LoadAsync("Beach");
	}
	private void Beach_Update()
    {

    }
	private void Beach_Exit()
    {

    }
	#endregion
}
