using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Generated.CsvData
{
    public class pureDnaBonusData
    {
        public int index;
        public ItemType partType;
        public int str;
        public int vit;
        public int agi;
        public int dex;
        public int luk;

        public void Load(Dictionary<string, string> data)
        {
            CSVTableHelper.SetValue(ref index, data["idx"]);
            CSVTableHelper.SetValue(ref partType, data["type"]);
            CSVTableHelper.SetValue(ref str, data["str"]);
            CSVTableHelper.SetValue(ref vit, data["vit"]);
            CSVTableHelper.SetValue(ref agi, data["agi"]);
            CSVTableHelper.SetValue(ref dex, data["dex"]);
            CSVTableHelper.SetValue(ref luk, data["luk"]);
        }
    }
}

public class PureDnaBonusTable : IDefTable
{
    private Dictionary<int, Generated.CsvData.pureDnaBonusData> _dicData = new Dictionary<int, Generated.CsvData.pureDnaBonusData>();
    public Dictionary<int, Generated.CsvData.pureDnaBonusData> DicData { get { return _dicData; } }

    override public void SetData(List<Dictionary<string, string>> csvTable)
    {
        for (int i = 0; i < csvTable.Count; ++i)
        {
            Generated.CsvData.pureDnaBonusData data = new Generated.CsvData.pureDnaBonusData();
            data.Load(csvTable[i]);

            if (_dicData.ContainsKey(data.index))
            {
                Debug.LogErrorFormat("{0} 테이블 {1}인덱스 중복!!", GetType(), data.index);
                continue;
            }
            _dicData.Add(data.index, data);
        }
    }

    public Generated.CsvData.pureDnaBonusData GetData(int index)
    {
        Generated.CsvData.pureDnaBonusData data;

        if (_dicData.TryGetValue(index, out data) == false)
            Debug.LogError(string.Format("{0} Table {1} Index Find Failed", GetType(), index));

        return data;
    }
}
