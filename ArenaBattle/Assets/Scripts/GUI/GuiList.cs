using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Devil.Gui
{
    [System.Serializable]
	public class GuiList
	{
		public enum LOAD_TYPE { NONE = -1, ADDRESSABLE, RESOURCE, }

        [System.Serializable]
        // GUI Open 상태 및 Depth 관리
        private class GuiInfo
		{
            public string prefabName;
			public GameObject instance;
			public int depth;
			public LOAD_TYPE loadType;
			public bool dontDestroy;

			public GuiInfo(string _prefabName, int _depth) : this(_prefabName, _depth, LOAD_TYPE.ADDRESSABLE, false) { }
			public GuiInfo(string _prefabName, int _depth, LOAD_TYPE _loadType) : this(_prefabName, _depth, _loadType, false) { }
			public GuiInfo(string _prefabName, int _depth, bool _dontDestroy) : this(_prefabName, _depth, LOAD_TYPE.ADDRESSABLE, _dontDestroy) { }
			public GuiInfo(string _prefabName, int _depth, LOAD_TYPE _loadType, bool _dontDestroy) : this(_prefabName, null, _depth, _loadType, _dontDestroy) { }
			public GuiInfo(string _prefabName, GameObject _instance, int _depth, LOAD_TYPE _loadType, bool _dontDestroy)
			{
                prefabName = _prefabName;
                instance = _instance;
				depth = _depth;
				loadType = _loadType;
				dontDestroy = _dontDestroy;
			}
		}

        [SerializeField]
        private List<GuiInfo> list = new List<GuiInfo>();
        private readonly Dictionary<string, GuiInfo> dic = new Dictionary<string, GuiInfo>();


		public GuiList()
		{
		}

        public void CreateDic()
        {
            for (int i = 0; i < list.Count; i++)
                dic.Add(list[i].prefabName, list[i]);
        }

		public bool HasName(string key)
		{
			//Debug.Log(name);
			return dic.ContainsKey(key);
		}

		public void OnFinalize()
		{
			foreach (var the in dic)
			{
				if (the.Value != null && the.Value.instance != null)
				{
					the.Value.instance.SendMessage("OnFinalize", SendMessageOptions.DontRequireReceiver);
				}
			}
		}

		public GameObject GetInstance(string key)
		{
			if (!HasName(key))
			{
				return null;
			}

			return dic[key].instance;
		}

		public void AddInstance(string key, GameObject uiObject)
		{
			if (!HasName(key))
			{
				Debug.Log("no has name in list");
				return;
			}

			if (dic[key].instance != null)
			{
				Debug.Log("Already exist instance");
				return;
			}

			dic[key].instance = uiObject;
			SortingDepth(key);
		}

		public void RemoveInstance(bool isAll)
		{
			foreach (var item in dic)
			{
				if (item.Value == null || item.Value.instance == null) continue;
				if (!isAll && item.Value.dontDestroy) continue;

				Object.Destroy(item.Value.instance);
				item.Value.instance = null;
			}
		}

		public LOAD_TYPE GetLoadType(string _key)
		{
			if (!HasName(_key))
			{
				Debug.LogError("no has name in list");
				return LOAD_TYPE.NONE;
			}

			return dic[_key].loadType;
		}

		// list의 depth 정보를 이용해, UI의 Sibling 값 적용
		private void SortingDepth(string key)
		{
			int minDiff = int.MaxValue;
			int resultSibling = 0;

			foreach (string k in dic.Keys)
			{
				if (key == k || !dic[k].instance) continue;

				//Debug.Log(k + " >> " + list[k].depth);

				// Depth 기준으로 가장 가까운 UI의 정보를 알아옴
				int diff = dic[key].depth - dic[k].depth;
				if (Mathf.Abs(diff) < minDiff)
				{
					if (diff < 0)
					{
						// Low  
						resultSibling = dic[k].instance.transform.GetSiblingIndex();
					}
					else
					{
						// High
						resultSibling = dic[k].instance.transform.GetSiblingIndex() + 1;
					}
					minDiff = Mathf.Abs(diff);
				}
			}

			dic[key].instance.transform.SetSiblingIndex(resultSibling);
		}
	}
}
