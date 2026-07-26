using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;


//애니메이션 기준
public partial class Character
{
    private void OnDamage_Enter()
    {
        setDamageInfo = false;
        SetDamageInfo();
    }

    private void OnDamage_Exit()
    {
        setDamageInfo = false;

        if (InGameData.Instance.GetCharacterCC_Clear(this) == true)
            Messenger.Broadcast(Definition.CheckRemoveCC_Clear, (CharacterBase)this);
    }

    private void SetDamageInfo()
    {
        if (damageInfo == null)
            damageInfo = new DamageInfo();

        var skillData = InGameData.Instance.CurrentSkillData;
        string hit = skillData.res_fx_hit;

        var effectData = EffectManager.Instance.GetData(hit);

        GameObject parent = GetEffectParent(effectData.correct_pos, effectData.link_bone_move);

        damageInfo = new DamageInfo();
        damageInfo.HitEffect = hit;
        damageInfo.HitParent = parent;

        setDamageInfo = true;
    }
}
