using UnityEngine;
using Generated.CsvData;
public class Animation_EventTrigger : StateMachineBehaviour
{
    string aniname = "";
    AnimatorClipInfo[] next = null;
    //    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex, AnimatorControllerPlayable controller)
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        //bRes = true;
        AnimatorClipInfo[] cur = animator.GetCurrentAnimatorClipInfo(layerIndex);// GetCurrentAnimatorClipInfo(layerIndex);
        next = animator.GetNextAnimatorClipInfo(layerIndex);// GetCurrentAnimatorClipInfo(layerIndex);

        // 다음 정보가 없다면 현재 정보가 실행된 것이므로 현재 정보로 이벤트 정보를 얻어온다
        if (next.Length == 0)
        {
            if (cur.Length > 0)
            {
                aniname = cur[0].clip.name;

                // aniname으로 테이블 검색해서 이벤트 정보 얻어온다
                string str = string.Format("Player   {0}   Player Animation Name   {1}", animator.name, aniname);
                CharacterBase actor = animator.GetComponent<CharacterBase>();

                // 테이블로 진행 시
                eventData data = CSVDataManager.GetTable<Anim_EventTable>().GetData(aniname);
                if (data != null)
                {
                    data.aniLenght = cur[0].clip.length;
                    AnimationEventManager.Instance.AddCharacterEvent(actor, data);
                }
                /*
                // 테이블 없이 진행시 
                AnimEvent eventdata = Animation_EventTable.Get_AnimEvent(aniname);
                if (data != null)
                    AnimationEventManager.Instance.AddCharacterEvent(actor, eventdata);
                */
                Debug.Log(str);
            }
            else
                Debug.LogError("Animation Event Info is NULL!!!!!");
        }
        else
        {
            if (next.Length > 0)
            {
                aniname = next[0].clip.name;
                aniname.Replace("  ", string.Empty);

                // aniname으로 테이블 검색해서 이벤트 정보 얻어온다
                //                string str = string.Format("Player   {0}   Player Animation Name   {1}", animator.name, aniname);

                CharacterBase actor = animator.GetComponent<CharacterBase>();

                if (actor == null || (actor != null && actor.Get_AnimBase == null))
                    return;

                if (actor.Get_AnimBase.m_curEventAnimName == aniname)
                    return;

                actor.Get_AnimBase.m_curEventAnimName = aniname;

                /*
				// 테이블 없이 진행시
				if (next[0].clip.events.Length > 0)
				{
                    AnimEvent eventdatainfo = new AnimEvent();
                    eventdatainfo.anim_name = aniname;
                    eventdatainfo.m_fTotalTime = next[0].clip.length;

                    for (int i = 0; i < next[0].clip.events.Length; i++)
                    {
                        AnimEventData data = new AnimEventData();
                        data.fTime = next[0].clip.events[i].time;
                        data.event_name = next[0].clip.events[i].stringParameter;
                        eventdatainfo.m_lstEvent.Add(data);
                        //next[0].clip.events[i].
                    }

                    if (eventdatainfo != null && eventdatainfo.m_lstEvent[0].fTime != 0)
                    {
                        AnimationEventManager.Instance.AddCharacterEvent(actor, eventdatainfo);
                    }
                }
                */

                // 테이블로 진행시
                eventData data = CSVDataManager.GetTable<Anim_EventTable>().GetData(aniname);

                if (data != null && data.eventList[0].fTime != 0)
                {
                    data.aniLenght = next[0].clip.length;
                    AnimationEventManager.Instance.AddCharacterEvent(actor, data);
                }
            }
			else
                Debug.LogError("Animation Event Info is NULL!!!!!");
        }
    }

    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        AnimatorClipInfo[] cur = animator.GetCurrentAnimatorClipInfo(layerIndex);
        if (cur.Length > 0)
        {
            aniname = cur[0].clip.name;
        }
    }
}
