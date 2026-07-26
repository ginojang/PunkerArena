using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Generated.CsvData
{
    public class aniskillData
    {
        public int index;
        public string aniName;

        public void Load(Dictionary<string, string> data)
        {
            CSVTableHelper.SetValue(ref index, data["idx"]);
            CSVTableHelper.SetValue(ref aniName, data["filename"]);
        }
    }
}

public class AniskillTable : IDefTable
{
    private MultiDictionary<int, Generated.CsvData.aniskillData> _dicData = new MultiDictionary<int, Generated.CsvData.aniskillData>();
    public MultiDictionary<int, Generated.CsvData.aniskillData> DicData { get { return _dicData; } }

    override public void SetData(List<Dictionary<string, string>> csvTable)
    {
        for (int i = 0; i < csvTable.Count; ++i)
        {
            Generated.CsvData.aniskillData data = new Generated.CsvData.aniskillData();
            data.Load(csvTable[i]);

            /*if (_dicData.ContainsKey(data.index))
            {
                Debug.LogErrorFormat("{0} 테이블 {1}인덱스 중복!!", GetType(), data.index);
                continue;
            }*/
            _dicData.Add(data.index, data);
        }
    }

    public List<Generated.CsvData.aniskillData> GetData(int idx)
    {
        List<Generated.CsvData.aniskillData> data;

        //if (!_dicData.ContainsKey(idx))
        //    Debug.LogError(string.Format("{0} Table {1} Index Find Failed", GetType(), idx));

        data = _dicData[idx];
        
        return data;
    }
}