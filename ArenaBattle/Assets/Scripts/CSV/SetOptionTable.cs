using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Generated.CsvData
{
    public class setOptionData
    {
        public int index;
        public string tagTitleString;
        public int setCountMax;
        public int set2OptionType;
        public int set2OptionValue;
        public int set3OptionType;
        public int set3OptionValue;
        public int set4OptionType;
        public int set4OptionValue;
        public int set5OptionType;
        public int set5OptionValue;
        public int set6OptionType;
        public int set6OptionValue;

        public string head;
        public string eyes;
        public string mouth;
        public string back;
        public string tail;
        public string wing;


        public void Load(Dictionary<string, string> data)
        {
            CSVTableHelper.SetValue(ref index, data["idx"]);
            CSVTableHelper.SetValue(ref tagTitleString, data["tag_title_string"]);
            CSVTableHelper.SetValue(ref setCountMax, data["set_count_max"]);
            CSVTableHelper.SetValue(ref set2OptionType, data["2set_option_type"]);
            CSVTableHelper.SetValue(ref set2OptionValue, data["2set_option_value"]);
            CSVTableHelper.SetValue(ref set2OptionType, data["3set_option_type"]);
            CSVTableHelper.SetValue(ref set2OptionValue, data["3set_option_value"]);
            CSVTableHelper.SetValue(ref set2OptionType, data["4set_option_type"]);
            CSVTableHelper.SetValue(ref set2OptionValue, data["4set_option_value"]);
            CSVTableHelper.SetValue(ref set2OptionType, data["5set_option_type"]);
            CSVTableHelper.SetValue(ref set2OptionValue, data["5set_option_value"]);
            CSVTableHelper.SetValue(ref set2OptionType, data["6set_option_type"]);
            CSVTableHelper.SetValue(ref set2OptionValue, data["6set_option_value"]);
            CSVTableHelper.SetValue(ref head, data["head"]);
            CSVTableHelper.SetValue(ref eyes, data["eyes"]);
            CSVTableHelper.SetValue(ref mouth, data["mouth"]);
            CSVTableHelper.SetValue(ref back, data["back"]);
            CSVTableHelper.SetValue(ref tail, data["tail"]);
            CSVTableHelper.SetValue(ref wing, data["wing"]);
        }
    }
}

public class SetOptionTable : IDefTable
{
    private Dictionary<int, Generated.CsvData.setOptionData> _dicData = new Dictionary<int, Generated.CsvData.setOptionData>();
    public Dictionary<int, Generated.CsvData.setOptionData> DicData { get { return _dicData; } }

    override public void SetData(List<Dictionary<string, string>> csvTable)
    {
        for (int i = 0; i < csvTable.Count; ++i)
        {
            Generated.CsvData.setOptionData data = new Generated.CsvData.setOptionData();
            data.Load(csvTable[i]);

            if (_dicData.ContainsKey(data.index))
            {
                Debug.LogErrorFormat("{0} 테이블 {1}인덱스 중복!!", GetType(), data.index);
                continue;
            }
            _dicData.Add(data.index, data);
        }
    }

    public Generated.CsvData.setOptionData GetData(int index)
    {
        Generated.CsvData.setOptionData data;

        if (_dicData.TryGetValue(index, out data) == false)
            Debug.LogError(string.Format("{0} Table {1} Index Find Failed", GetType(), index));

        return data;
    }
}
