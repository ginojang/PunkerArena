using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Generated.CsvData;



public class CCManager : MonoBehaviour
{
    public class CCData
    {
        private WeakReference<CharacterBase> caster = null;
        private WeakReference<CharacterBase> owner = null;

        Trigger_Timing timing = Trigger_Timing.None;
        private int duration = 0;
        private int count = 0;

        private Skill_CcData data = null;

        public Cc_Group CCGroup
        {
            get { return data.group; }
        }
        public int GetDuration
        {
            get { return duration; }
        }
        public Type_Attack_Lock GetAttackLock
        {
            get { return data.type_attack_lock; }
        }
        public Type_Action_Lock GetActionLock
        {
            get { return data.type_action_lock; }
        }
        public Type_Target_Lock GetTargetLock
        {
            get { return data.type_target_lock; }
        }
        public Type_CC_Clear GetCC_Clear
        {
            get { return data.type_cc_clear; }
        }
        public Type_Target_Auto_AI GetAutoAI
        {
            get { return data.type_target_auto_ai; }
        }
        public int Priority_Level
        {
            get { return data.group_priority_level; }
        }

        public void InitData(TriggerData.TriggerInfo _info, ref Skill_CcData _data, ref CharacterBase _caster, ref CharacterBase _owner)
        {
            caster = new WeakReference<CharacterBase>(_caster);
            owner = new WeakReference<CharacterBase>(_owner);

            data = _data;

            duration = _info.action_duration_turn;
            count = _info.action_count;

            CharacterBase charbase = null;
            owner.TryGetTarget(out charbase);
            EffectManager.Instance.AddEffectList(charbase.gameObject, data.res_fx_cc, objectType.character, true);

            if (data.res_atlas != "none" && data.res_icon != "none")
                Messenger.Broadcast(Definition.InsertInfoIcon, charbase, data.res_atlas, data.res_icon, Action_Type.CC);

            // Start Ani
        }

        public bool DecreaseCCDuration()
        {
            duration--;
            if(duration <=0)
            {
                DeleteData();
                return true;
            }

            return false;
        }

        public void DeleteData()
        {
            CharacterBase charbase = null;
            owner.TryGetTarget(out charbase);

            Messenger.Broadcast(Definition.RemoveInfoIcon, charbase, data.res_icon, Action_Type.CC);
            EffectManager.Instance.DestroyEffect(charbase.gameObject, data.res_fx_cc);
        }
    }

    private Skill_CcTable table = null;
    private Dictionary<CharacterBase, List<CCData>> characterCCDic;

    private void Awake()
    {
        InitData();
        MessageAddListner();
    }
    private void OnDestroy()
    {
        MassageRemoveListner();
    }
    private void InitData()
    {
        table = CSVDataManager.GetTable<Skill_CcTable>();
        characterCCDic = new Dictionary<CharacterBase, List<CCData>>();
    }

    #region InsertCC
    private void InsertCC(TriggerData.TriggerInfo _info)
    {
        var targetList = InGameData.Instance.TriggerTarget;
        var data = table.GetData(_info.action_id);
        var caster = InGameData.Instance.CurrentTurnCharacter;

        for(int i = 0; i < targetList.Count; i++)
        {
            CCData cc = new CCData();
            CharacterBase owner = targetList[i];
            cc.InitData(_info, ref data, ref caster, ref owner);

            List<CCData> ccList = null;
            if(characterCCDic.TryGetValue(owner, out ccList) == false)
            {
                ccList = new List<CCData>();
                characterCCDic.Add(owner, ccList);
            }

            ccList.Add(cc);
        }

        Messenger.Broadcast(Definition.InsertComplete);
    }
    #endregion 

    #region Remove
    private void CheckRemoveCC()
    {
        foreach(var item in characterCCDic)
        {
            List<CCData> ccList = item.Value;
            for(int i = ccList.Count - 1; i >= 0; i--)
            {
                bool remove = ccList[i].DecreaseCCDuration();
                if (remove)
                    ccList.Remove(ccList[i]);
            }
        }

        Messenger.Broadcast(Definition.BattleManagerInvokeAction);
    }

    private void CheckRemoveCC_Clear(CharacterBase target)
    {
        List<CCData> dataList = null;
        characterCCDic.TryGetValue(target, out dataList);

        for(int i =dataList.Count - 1; i >= 0; i--)
        {
            if (dataList[i].GetCC_Clear == Type_CC_Clear.Possible)
            {
                dataList[i].DeleteData();
                dataList.Remove(dataList[i]);
            }
        }
    }
    #endregion

    #region Get
    private void GetCharacterState(CharacterBase owner, ref CharacterState state)
    {
        List<CCData> dataList = null;

        if(characterCCDic.TryGetValue(owner, out dataList) == true)
        {
            int prev = 0;
            foreach (var item in dataList)
            {
                switch (item.GetAttackLock)
                {
                    case Type_Attack_Lock.Basic:
                        state.InsertAttackLock((int)Type_Attack_Lock.Basic);
                        break;
                    case Type_Attack_Lock.Skill1:
                        state.InsertAttackLock((int)Type_Attack_Lock.Skill1);
                        break;
                    case Type_Attack_Lock.Skill2:
                        state.InsertAttackLock((int)Type_Attack_Lock.Skill2);
                        break;
                    case Type_Attack_Lock.Skills:
                        for (int i = (int)Type_Attack_Lock.Skill1; i < (int)Type_Attack_Lock.Skills; i++)
                            state.InsertAttackLock(i);
                        break;
                    case Type_Attack_Lock.All:
                        for(int i = (int)Type_Attack_Lock.Basic; i < (int)Type_Attack_Lock.Skills; i++)
                            state.InsertAttackLock(i);
                        break;
                }

                if(prev < item.Priority_Level)
                {
                    state.Auto_AI = item.GetAutoAI;
                    prev = item.Priority_Level;
                }

                state.InsertCCState(item.GetTargetLock, item.GetActionLock, item.GetCC_Clear);
            }
        }
    }

    private void SetCharacterState()
    {
        var charDic = InGameData.Instance.AllyList;
        foreach(var data in charDic)
        {
            CharacterState state = data.Value;
            state.InitState();
            GetCharacterState(data.Key, ref state);
        }
        
        charDic = InGameData.Instance.EnemyList;
        foreach (var data in charDic)
        {
            CharacterState state = data.Value;
            state.InitState();
            GetCharacterState(data.Key, ref state);
        }

        Messenger.Broadcast(Definition.BattleManagerInvokeAction);
    }
    #endregion


    private void MessageAddListner()
    {
        Messenger.AddListener(Definition.SetCharacterState, SetCharacterState);
        Messenger.AddListener(Definition.CheckRemoveCC, CheckRemoveCC);
        Messenger.AddListener<TriggerData.TriggerInfo>(Definition.InsertCC, InsertCC);
        Messenger.AddListener<CharacterBase>(Definition.CheckRemoveCC_Clear, CheckRemoveCC_Clear);
    }
    private void MassageRemoveListner()
    {
        Messenger.RemoveListener(Definition.SetCharacterState, SetCharacterState);
        Messenger.RemoveListener(Definition.CheckRemoveCC, CheckRemoveCC);
        Messenger.RemoveListener<TriggerData.TriggerInfo>(Definition.InsertCC, InsertCC);
        Messenger.RemoveListener<CharacterBase>(Definition.CheckRemoveCC_Clear, CheckRemoveCC_Clear);
    }
}
