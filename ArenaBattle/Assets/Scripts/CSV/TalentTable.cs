using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Generated.CsvData
{
    public class talentData
    {
        public int index;
        public int type;
        public int setGroup;

        public Dictionary<CharacterClass, int> setgroupList = new Dictionary<CharacterClass, int>();

        public void Load(Dictionary<string, string> data)
        {
            CSVTableHelper.SetValue(ref index, data["idx"]);
            CSVTableHelper.SetValue(ref type, data["type"]);

            int val = 0;

            CSVTableHelper.SetValue(ref val, data["banana_set_group"]);
            setgroupList.Add(CharacterClass.banana, val);
            CSVTableHelper.SetValue(ref val, data["orange_set_group"]);
            setgroupList.Add(CharacterClass.orange, val);
            CSVTableHelper.SetValue(ref val, data["water_melon_set_group"]);
            setgroupList.Add(CharacterClass.watermelon, val);
            CSVTableHelper.SetValue(ref val, data["durian_set_group"]);
            setgroupList.Add(CharacterClass.durian, val);
            CSVTableHelper.SetValue(ref val, data["coconut_set_group"]);
            setgroupList.Add(CharacterClass.coconut, val);
            CSVTableHelper.SetValue(ref val, data["rambutan_set_group"]);
            setgroupList.Add(CharacterClass.rambutan, val);
            CSVTableHelper.SetValue(ref val, data["blueberry_set_group"]);
            setgroupList.Add(CharacterClass.blueberry, val);
            CSVTableHelper.SetValue(ref val, data["pineapple_set_group"]);
            setgroupList.Add(CharacterClass.pineapple, val);
            CSVTableHelper.SetValue(ref val, data["melon_set_group"]);
            setgroupList.Add(CharacterClass.melon, val);
            CSVTableHelper.SetValue(ref val, data["dragon_fruits_set_group"]);
            setgroupList.Add(CharacterClass.dragonfruit, val);
            CSVTableHelper.SetValue(ref val, data["limited_set_group"]);
            setgroupList.Add(CharacterClass.limited, val);
        }
    }
}

public class TalentTable : IDefTable
{
    private Dictionary<CharacterTalent, Dictionary<int, Generated.CsvData.talentData>> _dicDataList = new Dictionary<CharacterTalent, Dictionary<int, Generated.CsvData.talentData>>();

    override public void SetData(List<Dictionary<string, string>> csvTable)
    {
        Dictionary<int, Generated.CsvData.talentData> datas = new Dictionary<int, Generated.CsvData.talentData>();

        for (int i = 0; i < csvTable.Count; ++i)
        {
            Generated.CsvData.talentData data = new Generated.CsvData.talentData();

            data.Load(csvTable[i]);

            CharacterTalent type = (CharacterTalent)(data.type);

            if (!_dicDataList.ContainsKey(type))
            {
                datas = new Dictionary<int, Generated.CsvData.talentData>();
                _dicDataList.Add(type, datas);
            }
            else
                datas = _dicDataList[type];

            if (datas.ContainsKey(data.index))
            {
                Debug.LogErrorFormat($"{GetType()} 테이블 {data.index}인덱스 중복!!");
                continue;
            }
            datas.Add(data.index, data);
        }
    }

    public Generated.CsvData.talentData GetData(CharacterTalent charTalent, CharacterClass charType, int index)
    {
        if (!_dicDataList.ContainsKey(charTalent))
        {
            Debug.LogErrorFormat($"{charTalent} 테이블 없음!!");
            return null;
        }

        Dictionary<int, Generated.CsvData.talentData> datas = _dicDataList[charTalent];

        Generated.CsvData.talentData data;

        if (datas.TryGetValue(index, out data) == false)
            Debug.LogError(string.Format("{0} Table {1} Index Find Failed", GetType(), index));

        return data;
    }

    public Dictionary<int, Generated.CsvData.talentData> GetDataList(CharacterTalent charTalent, CharacterClass charType)
    {
        if (!_dicDataList.ContainsKey(charTalent))
            Debug.LogError(string.Format("{0} Table {1} Index Find Failed", GetType(), charType));

        Dictionary<int, Generated.CsvData.talentData> data = _dicDataList[charTalent];

        return data;
    }

    public int GetPartInfo(CharacterTalent charTalent, CharacterClass charType, int setIndex)
    {
        if (!_dicDataList.ContainsKey(charTalent))
            Debug.LogError(string.Format("{0} Table {1} Index Find Failed", GetType(), charTalent));

        Dictionary<int, Generated.CsvData.talentData> data = _dicDataList[charTalent];

        Generated.CsvData.talentData partsdata = GetData(charTalent, charType, setIndex);

        return partsdata.setgroupList[charType];
    }
}
