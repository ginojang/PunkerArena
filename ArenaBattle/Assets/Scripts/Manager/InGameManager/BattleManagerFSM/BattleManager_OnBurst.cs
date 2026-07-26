using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public partial class BattleManager
{
    private void OnBurst_Enter()
    {
        StartCoroutine(SetAction(TRIGGER_FSM.OnBurst, SetOnBurstAction));
    }
    private void OnBurst_Update()
    {

    }
    private void OnBurst_Exit()
    {
        ResetAction();
    }
    private void SetOnBurstAction()
    {
        stateAction.Add(FSMChangeNextState);
    }
}
