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

                foreach (Flank f in player.playerFlanks)
                {
                    if (f.currentFireMode != Flank.FireMode.ShotgunBurst || f.currentFireMode != Flank.FireMode.AutoShotgunBurst)
                        f.AddAmmo(bulletsToGiveToPlayer);
                }
            }
        }
    }
}
