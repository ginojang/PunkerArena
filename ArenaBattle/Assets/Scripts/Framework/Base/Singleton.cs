using UnityEngine;

using System;
using System.Reflection;
using System.Collections.Generic;


public static class SingletonCollect
{
	private static readonly List<System.Object> _singleton_list = new List<System.Object>();

	public static void Add(System.Object singleton)
	{
		//if (_singleton_list.Contains(singleton) == false)
		//{
		//    _singleton_list.Add(singleton);
		//    TsLog.Log(string.Format("[MemTrace] Singleton_CreateInstance : {0}({1})", singleton.GetType().Name, _singleton_list.Count.ToString()));
		//}
	}

	public static void Destory()
	{
		foreach (System.Object obj in _singleton_list)
		{
			IDisposable disposable = obj as IDisposable;
			if (disposable != null)
			{
				disposable.Dispose();
				Debug.Log(string.Format("[MemTrace] Singleton_DestoryInstance {0}", disposable.GetType().Name));
			}
			else
			{
				Debug.Log(string.Format("[MemTrace] Singleton_NonDestoryInstance {0}", obj.GetType().Name));
			}
		}

		_singleton_list.Clear();
		GC.Collect();
	}
}

public abstract class Singleton<T> where T : class
{
	public static T Instance
	{
		get { return SingletonAllocator.InstanceObject; }
	}

	public void Destory()
	{
		SingletonAllocator.InstanceObject = null;
	}

	private static class SingletonAllocator
	{
// ReSharper disable StaticFieldInGenericType
		internal static T InstanceObject;
// ReSharper restore StaticFieldInGenericType

		static SingletonAllocator()
		{
			_CreateInstance(typeof(T));
			_Initialize(typeof(T));
		}


		private static void _CreateInstance(Type type)
		{
			ConstructorInfo ctor_private = type.GetConstructor(
				BindingFlags.Instance | BindingFlags.NonPublic, null, new Type[0], new ParameterModifier[0]);

			ConstructorInfo[] ctor_public = type.GetConstructors(
				BindingFlags.Instance | BindingFlags.Public);

			if (ctor_public.Length > 0)
			{
				throw new Exception(
					type.FullName + " has one more public constructors so the property cannot be enforced.");
			}

			if (null == ctor_private)
			{
				throw new Exception(
					type.FullName + " doesn't have a private/protected constructor so the property cannnot be enforced.");
			}

			try
			{
				System.Object createobject = ctor_private.Invoke(new object[0]);
				SingletonCollect.Add(createobject);
				InstanceObject = (T)createobject;
			}
			catch (Exception e)
			{
				throw new Exception(
					"The singleton couldnt be constructed, check if " + type.FullName + " has a default constructor", e);
			}
		}


		private static void _Initialize(Type type)
		{
			MethodInfo method_initialize = type.GetMethod("Initialize");
			if (null == method_initialize) return;
			method_initialize.Invoke(InstanceObject, null);
		}
	}
}
