using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Necromancer : Ability
{
    public float[] activationPercentage;
    public float[] damageIncreasePercentage;
    public float affectedRadius;

    public override void ApplyEffect()
    {
        base.ApplyEffect();

        ApplyNecromancer();
    }

    void ApplyNecromancer()
    {
        AbilityManager am = GameObject.FindGameObjectWithTag("AbilityManager").GetComponent<AbilityManager>();
        
        Vector3 center = am.lastKilledEnemy.transform.position;

        if (am.GetBoolFromPercentage(activationPercentage[currentAbilityLevel]))
        {

        }
    }
}
