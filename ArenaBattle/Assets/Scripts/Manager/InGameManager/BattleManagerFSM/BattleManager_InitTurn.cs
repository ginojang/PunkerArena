using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public partial class BattleManager
{
    private void InitTurn_Enter()
    {
        StartCoroutine(SetAction(TRIGGER_FSM.InitTurn, SetInitTurnAction));
    }
    private void InitTurn_Update()
    {

    }
    private void InitTurn_Exit()
    {
        ResetAction();
    }
    
    private void SetInitTurnAction()
    {
        stateAction.Add(SetStartTurn);
        stateAction.Add(SetCharacterState);
        stateAction.Add(SetTurnComplete);
    }
    private void SetStartTurn()
    {
        Messenger.Broadcast(Definition.StartMakeCurrentTurn);
    }
    private void SetTurnComplete()
    {
        CharacterBase turnChar = InGameData.Instance.CurrentTurnCharacter;

        if (turnChar != null)
        {
            if (InGameData.Instance.GetCharacterActionLock(turnChar) == true)
                Messenger.Broadcast(Definition.PassClick);
            else
                FSMChangeNextState();
        }
        else
            GoToState(TRIGGER_FSM.SetRoundUI);
    }
    private void SetCharacterState()
    {
        Messenger.Broadcast(Definition.SetCharacterState);
    }
}
