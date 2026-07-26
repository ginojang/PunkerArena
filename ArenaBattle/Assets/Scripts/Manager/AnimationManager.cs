using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ANIM_PARAMS_CHARACTER
{
    bWalk,
    tAttack,
    tAttack2,
    tAttack3,
    tHit,
    tDie,
    bAttackIdle,
    tVictory,
    Max,
};

public enum ANIM_PARAMS_MONSTER
{
    bWalk,
    tAttack,
    tAttack2,
    tAttack3,
    tHit,
    tDie,
    bAttackIdle,
    tVictory,
    Max,
};

public class AnimationManager : MonoBehaviour
{
    public static AnimationManager instance_value;

    public static AnimationManager Instance
    {
        get
        {
            if (instance_value == null)
            {
                GameObject go = new GameObject("AnimationManager");
                instance_value = go.AddComponent<AnimationManager>();
                ImmortalGameObject.AttachObject(go);
                
                instance_value.Initialize();
            }

            return instance_value;
        }
    }

    private List<int> animParamsId_Character = new List<int>();
    private List<int> animParamsId_Monster = new List<int>();

    public void Initialize()
	{
        MakeanimParamsId();
	}

    protected void MakeanimParamsId()
    {
        for (int idx = 0; idx < (int)ANIM_PARAMS_CHARACTER.Max; idx++)
        {
            string idStr = ((ANIM_PARAMS_CHARACTER)(idx)).ToString();
            int id = Animator.StringToHash(idStr);
            animParamsId_Character.Add(id);
        }

        for (int idx = 0; idx < (int)ANIM_PARAMS_MONSTER.Max; idx++)
        {
            string idStr = ((ANIM_PARAMS_MONSTER)(idx)).ToString();
            int id = Animator.StringToHash(idStr);
            animParamsId_Monster.Add(id);
        }
    }

    public int GetAnimParamID(object id, CampType chartype)
    {
        switch (chartype)
        {
            case CampType.Owner:
                {
                    if ((ANIM_PARAMS_CHARACTER)id >= 0 && (int)(ANIM_PARAMS_CHARACTER)id < animParamsId_Character.Count)
                    {
                        return animParamsId_Character[(int)id];
                    }
                }
                break;
            case CampType.Enemy:
                {
                    if ((ANIM_PARAMS_MONSTER)id >= 0 && (int)(ANIM_PARAMS_MONSTER)id < animParamsId_Monster.Count)
                    {
                        return animParamsId_Monster[(int)id];
                    }
                }
                break;
        }

        return -1;
    }

    Dictionary<int, int> animParamDic = new Dictionary<int, int>();

    public int GetAnimRandomParam(int id)
    {
        if (animParamDic.ContainsKey(id))
        {
            return animParamDic[id];
        }
        return -1;
    }

    public bool GetBool(CharacterBase actor, object id)
    {
        return actor.Get_AnimBase.Get_Animator.GetBool(GetAnimParamID(id, actor.Get_AnimBase.CharType));
    }

    public int GetInt(CharacterBase actor, object id)
    {
        return actor.Get_AnimBase.Get_Animator.GetInteger(GetAnimParamID(id, actor.Get_AnimBase.CharType));
    }

    public float GetFloat(CharacterBase actor, object id)
    {
        return actor.Get_AnimBase.Get_Animator.GetFloat(GetAnimParamID(id, actor.Get_AnimBase.CharType));
    }

    public void Set_AniBool(CharacterBase actor, string id, bool flag)
    {
        if (actor == null)
            Debug.Log("m_animator null - " + actor.transform.gameObject.name);

        actor.Get_AnimBase.Get_Animator.SetBool(id, flag);
    }

    public void Set_AniBool(CharacterBase actor, object id, bool flag)
    {
        if (actor == null)
            Debug.Log("m_animator null - " + actor.transform.gameObject.name);

        int index = GetAnimParamID(id, actor.Get_AnimBase.CharType);

        actor.Get_AnimBase.Get_Animator.SetBool(index, flag);
    }

    public void Set_AniInteger(CharacterBase actor, string id, int value)
    {
        actor.Get_AnimBase.Get_Animator.SetInteger(id, value);
    }

    public void Set_AniInteger(CharacterBase actor, object id, int value)
    {
        int index = GetAnimParamID(id, actor.Get_AnimBase.CharType);

        //int i = actor.Get_AnimBase.Get_Animator.GetInteger(index);
        actor.Get_AnimBase.Get_Animator.SetInteger(index, value);
    }

    public void Set_AniFloat(CharacterBase actor, string id, float value)
    {
        actor.Get_AnimBase.Get_Animator.SetFloat(id, value);
    }

    public void Set_AniFloat(CharacterBase actor, object id, float value)
    {
        int index = GetAnimParamID(id, actor.Get_AnimBase.CharType);

        actor.Get_AnimBase.Get_Animator.SetFloat(index, value);
    }



    public void Set_AniTrigger(CharacterBase actor, string id)
    {
        actor.Get_AnimBase.Get_Animator.SetTrigger(id);
    }

    public void Set_AniTrigger(CharacterBase actor, object id)
    {
        if (actor == null)
            Debug.Log("m_animator null - " + actor.transform.gameObject.name);

        int index = GetAnimParamID(id, actor.Get_AnimBase.CharType);

        actor.Get_AnimBase.Get_Animator.SetTrigger(index);
    }

    public void Set_AniResetTrigger(CharacterBase actor, object id)
    {
        if (actor == null)
            Debug.Log("m_animator null - " + actor.transform.gameObject.name);

        int index = GetAnimParamID(id, actor.Get_AnimBase.CharType);

        actor.Get_AnimBase.Get_Animator.ResetTrigger(index);
    }
}