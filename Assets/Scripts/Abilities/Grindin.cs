using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grindin : Ability
{
    public float[] activationChance;
    public float[] lifePercentageToGive;

    public override void ApplyEffect()
    {
        base.ApplyEffect();

        AbilityManager am = GetComponentInParent<AbilityManager>();
        Player player = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();

        if (am != null)
        {
            if (am.GetBoolFromPercentage(activationChance[currentAbilityLevel]))
            {
                int healthFromEnemy = (int)am.GetPercentageFromValue(am.damageDealtToLastEnemy, lifePercentageToGive[currentAbilityLevel]);
                player.Heal(healthFromEnemy);

                Debug.Log("effect applied");
            }
        }
    }

    public void ApplyGrindin()
    {
        
    }
}
