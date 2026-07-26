using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Generated.CsvData
{
    public class StageData
    {
        public int idx;
        public string name_string;
        public string desc_string;
        public int type;
        public int chapter;
        public int difficulty;
        public int play_cost;
        public int time_fix;
        public Attributes time_start;
        public int wave;
        public int[] monsterGroupID = new int[5] { 0,0,0,0,0};
        public int reward_exp_account;
        public int reward_exp_dino;
        public int reward_fat_count;
        public int reward_item1_id;
        public int reward_item1_count;
        public int reward_item2_id;
        public int reward_item2_count;
        public int reward_item3_id;
        public int reward_item3_count;
        public string res_prefab;

        public void Load(Dictionary<string, string> data)
        {
            CSVTableHelper.SetValue(ref idx, data["idx"]);
            CSVTableHelper.SetValue(ref name_string, data["name_string"]);
            CSVTableHelper.SetValue(ref desc_string, data["desc_string"]);
            CSVTableHelper.SetValue(ref type, data["type"]);
            CSVTableHelper.SetValue(ref chapter, data["chapter"]);
            CSVTableHelper.SetValue(ref difficulty, data["difficulty"]);
            CSVTableHelper.SetValue(ref play_cost, data["play_cost"]);
            CSVTableHelper.SetValue(ref time_fix, data["time_fix"]);
            CSVTableHelper.SetValue(ref time_start, data["time_start"]);
            CSVTableHelper.SetValue(ref wave, data["wave"]);
            CSVTableHelper.SetValue(ref monsterGroupID[0], data["wave1_monster_group_id"]);
            CSVTableHelper.SetValue(ref monsterGroupID[1], data["wave2_monster_group_id"]);
            CSVTableHelper.SetValue(ref monsterGroupID[2], data["wave3_monster_group_id"]);
            CSVTableHelper.SetValue(ref monsterGroupID[3], data["wave4_monster_group_id"]);
            CSVTableHelper.SetValue(ref monsterGroupID[4], data["wave5_monster_group_id"]);
            CSVTableHelper.SetValue(ref reward_exp_account, data["reward_exp_account"]);
            CSVTableHelper.SetValue(ref reward_exp_dino, data["reward_exp_dino"]);
            CSVTableHelper.SetValue(ref reward_fat_count, data["reward_fat_count"]);
            CSVTableHelper.SetValue(ref reward_item1_id, data["reward_item1_id"]);
            CSVTableHelper.SetValue(ref reward_item1_count, data["reward_item1_count"]);
            CSVTableHelper.SetValue(ref reward_item2_id, data["reward_item2_id"]);
            CSVTableHelper.SetValue(ref reward_item2_count, data["reward_item2_count"]);
            CSVTableHelper.SetValue(ref res_prefab, data["res_prefab"]);
        }
    }
}

public class StageTable : IDefTable
{
    private Dictionary<int, Generated.CsvData.StageData> _dicData = new Dictionary<int, Generated.CsvData.StageData>();

#if UNITY_EDITOR
    public Dictionary<int, Generated.CsvData.StageData> DicData
    {
        get { return _dicData; }
    }
#endif

    public override void SetData(List<Dictionary<string, string>> csvTable)
    {
        for(int i = 0; i < csvTable.Count; i++)
        {
            Generated.CsvData.StageData data = new Generated.CsvData.StageData();
            data.Load(csvTable[i]);

            if(_dicData.ContainsKey(data.idx))
            {
                Debug.LogErrorFormat("{0} ���̺� {1}�ε��� �ߺ�!!", GetType(), data.idx);
                continue;
            }

            _dicData.Add(data.idx, data);
        }
    }

    public Generated.CsvData.StageData GetData(int index)
    {
        Generated.CsvData.StageData data = null;
        if (_dicData.TryGetValue(index, out data) == false)
            Debug.LogError(string.Format("{0} Table {1} Index Find Failed", GetType(), index));

        return data;
    }

}
