using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class dinoPartsData : dinoPartsBaseData
{
	[System.Serializable]
	public class dinoData : BaseData
	{
		public string itemResourcePath;
		public Vector2Int itemResourceSize;
		public Vector2Int itemResourceCenter;
		public Vector2Int dustDensity;

		public dinoData()
		{
			stepUid = 0;
			minigameId = string.Empty;
			itemResourcePath = string.Empty;
			itemResourceSize = Vector2Int.zero;
			itemResourceCenter = Vector2Int.zero;
			dustDensity = Vector2Int.one;
		}
	}

//	public int testEpsodeID;
//	public int testActionID;

	public List<dinoData> datas = new List<dinoData>();

	public override int AddData(BaseData _data)
	{
		datas.Add((dinoData)_data);
		return datas.Count - 1;
	}

	public override BaseData GetDataByIndex(int _index)
	{
		if (datas.Count > _index)
			return datas[_index];
		return null;
	}

	public override void DeleteData(BaseData _data)
	{
		datas.Remove((dinoData)_data);
	}

	public override void DeleteData(int _index)
	{
		datas.RemoveAt(_index);
	}

	public override int Count()
	{
		return datas.Count;
	}

	public override BaseData GetData(uint stepUid)
	{
		return datas.Find(d => d.stepUid == stepUid);
	}
}
