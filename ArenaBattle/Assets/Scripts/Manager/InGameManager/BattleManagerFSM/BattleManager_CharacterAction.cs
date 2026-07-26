using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public partial class BattleManager
{
    private bool _insertBuffComplete = false;

    private void CharacterAction_Enter()
    {
        InGameData.Instance.Trigger_Timing = Trigger_Timing.Hit;

        StartCoroutine(SetAction(TRIGGER_FSM.CharacterAction, SetSkillAction));
    }

    private void CharacterAction_Exit()
    {
        _checkCharacterBehavior = false;

        ResetAction();

        Messenger.Broadcast(Definition.GridOff);
        Messenger.RemoveListener(Definition.CompleteInsertTrigger, CompleteInsertBuff);
    }

    private void SetSkillAction()
    {
        Messenger.AddListener(Definition.CompleteInsertTrigger, CompleteInsertBuff);

        stateAction.Add(SetTriggerData);
        stateAction.Add(TargetOnDamageState);
        stateAction.Add(CharacterSkillAction);
        stateAction.Add(InsertBurstGauge);
        stateAction.Add(StartWaitForInsertBuff);
        stateAction.Add(CharacterCheckBuff);
        stateAction.Add(FSMChangeNextState);
    }
    private void TargetOnDamageState()
    {
        var targetList = InGameData.Instance.TargetList;
        for(int i = 0; i < targetList.Count ; i++)
            targetList[i].StartSubAction<SUB_ACTION_ONDAMAGE>(SUB_ACTION_ONDAMAGE.ActionStart);

        InvokeAction();
    }

    private void CharacterSkillAction()
    {
        _checkCharacterBehavior = true;
        var skillData = InGameData.Instance.CurrentSkillData;

        Messenger.Broadcast(Definition.SkillDescriptionOff);

        switch (skillData.type_action)
        {
            case Type_Action.Melee:
                InGameData.Instance.CurrentTurnCharacter.StartSubAction<SUB_ACTION_MELEE>(SUB_ACTION_MELEE.ActionStart);
                break;
            case Type_Action.CenterMelee:
                InGameData.Instance.CurrentTurnCharacter.StartSubAction<SUB_ACTION_CENTERMELEE>(SUB_ACTION_CENTERMELEE.ActionStart);
                break;
            case Type_Action.AcionMoveMelee:
                InGameData.Instance.CurrentTurnCharacter.StartSubAction<SUB_ACTION_MELEE>(SUB_ACTION_MELEE.ActionStart);
                break;
            case Type_Action.ActionMoveCenterMelee:
                InGameData.Instance.CurrentTurnCharacter.StartSubAction<SUB_ACTION_CENTERMELEE>(SUB_ACTION_CENTERMELEE.ActionStart);
                break;
            case Type_Action.Range:
                InGameData.Instance.CurrentTurnCharacter.StartSubAction<SUB_ACTION_RANGE>(SUB_ACTION_RANGE.ActionStart);
                break;
            case Type_Action.ParabolaRange:
                InGameData.Instance.CurrentTurnCharacter.StartSubAction<SUB_ACTION_PARABOLARANGE>(SUB_ACTION_PARABOLARANGE.ActionStart);
                break;
        }

    }
    private void SetTriggerData()
    {
        int trigger = InGameData.Instance.CurrentSkillData.trigger_id;

        if (trigger > 0)
            InGameData.Instance.CurrentTriggerData = CSVDataManager.GetTable<TriggerTable>().GetData(trigger);

        InvokeAction();
    }

    private void StartWaitForInsertBuff()
    {
        StartCoroutine(WaitForInsertBuff());
    }
    private IEnumerator WaitForInsertBuff()
    {
        yield return new WaitUntil(() => _insertBuffComplete == true);

        InvokeAction();
    }
    private void CompleteInsertBuff()
    {
        _insertBuffComplete = true;
    }


    private void CharacterCheckBuff()
    {
        Messenger.Broadcast(Definition.CharacterCheckTriggerBuff);
    }
    private void InsertBurstGauge()
    {
        if(InGameData.Instance.BurstGauge >= 100)
        {
            InvokeAction();
            return;
        }

        var data = InGameData.Instance.CurrentSkillData;
        float burstGauge = data.burst_point;

        InGameData.Instance.BurstGauge += burstGauge;

        Messenger.Broadcast(Definition.AddBurst);
    }
}
