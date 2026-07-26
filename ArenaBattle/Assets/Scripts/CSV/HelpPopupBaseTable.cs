using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Generated.CsvData
{
    public class helpPopupBaseData
    {
        public string mIdx;
        public string mPrefabName;

        public void Load(Dictionary<string, string> _data)
        {
            CSVTableHelper.SetValue(ref mIdx, _data["idx"]);
            CSVTableHelper.SetValue(ref mPrefabName, _data["prefabname"]);
        }

        public string GetPrefabName()
        {
            return mPrefabName;
        }
    }
}

public class HelpPopupBaseTable : IDefTable
{
    // Start is called before the first frame update
    private Dictionary<string, Generated.CsvData.helpPopupBaseData> mDicData = new Dictionary<string, Generated.CsvData.helpPopupBaseData>();
    public Dictionary<string, Generated.CsvData.helpPopupBaseData> DicData { get { return mDicData; } }

    override public void SetData(List<Dictionary<string, string>> _csvTable)
    {
        for (int i = 0; i < _csvTable.Count; ++i)
        {
            Generated.CsvData.helpPopupBaseData data = new Generated.CsvData.helpPopupBaseData();
            data.Load(_csvTable[i]);

            if (mDicData.ContainsKey(data.mIdx))
            {
                Debug.LogErrorFormat("{0} 테이블 {1}인덱스 중복!!", GetType(), data.mIdx);
                continue;
            }
            mDicData.Add(data.mIdx, data);
        }
    }

    public Generated.CsvData.helpPopupBaseData GetData(string _index)
    {
        Generated.CsvData.helpPopupBaseData data;

        if (mDicData.TryGetValue(_index, out data) == false)
            Debug.LogError(string.Format("{0} Table {1} Index Find Failed", GetType(), _index));

        return data;
    }

    public string GetPrefabName(string _index)
    {
        Generated.CsvData.helpPopupBaseData data = GetData(_index);

        if (mDicData.TryGetValue(_index, out data) == false)
            Debug.LogError(string.Format("{0} Table {1} Index Find Failed", GetType(), _index));

        return data.GetPrefabName();
    }
}

