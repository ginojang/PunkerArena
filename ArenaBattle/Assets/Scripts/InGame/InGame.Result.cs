using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public partial class InGame
{
    private void Result_Enter()
    {
        CompleteState(Complete.InGame);
        InGameResult();
    }
    private void Result_Update()
    {

    }
}
