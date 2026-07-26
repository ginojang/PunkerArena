using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Generated.CsvData
{
    public class stringData
    {
        public string index;
        public Dictionary<LANG, string> stringList = new Dictionary<LANG, string>();

        public void Load(Dictionary<string, string> data)
        {
            string lang = "";
            CSVTableHelper.SetValue(ref index, data["idx"]);
            CSVTableHelper.SetValue(ref lang, data["kr312"]);
            stringList.Add(LANG.KOR, lang);
            CSVTableHelper.SetValue(ref lang, data["en52"]);
            stringList.Add(LANG.ENG, lang);
        }
    }
}

public class StringTable : IDefTable
{
    private Dictionary<string, Generated.CsvData.stringData> _dicData = new Dictionary<string, Generated.CsvData.stringData>();
    public Dictionary<string, Generated.CsvData.stringData> DicData { get { return _dicData; } }

    override public void SetData(List<Dictionary<string, string>> csvTable)
    {
        for (int i = 0; i < csvTable.Count; ++i)
        {
            Generated.CsvData.stringData data = new Generated.CsvData.stringData();
            data.Load(csvTable[i]);

            if (_dicData.ContainsKey(data.index))
            {
                Debug.LogErrorFormat("{0} 테이블 {1}인덱스 중복!!", GetType(), data.index); 
                continue;
            }
            _dicData.Add(data.index, data);
        }
    }

    public Generated.CsvData.stringData GetData(string index)
    {
        Generated.CsvData.stringData data;

        if (_dicData.TryGetValue(index, out data) == false)
            Debug.LogError(string.Format("{0} Table {1} Index Find Failed", GetType(), index));

        if (GameDataManager.Instance.PlayerLanguage == SystemLanguage.Korean)
        {

        }
        else
        {
        }
        return data;
    }
    public string GetString(string index)
    {
        Generated.CsvData.stringData data;
        string str = "";

        if (_dicData.TryGetValue(index, out data) == false)
            Debug.LogError(string.Format("{0} Table {1} Index Find Failed", GetType(), index));

#if UNITY_EDITOR
        if (Application.isPlaying == true)
        {
            if (GameDataManager.Instance.PlayerLanguage == SystemLanguage.Korean)
            {
                data.stringList.TryGetValue(LANG.KOR, out str);
            }
            else
            {
                data.stringList.TryGetValue(LANG.ENG, out str);
            }
        }
        else
            data.stringList.TryGetValue(LANG.KOR, out str);
#else
        if (GameDataManager.Instance.PlayerLanguage == SystemLanguage.Korean)
        {
            data.stringList.TryGetValue(LANG.KOR, out str);
        }
        else
        {
            data.stringList.TryGetValue(LANG.ENG, out str);
        }
#endif

        return str;
    }

    public string GetLocalString(string index, LANG lang = LANG.KOR)
	{
        Generated.CsvData.stringData data = GetData(index);

        return data.stringList[lang];
	}


}
