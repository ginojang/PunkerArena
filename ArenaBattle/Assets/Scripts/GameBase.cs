using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MonsterLove.StateMachine;

public enum E_GAME_FSM
{
    Nothing,
    Load,
    Ready,
    Game,
    Result,
}

public class GameBase : MonoBehaviour
{
    private StateMachine<E_GAME_FSM> fsm;

    // Start is called before the first frame update
    protected virtual void Start()
    {
        if (fsm == null)
        {
            fsm = StateMachine<E_GAME_FSM>.Initialize(this, E_GAME_FSM.Nothing);
            fsm.ChangeState(E_GAME_FSM.Load);
        }
        else
            fsm.ChangeState(E_GAME_FSM.Load);
    }

    // Update is called once per frame
    protected virtual void Update()
    {
        
    }

    protected virtual void OnDestroy()
    {
    }

    public virtual void ChangeGameSceneState(E_GAME_FSM state)
	{
        fsm.ChangeState(state);
    }
    public virtual void ChangeNextState()
    {
        var state = fsm.State;
        
        if(state == E_GAME_FSM.Result)
        {
            state = E_GAME_FSM.Result;
            return;
        }

        state++;
        fsm.ChangeState(state);
    }

    public void TestFunction()
	{
	}
}
