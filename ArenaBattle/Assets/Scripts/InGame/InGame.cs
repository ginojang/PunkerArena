using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.EventSystems;
    [Flags]public enum Complete
    {
        None = 0,
        InGame = 1 << 0,
        UIGame = 1 << 1,
        BattleManager = 1 << 2,
        TurnManager =1 << 3,
        Max = 1 << 4,
    }


public partial class InGame : GameBase
{

    private Complete states;

    protected override void Start()
    {
        base.Start();
    }
    protected override void Update()
    {
        base.Update();
    }
    protected override void OnDestroy()
    {
        base.OnDestroy();
        MessengerRemoveListner();
        //ResourcePoolManager.Instance.DestroyGameDatapool();
        GameDataManager.Instance.GameSceneMessengerRemoveListner();
    }

    #region Initialize
    private void InitializeCompleteState()
    {
        states = Complete.InGame | Complete.UIGame | Complete.BattleManager | Complete.TurnManager;
    }
    private void CompleteState(Complete state)
    {
        states &= ~state;
        if(states == Complete.None)
        {
            InitializeCompleteState();
            ChangeNextState();
        }
    }
    #endregion

    #region InGameState
    private void InGameLoad()
    {
        Messenger.Broadcast(Definition.InGameLoad);
    }
    private void InGameReady()
    {
        Messenger.Broadcast(Definition.InGameReady);
    }
    private void InGameGame()
    {
        Messenger.Broadcast(Definition.InGameGame);
    }
    private void InGameResult()
    {
        Messenger.Broadcast(Definition.InGameResult);
    }
    public override void ChangeNextState()
    {
        base.ChangeNextState();
    }
    #endregion
    #region Messenger
    private void MessengerAddListner()
    {
        Messenger.AddListener<Complete>(Definition.CompleteState, CompleteState);
    }
    private void MessengerRemoveListner()
    {
        Messenger.RemoveListener<Complete>(Definition.CompleteState, CompleteState);
    }
    #endregion
}
