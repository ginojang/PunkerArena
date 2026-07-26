using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public partial class InGame
{
    private void Ready_Enter()
    {
        CompleteState(Complete.InGame);
        InGameReady();
    }
    private void Ready_Update()
    {

    }

}
