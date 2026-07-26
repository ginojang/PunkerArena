using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public partial class Monster
{
    private void RunMoveBack_Enter()
    {
        StartCoroutine(StartMoveBack());
    }
    private void RunMoveBack_Exit()
    {

    }

    private IEnumerator StartMoveBack()
    {
        yield return new WaitForSeconds(0.5f);

        Vector3 target = transform.parent.position;
        float speed = 5;

        int runAni = InGameData.Instance.GetRunAnimIndex;
        characterAnimation.Set_Move(this, runAni);
        while (transform.position != target)
        {
            transform.LookAt(target);
            transform.position = Vector3.MoveTowards(transform.position, target, speed * Time.deltaTime);

            yield return null;
        }

        characterAnimation.Set_Move(this, 0);
        ChangeNextSubState();
    }
}
