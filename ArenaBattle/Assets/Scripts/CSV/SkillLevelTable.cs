using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Generated.CsvData
{
    public class SkillLevelData
    {
        public int idx;
        public int skilllv;
        public int skill_value;
        public float skill_dmgvalue;

        public void Load(Dictionary<string, string> data)
        {
            CSVTableHelper.SetValue(ref idx, data["idx"]);
            CSVTableHelper.SetValue(ref skilllv, data["skilllv"]);
            CSVTableHelper.SetValue(ref skill_value, data["skill_value"]);
            CSVTableHelper.SetValue(ref skill_dmgvalue, data["skill_dmgvalue"]);
        }
    }
}

public class SkillLevelTable : IDefTable
{
    private Dictionary<int, Generated.CsvData.SkillLevelData> _dicData = new Dictionary<int, Generated.CsvData.SkillLevelData>();
    public Dictionary<int, Generated.CsvData.SkillLevelData> DicData { get { return _dicData; } }

    public override void SetData(List<Dictionary<string, string>> csvTable)
    {
        for (int i = 0; i < csvTable.Count; i++)
        {
            Generated.CsvData.SkillLevelData data = new Generated.CsvData.SkillLevelData();
            data.Load(csvTable[i]);

            if (_dicData.ContainsKey(data.idx))
            {
                Debug.LogErrorFormat("{0} 테이블 {1}인덱스 중복!!", GetType(), data.idx);
                continue;
            }

            _dicData.Add(data.idx, data);
        }
    }

    public Generated.CsvData.SkillLevelData GetData(int index)
    {
        Generated.CsvData.SkillLevelData data;

        if (_dicData.TryGetValue(index, out data) == false)
            Debug.LogError(string.Format("{0} Table {1} Index Find Failed", GetType(), index));

        return data;
    }

}
