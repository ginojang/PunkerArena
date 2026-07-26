using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public partial class Monster
{
    private void ActionEnd_Enter()
    {
        DebugLog("ActionEnd");

        SetLook();


        ActionEnd();
    }
    private void ActionEnd_Exit()
    {

    }
    private void SetLook()
    {
        transform.localRotation = Quaternion.identity;
    }

    private void ActionEnd()
    {
        Get_AnimBase.ClearEvent();
        DeastroyFSM();

        Messenger.Broadcast(Definition.RemoveCharacterBehavior);
    }
}
