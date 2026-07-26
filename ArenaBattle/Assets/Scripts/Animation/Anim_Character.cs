using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Anim_Character : AnimBase
{
	public Anim_Character(CampType camp)
	{
        CharType = camp;
	}

    //public override void NormalAttack(CharacterBase actor)
    //{
    //    base.NormalAttack(actor);
    //}
    //public override void Set_AttackIdle(CharacterBase actor, bool attack)
    //{
    //    base.Set_AttackIdle(actor, attack);
    //    AnimationManager.Instance.Set_AniBool(actor, ANIM_PARAMS_CHARACTER.bAttackIdle, attack);
    //}
    //public override void Set_Die(CharacterBase actor)
    //{
    //    base.Set_Die(actor);
    //    AnimationManager.Instance.Set_AniTrigger(actor, ANIM_PARAMS_CHARACTER.tDie);
    //}
    //public override void Set_Hit(CharacterBase actor)
    //{
    //    base.Set_Hit(actor);
    //    AnimationManager.Instance.Set_AniTrigger(actor, ANIM_PARAMS_CHARACTER.tHit);
    //}
    //public override void Set_MeleeAttackAnimation(CharacterBase actor)
    //{
    //    base.Set_MeleeAttackAnimation(actor);
    //    AnimationManager.Instance.Set_AniTrigger(actor, ANIM_PARAMS_CHARACTER.tAttack);
    //}
    //public override void Set_MultiMeleeAttackAnimation(CharacterBase actor)
    //{
    //    base.Set_MultiMeleeAttackAnimation(actor);
    //    AnimationManager.instance_value.Set_AniTrigger(actor, ANIM_PARAMS_CHARACTER.tAttack3);
    //}
    //public override void Set_Move(CharacterBase actor, bool move)
    //{
    //    base.Set_Move(actor, move);
    //    AnimationManager.Instance.Set_AniBool(actor, ANIM_PARAMS_CHARACTER.bWalk, move);
    //}
    //public override void Set_RangeAttackAnimation(CharacterBase actor)
    //{
    //    base.Set_RangeAttackAnimation(actor);
    //    AnimationManager.Instance.Set_AniTrigger(actor, ANIM_PARAMS_CHARACTER.tAttack2);
    //}
    //public override void Set_Victory(CharacterBase actor)
    //{
    //    base.Set_Victory(actor);
    //    AnimationManager.Instance.Set_AniTrigger(actor, ANIM_PARAMS_CHARACTER.tVictory);
    //}
}
