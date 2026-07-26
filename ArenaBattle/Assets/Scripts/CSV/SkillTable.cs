using Generated.CsvData;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Generated.CsvData
{
    public class SkillData
    {
        public int idx;
        public string name_string;
        public string desc_string;
        public Type_Active type_active;
        public Type_Range type_range;
        public Type_Target type_target;
        public int max_level;
        public int level_up_item1_id;
        public int level_up_item1_count;
        public float level_up_item1_count_per_level;
        public int level_up_item2_id;
        public int level_up_item2_count;
        public float level_up_item2_count_per_level;
        public int level_up_cost;
        public float level_up_cost_per_level;
        public int turn_next;
        public int turn_cool;
        public int turn_control;
        public string type_radius;
        public Type_Calculation type_calculation;
        public float dmg_rate;
        public float dmg_rate_per_level;
        public Dmg_Add_Type dmg_add_type;
        public float dmg_add_rate;
        public float dmg_add_rate_per_level;
        public int trigger_id;
        public Type_Action type_action;
        public string res_fx_run;
        public string res_fx_attack;
        public string res_fx_hit;
        public string res_fx_projectile;
        public int res_ani_attack;
        public string res_atlas;
        public string res_icon;
        public float burst_point;
        public int multi_hit_count;
        public float multi_hit_term;
        public int res_ani_move_action;


        public void Load(Dictionary<string, string> data)
        {
            CSVTableHelper.SetValue(ref idx, data["idx"]);
            CSVTableHelper.SetValue(ref name_string, data["name_string"]);
            CSVTableHelper.SetValue(ref desc_string, data["desc_string"]);
            CSVTableHelper.SetValue(ref type_active, data["type_active"]);
            CSVTableHelper.SetValue(ref type_range, data["type_range"]);
            CSVTableHelper.SetValue(ref type_target, data["type_target"]);
            CSVTableHelper.SetValue(ref max_level, data["max_level"]);
            CSVTableHelper.SetValue(ref level_up_item1_id, data["level_up_item1_id"]);
            CSVTableHelper.SetValue(ref level_up_item1_count, data["level_up_item1_count"]);
            CSVTableHelper.SetValue(ref level_up_item1_count_per_level, data["level_up_item1_count_per_level"]);
            CSVTableHelper.SetValue(ref level_up_item2_id, data["level_up_item2_id"]);
            CSVTableHelper.SetValue(ref level_up_item2_count, data["level_up_item2_count"]);
            CSVTableHelper.SetValue(ref level_up_item2_count_per_level, data["level_up_item2_count_per_level"]);
            CSVTableHelper.SetValue(ref level_up_cost, data["level_up_cost"]);
            CSVTableHelper.SetValue(ref level_up_cost_per_level, data["level_up_cost_per_level"]);
            CSVTableHelper.SetValue(ref turn_next, data["turn_next"]);
            CSVTableHelper.SetValue(ref turn_cool, data["turn_cool"]);
            CSVTableHelper.SetValue(ref turn_control, data["turn_control"]);
            CSVTableHelper.SetValue(ref type_radius, data["type_radius"]);
            CSVTableHelper.SetValue(ref type_calculation, data["type_calculation"]);
            CSVTableHelper.SetValue(ref dmg_rate, data["dmg_rate"]);
            CSVTableHelper.SetValue(ref dmg_rate_per_level, data["dmg_rate_per_level"]);
            CSVTableHelper.SetValue(ref dmg_add_type, data["dmg_add_type"]);
            CSVTableHelper.SetValue(ref dmg_add_rate, data["dmg_add_rate"]);
            CSVTableHelper.SetValue(ref dmg_add_rate_per_level, data["dmg_add_rate_per_level"]);
            CSVTableHelper.SetValue(ref trigger_id, data["trigger_id"]);
            CSVTableHelper.SetValue(ref type_action, data["type_action"]);
            CSVTableHelper.SetValue(ref res_fx_run, data["res_fx_run"]);
            CSVTableHelper.SetValue(ref res_fx_attack, data["res_fx_attack"]);
            CSVTableHelper.SetValue(ref res_fx_hit, data["res_fx_hit"]);
            CSVTableHelper.SetValue(ref res_fx_projectile, data["res_fx_projectile"]);
            CSVTableHelper.SetValue(ref res_ani_attack, data["res_ani_attack"]);
            CSVTableHelper.SetValue(ref res_atlas, data["res_atlas"]);
            CSVTableHelper.SetValue(ref res_icon, data["res_icon"]);
            CSVTableHelper.SetValue(ref burst_point, data["burst_point"]);
            CSVTableHelper.SetValue(ref multi_hit_count, data["multi_hit_count"]);
            CSVTableHelper.SetValue(ref multi_hit_term, data["multi_hit_term"]);
            CSVTableHelper.SetValue(ref res_ani_move_action, data["res_ani_move_action"]);
        }
    }
}

