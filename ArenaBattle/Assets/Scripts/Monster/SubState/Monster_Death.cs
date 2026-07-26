using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public partial class Monster
{
    private void Death_Enter()
    {
        DebugLog("Death");

        if (CharacterInfo.CharacterStatus.hp > 0)
        {
            ChangeNextSubState();
            return;
        }

        characterAnimation.Set_Death(this);
    }
    private void Death_Update()
    {

    }
    private void Death_Exit()
    {

    }

    //public override void Death()
    //{
    //    characterInfo.Death = true;

    //    Messenger.Broadcast(Definition.CharacterDeath, characterInfo.CharacterStatus);
    //    Messenger.Broadcast(Definition.CharacterInfoSliderOFF, (CharacterBase)this);

    //    var index = CSVDataManager.GetEffectIndex("fx_death");

    //    var effect = EffectManager.Instance.CreateEffect(index);
    //    effect.transform.SetParent(transform.parent);
    //    effect.transform.localPosition = Vector3.zero;

    //    Tweener tween = transform.DOMoveY(-2, 0.3f);
    //    tween.onKill = () =>
    //    {
    //        DestroyAllEffect();
    //        ChangeNextSubState();
    //    };
    //}
}
