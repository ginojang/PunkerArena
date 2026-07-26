using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Generated.CsvData
{
    public class ItemData
    {
        public int idx;
        public string name_string;
        public string desc_string;
        public ItemType type;
        public int grade;
        public bool stack;
        public int get_info_id;
        public string res_atlas;
        public string res_icon;

        public void Load(Dictionary<string, string> data)
        {
            CSVTableHelper.SetValue(ref idx, data["idx"]);
            CSVTableHelper.SetValue(ref name_string, data["name_string"]);
            CSVTableHelper.SetValue(ref desc_string, data["desc_string"]);
            CSVTableHelper.SetValue(ref type, data["type"]);
            CSVTableHelper.SetValue(ref grade, data["grade"]);
            CSVTableHelper.SetValue(ref get_info_id, data["get_info_id"]);
            CSVTableHelper.SetValue(ref res_atlas, data["res_atlas"]);
            CSVTableHelper.SetValue(ref res_icon, data["res_icon"]);

            int temp = 0;
            stack = false;
            CSVTableHelper.SetValue(ref temp, data["stack"]);
            if (temp > 0)
                stack = true;
        }
    }
}

public class ItemTable : IDefTable
{
    private Dictionary<int, Generated.CsvData.ItemData> _dicData = new Dictionary<int, Generated.CsvData.ItemData>();

    public override void SetData(List<Dictionary<string, string>> csvTable)
    {
        for (int i = 0; i < csvTable.Count; i++)
        {
            Generated.CsvData.ItemData data = new Generated.CsvData.ItemData();
            data.Load(csvTable[i]);

            if (_dicData.ContainsKey(data.idx))
            {
                Debug.LogErrorFormat("{0} 테이블 {1}인덱스 중복!!", GetType(), data.idx);
                continue;
            }

            _dicData.Add(data.idx, data);
        }
    }

    public Generated.CsvData.ItemData GetData(int index)
    {
        Generated.CsvData.ItemData data;
        if (_dicData.TryGetValue(index, out data) == false)
            Debug.LogError(string.Format("{0} Table {1} Index Find Failed", GetType(), index));

        return data;
    }
}
