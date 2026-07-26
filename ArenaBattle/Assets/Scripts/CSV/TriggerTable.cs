using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Generated.CsvData
{
    public class TriggerData
    {
        public struct TriggerInfo
        {
            public Action_Type action_type;
            public Trigger_Timing action_timing;
            public int action_condition_id;
            public float action_rate;
            public float action_rate_per_level;
            public Type_Target action_type_target;
            public int action_id;
            public int action_duration_turn;
            public int action_count;
        }

        public int idx;
        public List<TriggerInfo> triggerInfoList = new List<TriggerInfo>();

        public void Load(Dictionary<string, string> data)
        {
            if(triggerInfoList == null)
                triggerInfoList = new List<TriggerInfo>();

            CSVTableHelper.SetValue(ref idx, data["idx"]);

            for (int i = 0; i < 5; i++)
            {
                int id = 0;
                CSVTableHelper.SetValue(ref id, data[$"action{i+1}_id"]);
                if (id == 0)
                    break;

                TriggerInfo info = new TriggerInfo();

                CSVTableHelper.SetValue(ref info.action_timing, data[$"action{i + 1}_timing"]);
                CSVTableHelper.SetValue(ref info.action_type, data[$"action{i + 1}_type"]);
                CSVTableHelper.SetValue(ref info.action_condition_id, data[$"action{i + 1}_condition_id"]);
                CSVTableHelper.SetValue(ref info.action_rate, data[$"action{i + 1}_rate"]);
                CSVTableHelper.SetValue(ref info.action_rate_per_level, data[$"action{i + 1}_rate_per_level"]);
                CSVTableHelper.SetValue(ref info.action_type_target, data[$"action{i + 1}_type_target"]);
                info.action_id = id;
                CSVTableHelper.SetValue(ref info.action_duration_turn, data[$"action{i + 1}_duration_turn"]);
                CSVTableHelper.SetValue(ref info.action_count, data[$"action{i + 1}_count"]);

                triggerInfoList.Add(info);
            }
        }
    }
}

public class TriggerTable : IDefTable
{
    private Dictionary<int, Generated.CsvData.TriggerData> _dicData = new Dictionary<int, Generated.CsvData.TriggerData>();
    
    public Dictionary<int, Generated.CsvData.TriggerData> DicData { get { return _dicData; } }

    public override void SetData(List<Dictionary<string, string>> csvTable)
    {
        for(int i = 0; i < csvTable.Count; i++)
        {
            Generated.CsvData.TriggerData data = new Generated.CsvData.TriggerData();
            data.Load(csvTable[i]);

            if(_dicData.ContainsKey(data.idx))
            {
                Debug.LogErrorFormat("{0} 테이블 {1}인덱스 중복!!", GetType(), data.idx);
                continue;
            }

            _dicData.Add(data.idx, data);
        }
    }

    public Generated.CsvData.TriggerData GetData(int index)
    {
        Generated.CsvData.TriggerData data;
        if(_dicData.TryGetValue(index, out data) == false)
            Debug.LogError(string.Format("{0} Table {1} Index Find Failed", GetType(), index));

        return data;
    }

}
