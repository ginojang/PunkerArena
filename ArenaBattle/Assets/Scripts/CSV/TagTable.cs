using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Generated.CsvData
{
    public class TagData
    {
        public int idx;
        public string name_string;
        public string desc_string;
        public string res_icon;

        public void Load(Dictionary<string, string> data)
        {
            CSVTableHelper.SetValue(ref idx, data["idx"]);
            CSVTableHelper.SetValue(ref name_string, data["name_string"]);
            CSVTableHelper.SetValue(ref desc_string, data["desc_string"]);
            CSVTableHelper.SetValue(ref res_icon, data["res_icon"]);
        }
    }
}
public class TagTable : IDefTable
{
    private Dictionary<int, Generated.CsvData.TagData> _dicData = new Dictionary<int, Generated.CsvData.TagData>();

    public override void SetData(List<Dictionary<string, string>> csvTable)
    {
        for (int i = 0; i < csvTable.Count; i++)
        {
            Generated.CsvData.TagData data = new Generated.CsvData.TagData();
            data.Load(csvTable[i]);

            if (_dicData.ContainsKey(data.idx))
            {
                Debug.LogErrorFormat("{0} 테이블 {1}인덱스 중복!!", GetType(), data.idx);
                continue;
            }

            _dicData.Add(data.idx, data);
        }
    }

    public Generated.CsvData.TagData GetData(int index)
    {
        Generated.CsvData.TagData data;
        if (_dicData.TryGetValue(index, out data) == false)
            Debug.LogError(string.Format("{0} Table {1} Index Find Failed", GetType(), index));

        return data;
    }
}
