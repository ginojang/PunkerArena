using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Generated.CsvData;
using System;
using Generated.CsvData;

public class StackInfo
{
    Skill_StackData data;

    public void InitData(Skill_StackData _data)
    {
        data = _data;

        InsertStack();
    }

    private void InsertStack()
    {
        string atlas = data.res_atlas;
        string icon = data.res_icon;
    }
}

public class StackManager : MonoBehaviour
{
    private Dictionary<CharacterBase, StackInfo> characterStackDic;
    private Skill_StackTable table;

    private List<Action> actionList;
    private int actionIndex;
    private void InitData()
    {
        table = CSVDataManager.GetTable<Skill_StackTable>();
        actionList = new List<Action>();
        actionIndex = 0;
        characterStackDic = new Dictionary<CharacterBase, StackInfo>();
    }

    private void InsertStack(int stackId)
    {
        Skill_StackData data = table.GetData(stackId);
        

    }
    private void CheckUseStack()
    {

    }
}
