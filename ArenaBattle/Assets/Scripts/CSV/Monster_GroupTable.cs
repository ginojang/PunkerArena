using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Generated.CsvData
{
    public class Monster_GroupData
    {
        public int idx;
        public int[] monsterID = new int[9] {0,0,0,0,0,0,0,0,0 };

        public void Load(Dictionary<string, string> data)
        {
            CSVTableHelper.SetValue(ref idx, data["idx"]);
            CSVTableHelper.SetValue(ref monsterID[0], data["pos1_monster_id"]);
            CSVTableHelper.SetValue(ref monsterID[1], data["pos2_monster_id"]);
            CSVTableHelper.SetValue(ref monsterID[2], data["pos3_monster_id"]);
            CSVTableHelper.SetValue(ref monsterID[3], data["pos4_monster_id"]);
            CSVTableHelper.SetValue(ref monsterID[4], data["pos5_monster_id"]);
            CSVTableHelper.SetValue(ref monsterID[5], data["pos6_monster_id"]);
            CSVTableHelper.SetValue(ref monsterID[6], data["pos7_monster_id"]);
            CSVTableHelper.SetValue(ref monsterID[7], data["pos8_monster_id"]);
            CSVTableHelper.SetValue(ref monsterID[8], data["pos9_monster_id"]);
        }
    }
}
public class Monster_GroupTable : IDefTable
{
    private Dictionary<int, Generated.CsvData.Monster_GroupData> _dicData = new Dictionary<int, Generated.CsvData.Monster_GroupData>();

    public override void SetData(List<Dictionary<string, string>> csvTable)
    {
        for (int i = 0; i < csvTable.Count; i++)
        {
            Generated.CsvData.Monster_GroupData data = new Generated.CsvData.Monster_GroupData();
            data.Load(csvTable[i]);

            if (_dicData.ContainsKey(data.idx))
            {
                Debug.LogErrorFormat("{0} 테이블 {1}인덱스 중복!!", GetType(), data.idx);
                continue;
            }

            _dicData.Add(data.idx, data);
        }
    }

    public Generated.CsvData.Monster_GroupData GetData(int index)
    {
        Generated.CsvData.Monster_GroupData data;
        if (_dicData.TryGetValue(index, out data) == false)
            Debug.LogError(string.Format("{0} Table {1} Index Find Failed", GetType(), index));

        return data;
    }
}
