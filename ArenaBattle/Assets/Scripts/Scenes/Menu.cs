using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Menu : UnityScene
{
	protected override void Awake()
	{
		Debug.Log("################ Menu Awake");
		base.Awake();
	}

	protected override void Start()
	{
		Debug.Log("################ Menu Start");
		base.Start();
	}

	protected override void OnDestroy()
	{
		base.OnDestroy();
	}

	protected override void OnClose()
	{
		base.OnClose();
	}

	protected override void OnPostLoad()
	{
		Debug.Log("################ Menu OnPostLoad");
	}
}
