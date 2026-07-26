using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Generated.CsvData
{
    public class stageMonsterData
    {
        public int stageidx;
        public int name_string;
        public MonsterGrade monster_grade;
        public int stagetype;
        public string desc;
        public int monster;
        public CharacterArrangement batch01;
        public int batch02;
        public string monsterprefab;
        public CharacterAttribute starttime;
        public int starttimeidx;

        public void Load(Dictionary<string, string> data)
        {
            CSVTableHelper.SetValue(ref stageidx, data["stageidx"]);
            CSVTableHelper.SetValue(ref name_string, data["name_string"]);
            CSVTableHelper.SetValue(ref monster_grade, data["monster_grade"]);
            CSVTableHelper.SetValue(ref stagetype, data["stagetype"]);
            CSVTableHelper.SetValue(ref desc, data["desc"]);
            CSVTableHelper.SetValue(ref monster, data["monster"]);
            CSVTableHelper.SetValue(ref batch01, data["batch01"]);
            CSVTableHelper.SetValue(ref batch02, data["batch02"]);
            CSVTableHelper.SetValue(ref monsterprefab, data["monsterprefab"]);
            CSVTableHelper.SetValue(ref starttime, data["starttime"]);
            CSVTableHelper.SetValue(ref starttimeidx, data["starttimeidx"]);
        }
    }
}

public class StageMonsterTable : IDefTable
{
    public class stageData
    {
        public CharacterAttribute startTime;
        public int timeIndex;
    }
    
    private Dictionary<int, List<Generated.CsvData.stageMonsterData>> _dicData = new Dictionary<int, List<Generated.CsvData.stageMonsterData>>();
    public Dictionary<int, List<Generated.CsvData.stageMonsterData>> DicData { get { return _dicData; } }
    
    public Dictionary<int, stageData> _stageDicData = new Dictionary<int, stageData>();

    public override void SetData(List<Dictionary<string, string>> csvTable)
    {
        for(int i = 0; i < csvTable.Count; i++)
        {
            Generated.CsvData.stageMonsterData data = new Generated.CsvData.stageMonsterData();
            data.Load(csvTable[i]);

            if(_dicData.ContainsKey(data.stageidx))
            {
                List<Generated.CsvData.stageMonsterData> temp = null;
                _dicData.TryGetValue(data.stageidx, out temp);
                temp.Add(data);
            }
            else
            {
                List<Generated.CsvData.stageMonsterData> temp = new List<Generated.CsvData.stageMonsterData>();
                temp.Add(data);
                _dicData.Add(data.stageidx, temp);
            }

            if (_stageDicData.ContainsKey(data.stageidx) == false)
            {
                stageData newData = new stageData();
                newData.startTime = data.starttime;
                newData.timeIndex = data.starttimeidx;
                
                _stageDicData.Add(data.stageidx, newData);
            }
        }
    }
    public List<Generated.CsvData.stageMonsterData> GetData(int stageIndex)
    {
        if (_dicData.ContainsKey(stageIndex) == false)
        {
            Debug.LogError("Stage Info None");
            return null;
        }

        List<Generated.CsvData.stageMonsterData> temp = null;

        _dicData.TryGetValue(stageIndex, out temp);

        if (temp == null)
        {
            Debug.LogError("Stage Info None");
            return null;
        }

        return temp;
    }
}
