using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Generated.CsvData
{
    public class DinoStatusData
    {
        public int idx;

        public Dictionary<DinoStatus, dinoStatusValue> dinoStatusDic = new Dictionary<DinoStatus, dinoStatusValue>();

        public void Load(Dictionary<string, string> data)
        {
            CSVTableHelper.SetValue(ref idx, data["idx"]);

            dinoStatusValue str = new dinoStatusValue();
            CSVTableHelper.SetValue(ref str.statusOption, data["dino_str"]);
            CSVTableHelper.SetValue(ref str.value, data["dino_str_value"]);
            dinoStatusDic.Add(DinoStatus.Str, str);

            dinoStatusValue vit = new dinoStatusValue();
            CSVTableHelper.SetValue(ref vit.statusOption, data["dino_vit"]);
            CSVTableHelper.SetValue(ref vit.value, data["dino_vit_value"]);
            dinoStatusDic.Add(DinoStatus.Vit, vit);

            dinoStatusValue dex = new dinoStatusValue();
            CSVTableHelper.SetValue(ref dex.statusOption, data["dino_dex"]);
            CSVTableHelper.SetValue(ref dex.value, data["dino_dex_value"]);
            dinoStatusDic.Add(DinoStatus.Dex, dex);

            dinoStatusValue agi = new dinoStatusValue();
            CSVTableHelper.SetValue(ref agi.statusOption, data["dino_agi"]);
            CSVTableHelper.SetValue(ref agi.value, data["dino_agi_value"]);
            dinoStatusDic.Add(DinoStatus.Agi, agi);

            dinoStatusValue luk = new dinoStatusValue();
            CSVTableHelper.SetValue(ref luk.statusOption, data["dino_luk"]);
            CSVTableHelper.SetValue(ref luk.value, data["dino_luk_value"]);
            dinoStatusDic.Add(DinoStatus.Luk, luk);
        }
    }
}

public class DinoStatusTable : IDefTable
{
    private Dictionary<int, Generated.CsvData.DinoStatusData> _dicData = new Dictionary<int, Generated.CsvData.DinoStatusData>();
    public Dictionary<int, Generated.CsvData.DinoStatusData> DicData { get { return _dicData; } }

    public override void SetData(List<Dictionary<string, string>> csvTable)
    {
        for(int i = 0; i < csvTable.Count; i++)
        {
            Generated.CsvData.DinoStatusData data = new Generated.CsvData.DinoStatusData();
            data.Load(csvTable[i]);

            if(_dicData.ContainsKey(data.idx))
            {
                Debug.LogErrorFormat("{0} 테이블 {1}인덱스 중복!!", GetType(), data.idx);
                continue;
            }

            _dicData.Add(data.idx, data);
        }
    }

    public Generated.CsvData.DinoStatusData GetData(int index)
    {
        Generated.CsvData.DinoStatusData data;

        if (_dicData.TryGetValue(index, out data) == false)
            Debug.LogError(string.Format("{0} Table {1} Index Find Failed", GetType(), index));

        return data;
    }

    public dinoStatusValue GetStatusValue(int index, DinoStatus status)
    {
        Generated.CsvData.DinoStatusData data;
        _dicData.TryGetValue(index, out data);

        dinoStatusValue value;

        data.dinoStatusDic.TryGetValue(status, out value);

        return value;
    }
}
