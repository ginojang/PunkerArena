using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Generated.CsvData
{
    public class burstData
    {
        public int burst_timeLineDistancePer;
        public float burst_recoverytime;
        public float burst_startgauge;
        public int burst_waitslot_rate;
        public int burst_timeline_rate;
        public int burst_ondamagecount;
        public float burst_ondamagetime;
        public float burst_ondamagehittime;
        public int burst_effectidx;
        public int burst_full_effectidx;
        public float burst_normal_value;
        public float burst_full_value;
        public float burst_ultra_value;

        public void Load(Dictionary<string, string> data)
        {
            CSVTableHelper.SetValue(ref burst_timeLineDistancePer, data["burst_timeLineDistancePer"]);
            CSVTableHelper.SetValue(ref burst_recoverytime, data["burst_recoverytime"]);
            CSVTableHelper.SetValue(ref burst_startgauge, data["burst_startgauge"]);
            CSVTableHelper.SetValue(ref burst_waitslot_rate, data["burst_waitslot_rate"]);
            CSVTableHelper.SetValue(ref burst_timeline_rate, data["burst_timeline_rate"]);
            CSVTableHelper.SetValue(ref burst_ondamagecount, data["burst_ondamagecount"]);
            CSVTableHelper.SetValue(ref burst_ondamagetime, data["burst_ondamagetime"]);
            CSVTableHelper.SetValue(ref burst_ondamagehittime, data["burst_ondamagehittime"]);
            CSVTableHelper.SetValue(ref burst_effectidx, data["burst_effectidx"]);
            CSVTableHelper.SetValue(ref burst_full_effectidx, data["burst_full_effectidx"]);
            CSVTableHelper.SetValue(ref burst_normal_value, data["burst_normal_value"]);
            CSVTableHelper.SetValue(ref burst_full_value, data["burst_full_value"]);
            CSVTableHelper.SetValue(ref burst_ultra_value, data["burst_ultra_value"]);
        }
    }
}

public class BurstTable : IDefTable
{
    private Generated.CsvData.burstData _busrtdata;

    public override void SetData(List<Dictionary<string, string>> csvTable)
    {
        for(int i = 0; i < csvTable.Count; i++)
        {
            _busrtdata = new Generated.CsvData.burstData();
            _busrtdata.Load(csvTable[i]);
        }
    }

    public Generated.CsvData.burstData GetData()
    {
        return _busrtdata;
    }
}
