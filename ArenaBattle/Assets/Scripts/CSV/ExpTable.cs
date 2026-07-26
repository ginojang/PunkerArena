using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Generated.CsvData
{
    public class expData
    {
        public int level;
        public int exp;

        public void Load(Dictionary<string, string> data)
        {
            CSVTableHelper.SetValue(ref level, data["lv"]);
            CSVTableHelper.SetValue(ref exp, data["exp"]);
        }
    }
}

public class ExpTable : IDefTable
{
    private Dictionary<int, Generated.CsvData.expData> _dicData = new Dictionary<int, Generated.CsvData.expData>();
    public Dictionary<int, Generated.CsvData.expData> DicData { get { return _dicData; } }

    override public void SetData(List<Dictionary<string, string>> csvTable)
    {
        for (int i = 0; i < csvTable.Count; ++i)
        {
            Generated.CsvData.expData data = new Generated.CsvData.expData();
            data.Load(csvTable[i]);

            if (_dicData.ContainsKey(data.level))
            {
                Debug.LogErrorFormat("{0} 테이블 {1}인덱스 중복!!", GetType(), data.level);
                continue;
            }
            _dicData.Add(data.level, data);
        }
    }

    public Generated.CsvData.expData GetData(int lev)
    {
        Generated.CsvData.expData data;

        if (_dicData.TryGetValue(lev, out data) == false)
            Debug.LogError(string.Format("{0} Table {1} Index Find Failed", GetType(), lev));

        return data;
    }
}
