using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Generated.CsvData
{
    public class MonsterAIData
    {
        public int idx;
        public int ainumber;
        public int skillid;
        public TargetAIDistance targetaidis;
        public TargetHP targethp;
        public float delaytime;

        public void Load(Dictionary<string, string> data)
        {
            CSVTableHelper.SetValue(ref idx, data["idx"]);
            CSVTableHelper.SetValue(ref ainumber, data["ainumber"]);
            CSVTableHelper.SetValue(ref skillid, data["skillid"]);
            CSVTableHelper.SetValue(ref targetaidis, data["targetaidis"]);
            CSVTableHelper.SetValue(ref targethp, data["targethp"]);
            CSVTableHelper.SetValue(ref delaytime, data["delaytime"]);
        }
    }
}

public class MonsterAITable : IDefTable
{
    private Dictionary<int, Generated.CsvData.MonsterAIData> _dicData = new Dictionary<int, Generated.CsvData.MonsterAIData>();
    private Dictionary<int, Generated.CsvData.MonsterAIData> DicData { get { return _dicData; } }

    public override void SetData(List<Dictionary<string, string>> csvTable)
    {
        for (int i = 0; i < csvTable.Count; i++)
        {
            Generated.CsvData.MonsterAIData data = new Generated.CsvData.MonsterAIData();
            data.Load(csvTable[i]);

            if (_dicData.ContainsKey(data.idx))
            {
                Debug.LogErrorFormat("{0} 테이블 {1}인덱스 중복!!", GetType(), data.idx);
                continue;
            }

            _dicData.Add(data.idx, data);
        }
    }

    public Generated.CsvData.MonsterAIData GetData(int index)
    {
        Generated.CsvData.MonsterAIData data;

        if (_dicData.TryGetValue(index, out data) == false)
            Debug.LogError(string.Format("{0} Table {1} Index Find Failed", GetType(), index));

        return data;
    }
}
