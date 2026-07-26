using Generated.CsvData;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MonsterLove.StateMachine;


public partial class Monster : CharacterBase
{
    private void Update()
    {
        //if (effectList != null)
        //{
        //    if (effectList.Count > 0)
        //        CheckEffectPlayStop();
        //}
    }

    #region Initialize
    protected override void InitializeData()
    {
        //ai = true;
        //effectList = new List<EffectData>();
    }
    public override void InitializeFSM()
    {
        base.InitializeFSM();
    }
    #endregion

    #region Battle
    public override void BattleEnter()
    {
        base.BattleEnter();
    }
    public override void BattleExit()
    {
        base.BattleExit();
    }
    public override void Result(CampType type)
    {
        base.Result(type);
    }
    #endregion


    #region Animation
    public override void InitializeCharacterAnimator()
    {
        base.InitializeCharacterAnimator();
        characterAnimator = GetComponent<Animator>();

        if (characterAnimation == null)
            characterAnimation = new AnimBase();

        characterAnimation = new Anim_Monster();

        characterAnimation.Init(characterAnimator);
        // MonsterStatus status = (MonsterStatus)characterInfo.CharacterStatus;
        // MonsterGrade grade = status.monsterGrade;
    }
    public override void DummyAnimEvent()
    {
        base.DummyAnimEvent();
    }
    public override void OnDamage()
    {
        base.OnDamage();
    }

    public override void ChangeState()
    {
        ChangeNextSubState();
    }
    public override void OnHeal()
    {
        base.OnHeal();
    }
    public override void OnHit()
    {
        base.OnHit();
    }
    #endregion


    #region Buff
    #endregion


    #region Skill
    public override void SkillAction(SkillData data, int skillListIndex)
    {
        
    }
    #endregion


    #region FSM
    protected override void ChangeFSMState(CHARACTER_FSM state)
    {
        base.ChangeFSMState(state);
    }
    #endregion


    #region Effect
    protected override void FindDinoAttachEffect()
    {
        base.FindDinoAttachEffect();
    }
    protected override void FindMonsterAttachEffect()
    {
        base.FindMonsterAttachEffect();
    }
    //public override void AttachEffect(int effectIndex)
    //{
    //    base.AttachEffect(effectIndex);
    //}
    #endregion

    protected override void OnDestroy()
    {
        base.OnDestroy();
    }

    #region Messenger
    protected override void MessengerAddListner()
    {
    }
    protected override void MessengerRemoveListner()
    {
    }
    #endregion
}
