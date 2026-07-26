using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum SoundType
{
    BGM = 1,
    EFFECT,
}

namespace Generated.CsvData
{
    public class soundData
    {
        public int index;
        public string filename;
        public SoundType type;

        public void Load(Dictionary<string, string> data)
        {
            CSVTableHelper.SetValue(ref index, data["idx"]);
            CSVTableHelper.SetValue(ref filename, data["filename"]);
            CSVTableHelper.SetValue(ref type, data["type"]);
        }
    }
}

public class SoundTable : IDefTable
{
    private Dictionary<int, Generated.CsvData.soundData> _dicData = new Dictionary<int, Generated.CsvData.soundData>();
    public Dictionary<int, Generated.CsvData.soundData> DicData { get { return _dicData; } }

    override public void SetData(List<Dictionary<string, string>> csvTable)
    {
        for (int i = 0; i < csvTable.Count; ++i)
        {
            Generated.CsvData.soundData data = new Generated.CsvData.soundData();
            data.Load(csvTable[i]);

            if (_dicData.ContainsKey(data.index))
            {
                Debug.LogErrorFormat("{0} 테이블 {1}인덱스 중복!!", GetType(), data.index);
                continue;
            }
            _dicData.Add(data.index, data);
        }
    }

    public Generated.CsvData.soundData GetData(int index)
    {
        Generated.CsvData.soundData data;

        if (_dicData.TryGetValue(index, out data) == false)
            Debug.LogError(string.Format("{0} Table {1} Index Find Failed", GetType(), index));

        return data;
    }
}
