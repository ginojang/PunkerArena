using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Generated.CsvData
{
    public class constBaseData
    {
        public int mIdx;
        public int mValue;

        public void Load(Dictionary<string, string> _data)
        {
            CSVTableHelper.SetValue(ref mIdx, _data["idx"]);
            CSVTableHelper.SetValue(ref mValue, _data["value"]);
        }

        public int GetValue()
        {
            return mValue;
        }
    }
}

public class ConstBaseTable : IDefTable
{
    // Start is called before the first frame update
    private Dictionary<int, Generated.CsvData.constBaseData> mDicData = new Dictionary<int, Generated.CsvData.constBaseData>();
    public Dictionary<int, Generated.CsvData.constBaseData> DicData { get { return mDicData; } }

    override public void SetData(List<Dictionary<string, string>> _csvTable)
    {
        for (int i = 0; i < _csvTable.Count; ++i)
        {
            Generated.CsvData.constBaseData data = new Generated.CsvData.constBaseData();
            data.Load(_csvTable[i]);

            if (mDicData.ContainsKey(data.mIdx))
            {
                Debug.LogErrorFormat("{0} 테이블 {1}인덱스 중복!!", GetType(), data.mIdx);
                continue;
            }
            mDicData.Add(data.mIdx, data);
        }
    }

    public Generated.CsvData.constBaseData GetData(int _index)
    {
        Generated.CsvData.constBaseData data;

        if (mDicData.TryGetValue(_index, out data) == false)
            Debug.LogError(string.Format("{0} Table {1} Index Find Failed", GetType(), _index));

        return data;
    }
}
