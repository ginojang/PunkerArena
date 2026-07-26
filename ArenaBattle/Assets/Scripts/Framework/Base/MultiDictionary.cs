using System.Collections;
using System.Collections.Generic;

public class MultiDictionary<Key, Value>
{
	private readonly Dictionary<Key, List<Value>> _dict = new Dictionary<Key, List<Value>>();

	public void Add(Key key, Value value)
	{
		List<Value> list;
		if (_dict.TryGetValue(key, out list))
		{
			list.Add(value);
		}
		else
		{
			list = new List<Value> { value };
			_dict.Add(key, list);
		}
	}

	public void RemoveList(Key key)
	{
		_dict.Remove(key);
	}

	public void Remove(Key key, Value value)
	{
		List<Value> list;
		if (_dict.TryGetValue(key, out list))
		{
			list.Remove(value);
		}
	}

	public void Clear()
	{
		_dict.Clear();
	}

	public bool ContainsKey(Key key)
	{
		return _dict.ContainsKey(key);
	}

	public List<Value> this[Key key]
	{
		get
		{
			List<Value> list;
			if (!_dict.TryGetValue(key, out list))
			{
				list = new List<Value>();
				_dict.Add(key, list);
			}

			return list;
		}
	}

	public IEnumerable<Key> Keys
	{
		get
		{
			return _dict.Keys;
		}
	}
}