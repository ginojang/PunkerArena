using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public partial class Monster
{
    private void ActionStart_Enter()
    {
        DebugLog("Start");
        Messenger.Broadcast(Definition.AddCharacterBehavior);
        StartCoroutine(StartFSM());
    }
    private void ActionStart_Exit()
    {

    }

    private IEnumerator StartFSM()
    {
        yield return new WaitUntil(() => sub_Fsm != null);

        ChangeNextSubState();
        yield break;
    }
}
