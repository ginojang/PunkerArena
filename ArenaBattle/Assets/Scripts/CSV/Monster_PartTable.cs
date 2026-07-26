using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Generated.CsvData
{
    public class Monster_PartData
    {
        public int idx;
        public string body;
        public string head;
        public string eyes;
        public string mouth;
        public string back;
        public string tail;
        public string wing;
        public string belly;
        public string color;

        public void Load(Dictionary<string, string> data)
        {
            CSVTableHelper.SetValue(ref idx, data["idx"]);
            CSVTableHelper.SetValue(ref body, data["body"]);
            CSVTableHelper.SetValue(ref head, data["head"]);
            CSVTableHelper.SetValue(ref eyes, data["eyes"]);
            CSVTableHelper.SetValue(ref mouth, data["mouth"]);
            CSVTableHelper.SetValue(ref back, data["back"]);
            CSVTableHelper.SetValue(ref tail, data["tail"]);
            CSVTableHelper.SetValue(ref wing, data["wing"]);
            CSVTableHelper.SetValue(ref belly, data["belly"]);
            CSVTableHelper.SetValue(ref color, data["color"]);
        }
    }
}

public class Monster_PartTable : IDefTable
{
    private Dictionary<int, Generated.CsvData.Monster_PartData> _dicData = new Dictionary<int, Generated.CsvData.Monster_PartData>();

    public override void SetData(List<Dictionary<string, string>> csvTable)
    {
        for (int i = 0; i < csvTable.Count; i++)
        {
            Generated.CsvData.Monster_PartData data = new Generated.CsvData.Monster_PartData();
            data.Load(csvTable[i]);

            if (_dicData.ContainsKey(data.idx))
            {
                Debug.LogErrorFormat("{0} 테이블 {1}인덱스 중복!!", GetType(), data.idx);
                continue;
            }

            _dicData.Add(data.idx, data);
        }
    }

    public Generated.CsvData.Monster_PartData GetData(int index)
    {
        Generated.CsvData.Monster_PartData data;
        if (_dicData.TryGetValue(index, out data) == false)
            Debug.LogError(string.Format("{0} Table {1} Index Find Failed", GetType(), index));

        return data;
    }
}
