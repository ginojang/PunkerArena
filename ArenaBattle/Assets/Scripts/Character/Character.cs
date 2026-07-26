using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using Generated.CsvData;
using MonsterLove.StateMachine;
using System;


public partial class Character : CharacterBase
{
    #region Initialize
    protected override void InitializeData()
    {
        //effectList = new List<EffectData>();
    }
    public override void InitializeFSM()
    {
        base.InitializeFSM();
    }
    protected override void ChangeFSMState(CHARACTER_FSM state)
    {
        base.ChangeFSMState(state);
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
    #endregion

    #region SkillSetting
    public override void SkillAction(SkillData data, int skillListIndex)
    {
        //actionData.InitializeSubStateData(this);
        AddSkillCool(skillListIndex);
    }
    public override void AddSkillCool(int skillListIndex)
    {

    }


    #endregion

    #region Battle
   
    public override void Result(CampType type)
    {
        //if(characterInfo.CharacterCampType == type)
        //    victory = true;
        //else
        //    victory = false;

        //ChangeFSMState(CHARACTER_FSM.Result);
    }
    #endregion

    #region Animation
    public override void InitializeCharacterAnimator()
    {
        base.InitializeCharacterAnimator();
        characterAnimator = GetComponent<Animator>();

        if (characterAnimation == null)
            characterAnimation = new AnimBase();

        switch(characterInfo.Character_Talent)
        {
            case CharacterTalent.Carnivore:
                characterAnimation = new Anim_Carnivore();
                break;
            case CharacterTalent.Herbivore:
                characterAnimation = new Anim_Herbivore();
                break;
            case CharacterTalent.Omnivore:
                characterAnimation = new Anim_Omnivore();
                break;
        }

        characterAnimation.Init(characterAnimator);
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
    public override void RangeAttack()
    {
        base.RangeAttack();
    }
    #endregion

    #region Effect
    //public override void AttachEffect(int effectIndex)
    //{
    //    base.AttachEffect(effectIndex);
    //}
    protected override void FindDinoAttachEffect()
    {
        base.FindDinoAttachEffect();
    }

    #endregion

    #region Buff
    #endregion



    #region BattleMessenger
    protected override void MessengerAddListner()
    {
    }
    protected override void MessengerRemoveListner()
    {
    }
    #endregion

    protected override void OnDestroy()
    {
        base.OnDestroy();
        MessengerRemoveListner();
        
    }

}
