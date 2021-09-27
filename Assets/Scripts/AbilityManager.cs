using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AbilityManager : MonoBehaviour
{
    public List<Ability> allAbilities = new List<Ability>();
    public List<Ability> attributeModifierAbilities = new List<Ability>();
    public List<Ability> preventsOrAffectPlayerDamageAbilities = new List<Ability>();
    public List<Ability> activateOnEnemyDeathAbilities = new List<Ability>();
    public List<Ability> activateOnEnemyDamageAbilities = new List<Ability>();
    public string abilityToUpgradeDebug;
    [Header("Grindin' Attributes")]
    public int damageDealtToLastEnemy;
    [Header("Trigger Efficiency Attributes")]
    public int bulletsRecievedByLastEnemy;
    [Header("Blastiditis Attributes")]
    public Projectile lastProjectileHitPlayer;

    void Start()
    {
        
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.P))
            UpgradeEffect(abilityToUpgradeDebug);
    }

    public void UpgradeEffect(string _abilityName)
    {   
        for (int i = 0; i < allAbilities.Count; i++)
        {
            if (allAbilities[i].abilityName == _abilityName)
            {
                if (allAbilities[i].currentAbilityLevel < allAbilities[i].abilityLevels - 1)
                {
                    allAbilities[i].currentAbilityLevel++;
                    break;
                }
            }
        }
    }

    public float GetPercentageFromValue(int fullValue, float percentageToTake)
    {
        return fullValue * percentageToTake / 100f;
    }

    public bool GetBoolFromPercentage(float percentage)
    {
        percentage = percentage / 100f;

        if (Random.value <= percentage)
            return true;
        
        return false;
    }
}
