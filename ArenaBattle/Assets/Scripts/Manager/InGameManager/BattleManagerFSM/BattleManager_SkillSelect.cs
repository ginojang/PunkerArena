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

        CharacterState state = null;

       switch(camp)
        {
            case Camp.Ally:
                InGameData.Instance.AllyList.TryGetValue(character, out state);
                if (state.Auto_AI != Type_Target_Auto_AI.None)
                    Messenger.Broadcast(Definition.SetMakeAIData);
                break;
            case Camp.Enemy:
                Messenger.Broadcast(Definition.SetMakeAIData);
                break;
        }
    }
}
