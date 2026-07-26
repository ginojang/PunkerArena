using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class TriggerSystem : MonoBehaviour
{
    public static TriggerSystem Instance = null;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
    }

    public void CharacterBuffTriggerCheck()
    {
        Messenger.Broadcast(Definition.BattleManagerInvokeAction);
    }


    public bool CheckCondition()
    {
        bool checkCondition = false;

        return checkCondition;
    }
    private bool ReturnCondition(Condition_Type buffCondition, float value)
    {
        bool condition = false;

        return condition;
    }

    private void OnDestroy()
    {

    }
}