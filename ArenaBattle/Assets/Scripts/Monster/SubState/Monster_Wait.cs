using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public partial class Monster
{
    private void Wait_Enter()
    {
        DebugLog("Wait");
        StartCoroutine(Wait());
    }
    private void Wait_Update()
    {

    }
    private void Wait_Exit()
    {

    }
    private IEnumerator Wait()
    {
        yield return new WaitForSeconds(2f);
        ChangeNextSubState();

        yield break;
    }
}