public class SkillTable : IDefTable
{
    private Dictionary<int, SkillData> _dicData = new Dictionary<int, SkillData>();
    public Dictionary<int, SkillData> DicData { get { return _dicData; } }

    override public void SetData(List<Dictionary<string, string>> csvTable)
    {
        for (int i = 0; i < csvTable.Count; ++i)
        {
            SkillData data = new SkillData();
            data.Load(csvTable[i]);

            if (_dicData.ContainsKey(data.idx))
            {
                Debug.LogErrorFormat("{0} 테이블 {1}인덱스 중복!!", GetType(), data.idx);
                continue;
            }
            _dicData.Add(data.idx, data);
        }
    }

    public SkillData GetData(int index)
    {
        SkillData data;

        if (_dicData.TryGetValue(index, out data) == false)
            Debug.LogError(string.Format("{0} Table {1} Index Find Failed", GetType(), index));

        return data;
    }

    public static void GetData(ref List<SkillData> result, CharacterTalent talent, ItemType type, string partsName)
    {
        if (type == ItemType.eyes || type == ItemType.mouth || type == ItemType.wing ||
            type == ItemType.body || type == ItemType.back || type == ItemType.pattern)
            return;

        var partsClass = CSVDataManager.SearchClass(partsName);
        var dicData = CSVDataManager.GetTable<PartsTable>().GetDataList(partsClass);

        foreach (var data in dicData)
        {
            string str = "";
            switch (talent)
            {
                case CharacterTalent.Carnivore:
                    str = "ca_";
                    break;
                case CharacterTalent.Herbivore:
                    str = "he_";
                    break;
                case CharacterTalent.Omnivore:
                    str = "om_";
                    break;
            }

            partsInfo parts;
            data.Value.partList.TryGetValue(type, out parts);
            str += parts.fileName;
            if (str != partsName)
                continue;

            SkillData addData = null;
            if (parts.skill01 > 0)
            {
                addData = CSVDataManager.GetTable<SkillTable>().GetData(parts.skill01);
                result.Add(addData);
            }

            if (parts.skill02 > 0)
            {
                addData = CSVDataManager.GetTable<SkillTable>().GetData(parts.skill02);
                result.Add(addData);
            }
            break;
        }
    }

    public void GetDataList(ref List<SkillData> result, int monsterIndex, CharacterTalent talent, Dictionary<ItemType, string> itemlist)
    {
        if (monsterIndex > 0)
        {
            var monsterTable = CSVDataManager.GetTable<MonsterTable>().GetData(monsterIndex);

            SkillData addData = CSVDataManager.GetTable<SkillTable>().GetData(monsterTable.skill_id_active1);
            if (addData != null)
                result.Add(addData);

            addData = CSVDataManager.GetTable<SkillTable>().GetData(monsterTable.skill_id_active2);
            if (addData != null)
                result.Add(addData);
        }
        else
        {
            var basicAttack = CSVDataManager.GetTable<SkillTable>().GetData(1);
            result.Add(basicAttack);

            foreach (var element in itemlist)
            {
                GetData(ref result, talent, element.Key, element.Value);
            }
        }
    }

    public void GetDataIndexList(ref List<Generated.CsvData.SkillData> result, int monsterIndex, CharacterTalent talent, Dictionary<ItemType, string> itemlist)
    {
        List<SkillData> findData = new List<SkillData>();
        CSVDataManager.GetTable<SkillTable>().GetDataList(ref findData, monsterIndex, talent, itemlist);

        result.AddRange(findData);
    }
}
