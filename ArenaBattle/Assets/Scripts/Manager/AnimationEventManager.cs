using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Generated.CsvData;

public class eventInfo
{
    public eventInfo(CharacterBase character, eventData eventdata)
    {
        actor = character;
        data = eventdata;
    }
    public CharacterBase actor;
    public eventData data;
}

public class AnimationEventManager : MonoBehaviour
{
    public static AnimationEventManager instance_value;

    public static AnimationEventManager Instance
    {
        get
        {
            if (instance_value == null)
            {
                GameObject go = new GameObject("AnimationEventManager");
                instance_value = go.AddComponent<AnimationEventManager>();
                ImmortalGameObject.AttachObject(go);
#if DINO_SKILL_EDITOR
                instance_value.Initialize();
#endif
            }

            return instance_value;
        }
    }

    private List<eventInfo> eventStack = new List<eventInfo>();
    
    private DinoSkillTool dinoSkillTool;
#if DINO_SKILL_EDITOR
    private void Initialize()
    {
        dinoSkillTool = GameObject.Find("DinoSkillTool").GetComponent<DinoSkillTool>();
    }
#endif

    public void AttackEndEvent(CharacterBase actor, bool stopCoroutine = true)
    {
        actor.StopAllCoroutines();

        for (int i = 0; i < actor.Get_AnimBase.m_EventList.Count; i++)
        {
            StopCoroutine(actor.Get_AnimBase.m_EventList[i]);
        }

        actor.Get_AnimBase.m_curEventAnimName = "";
        actor.Get_AnimBase.m_EventList.Clear();
    }

    public void AllClearEvent()
    {
    }

    public void AddCharacterEvent(CharacterBase actor, eventData data)
    {
#if DINO_SKILL_EDITOR
        eventInfo info = null;
        if (actor.Get_AnimBase.CharType == CampType.Enemy)
        {
            info = new eventInfo(actor, dinoSkillTool.curMonsterSkillEventList);
            info.data.aniLenght = data.aniLenght;
            dinoSkillTool.aniLength = data.aniLenght;
        }
        else
        {
            info = new eventInfo(actor, dinoSkillTool.curSkillEventList);
            info.data.aniLenght = data.aniLenght;
            dinoSkillTool.aniLength = data.aniLenght;
        }

        eventStack.Add(info);
#else
        eventInfo info = new eventInfo(actor, data);
        eventStack.Add(info);
#endif
    }
    
    private void Update()
    {
        if (eventStack.Count > 0)
        {
            SetAnimationEvent(eventStack[0].actor, eventStack[0].data);
            eventStack.Remove(eventStack[0]);
        }
    }

    // 테이블로 진행 시
    public void SetAnimationEvent(CharacterBase actor, eventData data)
    {
        int count = data.eventCount;
        if (count > 0)
        {
            for (int i = 0; i < data.eventCount; i++)
            {
                object[] param = null;
                if (data.eventList[i].param == null)
                    param = new object[] { data.eventList[i].fTime, actor, data.eventList[i].effectarrayidx, data.eventList[i].effecttarget };
                else
                    param = new object[] { data.eventList[i].fTime, actor, data.eventList[i].param };

                bool bExist = false;
/*                for (int k = 0; k < actor.Get_AnimBase.m_EventList.Count; k++)
                {
                    if (actor.Get_AnimBase.m_EventList[k] == data.eventList[i].eventName)
                    {
                        bExist = true;
                        break;
                    }
                }*/

                // 중복적인 함수 실행 가능하게 일부 수정
                if (!bExist)
                {
#if DINO_SKILL_EDITOR
                    dinoSkillTool.realPlay = true;
                    dinoSkillTool.StartCoroutine(data.eventList[i].eventName, param);
#else
                    StartCoroutine(data.eventList[i].eventName, param);
#endif
                    if(!actor.Get_AnimBase.m_EventList.ContainsKey(i))
                        actor.Get_AnimBase.m_EventList.Add(i, data.eventList[i].eventName);
                }
            }
        }
    }

    //EX
    IEnumerator OnDamage(object[] param)
    {
        yield return new WaitForSeconds((float)param[0]);
        CharacterBase actor = (CharacterBase)param[1];

        actor.OnDamage();
    }
    IEnumerator RangeAttack(object[] param)
    {
        yield return new WaitForSeconds((float)param[0]);
        CharacterBase actor = (CharacterBase)param[1];

        actor.RangeAttack();
    }

    IEnumerator ChangeState(object[] param)
    {
        yield return new WaitForSeconds((float)param[0]);
        CharacterBase actor = (CharacterBase)param[1];

        actor.ChangeState();
    }

    IEnumerator AnimEventClearAttack(object[] param)
    {
        yield return new WaitForSeconds((float)param[0]);

        CharacterBase actor = (CharacterBase)param[1];
        /*
                if (actor.Get_AnimBase.Interval)
                {
                    actor.Get_AnimBase.Interval = false;
                    AnimationManager.Instance.Reset_AnimParam(actor);

                    actor.GetComponent<Defender>().Reset_Defender();
                }
        */
    }
    IEnumerator AttachEffect(object[] param)
    {
        yield return new WaitForSeconds((float)param[0]);
        CharacterBase actor = (CharacterBase)param[1];

        //actor.AttachEffect((int)param[2], (EffectTarget)param[3]);
    }

}
