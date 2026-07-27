using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using System;

public class GridManager : MonoBehaviour
{
    private enum State
    {
        None,
        SkillSelected,
        FirstTouch,
        SecondTouch,
        AIGrid,
        DrawGrid,
    }

    [SerializeField] GameObject characterPosition = null;
    private Grid[,] allyGrid = new Grid[3,3];
    private Grid[,] enemyGrid = new Grid[3,3];
    private int[,] tempDrawGrid = new int[3, 3];
    private int drawVertical = 0;
    private int drawHorizontal = 0;

    private bool touchOn = false;
    private Ray ray;
    private RaycastHit hit;
    private Grid currentTarget = null;

    private State currentState = State.None;
    // 더 세분화?
    private List<Action> skillSelected = new List<Action>();
    private List<Action> firstTouch = new List<Action>();
    private List<Action> secondTouch = new List<Action>();
    private List<Action> aiGrid = new List<Action>();
    private List<Action> drawGrid = new List<Action>();
    private int actionIndex = 0;


    private void Awake()
    {
        InitailizeData();
    }

    private void Update()
    {
        if(touchOn == true)
        {
            if(Input.GetMouseButtonDown(0))
            {
                if(!EventSystem.current.IsPointerOverGameObject())
                {
                    ray = Camera.main.ScreenPointToRay(Input.mousePosition);

                    if(Physics.Raycast(ray, out hit))
                    {
                        var obj = hit.transform.gameObject;
                        Grid grid = obj.GetComponentInParent<Grid>();
                        if(grid != null)
                        {
                            if (currentTarget == grid)
                            {
                                StartSecondTouch();
                            }
                            else
                            {
                                if (grid.TouchOn == false)
                                    return;

                                currentTarget = grid;
                                StartFirstTouch();
                            }
                        }
                    }
                }
            }
        }
    }
    private void OnDestroy()
    {
        MessageRemoveListner();
    }
    private void MessageAddListner()
    {
        Messenger.AddListener<Camp, int, GameObject>(Definition.SetPosition, SetPosition);
        Messenger.AddListener(Definition.SkillSelect, SkillSelect);
        Messenger.AddListener(Definition.StartAIGrid, StartAIGrid);
    }
    private void MessageRemoveListner()
    {
        Messenger.RemoveListener<Camp, int, GameObject>(Definition.SetPosition, SetPosition);
        Messenger.RemoveListener(Definition.SkillSelect, SkillSelect);
        Messenger.RemoveListener(Definition.StartAIGrid, StartAIGrid);
    }
    private void InitailizeData()
    {
        touchOn = false;
        var ally = characterPosition.transform.Find("Player").gameObject.GetComponentsInChildren<Grid>();
        var enemy = characterPosition.transform.Find("Monster").gameObject.GetComponentsInChildren<Grid>();

        for(int i = 0; i < allyGrid.GetLength(0); i++)
        {
            for(int j = 0; j < allyGrid.GetLength(1); j++)
            {
                allyGrid[i, j] = ally[(i * 3) + j];
                allyGrid[i, j].InitializeGrid(Camp.Ally);
            }
        }

        for (int i = 0; i < enemyGrid.GetLength(0); i++)
        {
            for (int j = 0; j < enemyGrid.GetLength(1); j++)
            {
                enemyGrid[i, j] = enemy[(i * 3) + j];
                enemyGrid[i, j].InitializeGrid(Camp.Enemy);
            }
        }

        MessageAddListner();
        SetSkillSeletedAction();
        SetFirstTouchAction();
        SetSecondTouchAction();
        SetDrawGridAction();
        SetAIAction();
    }

    #region Skill Selected
    private void SkillSelect()
    {
        StartCoroutine(StartActionState(State.SkillSelected));
    }
    private void SetSkillSeletedAction()
    {
        skillSelected.Add(GridOn);
    }
   
    private void GridOn()
    {
        Camp currentCharacterCamp = InGameData.Instance.CurrentTurnCharacter.CharacterInfo.BattleCamp;
        Type_Target skillCamp = InGameData.Instance.CurrentSkillData.type_target;

        switch(currentCharacterCamp)
        {
            case Camp.Ally:
                switch (skillCamp)
                {
                    case Type_Target.Ally:
                        Messenger.Broadcast(Definition.GridTouchOn, Camp.Ally);
                        break;
                    case Type_Target.Enemey:
                        Messenger.Broadcast(Definition.GridTouchOn, Camp.Enemy);
                        break;
                    case Type_Target.Self:
                        break;
                }
                break;
            case Camp.Enemy:
                switch (skillCamp)
                {
                    case Type_Target.Ally:
                        Messenger.Broadcast(Definition.GridTouchOn, Camp.Enemy);
                        break;
                    case Type_Target.Enemey:
                        Messenger.Broadcast(Definition.GridTouchOn, Camp.Ally);
                        break;
                    case Type_Target.Self:
                        break;
                }
                break;
        }


        StartAction();
    }
    #endregion

    #region SetTarget
  
