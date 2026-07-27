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

        // [FIX] 죽어서 비활성된 캐릭터가 턴을 받으면 inactive 오브젝트에 서브액션 시작 → 스톨. null처럼 스킵.
        if (turnChar != null && turnChar.gameObject.activeInHierarchy)
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
