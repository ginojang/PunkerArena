using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Generated.CsvData
{
    public class Dino_ClassData
    {
        public int idx;
        public string name_string;
        public CharacterClass type;
        public Grade grade;
        public Attributes attribute;
        public CharacterTalent talent;
        public Role role;
        public float str;
        public float vit;
        public float agi;
        public float dex;
        public float luk;
        public float atk;
        public float def;
        public float def_pen;
        public float hp;
        public float hit;
        public float avd;
        public float cri;
        public float cri_dmg;
        public float res;
        public float spd;
        public string res_atlas;
        public string res_icon;

        public void Load(Dictionary<string, string> data)
        {
            CSVTableHelper.SetValue(ref idx, data["idx"]);
            CSVTableHelper.SetValue(ref name_string, data["name_string"]);
            CSVTableHelper.SetValue(ref type, data["type"]);
            CSVTableHelper.SetValue(ref grade, data["grade"]);
            CSVTableHelper.SetValue(ref attribute, data["attribute"]);
            CSVTableHelper.SetValue(ref talent, data["talent"]);
            CSVTableHelper.SetValue(ref role, data["role"]);
            CSVTableHelper.SetValue(ref vit, data["vit"]);
            CSVTableHelper.SetValue(ref agi, data["agi"]);
            CSVTableHelper.SetValue(ref dex, data["dex"]);
            CSVTableHelper.SetValue(ref atk, data["atk"]);
            CSVTableHelper.SetValue(ref def, data["def"]);
            CSVTableHelper.SetValue(ref def_pen, data["def_pen"]);
            CSVTableHelper.SetValue(ref hp, data["hp"]);
            CSVTableHelper.SetValue(ref hit, data["hit"]);
            CSVTableHelper.SetValue(ref avd, data["avd"]);
            CSVTableHelper.SetValue(ref cri, data["cri"]);
            CSVTableHelper.SetValue(ref cri_dmg, data["cri_dmg"]);
            CSVTableHelper.SetValue(ref res, data["res"]);
            CSVTableHelper.SetValue(ref spd, data["spd"]);
            CSVTableHelper.SetValue(ref res_atlas, data["res_atlas"]);
            CSVTableHelper.SetValue(ref res_icon, data["res_icon"]);
        }
    }
}

public class Dino_ClassTable : IDefTable
{
    private Dictionary<int, Generated.CsvData.Dino_ClassData> _dicData = new Dictionary<int, Generated.CsvData.Dino_ClassData>();

    public Dictionary<int, Generated.CsvData.Dino_ClassData> Dic
    {
        get { return _dicData; }
    }

    public override void SetData(List<Dictionary<string, string>> csvTable)
    {
        for (int i = 0; i < csvTable.Count; i++)
        {
            Generated.CsvData.Dino_ClassData data = new Generated.CsvData.Dino_ClassData();
            data.Load(csvTable[i]);

            if (_dicData.ContainsKey(data.idx))
            {
                Debug.LogErrorFormat("{0} 테이블 {1}인덱스 중복!!", GetType(), data.idx);
                continue;
            }

            _dicData.Add(data.idx, data);
        }
    }

    public Generated.CsvData.Dino_ClassData GetData(int index)
    {
        Generated.CsvData.Dino_ClassData data;
        if (_dicData.TryGetValue(index, out data) == false)
            Debug.LogError(string.Format("{0} Table {1} Index Find Failed", GetType(), index));

        return data;
    }
}