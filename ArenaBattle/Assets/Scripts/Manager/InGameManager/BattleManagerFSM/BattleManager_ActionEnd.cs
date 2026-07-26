using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public partial class BattleManager
{
    private void ActionEnd_Enter()
    {
        StartCoroutine(SetAction(TRIGGER_FSM.ActionEnd, SetActionEndAction));
    }
    private void ActionEnd_Update()
    {

    }
    private void ActionEnd_Exit()
    {
        ResetAction();
    }
    private void SetActionEndAction()
    {
        stateAction.Add(FSMChangeNextState);
    }
}
