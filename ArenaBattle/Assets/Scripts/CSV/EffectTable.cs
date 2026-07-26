using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Generated.CsvData
{
    public class effectData
    {
        public string idx;
        public Fx_Projectile_Type projectile_type;
        public Link_Bone_Move link_bone_move;
        public string active_pos_type;
        public string correct_pos;

        public void Load(Dictionary<string, string> data)
        {
            CSVTableHelper.SetValue(ref idx, data["idx"]);
            CSVTableHelper.SetValue(ref projectile_type, data["projectile_type"]);
            CSVTableHelper.SetValue(ref link_bone_move, data["link_bone_move"]);
            CSVTableHelper.SetValue(ref active_pos_type, data["active_pos_type"]);
            CSVTableHelper.SetValue(ref correct_pos, data["correct_pos"]);
        }
    }
}

public class EffectTable : IDefTable
{
    private Dictionary<string, Generated.CsvData.effectData> _dicData = new Dictionary<string, Generated.CsvData.effectData>();
    public Dictionary<string, Generated.CsvData.effectData> DicData { get { return _dicData; } }

    override public void SetData(List<Dictionary<string, string>> csvTable)
    {
        for (int i = 0; i < csvTable.Count; ++i)
        {
            Generated.CsvData.effectData data = new Generated.CsvData.effectData();
            data.Load(csvTable[i]);

            if (_dicData.ContainsKey(data.idx))
            {
                Debug.LogErrorFormat("{0} 테이블 {1}인덱스 중복!!", GetType(), data.idx);
                continue;
            }
            _dicData.Add(data.idx, data);
        }
    }

    public Generated.CsvData.effectData GetData(string effectName)
    {
        Generated.CsvData.effectData data;

        if (_dicData.TryGetValue(effectName, out data) == false)
            Debug.LogError(string.Format("{0} Table {1} Index Find Failed", GetType(), effectName));

        return data;
    }

}
