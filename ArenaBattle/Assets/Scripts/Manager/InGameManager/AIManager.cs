using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using System;

public class AIManager : MonoBehaviour
{
    private enum State
    {
        None,
        Make,
    }
    private State currentState = State.None;
    private List<Action> actionList;
    private int actionIndex;

    private void Awake()
    {
        MessageAddListner();
        InitData();
    }
    private void OnDestroy()
    {
        MessageRemoveListner();
        }

    private void InitData()
    {
        actionList = new List<Action>();
        actionIndex = 0;
    }

    #region Make AI Data
    private void SetMakeAIData()
    {
        StartCoroutine(SetAction(State.Make, SetSkillData));
    }
    private void SetSkillData()
    {
        actionList.Add(MakeSkill);
        actionList.Add(SetCasterNextTurn);
        actionList.Add(MakeTarget);
        actionList.Add(MakeComplete);
    }
    private void MakeTarget()
    {
        int randomIndex = -1;
        var manager = InGameData.Instance;
        CharacterBase caster = manager.CurrentTurnCharacter;
        var auto = manager.GetCharacterAuto_AI(caster);
        Camp camp = caster.CharacterInfo.BattleCamp;
        List<CharacterBase> randomTarget = new List<CharacterBase>();

        if(auto == Type_Target_Auto_AI.Confusion)
        {
            randomTarget.AddRange(InGameData.Instance.AllyList.Keys);
            randomTarget.AddRange(InGameData.Instance.EnemyList.Keys);
        }
        else
        {
            switch (camp)
            {
                case Camp.Ally:
                    randomTarget.AddRange(InGameData.Instance.EnemyList.Keys);
                    break;
                case Camp.Enemy:
                    randomTarget.AddRange(InGameData.Instance.AllyList.Keys);
                    break;
            }
        }

        for(int i = randomTarget.Count - 1; i >= 0; i--)
        {
            if (manager.GetCharacterTargetLock(randomTarget[i]) == true)
                randomTarget.Remove(randomTarget[i]);
        }
        

        randomIndex = UnityEngine.Random.Range(0, randomTarget.Count);
        CharacterBase character = randomTarget[randomIndex];

        InGameData.Instance.CurrentTargetCharacter = character;

        InvokeAction();
    }
    private void SetCasterNextTurn()
    {
        Messenger.Broadcast(Definition.SetPassNextTurn);

        InvokeAction();
    }
    private void MakeSkill()
    {
        var manager = InGameData.Instance;
        var caster = manager.CurrentTurnCharacter;
        var skillData = caster.CharacterInfo.characterSkillList;
        List<Generated.CsvData.SkillData> randomSkill = new List<Generated.CsvData.SkillData>();

        Attack_Lock attackLock = manager.GetCharacterAttackLock(caster);
        Type_Target_Auto_AI auto = manager.GetCharacterAuto_AI(caster);

        int randomIndex = -1;
        if(auto == Type_Target_Auto_AI.Anger)
        {
            // [FIX] Anger must use the basic skill; add it so randomSkill[0] is valid
            if (attackLock.HasFlag(Attack_Lock.Basic) == false && skillData.Count > 0)
            {
                randomSkill.Add(skillData[0]);
                randomIndex = 0;
            }
        }
        else
        {
            for (int i = 0; i < skillData.Count; i++)
                if (attackLock.HasFlag((Attack_Lock)i + 1) == false)
                    randomSkill.Add(skillData[i]);

            if (randomSkill.Count == 0)
                return; // ½ºÅµ

            if (caster.CharacterInfo.BattleCamp == Camp.Enemy)
                randomIndex = 0;
            else
                randomIndex = UnityEngine.Random.Range(0, randomSkill.Count);
        }
        
        // [FIX] guard empty/invalid index (e.g. Anger with basic locked) -> skip action
        if (randomIndex < 0 || randomIndex >= randomSkill.Count)
            return;

        InGameData.Instance.CurrentSkillData = randomSkill[randomIndex];

        InvokeAction();
    }
    private void MakeComplete()
    {
        if (InGameData.Instance.CurrentSkillData == null || InGameData.Instance.CurrentTargetCharacter == null)
            Messenger.Broadcast(Definition.PassClick);
        else
            Messenger.Broadcast(Definition.StartAIGrid);

        actionList.Clear();
        actionIndex = 0;
        currentState = State.None;
    }

    #endregion

    #region Action
    private IEnumerator SetAction(State state , Action action)
    {
        yield return new WaitUntil(() => currentState == State.None);
        currentState = state;

        action.Invoke();
        InvokeAction();
    }
    private void InvokeAction()
    {
        if (actionIndex >= actionList.Count) // [FIX] was '>' -> off-by-one IndexOutOfRange
            return;

        var action = actionList[actionIndex];
        actionIndex++;

        action.Invoke();
    }
    #endregion

    #region Messenger
    private void MessageAddListner()
    {
        Messenger.AddListener(Definition.SetMakeAIData, SetMakeAIData);
    }
    private void MessageRemoveListner()
    {
        Messenger.RemoveListener(Definition.SetMakeAIData, SetMakeAIData);
    }
    #endregion
}
