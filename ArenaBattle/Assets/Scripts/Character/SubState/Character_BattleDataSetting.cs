using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public partial class Character
{
    private void BattleDataSetting_Enter()
    {
        MessengerAddListner();
        FindDinoAttachEffect();
        ChangeNextSubState();
    }
    private void BattleDataSetting_Update()
    {

    }
    private void BattleDataSetting_Exit()
    {

    }
}
