using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public partial class Monster
{
    private void RangeActionDirect_Enter()
    {
        rangeAction = DirectRange;

        int animation = InGameData.Instance.GetAttackAnimIndex;
        characterAnimation.Set_AttackAnimation(this, animation);
    }
    private void RangeActionDirect_Exit()
    {
        Get_AnimBase.Set_AttackAnimation(this, 0);
        Messenger.Broadcast(Definition.StartCheckTrigger);
    }

    private void DirectRange()
    {
        StartCoroutine(StartRange());
    }

    private IEnumerator StartRange()
    {
        var data = InGameData.Instance.CurrentSkillData;
        var enemyList = InGameData.Instance.TargetList;
        var effectData = EffectManager.Instance.GetData(data.res_fx_projectile);
        int count = data.multi_hit_count;
        float term = data.multi_hit_term;
        GameObject projectile = null;

        for (int i = 0; i < enemyList.Count; i++)
        {
            for (int j = 0; j < count; j++)
            {
                EffectManager.Instance.GetProjectileObject(effectData.idx, (obj) =>
                {
                    projectile = (GameObject)obj;
                    var parent = GetEffectParent(effectData.active_pos_type, effectData.link_bone_move);

                    projectile.transform.position = parent.transform.position;
                });

                yield return new WaitUntil(() => projectile != null);

                CharacterBase target = enemyList[i];
                Transform trans = enemyList[i].transform;

                transform.LookAt(trans);

                StartCoroutine(DirectMove(projectile, target, data.res_fx_hit));

                yield return new WaitForSeconds(term);
            }
        }
    }

    private IEnumerator DirectMove(GameObject _projectile, CharacterBase _target, string _effect)
    {
        float speed = 5f;
        Vector3 completePosition = _target.transform.position;

        _projectile.SetActive(true);
        while (_projectile.transform.position != completePosition)
        {
            _projectile.transform.LookAt(completePosition);

            _projectile.transform.position = Vector3.MoveTowards(_projectile.transform.position, completePosition, speed * Time.deltaTime);
            yield return null;
        }

        _target.OnHit();
        EffectManager.Instance.DestroyEffect(_projectile);
        yield break;
    }
}
