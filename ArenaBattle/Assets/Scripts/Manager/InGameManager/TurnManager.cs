using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;

public class TurnManager : MonoBehaviour
{
    private enum State
    {
        None,
        RoundProfile,
        Start,
        InsertTurn,
        Exit,
        Empty,
    }

    private int casterNextSlot = 0;
    private int targetNextSlot = 0;

   private Dictionary<int, CharacterBase[]> roundDic = null;
    private State currentState = State.None;

    private List<Action> actionState = new List<Action>();
    private int actionIndex = 0;


    private void Awake()
    {
        MessengerAddListner();
    }

    private void TurnLoad()
    {
        InitializeData();
        Messenger.Broadcast(Definition.CompleteState, Complete.TurnManager);
    }
    private void TurnReady()
    {
        InitRound();
        Messenger.Broadcast(Definition.CompleteState, Complete.TurnManager);
    }
    private void TurnGame()
    {

    }
    private void TurnResult()
    {

    }

    #region Initialize
    private void InitializeData()
    {
        roundDic = new Dictionary<int, CharacterBase[]>();
    }
    private void InitRound()
    {
        List<CharacterBase> list = new List<CharacterBase>();
        list.AddRange(InGameData.Instance.AllyList.Keys);
        list.AddRange(InGameData.Instance.EnemyList.Keys);

        var sort = list.OrderByDescending(x => x.CharacterInfo.CharacterStatus.spd).ToArray();
        for(int i = 0; i < 2; i++)
        {
            CharacterBase[] newRound = new CharacterBase[10];
            if (roundDic.ContainsKey(i+1) == false)
                roundDic.Add(i + 1, newRound);
        }

        int characterIndex = 0;
        foreach(var item in roundDic)
        {
            var charList = item.Value;
            for(int i = 0; i < charList.Length; i++)
            {
                if (sort.Length <= characterIndex)
                    break;

                charList[i] = sort[characterIndex];
                characterIndex++;
            }
        }
    }
    #endregion

    #region Temp Turn Draw

    private void SetPassNextTurn()
    {
        int turn_next = 5;
        var currentTurn = InGameData.Instance.CurrentTurn - 1;
        int result = currentTurn % 10;
        var round = InGameData.Instance.CurrentRound;

        casterNextSlot = (round * 10) + result + turn_next;

        int nextSlot = turn_next;
        if (nextSlot < 20)
        {
            CharacterProfile profile = InGameData.Instance.CurrentTurnCharacter.Profile;
            Messenger.Broadcast(Definition.SetNextTurnUI, nextSlot, profile);
        }
    }
    private void SetCasterNextTurn()
    {
        var data = InGameData.Instance.CurrentSkillData;
        var currentTurn = InGameData.Instance.CurrentTurn - 1;
        int result = currentTurn % 10;
        var round = InGameData.Instance.CurrentRound;

        casterNextSlot = (round * 10) + result + data.turn_next;

        int nextSlot = data.turn_next;
        if (nextSlot < 20)
        {
            CharacterProfile profile = InGameData.Instance.CurrentTurnCharacter.Profile;
            Messenger.Broadcast(Definition.SetNextTurnUI, nextSlot, profile);
        }
    }

    private void SetTargetNextTurn()
    {
        var target = InGameData.Instance.CurrentTargetCharacter;
        targetNextSlot = 0;

        foreach(var item in roundDic)
        {
            CharacterBase[] array = item.Value;
            if (array.Contains(target) == false) continue;
            else
            {
                int index = Array.IndexOf(array, target);
                targetNextSlot = index + ((item.Key - 1) * 10);
                break;
            }
        }

        int turn = InGameData.Instance.CurrentTurn - 1;
        int next = InGameData.Instance.CurrentSkillData.turn_control;
        int result = targetNextSlot - (next - turn) * -1;

        if(result < 20)
            Messenger.Broadcast(Definition.SetNextTurnUI, result, target.Profile);
    }
    #endregion

    #region Set Round Profile
    private void StartSetRoundProfile()
    {
        StartCoroutine(SetRoundProfile(State.RoundProfile));
    }
    private IEnumerator SetRoundProfile(State state)
    {
        yield return new WaitUntil(() => currentState == State.None);

        actionIndex = 0;
        currentState = state;

        if (actionState.Count > 0)
            actionState.Clear();

        actionState.Add(SetRound);
        actionState.Add(SetRoundProfileComplete);

        InvokeAction();
    }
    private void SetRound()
    {
        CharacterProfile[] characterProfile = InGameData.Instance.UIProfile;
        int turn = InGameData.Instance.CurrentTurn;
        int resultTurn = turn % 10;
        int resultRound = (turn / 10) + 1;

        CharacterBase[] outRound = null;
        roundDic.TryGetValue(resultRound, out outRound);

        for(int i = 0; i < characterProfile.Length; i++)
        {
            if (outRound.Length <= resultTurn)
            {
                outRound = null;
                resultRound++;
                if(roundDic.ContainsKey(resultRound) == false)
                {
                    outRound = new CharacterBase[10];
                    roundDic.Add(resultRound, outRound);
                    Debug.LogError($"{resultRound} Insert Round!!!! (Set Round)");
                }
                else
                    roundDic.TryGetValue(resultRound, out outRound);

                resultTurn = 0;
            }

            if (outRound[resultTurn] != null)
                characterProfile[i] = outRound[resultTurn].Profile;
            else
                characterProfile[i] = null;

            resultTurn++;
        }

        InvokeAction();
    }
    private void SetRoundProfileComplete()
    {
        actionState.Clear();
        actionIndex = 0;

        currentState = State.None;
        Messenger.Broadcast(Definition.BattleManagerInvokeAction);
    }
    #endregion

