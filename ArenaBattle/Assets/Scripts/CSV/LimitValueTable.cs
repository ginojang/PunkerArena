using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Generated.CsvData
{
    public class LimitValueData
    {
        public int idx;
        public float maxrange;
        public float maxcritical;
        public float maxcriticaldmg;
        public int minrandom;
        public int maxrandom;

        public void Load(Dictionary<string, string> data)
        {
            CSVTableHelper.SetValue(ref idx, data["idx"]);
            CSVTableHelper.SetValue(ref maxrange, data["maxrange"]);
            CSVTableHelper.SetValue(ref maxcritical, data["maxcritical"]);
            CSVTableHelper.SetValue(ref maxcriticaldmg, data["maxcriticaldmg"]);
            CSVTableHelper.SetValue(ref minrandom, data["minrandom"]);
            CSVTableHelper.SetValue(ref maxrandom, data["maxrandom"]);
        }
    }
}

public class LimitValueTable : IDefTable
{
    private Dictionary<int, Generated.CsvData.LimitValueData> _dicData = new Dictionary<int, Generated.CsvData.LimitValueData>();
    public Dictionary<int, Generated.CsvData.LimitValueData> DicData { get { return _dicData; } }

    public override void SetData(List<Dictionary<string, string>> csvTable)
    {
        for(int i = 0; i < csvTable.Count; i++)
        {
            Generated.CsvData.LimitValueData data = new Generated.CsvData.LimitValueData();
            data.Load(csvTable[i]);

            if (_dicData.ContainsKey(data.idx))
            {
                Debug.LogErrorFormat("{0} 테이블 {1}인덱스 중복!!", GetType(), data.idx);
                continue;
            }
            _dicData.Add(data.idx, data);
        }
    }
    public Generated.CsvData.LimitValueData GetData(int index)
    {
        Generated.CsvData.LimitValueData data;

        if (_dicData.TryGetValue(index, out data) == false)
            Debug.LogError(string.Format("{0} Table {1} Index Find Failed", GetType(), index));

        return data;
    }

}
