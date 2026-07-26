using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CalculateSystem
{
    //private float maxrange;
    //private float maxcritical;
    //private float maxcriticaldmg;
    //private int minrandom;
    //private int maxrandom;

    //public CalculateSystem(int index)
    //{
    //    InitializeCalculate(index);
    //}

    //public void InitializeCalculate(int index)
    //{
    //    var data = CSVDataManager.GetTable<LimitValueTable>().GetData(index);

    //    maxrange = data.maxrange;
    //    maxcritical = data.maxcritical;
    //    maxcriticaldmg = data.maxcriticaldmg;
    //    minrandom = data.minrandom;
    //    maxrandom = data.maxrandom;
    //    maxcritical = data.maxcritical;
    //}

    //public float CasterDamage(StatusBase caster, SkillDamageType type, float skillDamageValue, float skillDamagePer)
    //{
    //    float totalDamage = 0;
    //    float casterDamage = 0;

    //    switch(type)
    //    {
    //        case SkillDamageType.Melee:
    //            casterDamage = caster.meleedamage;
    //            break;
    //        case SkillDamageType.Range:
    //            casterDamage = caster.rangedamage;
    //            break;
    //    }

    //    totalDamage = (caster.meleedamage * skillDamagePer) + skillDamageValue;

       

    //    return totalDamage;
    //}
    
    //public float TargetDefence(StatusBase defender)
    //{
    //    float totalDefence = 0;

    //    totalDefence = defender.defence;

    //    return totalDefence;
    //}
    //public bool CheckEvade(StatusBase caster, StatusBase target)
    //{
    //    float totalEvade = caster.attackrating - target.evade;
    //    bool evade = false;

    //    if (totalEvade >= maxrange)
    //    {
    //        totalEvade = maxrange;
    //    }
        
    //    float random = Random.Range(0, 100);
    //    if (random <= totalEvade)
    //        evade = false;
    //    else
    //        evade = true;



    //    return evade;
    //}
    //public bool CheckCritical(StatusBase caster)
    //{
    //    bool critical = false;
    //    float criticalValue = caster.critical;

    //    if (criticalValue > maxcritical)
    //        criticalValue = maxcritical;

    //    float random = Random.Range(0, 100);

    //    if (random <= criticalValue)
    //        critical = true;

    //    return critical;
    //}
    //public float TotalAttackDamage(CharacterStatus caster, CharacterStatus target, SkillDamageType type, float skillDamageValue, float skillDamagePer)
    //{
    //    float totalDamage = 0;
    //    float casterDmg = CasterDamage(caster, type, skillDamageValue, skillDamagePer);
    //    float targetDef = TargetDefence(target);
    //    //bool evade = CheckEvade(caster, target);
    //    //bool critical = CheckCritical(caster);

    //    //if (evade == true)
    //    //{

    //    //    return totalDamage = 1;
    //    //}

    //    //if(critical == true)
    //    //{

    //    //    casterDmg *= caster.criticaldamage;
    //    //}

    //    float randomDamage = Random.Range(minrandom, maxrandom);
    //    randomDamage *= 0.01f;
    //    totalDamage = casterDmg * randomDamage;

    //    return totalDamage - targetDef;
    //}
    //public float CriticalDamage(float dmg, float criticaldmg)
    //{
    //    if (criticaldmg > maxcriticaldmg)
    //        criticaldmg = maxcriticaldmg;

    //    dmg *= criticaldmg;

    //    return dmg;
    //}
}
