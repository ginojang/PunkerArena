using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public partial class BattleManager
{
    private void InitData_Enter()
    {
        StartCoroutine(SetAction(TRIGGER_FSM.InitData, SetInitDataAction));
    }
    private void InitData_Update()
    {

    }
    private void InitData_Exit()
    {
        ResetAction();
    }
    private void SetInitDataAction()
    {
        stateAction.Add(ClearInGameData);
        stateAction.Add(FSMChangeNextState);
    }
    private void ClearInGameData()
    {
        InGameData.Instance.ResetData();
        InvokeAction();
    }
}
