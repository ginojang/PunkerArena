using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//애니메이션 조건
public partial class Character
{
    private void MeleeAction_Enter()
    {
        DebugLog("MeleeAttack");

        MeleeAttack();
    }

    private void MeleeAction_Exit()
    {
        Get_AnimBase.Set_AttackAnimation(this, 0);
        Messenger.Broadcast(Definition.StartCheckTrigger);
    }
    private void MeleeAttack()
    {
        var data = InGameData.Instance.CurrentSkillData;
        GameObject effect = null;

        int animation = InGameData.Instance.GetAttackAnimIndex;
        characterAnimation.Set_AttackAnimation(this, animation);

        if (data.res_fx_attack != "none")
        {
            string attach = CSVDataManager.GetTable<EffectTable>().GetData(data.res_fx_attack).active_pos_type;
            Link_Bone_Move link_bone_move = CSVDataManager.GetTable<EffectTable>().GetData(data.res_fx_attack).link_bone_move;

            GameObject parent = null;

            if(effectTransDic.TryGetValue(attach, out parent) == false)
                Debug.LogError("Effect Attach Point is None");    

            if (link_bone_move == Link_Bone_Move.Fix)
                parent = gameObject;

            EffectManager.Instance.PlayDoOnceEffect(data.res_fx_attack, parent.transform);
        }
    }

}
