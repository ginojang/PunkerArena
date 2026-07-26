using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MonsterLove.StateMachine;

public interface ISubMachine
{
	
}
public enum SUB_ACTION_MELEE
{
	ActionStart,
	RunMoveForward,
	MeleeAction,
	RunMoveBack,
	ActionEnd,
}
public enum SUB_ACTION_CENTERMELEE
{
	ActionStart,
	RunMoveCenter,
	MeleeAction,
	RunMoveBack,
	ActionEnd,
}
public enum SUB_ACTION_RANGE
{
	ActionStart,
	RangeActionDirect,
	ActionEnd,
}
public enum SUB_ACTION_PARABOLARANGE
{
	ActionStart,
	RangeActionParabola,
	ActionEnd,
}

public enum SUB_ACTION_INTRO
{
	ActionStart,
	RunMoveBack,
	BattleDataSetting,
	ActionEnd,
}
public enum SUB_ACTION_ONDAMAGE
{
	ActionStart,
	OnDamage,
	Death,
	ActionEnd,
}



public class Character_SubAction<T> where T : struct, IConvertible, IComparable
{
	StateMachine<T> sub_fsm;
	CharacterBase charBase;
	T tType = new T();

	public StateMachine<T> SUB_FSM
	{
		get { return sub_fsm; }
	}

	public Character_SubAction(CharacterBase charbase, T value)
	{
		//Type code = tType.GetType();
		charBase = charbase;
		sub_fsm = StateMachine<T>.Initialize(charbase, value);
	}

	public bool ChangeNextState()
	{
		bool bres = true;

		if (typeof(T).IsEnum)
		{
			bres = sub_fsm.ChangeNextState();
		}

		return bres;
	}
}
