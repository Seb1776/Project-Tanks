using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerEfficiency : Ability
{
    public float[] activationChance;
    public float[] percentageOfBulletsToGive;

    public override void ApplyEffect()
    {
        base.ApplyEffect();

        ApplyTriggerEfficiency();
    }

    public void ApplyTriggerEfficiency()
    {
        AbilityManager am = GetComponentInParent<AbilityManager>();
        Player player = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();

        if (am != null)
        {
            if (am.GetBoolFromPercentage(activationChance[currentAbilityLevel]))
            {
                int bulletsToGiveToPlayer = (int)am.GetPercentageFromValue(am.bulletsRecievedByLastEnemy, percentageOfBulletsToGive[currentAbilityLevel]);
                Debug.Log("Dealt " + am.bulletsRecievedByLastEnemy + " bullets and gave back " + bulletsToGiveToPlayer + " bullets.");
            }
        }
    }
}
