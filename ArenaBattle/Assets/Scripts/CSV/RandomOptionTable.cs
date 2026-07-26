using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Generated.CsvData
{
    public class randomOptionData
    {
        public int index;
        public int strMin;
        public int strMax;
        public int vitMin;
        public int vitMax;
        public int agiMin;
        public int agiMax;
        public int dexMin;
        public int dexMax;
        public int lukMin;
        public int lukMax;


        public void Load(Dictionary<string, string> data)
        {
            CSVTableHelper.SetValue(ref index, data["idx"]);
            CSVTableHelper.SetValue(ref strMin, data["str_min"]);
            CSVTableHelper.SetValue(ref strMax, data["str_max"]);
            CSVTableHelper.SetValue(ref vitMin, data["vit_min"]);
            CSVTableHelper.SetValue(ref vitMax, data["vit_max"]);
            CSVTableHelper.SetValue(ref agiMin, data["agi_min"]);
            CSVTableHelper.SetValue(ref agiMax, data["agi_max"]);
            CSVTableHelper.SetValue(ref dexMin, data["dex_min"]);
            CSVTableHelper.SetValue(ref dexMax, data["dex_max"]);
            CSVTableHelper.SetValue(ref lukMin, data["luk_min"]);
            CSVTableHelper.SetValue(ref lukMax, data["luk_max"]);
        }
    }
}

public class randomOptionTable : IDefTable
{
    private Dictionary<int, Generated.CsvData.randomOptionData> _dicData = new Dictionary<int, Generated.CsvData.randomOptionData>();
    public Dictionary<int, Generated.CsvData.randomOptionData> DicData { get { return _dicData; } }

    override public void SetData(List<Dictionary<string, string>> csvTable)
    {
        for (int i = 0; i < csvTable.Count; ++i)
        {
            Generated.CsvData.randomOptionData data = new Generated.CsvData.randomOptionData();
            data.Load(csvTable[i]);

            if (_dicData.ContainsKey(data.index))
            {
                Debug.LogErrorFormat("{0} 테이블 {1}인덱스 중복!!", GetType(), data.index);
                continue;
            }
            _dicData.Add(data.index, data);
        }
    }

    public Generated.CsvData.randomOptionData GetData(int index)
    {
        Generated.CsvData.randomOptionData data;

        if (_dicData.TryGetValue(index, out data) == false)
            Debug.LogError(string.Format("{0} Table {1} Index Find Failed", GetType(), index));

        return data;
    }
}
