using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Generated.CsvData
{
    public class partsData
    {
        public int index;
        public int type;
        public string limited_name;
        public int setGroup;

        public Dictionary<ItemType, partsInfo> partList = new Dictionary<ItemType, partsInfo>();

        public void Load(Dictionary<string, string> data)
        {
            CSVTableHelper.SetValue(ref index, data["idx"]);
            CSVTableHelper.SetValue(ref type, data["dino_type"]);
            CSVTableHelper.SetValue(ref limited_name, data["limited_name"]);
            CSVTableHelper.SetValue(ref setGroup, data["set_group"]);

            partsInfo partinfo = new partsInfo();
            CSVTableHelper.SetValue(ref partinfo.fileName, data["body"]);
            CSVTableHelper.SetValue(ref partinfo.partName, data["body_name_string"]);
            //CSVTableHelper.SetValue(ref partinfo.partSkill, data["body_skill"]);
            //CSVTableHelper.SetValue(ref partinfo.partOptionType, data["body_option_type"]);
            //CSVTableHelper.SetValue(ref partinfo.partOptionValue, data["body_option_value"]);
            CSVTableHelper.SetValue(ref partinfo.skill01, data["body_skill01"]);
            CSVTableHelper.SetValue(ref partinfo.skill02, data["body_skill02"]);

            partList.Add(ItemType.body, partinfo);

            partinfo = new partsInfo();
            CSVTableHelper.SetValue(ref partinfo.fileName, data["head"]);
            CSVTableHelper.SetValue(ref partinfo.partName, data["head_name_string"]);
            //CSVTableHelper.SetValue(ref partinfo.partSkill, data["head_skill"]);
            //CSVTableHelper.SetValue(ref partinfo.partOptionType, data["head_option_type"]);
            //CSVTableHelper.SetValue(ref partinfo.partOptionValue, data["head_option_value"]);
            CSVTableHelper.SetValue(ref partinfo.skill01, data["head_skill01"]);
            CSVTableHelper.SetValue(ref partinfo.skill02, data["head_skill02"]);
            partList.Add(ItemType.headparts, partinfo);

            partinfo = new partsInfo();
            CSVTableHelper.SetValue(ref partinfo.fileName, data["mouth"]);
            CSVTableHelper.SetValue(ref partinfo.partName, data["mouth_name_string"]);
            //CSVTableHelper.SetValue(ref partinfo.partSkill, data["mouth_skill"]);
            //CSVTableHelper.SetValue(ref partinfo.partOptionType, data["mouth_option_type"]);
            //CSVTableHelper.SetValue(ref partinfo.partOptionValue, data["mouth_option_value"]);
            CSVTableHelper.SetValue(ref partinfo.skill01, data["mouth_skill01"]);
            CSVTableHelper.SetValue(ref partinfo.skill02, data["mouth_skill02"]);
            partList.Add(ItemType.mouth, partinfo);

            partinfo = new partsInfo();
            CSVTableHelper.SetValue(ref partinfo.fileName, data["eyes"]);
            CSVTableHelper.SetValue(ref partinfo.partName, data["eyes_name_string"]);
            //CSVTableHelper.SetValue(ref partinfo.partSkill, data["eyes_skill"]);
            //CSVTableHelper.SetValue(ref partinfo.partOptionType, data["eyes_option_type"]);
            //CSVTableHelper.SetValue(ref partinfo.partOptionValue, data["eyes_option_value"]);
            CSVTableHelper.SetValue(ref partinfo.skill01, data["eyes_skill01"]);
            CSVTableHelper.SetValue(ref partinfo.skill02, data["eyes_skill02"]);
            partList.Add(ItemType.eyes, partinfo);

            partinfo = new partsInfo();
            CSVTableHelper.SetValue(ref partinfo.fileName, data["back"]);
            CSVTableHelper.SetValue(ref partinfo.partName, data["back_name_string"]);
            //CSVTableHelper.SetValue(ref partinfo.partSkill, data["back_skill"]);
            //CSVTableHelper.SetValue(ref partinfo.partOptionType, data["back_option_type"]);
            //CSVTableHelper.SetValue(ref partinfo.partOptionValue, data["back_option_value"]);
            CSVTableHelper.SetValue(ref partinfo.skill01, data["back_skill01"]);
            CSVTableHelper.SetValue(ref partinfo.skill02, data["back_skill02"]);
            partList.Add(ItemType.back, partinfo);

            partinfo = new partsInfo();
            CSVTableHelper.SetValue(ref partinfo.fileName, data["tail"]);
            CSVTableHelper.SetValue(ref partinfo.partName, data["tail_name_string"]);
            //CSVTableHelper.SetValue(ref partinfo.partSkill, data["tail_skill"]);
            //CSVTableHelper.SetValue(ref partinfo.partOptionType, data["tail_option_type"]);
            //CSVTableHelper.SetValue(ref partinfo.partOptionValue, data["tail_option_value"]);
            CSVTableHelper.SetValue(ref partinfo.skill01, data["tail_skill01"]);
            CSVTableHelper.SetValue(ref partinfo.skill02, data["tail_skill02"]);
            partList.Add(ItemType.tail, partinfo);

            partinfo = new partsInfo();
            CSVTableHelper.SetValue(ref partinfo.fileName, data["wing"]);
            CSVTableHelper.SetValue(ref partinfo.partName, data["wing_name_string"]);
            //CSVTableHelper.SetValue(ref partinfo.partSkill, data["wing_skill"]);
            //CSVTableHelper.SetValue(ref partinfo.partOptionType, data["wing_option_type"]);
            //CSVTableHelper.SetValue(ref partinfo.partOptionValue, data["wing_option_value"]);
            CSVTableHelper.SetValue(ref partinfo.skill01, data["wing_skill01"]);
            CSVTableHelper.SetValue(ref partinfo.skill02, data["wing_skill02"]);
            partList.Add(ItemType.wing, partinfo);
        }
    }
}

