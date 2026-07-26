using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Generated.CsvData
{
    public class dinoData
    {
        public int index;
        public CharacterClass charClass; // 과일 클래스
        public int charName; // 과일 이름 테이블 인덱스
        public CharacterAttribute charAttribute;
        public CharacterRanking charGrade;
        public int charRole;
        public int charUltimateSkill; // 필살기 스킬 인덱스
        public int charBasicStr;
        public int charBasicVit;
        public int charBasicAgi;
        public int charBasicDex;
        public int charBasicLuck;
        public int basic_hp;
        public int basic_sp;
        public float basic_meleedamage;
        public float basic_rangedamage;
        public int basic_defense;
        public float basic_attackrating;
        public float basic_evade;
        public float basic_critical;
        public float basic_criticaldamage;
        public float attackspeed;



        public void Load(Dictionary<string, string> data)
        {
            CSVTableHelper.SetValue(ref index, data["idx"]);
            CSVTableHelper.SetValue(ref charClass, data["dino_type"]);
            CSVTableHelper.SetValue(ref charName, data["name_string"]);
            CSVTableHelper.SetValue(ref charAttribute, data["attribute"]);
            CSVTableHelper.SetValue(ref charGrade, data["grade"]);
            CSVTableHelper.SetValue(ref charRole, data["role"]);
            CSVTableHelper.SetValue(ref charUltimateSkill, data["ultimate_skill"]);
            CSVTableHelper.SetValue(ref charBasicStr, data["basic_str"]);
            CSVTableHelper.SetValue(ref charBasicVit, data["basic_vit"]);
            CSVTableHelper.SetValue(ref charBasicAgi, data["basic_agi"]);
            CSVTableHelper.SetValue(ref charBasicDex, data["basic_dex"]);
            CSVTableHelper.SetValue(ref charBasicLuck, data["basic_luk"]);
            CSVTableHelper.SetValue(ref basic_hp, data["basic_hp"]);
            CSVTableHelper.SetValue(ref basic_sp, data["basic_sp"]);
            CSVTableHelper.SetValue(ref basic_meleedamage, data["basic_meleedamage"]);
            CSVTableHelper.SetValue(ref basic_rangedamage, data["basic_rangedamage"]);
            CSVTableHelper.SetValue(ref basic_defense, data["basic_defense"]);
            CSVTableHelper.SetValue(ref basic_attackrating, data["basic_attackrating"]);
            CSVTableHelper.SetValue(ref basic_evade, data["basic_evade"]);
            CSVTableHelper.SetValue(ref basic_critical, data["basic_critical"]);
            CSVTableHelper.SetValue(ref basic_criticaldamage, data["basic_criticaldamage"]);
            CSVTableHelper.SetValue(ref attackspeed, data["attackspeed"]);
        }
    }
}

public class DinoTable : IDefTable
{
    private Dictionary<int, Generated.CsvData.dinoData> _dicData = new Dictionary<int, Generated.CsvData.dinoData>();
    public Dictionary<int, Generated.CsvData.dinoData> DicData { get { return _dicData; } }

    override public void SetData(List<Dictionary<string, string>> csvTable)
    {
        for (int i = 0; i < csvTable.Count; ++i)
        {
            Generated.CsvData.dinoData data = new Generated.CsvData.dinoData();
            data.Load(csvTable[i]);

            if (_dicData.ContainsKey(data.index))
            {
                Debug.LogErrorFormat("{0} 테이블 {1}인덱스 중복!!", GetType(), data.index);
                continue;
            }
            _dicData.Add(data.index, data);
        }
    }

    public Generated.CsvData.dinoData GetData(int index)
    {
        Generated.CsvData.dinoData data;

        if (_dicData.TryGetValue(index, out data) == false)
            Debug.LogError(string.Format("{0} Table {1} Index Find Failed", GetType(), index));

        return data;
    }
}
