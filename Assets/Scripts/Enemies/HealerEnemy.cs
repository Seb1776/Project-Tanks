using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealerEnemy : Enemy
{
    [Header("----------")]
    [Header("Medic Unit")]
    public int healAmount;
    public float cooldownTime;
    public float healRadius;
    public bool cooldown;
    public LayerMask enemyMask;

    int currentUnitLimit;
    float currentCooldownTime;

    public override void Start()
    {
        base.Start();
    }

    public override void Update()
    {
        base.Update();
        HandleHealer();
    }

    void HandleHealer()
    {   
        if (!cooldown)
        {
            Collider2D[] nearEnemies = Physics2D.OverlapCircleAll(transform.position, healRadius, enemyMask);

            foreach (Collider2D e in nearEnemies)
            {
                if (e.transform.GetComponent<LivingThing>().currentHealth < e.transform.GetComponent<LivingThing>().startingHealth && e.transform.gameObject != this.gameObject)
                {
                    e.transform.GetComponent<LivingThing>().Heal(healAmount);
                    Debug.Log("healed " + e.transform.gameObject.name);
                    cooldown = true;
                }
            }
        }

        else
        {
            if (currentCooldownTime >= cooldownTime)
            {
                currentCooldownTime = 0f;
                cooldown = false;
            }

            else
                currentCooldownTime += Time.deltaTime;
        }
    }

    public override void OnDrawGizmosSelected()
    {
        base.OnDrawGizmosSelected();

        if (showDebugLimits)
        {
            Gizmos.color = Color.cyan;
            Gizmos.DrawWireSphere(transform.position, healRadius);
        }
    }
}
