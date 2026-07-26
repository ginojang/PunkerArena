using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public partial class BattleManager
{
   
    private void TurnReady_Enter()
    {
        StartCoroutine(SetAction(TRIGGER_FSM.TurnReady, SetTurnReadyAction));
    }
    private void TurnReady_Update()
    {

    }
    private void TurnReady_Exit()
    {
        ResetAction();
    }

    private void SetTurnReadyAction()
    {
        stateAction.Add(FSMChangeNextState);
    }


}
