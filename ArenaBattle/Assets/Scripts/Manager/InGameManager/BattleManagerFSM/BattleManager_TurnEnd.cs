using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public partial class BattleManager
{
    private void TurnEnd_Enter()
    {
        StartCoroutine(SetAction(TRIGGER_FSM.TurnEnd, SetTurnEndAction));
    }
    private void TurnEnd_Update()
    {

    }
    private void TurnEnd_Exit()
    {
        ResetAction();
    }
    private void SetTurnEndAction()
    {
        stateAction.Add(CheckRemoveBuff);
        stateAction.Add(CheckRemoveCC);
        stateAction.Add(GridOff);
        stateAction.Add(ClearCurrentTurnData);
        stateAction.Add(()=> GoToState(TRIGGER_FSM.TurnReady));
    }
    private void GridOff()
    {
        Messenger.Broadcast(Definition.GridOff);
        InvokeAction();
    }
    private void ClearCurrentTurnData()
    {
        Messenger.Broadcast(Definition.ExitTurn);
    }
    private void CheckRemoveBuff()
    {
        Messenger.Broadcast(Definition.CheckRemoveBuff);
    }
    private void CheckRemoveCC()
    {
        Messenger.Broadcast(Definition.CheckRemoveCC);
    }
}
