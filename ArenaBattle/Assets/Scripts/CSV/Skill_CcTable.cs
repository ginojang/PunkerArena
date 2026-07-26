using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Generated.CsvData
{
    public class Skill_CcData
    {
        public int idx;
        public string name_string;
        public Cc_Type type;
        public Type_Action_Lock type_action_lock;
        public Type_Target_Lock type_target_lock;
        public Type_CC_Clear type_cc_clear;
        public Type_Attack_Lock type_attack_lock;

        public Type_Target type_target;
        public Type_Target_Auto_AI type_target_auto_ai;
        public int duration_turn;
        public float duration_turn_per_level;
        public Cc_Group group;
        public int group_priority_level;
        public string res_fx_cc;
        public int res_ani_cc;
        public string res_atlas;
        public string res_icon;
        public void Load(Dictionary<string, string> data)
        {
            CSVTableHelper.SetValue(ref idx, data["idx"]);
            CSVTableHelper.SetValue(ref name_string, data["name_string"]);
            CSVTableHelper.SetValue(ref type, data["type"]);
            CSVTableHelper.SetValue(ref type_action_lock, data["type_action_lock"]);
            CSVTableHelper.SetValue(ref type_target_lock, data["type_target_lock"]);
            CSVTableHelper.SetValue(ref type_cc_clear, data["type_cc_clear"]);
            CSVTableHelper.SetValue(ref type_attack_lock, data["type_attack_lock"]);
            CSVTableHelper.SetValue(ref type_target, data["type_target"]);
            CSVTableHelper.SetValue(ref type_target_auto_ai, data["type_target_auto_ai"]);
            CSVTableHelper.SetValue(ref duration_turn, data["duration_turn"]);
            CSVTableHelper.SetValue(ref duration_turn_per_level, data["duration_turn_per_level"]);
            CSVTableHelper.SetValue(ref group, data["group"]);
            CSVTableHelper.SetValue(ref group_priority_level, data["group_priority_level"]);
            CSVTableHelper.SetValue(ref res_fx_cc, data["res_fx_cc"]);
            CSVTableHelper.SetValue(ref res_ani_cc, data["res_ani_cc"]);
            CSVTableHelper.SetValue(ref res_atlas, data["res_atlas"]);
            CSVTableHelper.SetValue(ref res_icon, data["res_icon"]);
        }
    }
}

public class Skill_CcTable : IDefTable
{
    private Dictionary<int, Generated.CsvData.Skill_CcData> _dicData = new Dictionary<int, Generated.CsvData.Skill_CcData>();
    public Dictionary<int , Generated.CsvData.Skill_CcData> DicData { get { return _dicData; } }

    public override void SetData(List<Dictionary<string, string>> csvTable)
    {
        for(int i = 0; i < csvTable.Count; i++)
        {
            Generated.CsvData.Skill_CcData data = new Generated.CsvData.Skill_CcData();
            data.Load(csvTable[i]);

            if(_dicData.ContainsKey(data.idx))
            {
                Debug.LogErrorFormat("{0} 테이블 {1}인덱스 중복!!", GetType(), data.idx);
                continue;
            }

            _dicData.Add(data.idx, data);
        }
    }
    public Generated.CsvData.Skill_CcData GetData(int index)
    {
        Generated.CsvData.Skill_CcData data;
        if(_dicData.TryGetValue(index, out data) == false)
            Debug.LogError(string.Format("{0} Table {1} Index Find Failed", GetType(), index));

        return data;
    }
}
