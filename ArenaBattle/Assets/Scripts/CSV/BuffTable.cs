using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Generated.CsvData
{
    public class buffData
    {
        public int idx;
        public string res_atlas;
        public string buff_icon;
        public BuffType buff_type;
        public BuffUseCondition buff_usecondition;
        public BuffTargetType buff_target_type;
        public BuffEffect buff_effect;
        public float buff_effectrandomrate;
        public float buff_effect_value01;
        public float buffeffect_timetickdmg;
        // public BuffTimeType buff_timetype;
        public float buff_timevalue;
        public int buff_effectidx;
        public int buff_ani;
        public int buff_group;
        public int buff_level;
        
        public void Load(Dictionary<string, string> data)
        {
            CSVTableHelper.SetValue(ref idx, data["idx"]);
            CSVTableHelper.SetValue(ref buff_icon, data["buff_icon"]);
            CSVTableHelper.SetValue(ref buff_type, data["buff_type"]);
            CSVTableHelper.SetValue(ref buff_target_type, data["buff_target_type"]);
            CSVTableHelper.SetValue(ref buff_usecondition, data["buff_usecondition"]);
            CSVTableHelper.SetValue(ref buff_effect, data["buff_effect"]);
            CSVTableHelper.SetValue(ref buff_effect_value01, data["buff_effect_value01"]);
            // CSVTableHelper.SetValue(ref buff_timetype, data["buff_timetype"]);
            CSVTableHelper.SetValue(ref buff_timevalue, data["buff_timevalue"]);
            CSVTableHelper.SetValue(ref buff_effectidx, data["buff_effectidx"]);
            CSVTableHelper.SetValue(ref buff_ani, data["buff_ani"]);
            CSVTableHelper.SetValue(ref buff_group, data["buff_group"]);
            CSVTableHelper.SetValue(ref buff_level, data["buff_level"]);
        }
    }
}

public class BuffTable : IDefTable
{
    private Dictionary<int, Generated.CsvData.buffData> _dicData = new Dictionary<int, Generated.CsvData.buffData>();
    private Dictionary<int, Generated.CsvData.buffData> DicData { get { return _dicData; } }

    public override void SetData(List<Dictionary<string, string>> csvTable)
    {
        for(int i = 0; i < csvTable.Count; i++)
        {
            Generated.CsvData.buffData data = new Generated.CsvData.buffData();
            data.Load(csvTable[i]);

            if(_dicData.ContainsKey(data.idx))
            {
                Debug.LogErrorFormat("{0} 테이블 {1}인덱스 중복!!", GetType(), data.idx);
                continue;
            }

            _dicData.Add(data.idx, data);
        }
    }

    public Generated.CsvData.buffData GetData(int index)
    {
        Generated.CsvData.buffData data;

        if (_dicData.TryGetValue(index, out data) == false)
            Debug.LogError(string.Format("{0} Table {1} Index Find Failed", GetType(), index));

        return data;
    }

}

