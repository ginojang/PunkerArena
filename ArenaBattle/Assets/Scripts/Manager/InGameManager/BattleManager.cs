using System.Collections;
using System.Collections.Generic;
using Generated.CsvData;
using UnityEngine;
using System;
using Devil.Common;
using MonsterLove.StateMachine;

public partial class BattleManager : MonoBehaviour
{
    public enum TRIGGER_FSM
    {
        None = 0,
        Load,
        Intro,
        InitTurn,
        SkillSelect,
        CharacterAction,
        SetRoundUI,
        InitData,
        TurnEnd,
        GameResult,
    }
    private StateMachine<TRIGGER_FSM> fsm;
    private TRIGGER_FSM currentState = TRIGGER_FSM.None;

    [Flags] private enum Character_Behavior
    {
        None = 0,
        Character1 = 1 << 0,
        Character2 = 1 << 1,
        Character3 = 1 << 2,
        Character4 = 1 << 3,
        Character5 = 1 << 4,
        Character6 = 1 << 5,
        Character7 = 1 << 6,
        Character8 = 1 << 7,
        Character9 = 1 << 8,
        Character10 = 1 << 9,
        Character11 = 1 << 10,
        Character12 = 1 << 11,
        Character13 = 1 << 12,
        Character14 = 1 << 13,
        Character15 = 1 << 14,
        Character16 = 1 << 15,
        Character17 = 1 << 16,
        Character18 = 1 << 17,
        Max = 1 << 18
    }
    private Character_Behavior _characterBehavior = Character_Behavior.None;
    private bool _checkCharacterBehavior = false;

    #region Action
    private int actionIndex = 0;
    private List<Action> stateAction = new List<Action>();
    #endregion

    #region Position
    [SerializeField] GameObject _loadPosition = null;
    #endregion


    private void Awake()
    {
        InitializeFSM();
        MessageAddListner();
    }

    #region InGame Flow
    private void BattleLoad()
    {
        StartCoroutine(WaitForNoneState(TRIGGER_FSM.Load));
    }
    private void BattleReady()
    {
        StartCoroutine(WaitForNoneState(TRIGGER_FSM.Intro));
    }
    private IEnumerator WaitForNoneState(TRIGGER_FSM state)
    {
        yield return new WaitUntil(() => fsm.State == TRIGGER_FSM.None);

        fsm.ChangeState(state);
    }
    private void BattleGame()
    {
        StartCoroutine(WaitForNoneState(TRIGGER_FSM.InitTurn));
    }
    private void BattleResult()
    {

    }
    #endregion

    #region Check Character Behavior
    
    private void AddCharacterBehavior()
    {
        Character_Behavior behavior = _characterBehavior + 1;
        _characterBehavior |= behavior;
    }
    private void RemoveCharacterBehavior()
    {
        for(int i = (int)Character_Behavior.Character1; i <= (int)_characterBehavior; i++)
        {
            if(_characterBehavior.HasFlag((Character_Behavior)i) == true)
            {
                _characterBehavior &= ~(Character_Behavior)i;
                break;
            }
        }

        if (_characterBehavior == Character_Behavior.None)
        {
            if(_checkCharacterBehavior == true)
            {
                InvokeAction();
            }
        }
    }

    #endregion

    #region Action

    private IEnumerator SetAction(TRIGGER_FSM _state, Action _action)
    {
        yield return new WaitUntil(() => currentState == TRIGGER_FSM.None);
        currentState = _state;

        _action.Invoke();
        InvokeAction();
    }
    private void InvokeAction()
    {
        if(stateAction.Count <= actionIndex)
        {
            return;
        }

        Action action = stateAction[actionIndex];
        actionIndex++;

        action.Invoke();
    }
    private void ResetAction()
    {
        currentState = TRIGGER_FSM.None;
        stateAction.Clear();
        actionIndex = 0;
    }

    #endregion

    #region FSM
    private void GoToState(TRIGGER_FSM _state)
    {
        fsm.ChangeState(_state);
    }
    
    private void InitializeFSM()
    {
        if (fsm == null)
            fsm = StateMachine<TRIGGER_FSM>.Initialize(this, TRIGGER_FSM.None);
        else
            fsm.ChangeState(TRIGGER_FSM.None);
    }
    private void FSMChangeNextState()
    {
        fsm.ChangeNextState();
    }
    #endregion


    private void MessageAddListner()
    {
        Messenger.AddListener(Definition.InGameLoad, BattleLoad);
        Messenger.AddListener(Definition.InGameReady, BattleReady);
        Messenger.AddListener(Definition.InGameGame, BattleGame);
        Messenger.AddListener(Definition.InGameResult, BattleResult);

        Messenger.AddListener(Definition.BattleManagerInvokeAction, InvokeAction);
        Messenger.AddListener(Definition.AddCharacterBehavior, AddCharacterBehavior);
        Messenger.AddListener(Definition.RemoveCharacterBehavior, RemoveCharacterBehavior);
        Messenger.AddListener<TRIGGER_FSM>(Definition.GoToState, GoToState);
    }
    private void MessageRemoveListner()
    {
        Messenger.RemoveListener(Definition.InGameLoad, BattleLoad);
        Messenger.RemoveListener(Definition.InGameReady, BattleReady);
        Messenger.RemoveListener(Definition.InGameGame, BattleGame);
        Messenger.RemoveListener(Definition.InGameResult, BattleResult);

        Messenger.RemoveListener(Definition.BattleManagerInvokeAction, InvokeAction);
        Messenger.RemoveListener(Definition.AddCharacterBehavior, AddCharacterBehavior);
        Messenger.RemoveListener(Definition.RemoveCharacterBehavior, RemoveCharacterBehavior);
        Messenger.RemoveListener<TRIGGER_FSM>(Definition.GoToState, GoToState);
    }

    private void OnDestroy()
    {
        MessageRemoveListner();
    }
}
