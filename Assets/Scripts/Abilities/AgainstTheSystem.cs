using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AgainstTheSystem : Ability
{
    public float[] healthPercentageActivation;
    public float[] invencibilityTime;
    public float[] cooldownTime;
    public float[] addedKnockChance;

    float currentInvencibilityTime;
    float currentCooldown;
    bool cooldown;
    bool activateTimer;

    public override void ApplyEffect()
    {
        base.ApplyEffect();

        ApplyAgainstTheSystem();
    }

    public override void Update()
    {
        base.Update();

        if (activateTimer && !cooldown)
        {
            if (currentInvencibilityTime >= invencibilityTime[currentAbilityLevel])
            {
                activateTimer = false;
                cooldown = true;
                currentInvencibilityTime = 0f;
                Player player = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();
                player.knockChance -= addedKnockChance[currentAbilityLevel];
            }

            else
                currentInvencibilityTime += Time.deltaTime;
        }

        if (cooldown)
        {
            if (currentCooldown >= cooldownTime[currentAbilityLevel])
            {
                cooldown = false;
                activateTimer = false;
                currentCooldown = 0f;
            }

            else
                currentCooldown += Time.deltaTime;
        }
    }

    public void ApplyAgainstTheSystem()
    {
        AbilityManager am = GameObject.FindGameObjectWithTag("AbilityManager").GetComponent<AbilityManager>();
        Player player = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();

        int minimumHealth = (int)am.GetPercentageFromValue(player.baseHealth, healthPercentageActivation[currentAbilityLevel]);

        if (player.currentHealth > minimumHealth)
        {
            player.MakeDamage(am.lastProjectileHitPlayer.damage);
        }

        else
        {
            if (!activateTimer)
            {
                activateTimer = true;
                player.knockChance += addedKnockChance[currentAbilityLevel];
            }
            
            else if (cooldown)
            {
                player.MakeDamage(am.lastProjectileHitPlayer.damage);
                Debug.Log("abiltiy on cooldown, taking damage");
            }
        }
    }
}
