using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public partial class InGame
{
    private void Game_Enter()
    {
        CompleteState(Complete.InGame);
        InGameGame();
    }
    private void Game_Update()
    {

    }
}
