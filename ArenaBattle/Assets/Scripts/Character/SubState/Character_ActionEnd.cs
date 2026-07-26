using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

// ¸¶¹«¸®
public partial class Character
{
    private void ActionEnd_Enter()
    {
        SetLook();
        ActionEnd();
    }
    private void ActionEnd_Exit()
    {

    }
    private void ActionEnd()
    {
        Get_AnimBase.ClearEvent();
        DeastroyFSM();

        Messenger.Broadcast(Definition.RemoveCharacterBehavior);
    }
    private void SetLook()
    {
        transform.localRotation = Quaternion.identity;
    }
}
