using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;


[Flags]
public enum Auto_AI
{
    None = 0,
    Anger = 1 << 0,
    Confusion = 1 << 1,
    Fascination = 1 << 2,
}
[Flags]
public enum Attack_Lock
{
    None = 0,
    Basic = 1 << 0,
    Skill1 = 1 << 1,
    Skill2 = 1 << 2,
    Max = 1 << 3,
}
public class CharacterState
{

    public Attack_Lock AttackLock { private set; get; }
    public CharacterState()
    {
        ActionLock = Type_Action_Lock.Possible;
        TargetLock = Type_Target_Lock.Possible;
        CC_Clear = Type_CC_Clear.None;
        AttackLock = Attack_Lock.None;
        Auto_AI = Type_Target_Auto_AI.None;
    }

    public Type_Action_Lock ActionLock { get; set; }
    public Type_Target_Lock TargetLock { get; set; }
    public Type_CC_Clear CC_Clear { get; set; }
    public Type_Target_Auto_AI Auto_AI { get; set; }

    public void InitState()
    {
        ActionLock = Type_Action_Lock.Possible;
        TargetLock = Type_Target_Lock.Possible;
        CC_Clear = Type_CC_Clear.None;
        AttackLock = Attack_Lock.None;
        
    }
    public void InsertAttackLock(int _attackLock)
    {
        if (AttackLock.HasFlag((Attack_Lock)_attackLock) == true)
            return;

        AttackLock |= (Attack_Lock)_attackLock;
    }
 
    public void InsertCCState(Type_Target_Lock _targetLcok, Type_Action_Lock _actionLock, Type_CC_Clear _ccClear)
    {
        if(TargetLock < _targetLcok)
            TargetLock = _targetLcok;
        if(ActionLock < _actionLock)
            ActionLock = _actionLock;
        if (CC_Clear < _ccClear)
            CC_Clear = _ccClear;
    }
}

public class InGameData : MonoBehaviour
{
    public static InGameData Instance = null;

    #region Data
    public float BurstGauge { get; set; }
    #endregion

    #region Round Data
    public int CurrentTurn { set; get; }
    public int CurrentRound { set; get; }
    public int CurrentWave { set; get; }
    public int MaxWave { set; get; }
    public Attributes CurrentAttribute { set; get; }
    #endregion

    #region Character List
    public Dictionary<CharacterBase, CharacterState> AllyList { private set; get; }
    public Dictionary<CharacterBase, CharacterState> EnemyList { private set; get; }
    public List<CharacterBase> TargetList { private set; get; }
    public CharacterBase CurrentTurnCharacter {  set; get; }
    public CharacterState CurrentCharacterState { set; get; }
    public CharacterBase CurrentTargetCharacter {  set; get; }
    #endregion

    #region Skill Data
    public Generated.CsvData.SkillData CurrentSkillData { set; get; }
    public Dictionary<string, GameObject> SkillEffect { set; get; }
    #endregion

    #region UI Data
    public CharacterProfile[] UIProfile { set; get; }
    public Texture2D[] TurnTimingTexture { set; get; }
    #endregion
    #region Trigger
    public Generated.CsvData.TriggerData CurrentTriggerData { set; get; }
    public List<Generated.CsvData.TriggerData.TriggerInfo> TriggerInfoList { get { return CurrentTriggerData.triggerInfoList; } }
    public Trigger_Timing Trigger_Timing { set; get; }
    public List<CharacterBase> TriggerTarget { set; get; }

    #endregion

    public bool PlayerAI { set; get; }

    #region Character Action
    public Transform SkillMovePosition { set; get; }
    #endregion

    #region Buff Data

    #endregion

    #region CC Data


    #endregion


    public int GetAttackAnimIndex
    {
        get
        {
            int index = CurrentTurnCharacter.CharacterInfo.characterSkillList.IndexOf(CurrentSkillData);
            index++;

            return index;
        }
    }
    public int GetRunAnimIndex
    {
        get 
        {
            if (CurrentSkillData == null)
                return 1;
            else
            {
                int index = CurrentTurnCharacter.CharacterInfo.characterSkillList.IndexOf(CurrentSkillData);
                index++;

                return index;
            }

        }
    }

    private void Awake()
    {
        if (Instance == null)
            Instance = this;

        InitailizeData();
    }

    private void InitailizeData()
    {
        AllyList = new Dictionary<CharacterBase, CharacterState>();
        EnemyList = new Dictionary<CharacterBase, CharacterState>();
        TargetList = new List<CharacterBase>();
        UIProfile = new CharacterProfile[20];
        SkillEffect = new Dictionary<string, GameObject>();
        TriggerTarget = new List<CharacterBase>();

        CurrentTurn = 0;
        CurrentRound = 1;
        CurrentWave = 1;

        PlayerAI = false;
    }

    public void ResetData()
    {
        CurrentTurnCharacter = null;
        CurrentTargetCharacter = null;
        CurrentTriggerData = null;
        SkillMovePosition = null;
        Trigger_Timing = Trigger_Timing.None;


        TargetList.Clear();
        Array.Clear(InGameData.Instance.UIProfile, 0, InGameData.Instance.UIProfile.Length);
    }

    public CharacterState GetCharacterState(CharacterBase target)
    {
        CharacterState state = null;

        Camp camp = target.CharacterInfo.BattleCamp;
        switch(camp)
        {
            case Camp.Ally:
                AllyList.TryGetValue(target, out state);
                break;
            case Camp.Enemy:
                EnemyList.TryGetValue(target, out state);
                break;
        }

        return state;
    }

    // [FIX] 죽어서 리스트에서 빠진 대상은 GetCharacterState가 null → 무가드 deref로 NRE(스톨). 전부 null 가드.
    public bool GetCharacterTargetLock(CharacterBase target)
    {
        CharacterState state = GetCharacterState(target);
        if (state == null) return false;
        return state.TargetLock == Type_Target_Lock.Impossible;
    }

    public bool GetCharacterActionLock(CharacterBase _target)
    {
        CharacterState state = GetCharacterState(_target);
        if (state == null) return false;
        return state.ActionLock == Type_Action_Lock.Impossible;
    }
    public bool GetCharacterCC_Clear(CharacterBase _target)
    {
        CharacterState state = GetCharacterState(_target);
        if (state == null) return false;
        return state.CC_Clear == Type_CC_Clear.Possible;
    }
    public Type_Target_Auto_AI GetCharacterAuto_AI(CharacterBase _target)
    {
        CharacterState state = GetCharacterState(_target);
        if (state == null) return Type_Target_Auto_AI.None;
        return state.Auto_AI;
    }
    public Attack_Lock GetCharacterAttackLock(CharacterBase _target)
    {
        CharacterState state = GetCharacterState(_target);
        if (state == null) return Attack_Lock.None;
        return state.AttackLock;
    }


    public void ChangeAttribute()
    {
        CurrentAttribute++;
        if (CurrentAttribute == Attributes.Eclipse)
            CurrentAttribute = Attributes.Day;
    }
}
  
