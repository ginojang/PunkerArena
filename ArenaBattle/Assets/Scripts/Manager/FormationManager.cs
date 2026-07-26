using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum EFormationType
{
    None,
    NormalBattle,
    RaidBattle,
}

public class FormationElement
{
    public EFormationType mType = EFormationType.None;
    public int mSlotIndex = -1;
    public List<FormationSlotElement> m_List = new List<FormationSlotElement>();

    public void Add(int _index, int _objectID)
    {
        FormationSlotElement addSlotElement = new FormationSlotElement(_index, _objectID);
        m_List.Add(addSlotElement);
    }

    public void Remove(int _index)
    {
        for( int i = 0; i < m_List.Count; i++ )
        {
            if (m_List[i].mSlotIndex != _index)
                continue;

            m_List.RemoveAt(i);
            break;
        }
    }

    public void Swap(int _srcSlotIndex, int _srcObjectID, int _targetSlotIndex, int _targetObjectID)
    {
        // 포메이션은 중간에 비어있을수가 있으므로 슬롯인덱만 바꾸어준다.
        FormationSlotElement srcSlotElement = _srcSlotIndex < m_List.Count ? m_List[_srcSlotIndex] : null;
        FormationSlotElement targetSlotElement = _targetSlotIndex < m_List.Count ? m_List[_targetSlotIndex] : null;

        if(srcSlotElement == null)
        {
            Add(_srcSlotIndex, _srcObjectID);
        }

        srcSlotElement.Set(_targetSlotIndex, _targetObjectID);

        if(targetSlotElement != null)
            targetSlotElement.Set(_srcSlotIndex, _srcObjectID);
    }
}

public class FormationSlotElement
{
    public int mSlotIndex = -1;
    public int mObjectID = -1;

    public FormationSlotElement(int _slotIndex, int _objectID)
    {
        Set(_slotIndex, _objectID);
    }

    public void Set(int _slotIndex, int _objectID)
    {
        mSlotIndex = _slotIndex;
        mObjectID = _objectID;
    }
}

public class FormationManager : MonoBehaviour
{
    private static FormationManager instance_value;

    public static FormationManager Instance
    {
        get
        {
            if (instance_value == null)
            {
                GameObject go = new GameObject("FormationManager");
                instance_value = go.AddComponent<FormationManager>();
                ImmortalGameObject.AttachObject(go);
            }

            return instance_value;
        }
    }

    private Dictionary<EFormationType, List<FormationElement>> mDic = 
        new Dictionary<EFormationType, List<FormationElement>>();

    public void Init()
    {
        Clear();
    }

    public void Set()
    {
        // 스왑에서 추가가 일어나므로 걍 스왑함수로 다 퉁치자.
        //Swap
    }

    public void Clear()
    {
        mDic.Clear();
    }
}
