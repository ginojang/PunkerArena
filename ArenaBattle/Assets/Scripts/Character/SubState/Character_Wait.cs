using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 시간 기준
public partial class Character
{
    private void Wait_Enter()
    {
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
