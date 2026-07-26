using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public partial class UiGame
{
    private void Load_Enter()
    {
        SetUIGameObject();
        Messenger.Broadcast(Definition.CompleteState, Complete.UIGame);
    }
    private void Load_Update()
    {

    }
}
