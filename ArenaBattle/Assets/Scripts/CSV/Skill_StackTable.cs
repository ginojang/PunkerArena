using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Generated.CsvData
{
    public class Skill_StackData
    {
        public int idx;
        public string name_string;
        public Stack_Type type;
        public Type_Calculation type_calculation;
        public float dmg_rate;
        public float dmg_rate_per_level;
        public Dmg_Add_Type dmg_add_type;
        public float dmg_add_rate;
        public float dmg_add_rate_per_level;
        public int trigger_id;
        public string res_fx_activation_stack;
        public string res_atlas;
        public string res_icon;

        public void Load(Dictionary<string, string> data)
        {
            CSVTableHelper.SetValue(ref idx, data["idx"]);
            CSVTableHelper.SetValue(ref name_string, data["name_string"]);
            CSVTableHelper.SetValue(ref type, data["type"]);
            CSVTableHelper.SetValue(ref type_calculation, data["type_calculation"]);
            CSVTableHelper.SetValue(ref dmg_rate, data["dmg_rate"]);
            CSVTableHelper.SetValue(ref dmg_rate_per_level, data["dmg_rate_per_level"]);
            CSVTableHelper.SetValue(ref dmg_add_type, data["dmg_add_type"]);
            CSVTableHelper.SetValue(ref dmg_add_rate, data["dmg_add_rate"]);
            CSVTableHelper.SetValue(ref dmg_add_rate_per_level, data["dmg_add_rate_per_level"]);
            CSVTableHelper.SetValue(ref trigger_id, data["trigger_id"]);
            CSVTableHelper.SetValue(ref res_fx_activation_stack, data["res_fx_activation_stack"]);
            CSVTableHelper.SetValue(ref res_atlas, data["res_atlas"]);
            CSVTableHelper.SetValue(ref res_icon, data["res_icon"]);
        }
    }
}

public class Skill_StackTable : IDefTable
{
    private Dictionary<int, Generated.CsvData.Skill_StackData> _dicData = new Dictionary<int, Generated.CsvData.Skill_StackData>();
    public Dictionary <int, Generated.CsvData.Skill_StackData> DicData { get { return _dicData; } }

    public override void SetData(List<Dictionary<string, string>> csvTable)
    {
        for(int i = 0; i < csvTable.Count; i++)
        {
            Generated.CsvData.Skill_StackData data = new Generated.CsvData.Skill_StackData();
            data.Load(csvTable[i]);

            if (_dicData.ContainsKey(data.idx))
            {
                Debug.LogErrorFormat("{0} 테이블 {1}인덱스 중복!!", GetType(), data.idx);
                continue;
            }
            _dicData.Add(data.idx, data);
        }
    }

    public Generated.CsvData.Skill_StackData GetData(int Index)
    {
        Generated.CsvData.Skill_StackData data;
        if(_dicData.TryGetValue(Index, out data) == false)
            Debug.LogError(string.Format("{0} Table {1} Index Find Failed", GetType(), Index));

        return data;
    }
}
