using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public partial class UiGame
{
    private void Result_Enter()
    {
        Messenger.Broadcast(Definition.CompleteState, Complete.UIGame);
        Invoke("OutInGame", 5f);
    }
    private void Result_Update()
    {

    }
}
