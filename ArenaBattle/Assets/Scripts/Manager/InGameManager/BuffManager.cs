using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Generated.CsvData;
using System;

public class BuffManager : MonoBehaviour
{
    private class Buff
    {
        private Skill_BuffData data = null;
        private WeakReference<CharacterBase> caster;
        private WeakReference<CharacterBase> owner;

        private int duration = 0;
        private int count = 0;

        private string durationEffect = "none";
        private string triggerEffect = "none";
        private string icon = "none";
        private string atlas = "none";

        public int GetDuration
        {
            get { return duration; }
        }

        public void InitData(Skill_BuffData _data, CharacterBase _owner, CharacterBase _caster, TriggerData.TriggerInfo info)
        {
            data = _data;

            owner = new WeakReference<CharacterBase>(_owner);
            caster = new WeakReference<CharacterBase>(_caster);

            duration = info.action_duration_turn;
            count = info.action_count;

            durationEffect = data.res_fx_buff;
            triggerEffect = data.res_fx_buff_trigger;
            icon = data.res_icon;
            atlas = data.res_atlas;

            var manager = EffectManager.Instance;
            CharacterBase charbase = null;
            owner.TryGetTarget(out charbase);
            manager.AddEffectList(charbase.gameObject, durationEffect, objectType.character, true);
            manager.AddEffectList(charbase.gameObject, triggerEffect, objectType.character);

            if (atlas != "none" && icon != "none")
                Messenger.Broadcast(Definition.InsertInfoIcon, charbase, atlas, icon, Action_Type.BuffDebuff);
        }

        public bool Use(CharacterBase target = null)
        {
            CharacterBase charbase = null;
            owner.TryGetTarget(out charbase);
            if (target != null && target == charbase)
                return false;

            EffectManager.Instance.PlayDoOnceEffect(triggerEffect, charbase.transform);
            count--;

            if (count <= 0)
            {
                DeleteData();
                return true;
            }

            return false;
        }
        private void DeleteData()
        {
            CharacterBase charbase = null;
            owner.TryGetTarget(out charbase);

            EffectManager.Instance.DestroyEffect(charbase.gameObject, triggerEffect);
            EffectManager.Instance.DestroyEffect(charbase.gameObject, durationEffect);
            Messenger.Broadcast(Definition.RemoveBuffIcon, charbase, icon, Action_Type.BuffDebuff);
        }

        public bool DecreaseDuration()
        {
            duration--;
            if (duration <= 0)
            {
                DeleteData();
                return true;
            }

            return false;
        }
    }

    private Dictionary<Trigger_Timing, List<Buff>> _characterBuffDic = new Dictionary<Trigger_Timing, List<Buff>>();

    private List<Action> _actionState = new List<Action>();
    private int _actionIndex = 0;

    private Skill_BuffTable table = null;

    private void Awake()
    {
        MessageAddListner();
    }
    private void Start()
    {
        InitiailzeData();
    }
    private void OnDestroy()
    {
        MessageRemoveListner();
    }


    private void InitiailzeData()
    {
        table = CSVDataManager.GetTable<Skill_BuffTable>();
    }

    #region Insert Buff
  
    private void InsertBuff(TriggerData.TriggerInfo _info)
    {
        var data = table.GetData(_info.action_id);
        List<CharacterBase> targetList = InGameData.Instance.TriggerTarget;
        CharacterBase applyer = InGameData.Instance.CurrentTurnCharacter;

        List<Buff> buffList = null;
        if(_characterBuffDic.TryGetValue(_info.action_timing, out buffList) == false)
        {
            buffList = new List<Buff>();
            _characterBuffDic.Add(_info.action_timing, buffList);
        }    

        for(int i = 0; i < targetList.Count; i++)
        {
            Buff buff = new Buff();
            buff.InitData(data, targetList[i], applyer, _info);

            buffList.Add(buff);
        }

        Messenger.Broadcast(Definition.InsertComplete);
    }

    #endregion

    #region Use Buff
    private void StartCheckTriggerBuff()
    {
        SetCheckTriggerBuff();
        InvokeAction();
    }
    private void SetCheckTriggerBuff()
    {
        _actionState.Clear();
        _actionIndex = 0;

        _actionState.Add(CharacterCheckTriggerBuff);
        _actionState.Add(EndBuffCheck);
    }

    private void CharacterCheckTriggerBuff()
    {
        List<Buff> data = null;
        Trigger_Timing timing = InGameData.Instance.Trigger_Timing;
        if (_characterBuffDic.TryGetValue(timing, out data) == true)
        {
            for (int i = data.Count - 1; i >= 0; i--)
            {
                Buff inBuff = data[i];
                bool remove = inBuff.Use();

                if (remove == true)
                    data.Remove(inBuff);
            }
        }

        InvokeAction();
    }

    private void CharacterCheckHitBuff(Trigger_Timing timing, CharacterBase target)
    {
        List<Buff> data = null;
        if (_characterBuffDic.TryGetValue(timing, out data) == false)
            return;

        for (int i = data.Count - 1; i >= 0; i--)
        {
            Buff inBuff = data[i];
            bool remove = inBuff.Use(target);

            if (remove == true)
                data.Remove(inBuff);
        }
    }

    private void EndBuffCheck()
    {
        Messenger.Broadcast(Definition.BattleManagerInvokeAction);
        InvokeAction();
    }

    #endregion 

    #region Action
    private void InvokeAction()
    {
        if (_actionState.Count <= _actionIndex)
            return;

        var action = _actionState[_actionIndex];
        _actionIndex++;

        action.Invoke();
    }
    #endregion

    #region CheckRemoveBuff
    private void CheckRemoveBuff()
    {
        foreach(var item in _characterBuffDic)
        {
            List<Buff> buffList = item.Value;
            for(int i = buffList.Count - 1; i >= 0; i--)
            {
                bool remove = buffList[i].DecreaseDuration();

                if (remove)
                    buffList.Remove(buffList[i]);
            }
        }

        Messenger.Broadcast(Definition.BattleManagerInvokeAction);
    }
    #endregion

    private void MessageAddListner()
    {
        Messenger.AddListener<Trigger_Timing, CharacterBase>(Definition.CharacterCheckHitBuff, CharacterCheckHitBuff);
        Messenger.AddListener<TriggerData.TriggerInfo>(Definition.InsertBuff, InsertBuff);
        Messenger.AddListener(Definition.CharacterCheckTriggerBuff, StartCheckTriggerBuff);
        Messenger.AddListener(Definition.CheckRemoveBuff, CheckRemoveBuff);
    }
    private void MessageRemoveListner()
    {
        Messenger.RemoveListener<TriggerData.TriggerInfo>(Definition.InsertBuff, InsertBuff);
        Messenger.RemoveListener(Definition.CharacterCheckTriggerBuff, StartCheckTriggerBuff);
        Messenger.RemoveListener(Definition.CheckRemoveBuff, CheckRemoveBuff);
    }
}
