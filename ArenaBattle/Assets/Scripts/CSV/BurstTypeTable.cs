using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Generated.CsvData
{
    public class burstTypeData
    {
        public int burst_type;
        public int burst_buffid01;
        public int burst_buffid02;

        public void Load(Dictionary<string, string> data)
        {
            CSVTableHelper.SetValue(ref burst_type, data["burst_type"]);
            CSVTableHelper.SetValue(ref burst_buffid01, data["burst_buffid01"]);
            CSVTableHelper.SetValue(ref burst_buffid02, data["burst_buffid02"]);
        }

    
    }
}

public class BurstTypeTable : IDefTable
{
    private Dictionary<int, Generated.CsvData.burstTypeData> _dicData = new Dictionary<int, Generated.CsvData.burstTypeData>();
    public Dictionary<int, Generated.CsvData.burstTypeData> DicData { get { return _dicData; } }

    public override void SetData(List<Dictionary<string, string>> csvTable)
    {
        for(int i = 0; i < csvTable.Count; i++)
        {
            Generated.CsvData.burstTypeData data = new Generated.CsvData.burstTypeData();
            data.Load(csvTable[i]);

            if(_dicData.ContainsKey(data.burst_type))
            {
                Debug.LogErrorFormat("{0} 테이블 {1}인덱스 중복!!", GetType(), data.burst_type);
                continue;
            }
            _dicData.Add(data.burst_type, data);
        }
    }

    public Generated.CsvData.burstTypeData GetData(int bursttype)
    {
        Generated.CsvData.burstTypeData data;
        if(_dicData.TryGetValue(bursttype, out data) == false)
            Debug.LogError(string.Format("{0} Table {1} Index Find Failed", GetType(), bursttype));

        return data;
    }
}
