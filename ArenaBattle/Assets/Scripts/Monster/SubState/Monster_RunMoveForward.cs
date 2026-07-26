using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public partial class Monster
{
    private void RunMoveForward_Enter()
    {
        StartCoroutine(StartRunForward());
    }
    private void RunMoveForward_Exit()
    {
        Get_AnimBase.Set_AttackAnimation(this, 0);
        Messenger.Broadcast(Definition.StartCheckTrigger);
    }
    private IEnumerator StartRunForward()
    {
        Transform target = InGameData.Instance.CurrentTargetCharacter.transform;
        var dist = transform.position.x - target.position.x;

        Vector3 targetPos = target.position;
        if (dist < 0)
            targetPos = target.position - (Vector3.right * 1.5f);
        else
            targetPos = target.position + (Vector3.right * 1.5f);

        float speed = 5;

        int runAni = InGameData.Instance.GetRunAnimIndex;
        characterAnimation.Set_Move(this, runAni);
        while (transform.position != targetPos)
        {
            transform.LookAt(targetPos);
            transform.position = Vector3.MoveTowards(transform.position, targetPos, speed * Time.deltaTime);

            yield return null;
        }

        characterAnimation.Set_Move(this, 0);

        transform.LookAt(target);
        ChangeNextSubState();
        yield break;
    }
}
