using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Generated.CsvData;
using System;

public class TriggerManager : MonoBehaviour
{
    public enum State
    {
        None,
        InsertTrigger,
        AttackCondition,

    }
    private State currentState = State.None;

    private int actionIndex = 0;

    private bool insertBuff = false;
    private List<Action> stateAction = new List<Action>();

    private bool insertComplete = false;
    private TriggerTable table = null;

    private void Awake()
    {
        MessageAddListner();
        InitData();
    }

    private void InitData()
    {
        table = CSVDataManager.GetTable<TriggerTable>();
    }

    #region CheckTrigger
    private void StartCheckTrigger()
    {
        StartCoroutine(SetInsertTrigger());
    }
    private IEnumerator SetInsertTrigger()
    {
        yield return new WaitUntil(() => currentState == State.None);

        stateAction.Clear();
        actionIndex = 0;

        stateAction.Add(StartInsertTrigger);
        stateAction.Add(InsertTriggerComplete);

        InvokeAction();
        yield break;
    }
    private void StartInsertTrigger()
    {
        StartCoroutine(InsertTrigger());
    }
    private IEnumerator InsertTrigger()
    {
        int triggerId = InGameData.Instance.CurrentSkillData.trigger_id;
        if (triggerId <= 0)
        {
            InvokeAction();
            yield break;
        }

        TriggerData data = table.GetData(triggerId);

        List<TriggerData.TriggerInfo> infoList = data.triggerInfoList;
        for(int i = 0; i < infoList.Count; i++)
        {
            insertComplete = false;
            var infoData = infoList[i];

            SetTarget(infoData.action_type_target);

            switch(infoData.action_type)
            {
                case Action_Type.None:
                    break;
                case Action_Type.BuffDebuff:
                    Messenger.Broadcast(Definition.InsertBuff, infoData);
                    break;
                case Action_Type.CC:
                    Messenger.Broadcast(Definition.InsertCC, infoData);
                    break;
                case Action_Type.Stack:
                    break;
            }

            yield return new WaitUntil(() => insertComplete == true);
        }

        InvokeAction();
    }
    private void SetTarget(Type_Target type_target)
    {
        List<CharacterBase> targetList = InGameData.Instance.TriggerTarget;
        if (targetList.Count > 0)
            targetList.Clear();

        Camp camp = InGameData.Instance.CurrentTurnCharacter.CharacterInfo.BattleCamp;
        switch (camp)
        {
            case Camp.Ally:
                switch (type_target)
                {
                    case Type_Target.Ally:
                        targetList.AddRange(InGameData.Instance.AllyList.Keys);
                        break;
                    case Type_Target.Enemey:
                        targetList.AddRange(InGameData.Instance.EnemyList.Keys);
                        break;
                    case Type_Target.Self:
                        targetList.Add(InGameData.Instance.CurrentTurnCharacter);
                        break;
                    case Type_Target.Target:
                        targetList.Add(InGameData.Instance.CurrentTargetCharacter);
                        break;
                }
                break;
            case Camp.Enemy:
                switch (type_target)
                {
                    case Type_Target.Ally:
                        targetList.AddRange(InGameData.Instance.EnemyList.Keys);
                        break;
                    case Type_Target.Enemey:
                        targetList.AddRange(InGameData.Instance.AllyList.Keys);
                        break;
                    case Type_Target.Self:
                        targetList.Add(InGameData.Instance.CurrentTurnCharacter);
                        break;
                    case Type_Target.Target:
                        targetList.Add(InGameData.Instance.CurrentTargetCharacter);
                        break;
                }
                break;
        }
    }

    private void InsertComplete()
    {
        insertComplete = true;
    }

    private void InsertTriggerComplete()
    {
        Messenger.Broadcast(Definition.CompleteInsertTrigger);
    }
    #endregion



    #region Action
    private void InvokeAction()
    {
        if(stateAction.Count <= actionIndex)
        {
            currentState = State.None;
            return;
        }

        var action = stateAction[actionIndex];
        actionIndex++;

        action.Invoke();
    }
    #endregion

    private void MessageAddListner()
    {
        Messenger.AddListener(Definition.StartCheckTrigger, StartCheckTrigger);
        Messenger.AddListener(Definition.InsertComplete, InsertComplete);
    }
    private void MessageRemoveListner()
    {
        Messenger.RemoveListener(Definition.StartCheckTrigger, StartCheckTrigger);
        Messenger.RemoveListener(Definition.InsertComplete, InsertComplete);
    }

    private void OnDestroy()
    {
        MessageRemoveListner();
    }
}
