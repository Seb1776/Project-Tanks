using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Blastiditis : Ability
{
    public float[] minimumHealthPercentage;

    public override void ApplyEffect()
    {
        base.ApplyEffect();

        ApplyBlastiditis();
    }

    void ApplyBlastiditis()
    {
        Player player = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();
        AbilityManager am = GameObject.FindGameObjectWithTag("AbilityManager").GetComponent<AbilityManager>();

        int minimunHealth = (int)am.GetPercentageFromValue(player.baseHealth, minimumHealthPercentage[currentAbilityLevel]);
        Debug.Log("called blastiditis " + minimunHealth);

        if (currentAbilityLevel == 0 && 
           (am.lastProjectileHitPlayer.currentProjectile == Projectile.ProjectileType.Fire || 
           am.lastProjectileHitPlayer.currentProjectile == Projectile.ProjectileType.Explosive) || 
           currentAbilityLevel == 1 && 
           (am.lastProjectileHitPlayer.currentProjectile == Projectile.ProjectileType.Fire || 
           am.lastProjectileHitPlayer.currentProjectile == Projectile.ProjectileType.Explosive || 
           am.lastProjectileHitPlayer.currentProjectile == Projectile.ProjectileType.Piercing))
        {
            if (player.currentHealth > minimunHealth)
            {
                player.MakeDamage(am.lastProjectileHitPlayer.damage);
            }

            else
                Debug.Log("applied blastiditis");
        }
    }
}
