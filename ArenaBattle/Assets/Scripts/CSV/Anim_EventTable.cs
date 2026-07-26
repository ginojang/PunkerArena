using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using System.Text;
using System.IO;

namespace Generated.CsvData
{
    public class AnimEventData
	{
        public float fTime;
        public string eventName;
        public int effectarrayidx;
        public int effecttarget;
        public object param;
    }

    public class eventData
    {
        public int index;
        public string aniName;
        public int eventCount;
        public float aniLenght;

        public List<AnimEventData> eventList = new List<AnimEventData>();

        public void Load(Dictionary<string, string> data)
        {
            CSVTableHelper.SetValue(ref index, data["idx"]);
            CSVTableHelper.SetValue(ref aniName, data["ani_name"]);
            CSVTableHelper.SetValue(ref eventCount, data["eventcount"]);

            for (int i = 0; i < eventCount; i++)
			{
                AnimEventData eventinfo = new AnimEventData();

                if (data.ContainsKey($"event{i+1}_time"))
				{
                    CSVTableHelper.SetValue(ref eventinfo.fTime, data[$"event{i+1}_time"]);
                    CSVTableHelper.SetValue(ref eventinfo.eventName, data[$"event{i+1}_name"]);
                    CSVTableHelper.SetValue(ref eventinfo.effectarrayidx, data[$"effectarrayidx{i + 1}"]);
                    CSVTableHelper.SetValue(ref eventinfo.effecttarget, data[$"effecttarget{i + 1}"]);
                }

                eventList.Add(eventinfo);
            }
        }
    }
}

public class Anim_EventTable : IDefTable
{
    private Dictionary<string, Generated.CsvData.eventData> _dicDataList = new Dictionary<string, Generated.CsvData.eventData>();

    override public void SetData(List<Dictionary<string, string>> csvTable)
    {
        Dictionary<string, Generated.CsvData.eventData> datas = new Dictionary<string, Generated.CsvData.eventData>();

        for (int i = 0; i < csvTable.Count; ++i)
        {
            Generated.CsvData.eventData data = new Generated.CsvData.eventData();

            data.Load(csvTable[i]);

            if (!_dicDataList.ContainsKey(data.aniName))
            {
                _dicDataList.Add(data.aniName, data);
            }
            else
			{
                Debug.LogError($"{data.aniName} is Exist");
                continue;
            }
        }
    }

    public Generated.CsvData.eventData GetData(string aniname)
    {
        Generated.CsvData.eventData data = null;

        if (_dicDataList.ContainsKey(aniname))
		{
            data = _dicDataList[aniname];
		}

        return data;
    }

    // Tool 에서 변경한 값을 변경
    public void ChangeValue(Generated.CsvData.eventData anidata)
    {
        Generated.CsvData.eventData tbldata = GetData(anidata.aniName);

        tbldata.eventList = anidata.eventList;

        tbldata.eventCount = tbldata.eventList.Count;
    }
    
    // 현재의 값들을 CSV 파일로 다시 저장
    public void SaveTable(string path)
    {
        List<string[]> rowData = new List<string[]>();
        
        int eventMax = 0;
        foreach (var data in _dicDataList)
        {
            if (data.Value.eventCount > eventMax)
                eventMax = data.Value.eventCount;
        }

        List<string> csvHead = new List<string>();
        csvHead.Add("idx");
        csvHead.Add("ani_name");
        csvHead.Add("eventcount");

        for (int i = 0; i < eventMax; i++)
        {
            csvHead.Add($"event{i+1}_time");
            csvHead.Add($"event{i+1}_name");
            csvHead.Add($"effectarrayidx{i+1}");
            csvHead.Add($"effecttarget{i+1}");
        }

        string[] headStrings = csvHead.ToArray();
        
        rowData.Add(headStrings);

        foreach (var data in _dicDataList)
        {
            List<string> coltemp = new List<string>();
            coltemp.Add(data.Value.index.ToString());
            coltemp.Add(data.Value.aniName);
            coltemp.Add(data.Value.eventCount.ToString());
            for (int i = 0; i < eventMax; i++)
            {
                if (i >= data.Value.eventCount)
                {
                    coltemp.Add("");
                    coltemp.Add("");
                    coltemp.Add("");
                    coltemp.Add("");
                }
                else
                {
                    coltemp.Add(data.Value.eventList[i].fTime.ToString());
                    coltemp.Add(data.Value.eventList[i].eventName);
                    coltemp.Add(data.Value.eventList[i].effectarrayidx.ToString());
                    coltemp.Add(data.Value.eventList[i].effecttarget.ToString());
                }
            }

            string[] colarray = coltemp.ToArray();
            
            rowData.Add(colarray);
        }
        
        string[][] output = new string[rowData.Count][];

        for (int i = 0; i < output.Length; i++)
        {
            output[i] = rowData[i];
        }
        int     length         = output.GetLength(0);
        string     delimiter     = ",";

        StringBuilder sb = new StringBuilder();

        for (int index = 0; index < length; index++)
            sb.AppendLine(string.Join(delimiter, output[index]));

        StreamWriter outStream = System.IO.File.CreateText(path);
        outStream.WriteLine(sb);
        outStream.Close();
    }
}
