using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Generated.CsvData
{
    public class MonsterData
    {
        public int idx;
        public string name_string;
        public string desc_string;
        public Monster_Type type;
        public CharacterClass monster_class;
        public Grade grade;
        public Attributes attribute;
        public CharacterTalent monster_talent;
        public Role role;
        public int tag_id;
        public Monster_AI_Type type_ai;
        public MonsterGrade boss_type;
        public int level;
        public float str;
        public float vit;
        public float agi;
        public float dex;
        public float luk;
        public float atk;
        public float def;
        public float def_pen;
        public float hp;
        public float hit;
        public float avd;
        public float cri;
        public float cri_dmg;
        public float res;
        public float spd;
        public int skill_id_basic_attack;
        public int skill_id_active1;
        public int skill_id_active1_level;
        public int skill_id_active2;
        public int skill_id_active2_level;
        public int skill_id_passive1;
        public int skill_id_passive1_level;
        public int skill_id_passive2;
        public int skill_id_passive2_level;
        public int monster_part_id;
        public string res_prefab;
        public string res_atlas;
        public string res_icon;
        public void Load(Dictionary<string, string> data)
        {
            CSVTableHelper.SetValue(ref idx, data["idx"]);
            CSVTableHelper.SetValue(ref name_string, data["name_string"]);
            CSVTableHelper.SetValue(ref desc_string, data["desc_string"]);
            CSVTableHelper.SetValue(ref type, data["type"]);
            CSVTableHelper.SetValue(ref monster_class, data["monster_class"]);
            CSVTableHelper.SetValue(ref grade, data["grade"]);
            CSVTableHelper.SetValue(ref attribute, data["attribute"]);
            CSVTableHelper.SetValue(ref monster_talent, data["talent"]);
            CSVTableHelper.SetValue(ref role, data["role"]);
            CSVTableHelper.SetValue(ref tag_id, data["tag_id"]);
            CSVTableHelper.SetValue(ref type_ai, data["type_ai"]);
            CSVTableHelper.SetValue(ref boss_type, data["boss_type"]);
            CSVTableHelper.SetValue(ref level, data["level"]);
            CSVTableHelper.SetValue(ref str, data["str"]);
            CSVTableHelper.SetValue(ref vit, data["vit"]);
            CSVTableHelper.SetValue(ref agi, data["agi"]);
            CSVTableHelper.SetValue(ref dex, data["dex"]);
            CSVTableHelper.SetValue(ref luk, data["luk"]);
            CSVTableHelper.SetValue(ref atk, data["atk"]);
            CSVTableHelper.SetValue(ref def, data["def"]);
            CSVTableHelper.SetValue(ref def_pen, data["def_pen"]);
            CSVTableHelper.SetValue(ref hp, data["hp"]);
            CSVTableHelper.SetValue(ref hit, data["hit"]);
            CSVTableHelper.SetValue(ref avd, data["avd"]);
            CSVTableHelper.SetValue(ref cri, data["cri"]);
            CSVTableHelper.SetValue(ref cri_dmg, data["cri_dmg"]);
            CSVTableHelper.SetValue(ref res, data["res"]);
            CSVTableHelper.SetValue(ref spd, data["spd"]);
            CSVTableHelper.SetValue(ref skill_id_basic_attack, data["skill_id_basic_attack"]);
            CSVTableHelper.SetValue(ref skill_id_active1, data["skill_id_active1"]);
            CSVTableHelper.SetValue(ref skill_id_active1_level, data["skill_id_active1_level"]);
            CSVTableHelper.SetValue(ref skill_id_active2, data["skill_id_active2"]);
            CSVTableHelper.SetValue(ref skill_id_active2_level, data["skill_id_active2_level"]);
            CSVTableHelper.SetValue(ref skill_id_passive1, data["skill_id_passive1"]);
            CSVTableHelper.SetValue(ref skill_id_passive1_level, data["skill_id_passive1_level"]);
            CSVTableHelper.SetValue(ref skill_id_passive2, data["skill_id_passive2"]);
            CSVTableHelper.SetValue(ref skill_id_passive2_level, data["skill_id_passive2_level"]);
            CSVTableHelper.SetValue(ref monster_part_id, data["monster_part_id"]);
            CSVTableHelper.SetValue(ref res_prefab, data["res_prefab"]);
            CSVTableHelper.SetValue(ref res_atlas, data["res_atlas"]);
            CSVTableHelper.SetValue(ref res_icon, data["res_icon"]);
        }
    }
}

public class MonsterTable : IDefTable
{
    private Dictionary<int, Generated.CsvData.MonsterData> _dicData = new Dictionary<int, Generated.CsvData.MonsterData>();
    public Dictionary<int, Generated.CsvData.MonsterData> DicData { get { return _dicData; } }

    public override void SetData(List<Dictionary<string, string>> csvTable)
    {
        for(int i = 0; i < csvTable.Count; i++)
        {
            Generated.CsvData.MonsterData data = new Generated.CsvData.MonsterData();
            data.Load(csvTable[i]);

            if(_dicData.ContainsKey(data.idx))
            {
                Debug.LogErrorFormat("{0} ���̺� {1}�ε��� �ߺ�!!", GetType(), data.idx);
                continue;
            }

            _dicData.Add(data.idx, data);
        }
    }

    public Generated.CsvData.MonsterData GetData(int index)
    {
        Generated.CsvData.MonsterData data;
       if (_dicData.TryGetValue(index, out data) == false)
            Debug.LogError(string.Format("{0} Table {1} Index Find Failed", GetType(), index));

        return data;
    }
}
