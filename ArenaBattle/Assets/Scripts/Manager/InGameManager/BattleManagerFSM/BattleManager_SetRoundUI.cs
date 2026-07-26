using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public partial class BattleManager
{
    private void SetRoundUI_Enter()
    {
        StartCoroutine(SetAction(TRIGGER_FSM.SetRoundUI, SetRoundUIAction));
    }
    private void SetRoundUI_Update()
    {

    }
    private void SetRoundUI_Exit()
    {
        ResetAction();
    }
    private void SetInsertRoundData()
    {
        Messenger.Broadcast(Definition.StartInsertNextTurn);
    }
    private void SetRoundProfile()
    {
        Messenger.Broadcast(Definition.StartSetRoundProfile);
    }
    private void SetRoundUI()
    {
        Messenger.Broadcast(Definition.SetRoundUI);
    }
    private void SetRoundUIAction()
    {
        stateAction.Add(SetInsertRoundData);
        stateAction.Add(SetRoundProfile);
        stateAction.Add(SetRoundUI);
        stateAction.Add(FSMChangeNextState);
    }
}
