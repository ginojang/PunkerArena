using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public partial class Monster
{
    private void RunMoveCenter_Enter()
    {
        StartCoroutine(StartRunCenter());
    }
    private void RunMoveCenter_Exit()
    {

    }
    private IEnumerator StartRunCenter()
    {
        Vector3 target = Vector3.zero;
        float speed = 5;

        int runAni = InGameData.Instance.GetRunAnimIndex;
        characterAnimation.Set_Move(this, runAni);
        while (transform.position != target)
        {
            transform.LookAt(target);
            transform.position = Vector3.MoveTowards(transform.position, target, speed * Time.deltaTime);

            yield return null;
        }

        characterAnimation.Set_Move(this,0);
        ChangeNextSubState();
        yield break;
    }
}
