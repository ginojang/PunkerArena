using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;

public abstract class dinoPartsBaseData : ScriptableObject
{
	[System.Serializable]
	public class BaseData
	{
		public uint stepUid;
		public uint uid;
		public string minigameId;
	}

	public abstract int AddData(BaseData _data);

	public abstract BaseData GetDataByIndex(int _index);
	
	public abstract void DeleteData(BaseData _data);

	public abstract void DeleteData(int _index);

	public abstract int Count();

	public abstract BaseData GetData(uint stepUid);
}
