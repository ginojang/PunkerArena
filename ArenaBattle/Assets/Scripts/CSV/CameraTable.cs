using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Generated.CsvData
{
    public class camaraData
    {
        public int idx;
        public string camaera;

        public void Load(Dictionary<string,string> data)
        {
            CSVTableHelper.SetValue(ref idx, data["idx"]);
            CSVTableHelper.SetValue(ref camaera, data["camaera"]);
        }
    }
}

public class CameraTable : IDefTable
{
    private Dictionary<int, Generated.CsvData.camaraData> _dicData = new Dictionary<int, Generated.CsvData.camaraData>();
    public Dictionary<int, Generated.CsvData.camaraData> DicData { get { return _dicData; } }

    public override void SetData(List<Dictionary<string, string>> csvTable)
    {
        for(int i = 0; i < csvTable.Count; i++)
        {
            Generated.CsvData.camaraData data = new Generated.CsvData.camaraData();
            data.Load(csvTable[i]);

            if(_dicData.ContainsKey(data.idx))
            {
                Debug.LogErrorFormat("{0} 테이블 {1}인덱스 중복!!", GetType(), data.idx);
                continue;
            }

            _dicData.Add(data.idx, data);
        }
    }
    public Generated.CsvData.camaraData GetData(int index)
    {
        Generated.CsvData.camaraData data;

        if (_dicData.TryGetValue(index, out data) == false)
            Debug.LogError(string.Format("{0} Table {1} Index Find Failed", GetType(), index));

        return data;
    }

}