    private void SetTargetPosition()
    {
        InGameData.Instance.SkillMovePosition = currentTarget.gameObject.transform;
    }
    #endregion

    #region First Touch
    private void StartFirstTouch()
    {
        StartCoroutine(StartActionState(State.FirstTouch));
    }
    private void SetFirstTouchAction()
    {
        firstTouch.Add(SetTargetNextTurnOff);
        firstTouch.Add(GridTargetOff);
        firstTouch.Add(SetGridData);
        firstTouch.Add(GridTargetOn);
        firstTouch.Add(SetTargetNextTurn);
    }
    private void SetTargetNextTurnOff()
    {
        CharacterBase prevChar = InGameData.Instance.CurrentTargetCharacter;
        if (prevChar != null)
        {
            Messenger.Broadcast(Definition.SetNextTurnUIOff, prevChar.Profile);
        }   

        StartAction();
    }
    private void GridTargetOff()
    {
        Messenger.Broadcast(Definition.GridTargetOff);
        StartAction();
    }
    private void SetGridData()
    {
        CharacterBase charbase = currentTarget.GetComponentInChildren<CharacterBase>();

        if (charbase != null)
            InGameData.Instance.CurrentTargetCharacter = charbase;

        StartAction();
    }
    private void GridTargetOn()
    {
        currentTarget.CasterOn();
        StartAction();
    }

    private void SetTargetNextTurn()
    {
        Messenger.Broadcast(Definition.SetTargetNextTurn);
        StartAction();
    }

    #endregion

    #region Second Touch
    private void StartSecondTouch()
    {
        StartCoroutine(StartActionState(State.SecondTouch));
    }
    private void SetSecondTouchAction()
    {
        secondTouch.Add(SetSkillButtonDisable);
        secondTouch.Add(InitailizeTouchData);
        secondTouch.Add(SetTarget);
    }
    private void SetSkillButtonDisable()
    {
        Messenger.Broadcast(Definition.SetSkillButtonInteractiveFalse);
        StartAction();
    }
    private void InitailizeTouchData()
    {
        StartAction();
    }
    private void SetTarget()
    {
        List<CharacterBase> temp = InGameData.Instance.TargetList;
        temp.Clear();

        Grid[,] tempGrid = null;
        Dictionary<CharacterBase, CharacterState> dic = null;
        switch (currentTarget.GridCamp)
        {
            case Camp.Ally:
                tempGrid = allyGrid;
                dic = InGameData.Instance.AllyList;
                break;
            case Camp.Enemy:
                tempGrid = enemyGrid;
                dic = InGameData.Instance.EnemyList;
                break;
        }

        for (int i = 0; i < tempGrid.GetLength(0); i++)
        {
            for(int j = 0; j < tempGrid.GetLength(1); j++)
            {
                if (tempGrid[i, j].SkillTarget == true)
                {
                    CharacterBase character = tempGrid[i, j].GetComponentInChildren<CharacterBase>();
                    if (character == null) continue; // [FIX] 죽어서 비활성된 셀 스킵
                    CharacterState state = null;

                    dic.TryGetValue(character, out state);
                    if (state == null) continue; // [FIX] 리스트에 없는(죽은) 대상 스킵
                    if (state.TargetLock == Type_Target_Lock.Impossible)
                        continue;
                    else
                        InGameData.Instance.TargetList.Add(character);
                }
            }
        }

        SetTargetPosition();

        currentTarget = null;
        StartAction();
    }
    #endregion

    #region AI
    private void StartAIGrid()
    {
        StartCoroutine(StartActionState(State.AIGrid));
    }
    private void SetAIAction()
    {
        aiGrid.Add(FindTargetCharacter);
        aiGrid.Add(SetGridData);
        aiGrid.Add(GridOn);
        aiGrid.Add(GetCenterPosition);
        aiGrid.Add(GetSkillDrawGrid);
        aiGrid.Add(DrawGrid);
        aiGrid.Add(SetTargetNextTurn);
    }

    private void FindTargetCharacter()
    {
        CharacterBase target = InGameData.Instance.CurrentTargetCharacter;

        Grid[,] temp = null;
        switch (target.CharacterInfo.BattleCamp)
        {
            case Camp.Ally:
                temp = allyGrid;
                break;
            case Camp.Enemy:
                temp = enemyGrid;
                break;
        }

        for(int i = 0; i < temp.GetLength(0); i++)
        {
            for (int j = 0; j < temp.GetLength(1); j++)
            {
                CharacterBase character = temp[i, j].GetComponentInChildren<CharacterBase>();
                if (target == character)
                {
                    currentTarget = temp[i, j];
                    break;
                }
            }

            if (currentTarget != null)
                break;
        }

        StartAction();
    }

    #endregion

