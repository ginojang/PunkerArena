using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public partial class BattleManager
{
    private void Intro_Enter()
    {
        StartCoroutine(SetAction(TRIGGER_FSM.Intro, SetIntroAction));
    }
    private void Intro_Exit()
    {
        InGameData.Instance.CurrentTurn++;

        ResetAction();
        Messenger.Broadcast(Definition.CompleteState, Complete.BattleManager);
    }
    private void SetIntroAction()
    {
        stateAction.Add(IntroCharacter);
        stateAction.Add(() => Messenger.Broadcast(Definition.StartSetRoundProfile));
        stateAction.Add(() => Messenger.Broadcast(Definition.SetRoundUI));
        stateAction.Add(InitializeFSM);
    }

    private void IntroCharacter()
    {
        StartCoroutine(CharacterIntroAction());
    }
    private IEnumerator CharacterIntroAction()
    {
        List<CharacterBase> list = new List<CharacterBase>();

        list.AddRange(InGameData.Instance.AllyList.Keys);
        list.AddRange(InGameData.Instance.EnemyList.Keys);
        for (int i = 0; i < list.Count; i++)
        {
            yield return new WaitForSeconds(0.2f);
            list[i].StartSubAction<SUB_ACTION_INTRO>(SUB_ACTION_INTRO.ActionStart);
        }
        yield return new WaitUntil(() => _characterBehavior == Character_Behavior.None);
        InvokeAction();
    }
}