    #region MakeCurrentTurn
    private void StartMakeCurrentTurn()
    {
        StartCoroutine(CurrentTurn(State.Start));
    }

    private IEnumerator CurrentTurn(State state)
    {
        yield return new WaitUntil(() => currentState == State.None);
        actionIndex = 0;
        currentState = state;

        if(actionState.Count > 0)
            actionState.Clear();

        actionState.Add(MakeCurrentTurn);
        actionState.Add(MakeCurrentTurnComplete);

        InvokeAction();
    }
    private void MakeCurrentTurn()
    {
        CharacterBase[] characters = null;
        roundDic.TryGetValue(InGameData.Instance.CurrentRound, out characters);

        int turn = InGameData.Instance.CurrentTurn - 1;
        int result = turn % 10;

        InGameData.Instance.CurrentTurnCharacter = characters[result];
        InvokeAction();
    }
    private void MakeCurrentTurnComplete()
    {
        actionState.Clear();
        actionIndex = 0;

        currentState = State.None;

        Messenger.Broadcast(Definition.BattleManagerInvokeAction);
    }
    #endregion

    #region Insert Turn
    private void StartInsertNextTurn()
    {
        StartCoroutine(SetInsertNextTurn(State.InsertTurn));
    }
    private IEnumerator SetInsertNextTurn(State state)
    {
        yield return new WaitUntil(()=> currentState == State.None);
        currentState = state;
        actionIndex = 0;

        if (actionState.Count > 0)
            actionState.Clear();

        actionState.Add(InsertCaster);
        actionState.Add(InsertTargetTurn);
        actionState.Add(InsertNextTurnComplete);

        InvokeAction();
    }
    private void InsertCaster()
    {
        CharacterBase charbase = InGameData.Instance.CurrentTurnCharacter;
        if(charbase != null)
        {
            RemoveCharacterInTurn(charbase);

            int round = (casterNextSlot / 10);
            int turn = casterNextSlot % 10;

            CharacterBase[] outRound = null;
            if(roundDic.TryGetValue(round, out outRound) == false)
            {
                outRound = new CharacterBase[10];
                roundDic.Add(round, outRound);
                Debug.LogError($"{round} Insert Round!!!! (Insert Caster)");
            }

            if (outRound[turn] == null)
            {
                outRound[turn] = charbase;
            }
            else
            {
                List<CharacterBase> list = FindEmptyTurn(round, turn);
                list.Insert(0, charbase);
                int resultTurn = turn;
                int resultRound = round;

                for(int i = 0; i < list.Count; i++)
                {
                    if (outRound.Length <= resultTurn)
                    {
                        outRound = null;
                        resultRound++;
                        if(roundDic.TryGetValue(resultRound, out outRound) == false)
                        {
                            outRound = new CharacterBase[10];
                            roundDic.Add(resultRound, outRound);
                            Debug.LogError($"{round} Insert Round!!!! (Insert Caster)");
                        }

                        resultTurn = 0;
                    }
                    outRound[resultTurn] = list[i];
                    resultTurn++;
                }
            }
        }

        InvokeAction();
    }

    private void InsertTargetTurn()
    {
        CharacterBase charbase = InGameData.Instance.CurrentTargetCharacter;
        if(charbase != null)
        {
            RemoveCharacterInTurn(charbase);

            int round = (targetNextSlot / 10) + 1;
            int turn = targetNextSlot % 10;

            CharacterBase[] outRound = null;
            if (roundDic.TryGetValue(round, out outRound) == false)
            {
                outRound = new CharacterBase[10];
                roundDic.Add(round, outRound);
                Debug.LogError($"{round} Insert Round!!!! (Insert Target Turn)");
            }

            if (outRound[turn] == null)
            {
                outRound[turn] = charbase;
            }
            else
            {
                List<CharacterBase> list = FindEmptyTurn(round, turn);
                list.Insert(0, charbase);
                int resultTurn = turn;
                int resultRound = round;

                for (int i = 0; i < list.Count; i++)
                {
                    if (outRound.Length <= resultTurn)
                    {
                        outRound = null;
                        resultRound++;
                        if (roundDic.TryGetValue(resultRound, out outRound) == false)
                        {
                            outRound = new CharacterBase[10];
                            roundDic.Add(resultRound, outRound);
                            Debug.LogError($"{resultRound} Insert Round!!!! (Insert Caster Turn)");
                        }

                        resultTurn = 0;
                    }
                    outRound[resultTurn] = list[i];
                    resultTurn++;
                }
            }
        }
      
        InvokeAction();
    }
    private void InsertNextTurnComplete()
    {
        actionIndex = 0;
        actionState.Clear();
        currentState = State.None;

        Messenger.Broadcast(Definition.BattleManagerInvokeAction);
    }
    private void RemoveCharacterInTurn(CharacterBase character)
    {
        foreach (var rounds in roundDic)
        {
            CharacterBase[] characters = rounds.Value;

            if (characters.Contains(character))
            {
                int index = Array.IndexOf(characters, character);
                Array.Clear(characters, index, 1);
                break;
            }
        }
    }

