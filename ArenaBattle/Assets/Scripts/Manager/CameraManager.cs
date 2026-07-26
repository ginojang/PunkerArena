using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum ANIM_PARAMS_CAMERA
{
    owner_intro,
    enemy_intro,
    battle_init,
    attack_normal,
    attack_skill,
    Max,
};

public class CameraManager : MonoBehaviour
{
    private static CameraManager m_this = null;
	
    public static CameraManager Instance
    {
        get
        {
            return m_this;
        }
    }

    private Animator m_animator;
    private Animation m_animation;
    private List<int> animParamsId_Camera = new List<int>();
    
    private bool isPlay = false;
    private Camera curCamera = null;

    public Camera Active_Camera
    {
        get { return curCamera; }
        set { curCamera = value;
            m_animator = curCamera.GetComponent<Animator>();
        }
    }

    public bool ISPLAY
    {
        get { return isPlay; }
    }

    public void Initialize()
    {
        for (int idx = 0; idx < (int)ANIM_PARAMS_CAMERA.Max; idx++)
        {
            string idStr = ((ANIM_PARAMS_CAMERA)(idx)).ToString();
            int id = Animator.StringToHash(idStr);
            animParamsId_Camera.Add(id);
        }
    }
    
    public int GetAnimParamID(object id)
    {
        if ((ANIM_PARAMS_CAMERA)id >= 0 && (int)(ANIM_PARAMS_CAMERA)id < animParamsId_Camera.Count)
        {
            return animParamsId_Camera[(int)id];
        }

        return -1;
    }

    public bool GetBool(object id)
    {
        return m_animator.GetBool(GetAnimParamID(id));
    }

    public int GetInt(object id)
    {
        return m_animator.GetInteger(GetAnimParamID(id));
    }

    public float GetFloat(CharacterBase actor, object id)
    {
        return m_animator.GetFloat(GetAnimParamID(id));
    }

    public void Set_AniBool(string id, bool flag)
    {
        m_animator.SetBool(id, flag);
    }

    public void Set_AniBool(object id, bool flag)
    {
        int index = GetAnimParamID(id);

        m_animator.SetBool(index, flag);
    }

    public void Set_AniInteger(string id, int value)
    {
        m_animator.SetInteger(id, value);
    }

    public void Set_AniInteger(object id, int value)
    {
        int index = GetAnimParamID(id);

        m_animator.SetInteger(index, value);
    }

    public void Set_AniFloat(string id, float value)
    {
        m_animator.SetFloat(id, value);
    }

    public void Set_AniFloat(object id, float value)
    {
        int index = GetAnimParamID(id);

        m_animator.SetFloat(index, value);
    }

    public void Set_AniTrigger(string id)
    {
        m_animator.SetTrigger(id);
    }

    public void Set_AniTrigger(object id)
    {
        int index = GetAnimParamID(id);

        m_animator.SetTrigger(index);
    }

    public void Set_AniResetTrigger(object id)
    {
        int index = GetAnimParamID(id);

        m_animator.ResetTrigger(index);
    }
}
