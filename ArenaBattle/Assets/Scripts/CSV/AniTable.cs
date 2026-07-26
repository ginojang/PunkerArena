using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Generated.CsvData
{
    public class aniTableData
    {
        public int idx;
        public CharacterTalent talent;
        public bool wing;
        public string state;
        public string res_ani;

        public void Load(Dictionary<string, string> data)
        {
            CSVTableHelper.SetValue(ref idx, data["idx"]);
            CSVTableHelper.SetValue(ref talent, data["talent"]);

            CSVTableHelper.SetValue(ref state, data["state"]);
            CSVTableHelper.SetValue(ref res_ani, data["res_ani"]);

            int temp = 0;
            wing = false;
            CSVTableHelper.SetValue(ref temp, data["wing"]);

            if (temp > 0)
                wing = true;
        }
    }
}

public class AniTable : IDefTable
{
    private MultiDictionary<int, Generated.CsvData.aniTableData> _dicData = new MultiDictionary<int, Generated.CsvData.aniTableData>();
    public MultiDictionary<int, Generated.CsvData.aniTableData> DicData { get { return _dicData; } }

    override public void SetData(List<Dictionary<string, string>> csvTable)
    {
        for (int i = 0; i < csvTable.Count; ++i)
        {
            Generated.CsvData.aniTableData data = new Generated.CsvData.aniTableData();
            data.Load(csvTable[i]);

            _dicData.Add(data.idx, data);
        }
    }

    public List<Generated.CsvData.aniTableData> GetData(int idx)
    {
        List<Generated.CsvData.aniTableData> data;

        data = _dicData[idx];
        
        return data;
    }

    public Generated.CsvData.aniTableData GetData(int idx, CharacterTalent talent, bool wing)
    {
        Generated.CsvData.aniTableData result = null;
        List<Generated.CsvData.aniTableData> data;

        data = GetData(idx);

        for ( int i = 0; i < data.Count; i++)
        {
            if (data[i].talent != talent)
                continue;

            if (data[i].wing != wing)
                continue;

            result = data[i];
            break;
        }

        return result;
    }
}