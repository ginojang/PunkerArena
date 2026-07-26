using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimBase
{
    public enum AnimationType
    {
        None,
        bGround,
        iRun,
        bAttack,
        bDeath,
        tHit,
        iAttack,
        bStun,
        bHit,
        iIdle,
        tBurst,
    }

    public GameObject m_Actor;
    private Animator m_animator;
    private Animation m_animation;

    CampType m_ePlayerType;

    public string m_curEventAnimName = "";
    public Dictionary<int, string> m_EventList = new Dictionary<int, string>();

    public AnimBase()
    {
    }

    public Animator Get_Animator
    {
        get
        {
            return m_animator;
        }
    }

    public Animation Animation
    {
        get
        {
            return m_animation;
        }
        set
        {
            m_animation = value;
        }
    }

    public CampType CharType
    {
        get { return m_ePlayerType; }
        set { m_ePlayerType = value; }
    }

    public void Init(Animator animator)
    {
        if (animator == null)
        {
        }
        else
        {
            if (m_animator == null)
            {
                m_animator = animator;
            }
        }
    }
    public void ClearEvent()
    {
        m_EventList.Clear();
    }

    public virtual void Set_AttackAnimation(CharacterBase actor, int index)
    {

    }
    public virtual void SetGround(CharacterBase actor, bool ground)
    {

    }
    public virtual void Set_Move(CharacterBase actor, int move = 1)
    {
        //Set_AniBool(actor, ANIM_PARAMS_CHARACTER.bWalk, move);
    }

    public virtual void Set_Hit(CharacterBase actor)
    {
        
    }
    public virtual void Set_Death(CharacterBase actor)
    {
        //Set_AniTrigger(actor, ANIM_PARAMS_CHARACTER.tDie);
    }
    public virtual void Set_Idle(CharacterBase actor, int idle)
    {

    }
    public virtual void Set_Burst(CharacterBase actor)
    {

    }
}
