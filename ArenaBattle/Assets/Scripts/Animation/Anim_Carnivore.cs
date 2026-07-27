using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Anim_Carnivore : AnimBase
{
    public override void Set_AttackAnimation(CharacterBase actor, int index)
    {
        AnimationManager.Instance.Set_AniInteger(actor, $"{AnimationType.iAttack}", index);
    }

    public override void Set_Move(CharacterBase actor, int move = 1)
    {
        AnimationManager.Instance.Set_AniInteger(actor, $"{AnimationType.iRun}", move);
    }

    public override void Set_Death(CharacterBase actor)
    {
        AnimationManager.Instance.Set_AniBool(actor, $"{AnimationType.bDeath}", true);
    }

    public override void Set_Idle(CharacterBase actor, int idle)
    {
        AnimationManager.Instance.Set_AniInteger(actor, $"{AnimationType.iIdle}", idle);
    }
    
    public override void Set_Hit(CharacterBase actor)
    {
        AnimationManager.Instance.Set_AniTrigger(actor, "tHit");
    }

    public override void Set_Burst(CharacterBase actor)
    {
        AnimationManager.Instance.Set_AniTrigger(actor, $"{AnimationType.tBurst}");
    }
}