    #region Action
    private IEnumerator StartActionState(State state)
    {
        touchOn = false;
        yield return new WaitUntil(() => currentState == State.None);

        currentState = state;
        StartAction();
    }
    private void StartAction()
    {
        if (currentState == State.None)
            return;

        List<Action> actionList = GetCurrentAction();
        if (actionIndex >= actionList.Count)
        {
            switch(currentState)
            {
                case State.FirstTouch:
                    touchOn = true;

                    StartDrawGrid();
                    break;
                case State.SecondTouch:
                    Messenger.Broadcast(Definition.BattleManagerInvokeAction);
                    break;
                case State.AIGrid:
                    StartSecondTouch();
                    break;
                case State.DrawGrid:
                    touchOn = true;

                    break;
                case State.SkillSelected:
                    touchOn = true;

                    if (currentTarget)
                        StartFirstTouch();
                    break;
            }

            actionIndex = 0;
            currentState = State.None;
            return;
        }

        Action action = actionList[actionIndex];
        actionIndex++;

        action.Invoke();
    }
    private List<Action> GetCurrentAction()
    {
        List<Action> actionList = null;
        switch (currentState)
        {
            case State.FirstTouch:
                actionList = firstTouch;
                break;
            case State.SecondTouch:
                actionList = secondTouch;
                break;
            case State.AIGrid:
                actionList = aiGrid;
                break;
            case State.DrawGrid:
                actionList = drawGrid;
                break;
            case State.SkillSelected:
                actionList = skillSelected;
                break;
        }
        return actionList;
    }
    #endregion

    #region SetPosition
    private void SetPosition(Camp _camp, int _slotIdx, GameObject _characterObj)
    {
        int position = _slotIdx / 3;
        int slot = _slotIdx % 3;

        CharacterBase owner = _characterObj.GetComponent<CharacterBase>();

        switch(_camp)
        {
            case Camp.Ally:
                allyGrid[position, slot].SetPosition(owner);
                break;
            case Camp.Enemy:
                enemyGrid[position, slot].SetPosition(owner);
                break;
        }
    }
    #endregion

    #region Skill Grid
    private void SetDrawGridAction()
    {
        drawGrid.Add(GetCenterPosition);
        drawGrid.Add(GetSkillDrawGrid);
        drawGrid.Add(DrawGrid);
    }
    private void StartDrawGrid()
    {
        StartCoroutine(StartActionState(State.DrawGrid));
    }
    private void DrawGrid()
    {
        Grid[,] grid = GetGridArray();
        for (int i = 0; i < tempDrawGrid.GetLength(0); i++)
        {
            int verticalindex = i + drawVertical;
            if (verticalindex < 0 || verticalindex >= tempDrawGrid.GetLength(0))
                continue;

            for(int j = 0; j < tempDrawGrid.GetLength(1); j++)
            {
                int horizontalindex = j + drawHorizontal;
                if (horizontalindex < 0 || horizontalindex >= tempDrawGrid.GetLength(1))
                    continue;

                int caseOf = tempDrawGrid[i, j];
                var obj = grid[i + drawVertical, j + drawHorizontal];
                switch (caseOf)
                {
                    case 0:
                        
                        break;
                    case 1:
                        obj.GridTargetOn();
                        break;
                    case 2:
                        obj.GridTargetOn();
                        break;
                }
            }
        }

        StartAction();
    }
    private void GetSkillDrawGrid()
    {
        var data = InGameData.Instance.CurrentSkillData;
        string radius = data.type_radius;

        string[] position = radius.Split('/');
        
        for(int i = 0; i < position.Length; i++)
        {
            string[] split = position[i].Split('+');
            for(int j = 0; j < split.Length; j++)
            {
                int on = int.Parse(split[j]);
                tempDrawGrid[i,j] = on;
            }
        }

        StartAction();
    }
    private void GetCenterPosition()
    {
        Grid[,] grid = GetGridArray();
        string data = InGameData.Instance.CurrentSkillData.type_radius;

        string[] position = data.Split('/');

        int[] vertical = FindCenter(position);

        int basicVertical = -vertical[0];
        int basicHorizontal = -vertical[1];
        for(int i = 0; i < grid.GetLength(0); i++)
        {
            for(int j = 0; j < grid.GetLength(1); j++)
            {
                if (grid[i, j] != currentTarget)
                    continue;

                drawVertical = i + basicVertical;
                drawHorizontal = j + basicHorizontal;
                break;
            }
        }
        
        StartAction();
    }
    private int[] FindCenter(string[] data)
    {
        int[] center = new int[2];
        int on = 0;
        for (int i = 0; i < data.Length; i++)
        {
            string[] vertical = data[i].Split('+');
            for (int j = 0; j < vertical.Length; j++)
            {
                on = int.Parse(vertical[j]);
                if (on == 2)
                {
                    center[1] = j;
                    break;
                }
            }
            if (on == 2)
            {
                center[0] = i;
                break;
            }
        }


        return center;
    }

    private Grid[,] GetGridArray()
    {
        Grid[,] grid = null;
        switch (currentTarget.GridCamp)
        {
            case Camp.Ally:
                grid = allyGrid;
                break;
            case Camp.Enemy:
                grid = enemyGrid;
                break;
        }

        return grid;
        //StartAction();
    }
    #endregion

}
