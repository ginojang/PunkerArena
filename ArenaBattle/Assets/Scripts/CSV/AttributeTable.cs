using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Generated.CsvData
{
    public class AttributeData
    {
        public int idx;
        public string name_string;
        public float day_buff_atk;
        public float day_buff_def;
        public float night_buff_atk;
        public float night_buff_def;
        public float dawn_buff_atk;
        public float dawn_buff_def;
        public float eclipse_buff_atk;
        public float eclipse_buff_def;
        public string res_atlas;
        public string res_icon;

        public void Load(Dictionary<string, string> data)
        {
            CSVTableHelper.SetValue(ref idx, data["idx"]);
            CSVTableHelper.SetValue(ref name_string, data["name_string"]);
            CSVTableHelper.SetValue(ref day_buff_atk, data["day_buff_atk"]);
            CSVTableHelper.SetValue(ref day_buff_def, data["day_buff_def"]);
            CSVTableHelper.SetValue(ref night_buff_atk, data["night_buff_atk"]);
            CSVTableHelper.SetValue(ref night_buff_def, data["night_buff_def"]);
            CSVTableHelper.SetValue(ref dawn_buff_atk, data["dawn_buff_atk"]);
            CSVTableHelper.SetValue(ref dawn_buff_def, data["dawn_buff_def"]);
            CSVTableHelper.SetValue(ref eclipse_buff_atk, data["eclipse_buff_atk"]);
            CSVTableHelper.SetValue(ref eclipse_buff_def, data["eclipse_buff_def"]);
            CSVTableHelper.SetValue(ref res_atlas, data["res_atlas"]);
            CSVTableHelper.SetValue(ref res_icon, data["res_icon"]);
        }
    }
}

public class AttributeTable : IDefTable
{
    private Dictionary<int, Generated.CsvData.AttributeData> _dicData = new Dictionary<int, Generated.CsvData.AttributeData>();

    public override void SetData(List<Dictionary<string, string>> csvTable)
    {
        for (int i = 0; i < csvTable.Count; i++)
        {
            Generated.CsvData.AttributeData data = new Generated.CsvData.AttributeData();
            data.Load(csvTable[i]);

            if (_dicData.ContainsKey(data.idx))
            {
                Debug.LogErrorFormat("{0} ���̺� {1}�ε��� �ߺ�!!", GetType(), data.idx);
                continue;
            }

            _dicData.Add(data.idx, data);
        }
    }

    public Generated.CsvData.AttributeData GetData(int index)
    {
        Generated.CsvData.AttributeData data;
        if (_dicData.TryGetValue(index, out data) == false)
            Debug.LogError(string.Format("{0} Table {1} Index Find Failed", GetType(), index));

        return data;
    }
}
