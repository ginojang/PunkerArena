using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Generated.CsvData
{
    public class Skill_BuffData
    {
        public int idx;
        public string name_string;
        public Buff_Type type;
        public Type_Target type_target;
        public Type_Calculation type_calculation;
        public int apply_priority_level;
        public float dmg_rate;
        public float dmg_rate_per_level;
        public Dmg_Add_Type dmg_add_type;
        public float dmg_add_rate;
        public float dmg_add_rate_per_level;
        public Buff_Stat_Type stat1_type;
        public int stat1_value;
        public int stat1_value_per_level;
        public Buff_Stat_Type stat2_type;
        public int stat2_value;
        public int stat2_value_per_level;
        public Buff_Special_Status_Type special_status_type;
        public float special_status_value;
        public float special_status_value_per_level;
        public int duration_turn;
        public float duration_turn_per_level;
        public string res_fx_buff;
        public string res_fx_buff_trigger;
        public string res_atlas;
        public string res_icon;

        public void Load(Dictionary<string, string> data)
        {
            CSVTableHelper.SetValue(ref idx, data["idx"]);
            CSVTableHelper.SetValue(ref name_string, data["name_string"]);
            CSVTableHelper.SetValue(ref type, data["type"]);
            CSVTableHelper.SetValue(ref type_target, data["type_target"]);
            CSVTableHelper.SetValue(ref type_calculation, data["type_calculation"]);
            CSVTableHelper.SetValue(ref apply_priority_level, data["apply_priority_level"]);
            CSVTableHelper.SetValue(ref dmg_rate, data["dmg_rate"]);
            CSVTableHelper.SetValue(ref dmg_rate_per_level, data["dmg_rate_per_level"]);
            CSVTableHelper.SetValue(ref dmg_add_type, data["dmg_add_type"]);
            CSVTableHelper.SetValue(ref dmg_add_rate, data["dmg_add_rate"]);
            CSVTableHelper.SetValue(ref dmg_add_rate_per_level, data["dmg_add_rate_per_level"]);
            CSVTableHelper.SetValue(ref stat1_type, data["stat1_type"]);
            CSVTableHelper.SetValue(ref stat1_value, data["stat1_value"]);
            CSVTableHelper.SetValue(ref stat1_value_per_level, data["stat1_value_per_level"]);
            CSVTableHelper.SetValue(ref stat2_type, data["stat2_type"]);
            CSVTableHelper.SetValue(ref stat2_value, data["stat2_value"]);
            CSVTableHelper.SetValue(ref stat2_value_per_level, data["stat2_value_per_level"]);
            CSVTableHelper.SetValue(ref special_status_type, data["special_status_type"]);
            CSVTableHelper.SetValue(ref special_status_value, data["special_status_value"]);
            CSVTableHelper.SetValue(ref special_status_value_per_level, data["special_status_value_per_level"]);
            CSVTableHelper.SetValue(ref res_fx_buff, data["res_fx_buff"]);
            CSVTableHelper.SetValue(ref res_fx_buff_trigger, data["res_fx_buff_trigger"]);
            CSVTableHelper.SetValue(ref res_atlas, data["res_atlas"]);
            CSVTableHelper.SetValue(ref res_icon, data["res_icon"]);
        }
    }
}

public class Skill_BuffTable : IDefTable
{
    private Dictionary<int, Generated.CsvData.Skill_BuffData> _dicData = new Dictionary<int, Generated.CsvData.Skill_BuffData>();
    public Dictionary<int , Generated.CsvData.Skill_BuffData> DicData { get { return _dicData; } }
    public override void SetData(List<Dictionary<string, string>> csvTable)
    {
        for(int i = 0; i < csvTable.Count; i++)
        {
            Generated.CsvData.Skill_BuffData data = new Generated.CsvData.Skill_BuffData();
            data.Load(csvTable[i]);

            if(_dicData.ContainsKey(data.idx))
            {
                Debug.LogErrorFormat("{0} 테이블 {1}인덱스 중복!!", GetType(), data.idx);
                continue;
            }

            _dicData.Add(data.idx, data);
        }
    }

    public Generated.CsvData.Skill_BuffData GetData(int index)
    {
        Generated.CsvData.Skill_BuffData data;
        if(_dicData.TryGetValue(index, out data) == false)
            Debug.LogError(string.Format("{0} Table {1} Index Find Failed", GetType(), index));

        return data;
    }
}
