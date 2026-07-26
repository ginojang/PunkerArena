using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public partial class Monster
{
    private void RangeActionParabola_Enter()
    {
        rangeAction = ParabolaRange;

        int animation = InGameData.Instance.GetAttackAnimIndex;
        characterAnimation.Set_AttackAnimation(this, animation);
    }
    private void RangeActionParabola_Exit()
    {
        Get_AnimBase.Set_AttackAnimation(this, 0);
        Messenger.Broadcast(Definition.StartCheckTrigger);
    }

    private void ParabolaRange()
    {
        StartCoroutine(StartParabolaRange());
    }

    private IEnumerator StartParabolaRange()
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
                StartCoroutine(ParabolaMove(projectile, target));

                yield return new WaitForSeconds(term);
            }
        }
    }
    private IEnumerator ParabolaMove(GameObject _projectile, CharacterBase _target)
    {
        float timer = 0;
        Vector3 start = _projectile.transform.position;
        Vector3 end = _target.transform.position;

        _projectile.SetActive(true);
        while (_projectile.transform.position.y >= start.y)
        {
            _projectile.transform.LookAt(end);

            timer += Time.deltaTime;
            Vector3 temp = Parabola(start, end, 3, timer);
            _projectile.transform.position = temp;
            yield return new WaitForEndOfFrame();
        }

        _target.OnHit();
        EffectManager.Instance.DestroyEffect(_projectile);
        yield break;
    }

    private Vector3 Parabola(Vector3 start, Vector3 end, float height, float t)
    {
        Func<float, float> f = x => -4 * height * x * x + 4 * height * x;
        var mid = Vector3.Lerp(start, end, t);

        return new Vector3(mid.x, f(t) + Mathf.Lerp(start.y, end.y, t), mid.z);
    }
}
