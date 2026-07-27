using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public partial class BattleManager
{
    private void SkillSelect_Enter()
    {
        StartCoroutine(SetAction(TRIGGER_FSM.SkillSelect, SetSkillSelectAction));
    }
    private void SkillSelect_Update()
    {

    }
    private void SkillSelect_Exit()
    {
        ResetAction();
    }

    private void SetSkillSelectAction()
    {
        stateAction.Add(() => Messenger.Broadcast(Definition.SetCurrentTurnUI));
        stateAction.Add(SetNextState);
        stateAction.Add(FSMChangeNextState);
    }
    private void SetNextState()
    {
        CharacterBase character = InGameData.Instance.CurrentTurnCharacter;
        Camp camp = character.CharacterInfo.BattleCamp;

        Debug.Log($"[TURN] {character.name} ({camp})"); // [CLI] 전투 로그

        // [AUTO/CLI] UI 무력화로 플레이어 입력 불가 → 아군/적 모두 AI가 선택(오토배틀)
        Messenger.Broadcast(Definition.SetMakeAIData);
    }
}
