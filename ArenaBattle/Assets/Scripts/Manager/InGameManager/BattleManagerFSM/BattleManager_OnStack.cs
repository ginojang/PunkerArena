using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public partial class BattleManager
{
    private void OnStack_Enter()
    {
        StartCoroutine(SetAction(TRIGGER_FSM.OnStack, SetOnStackAction));
    }
    private void OnStack_Update()
    {

    }
    private void OnStack_Exit()
    {
        ResetAction();
    }
    private void SetOnStackAction()
    {
        stateAction.Add(FSMChangeNextState);
    }
}
