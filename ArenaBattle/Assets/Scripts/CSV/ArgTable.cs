using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Generated.CsvData
{
    public class ArgData
    {
        public int idx;
        public string key;
        public ValueType type;
        public float value;

        public void Load(Dictionary<string, string> data)
        {
            CSVTableHelper.SetValue(ref idx, data["idx"]);
            CSVTableHelper.SetValue(ref key, data["key"]);
            CSVTableHelper.SetValue(ref type, data["type"]);
            CSVTableHelper.SetValue(ref value, data["value"]);
        }
    }
}

public class ArgTable : IDefTable
{
    private Dictionary<int, Generated.CsvData.ArgData> _dicData = new Dictionary<int, Generated.CsvData.ArgData>();

    public override void SetData(List<Dictionary<string, string>> csvTable)
    {
        for(int i = 0; i < csvTable.Count; i++)
        {
            Generated.CsvData.ArgData data = new Generated.CsvData.ArgData();
            data.Load(csvTable[i]);

            if(_dicData.ContainsKey(data.idx))
            {
                Debug.LogErrorFormat("{0} 테이블 {1}인덱스 중복!!", GetType(), data.idx);
                continue;
            }

            _dicData.Add(data.idx, data);
        }
    }

    public Generated.CsvData.ArgData GetData(int index)
    {
        Generated.CsvData.ArgData data;
        if (_dicData.TryGetValue(index, out data) == false)
            Debug.LogError(string.Format("{0} Table {1} Index Find Failed", GetType(), index));

        return data;
    }
}
