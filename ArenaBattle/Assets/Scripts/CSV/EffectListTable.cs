using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Generated.CsvData
{
    public class effectlistdata
    {
        public int idx;
        public int total;
        public int[] effectarray;

        public void Load(Dictionary<string, string> data)
        {
            CSVTableHelper.SetValue(ref idx, data["idx"]);
            CSVTableHelper.SetValue(ref total, data["total"]);

            effectarray = new int[total];
            for(int i = 0; i < effectarray.Length; i++)
            {
                CSVTableHelper.SetValue(ref effectarray[i], data[$"effectidx{i+1}"]);
            }
        }
    }
}

public class EffectListTable : IDefTable
{
    private Dictionary<int, Generated.CsvData.effectlistdata> _dicData = new Dictionary<int, Generated.CsvData.effectlistdata>();
    public Dictionary<int, Generated.CsvData.effectlistdata> DicData { get { return _dicData; } }

    public override void SetData(List<Dictionary<string, string>> csvTable)
    {
        for(int i = 0; i < csvTable.Count; i++)
        {
            Generated.CsvData.effectlistdata data = new Generated.CsvData.effectlistdata();
            data.Load(csvTable[i]);

            if(_dicData.ContainsKey(data.idx))
            {
                Debug.LogErrorFormat("{0} 테이블 {1}인덱스 중복!!", GetType(), data.idx);
                continue;
            }

            _dicData.Add(data.idx, data);
        }

    }

    public int[] GetData(int index)
    {
        Generated.CsvData.effectlistdata data;

        if(_dicData.TryGetValue(index, out data) == false)
            Debug.LogError(string.Format("{0} Table {1} Index Find Failed", GetType(), index));

        if (data == null)
            return null;

        return data.effectarray;
    }
}