public class PartsTable : IDefTable
{
    private Dictionary<CharacterClass, Dictionary<int, Generated.CsvData.partsData>> _dicDataList = new Dictionary<CharacterClass, Dictionary<int, Generated.CsvData.partsData>>();

#if UNITY_EDITOR
    private List<Generated.CsvData.partsData> _lsitDataList = new List<Generated.CsvData.partsData>();
    public List<Generated.CsvData.partsData> List
    {
        get { return _lsitDataList; }
    }
#endif

    override public void SetData(List<Dictionary<string, string>> csvTable)
    {
        Dictionary<int, Generated.CsvData.partsData> datas = new Dictionary<int, Generated.CsvData.partsData>();
        int count = 0;
        for (int i = 0; i < csvTable.Count; ++i)
        {
            Generated.CsvData.partsData data = new Generated.CsvData.partsData();

            data.Load(csvTable[i]);

            CharacterClass type = (CharacterClass)(data.type);

            if (!_dicDataList.ContainsKey(type))
            {
                count = 0;
                datas = new Dictionary<int, Generated.CsvData.partsData>();
                _dicDataList.Add(type, datas);
            }
            else
                datas = _dicDataList[type];

            if (datas.ContainsKey(count))
            {
                Debug.LogErrorFormat($"{GetType()} 테이블 {data.index}인덱스 중복!!");
                continue;
            }
            datas.Add(count, data);

#if UNITY_EDITOR
            _lsitDataList.Add(data);
#endif

            count++;
        }
    }

