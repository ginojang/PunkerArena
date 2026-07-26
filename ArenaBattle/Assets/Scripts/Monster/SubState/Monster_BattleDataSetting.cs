using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public partial class Monster
{
    private void BattleDataSetting_Enter()
    {
        if (characterInfo.costumeInfo == null)
            FindMonsterAttachEffect();
        else
            FindDinoAttachEffect();

        MessengerAddListner();
        ChangeNextSubState();
    }
    private void BattleDataSetting_Update()
    {

    }
    private void BattleDataSetting_Exit()
    {

    }
}
