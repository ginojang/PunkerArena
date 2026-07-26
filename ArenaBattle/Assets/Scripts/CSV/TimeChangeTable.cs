using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Generated.CsvData
{
    public class timeData
    {
        public int idx;
        public float dawn;
        public int dawn_buff;
        public float day;
        public int day_buff;
        public float night;
        public int night_buff;

        public void Load(Dictionary<string, string> data)
        {
            CSVTableHelper.SetValue(ref idx, data["idx"]);
            CSVTableHelper.SetValue(ref dawn, data["dawn"]);
            CSVTableHelper.SetValue(ref dawn_buff, data["dawn_buff"]);
            CSVTableHelper.SetValue(ref day, data["day"]);
            CSVTableHelper.SetValue(ref day_buff, data["day_buff"]);
            CSVTableHelper.SetValue(ref night, data["night"]);
            CSVTableHelper.SetValue(ref night_buff, data["night_buff"]);
        }
    }
}

public class TimeChangeTable : IDefTable
{
    private Dictionary<int, Generated.CsvData.timeData> _dicData = new Dictionary<int, Generated.CsvData.timeData>();
    public Dictionary<int, Generated.CsvData.timeData> DicData { get { return _dicData; } }

    public override void SetData(List<Dictionary<string, string>> csvTable)
    {
        for (int i = 0; i < csvTable.Count; i++)
        {
            Generated.CsvData.timeData data = new Generated.CsvData.timeData();
            data.Load(csvTable[i]);

            if (_dicData.ContainsKey(data.idx))
            {
                Debug.LogErrorFormat("{0} 테이블 {1}인덱스 중복!!", GetType(), data.idx);
                continue;
            }

            _dicData.Add(data.idx, data);
        }
    }

    public Generated.CsvData.timeData GetData(int index)
    {
        Generated.CsvData.timeData data;

        if (_dicData.TryGetValue(index, out data) == false)
            Debug.LogError(string.Format("{0} Table {1} Index Find Failed", GetType(), index));

        return data;
    }
}