    CharacterClass GetStringToCharacterClass(string type)
	{
        CharacterClass classtype = CharacterClass.None;

        switch(type)
		{
            case "banana":
				{
                    classtype = CharacterClass.banana;
				}
                break;
            case "orange":
                {
                    classtype = CharacterClass.orange;
                }
                break;
            case "watermelon":
                {
                    classtype = CharacterClass.watermelon;
                }
                break;
            case "durian":
                {
                    classtype = CharacterClass.durian;
                }
                break;
            case "coconut":
                {
                    classtype = CharacterClass.coconut;
                }
                break;
            case "rambutan":
                {
                    classtype = CharacterClass.rambutan;
                }
                break;
            case "blueberry":
                {
                    classtype = CharacterClass.blueberry;
                }
                break;
            case "pineapple":
                {
                    classtype = CharacterClass.pineapple;
                }
                break;
            case "melon":
                {
                    classtype = CharacterClass.melon;
                }
                break;
            case "dragonfruit":
                {
                    classtype = CharacterClass.dragonfruit;
                }
                break;
            case "limited":
                {
                    classtype = CharacterClass.limited;
                }
                break;
        }

        return classtype;
	}

    public Dictionary<int, Generated.CsvData.partsData> GetDataList(CharacterClass charType)
    {
        if (!_dicDataList.ContainsKey(charType))
            return null;
            //Debug.LogError(string.Format("{0} Table {1} Index Find Failed", GetType(), charType));

        Dictionary<int, Generated.CsvData.partsData> data = _dicDataList[charType];

        return data;
    }

    public Generated.CsvData.partsData GetData(CharacterClass charType, int index)
    {
        Dictionary<int, Generated.CsvData.partsData> datas = GetDataList(charType);

        Generated.CsvData.partsData data;

        if (datas == null)
            return null;

        if (datas.TryGetValue(index, out data) == false)
            Debug.LogError(string.Format("{0} Table {1} Index Find Failed", GetType(), index));

        return data;
    }

    public partsInfo GetPartInfo(CharacterClass charType, int setIndex, ItemType itemType)
    {
        if (!_dicDataList.ContainsKey(charType))
            Debug.LogError(string.Format("{0} Table {1} Index Find Failed", GetType(), charType));

        Dictionary<int, Generated.CsvData.partsData> data = _dicDataList[charType];

        Generated.CsvData.partsData partsdata = GetData(charType, setIndex);

        return partsdata.partList[itemType];
    }

    public Dictionary<int, Generated.CsvData.partsData> GetRandomDataList(ItemType type, ref CharacterClass charclass, ref int index)
	{
        List<CharacterClass> kList = new List<CharacterClass>(_dicDataList.Keys);

        Dictionary<CharacterClass, Dictionary<int, Generated.CsvData.partsData>> data2 = _dicDataList;

        while(kList.Count > 0)
		{
            int rand = UnityEngine.Random.Range(0, kList.Count);

            if((CharacterClass)kList[rand] == CharacterClass.dragonfruit)
                continue;

            // 선택된 과일의 아이템 리스트
            Dictionary<int, Generated.CsvData.partsData> data = data2[kList[rand]];

            var res = kList[rand];

            // 해당 과일의 아이템 세트 중에 한 세트를 랜덤하게 선택하여 그 안에 해당 파트 아이템이 존재하는지
            // 체크해보고 있으면 리턴, 없으면 다른 세트를 랜덤하게 다시 선택해서 반복

            List<int> kPartList = new List<int>(data.Keys);

            while (kPartList.Count > 0)
			{
                int rand2 = Random.Range(0, kPartList.Count);

                // 해당 세트에 해당 파트가 있으면 선택 완료
                if (data[kPartList[rand2]].partList.ContainsKey(type))
                {
                    if(data[kPartList[rand2]].partList[type].fileName != "none")
					{
                        charclass = kList[rand];
                        index = kPartList[rand2];

                        return data;
                    }
                }
                // 없으면 임시 리스트에서 삭제하고 다시 반복
                kPartList.Remove(kPartList[rand2]);
            }

            kList.Remove(kList[rand]);
		}

        return null;
    }
}