    private List<CharacterBase> FindEmptyTurn(int round, int turn)
    {
        List<CharacterBase> list = new List<CharacterBase>();
        
        roundDic = roundDic.OrderBy(x => x.Key).ToDictionary(x=> x.Key, x=> x.Value);
        foreach(var item in roundDic)
        {
            if (item.Key < round)
                continue;

            CharacterBase[] array = item.Value;
            bool result = false;
            for(int i = turn; i < array.Length; i++)
            {
                if (array[i] == null)
                {
                    result = true;
                    break;
                }

                list.Add(array[i]);
            }

            if (result == true)
                break;

            turn = 0;
        }

        return list;
    }
    #endregion

    #region Exit Turn
    private void StartExitTurn()
    {
        StartCoroutine(ExitTurn(State.Exit));
    }
    private IEnumerator ExitTurn(State state)
    {
        yield return new WaitUntil(() => currentState == State.None);
        currentState = state;
        actionIndex = 0;

        if (actionState.Count > 0)
            actionState.Clear();

        actionState.Add(InitNextTurn);
        actionState.Add(ResetTurnData);
        actionState.Add(ExitTurnComplete);

        InvokeAction();
    }
    private void InitNextTurn()
    {
        InGameData.Instance.CurrentTurn++;
        int prevRound = InGameData.Instance.CurrentRound;

        int turn = InGameData.Instance.CurrentTurn - 1;
        int roundResult = (turn / 10) + 1;
        if(roundResult != InGameData.Instance.CurrentRound)
        {
            InGameData.Instance.CurrentRound = roundResult;
            InGameData.Instance.ChangeAttribute();
        }

        if(prevRound < InGameData.Instance.CurrentRound)
            roundDic.Remove(prevRound);

        InvokeAction();
    }
    private void ResetTurnData()
    {
        casterNextSlot = 0;
        targetNextSlot = 0;

        InvokeAction();
    }
    private void ExitTurnComplete()
    {
        actionIndex = 0;
        actionState.Clear();

        currentState = State.None;

        Messenger.Broadcast(Definition.BattleManagerInvokeAction);
    }
    #endregion

    #region Action
    private void InvokeAction()
    {
        if(actionState.Count <= actionIndex)
        {
            return;
        }

        var action = actionState[actionIndex];
        actionIndex++;
        action.Invoke();
    }
    #endregion

    private void MessengerAddListner()
    {
        Messenger.AddListener(Definition.InGameLoad, TurnLoad);
        Messenger.AddListener(Definition.InGameReady, TurnReady);
        Messenger.AddListener(Definition.InGameGame, TurnGame);
        Messenger.AddListener(Definition.InGameResult, TurnResult);

        Messenger.AddListener(Definition.SetCasterNextTurn, SetCasterNextTurn);
        Messenger.AddListener(Definition.SetTargetNextTurn, SetTargetNextTurn);
        Messenger.AddListener(Definition.ExitTurn, StartExitTurn);
        Messenger.AddListener(Definition.StartMakeCurrentTurn, StartMakeCurrentTurn);
        Messenger.AddListener(Definition.StartSetRoundProfile, StartSetRoundProfile);
        Messenger.AddListener(Definition.StartInsertNextTurn, StartInsertNextTurn);
        Messenger.AddListener(Definition.SetPassNextTurn, SetPassNextTurn);
    }
    private void MessengerRemoveListner()
    {
        Messenger.RemoveListener(Definition.InGameLoad, TurnLoad);
        Messenger.RemoveListener(Definition.InGameReady, TurnReady);
        Messenger.RemoveListener(Definition.InGameGame, TurnGame);
        Messenger.RemoveListener(Definition.InGameResult, TurnResult);

        Messenger.RemoveListener(Definition.SetCasterNextTurn, SetCasterNextTurn);
        Messenger.RemoveListener(Definition.SetTargetNextTurn, SetTargetNextTurn);
        Messenger.RemoveListener(Definition.ExitTurn, StartExitTurn);
        Messenger.RemoveListener(Definition.StartMakeCurrentTurn, StartMakeCurrentTurn);
        //Messenger.RemoveListener(Definition.StartEmptyTurn, StartEmptyTurn);
        Messenger.RemoveListener(Definition.StartSetRoundProfile, StartSetRoundProfile);
        Messenger.RemoveListener(Definition.StartInsertNextTurn, StartInsertNextTurn);
        Messenger.RemoveListener(Definition.SetPassNextTurn, SetPassNextTurn);
    }

    private void OnDestroy()
    {
        MessengerRemoveListner();
    }
}
