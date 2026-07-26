using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Generated.CsvData
{
    public class ConditionData
    {
        public int idx;
        public Condition_Type condition1_type;
        public int condition1_value;
        public Condition_Type condition2_type;
        public int condition2_value;
        public Condition_Type condition3_type;
        public int condition3_value;
        public Condition_Type condition4_type;
        public int condition4_value;
        public Condition_Type condition5_type;
        public int condition5_value;

        public void Load(Dictionary<string, string> data)
        {
            CSVTableHelper.SetValue(ref idx, data["idx"]);
            CSVTableHelper.SetValue(ref condition1_type, data["condition1_type"]);
            CSVTableHelper.SetValue(ref condition1_value, data["condition1_value"]);

            CSVTableHelper.SetValue(ref condition2_type, data["condition2_type"]);
            CSVTableHelper.SetValue(ref condition2_value, data["condition2_value"]);

            CSVTableHelper.SetValue(ref condition3_type, data["condition3_type"]);
            CSVTableHelper.SetValue(ref condition3_value, data["condition3_value"]);

            CSVTableHelper.SetValue(ref condition4_type, data["condition4_type"]);
            CSVTableHelper.SetValue(ref condition4_value, data["condition4_value"]);

            CSVTableHelper.SetValue(ref condition5_type, data["condition5_type"]);
            CSVTableHelper.SetValue(ref condition5_value, data["condition5_value"]);
        }
    }
}


public class ConditionTable : IDefTable
{
    private Dictionary<int, Generated.CsvData.ConditionData> _dicData = new Dictionary<int, Generated.CsvData.ConditionData>();
    public Dictionary<int, Generated.CsvData.ConditionData> DicData { get { return _dicData; } }

    public override void SetData(List<Dictionary<string, string>> csvTable)
    {
        for(int i = 0; i < csvTable.Count; i++)
        {
            Generated.CsvData.ConditionData data = new Generated.CsvData.ConditionData();
            data.Load(csvTable[i]);

            if(_dicData.ContainsKey(data.idx))
            {
                Debug.LogErrorFormat("{0} 테이블 {1}인덱스 중복!!", GetType(), data.idx);
                continue;
            }

            _dicData.Add(data.idx, data);
        }
    }

    public Generated.CsvData.ConditionData GetData(int index)
    {
        Generated.CsvData.ConditionData data;
        
        if(_dicData.TryGetValue(index, out data) == false)
            Debug.LogError(string.Format("{0} Table {1} Index Find Failed", GetType(), index));

        return data;
    }
}
