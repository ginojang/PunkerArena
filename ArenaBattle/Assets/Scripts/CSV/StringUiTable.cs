using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Generated.CsvData
{
    public class stringUiData
    {
        public int mIdx;
        public string mText;

        public void Load(Dictionary<string, string> _data)
        {
            CSVTableHelper.SetValue(ref mIdx, _data["idx"]);
            CSVTableHelper.SetValue(ref mText, _data["text"]);
        }

        public string GetSting()
        {
            return mText;
        }
    }
}

public class StringUiTable : IDefTable
{
    // Start is called before the first frame update
    private Dictionary<int, Generated.CsvData.stringUiData> mDicData = new Dictionary<int, Generated.CsvData.stringUiData>();
    public Dictionary<int, Generated.CsvData.stringUiData> DicData { get { return mDicData; } }

    override public void SetData(List<Dictionary<string, string>> _csvTable)
    {
        for (int i = 0; i < _csvTable.Count; ++i)
        {
            Generated.CsvData.stringUiData data = new Generated.CsvData.stringUiData();
            data.Load(_csvTable[i]);

            if (mDicData.ContainsKey(data.mIdx))
            {
                Debug.LogErrorFormat("{0} 테이블 {1}인덱스 중복!!", GetType(), data.mIdx);
                continue;
            }
            mDicData.Add(data.mIdx, data);
        }
    }

    public Generated.CsvData.stringUiData GetData(int _index)
    {
        Generated.CsvData.stringUiData data;

        if (mDicData.TryGetValue(_index, out data) == false)
            Debug.LogError(string.Format("{0} Table {1} Index Find Failed", GetType(), _index));

        return data;
    }

    public string GetString(int _index)
    {
        Generated.CsvData.stringUiData data = GetData(_index);

        if (data == null)
            return string.Empty;

        return data.mText;
    }
}
